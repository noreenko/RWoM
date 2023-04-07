using Verse;
using System.Collections.Generic;
using RimWorld;

namespace TorannMagic
{    
    public class Effect_ShadowBolt : VFECore.Abilities.Verb_CastAbility
    {
        bool validTarg;

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
            {
                if ((root - targ.Cell).LengthHorizontal < verbProps.range)
                {
                    if (CasterIsPawn && CasterPawn.apparel != null)
                    {
                        List<Apparel> wornApparel = CasterPawn.apparel.WornApparel;
                        for (int i = 0; i < wornApparel.Count; i++)
                        {
                            if (!wornApparel[i].AllowVerbCast(this))
                            {
                                return false;
                            }
                        }
                        validTarg = true;
                    }
                    else
                    {
                        validTarg = true;
                    }                    
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
            if (t != LocalTargetInfo.Invalid && t.Cell != default)
            {
                Thing launchedThing = new Thing
                {
                    def = TorannMagicDefOf.FlyingObject_ShadowBolt
                };
                FlyingObject_ShadowBolt flyingObject = (FlyingObject_ShadowBolt)GenSpawn.Spawn(ThingDef.Named("FlyingObject_ShadowBolt"), CasterPawn.Position, CasterPawn.Map);
                flyingObject.Launch(CasterPawn, t.Cell, launchedThing);
            }

            return result;
        }
    }    
}
