using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TorannMagic;

[StaticConstructorOnStartup]
public class FlyingObject_Advanced : Projectile
{
    protected new Vector3 origin;        
    protected new Vector3 destination;
    protected Vector3 trueOrigin;
    protected Vector3 trueDestination;

    public float speed = 30f;
    public Vector3 travelVector;
    protected new int ticksToImpact;
    protected Thing assignedTarget;
    protected Thing flyingThing;

    public ThingDef moteDef = null;
    public int moteFrequency = 0;
    public float moteSize = 1f;

    public bool spinning = false;
    public float curveVariance; // 0 = no curve
    private List<Vector3> curvePoints = new();
    public float force = 1f;
    private int destinationCurvePoint;
    private float impactRadius;
    private int explosionDamage;
    private bool isExplosive;
    private DamageDef impactDamageType;
    private bool fliesOverhead;

    private bool earlyImpact;
    private float impactForce;

    public DamageInfo? impactDamage;

    public bool damageLaunched = true;
    public bool explosion;
    public int weaponDmg;
    private int doublesidedVariance;

    Pawn pawn;

    //Magic related
    CompAbilityUserMagic comp;
    TMPawnSummoned newPawn = new TMPawnSummoned();

    protected new int StartingTicksToImpact
    {
        get
        {
            int num = Mathf.RoundToInt((origin - destination).magnitude / (speed / 100f));
            bool flag = num < 1;
            if (flag)
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
            Vector3 b = (destination - origin) * (1f - (float)ticksToImpact / StartingTicksToImpact);
            return origin + b + Vector3.up * def.Altitude;
        }
    }

    public new Quaternion ExactRotation => Quaternion.LookRotation(destination - origin);

    public override Vector3 DrawPos => ExactPosition;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look<Vector3>(ref origin, "origin");
        Scribe_Values.Look<Vector3>(ref destination, "destination");
        Scribe_Values.Look<int>(ref ticksToImpact, "ticksToImpact");
        Scribe_Values.Look<bool>(ref damageLaunched, "damageLaunched", true);
        Scribe_Values.Look<bool>(ref explosion, "explosion");
        Scribe_References.Look<Thing>(ref assignedTarget, "assignedTarget");
        Scribe_Deep.Look<Thing>(ref flyingThing, "flyingThing");
        Scribe_References.Look<Pawn>(ref pawn, "pawn");
    }

    public virtual void PreInitialize()
    {

    }

    private void Initialize()
    {
        PreInitialize();
        if (pawn != null)
        {
            FleckMaker.ThrowDustPuff(origin, Map, Rand.Range(1.2f, 1.8f));
        }
        else
        {
            flyingThing.ThingID += Rand.Range(0, 214).ToString();
        }            
    }

    public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
    {
        Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
    }

    public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
    {
        Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing);
    }

    public void AdvancedLaunch(Thing launcher, ThingDef effectMote, int moteFrequencyTicks, float curveAmount, bool shouldSpin, Vector3 origin, LocalTargetInfo targ, Thing flyingThing, int flyingSpeed, bool isExplosion, int _impactDamage, float _impactRadius, DamageDef damageType, DamageInfo? newDamageInfo = null, int doubleVariance = 0, bool flyOverhead = false, float moteEffectSize = 1f)
    {
        fliesOverhead = flyOverhead;
        explosionDamage = _impactDamage;
        isExplosive = isExplosion;
        impactRadius = _impactRadius;
        impactDamageType = damageType;
        moteFrequency = moteFrequencyTicks;
        moteDef = effectMote;
        moteSize = moteEffectSize;
        curveVariance = curveAmount;
        spinning = shouldSpin;
        speed = flyingSpeed;
        doublesidedVariance = doubleVariance;
        curvePoints = new List<Vector3>();
        curvePoints.Clear();
        Launch(launcher, origin, targ, flyingThing, newDamageInfo);
    }

    public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing, DamageInfo? newDamageInfo = null)
    {
        pawn = launcher as Pawn;
        if (flyingThing.Spawned)
        {               
            flyingThing.DeSpawn();
        }
        launcher = launcher;
        trueOrigin = origin;
        trueDestination = targ.Cell.ToVector3();
        impactDamage = newDamageInfo;
        flyingThing = flyingThing;
        if (targ.Thing != null)
        {
            assignedTarget = targ.Thing;
        }
        speed *= force;
        origin = origin;
        if(curveVariance > 0)
        {
            CalculateCurvePoints(trueOrigin, trueDestination, curveVariance);
            destinationCurvePoint++;
            destination = curvePoints[destinationCurvePoint];
        }
        else
        {
            destination = trueDestination;
        }            
        ticksToImpact = StartingTicksToImpact;
        Initialize();
    }        

    public void CalculateCurvePoints(Vector3 start, Vector3 end, float variance)
    {
        int variancePoints = 20;
        Vector3 initialVector = GetVector(start, end);
        initialVector.y = 0;
        travelVector = initialVector;
        float initialAngle = (initialVector).ToAngleFlat(); //Quaternion.AngleAxis(90, Vector3.up) *
        float curveAngle = variance;
        if(doublesidedVariance == 0)
        {
            if (Rand.Chance(.5f))
            {
                curveAngle = (-1) * variance;
            }
        }
        else
        {
            curveAngle = (doublesidedVariance * variance);
        }

        //calculate extra distance bolt travels around the ellipse
        float a = .47f * Vector3.Distance(start, end);
        float b = a * Mathf.Sin(.5f * Mathf.Deg2Rad * variance);
        float p = .5f * Mathf.PI * (3 * (a + b) - (Mathf.Sqrt((3 * a + b) * (a + 3 * b))));
                    
        float incrementalDistance = p / variancePoints; 
        float incrementalAngle = curveAngle / variancePoints * 2f;
        curvePoints.Add(trueOrigin);
        for(int i = 1; i <= (variancePoints + 1); i++)
        {
            curvePoints.Add(curvePoints[i - 1] + ((Quaternion.AngleAxis(curveAngle, Vector3.up) * initialVector) * incrementalDistance)); //(Quaternion.AngleAxis(curveAngle, Vector3.up) *
            curveAngle -= incrementalAngle;
        }
    }

    public Vector3 GetVector(Vector3 center, Vector3 objectPos)
    {
        Vector3 heading = (objectPos - center);
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        return direction;
    }

    public virtual void PreTick()
    {

    }

    public override void Tick()
    {
        PreTick();
        Vector3 exactPosition = ExactPosition;
        if (ticksToImpact >= 0 && moteDef != null && Find.TickManager.TicksGame % moteFrequency == 0)
        {
            DrawEffects(exactPosition);
        }
        ticksToImpact--;
        if (!ExactPosition.InBoundsWithNullCheck(Map))
        {
            ticksToImpact++;
            Position = ExactPosition.ToIntVec3();
            Destroy();
        }
        else if(!ExactPosition.ToIntVec3().Walkable(Map) && !fliesOverhead)
        {
            earlyImpact = true;
            impactForce = (DestinationCell - ExactPosition.ToIntVec3()).LengthHorizontal + (speed * .2f);
            ImpactSomething();
        }
        else
        {
            Position = ExactPosition.ToIntVec3();
            if(moteDef == null && Find.TickManager.TicksGame % 3 == 0)
            {
                FleckMaker.ThrowDustPuff(Position, Map, Rand.Range(0.6f, .8f));
            }
                
            bool flag2 = ticksToImpact <= 0;
            if (flag2)
            {
                if (curveVariance > 0)
                {
                    if (curvePoints.Count - 1 > destinationCurvePoint)
                    {
                        origin = curvePoints[destinationCurvePoint];
                        destinationCurvePoint++;
                        destination = curvePoints[destinationCurvePoint];
                        ticksToImpact = StartingTicksToImpact;
                    }
                    else
                    {
                        if (DestinationCell.InBoundsWithNullCheck(Map))
                        {
                            Position = DestinationCell;
                        }
                        ImpactSomething();
                    }
                }
                else
                {
                    if (DestinationCell.InBoundsWithNullCheck(Map))
                    {
                        Position = DestinationCell;
                    }
                    ImpactSomething();
                }
            }                
        }
        PostTick();
    }

    public virtual void PostTick()
    {

    }

    public override void Draw()
    {
        if (flyingThing != null)
        {
            if (flyingThing is Pawn flyingPawn)
            {
                if (!DrawPos.ToIntVec3().IsValid)
                {
                    return;
                }
                flyingPawn.Drawer.DrawAt(DrawPos);
                    
            }
            else
            {
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetTRS(DrawPos, ExactRotation, new Vector3(Graphic.drawSize.x, 13f, Graphic.drawSize.y));
                Graphics.DrawMesh(MeshPool.plane10,matrix, flyingThing.def.DrawMatSingle, 0);
            }
        }
        Comps_PostDraw();
    }

    public virtual void DrawEffects(Vector3 effectVec)
    {
        effectVec.x += Rand.Range(-0.4f, 0.4f);
        effectVec.z += Rand.Range(-0.4f, 0.4f);            
        TM_MoteMaker.ThrowGenericMote(moteDef, effectVec, Map, Rand.Range(.4f, .6f), Rand.Range(.05f, .1f), .03f, Rand.Range(.2f, .3f), Rand.Range(-200, 200), Rand.Range(.5f, 2f), Rand.Range(0, 360), Rand.Range(0, 360));
    }

    private void ImpactSomething()
    {
        if (assignedTarget != null)
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
        if (hitThing == null)
        {
            if (Position.GetThingList(Map).FirstOrDefault(t => t == assignedTarget) is Pawn hitPawn)
            {
                hitThing = hitPawn;
            }
        }
        if (impactDamage.HasValue)
        {
            hitThing?.TakeDamage(impactDamage.Value);
        }
        ImpactOverride();
        if (flyingThing is Pawn p)
        {
            try
            {
                SoundDefOf.Ambient_AltitudeWind.sustainFadeoutTime.Equals(30.0f);

                GenSpawn.Spawn(flyingThing, Position, Map);
                if (earlyImpact)
                {
                    damageEntities(p, impactForce, DamageDefOf.Blunt);
                    damageEntities(p, 2 * impactForce, DamageDefOf.Stun);
                }
                Destroy();
            }
            catch
            {
                GenSpawn.Spawn(flyingThing, Position, Map);
                Destroy();
            }
        }
        else
        {
            if(impactRadius > 0)
            {
                if(isExplosive)
                {
                    GenExplosion.DoExplosion(ExactPosition.ToIntVec3(), Map, impactRadius, impactDamageType, launcher as Pawn, explosionDamage, -1, impactDamageType.soundExplosion, def, null, null, null, 0f, 1, null, false, null, 0f, 0, 0.0f, true);
                }
                else
                {
                    int num = GenRadial.NumCellsInRadius(impactRadius);
                    IntVec3 curCell;
                    for (int i = 0; i < num; i++)
                    {
                        curCell = ExactPosition.ToIntVec3() + GenRadial.RadialPattern[i];
                        List<Thing> hitList = new List<Thing>();
                        hitList = curCell.GetThingList(Map);
                        for (int j = 0; j < hitList.Count; j++)
                        {
                            if (hitList[j] is Pawn && hitList[j] != pawn)
                            {
                                damageEntities(hitList[j], explosionDamage, impactDamageType);
                            }
                        }
                    }
                }
            }
            Destroy();
        }
    }

    public virtual void ImpactOverride()
    {

    }

    public void damageEntities(Thing e, float d, DamageDef type)
    {
        int amt = Mathf.RoundToInt(Rand.Range(.75f, 1.25f) * d);
        DamageInfo dinfo = new DamageInfo(type, amt, 0, -1, pawn);
        if (e != null)
        {
            e.TakeDamage(dinfo);
        }
    }
}
