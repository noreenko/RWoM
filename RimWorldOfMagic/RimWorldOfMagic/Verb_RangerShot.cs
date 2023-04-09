﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;



namespace TorannMagic
{
    public class Verb_RangerShot : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            if ( this.CasterPawn.equipment.Primary !=null && this.CasterPawn.equipment.Primary.def.IsRangedWeapon)
            {
                Thing wpn = this.CasterPawn.equipment.Primary;
                if (TM_Calc.HasLoSFromTo(this.CasterPawn.Position, this.currentTarget.Cell, this.CasterPawn, 0, ability.def.verbProperties.range))
                {
                    if (TM_Calc.IsUsingBow(this.CasterPawn))
                    {
                        base.TryCastShot();
                        return true;
                    }
                    else
                    {
                        if (this.CasterPawn.IsColonist)
                        {
                            Messages.Message("MustHaveBow".Translate(
                            this.CasterPawn.LabelShort,
                            wpn.LabelShort
                            ), MessageTypeDefOf.NegativeEvent);
                        }
                        return false;
                    }                    
                }
            }
            else
            {
                Messages.Message("MustHaveRangedWeapon".Translate(
                    this.CasterPawn.LabelCap
                ), MessageTypeDefOf.RejectInput);
                return false;
            }
            return false;
        }
    }
}
