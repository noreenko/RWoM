using RimWorld;
using System;
using System.Linq;
using Verse;

using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;

namespace TorannMagic
{
    public class Verb_SpiritRally : Verb_SB 
    {

        int rallyBonus = 0;
        protected override bool TryCastShot()
        {
            Pawn casterPawn = CasterPawn;
            Map map = CasterPawn.Map;
            
            if(casterPawn.story != null && casterPawn.story.Adulthood != null && casterPawn.story.Adulthood.identifier == "tm_lost_spirit")
            {
                rallyBonus = 1;
            }
            CompAbilityUserMagic comp = casterPawn.GetCompAbilityUserMagic();
            int pwrVal = TM_Calc.GetSkillPowerLevel(casterPawn, ability.def as TMAbilityDef);
            int verVal = TM_Calc.GetSkillVersatilityLevel(casterPawn, ability.def as TMAbilityDef);            
            int radius = 8 + (2 * verVal) + rallyBonus;
            int maxCount = 7 + (2 * pwrVal) + rallyBonus;
            List<IntVec3> tmpList = GenRadial.RadialCellsAround(casterPawn.Position, radius, true).ToList();
            List<Corpse> corpseList = new List<Corpse>();
            if (tmpList.Count > 0)
            {
                foreach (IntVec3 c in tmpList)
                {
                    if (c.IsValid && c.InBoundsWithNullCheck(map) && c.Walkable(map))
                    {
                        foreach(Thing t in c.GetThingList(map))
                        {
                            if(t is Corpse)
                            {
                                corpseList.Add((Corpse)t);
                                if(corpseList.Count >= maxCount)
                                {
                                    goto ExitSearch;
                                }
                            }
                        }
                    }
                }
                ExitSearch:;
                Effecter effecter = TorannMagicDefOf.TM_SpiritPulseED.Spawn();
                effecter.Trigger(new TargetInfo(casterPawn.Position, map, false), new TargetInfo(casterPawn.Position, map, false));
                effecter.Cleanup();
                for (int i = 0; i < corpseList.Count; i++)
                {
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_Ghost, corpseList[i].DrawPos, map, Rand.Range(.6f, .8f), .1f, .05f, .05f, 0, Rand.Range(1, 2), 0, 0);
                    IntVec3 cell = corpseList[i].Position;

                    AbilityUser.SpawnThings tempPod = new SpawnThings();
                    tempPod.def = TorannMagicDefOf.TM_SpiritWolfR;
                    tempPod.kindDef = TorannMagicDefOf.TM_SpiritWolf;
                    tempPod.spawnCount = 1;
                    tempPod.temporary = true;

                    Thing newPawn = null;
                    int duration = Rand.Range(880, 920) + (180 * (verVal + rallyBonus));
                    newPawn = TM_Action.SingleSpawnLoop(casterPawn, tempPod, cell, map, duration, true, false, casterPawn.Faction, false);
                    Pawn animal = newPawn as Pawn;

                    HealthUtility.AdjustSeverity(animal, TorannMagicDefOf.TM_AntiMovement, 6f - ((1.5f * pwrVal) * comp.arcaneDmg) - rallyBonus);
                    HealthUtility.AdjustSeverity(animal, TorannMagicDefOf.TM_Manipulation, rallyBonus + (pwrVal * comp.arcaneDmg));

                    foreach(Pawn item in PawnUtility.SpawnedMasteredPawns(casterPawn))
                    {
                        if(item.caller != null)
                        {
                            item.caller.Notify_Released();
                        }
                        item.jobs.EndCurrentJob(Verse.AI.JobCondition.InterruptForced);
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        FleckMaker.ThrowSmoke(animal.DrawPos, map, Rand.Range(.5f, 1.1f));
                    }
                }
            }
            burstShotsLeft = 0;
            return false;
        }
    }
}
