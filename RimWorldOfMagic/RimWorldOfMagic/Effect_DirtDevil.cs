using Verse;

namespace TorannMagic
{    
    public class Effect_DirtDevil : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            bool result = base.TryCastShot();
            LocalTargetInfo t = (LocalTargetInfo)ability.currentTargets[0];
            if (t != LocalTargetInfo.Invalid && t.Cell != default)
            {
                Thing dirtDevil = new Thing
                {
                    def = TorannMagicDefOf.FlyingObject_DirtDevil
                };
                FlyingObject_DirtDevil flyingObject = (FlyingObject_DirtDevil)GenSpawn.Spawn(ThingDef.Named("FlyingObject_DirtDevil"), CasterPawn.Position, CasterPawn.Map);
                flyingObject.Launch(CasterPawn, t.Cell, dirtDevil);
            }

            return result;
        }
    }    
}
