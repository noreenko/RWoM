using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_Scorn : Verb_BLOS
    {
        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = false;
            CompAbilityUserMagic comp = ability.Comp as CompAbilityUserMagic;
            float adjustedRadius = verbProps.defaultProjectile?.projectile?.explosionRadius ?? 1f;
            if (comp != null && comp.MagicData != null)
            {
                int verVal = TM_Calc.GetSkillVersatilityLevel(this.CasterPawn, ability.def as TMAbilityDef);
                adjustedRadius += verVal;
            }
            return adjustedRadius;
        }
    }
}
