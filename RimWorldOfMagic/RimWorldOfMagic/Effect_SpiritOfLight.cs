using Verse;

namespace TorannMagic;

public class Effect_SpiritOfLight : VFECore.Abilities.Verb_CastAbility
{
    protected override bool TryCastShot()
    {
        bool result = base.TryCastShot();
        CompAbilityUserMagic comp = (CompAbilityUserMagic)ability.Comp;
        if (comp.SoL != null)
        {
            comp.SoL.shouldDismiss = true;
        }
        else
        {
            LocalTargetInfo t = (LocalTargetInfo)ability.currentTargets[0];
            if (t != LocalTargetInfo.Invalid && t.Cell != default)
            {
                Thing sol = new Thing();
                sol.def = TorannMagicDefOf.FlyingObject_SpiritOfLight;

                LongEventHandler.QueueLongEvent(delegate
                {
                    FlyingObject_SpiritOfLight flyingObject = (FlyingObject_SpiritOfLight)GenSpawn.Spawn(ThingDef.Named("FlyingObject_SpiritOfLight"), CasterPawn.Position, CasterPawn.Map);
                    flyingObject.Launch(CasterPawn, t.Cell, sol);
                    flyingObject.shouldDismiss = false;
                    comp.SoL = flyingObject;
                }, "LaunchingFlyer", false, null);
            }
        }

        return result;
    }
}
