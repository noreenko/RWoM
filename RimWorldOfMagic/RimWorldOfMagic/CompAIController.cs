﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace TorannMagic
{
    [Serializable]
    public class CompAIController : ThingComp
	{
        private bool initialized;

        List<Pawn> closeThreats = new();
        List<Pawn> farThreats = new();
        List<Pawn> meleeThreats = new();
        List<Building> buildingThreats = new();

        public int nextRangedAttack;
        public int nextAoEAttack;
        public int nextKnockbackAttack;
        public int nextChargeAttack;
        public int nextTaunt;

        private int rangedBurstShots;
        private int rangedNextBurst;
        private float meleeRange = 1.4f;
        private LocalTargetInfo rangedTarget = null;

        private int age = -1;
        private bool deathOnce;

        //private int actionReady;
        //private int actionTick;

        //private LocalTargetInfo universalTarget = null;

        private int NextRangedAttack => Props.rangedCooldownTicks > 0 ? nextRangedAttack : Find.TickManager.TicksGame;

        private void StartRangedAttack()
        {
            nextRangedAttack = Props.rangedCooldownTicks + Find.TickManager.TicksGame;
            rangedBurstShots = Props.rangedBurstCount;
            rangedNextBurst = Find.TickManager.TicksGame + Props.rangedTicksBetweenBursts;
            nextChargeAttack = Find.TickManager.TicksGame + 150;
        }

        private void DoRangedAttack(LocalTargetInfo target)
        {
            if (target.Cell == default) return;

            Thing launchedThing = new Thing
            {
                def = ThingDef.Named("FlyingObject_DemonBolt")
            };
            FlyingObject_Advanced flyingObject = (FlyingObject_Advanced)GenSpawn.Spawn(ThingDef.Named("FlyingObject_DemonBolt"), Pawn.Position, Pawn.Map);
            flyingObject.AdvancedLaunch(Pawn, TorannMagicDefOf.Mote_Demon_Flame, 2, Rand.Range(5, 60), false, Pawn.DrawPos, target.Cell, launchedThing, Rand.Range(32, 38), true, Rand.Range(18, 26), Rand.Range(1.4f, 2.4f), DamageDefOf.Burn);
        }

        private int NextAoEAttack => Props.aoeCooldownTicks > 0 ? nextAoEAttack : Find.TickManager.TicksGame;

        private void DoAoEAttack(IntVec3 center, bool isExplosion, float radius, DamageDef damageType, int damageAmount)
        {
            nextAoEAttack = Props.aoeCooldownTicks + Find.TickManager.TicksGame;
            foreach (IntVec3 curCell in GenRadial.RadialCellsAround(center, radius, false))
            {
                if (curCell.IsValid && curCell.InBoundsWithNullCheck(Pawn.Map))
                {
                    if(isExplosion)
                    {
                        GenExplosion.DoExplosion(curCell, Pawn.Map, .4f, damageType, Pawn, damageAmount, Rand.Range(0, damageAmount), TorannMagicDefOf.TM_SoftExplosion, null, null, null, null, 0f, 1, null, false, null, 0f, 0);
                    }
                    else
                    {
                        List<Thing> thingList = curCell.GetThingList(Pawn.Map);
                        for(int j = 0; j < thingList.Count; j++)
                        {
                            DamageEntities(thingList[j], damageAmount, damageType, Pawn);
                        }
                    }
                }
            }
        }

        private int NextKnockbackAttack => Props.knockbackCooldownTicks > 0 ? nextKnockbackAttack : Find.TickManager.TicksGame;

        private void DoKnockbackAttack(IntVec3 center, IntVec3 target, float radius, float force)
        {
            nextKnockbackAttack = Props.knockbackCooldownTicks + Find.TickManager.TicksGame;
            foreach (IntVec3 curCell in GenRadial.RadialCellsAround(target, radius, true))
            {
                if (curCell.IsValid && curCell.InBoundsWithNullCheck(Pawn.Map))
                {
                    Vector3 launchVector = GetVector(Pawn.Position, curCell);
                    Pawn knockbackPawn = curCell.GetFirstPawn(Pawn.Map);
                    TM_MoteMaker.ThrowGenericFleck(FleckDefOf.Smoke, Pawn.DrawPos, Pawn.Map, Rand.Range(.6f, 1f), .01f, .01f, 1f, Rand.Range(50, 100), Rand.Range(5, 7), launchVector.ToAngleFlat(), Rand.Range(0, 360));
                    TM_MoteMaker.ThrowGenericFleck(FleckDefOf.Smoke, curCell.ToVector3Shifted(), Pawn.Map, Rand.Range(.6f, 1f), .01f, .01f, 1f, Rand.Range(50, 100), Rand.Range(5, 7), launchVector.ToAngleFlat(), Rand.Range(0, 360));
                    if (knockbackPawn != null && knockbackPawn != Pawn)
                    {
                        IntVec3 targetCell = knockbackPawn.Position + (force * force * launchVector).ToIntVec3();
                        if (targetCell != default)
                        {
                            if (knockbackPawn.Spawned && knockbackPawn.Map != null && !knockbackPawn.Dead)
                            {
                               FlyingObject_Spinning flyingObject = (FlyingObject_Spinning)GenSpawn.Spawn(ThingDef.Named("FlyingObject_Spinning"), knockbackPawn.Position, knockbackPawn.Map);
                                flyingObject.speed = 15 + (2*force);
                                flyingObject.Launch(Pawn, targetCell, knockbackPawn);
                            }
                        }
                    }
                }
            }
        }

        private int NextChargeAttack => Props.chargeCooldownTicks > 0 ? nextChargeAttack : Find.TickManager.TicksGame;

        private void DoChargeAttack(LocalTargetInfo t)
        {
            nextChargeAttack = Props.chargeCooldownTicks + Find.TickManager.TicksGame;
            if (t.Cell != default && t.Cell.DistanceToEdge(Pawn.Map) > 6)
            {
                Pawn.rotationTracker.Face(t.CenterVector3);
                FlyingObject_DemonFlight flyingObject = (FlyingObject_DemonFlight)GenSpawn.Spawn(ThingDef.Named("FlyingObject_DemonFlight"), Pawn.Position, Pawn.Map);
                flyingObject.Launch(Pawn, t.Cell, Pawn);

            }
        }

        private int NextTaunt => Props.chargeCooldownTicks > 0 ? nextTaunt : Find.TickManager.TicksGame;

        private void DoTaunt(Map map)
        {
            nextTaunt = Props.tauntCooldownTicks + Find.TickManager.TicksGame;
            List<Pawn> threatPawns = map?.mapPawns.AllPawnsSpawned;
            if (threatPawns is not { Count: > 0 }) return;

            bool anyPawnsTaunted = false;
            for (int i = 0; i < threatPawns.Count; i++)
            {
                if (threatPawns[i].Faction != null && Pawn.Faction != null && threatPawns[i].Faction.HostileTo(Pawn.Faction))
                {
                    if (threatPawns[i].jobs != null && threatPawns[i].CurJob != null && threatPawns[i].CurJob.targetA != null && threatPawns[i].CurJob.targetA.Thing != null && threatPawns[i].CurJob.targetA.Thing != Pawn)
                    {
                        if (Rand.Chance(Props.tauntChance) && (threatPawns[i].Position - Pawn.Position).LengthHorizontal < 60)
                        {
                            //Log.Message("taunting " + threatPawns[i].LabelShort + " doing job " + threatPawns[i].CurJobDef.defName + " with follow radius of " + threatPawns[i].CurJob.followRadius);
                            if(threatPawns[i].CurJobDef == JobDefOf.Follow || threatPawns[i].CurJobDef == JobDefOf.FollowClose)
                            {
                                Job job = new Job(JobDefOf.AttackMelee, Pawn);
                                threatPawns[i].jobs.TryTakeOrderedJob(job, JobTag.Misc);
                            }
                            else
                            {
                                Job job = new Job(threatPawns[i].CurJobDef, Pawn);
                                threatPawns[i].jobs.TryTakeOrderedJob(job, JobTag.Misc);
                            }
                            anyPawnsTaunted = true;
                            //Log.Message("taunting " + threatPawns[i].LabelShort);
                        }
                    }
                }
            }
            if (anyPawnsTaunted)
            {
                MoteMaker.ThrowText(Pawn.DrawPos, Pawn.Map, "TM_Taunting".Translate());
            }
        }

        private Pawn Pawn
        {
            get
            {
                Pawn pawn = parent as Pawn;
                if (pawn == null)
                {
                    Log.Error("pawn is null");
                }
                return pawn;
            }
        }

        public CompProperties_AIController Props => (CompProperties_AIController)props;

        public override void CompTick()
        {
            if (age > 0)
            {
                if (!initialized)
                {
                    if (Props.alwaysManhunter || Pawn.Faction != Faction.OfPlayer)
                    {
                        Pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true);
                    }
                    if (Pawn.def.defName is "TM_DemonR" or "TM_LesserDemonR")
                    {
                        HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_DemonHD"), .5f);
                    }
                    initialized = true;
                }

                if (Pawn.Spawned)
                {
                    if (!Pawn.Downed)
                    {
                        if (NextTaunt < Find.TickManager.TicksGame)
                        {
                            DoTaunt(Pawn.Map);
                            nextTaunt = Props.tauntCooldownTicks + Find.TickManager.TicksGame;
                        }

                        if (rangedBurstShots > 0 && rangedNextBurst < Find.TickManager.TicksGame)
                        {
                            DoRangedAttack(rangedTarget);
                            rangedBurstShots--;
                            rangedNextBurst = Find.TickManager.TicksGame + Props.rangedTicksBetweenBursts;
                        }

                        if (Find.TickManager.TicksGame % 30 == 0)
                        {
                            if (buildingThreats.Count > 0)
                            {
                                Building randomBuildingThreat = buildingThreats.RandomElement();
                                if ((randomBuildingThreat.Position - Pawn.Position).LengthHorizontal < 50 && NextRangedAttack < Find.TickManager.TicksGame && TargetIsValid(randomBuildingThreat))
                                {
                                    rangedTarget = randomBuildingThreat;
                                    StartRangedAttack();
                                }
                            }

                            Thing currentTargetThing = Pawn.CurJob.targetA.Thing;
                            if (currentTargetThing != null && Pawn.TargetCurrentlyAimingAt == null)
                            {
                                if ((currentTargetThing.Position - Pawn.Position).LengthHorizontal > (Props.maxRangeForCloseThreat * 2))
                                {
                                    if (Rand.Chance(.6f) && NextRangedAttack < Find.TickManager.TicksGame && TargetIsValid(currentTargetThing))
                                    {
                                        rangedTarget = currentTargetThing;
                                        StartRangedAttack();
                                    }
                                    else if (NextChargeAttack < Find.TickManager.TicksGame && TargetIsValid(currentTargetThing))
                                    {
                                        DoChargeAttack(currentTargetThing);
                                        goto exitTick;
                                    }
                                }
                            }
                            else if (Pawn.TargetCurrentlyAimingAt != null && closeThreats.Count > 1)
                            {
                                if (Rand.Chance(.2f) && NextAoEAttack < Find.TickManager.TicksGame)
                                {
                                    DoAoEAttack(Pawn.Position, true, 2f, DamageDefOf.Stun, Rand.Range(4, 8));
                                }

                                if (Rand.Chance(.2f) && farThreats.Count > (5 * closeThreats.Count))
                                {
                                    Pawn.CurJob.targetA = farThreats.RandomElement();
                                }
                            }                            

                            if (closeThreats.Count > 1 && ((closeThreats.Count * 2) > farThreats.Count || Rand.Chance(.3f)))
                            {
                                if (Rand.Chance(.8f) && NextKnockbackAttack < Find.TickManager.TicksGame)
                                {
                                    Pawn randomClosePawn = closeThreats.RandomElement();
                                    if ((randomClosePawn.Position - Pawn.Position).LengthHorizontal < 3 && TargetIsValid(randomClosePawn))
                                    {
                                        DoKnockbackAttack(Pawn.Position, randomClosePawn.Position, 1.4f, Rand.Range(3, 5f));
                                    }
                                }
                            }

                            if(farThreats.Count > 2 * closeThreats.Count && meleeThreats.Count < 1 && Rand.Chance(.3f))
                            {
                                if(NextChargeAttack < Find.TickManager.TicksGame)
                                {
                                    Thing tempTarget = farThreats.RandomElement();
                                    if (TargetIsValid(tempTarget))
                                    {
                                        Pawn.TryStartAttack(tempTarget);
                                        DoChargeAttack(tempTarget);
                                        goto exitTick;
                                    }
                                }
                            }

                            if (farThreats.Count > 2)
                            {
                                if (Rand.Chance(.4f) && NextRangedAttack < Find.TickManager.TicksGame)
                                {
                                    Pawn randomRangedPawn = farThreats.RandomElement();
                                    if ((randomRangedPawn.Position - Pawn.Position).LengthHorizontal < Props.maxRangeForFarThreat * 1.2f)
                                    {
                                        rangedTarget = randomRangedPawn;
                                        StartRangedAttack();
                                    }
                                }
                            }

                            if (currentTargetThing == null || currentTargetThing == Pawn)
                            {
                                if (closeThreats.Count > 0)
                                {
                                    Thing tempTarget = closeThreats.RandomElement();
                                    if (TargetIsValid(tempTarget))
                                    {
                                        Pawn.CurJob.targetA = tempTarget;
                                        Pawn.TryStartAttack(Pawn.CurJob.targetA);
                                    }
                                }
                                else if (farThreats.Count > 0)
                                {
                                    Thing tempTarget = farThreats.RandomElement();
                                    if (TargetIsValid(tempTarget))
                                    {
                                        Pawn.CurJob.targetA = tempTarget;
                                        Pawn.TryStartAttack(Pawn.CurJob.targetA);
                                    }
                                }
                                else if (buildingThreats.Count > 0)
                                {
                                    Thing tempTarget = buildingThreats.RandomElement();
                                    if (TargetIsValid(tempTarget))
                                    {
                                        Pawn.CurJob.targetA = tempTarget;
                                        Pawn.TryStartAttack(Pawn.CurJob.targetA);
                                    }
                                }
                            }

                        }

                        if (Pawn.Faction != Faction.OfPlayer && Find.TickManager.TicksGame % 300 == 0)
                        {
                            if (Props.alwaysManhunter)
                            {
                                Pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true);
                            }
                        }

                        if (Find.TickManager.TicksGame % 120 == 0)
                        {
                            DetermineThreats();
                        }
                    }

                    if (Pawn.Downed && Pawn.def.defName == "TM_DemonR" && Find.TickManager.TicksGame % 18 == 0)
                    {
                        if (!deathOnce)
                        {
                            CellRect cellRect = CellRect.CenteredOn(Pawn.Position, 3);
                            cellRect.ClipInsideMap(Pawn.Map);
                            GenExplosion.DoExplosion(cellRect.RandomCell, Pawn.Map, 2f, DamageDefOf.Burn, Pawn, Rand.Range(6, 12), -1, DamageDefOf.Bomb.soundExplosion, null, null, null, null, 0f, 1, null, false, null, 0f, 0, 0.2f, true);
                            DamageEntities(Pawn, 10f, TMDamageDefOf.DamageDefOf.TM_Shadow, Pawn);
                            deathOnce = true;
                        }
                        else if(!Pawn.Dead)
                        {
                            DamageInfo dinfo = new DamageInfo(TMDamageDefOf.DamageDefOf.TM_Spirit, 10, 2, -1, Pawn);
                            Pawn.Kill(dinfo);
                        }
                    }
                }
            }
            exitTick:;
            age++;
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
            if (dinfo.Instigator is not Building instigatorThing) return;

            if (instigatorThing.Faction != null && instigatorThing.Faction != Pawn.Faction)
            {
                buildingThreats.AddDistinct(instigatorThing);
            }
        }

        private void DetermineThreats()
        {
            try
            {
                closeThreats.Clear();
                farThreats.Clear();
                meleeThreats.Clear();
                List<Pawn> allPawns = Pawn.Map.mapPawns.AllPawnsSpawned;
                for (int i = 0; i < allPawns.Count; i++)
                {
                    if (!allPawns[i].DestroyedOrNull() && allPawns[i] != Pawn)
                    {
                        if (!allPawns[i].Dead && !allPawns[i].Downed)
                        {
                            if ((allPawns[i].Position - Pawn.Position).LengthHorizontal <= Props.maxRangeForCloseThreat)
                            {
                                if (Pawn.Faction.HostileTo(allPawns[i].Faction))
                                {
                                    closeThreats.Add(allPawns[i]);
                                }
                                else if (allPawns[i].Faction == null && allPawns[i].InMentalState)
                                {
                                    closeThreats.Add(allPawns[i]);
                                }
                            }
                            else if ((allPawns[i].Position - Pawn.Position).LengthHorizontal <= Props.maxRangeForFarThreat)
                            {
                                if (Pawn.Faction.HostileTo(allPawns[i].Faction))
                                {
                                    farThreats.Add(allPawns[i]);
                                }
                                else if (allPawns[i].Faction == null && allPawns[i].InMentalState)
                                {
                                    farThreats.Add(allPawns[i]);                                    
                                }
                            }
                            else if((allPawns[i].Position - Pawn.Position).LengthHorizontal <= meleeRange)
                            {
                                if (Pawn.Faction.HostileTo(allPawns[i].Faction))
                                {
                                    meleeThreats.Add(allPawns[i]);
                                }
                                else if (allPawns[i].Faction == null && allPawns[i].InMentalState)
                                {
                                    meleeThreats.Add(allPawns[i]);
                                }
                            }
                        }
                    }
                }
                if (closeThreats.Count < 1 && farThreats.Count < 1)
                {
                    Pawn randomMapPawn = allPawns.RandomElement();
                    if (TargetIsValid(randomMapPawn) && randomMapPawn.RaceProps.Humanlike)
                    {
                        if (randomMapPawn.Faction != null && randomMapPawn.Faction != Pawn.Faction && (Pawn.Faction.HostileTo(randomMapPawn.Faction) || randomMapPawn.InMentalState))
                        {
                            farThreats.Add(randomMapPawn);
                        }
                    }
                }
                for (int i = 0; i < buildingThreats.Count; i++)
                {
                    if (buildingThreats[i].DestroyedOrNull())
                    {
                        buildingThreats.Remove(buildingThreats[i]);
                    }
                }
            }
            catch(NullReferenceException)
            {
                //Log.Message("Error processing threats" + ex);
            }
        }

        public void DamageEntities(Thing e, float d, DamageDef type, Thing instigator)
        {
            int amt = Mathf.RoundToInt(Rand.Range(.75f, 1.25f) * d);
            DamageInfo dinfo = new DamageInfo(type, amt, Rand.Range(0,amt), -1, instigator);
            bool flag = e != null;
            if (flag)
            {
                e.TakeDamage(dinfo);
            }
        }

        public Vector3 GetVector(IntVec3 center, IntVec3 objectPos)
        {
            Vector3 heading = (objectPos - center).ToVector3();
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;
            return direction;
        }

        public bool TargetIsValid(Thing target)
        {
            if(target.DestroyedOrNull())
            {
                return false;
            }
            if(!target.Spawned)
            {
                return false;
            }
            if (target is Pawn targetPawn)
            {
                return !targetPawn.Downed;
            }
            if(target.Position.DistanceToEdge(Pawn.Map) < 8)
            {
                return false;
            }
            if(target.Faction != null && target.Faction != Pawn.Faction)
            {
                return (Pawn.Faction.HostileTo(target.Faction));
            }
            return true;
        }
    }
}
