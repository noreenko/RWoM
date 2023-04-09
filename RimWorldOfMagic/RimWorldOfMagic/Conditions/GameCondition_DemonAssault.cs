﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AbilityUser;
using UnityEngine;
using Verse;
using RimWorld;

namespace TorannMagic.Conditions
{
    class GameCondition_DemonAssault : GameCondition
    {
        public IntVec2 centerLocation;
        public IntVec2 edgeLocation;
        List<IntVec3> summoningCircle = new List<IntVec3>();
        private int summoningDuration = 300;
        private int nextBlackLightning = 300;
        public LookTargets lookTarget;
        private int areaRadius = 4;
        bool initialized = false;
        bool disabled = false;
        public List<Pawn> spawnedThings;
        private int nextEventTick = 0;
        private int ticksBetweenEvents = 4000;
        IntVec3 rndTarg = default(IntVec3);
        private bool doEventAction = false;
        float wealth = 0;
        float wealthMultiplier = 1f;
        float storytellerThreat = 1f;
        int eventDifficulty = 1;
        int minPointsForSpawn = 75000;
        int eventSpawnPoints = 75000;

        public override void GameConditionTick()
        {
            base.GameConditionTick();  
            if(!initialized)
            {
                initialized = true;
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_SummoningCircle, summoningCircle[0].ToVector3Shifted(), SingleMap, 3f, 2f, 1f, 2f, 0, 0, 0, Rand.Range(0, 360));
            }
            if(summoningDuration > 0)
            {
                summoningDuration--;
                DoSummoningCircle();
                if(summoningDuration == 0)
                {
                    SpawnDemons();
                }
            }

            if(Find.TickManager.TicksGame % 60 == 0)
            {
                if (spawnedThings != null)
                {
                    bool anySpawnedThingsRemaining = false;
                    if (spawnedThings.Any(p => (!p.DestroyedOrNull() && !p.Dead && !p.Downed)))
                    {
                        anySpawnedThingsRemaining = true;
                    }
                    if (!anySpawnedThingsRemaining)
                    {
                        End();
                    }
                }

                if (nextEventTick <= Find.TickManager.TicksGame)
                {
                    nextEventTick = Find.TickManager.TicksGame + ticksBetweenEvents;                    
                }
            }

            if(doEventAction && Find.TickManager.TicksGame % 10 == 0 && Find.TickManager.TicksGame > nextEventTick)
            {
                nextEventTick = Find.TickManager.TicksGame + ticksBetweenEvents;
                DoEvent();
            }
        }

        private void DoEvent()
        {
            IntVec3 rndTarg = new IntVec3(Rand.Range(16, SingleMap.Size.x - 16), 0, Rand.Range(16, SingleMap.Size.z - 16));
            if (Rand.Chance(.1f * eventDifficulty))
            {
                rndTarg = FindEnemyPawnOrBuilding(rndTarg);
            }
            IntVec3 rndPos = rndTarg;
            int accuracy = 5 - eventDifficulty;
            rndPos.x += Rand.Range(-accuracy, accuracy);
            rndPos.z += Rand.Range(-accuracy, accuracy);
            if (rndPos.IsValid && rndPos.InBoundsWithNullCheck(SingleMap) && rndPos.DistanceToEdge(SingleMap) > 6)
            {
                if (eventDifficulty > 2 && Rand.Chance(eventDifficulty * .05f))
                {
                    SkyfallerMaker.SpawnSkyfaller(TorannMagicDefOf.TM_Firestorm_Large, rndPos, SingleMap);
                }
                else if (eventDifficulty > 1 && Rand.Chance(eventDifficulty * .1f))
                {
                    SkyfallerMaker.SpawnSkyfaller(TorannMagicDefOf.TM_Firestorm_Small, rndPos, SingleMap);
                }
                else
                {
                    SkyfallerMaker.SpawnSkyfaller(TorannMagicDefOf.TM_Firestorm_Tiny, rndPos, SingleMap);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<Pawn>(ref spawnedThings, "spawnedThings", LookMode.Reference);
            Scribe_Values.Look<bool>(ref initialized, "initialized", true, false);
            Scribe_Values.Look<IntVec2>(ref centerLocation, "centerLocation", default(IntVec2), false);
            Scribe_Values.Look<IntVec2>(ref edgeLocation, "edgeLocation", default(IntVec2), false);
            Scribe_Values.Look<bool>(ref disabled, "disabled", false, false);
            Scribe_Values.Look<int>(ref ticksBetweenEvents, "ticksBetweenEvents", 4000, false);
            Scribe_Values.Look<int>(ref nextEventTick, "nextEventTick", 0, false);
            Scribe_Values.Look<int>(ref summoningDuration, "summoningDuration", 300, false);
            Scribe_Collections.Look<IntVec3>(ref summoningCircle, "summoningCircle", LookMode.Value);
        }

        public override void Init()
        {           
            base.Init();
            disabled = false;
            FindGoodEdgeLocation();
            lookTarget = new LookTargets(centerLocation.ToIntVec3, SingleMap);
            CalculateDifficultyModifiers();
            SetEventParameters();
            summoningCircle = new List<IntVec3>();
            summoningCircle.Clear();
            summoningCircle = GenRadial.RadialCellsAround(centerLocation.ToIntVec3, 5, false).ToList();                  
            InitializeVolcanicWinter();
            if (eventDifficulty >= 2)
            {
                InitializeDeathSkies();
            }
        }

        private void InitializeVolcanicWinter()
        {
            GameConditionManager gameConditionManager = gameConditionManager;
            int duration = Mathf.RoundToInt(Duration);
            GameCondition cond = GameConditionMaker.MakeCondition(GameConditionDefOf.VolcanicWinter, duration);
            gameConditionManager.RegisterCondition(cond);
        }

        private void InitializeDeathSkies()
        {
            GameConditionManager gameConditionManager = gameConditionManager;
            int duration = Mathf.RoundToInt(Duration);
            GameCondition cond2 = GameConditionMaker.MakeCondition(TorannMagicDefOf.DarkClouds, duration);
            gameConditionManager.RegisterCondition(cond2);
        }

        private void SetEventParameters()
        {
            
            int pts = (int)(Rand.Range(.7f, .9f) * wealth * wealthMultiplier * storytellerThreat);
            pts += 50000 * eventDifficulty;
            pts = Mathf.Clamp(pts, eventSpawnPoints, 1250000);
            //Log.Message("demon assault has pts: " + pts);
            if(eventSpawnPoints <= pts)
            {
                eventSpawnPoints = pts;
            }
            if (eventDifficulty > 0)
            {
                doEventAction = true;
                nextEventTick = Find.TickManager.TicksGame + 200;
                ticksBetweenEvents = Mathf.RoundToInt((float)Duration / (110f * eventDifficulty));
            }
        }

        private void DoSummoningCircle()
        {                  
            if(Find.TickManager.TicksGame % 2 ==0)
            {
                IntVec3 randomCircleCell = summoningCircle.RandomElement();
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_Demon_Flame, randomCircleCell.ToVector3Shifted(), SingleMap, Rand.Range(.5f, .9f), Rand.Range(.2f, .3f), .05f, Rand.Range(.2f, .4f), Rand.Range(-400, 400), Rand.Range(.8f, 1.2f) * (randomCircleCell - centerLocation.ToIntVec3).LengthHorizontal, (Quaternion.AngleAxis(90, Vector3.up) * TM_Calc.GetVector(randomCircleCell.ToVector3Shifted(), centerLocation.ToIntVec3.ToVector3Shifted())).ToAngleFlat(), Rand.Range(0, 359));
            }

            if(nextBlackLightning > summoningDuration)
            {
                DoLightningStrike();
                nextBlackLightning = summoningDuration - Rand.Range(25, 50);
            }            
        }

        public void SpawnDemons()
        {
            int ptsForDemon = (int)(TorannMagicDefOf.TM_Demon.combatPower * 30);
            int ptsForLesserDemon = (int)(TorannMagicDefOf.TM_LesserDemon.combatPower * 30);
            minPointsForSpawn = ptsForLesserDemon;
            float chanceForDemon = .2f * eventDifficulty;
            spawnedThings = new List<Pawn>();

            Faction faction = Find.FactionManager.FirstFactionOfDef(TorannMagicDefOf.TM_SkeletalFaction);
            if (faction != null)
            {
                if (!faction.HostileTo(Faction.OfPlayer))
                {
                    faction.TryAffectGoodwillWith(Faction.OfPlayerSilentFail, -200, false, false);
                }
            }
            else
            {
                faction = Find.FactionManager.RandomEnemyFaction(true, true);
            }

            while (eventSpawnPoints >= minPointsForSpawn)
            {
                
                SpawnThings spawnables = new SpawnThings();
                if(Rand.Chance(chanceForDemon) && eventSpawnPoints >= ptsForDemon)
                {
                    spawnables.def = TorannMagicDefOf.TM_DemonR;
                    spawnables.kindDef = TorannMagicDefOf.TM_Demon;                                       
                }
                else
                {
                    spawnables.def = TorannMagicDefOf.TM_LesserDemonR;
                    spawnables.kindDef = TorannMagicDefOf.TM_LesserDemon;
                }
                spawnables.temporary = false;
                eventSpawnPoints -= (int)(spawnables.kindDef.combatPower * 30);

                spawnedThings.Add(TM_Action.SingleSpawnLoop(null, spawnables, summoningCircle.RandomElement(), SingleMap, 0, false, false, faction) as Pawn);
            }
        }

        public override void End()
        {
            for (int i = 0; i < spawnedThings.Count; i++)
            {
                Pawn p = spawnedThings[i];
                if (!p.DestroyedOrNull())
                {
                    if (p.Map != null)
                    {
                        p.Map.weatherManager.eventHandler.AddEvent(new TM_WeatherEvent_MeshFlash(p.Map, p.Position, TM_MatPool.redLightning));
                    }
                    p.Destroy();
                }
            }
            List<GameCondition> gcs = new List<GameCondition>();
            GameCondition gcClouds = null;
            GameCondition gcVW = null;
            gcs = SingleMap.GameConditionManager.ActiveConditions;
            for(int i = 0; i < gcs.Count; i++)
            {
                if(gcs[i].def == TorannMagicDefOf.DarkClouds)
                {
                    gcClouds = gcs[i];
                }
                else if(gcs[i].def == GameConditionDefOf.VolcanicWinter)
                {
                    gcVW = gcs[i];
                }
            }
            if(gcClouds != null)
            {
                gcClouds.End();
            }
            if(gcVW != null)
            {
                gcVW.End();
            }
            MagicMapComponent mmc = SingleMap.GetComponent<MagicMapComponent>();
            if(mmc != null)
            {
                mmc.allowAllIncidents = false;
            }
            base.End();
        }

        private IntVec3 FindEnemyPawnOrBuilding(IntVec3 initialPos)
        {
            List<Thing> list = new List<Thing>();
            list.Clear();
            list = (from x in SingleMap.listerThings.AllThings
                    where x.Spawned && x.Faction != null && x.Faction.HostileTo(spawnedThings.RandomElement().Faction)
                    select x).ToList<Thing>();
            if (list != null && list.Count > 0)
            {
                return list.RandomElement().Position;
            }
            return initialPos;
        }

        public void DoLightningStrike()
        {
            SingleMap.weatherManager.eventHandler.AddEvent(new TM_WeatherEvent_MeshFlash(SingleMap, summoningCircle.RandomElement(), TM_MatPool.redLightning));
        }

        private void FindGoodEdgeLocation()
        {
            bool centerLocFound = false;
            if (SingleMap.Size.x <= 32 || SingleMap.Size.z <= 32)
            {
                throw new Exception("Map too small for a demon assault");
            }
            for (int i = 0; i < 20; i++)
            {
                int xVar = 0;
                int zVar = 0;
                if (Rand.Chance(.5f)) 
                {
                    xVar = Rand.Range(8, base.SingleMap.Size.x - 8);
                    zVar = Rand.Chance(.5f) ? Rand.Range(8, 16) : Rand.Range(base.SingleMap.Size.z - 16, base.SingleMap.Size.z - 8);
                }
                else
                {
                    xVar = Rand.Chance(.5f) ? Rand.Range(8, 16) : Rand.Range(base.SingleMap.Size.x - 16, base.SingleMap.Size.x - 8);
                    zVar = Rand.Range(8, base.SingleMap.Size.z - 8);
                }
                edgeLocation = new IntVec2(xVar, zVar);
                if (IsGoodCenterLocation(edgeLocation))
                {
                    centerLocation = edgeLocation;
                    centerLocFound = true;
                    break;
                }
            }
            if(!centerLocFound)
            {
                FindGoodCenterLocation();
            }
        }

        private void FindGoodCenterLocation()
        {
            if (SingleMap.Size.x <= 16 || SingleMap.Size.z <= 16)
            {
                throw new Exception("Map too small for a demon assault");
            }
            for (int i = 0; i < 10; i++)
            {
                centerLocation = new IntVec2(Rand.Range(8, base.SingleMap.Size.x - 8), Rand.Range(8, base.SingleMap.Size.z - 8));
                if (IsGoodCenterLocation(centerLocation))
                {
                    break;
                }
            }
        }

        private bool IsGoodLocationForSpawn(IntVec3 loc)
        {
            return loc.InBoundsWithNullCheck(base.SingleMap) && !loc.Roofed(base.SingleMap) && loc.Standable(base.SingleMap) && loc.IsValid && !loc.Fogged(base.SingleMap) && loc.Walkable(base.SingleMap);
        }

        private bool IsGoodCenterLocation(IntVec2 loc)
        {
            int num = 0;
            int num2 = (int)(3.14159274f * (float)areaRadius * (float)areaRadius / 2f);
            foreach (IntVec3 current in GetPotentiallyAffectedCells(loc))
            {
                if (IsGoodLocationForSpawn(current))
                {
                    num++;
                }
                if (num >= num2)
                {
                    break;
                }
            }
            
            return num >= num2 && (IsGoodLocationForSpawn(loc.ToIntVec3));
        }

        [DebuggerHidden]
        private IEnumerable<IntVec3> GetPotentiallyAffectedCells(IntVec2 center)
        {
            for (int x = center.x - areaRadius; x <= center.x + areaRadius; x++)
            {
                for (int z = center.z - areaRadius; z <= center.z + areaRadius; z++)
                {
                    if ((center.x - x) * (center.x - x) + (center.z - z) * (center.z - z) <= areaRadius * areaRadius)
                    {
                        yield return new IntVec3(x, 0, z);
                    }
                }
            }
        }

        public void CalculateDifficultyModifiers()
        {
            wealthMultiplier = .7f;
            wealth = SingleMap.PlayerWealthForStoryteller;
            if (wealth > 20000)
            {
                wealthMultiplier = .8f;
            }
            if (wealth > 50000)
            {
                wealthMultiplier = 1f;
            }
            if (wealth > 100000)
            {
                wealthMultiplier = 1.25f;
            }
            if (wealth > 250000)
            {
                wealthMultiplier = 1.5f;
            }
            if (wealth > 500000)
            {
                wealthMultiplier = 2f;
            }
            storytellerThreat = Mathf.Max(Find.Storyteller.difficulty.threatScale, 1f);
            
            eventDifficulty = Mathf.RoundToInt(ModOptions.Settings.Instance.demonAssaultChallenge);
            //Log.Message("wealth: " + wealth + " w_mult: " + wealthMultiplier + " threat scale: " + storytellerThreat + " event difficulty " + eventDifficulty);
        }
    }
}
