using System.Collections.Generic;
using RimWorld;
using Verse;


namespace TorannMagic
{
    public class Verb_BurningFury : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            Pawn pawn = currentTarget.Thing as Pawn;

            bool flag = pawn != null && !pawn.Dead;
            if (flag)
            {
                if(pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_BurningFuryHD))
                {
                    using (IEnumerator<Hediff> enumerator = pawn.health.hediffSet.hediffs.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Hediff rec = enumerator.Current;
                            if (rec.def == TorannMagicDefOf.TM_BurningFuryHD)
                            {
                                pawn.health.RemoveHediff(rec);
                            }
                        }
                    }
                }
                else
                {
                    HealthUtility.AdjustSeverity(pawn, TorannMagicDefOf.TM_BurningFuryHD, 1f);
                    FleckMaker.ThrowSmoke(pawn.DrawPos, pawn.Map, 1f);
                }
            }
            return true;
        }
    }
}
