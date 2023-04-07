using RimWorld;
using System;
using Verse;
using AbilityUser;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TorannMagic
{
    class Verb_Recall : Verb_UseAbility
    {
        private CompAbilityUserMagic comp;

        protected override bool TryCastShot()
        {
            comp = CasterPawn.GetCompAbilityUserMagic();

            if (CasterPawn is { Downed: false } && comp is { recallSet: true })
            {
                TM_Action.DoRecall(CasterPawn, comp, false);
            }
            else
            {
                Log.Warning("failed to TryCastShot");
            }

            burstShotsLeft = 0;
            return false;
        }

       
    }
}
