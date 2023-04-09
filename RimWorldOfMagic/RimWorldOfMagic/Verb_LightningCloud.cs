using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_LightningCloud : VFECore.Abilities.Verb_CastAbility
    {
        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            CompAbilityUserMagic comp = ability.Comp as CompAbilityUserMagic;
            float adjustedRadius = verbProps.defaultProjectile?.projectile?.explosionRadius - 2f ?? 1f;
            if (comp != null && comp.MagicData != null)
            {
                int verVal = TM_Calc.GetSkillVersatilityLevel(CasterPawn, ability.def as TMAbilityDef);
                adjustedRadius += verVal;
            }
            return adjustedRadius;
        }
    }
}
