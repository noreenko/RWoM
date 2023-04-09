using System;
using Verse;
using Verse.AI;




namespace TorannMagic
{
    public class Verb_Meditate : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            Pawn pawn = base.CasterPawn;

            pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            Job job = new Job(TorannMagicDefOf.JobDriver_TM_Meditate, CasterPawn.Position);
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            burstShotsLeft = 0;
            return false;
        }
    }
}
