using Verse;

using RimWorld;
using System.Linq;
using UnityEngine;

namespace TorannMagic
{    
    public class Effect_DragonStrike : VFECore.Abilities.Verb_CastAbility
    {
        bool validTarg;

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.Thing != null && targ.Thing == caster)
            {
                return verbProps.targetParams.canTargetSelf;
            }
            if (targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
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

        protected override bool TryCastShot()
        {
            bool result = base.TryCastShot();
            LocalTargetInfo t = currentTarget;
            Pawn casterPawn = base.CasterPawn;
            CompAbilityUserMight comp = (CompAbilityUserMight)ability.Comp;
            int pwrVal = TM_Calc.GetSkillPowerLevel(casterPawn, ability.def as TMAbilityDef, false);
            if (comp == null) return result;

            MightPowerSkill str = comp.MightData.MightPowerSkill_global_strength.FirstOrDefault(static mps => mps.label == "TM_global_strength_pwr");
            DamageInfo dinfo2 = new DamageInfo(TMDamageDefOf.DamageDefOf.TM_DragonStrike, Mathf.RoundToInt(Rand.Range(8f, 15f) * (1 + .1f * pwrVal + .05f * str.level)), 0, -1, CasterPawn);
            if (t.Cell != default)
            {
                //Ability.PostAbilityAttempt();
                if (ModCheck.Validate.GiddyUp.Core_IsInitialized())
                {
                    ModCheck.GiddyUp.ForceDismount(base.CasterPawn);
                }

                LongEventHandler.QueueLongEvent(delegate
                {
                    FlyingObject_DragonStrike flyingObject = (FlyingObject_DragonStrike)GenSpawn.Spawn(ThingDef.Named("FlyingObject_DragonStrike"), CasterPawn.Position, CasterPawn.Map);
                    flyingObject.Launch(CasterPawn, t, CasterPawn, dinfo2);
                }, "LaunchingFlyer", false, null);
            }

            return result;
        }
    }    
}
