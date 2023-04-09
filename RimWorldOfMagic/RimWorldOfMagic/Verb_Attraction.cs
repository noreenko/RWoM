using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_Attraction : Verb_SB
    {
        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            CompAbilityUserMagic comp = ability.Comp as CompAbilityUserMagic;
            float adjustedRadius = verbProps.defaultProjectile?.projectile?.explosionRadius ?? 1f;
            if (comp != null && comp.MagicData != null)
            {
                int pwrVal = TM_Calc.GetSkillPowerLevel(this.CasterPawn, ability.def as TMAbilityDef);
                adjustedRadius += (1.5f * pwrVal);
            }
            return adjustedRadius;
        }
    }
}
