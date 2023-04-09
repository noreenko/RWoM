using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using Verse.AI;


namespace TorannMagic
{
    public class Verb_SpiritDrain : Verb_SB
    {       

        protected override bool TryCastShot()
        {
            Pawn hitPawn = (Pawn)currentTarget;
            Pawn casterPawn = base.CasterPawn;

            if (hitPawn != null & !hitPawn.Dead && hitPawn != casterPawn && !TM_Calc.IsUndead(hitPawn) && !TM_Calc.IsRobotPawn(hitPawn) && !TM_Calc.IsGolem(hitPawn))
            {
                CompAbilityUserMagic compCaster = casterPawn.GetCompAbilityUserMagic();

                Job job = new Job(TorannMagicDefOf.JobDriver_SpiritDrain, hitPawn);
                casterPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                casterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                burstShotsLeft = 0;
                return false;
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
