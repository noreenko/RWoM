using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace TorannMagic;

[StaticConstructorOnStartup]
public class FlyingObject_SpiritWolves : Projectile
{
    private float angle;
    Vector3 direction;
    IEnumerable<IntVec3> lastRadial;

    protected float speed = 20f;
    protected Thing assignedTarget;
    protected Thing flyingThing;
    Pawn pawn;

    public bool damageLaunched = true;
    public bool explosion;

    private int verVal;
    private int pwrVal;
    private float arcaneDmg = 1f;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref direction, "direction");
        Scribe_Values.Look(ref angle, "angle");
        Scribe_Values.Look(ref speed, "speed", 20);
        Scribe_Values.Look(ref damageLaunched, "damageLaunched", true);
        Scribe_Values.Look(ref explosion, "explosion");
        Scribe_References.Look(ref pawn, "pawn");
        Scribe_References.Look(ref assignedTarget, "assignedTarget");
        Scribe_References.Look(ref flyingThing, "flyingThing");
        Scribe_Values.Look(ref verVal, "verVal");
        Scribe_Values.Look(ref pwrVal, "pwrVal");
        Scribe_Values.Look(ref arcaneDmg, "arcaneDmg", 1f);
    }

    protected new int StartingTicksToImpact
    {
        get
        {
            int num = Mathf.RoundToInt((origin - destination).magnitude / (speed / 100f));
            return num < 1 ? 1 : num;
        }
    }

    protected new IntVec3 DestinationCell => new(destination);

    public override Vector3 DrawPos => ExactPosition;

    private void Initialize()
    {
        if (pawn == null) return;

        FleckMaker.Static(origin, Map, FleckDefOf.ExplosionFlash, 12f);
        SoundDefOf.Ambient_AltitudeWind.sustainFadeoutTime.Equals(30.0f);
        FleckMaker.ThrowDustPuff(origin, Map, Rand.Range(1.2f, 1.8f));
        GetVector();
        angle = (Quaternion.AngleAxis(90, Vector3.up) * direction).ToAngleFlat();
        CompAbilityUserMagic comp = pawn.GetCompAbilityUserMagic();
        if(comp is { IsMagicUser: true })
        {
            verVal = TM_Calc.GetSkillVersatilityLevel(pawn, TorannMagicDefOf.TM_SpiritWolves);
            pwrVal = TM_Calc.GetSkillPowerLevel(pawn, TorannMagicDefOf.TM_SpiritWolves);
            arcaneDmg = comp.arcaneDmg;
        }
    }

    public void GetVector()
    {
        Vector3 heading = (destination - ExactPosition);
        float distance = heading.magnitude;
        direction = heading / distance;
    }

    public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
    {
        Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
    }

    public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
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
        this.launcher = launcher;
        this.origin = origin;
        speed = def.projectile.speed;
        this.flyingThing = flyingThing;
        if (targ.Thing != null)
        {
            assignedTarget = targ.Thing;
        }
        destination = targ.Cell.ToVector3Shifted();
        ticksToImpact = StartingTicksToImpact;
        Initialize();
    }

    public override void Tick()
    {
        if (ticksToImpact >= 0 && Find.TickManager.TicksGame % 3 == 0)
        {
            IEnumerable<IntVec3> effectRadial = GenRadial.RadialCellsAround(ExactPosition.ToIntVec3(), 2 + (.2f * verVal), true);
            DrawEffects(effectRadial);
            DoEffects(effectRadial);
            lastRadial = effectRadial;
        }
        ticksToImpact--;
        Position = ExactPosition.ToIntVec3();
        bool flag = !ExactPosition.InBoundsWithNullCheck(Map);
        if (flag)
        {
            ticksToImpact++;
            Destroy();
        }
        else
        {
            if (ticksToImpact <= 0)
            {
                if (DestinationCell.InBoundsWithNullCheck(Map))
                {
                    Position = DestinationCell;
                }
                ImpactSomething();
            }
        }
    }

    public void DrawEffects(IEnumerable<IntVec3> effectRadial)
    {
        if (effectRadial != null && effectRadial.Count() > 0)
        {
            IntVec3 curCell = effectRadial.RandomElement();

            bool flag2 = Find.TickManager.TicksGame % 3 == 0;
            float fadeIn = .2f;
            float fadeOut = .25f;
            float solidTime = .05f;
            if (direction.ToAngleFlat() >= -135 && direction.ToAngleFlat() < -45)
            {
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_SpiritWolf_North, curCell.ToVector3(), Map, .8f, solidTime, fadeIn, fadeOut, 0, Rand.Range(5, 8), angle + Rand.Range(-20, 20), 0);
                if (flag2)
                {
                    IEnumerable<IntVec3> effectRadialSmall = GenRadial.RadialCellsAround(ExactPosition.ToIntVec3(), 1, true);
                    curCell = effectRadialSmall.RandomElement();
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_SpiritWolf_North, curCell.ToVector3(), Map, .8f, solidTime, fadeIn, fadeOut, 0, Rand.Range(10, 15), angle + Rand.Range(-20, 20), 0);
                }
            }
            else if (direction.ToAngleFlat() >= 45 && direction.ToAngleFlat() < 135)
            {
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_SpiritWolf_South, curCell.ToVector3(), Map, .8f, solidTime, fadeIn, fadeOut, 0, Rand.Range(5, 8), angle + Rand.Range(-20, 20), 0);
                if (flag2)
                {
                    IEnumerable<IntVec3> effectRadialSmall = GenRadial.RadialCellsAround(ExactPosition.ToIntVec3(), 1, true);
                    curCell = effectRadialSmall.RandomElement();
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_SpiritWolf_South, curCell.ToVector3(), Map, .8f, solidTime, fadeIn, fadeOut, 0, Rand.Range(10, 15), angle + Rand.Range(-20, 20), 0);
                }
            }
            else if (direction.ToAngleFlat() >= -45 && direction.ToAngleFlat() < 45)
            {
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_SpiritWolf_East, curCell.ToVector3(), Map, .8f, solidTime, fadeIn, fadeOut, 0, Rand.Range(5, 8), angle + Rand.Range(-20, 20), 0);
                if (flag2)
                {
                    IEnumerable<IntVec3> effectRadialSmall = GenRadial.RadialCellsAround(ExactPosition.ToIntVec3(), 1, true);
                    curCell = effectRadialSmall.RandomElement();
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_SpiritWolf_East, curCell.ToVector3(), Map, .8f, solidTime, fadeIn, fadeOut, 0, Rand.Range(10, 15), angle + Rand.Range(-20, 20), 0);
                }
            }
            else
            {
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_SpiritWolf_West, curCell.ToVector3(), Map, .8f, solidTime, fadeIn, fadeOut, 0, Rand.Range(5, 8), angle + Rand.Range(-20, 20), 0);
                if (flag2)
                {
                    IEnumerable<IntVec3> effectRadialSmall = GenRadial.RadialCellsAround(ExactPosition.ToIntVec3(), 1, true);
                    curCell = effectRadialSmall.RandomElement();
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_SpiritWolf_West, curCell.ToVector3(), Map, .8f, solidTime, fadeIn, fadeOut, 0, Rand.Range(10, 15), angle + Rand.Range(-20, 20), 0);
                }
            }

            if (lastRadial != null && lastRadial.Count() > 0)
            {
                curCell = lastRadial.RandomElement();
                if (curCell.InBoundsWithNullCheck(Map) && curCell.IsValid)
                {
                    ThingDef moteSmoke = TorannMagicDefOf.Mote_Base_Smoke;
                    if (Rand.Chance(.5f))
                    {
                        TM_MoteMaker.ThrowGenericMote(moteSmoke, curCell.ToVector3(), Map, Rand.Range(1.2f, 1.5f), moteSmoke.mote.solidTime, moteSmoke.mote.fadeInTime, moteSmoke.mote.fadeOutTime, Rand.Range(-2, 2), Rand.Range(.3f, .4f), direction.ToAngleFlat(), Rand.Range(0, 360));
                    }
                    else
                    {
                        TM_MoteMaker.ThrowGenericMote(moteSmoke, curCell.ToVector3(), Map, Rand.Range(1.2f, 1.5f), moteSmoke.mote.solidTime, moteSmoke.mote.fadeInTime, moteSmoke.mote.fadeOutTime, Rand.Range(-2, 2), Rand.Range(.3f, .4f), 180 + direction.ToAngleFlat(), Rand.Range(0, 360));
                    }
                }
            }
        }
    }

    public void DoEffects(IEnumerable<IntVec3> effectRadial1)
    {
        IEnumerable<IntVec3> effectRadial2 = GenRadial.RadialCellsAround(ExactPosition.ToIntVec3(), 1, true);
        IEnumerable<IntVec3> effectRadial = effectRadial1.Except(effectRadial2);
        IntVec3 curCell;
        List<Thing> hitList = new List<Thing>();
        bool shouldAddAbilities = false;
        bool addAbilities = false;
        if (pawn != null)
        {
            CompAbilityUserMagic comp = pawn.GetCompAbilityUserMagic();
            for (int i = 0; i < effectRadial.Count(); i++)
            {
                curCell = effectRadial.ToArray()[i];
                if (!curCell.InBoundsWithNullCheck(Map) || !curCell.IsValid) continue;

                hitList = curCell.GetThingList(Map);
                for (int j = 0; j < hitList.Count; j++)
                {
                    if (hitList[j] is not Pawn || hitList[j].Faction == pawn.Faction) continue;

                    DamageEntities(hitList[j]);
                    if (verVal < 3 || hitList[j].DestroyedOrNull() || !Rand.Chance(.5f)) continue;
                    if (comp == null) continue;

                    shouldAddAbilities = comp.HexedPawns.Count <= 0;
                    Pawn newPawn = hitList[j] as Pawn;
                    if (TM_Calc.IsUndead(newPawn) || !newPawn.RaceProps.IsFlesh || newPawn.Destroyed || newPawn.Dead) continue;
                    if (!Rand.Chance(.3f) || newPawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_HexHD)) continue;

                    HealthUtility.AdjustSeverity(newPawn, TorannMagicDefOf.TM_HexHD, 1f);
                    if (!comp.HexedPawns.Contains(newPawn))
                    {
                        comp.HexedPawns.Add(newPawn);
                    }
                    addAbilities = true;
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_Hex, newPawn.DrawPos, newPawn.Map, .6f, .1f, .2f, .2f, 0, 0, 0, 0);
                }
            }
            if (shouldAddAbilities && addAbilities)
            {
                comp.GiveAbility(TorannMagicDefOf.TM_Hex_CriticalFail);
                comp.GiveAbility(TorannMagicDefOf.TM_Hex_Pain);
                comp.GiveAbility(TorannMagicDefOf.TM_Hex_MentalAssault);
            }
        }
    }

    public void DamageEntities(Thing e)
    {
        int amt = Mathf.RoundToInt(Rand.Range(def.projectile.GetDamageAmount(1) * .9f, def.projectile.GetDamageAmount(1) * 1.4f) + pwrVal);
        DamageInfo dinfo = new DamageInfo(DamageDefOf.Stun, amt, 5, -1, pawn);
        DamageInfo dinfo2 = new DamageInfo(TMDamageDefOf.DamageDefOf.TM_Spirit, Mathf.RoundToInt(amt *.15f), 5, -1, pawn);
        bool flag = e != null;
        if (flag)
        {
            e.TakeDamage(dinfo);
            e.TakeDamage(dinfo2);
        }
    }

    public override void Draw()
    {
        if (flyingThing == null) return;
        Graphics.DrawMesh(MeshPool.plane10, DrawPos, ExactRotation, flyingThing.def.DrawMatSingle, 0);
        Comps_PostDraw();
    }

    protected override void ImpactSomething()
    {
        if (assignedTarget != null)
        {
            if (assignedTarget is Pawn p && p.GetPosture() != PawnPosture.Standing && (origin - destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.2f)
            {
                Impact(null, false);
            }
            else
            {
                Impact(assignedTarget, false);
            }
        }
        else
        {
            Impact(null, false);
        }
    }

    protected override void Impact(Thing hitThing, bool blockedByShield)
    {
        if (hitThing == null)
        {
            if (Position.GetThingList(Map).FirstOrDefault(x => x == assignedTarget) is Pawn hitPawn)
            {
                hitThing = hitPawn;
            }
        }
        Destroy();
    }
}
