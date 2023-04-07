using Verse;

namespace TorannMagic;

public class Effect_SpiritStorm : VFECore.Abilities.Verb_CastAbility
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

    public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
    {
        needLOSToCenter = true;
        TargetAoEProperties targetAoEProperties = UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties;
        if (targetAoEProperties == null || !targetAoEProperties.showRangeOnSelect)
        {
            CompAbilityUserMagic comp = CasterPawn.GetCompAbilityUserMagic();
            float adjustedRadius = verbProps.defaultProjectile?.projectile?.explosionRadius ?? 1f;
            if (comp != null && comp.MagicData != null)
            {
                int verVal = TM_Calc.GetSkillVersatilityLevel(CasterPawn, ability.def as TMAbilityDef);
                adjustedRadius += verVal;
            }
            return adjustedRadius;
        }
        return (float)targetAoEProperties.range;        
    }

    protected override bool TryCastShot()
    {
        bool result = base.TryCastShot();
        LocalTargetInfo t = (LocalTargetInfo)ability.currentTargets[0];
        if (t != LocalTargetInfo.Invalid && t.Cell != default)
        {                
            base.CasterPawn.rotationTracker.Face(t.CenterVector3);
            Thing ss = new Thing
            {
                def = TorannMagicDefOf.FlyingObject_SpiritStorm
            };
            FlyingObject_SpiritStorm flyingObject = (FlyingObject_SpiritStorm)GenSpawn.Spawn(ThingDef.Named("FlyingObject_SpiritStorm"), t.Cell, CasterPawn.Map);
            flyingObject.Launch(CasterPawn, t.Cell, ss);
        }

        return result;
    }
}
