using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;


namespace TorannMagic
{
    public class Verb_DispelLivingWall : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            bool flag = false;
            CompAbilityUserMagic comp = this.CasterPawn.GetCompAbilityUserMagic();

            if (comp.IsMagicUser)
            {
                if (comp.livingWall.Value != null && comp.livingWall.Value.Spawned)
                {
                    comp.livingWall.Value.Destroy(DestroyMode.Vanish);
                    comp.livingWall.Set(null);
                }
            }

            return false;
        }
    }
}
