using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;


namespace TorannMagic
{
    public class Verb_HarvestPassion : VFECore.Abilities.Verb_CastAbility
    {
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
            Pawn hitPawn = (Pawn)currentTarget;
            Pawn casterPawn = base.CasterPawn;

            bool flag = hitPawn != null && !hitPawn.Dead && !hitPawn.RaceProps.Animal && hitPawn.skills != null && hitPawn.health != null && hitPawn.health.hediffSet != null;
            if (flag && !TM_Calc.IsUndead(hitPawn))
            {
                if (casterPawn.Inspired)
                {
                    HealthUtility.AdjustSeverity(hitPawn, TorannMagicDefOf.TM_HarvestPassionHD, .5f);
                    hitPawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_HarvestPassionHD).TryGetComp<HediffComp_HarvestPassion>().caster = casterPawn;
                    casterPawn.mindState.inspirationHandler.EndInspiration(casterPawn.Inspiration);
                    if(!hitPawn.HostileTo(casterPawn.Faction))
                    {
                        
                    }
                }
                else
                {
                    Messages.Message("TM_MustHaveInspiration".Translate(
                    CasterPawn.LabelShort,
                    ability.def.label
                ), MessageTypeDefOf.RejectInput);
                }
            }
            else
            {
                Messages.Message("TM_InvalidTarget".Translate(
                    CasterPawn.LabelShort,
                    ability.def.label
                ), MessageTypeDefOf.RejectInput);
            }
            return false;
        }
    }
}
