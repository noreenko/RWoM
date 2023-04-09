﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_DismissCooler : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            Pawn caster = base.CasterPawn;
            Pawn pawn = this.currentTarget.Thing as Pawn;

            CompAbilityUserMagic comp = pawn.GetCompAbilityUserMagic();
            if(comp.IsMagicUser)
            {
                if(comp.summonedCoolers.Count > 0)
                {
                    Thing cooler = comp.summonedCoolers[0];
                    cooler.Destroy();
                }
                else
                {

                }
            }
            return true;
        }
    }
}
