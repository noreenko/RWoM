﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_DismissPowerNode : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            Pawn caster = base.CasterPawn;
            Pawn pawn = this.currentTarget.Thing as Pawn;

            CompAbilityUserMagic comp = pawn.GetCompAbilityUserMagic();
            if(comp.IsMagicUser)
            {
                if(comp.summonedPowerNodes.Count > 0)
                {
                    Thing powernode = comp.summonedPowerNodes[0];
                    powernode.Destroy();
                }
                else
                {

                }
            }
            return true;
        }
    }
}
