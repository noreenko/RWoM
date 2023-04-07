using RimWorld;
using Verse;

namespace TorannMagic
{
    public class Verb_ThickSkin : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            Pawn pawn = this.currentTarget.Thing as Pawn;

            bool flag = pawn != null && !pawn.Dead;
            if (flag)
            {
                if(pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_HediffThickSkin))
                {
                    Hediff rec = pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_HediffThickSkin);
                    pawn.health.RemoveHediff(rec);
                }
                else
                {
                    HealthUtility.AdjustSeverity(pawn, TorannMagicDefOf.TM_HediffThickSkin, .5f );
                    FleckMaker.ThrowDustPuff(pawn.Position, pawn.Map, 1f);
                }
            }
            return true;
        }
    }
}
