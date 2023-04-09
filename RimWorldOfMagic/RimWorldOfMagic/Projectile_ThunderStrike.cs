using Verse;

using UnityEngine;

namespace TorannMagic
{
    [StaticConstructorOnStartup]
    public class Projectile_ThunderStrike : VFECore.Abilities.AbilityProjectile
    {
        Vector3 direction;
        Vector3 directionOffsetRight;
        Vector3 directionOffsetLeft;

        int iteration;
        int maxIteration = 4;
        float directionMagnitudeOffset = 1.5f;
        private bool initialized;

        private int verVal;
        private int pwrVal;
        private float arcaneDmg = 1f;

        int nextEventTick;
        int nextRightEventTick;
        int nextLeftEventTick;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector3>(ref origin, "origin");
            Scribe_Values.Look<Vector3>(ref destination, "destination");
            Scribe_Values.Look<Vector3>(ref direction, "direction");
            Scribe_Values.Look<Vector3>(ref directionOffsetRight, "directionOffsetRight");
            Scribe_Values.Look<Vector3>(ref directionOffsetLeft, "directionOffsetLeft");
            Scribe_Values.Look<int>(ref verVal, "verVal");
            Scribe_Values.Look<int>(ref pwrVal, "pwrVal");
            Scribe_Values.Look<int>(ref iteration, "iteration");
            Scribe_Values.Look<bool>(ref initialized, "initialized");
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);

            if(!initialized)
            {
                Initialize(Position, launcher as Pawn);
            }
            Vector3 directionOffset;

            if(initialized && nextLeftEventTick < Find.TickManager.TicksGame && nextLeftEventTick != 0)
            {
                directionOffset = directionOffsetLeft * (directionMagnitudeOffset * iteration);
                DoThunderStrike(directionOffset);
                nextLeftEventTick = 0;
            }
            if(initialized && nextRightEventTick < Find.TickManager.TicksGame && nextRightEventTick != 0)
            {
                directionOffset = directionOffsetRight * (directionMagnitudeOffset * iteration);
                DoThunderStrike(directionOffset);
                nextRightEventTick = 0;
            }
            if (initialized && nextEventTick < Find.TickManager.TicksGame)
            {               
                
                if(iteration == 1 && verVal > 0)
                {
                    nextRightEventTick = Find.TickManager.TicksGame + Rand.Range(2, 6);
                    nextLeftEventTick = Find.TickManager.TicksGame + Rand.Range(2, 6);                    
                }
                if (iteration == 3 && verVal > 1)
                {
                    nextRightEventTick = Find.TickManager.TicksGame + Rand.Range(2, 6);
                    nextLeftEventTick = Find.TickManager.TicksGame + Rand.Range(2, 6);
                }
                if (iteration == 5 && verVal > 2)
                {
                    nextRightEventTick = Find.TickManager.TicksGame + Rand.Range(2, 6);
                    nextLeftEventTick = Find.TickManager.TicksGame + Rand.Range(2, 6);
                }
                iteration++;
                directionOffset = direction * (directionMagnitudeOffset * iteration);
                DoThunderStrike(directionOffset);

                nextEventTick = Find.TickManager.TicksGame + Rand.Range(2,5);                
            }                       

        }

        private void DoThunderStrike(Vector3 directionOffset)
        {
            if (directionOffset == default) return;

            IntVec3 currentPos = (origin + directionOffset).ToIntVec3();
            if (currentPos != default && currentPos.IsValid && currentPos.InBoundsWithNullCheck(Map) && currentPos.Walkable(Map) && currentPos.DistanceToEdge(Map) > 3)
            {
                CellRect cellRect = CellRect.CenteredOn(currentPos, 1);
                //cellRect.ClipInsideMap(Map);
                IntVec3 rndCell = cellRect.RandomCell;
                if (rndCell != default && rndCell.IsValid && rndCell.InBoundsWithNullCheck(Map) && rndCell.Walkable(Map) && rndCell.DistanceToEdge(Map) > 3)
                {
                    Map.weatherManager.eventHandler.AddEvent(new TM_WeatherEvent_MeshFlash(Map, rndCell, TM_MatPool.chiLightning, TMDamageDefOf.DamageDefOf.TM_ChiBurn, launcher, Mathf.RoundToInt(Rand.Range(8, 14) * (1 +(.12f * pwrVal)) * arcaneDmg), Rand.Range(1.5f, 2f)));
                }
            }
        }

        private void Initialize(IntVec3 target, Pawn pawn)
        {
            if (pawn != null)
            {
                verVal = TM_Calc.GetSkillVersatilityLevel(pawn, TorannMagicDefOf.TM_ThunderStrike, false);
                pwrVal = TM_Calc.GetSkillPowerLevel(pawn, TorannMagicDefOf.TM_ThunderStrike, false);
                arcaneDmg = pawn.GetCompAbilityUserMight().mightPwr;
                origin = pawn.Position.ToVector3Shifted();
                destination = target.ToVector3Shifted();
                direction = TM_Calc.GetVector(origin, destination);
                directionOffsetRight = Quaternion.AngleAxis(30, Vector3.up) * direction;
                directionOffsetLeft = Quaternion.AngleAxis(-30, Vector3.up) * direction;
                //Log.Message("origin: " + origin + " destination: " + destination + " direction: " + direction + " directionRight: " + directionOffsetRight);
                maxIteration += verVal;
                initialized = true;
                HealthUtility.AdjustSeverity(pawn, TorannMagicDefOf.TM_HediffInvulnerable, .05f);
            }
            else
            {
                Log.Warning("Failed to initialize " + def.defName);
                iteration = maxIteration;
            }            
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (iteration < maxIteration) return;

            Pawn pawn = launcher as Pawn;
            if(!pawn.DestroyedOrNull() && !pawn.Dead && pawn.Spawned)
            {
                Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_HediffInvulnerable);
                if (hediff != null)
                {
                    pawn.health.RemoveHediff(hediff);
                }
            }
            base.Destroy(mode);
        }
    }
}
