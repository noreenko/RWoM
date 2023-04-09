using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TorannMagic.ModOptions;
using UnityEngine;
using Verse;
using Verse.Sound;
using VFECore.Abilities;

namespace TorannMagic
{
    [StaticConstructorOnStartup]
    public class FlyingObject_PsionicStorm : AbilityProjectile
    {
        private int verVal;

        protected new Vector3 origin;
        protected new Vector3 destination;
        protected Vector3 trueOrigin;
        protected Vector3 targetCenter;
        private Vector3 nearApex;
        private Vector3 farApex;
        private Vector3 direction;
        List<IntVec3> targetCells;

        private List<Vector3> curvePoints = new();
        private int destinationCurvePoint;
        private int stage;
        private float curveAngle;

        protected float speed = 40f;

        protected new int ticksToImpact = 60;
        private int nextAttackTick;

        protected Thing assignedTarget;
        protected Thing flyingThing;

        public DamageInfo? impactDamage;

        public bool damageLaunched = true;
        public bool explosion;
        public int timesToDamage = 3;
        public int weaponDmg;

        Pawn pawn;

        //local variables
        float targetCellRadius = 4;
        float circleFlightSpeed = 10;
        float circleRadius = 10;
        int attackFrequencyLow = 10;
        int attackFrequencyHigh = 40;

        protected new int StartingTicksToImpact
        {
            get
            {
                int num = Mathf.RoundToInt((origin - destination).magnitude / (speed / 100f));
                if (num < 1)
                {
                    num = 1;
                }
                return num;
            }
        }

        protected new IntVec3 DestinationCell => new(destination);

        public new Vector3 ExactPosition
        {
            get
            {
                Vector3 b = (destination - origin) * (1f - ticksToImpact / (float)StartingTicksToImpact);
                return origin + b + Vector3.up * def.Altitude;
            }
        }

        public new Quaternion ExactRotation => Quaternion.LookRotation(destination - origin);

        public override Vector3 DrawPos => ExactPosition;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref origin, "origin");
            Scribe_Values.Look(ref destination, "destination");
            Scribe_Values.Look(ref trueOrigin, "trueOrigin");
            Scribe_Values.Look(ref nearApex, "nearApex");
            Scribe_Values.Look(ref farApex, "farApex");
            Scribe_Values.Look(ref direction, "direction");
            Scribe_Values.Look(ref ticksToImpact, "ticksToImpact");
            Scribe_Values.Look(ref weaponDmg, "weaponDmg");
            Scribe_Values.Look(ref verVal, "verVal");
            Scribe_Values.Look(ref destinationCurvePoint, "destinationCurvePoint");
            Scribe_Values.Look(ref damageLaunched, "damageLaunched", true);
            Scribe_Values.Look(ref explosion, "explosion");
            Scribe_References.Look(ref assignedTarget, "assignedTarget");
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Deep.Look(ref flyingThing, "flyingThing");
            Scribe_Collections.Look(ref targetCells, "targetCells", LookMode.Value);
            Scribe_Collections.Look(ref curvePoints, "curvePoints", LookMode.Value);
        }

        private void Initialize()
        {
            if (pawn != null)
            {
                FleckMaker.Static(origin, Map, FleckDefOf.ExplosionFlash, 12f);
                FleckMaker.ThrowDustPuff(origin, Map, Rand.Range(1.2f, 1.8f));
                curvePoints = new List<Vector3>();
                curvePoints.Clear();
                targetCells = new List<IntVec3>();
                targetCells.Clear();
                targetCells = GenRadial.RadialCellsAround(targetCenter.ToIntVec3(), 4, true).ToList();
                verVal = pawn.GetCompAbilityUserMight().MightData.MightPowerSkill_PsionicStorm.FirstOrDefault(x => x.label == "TM_PsionicStorm_ver").level;
            }
            //flyingThing.ThingID += Rand.Range(0, 2147).ToString();
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
        {
            Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, Verb verb)
        {
            Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing);
        }

        public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing, DamageInfo? newDamageInfo = null)
        {
            bool spawned = flyingThing.Spawned;
            pawn = launcher as Pawn;
            if (spawned)
            {
                flyingThing.DeSpawn();
            }
            Constants.SetPawnInFlight(true);
            this.origin = origin;
            trueOrigin = origin;
            impactDamage = newDamageInfo;
            this.flyingThing = flyingThing;
            bool flag = targ.Thing != null;
            if (flag)
            {
                assignedTarget = targ.Thing;
            }
            targetCenter = targ.Cell.ToVector3Shifted();
            direction = GetVector(trueOrigin, targetCenter);
            nearApex = targetCenter + -circleRadius * direction;
            farApex = targetCenter + circleRadius * direction;
            destination = nearApex; //set initial destination to be outside of storm circle
            ticksToImpact = StartingTicksToImpact;
            Initialize();
        }

        public void CalculateCurvePoints(Vector3 start, Vector3 end, float variance)
        {
            destinationCurvePoint = 0;
            curvePoints.Clear();
            int variancePoints = 20;
            Vector3 initialVector = GetVector(start, end);
            initialVector.y = 0;
            initialVector.ToAngleFlat();
            if (curveAngle == 0)
            {
                if (Rand.Chance(.5f))
                {
                    curveAngle = variance;
                }
                else
                {
                    variance = -1 * variance;
                    curveAngle = variance;
                }
            }
            else
            {
                variance = curveAngle;
            }
            //calculate extra distance bolt travels around the ellipse
            float a = .5f * Vector3.Distance(start, end);
            float b = a * Mathf.Sin(.5f * Mathf.Deg2Rad * Mathf.Abs(curveAngle));
            float p = .5f * Mathf.PI * (3 * (a + b) - Mathf.Sqrt((3 * a + b) * (a + 3 * b)));

            float incrementalDistance = p / variancePoints;
            float incrementalAngle = (variance / variancePoints) * 2;
            curvePoints.Add(start);
            for (int i = 1; i < variancePoints; i++)
            {
                curvePoints.Add(curvePoints[i - 1] + Quaternion.AngleAxis(variance, Vector3.up) * initialVector * incrementalDistance); //(Quaternion.AngleAxis(curveAngle, Vector3.up) *
                variance -= incrementalAngle;
            }
        }

        public override void Tick()
        {
            //base.Tick();
            ticksToImpact--;
            bool flag = !ExactPosition.InBoundsWithNullCheck(Map) || Position.DistanceToEdge(Map) <= 1;
            if (stage is > 0 and < 4 && nextAttackTick < Find.TickManager.TicksGame)
            {
                IntVec3 targetVariation = targetCells.RandomElement();
                float angle = (Quaternion.AngleAxis(90, Vector3.up) * GetVector(ExactPosition, targetVariation.ToVector3Shifted())).ToAngleFlat();
                Vector3 drawPos = ExactPosition + (GetVector(ExactPosition, targetVariation.ToVector3Shifted()) * .5f);
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_PsiBlastStart, drawPos, Map, Rand.Range(.4f, .6f), Rand.Range(.0f, .05f), 0f, .1f, 0, 0, 0, angle); //throw psi blast start
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_PsiBlastEnd, drawPos, Map, Rand.Range(.4f, .8f), Rand.Range(.0f, .1f), .2f, .3f, 0, Rand.Range(1f, 1.5f), angle, angle); //throw psi blast end
                TryLaunchProjectile(ThingDef.Named("TM_Projectile_PsionicBlast"), targetVariation);
                nextAttackTick = Find.TickManager.TicksGame + Mathf.RoundToInt(Rand.Range(attackFrequencyLow, attackFrequencyHigh) * (1 - .1f * verVal));
            }
            if (flag)
            {
                ticksToImpact++;
                Position = ExactPosition.ToIntVec3();
                GenPlace.TryPlaceThing(flyingThing, Position, Map, ThingPlaceMode.Near);
                //GenSpawn.Spawn(flyingThing, base.Position, base.Map);
                Constants.SetPawnInFlight(false);
                Pawn p = flyingThing as Pawn;
                if (p.IsColonist)
                {
                    p.drafter.Drafted = true;
                    if (Settings.Instance.cameraSnap)
                    {
                        CameraJumper.TryJumpAndSelect(p);
                    }
                }
                Destroy();
            }
            else
            {
                Position = ExactPosition.ToIntVec3();
                FleckMaker.ThrowDustPuff(Position, Map, Rand.Range(0.8f, 1.2f));
                if (ticksToImpact > 0) return;
                switch (stage)
                {
                    case 0:
                        CalculateCurvePoints(nearApex, farApex, 90);
                        origin = curvePoints[destinationCurvePoint];
                        destinationCurvePoint++;
                        destination = curvePoints[destinationCurvePoint];
                        speed = circleFlightSpeed;
                        ticksToImpact = StartingTicksToImpact;
                        nextAttackTick = Find.TickManager.TicksGame + Mathf.RoundToInt(Rand.Range(attackFrequencyLow, attackFrequencyHigh) * (1 - .1f * verVal));
                        stage = 1;
                        break;
                    case 1 when curvePoints.Count - 1 > destinationCurvePoint:
                        origin = curvePoints[destinationCurvePoint];
                        destinationCurvePoint++;
                        destination = curvePoints[destinationCurvePoint];
                        ticksToImpact = StartingTicksToImpact;
                        break;
                    case 1:
                        origin = curvePoints[destinationCurvePoint];
                        CalculateCurvePoints(origin, nearApex, 90);
                        destinationCurvePoint++;
                        destination = curvePoints[destinationCurvePoint];
                        ticksToImpact = StartingTicksToImpact;
                        stage = 2;
                        break;
                    case 2 when curvePoints.Count - 1 > destinationCurvePoint:
                        origin = curvePoints[destinationCurvePoint];
                        destinationCurvePoint++;
                        destination = curvePoints[destinationCurvePoint];
                        ticksToImpact = StartingTicksToImpact;
                        break;
                    case 2:
                        origin = curvePoints[destinationCurvePoint];
                        destination = nearApex;
                        ticksToImpact = StartingTicksToImpact;
                        //speed = 15;
                        stage = 3;
                        break;
                    case 3:
                        speed = 25f;
                        origin = nearApex;
                        destination = trueOrigin;
                        ticksToImpact = StartingTicksToImpact;
                        stage = 4;
                        break;
                    default:
                    {
                        if (DestinationCell.InBoundsWithNullCheck(Map))
                        {
                            Position = DestinationCell;
                        }
                        ImpactSomething();
                        break;
                    }
                }

            }
        }

        public override void Draw()
        {
            if (flyingThing == null) return;

            float angleToCenter = GetVector(ExactPosition, targetCenter).ToAngleFlat();
            flyingThing.Rotation = angleToCenter switch
            {
                > -45 and < 45 => Rot4.East,
                > 45 and < 135 => Rot4.South,
                > 135 or < -135 => Rot4.West,
                _ => Rot4.North
            };

            if (flyingThing is Pawn)
            {
                if (!DrawPos.ToIntVec3().IsValid) return;
                Pawn pawn = flyingThing as Pawn;
                pawn.Drawer.DrawAt(DrawPos);
            }
            else
            {
                Graphics.DrawMesh(MeshPool.plane10, DrawPos, ExactRotation, flyingThing.def.DrawMatSingle, 0);
            }
            Comps_PostDraw();
        }

        private void TryLaunchProjectile(ThingDef projectileDef, LocalTargetInfo launchTarget)
        {
                Vector3 drawPos = ExactPosition;
                AbilityProjectile projectile_AbilityBase = (AbilityProjectile)GenSpawn.Spawn(projectileDef, ExactPosition.ToIntVec3(), Map);
                //ShotReport shotReport = ShotReport.HitReportFor(pawn, verb, launchTarget);
                SoundDef expr_C8 = TorannMagicDefOf.TM_AirWoosh;
                if (expr_C8 != null)
                {
                    expr_C8.PlayOneShot(new TargetInfo(ExactPosition.ToIntVec3(), Map));
                }
                // TODO jecstools - Check that this change is correct. Had to alter args
                projectile_AbilityBase.Launch(pawn, drawPos, launchTarget, launchTarget, ProjectileHitFlags.All);
        }

        private void ImpactSomething()
        {
            bool flag = assignedTarget != null;
            if (flag)
            {
                Pawn pawn = assignedTarget as Pawn;
                bool flag2 = pawn != null && pawn.GetPosture() != PawnPosture.Standing && (origin - destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.2f;
                if (flag2)
                {
                    Impact(null);
                }
                else
                {
                    Impact(assignedTarget);
                }
            }
            else
            {
                Impact(null);
            }
        }

        protected void Impact(Thing hitThing)
        {
            bool flag = hitThing == null;
            if (flag)
            {
                Pawn pawn;
                bool flag2 = (pawn = (Position.GetThingList(Map).FirstOrDefault(x => x == assignedTarget) as Pawn)) != null;
                if (flag2)
                {
                    hitThing = pawn;
                }
            }
            bool hasValue = impactDamage.HasValue;
            if (hasValue)
            {
                for (int i = 0; i < timesToDamage; i++)
                {
                    bool flag3 = damageLaunched;
                    if (flag3)
                    {
                        flyingThing.TakeDamage(impactDamage.Value);
                    }
                    else
                    {
                        hitThing.TakeDamage(impactDamage.Value);
                    }
                }
                if (explosion)
                {
                    GenExplosion.DoExplosion(Position, Map, 0.9f, DamageDefOf.Stun, this, -1, 0);
                }
            }
            GenSpawn.Spawn(flyingThing, Position, Map);
            Constants.SetPawnInFlight(false);
            Pawn p = flyingThing as Pawn;
            if(p.IsColonist)
            {
                p.drafter.Drafted = true;
                if (Settings.Instance.cameraSnap)
                {
                    CameraJumper.TryJumpAndSelect(p);
                }
            }
            Destroy();
        }

        public Vector3 GetVector(Vector3 center, Vector3 objectPos)
        {
            Vector3 heading = objectPos - center;
            float distance = heading.magnitude;
            return heading / distance;
        }
    }
}
