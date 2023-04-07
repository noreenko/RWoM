using Verse;

using RimWorld;

namespace TorannMagic
{    
    public class Effect_PsionicStorm : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            bool result = base.TryCastShot();
            LocalTargetInfo t = (LocalTargetInfo)ability.currentTargets[0];
            if (t != LocalTargetInfo.Invalid && t.Cell != default)
            {
                //Ability.PostAbilityAttempt();
                if (ModCheck.Validate.GiddyUp.Core_IsInitialized())
                {
                    ModCheck.GiddyUp.ForceDismount(base.CasterPawn);
                }

                LongEventHandler.QueueLongEvent(delegate
                {
                    FlyingObject_PsionicStorm flyingObject = (FlyingObject_PsionicStorm)GenSpawn.Spawn(ThingDef.Named("FlyingObject_PsionicStorm"), CasterPawn.Position, CasterPawn.Map);
                    flyingObject.Launch(CasterPawn, t.Cell, CasterPawn, this);
                }, "LaunchingFlyer", false, null);
            }

            return result;
        }
    }
}
