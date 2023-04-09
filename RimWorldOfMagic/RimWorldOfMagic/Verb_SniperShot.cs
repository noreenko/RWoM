using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;



namespace TorannMagic
{
    public class Verb_SniperShot : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            if (TM_Calc.IsUsingRanged(CasterPawn))
            {
                Thing wpn = CasterPawn.equipment.Primary;
                ThingDef newProjectile = wpn.def.Verbs.FirstOrDefault().defaultProjectile;
                Type oldThingclass = newProjectile.thingClass;
                newProjectile.thingClass = Projectile.thingClass;
                SoundInfo info = SoundInfo.InMap(new TargetInfo(CasterPawn.Position, CasterPawn.Map));
                SoundDef.Named(wpn.def.Verbs.FirstOrDefault().soundCast.ToString()).PlayOneShot(info);
                bool? flag4 = TryLaunchProjectile(newProjectile, currentTarget);
                if (flag4 is not true)
                {
                    Ability.Notify_AbilityFailed(UseAbilityProps.refundsPointsAfterFailing);
                }
                newProjectile.thingClass = oldThingclass;
                burstShotsLeft = 0;
                return false;
            }
            Messages.Message("MustHaveRangedWeapon".Translate(
                CasterPawn.LabelCap
            ), MessageTypeDefOf.RejectInput);
            return false;
        }
    }
}
