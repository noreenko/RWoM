using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;


namespace TorannMagic
{
    public class Verb_SpiritPossession : Verb_SB
    {        

        protected override bool TryCastShot()
        {
            Pawn hitPawn = (Pawn)currentTarget;
            Pawn casterPawn = base.CasterPawn;

            if (hitPawn is { Dead: false, Spawned: true, story.traits: { }, jobs: { } } && hitPawn != casterPawn && !TM_Calc.IsPossessedByOrIsSpirit(hitPawn) && hitPawn.RaceProps != null && hitPawn.RaceProps.IsFlesh)
            {
                CompAbilityUserMagic targetComp = hitPawn.GetCompAbilityUserMagic();
                if (targetComp != null)
                {
                    TryLaunchProjectile(verbProps.defaultProjectile, hitPawn);
                }
                else
                {
                    Messages.Message("TM_InvalidTarget".Translate(
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
