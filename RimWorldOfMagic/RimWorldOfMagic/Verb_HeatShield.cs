﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using Verse.AI;
using UnityEngine;
using Verse.Sound;


namespace TorannMagic
{
    public class Verb_HeatShield : Verb_SB
    {
        protected override bool TryCastShot()
        {            
            bool flag = false;
            if (this.CasterPawn.Map != null)
            {
                IEnumerable<Pawn> pList = from obj in this.CasterPawn.Map.mapPawns.AllPawnsSpawned
                                           where (!obj.HostileTo(CasterPawn.Faction) && (obj.Position - currentTarget.Cell).LengthHorizontal <= ability.GetRangeForPawn())
                                           select obj;
                foreach (Pawn pawn in pList)
                {
                    if (pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_HeatShieldHD))
                    {
                        RemoveHediffs(pawn);
                    }
                    ApplyHeatShield(pawn);
                }
            }

            return false;
        }

        public void ApplyHeatShield(Pawn pawn)
        {
            HealthUtility.AdjustSeverity(pawn, TorannMagicDefOf.TM_HeatShieldHD, 1f);
            SoundInfo info = SoundInfo.InMap(new TargetInfo(pawn.Position, pawn.Map));
            info.pitchFactor = 1.3f;
            info.volumeFactor = .5f;
            TorannMagicDefOf.TM_FireWooshSD.PlayOneShot(info);
            FleckMaker.ThrowLightningGlow(pawn.DrawPos, pawn.Map, 1.5f);
            Effecter effecter = TorannMagicDefOf.TM_HeatShieldED.Spawn();
            effecter.def.offsetTowardsTarget = FloatRange.Zero;
            effecter.Trigger(new TargetInfo(pawn.Position, pawn.Map), new TargetInfo(pawn.Position, pawn.Map));
            effecter.Cleanup();
        }        

        private void RemoveHediffs(Pawn target)
        {
            Hediff hediff = target.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_HeatShieldHD);
            target.health.RemoveHediff(hediff);
        }

    }
}
