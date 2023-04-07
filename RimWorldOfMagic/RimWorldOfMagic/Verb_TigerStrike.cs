using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using System.Linq;


namespace TorannMagic
{
    [StaticConstructorOnStartup]
    public class Verb_TigerStrike : VFECore.Abilities.Verb_CastAbility
    {

        public int verVal = 0;
        public int pwrVal = 0;

        protected override bool TryCastShot()
        {
            bool continueAttack = true;
            if (this.CasterPawn.equipment.Primary == null)
            {
                Thing target = this.currentTarget.Thing;
                if (target != null && target is Pawn targetPawn && this.burstShotsLeft > 0)
                {
                    int dmgNum = this.GetAttackDmg(this.CasterPawn);
                    BodyPartRecord hitPart = null;
                    DamageDef dmgType = TMDamageDefOf.DamageDefOf.TM_TigerStrike;
                    if(verVal > 0 && Rand.Chance(.1f))
                    {
                        TM_Action.DamageEntities(target, null, 4, DamageDefOf.Stun, this.CasterPawn);
                    }
                    if(verVal > 1 && Rand.Chance(.4f))
                    {
                        if(TM_Calc.IsMagicUser(targetPawn))
                        {
                            CompAbilityUserMagic compMagic = targetPawn.GetCompAbilityUserMagic();
                            float manaDrain = Mathf.Clamp(compMagic.Mana.CurLevel, 0, .25f);
                            this.CasterPawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_ChiHD).Severity += (manaDrain * 100);
                            compMagic.Mana.CurLevel -= manaDrain;

                        }
                        else if(TM_Calc.IsMightUser(targetPawn))
                        {
                            CompAbilityUserMight compMight = targetPawn.GetCompAbilityUserMight();
                            float staminaDrain = Mathf.Clamp(compMight.Stamina.CurLevel, 0, .25f);
                            this.CasterPawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_ChiHD).Severity += (staminaDrain * 100);
                            compMight.Stamina.CurLevel -= staminaDrain;
                        }
                    }
                    if(verVal > 2 && Rand.Chance(.1f))
                    {
                        IEnumerable<BodyPartRecord> rangeOfParts = (targetPawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.BloodPumpingSource).Concat(
                            targetPawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.BloodFiltrationSource).Concat(
                                targetPawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.BreathingPathway).Concat(
                                    targetPawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.SightSource)))));

                        hitPart = rangeOfParts.RandomElement();
                        dmgNum = Mathf.RoundToInt(dmgNum * 1.4f);
                    }
                    //DamageInfo dinfo2 = new DamageInfo(TMDamageDefOf.DamageDefOf.TM_TigerStrike, dmgNum, 0, (float)-1, this.CasterPawn, null, null, DamageInfo.SourceCategory.ThingOrUnknown);
                    TM_Action.DamageEntities(target, hitPart, dmgNum, dmgType, this.CasterPawn);
                    Vector3 strikeEndVec = this.currentTarget.CenterVector3;
                    strikeEndVec.x += Rand.Range(-.2f, .2f);
                    strikeEndVec.z += Rand.Range(-.2f, .2f);
                    Vector3 strikeStartVec = this.CasterPawn.DrawPos;
                    strikeStartVec.z += Rand.Range(-.2f, .2f);
                    strikeStartVec.x += Rand.Range(-.2f, .2f);
                    Vector3 angle = TM_Calc.GetVector(strikeStartVec, strikeEndVec);
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_TigerStrike, strikeStartVec, this.CasterPawn.Map, .4f, .08f, .03f, .05f, 0, 8f, (Quaternion.AngleAxis(90, Vector3.up) * angle).ToAngleFlat(), (Quaternion.AngleAxis(90, Vector3.up) * angle).ToAngleFlat());
                    if(!target.DestroyedOrNull())
                    {
                        if(targetPawn.Downed || targetPawn.Dead)
                        {
                            continueAttack = false;
                        }
                    }
                    if(this.burstShotsLeft <= (10 - (4 + verVal)))
                    {
                        this.burstShotsLeft = 0;
                        continueAttack = false;
                    }
                }
            }
            else
            {
                Messages.Message("MustBeUnarmed".Translate(
                    this.CasterPawn.LabelCap,
                    this.verbProps.label
                ), MessageTypeDefOf.RejectInput);
                this.burstShotsLeft = 0;
                return false;
            }
            return continueAttack;

        }

        public int GetAttackDmg(Pawn pawn)
        {
            CompAbilityUserMight comp = pawn.GetCompAbilityUserMight();
            verVal = TM_Calc.GetSkillVersatilityLevel(pawn, ability.def as TMAbilityDef, false);
            pwrVal = TM_Calc.GetSkillPowerLevel(pawn, ability.def as TMAbilityDef, false);
            float pawnDPS = pawn.GetStatValue(StatDefOf.MeleeDPS, false);
            float skillMultiplier = (.8f + (.08f * pwrVal));
            return Mathf.RoundToInt(skillMultiplier * (pawnDPS) * comp.mightPwr * Rand.Range(.75f, 1.25f));
        }
    }
}
