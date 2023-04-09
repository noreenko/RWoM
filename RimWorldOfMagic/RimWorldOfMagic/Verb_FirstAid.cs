﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_FirstAid : VFECore.Abilities.Verb_CastAbility
    {

        private int pwrVal = 0;
        private int verVal = 0;

        protected override bool TryCastShot()
        {
            Pawn caster = base.CasterPawn;

            pwrVal = TM_Calc.GetSkillPowerLevel(caster, ability.def as TMAbilityDef);
            verVal = TM_Calc.GetSkillVersatilityLevel(caster, ability.def as TMAbilityDef);

            if (caster == null) return false;

            IEnumerable<Hediff_Injury> injuriesToTend = caster.health.hediffSet.hediffs
                .OfType<Hediff_Injury>()
                .Where(injury => injury.CanHealNaturally() && injury.TendableNow())
                .Take(2 + pwrVal);
            foreach (Hediff_Injury injury in injuriesToTend)
            {
                injury.Tended(Rand.Range(0, 0.4f) + .1f * verVal, 1f);
            }
            return false;
        }
    }
}
