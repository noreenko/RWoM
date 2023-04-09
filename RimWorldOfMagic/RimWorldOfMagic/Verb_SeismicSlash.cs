using Verse;

using UnityEngine;
using RimWorld;
using System.Linq;
using System.Collections.Generic;


namespace TorannMagic
{
    [StaticConstructorOnStartup]
    public class Verb_SeismicSlash : VFECore.Abilities.Verb_CastAbility
    {
        private static int verVal;
        private static int pwrVal;

        protected Vector3 origin;
        protected Vector3 destination;
        protected int ticksToImpact;

        private static readonly Color cleaveColor = new Color(160f, 160f, 160f);
        private static readonly Material bladeMat = MaterialPool.MatFrom("Spells/cleave", false);

        bool validTarg;
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.Thing != null && targ.Thing == caster)
            {
                return verbProps.targetParams.canTargetSelf;
            }
            if (targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
            {
                if ((root - targ.Cell).LengthHorizontal < verbProps.range)
                {
                    ShootLine shootLine;
                    validTarg = TryFindShootLineFromTo(root, targ, out shootLine);
                }
                else
                {
                    validTarg = false;
                }
            }
            else
            {
                validTarg = false;
            }
            return validTarg;
        }

        protected int StartingTicksToImpact
        {
            get
            {
                int num = Mathf.RoundToInt((origin - destination).magnitude);
                bool flag = num < 1;
                if (flag)
                {
                    num = 1;
                }
                return num;
            }
        }

        public virtual Vector3 ExactPosition
        {
            get
            {
                Vector3 b = (destination - origin) * (1f - (float)ticksToImpact / (float)StartingTicksToImpact);
                return origin + b + Vector3.up * 2;
            }
        }

        public virtual Quaternion ExactRotation
        {
            get
            {
                return Quaternion.LookRotation(destination - origin);
            }
        }

        public static int GetWeaponDmg(Pawn pawn)
        {
            CompAbilityUserMight comp = pawn.GetCompAbilityUserMight();
            verVal = TM_Calc.GetSkillVersatilityLevel(pawn, TorannMagicDefOf.TM_SeismicSlash);
            pwrVal = TM_Calc.GetSkillPowerLevel(pawn, TorannMagicDefOf.TM_SeismicSlash);
            int dmgNum = 0;
            ThingWithComps weaponComp = pawn.equipment.Primary;
            float weaponDPS = weaponComp.GetStatValue(StatDefOf.MeleeWeapon_AverageDPS, false) * .7f;
            float dmgMultiplier = weaponComp.GetStatValue(StatDefOf.MeleeWeapon_DamageMultiplier, false);
            float pawnDPS = pawn.GetStatValue(StatDefOf.MeleeDPS, false);
            float skillMultiplier = (.7f + (.07f * pwrVal));
            return Mathf.RoundToInt(skillMultiplier * dmgMultiplier * (pawnDPS + weaponDPS) * comp.mightPwr);
        }

        protected override bool TryCastShot()
        {
            CompAbilityUserMight comp = CasterPawn.GetCompAbilityUserMight();
            //MightPowerSkill ver = comp.MightData.MightPowerSkill_SeismicSlash.FirstOrDefault((MightPowerSkill x) => x.label == "TM_SeismicSlash_ver");
            pwrVal = TM_Calc.GetSkillPowerLevel(CasterPawn, ability.def as TMAbilityDef);
            CellRect cellRect = CellRect.CenteredOn(base.CasterPawn.Position, 1);
            Map map = base.CasterPawn.Map;
            cellRect.ClipInsideMap(map);

            IntVec3 centerCell = cellRect.CenterCell;
            origin = base.CasterPawn.Position.ToVector3();
            destination = currentTarget.Cell.ToVector3Shifted();
            ticksToImpact = Mathf.RoundToInt((origin - destination).magnitude);

            if (CasterPawn.equipment.Primary != null && !CasterPawn.equipment.Primary.def.IsRangedWeapon)
            {
                TMAbilityDef ad = (TMAbilityDef)ability.def;
                int dmgNum = Mathf.RoundToInt(comp.weaponDamage * ad.weaponDamageFactor * (1 + (.1f * pwrVal)));
                
                if (!CasterPawn.IsColonist && ModOptions.Settings.Instance.AIHardMode)
                {
                    dmgNum += 10;
                }

                Vector3 strikeVec = origin;
                DrawBlade(strikeVec, 0);
                for (int i = 0; i < StartingTicksToImpact; i++)
                {
                    strikeVec = ExactPosition;
                    Pawn victim = strikeVec.ToIntVec3().GetFirstPawn(map);
                    if (victim != null && victim.Faction != base.CasterPawn.Faction)
                    {
                        DrawStrike(strikeVec.ToIntVec3(), strikeVec, map);
                        damageEntities(victim, null, dmgNum, DamageDefOf.Cut);
                    }
                    float angle = (Quaternion.AngleAxis(90, Vector3.up) * TM_Calc.GetVector(CasterPawn.DrawPos, currentTarget.CenterVector3)).ToAngleFlat();
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_DirectionalDirt, strikeVec, CasterPawn.Map, .3f + (.08f * i), .05f, .15f, .38f, 0, 5f - (.2f * i), angle, angle);
                    if (i == 2)
                    {
                        TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_Cleave, strikeVec, CasterPawn.Map, .6f + (.05f * i), .05f, .04f + (.03f * i), .15f, -10000, 30, angle, angle);
                    }
                    for (int j = 0; j < 2+(2*verVal); j++)
                    {
                        IntVec3 searchCell = strikeVec.ToIntVec3() + GenAdj.AdjacentCells8WayRandomized()[j];
                        TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_DirectionalDirt, searchCell.ToVector3Shifted(), CasterPawn.Map, .1f + (.04f * i), .05f, .04f, .28f, 0, 4f - (.2f * i), angle, angle);
                        victim = searchCell.GetFirstPawn(map);
                        if (victim != null && victim.Faction != base.CasterPawn.Faction)
                        {
                            DrawStrike(searchCell, searchCell.ToVector3(), map);
                            damageEntities(victim, null, dmgNum, DamageDefOf.Cut);
                        }
                    }
                    ticksToImpact--;
                }
            }
            else
            {
                Messages.Message("MustHaveMeleeWeapon".Translate(
                    CasterPawn.LabelCap
                ), MessageTypeDefOf.RejectInput);
                return false;
            }

            burstShotsLeft = 0;
            return false;
        }

        private void DrawBlade(Vector3 center, float magnitude)
        {
            Graphics.DrawMesh(MeshPool.plane10, center, ExactRotation, Verb_SeismicSlash.bladeMat, 0);           
        }

        public void DrawStrike(IntVec3 center, Vector3 strikePos, Map map)
        {
            TM_MoteMaker.ThrowMultiStrike(strikePos, map, .5f);
            TM_MoteMaker.ThrowBloodSquirt(strikePos, map, 1.2f);
        }        

        public void damageEntities(Pawn victim, BodyPartRecord hitPart, int amt, DamageDef type)
        {
            DamageInfo dinfo;
            amt = (int)(amt * Rand.Range(.7f, 1.3f));
            dinfo = new DamageInfo(type, amt, 0, -1, CasterPawn, hitPart);
            dinfo.SetAllowDamagePropagation(false);
            victim.TakeDamage(dinfo);
        }
    }
}
