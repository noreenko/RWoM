using Verse;

namespace TorannMagic;

public class Effect_ValiantCharge : VFECore.Abilities.Verb_CastAbility
{
    bool validTarg;

    public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
    {
        if (targ.Thing != null && targ.Thing == caster)
        {
            return verbProps.targetParams.canTargetSelf;
        }
        if ( targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
        {
            if ((root - targ.Cell).LengthHorizontal < verbProps.range)
            {
                validTarg = TryFindShootLineFromTo(root, targ, out _);
            }
            else
            {
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
            float adjustedRadius = verbProps.defaultProjectile?.projectile?.explosionRadius ?? 1.2f;
            if (comp != null && comp.MagicData != null)
            {
                int verVal = TM_Calc.GetSkillVersatilityLevel(CasterPawn, ability.def as TMAbilityDef);
                adjustedRadius = 1.2f + (.8f * verVal);
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
            if (ModCheck.Validate.GiddyUp.Core_IsInitialized())
            {
                ModCheck.GiddyUp.ForceDismount(base.CasterPawn);
            }

            LongEventHandler.QueueLongEvent(delegate
            {
                FlyingObject_ValiantCharge flyingObject = (FlyingObject_ValiantCharge)GenSpawn.Spawn(ThingDef.Named("FlyingObject_ValiantCharge"), CasterPawn.Position, CasterPawn.Map);
                flyingObject.Launch(CasterPawn, t.Cell, CasterPawn);
            }, "LaunchingFlyer", false, null);
        }

        return result;
    }
}
