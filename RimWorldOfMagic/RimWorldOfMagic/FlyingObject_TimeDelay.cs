using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TorannMagic
{
    [StaticConstructorOnStartup]
    public class FlyingObject_TimeDelay : Projectile
    {
        protected new Vector3 origin;
        protected new Vector3 destination;
        private Vector3 variationDestination;
        private Vector3 drawPosition;

        public float speed = 10f;
        public int spinRate;        //spin rate > 0 makes the object rotate every spinRate Ticks
        public float xVariation;    //x variation makes the object move side to side by +- variation
        public float zVariation;    //z variation makes the object move up and down by +- variation
        private int rotation;
        protected new int ticksToImpact;
        //protected new Thing launcher;
        protected Thing assignedTarget;
        protected Thing flyingThing;
        private bool drafted;
        public float destroyPctAtEnd;

        public int moteFrequency;
        public ThingDef moteDef;
        public float fadeInTime = .25f;
        public float fadeOutTime = .25f;
        public float solidTime = .5f;
        public float moteScale = 1f;

        public float force = 1f;
        public int duration = 600;

        private bool earlyImpact = false;
        private float impactForce = 0;
        private int variationShiftTick = 100;

        public bool damageLaunched = true;

        public bool explosion;

        Pawn pawn;

        protected new int StartingTicksToImpact
        {
            get
            {
                int num = Mathf.RoundToInt((origin - destination).magnitude / (speed / 100f));
                return num < 1 ? 1 : num;
            }
        }

        protected new IntVec3 DestinationCell => new IntVec3(destination);

        public new Vector3 ExactPosition => origin + Vector3.up * def.Altitude;

        public new Quaternion ExactRotation => Quaternion.LookRotation(origin - destination);

        public override Vector3 DrawPos => ExactPosition;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref origin, "origin");
            Scribe_Values.Look(ref destination, "destination");
            Scribe_Values.Look(ref ticksToImpact, "ticksToImpact");
            Scribe_Values.Look(ref damageLaunched, "damageLaunched", true);
            Scribe_Values.Look(ref explosion, "explosion");
            Scribe_References.Look(ref assignedTarget, "assignedTarget");
            //Scribe_References.Look<Thing>(ref this.launcher, "launcher", false);
            Scribe_Deep.Look(ref flyingThing, "flyingThing");
            Scribe_Values.Look(ref drafted, "drafted");
            Scribe_Values.Look(ref xVariation, "xVariation");
            Scribe_Values.Look(ref zVariation, "zVariation");
            Scribe_Values.Look(ref solidTime, "solidTime", .5f);
            Scribe_Values.Look(ref fadeInTime, "fadeInTime", .25f);
            Scribe_Values.Look(ref fadeOutTime, "fadeOutTime", .25f);
            Scribe_Defs.Look(ref moteDef, "moteDef");
            Scribe_Values.Look(ref moteScale, "moteScale", 1f);
            Scribe_Values.Look(ref moteFrequency, "moteFrequency");
            Scribe_Values.Look(ref destroyPctAtEnd, "destroyPctAtEnd");
        }

        private void Initialize()
        {
            if (pawn != null)
            {
                FleckMaker.ThrowDustPuff(origin, Map, Rand.Range(1.2f, 1.8f));
            }

            TM_Calc.GetVector(origin.ToIntVec3(), destination.ToIntVec3());
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
        {
            Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
        {
            Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing);
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, int _spinRate)
        {
            spinRate = _spinRate;
            Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing);
        }

        public void LaunchVaryPosition(Thing launcher, LocalTargetInfo targ, Thing flyingThing, int _spinRate, float _xVariation, float _zVariation, ThingDef mote = null, int moteFreq = 0, float destroy = 0f)
        {
            destroyPctAtEnd = destroy;
            moteDef = mote;
            moteFrequency = moteFreq;
            xVariation = _xVariation;
            zVariation = _zVariation;
            spinRate = _spinRate;
            Launch(launcher, flyingThing.DrawPos, flyingThing.Position, flyingThing);
        }

        public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing, DamageInfo? newDamageInfo = null)
        {
            bool spawned = flyingThing.Spawned;
            pawn = launcher as Pawn;
            if (pawn != null && pawn.Drafted)
            {
                drafted = true;
            }
            if (spawned)
            {
                flyingThing.DeSpawn();
            }
            speed *= force;
            this.launcher = launcher;
            this.origin = origin;
            this.flyingThing = flyingThing;

            bool flag = targ.Thing != null;
            if (flag)
            {
                assignedTarget = targ.Thing;
            }
            destination = targ.Cell.ToVector3Shifted();
            ticksToImpact = StartingTicksToImpact;
            variationDestination = Position.ToVector3Shifted(); //this.DrawPos //not initialized?
            drawPosition = Position.ToVector3Shifted(); //this.DrawPos;
            Initialize();
        }

        public override void Tick()
        {
            duration--;
            Position = origin.ToIntVec3();
            bool flag2 = duration <= 0;
            if(moteDef != null && Map != null && Find.TickManager.TicksGame % moteFrequency == 0)
            {
                TM_MoteMaker.ThrowGenericMote(moteDef, ExactPosition, Map, Rand.Range(moteScale * .75f, moteScale * 1.25f), solidTime, fadeInTime, fadeOutTime, Rand.Range(200, 400), 0, 0, Rand.Range(0, 360));
            }
            if (flag2)
            {
                ImpactSomething();
            }

        }

        public override void Draw()
        {
            if (flyingThing != null)
            {
                if (spinRate > 0)
                {
                    if(Find.TickManager.TicksGame % spinRate ==0)
                    {
                        rotation++;
                        if(rotation >= 4)
                        {
                            rotation = 0;
                        }
                    }
                    if (rotation == 0)
                    {
                        flyingThing.Rotation = Rot4.West;
                    }
                    else if (rotation == 1)
                    {
                        flyingThing.Rotation = Rot4.North;
                    }
                    else if (rotation == 2)
                    {
                        flyingThing.Rotation = Rot4.East;
                    }
                    else
                    {
                        flyingThing.Rotation = Rot4.South;
                    }
                }

                bool flag2 = flyingThing is Pawn;
                if (flag2 && zVariation == 0 && xVariation == 0)
                {
                    if (!DrawPos.ToIntVec3().IsValid)
                    {
                        return;
                    }
                    Pawn pawn = flyingThing as Pawn;
                    pawn.Drawer.DrawAt(DrawPos);
                    Material bubble = TM_MatPool.TimeBubble;
                    Vector3 vec3 = DrawPos;
                    vec3.y++;
                    Vector3 s = new Vector3(2f, 1f, 2f);
                    Matrix4x4 matrix = default(Matrix4x4);
                    matrix.SetTRS(vec3, Quaternion.AngleAxis(0, Vector3.up), s);
                    Graphics.DrawMesh(MeshPool.plane10, matrix, bubble, 0, null);
                    //Graphics.DrawMesh(MeshPool.plane10, vec3, this.ExactRotation, bubble, 0);
                }
                else if(zVariation != 0 || xVariation != 0)
                {
                    drawPosition = VariationPosition(drawPosition);
                    flyingThing.DrawAt(drawPosition);
                }
                else
                {
                    Graphics.DrawMesh(MeshPool.plane10, DrawPos, ExactRotation, flyingThing.def.DrawMatSingle, 0);
                }
            }
            Comps_PostDraw();
        }

        private Vector3 VariationPosition(Vector3 currentDrawPos)
        {
            Vector3 startPos = currentDrawPos;
            startPos.y = 10f;
            float variance = (xVariation / 100f);
            if ((startPos.x - variationDestination.x) < -variance)
            {
                startPos.x += variance;
            }
            else if((startPos.x - variationDestination.x) > variance)
            {
                startPos.x += -variance;
            }
            else if (xVariation != 0)
            {
                variationDestination.x = DrawPos.x + Rand.Range(-xVariation, xVariation);
            }
            variance = (zVariation / 100f);
            if ((startPos.z - variationDestination.z) < -variance)
            {
                startPos.z += variance;
            }
            else if ((startPos.z - variationDestination.z) > variance)
            {
                startPos.z += -variance;
            }
            else if (zVariation != 0)
            {
                variationDestination.z = DrawPos.z + Rand.Range(-zVariation, zVariation);
            }

            return startPos;
        }


        private void ImpactSomething()
        {
            Impact(null);
        }

        protected void Impact(Thing hitThing)
        {
            if (Map != null)
            {
                GenPlace.TryPlaceThing(flyingThing, Position, Map, ThingPlaceMode.Direct);
                if (flyingThing is Pawn p)
                {
                    if (p.IsColonist && drafted && p.drafter != null)
                    {
                        p.drafter.Drafted = true;
                    }
                }

                if (destroyPctAtEnd != 0)
                {
                    int rangeMax = 10;
                    for (int i = 0; i < rangeMax; i++)
                    {
                        float direction = Rand.Range(0, 360);
                        Vector3 rndPos = flyingThing.DrawPos;
                        rndPos.x += Rand.Range(-.3f, .3f);
                        rndPos.z += Rand.Range(-.3f, .3f);
                        ThingDef mote = TorannMagicDefOf.Mote_Shadow;
                        TM_MoteMaker.ThrowGenericMote(mote, rndPos, Map, Rand.Range(.5f, 1f), 0.4f, Rand.Range(.1f, .4f), Rand.Range(1.2f, 2f), Rand.Range(-200, 200), Rand.Range(1.2f, 2f), direction, direction);

                    }
                    SoundInfo info = SoundInfo.InMap(new TargetInfo(Position, Map));
                    info.pitchFactor = .8f;
                    info.volumeFactor = 1.2f;
                    TorannMagicDefOf.TM_Vibration.PlayOneShot(info);
                }

                if (destroyPctAtEnd >= 1f)
                {
                    flyingThing.Destroy();
                }
                else if (destroyPctAtEnd != 0)
                {
                    flyingThing.SplitOff(Mathf.RoundToInt(flyingThing.stackCount * destroyPctAtEnd)).Destroy();
                }
            }
            Destroy();
        }
    }
}
