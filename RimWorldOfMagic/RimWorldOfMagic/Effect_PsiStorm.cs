using Verse;

namespace TorannMagic
{    
    public class Effect_PsiStorm : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            bool result = base.TryCastShot();
            LocalTargetInfo t = (LocalTargetInfo)ability.currentTargets[0];
            if (t != LocalTargetInfo.Invalid && t.Cell != default)
            {
                //Ability.PostAbilityAttempt();
                Thing psiOrb = new Thing
                {
                    def = TorannMagicDefOf.FlyingObject_PsiStorm
                };
                if (ModCheck.Validate.GiddyUp.Core_IsInitialized())
                {
                    ModCheck.GiddyUp.ForceDismount(base.CasterPawn);
                }

                LongEventHandler.QueueLongEvent(delegate
                {
                    FlyingObject_PsiStorm flyingObject = (FlyingObject_PsiStorm)GenSpawn.Spawn(ThingDef.Named("FlyingObject_PsiStorm"), CasterPawn.Position, CasterPawn.Map);
                    flyingObject.Launch(CasterPawn, t.Cell, psiOrb);
                }, "LaunchingFlyer", false, null);
            }

            return result;
        }
    }    
}
