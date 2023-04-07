using Verse;
using UnityEngine;
using System.Collections.Generic;
using RimWorld;

namespace TorannMagic
{
    class Verb_ChiBurst : VFECore.Abilities.Verb_CastAbility  
    {

        bool validTarg;
        private int pwrVal;

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
                    validTarg = TryFindShootLineFromTo(root, targ, out _);
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

        protected override bool TryCastShot()
        {
            Pawn casterPawn = CasterPawn;
            pwrVal = casterPawn.GetCompAbilityUserMight().MightData.MightPowerSkill_Chi.FirstOrDefault((MightPowerSkill x) => x.label == "TM_Chi_pwr").level;

            if (!casterPawn.IsColonist && ModOptions.Settings.Instance.AIHardMode)
            {
                pwrVal = 3;
            }

            Effecter SabotageEffect = TorannMagicDefOf.TM_ChiBurstED.Spawn();
            SabotageEffect.Trigger(new TargetInfo(currentTarget.Cell, casterPawn.Map), new TargetInfo(currentTarget.Cell, casterPawn.Map));
            SabotageEffect.Cleanup();

            List<Pawn> classPawns = GetMapClassPawnsAround(casterPawn.Map, currentTarget.Cell, ability.GetRangeForPawn());
            if(classPawns != null && classPawns.Count > 0)
            {
                for(int i =0; i < classPawns.Count; i++)
                {
                    if(classPawns[i] != casterPawn)
                    {
                        if (classPawns[i].health != null && classPawns[i].Faction != null && classPawns[i].health.hediffSet != null && classPawns[i].story != null && !classPawns[i].NonHumanlikeOrWildMan())
                        {
                            float successChance = TM_Calc.GetSpellSuccessChance(CasterPawn, classPawns[i], false);
                            if (Rand.Chance(successChance))
                            {
                                DisruptClassPawn(classPawns[i]);
                            }
                            else
                            {
                                MoteMaker.ThrowText(classPawns[i].DrawPos, classPawns[i].Map, "TM_ResistedSpell".Translate());
                            }
                        }                        
                    }
                }
            }
         
            return false;
        }

        public List<Pawn> GetMapClassPawnsAround(Map map, IntVec3 centerPos, float radius)
        {
            List<Pawn> classPawns = new List<Pawn>();
            List<Pawn> mapPawns = map.mapPawns.AllPawnsSpawned;
            for(int i =0; i < mapPawns.Count; i++)
            {
                if((mapPawns[i].Position - centerPos).LengthHorizontal <= radius)
                {
                    if(TM_Calc.IsMagicUser(mapPawns[i]) || TM_Calc.IsMightUser(mapPawns[i]))
                    {
                        classPawns.Add(mapPawns[i]);
                    }
                    else if(!mapPawns[i].DestroyedOrNull())
                    {
                        DisruptMentalState_NonClass(mapPawns[i]);
                    }
                }
            }
            return classPawns;
        }

        private void DisruptClassPawn(Pawn pawn)
        {
            Hediff classHediff = null;
            float energyBurn = 0;
            if (TM_Calc.IsMightUser(pawn))
            {
                CompAbilityUserMight mightComp = pawn.GetCompAbilityUserMight();
                classHediff = pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_ChiHD);
                if(mightComp != null && mightComp.Stamina != null)
                {
                    energyBurn = Mathf.Clamp(mightComp.Stamina.CurLevel, 0, (.5f * (1f + (.20f * pwrVal))));
                    TM_Action.DamageEntities(pawn, null, Mathf.RoundToInt(Rand.Range(30f, 50f) * energyBurn), TMDamageDefOf.DamageDefOf.TM_ChiBurn, CasterPawn);
                    mightComp.Stamina.CurLevel -= energyBurn;
                }
            }
            else if (TM_Calc.IsMagicUser(pawn))
            {
                CompAbilityUserMagic magicComp = pawn.GetCompAbilityUserMagic();
                classHediff = pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_BloodHD);
                if (magicComp != null && magicComp.Mana != null)
                {
                    energyBurn = Mathf.Clamp(magicComp.Mana.CurLevel, 0, (.5f * (1f + (.20f * pwrVal))));
                    TM_Action.DamageEntities(pawn, null, Mathf.RoundToInt(Rand.Range(30f, 50f) * energyBurn), TMDamageDefOf.DamageDefOf.TM_ChiBurn, CasterPawn);
                    magicComp.Mana.CurLevel -= energyBurn;
                }
            }
            TM_Action.DamageEntities(pawn, null, Mathf.RoundToInt(Rand.Range(20f, 30f) * energyBurn), DamageDefOf.Stun, CasterPawn);
            if (classHediff != null)
            {
                energyBurn = Mathf.Clamp(classHediff.Severity, 0, (.5f * (1f + (.20f * pwrVal))) * 100);
                classHediff.Severity -= energyBurn;
            }
        }

        private void DisruptMentalState_NonClass(Pawn pawn)
        {
            if (Rand.Chance(TM_Calc.GetSpellSuccessChance(CasterPawn, pawn)))
            {
                if (pawn.RaceProps.Humanlike && Rand.Chance(.08f))
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);
                }
                else if (pawn.RaceProps.Animal && Rand.Chance(.1f))
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
                }
                else if (Rand.Chance(.5f))
                {
                    TM_Action.DamageEntities(pawn, null, Rand.Range(4, 8), DamageDefOf.Stun, CasterPawn);
                }
            }
            else
            {
                MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "TM_ResistedSpell".Translate());
            }
        }
    }
}
