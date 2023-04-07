using RimWorld;
using Verse;

namespace TorannMagic
{
    public class Verb_Transmutate : VFECore.Abilities.Verb_CastAbility
    {

        private int verVal;
        private int pwrVal;

        bool validTarg;
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map))
            {
                if ((root - targ.Cell).LengthHorizontal < this.verbProps.range)
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
            bool flagRawResource = false;
            bool flagStuffItem = false;
            bool flagNoStuffItem = false;
            bool flagNutrition = false;
            bool flagCorpse = false;

            Thing transmutateThing = TM_Calc.GetTransmutableThingFromCell(this.currentTarget.Cell, this.CasterPawn, out flagRawResource, out flagStuffItem, out flagNoStuffItem, out flagNutrition, out flagCorpse, true);

            if(transmutateThing != null)
            {
                TM_Action.DoTransmutate(this.CasterPawn, transmutateThing, flagNoStuffItem, flagRawResource, flagStuffItem, flagNutrition, flagCorpse);
            }
            else
            {
                Messages.Message("TM_NoThingToTransmutate".Translate(
                    this.CasterPawn.LabelShort
                ), MessageTypeDefOf.RejectInput);
            }

            this.burstShotsLeft = 0;
            return false;
        }
    }
}
