using System;
using Verse;
using Verse.AI;




namespace TorannMagic
{
    public class Verb_PsionicBarrier : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            if(verbProps.targetParams.canTargetLocations)
            {
                Job job = new Job(TorannMagicDefOf.JobDriver_PsionicBarrier, currentTarget);
                CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
            else
            {
                Job job = new Job(TorannMagicDefOf.JobDriver_PsionicBarrier, CasterPawn.Position);
                CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }

            burstShotsLeft = 0;
            return false;
        }
    }
}
