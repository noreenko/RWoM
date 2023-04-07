using Verse;

namespace TorannMagic
{    
    public class Effect_Flight : VFECore.Abilities.Verb_CastAbility
    {
        bool validTarg;

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
            {
                if ((root - targ.Cell).LengthHorizontal < verbProps.range)
                {
                    validTarg = TryFindShootLineFromTo(root, targ, out _);
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
            bool result = base.TryCastShot();
            LocalTargetInfo t = (LocalTargetInfo)ability.currentTargets[0];
            if (t == LocalTargetInfo.Invalid || t.Cell == default) return result;
            if (ModCheck.Validate.GiddyUp.Core_IsInitialized())
            {
                ModCheck.GiddyUp.ForceDismount(base.CasterPawn);
            }
            base.CasterPawn.rotationTracker.Face(t.CenterVector3);
            LongEventHandler.QueueLongEvent(delegate
            {
                FlyingObject_Flight flyingObject = (FlyingObject_Flight)GenSpawn.Spawn(ThingDef.Named("FlyingObject_Flight"), CasterPawn.Position, CasterPawn.Map);
                flyingObject.Launch(CasterPawn, t.Cell, base.CasterPawn);
            }, "LaunchingFlyer", false, null);
            return result;
        }
    }    
}
