using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;


namespace TorannMagic
{
    public class Verb_Regenerate : VFECore.Abilities.Verb_CastAbility
    {
        bool validTarg;
        private int verVal;
        private int pwrVal;
        private float arcaneDmg = 1;
        //Used specifically for non-unique verbs that ignore LOS (can be used with shield belt)
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ != null && targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
            {
                if ((root - targ.Cell).LengthHorizontal < verbProps.range)
                {
                    validTarg = true;
                }
                else
                {
                    //out of range
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
            bool flag = false;

            Map map = CasterPawn.Map;

            Pawn hitPawn = currentTarget.Thing as Pawn;
            Pawn caster = CasterPawn;

            try
            {
                verVal = TM_Calc.GetSkillVersatilityLevel(caster, ability.def as TMAbilityDef);
                pwrVal = TM_Calc.GetSkillPowerLevel(caster, ability.def as TMAbilityDef);
            }
            catch(NullReferenceException ex)
            {
                //ex
            }
            try
            {
                if (!hitPawn.DestroyedOrNull() & !hitPawn.Dead && hitPawn.Spawned && map != null)
                {
                    if (pwrVal == 3)
                    {
                        HealthUtility.AdjustSeverity(hitPawn, TorannMagicDefOf.TM_Regeneration_III, Rand.Range(1f + verVal, 3f + (verVal * 3)) * arcaneDmg);
                        TM_MoteMaker.ThrowRegenMote(hitPawn.DrawPos, map, 1f + (.2f * (verVal + pwrVal)));
                    }
                    else if (pwrVal == 2)
                    {
                        HealthUtility.AdjustSeverity(hitPawn, TorannMagicDefOf.TM_Regeneration_II, Rand.Range(1f + verVal, 3f + (verVal * 3)) * arcaneDmg);
                        TM_MoteMaker.ThrowRegenMote(hitPawn.DrawPos, map, 1f + (.2f * (verVal + pwrVal)));
                    }
                    else if (pwrVal == 1)
                    {
                        HealthUtility.AdjustSeverity(hitPawn, TorannMagicDefOf.TM_Regeneration_I, Rand.Range(1f + verVal, 3f + (verVal * 3)) * arcaneDmg);
                        TM_MoteMaker.ThrowRegenMote(hitPawn.DrawPos, map, 1f + (.2f * (verVal + pwrVal)));
                    }
                    else
                    {
                        HealthUtility.AdjustSeverity(hitPawn, TorannMagicDefOf.TM_Regeneration, Rand.Range(1f + verVal, 3f + (verVal * 3)) * arcaneDmg);
                        TM_MoteMaker.ThrowRegenMote(hitPawn.DrawPos, map, 1f + (.2f * (verVal + pwrVal)));
                    }
                }
                else
                {
                    Messages.Message("TM_NothingToRegenerate".Translate(), MessageTypeDefOf.NeutralEvent);
                }
            }
            catch (NullReferenceException ex)
            {
                //ex
            }
            return false;
        }
    }
}
