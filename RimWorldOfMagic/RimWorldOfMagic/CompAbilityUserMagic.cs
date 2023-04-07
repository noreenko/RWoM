using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using AbilityUser;
using Verse;
using Verse.AI;
using Verse.Sound;
using AbilityUserAI;
using TorannMagic.Ideology;
using TorannMagic.ModOptions;
using TorannMagic.TMDefs;
using TorannMagic.Utils;

namespace TorannMagic
{
    [CompilerGenerated]
    [Serializable]
    public class CompAbilityUserMagic : CompAbilityUserTMBase
    {
        public const float TOLERANCE = Constants.TOLERANCE;
        public string LabelKey = "TM_Magic";

        public bool firstTick;
        public bool magicPowersInitialized;
        public bool magicPowersInitializedForColonist = true;
        private bool colonistPowerCheck = true;
        private int resMitigationDelay;
        private int damageMitigationDelay;
        private int damageMitigationDelayMS;
        public int magicXPRate = 1000;
        public int lastXPGain;
        
        private bool doOnce = true;
        private List<IntVec3> deathRing = new();
        public float weaponCritChance;
        public List<TM_EventRecords> magicUsed = new();

        public bool spell_Rain;
        public bool spell_Blink;
        public bool spell_Teleport;
        public bool spell_Heal;
        public bool spell_Heater;
        public bool spell_Cooler;
        public bool spell_DryGround;
        public bool spell_WetGround;
        public bool spell_ChargeBattery;
        public bool spell_SmokeCloud;
        public bool spell_Extinguish;
        public bool spell_EMP;
        public bool spell_Firestorm;
        public bool spell_Blizzard;
        public bool spell_SummonMinion;
        public bool spell_TransferMana;
        public bool spell_SiphonMana;
        public bool spell_RegrowLimb;
        public bool spell_EyeOfTheStorm;
        public bool spell_ManaShield;
        public bool spell_FoldReality;
        public bool spell_Resurrection;
        public bool spell_PowerNode;
        public bool spell_Sunlight;
        public bool spell_HolyWrath;
        public bool spell_LichForm;
        public bool spell_Flight;
        public bool spell_SummonPoppi;
        public bool spell_BattleHymn;
        public bool spell_CauterizeWound;
        public bool spell_FertileLands;
        public bool spell_SpellMending;
        public bool spell_ShadowStep;
        public bool spell_ShadowCall;
        public bool spell_Scorn;
        public bool spell_PsychicShock;
        public bool spell_SummonDemon;
        public bool spell_Meteor;
        public bool spell_Teach;
        public bool spell_OrbitalStrike;
        public bool spell_BloodMoon;
        public bool spell_EnchantedAura;
        public bool spell_Shapeshift;
        public bool spell_ShapeshiftDW;
        public bool spell_Blur;
        public bool spell_BlankMind;
        public bool spell_DirtDevil;
        public bool spell_MechaniteReprogramming;
        public bool spell_ArcaneBolt;
        public bool spell_LightningTrap;
        public bool spell_Invisibility;
        public bool spell_BriarPatch;
        public bool spell_Recall;
        public bool spell_MageLight;
        public bool spell_SnapFreeze;
        public bool spell_Ignite;
        public bool spell_CreateLight;
        public bool spell_EqualizeLight;
        public bool spell_HeatShield;

        private bool item_StaffOfDefender;

        public float maxMP = 1;
        public float mpRegenRate = 1;
        public float mpCost = 1;
        public float arcaneDmg = 1;

        public List<TM_ChaosPowers> chaosPowers = new();
        public TMAbilityDef mimicAbility = null;

        public IntVec3 earthSprites;
        public bool earthSpritesInArea;
        public Map earthSpriteMap;
        public int nextEarthSpriteAction;
        public int nextEarthSpriteMote;

        // Dismiss<T> instances are initialized during constructor because they must reference this
        public readonly DismissPawnList<TMPawnSummoned> summonedMinions;
        public readonly DismissPawnList<Pawn> supportedUndead;
        public readonly DismissList<Thing> summonedSentinels;
        public readonly DismissPawnList<Pawn> stoneskinPawns;
        public readonly DismissList<Thing> summonedLights;
        public readonly DismissList<Thing> summonedHeaters;
        public readonly DismissList<Thing> summonedCoolers;
        public readonly DismissList<Thing> summonedPowerNodes;
        public readonly DismissList<Thing> enchanterStones;
        public readonly DismissList<Thing> lightningTraps;
        public readonly DismissPawnList<Pawn> weaponEnchants;
        public readonly DismissThing<FlyingObject_LivingWall> livingWall;
        public readonly DismissValue<int> earthSpriteType;

        // Should probably combine these two variables into a class
        public readonly DismissThing<Pawn> bondedSpirit;
        public ThingDef guardianSpiritType;

        // This one should be combined into one list that contains Pawn + Def together
        // something like: public DismissPawnList<TM_Branding> brandings;
        public readonly DismissPawnList<Pawn> BrandPawns;
        public List<HediffDef> BrandDefs = new ();

        public Pawn soulBondPawn;
        public List<IntVec3> fertileLands = new();
        public Thing mageLightThing;
        public bool mageLightActive;
        public bool mageLightSet;
        public bool useTechnoBitToggle = true;
        public bool useTechnoBitRepairToggle = true;
        public Vector3 bitPosition = Vector3.zero;
        private bool bitFloatingDown = true;
        private float bitOffset = .45f;
        public int technoWeaponDefNum = -1;
        public Thing technoWeaponThing;
        public ThingDef technoWeaponThingDef;
        public QualityCategory technoWeaponQC = QualityCategory.Normal;
        public bool useElementalShotToggle = true;
        public Building overdriveBuilding;
        public int overdriveDuration;
        public float overdrivePowerOutput;
        public int overdriveFrequency = 100;
        public bool ArcaneForging;
        public Thing enchanterStone;
        public IncidentDef predictionIncidentDef;
        public int predictionTick;
        public int predictionHash;
        private List<Pawn> hexedPawns = new();
        //Recall fields
        //position, hediffs, needs, mana, manual recall bool, recall duration
        public IntVec3 recallPosition;
        public Map recallMap;
        public List<string> recallNeedDefnames;
        public List<float> recallNeedValues;
        public List<Hediff> recallHediffList;
        public List<float> recallHediffDefSeverityList;
        public List<int> recallHediffDefTicksRemainingList;
        public List<Hediff_Injury> recallInjuriesList;
        public bool recallSet;
        public int recallExpiration;
        public bool recallSpell;
        public FlyingObject_SpiritOfLight SoL;
        public bool sigilSurging;
        public bool sigilDraining;
        public int lastChaosTraditionTick;
        public ThingOwner<ThingWithComps> magicWardrobe;

        private static HashSet<ushort> magicTraitIndexes = new()
        {
            TorannMagicDefOf.Enchanter.index,
            TorannMagicDefOf.BloodMage.index,
            TorannMagicDefOf.Technomancer.index,
            TorannMagicDefOf.Geomancer.index,
            TorannMagicDefOf.Warlock.index,
            TorannMagicDefOf.Succubus.index,
            TorannMagicDefOf.Faceless.index,
            TorannMagicDefOf.InnerFire.index,
            TorannMagicDefOf.HeartOfFrost.index,
            TorannMagicDefOf.StormBorn.index,
            TorannMagicDefOf.Arcanist.index,
            TorannMagicDefOf.Paladin.index,
            TorannMagicDefOf.Summoner.index,
            TorannMagicDefOf.Druid.index,
            TorannMagicDefOf.Necromancer.index,
            TorannMagicDefOf.Lich.index,
            TorannMagicDefOf.Priest.index,
            TorannMagicDefOf.TM_Bard.index,
            TorannMagicDefOf.Chronomancer.index,
            TorannMagicDefOf.ChaosMage.index,
            TorannMagicDefOf.TM_Wanderer.index
        };

        public class ChainedMagicAbility
        {
            public ChainedMagicAbility(TMAbilityDef _ability, int _expirationTicks, bool _expires)
            {
                abilityDef = _ability;
                expirationTicks = _expirationTicks;
                expires = _expires;
            }
            public TMAbilityDef abilityDef;
            public int expirationTicks;
            public bool expires;
        }
        public List<ChainedMagicAbility> chainedAbilitiesList = new();

        private Effecter powerEffecter;
        private int powerModifier;
        private int maxPower = 10;
        private int previousHexedPawns;
        public int nextEntertainTick = -1;

        public ThingOwner<ThingWithComps> MagicWardrobe => magicWardrobe ??= new ThingOwner<ThingWithComps>();

        public List<TM_EventRecords> MagicUsed
        {
            get => magicUsed ??= new List<TM_EventRecords>();
            set => magicUsed = value;
        }

        public DismissPawnList<Pawn> StoneskinPawns
        {
            get
            {
                stoneskinPawns.Cleanup();
                return stoneskinPawns;
            }
        }

        public ThingDef GuardianSpiritType
        {
            get
            {
                if (guardianSpiritType != null) return guardianSpiritType;

                return guardianSpiritType = Rand.Value switch
                {
                    < .34f => TorannMagicDefOf.TM_SpiritBearR,
                    < .67f => TorannMagicDefOf.TM_SpiritMongooseR,
                    _ => TorannMagicDefOf.TM_SpiritCrowR
                };
            }
        }

        public CompAbilityUserMagic()
        {
            summonedMinions = new DismissPawnList<TMPawnSummoned>(this, TorannMagicDefOf.TM_DismissMinion);
            supportedUndead = new DismissPawnList<Pawn>(this, TorannMagicDefOf.TM_DismissUndead);
            summonedSentinels = new DismissList<Thing>(this, TorannMagicDefOf.TM_ShatterSentinel);
            stoneskinPawns = new DismissPawnList<Pawn>(this, TorannMagicDefOf.TM_DispelStoneskin);
            summonedLights = new DismissList<Thing>(this, TorannMagicDefOf.TM_DismissSunlight);
            summonedHeaters = new DismissList<Thing>(this, TorannMagicDefOf.TM_DismissHeater);
            summonedCoolers = new DismissList<Thing>(this, TorannMagicDefOf.TM_DismissCooler);
            summonedPowerNodes = new DismissList<Thing>(this, TorannMagicDefOf.TM_DismissPowerNode);
            enchanterStones = new DismissList<Thing>(this, TorannMagicDefOf.TM_DismissEnchanterStones);
            lightningTraps = new DismissList<Thing>(this, TorannMagicDefOf.TM_DismissLightningTrap);
            weaponEnchants = new DismissPawnList<Pawn>(this, TorannMagicDefOf.TM_DispelEnchantWeapon);
            BrandPawns = new DismissPawnList<Pawn>(this, TorannMagicDefOf.TM_DispelBranding);
            livingWall = new DismissThing<FlyingObject_LivingWall>(this, TorannMagicDefOf.TM_DispelLivingWall, null);
            bondedSpirit = new DismissThing<Pawn>(this, TorannMagicDefOf.TM_DismissGuardianSpirit, null);
            earthSpriteType = new DismissValue<int>(this, TorannMagicDefOf.TM_DismissEarthSprites, 0);
        }

        public bool HasTechnoBit
        {
            get
            {
                return IsMagicUser && MagicData.MagicPowersT.First(mp => mp.abilityDef == TorannMagicDefOf.TM_TechnoBit).learned;
            }
        }

        public bool HasTechnoWeapon
        {
            get
            {
                return IsMagicUser && MagicData.MagicPowersT.First(mp => mp.abilityDef == TorannMagicDefOf.TM_TechnoWeapon).learned;
            }
        }

        public int PowerModifier
        {
            get => powerModifier;
            set
            {
                TM_MoteMaker.ThrowSiphonMote(Pawn.DrawPos, Pawn.Map, 1f);
                powerModifier = Mathf.Clamp(value, 0, maxPower);

                if (powerModifier != 0 || powerEffecter == null) return;
                powerEffecter.Cleanup();
                powerEffecter = null;
            }
        }

        public float GetSkillDamage()
        {
            float result;
            float strFactor = 1f;
            if (IsMagicUser)
            {
                strFactor = arcaneDmg;
            }

            if (Pawn.equipment?.Primary != null)
            {
                if (Pawn.equipment.Primary.def.IsMeleeWeapon)
                {
                    result = TM_Calc.GetSkillDamage_Melee(Pawn, strFactor);
                    weaponCritChance = TM_Calc.GetWeaponCritChance(Pawn.equipment.Primary);
                }
                else
                {
                    result = TM_Calc.GetSkillDamage_Range(Pawn, strFactor);
                    weaponCritChance = 0f;
                }
            }
            else
            {
                result = Pawn.GetStatValue(StatDefOf.MeleeDPS, false) * strFactor;
            }

            return result;
        }

        private MagicData magicData;
        public MagicData MagicData
        {
            get
            {
                if (magicData == null && IsMagicUser)
                {
                    magicData = new MagicData(this);
                }
                return magicData;
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            powerEffecter?.Cleanup();
        }

        public List<Pawn> HexedPawns
        {
            get
            {
                hexedPawns.RemoveAll(static pawn =>
                    pawn?.health?.hediffSet == null || pawn.Destroyed || pawn.Dead 
                    || !pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_HexHD));
                return hexedPawns;
            }
        }

        public bool shouldDraw = true;
        public override void PostDraw()
        {
            if (shouldDraw && IsMagicUser)
            {
                if (Pawn.Faction.IsPlayer)
                {
                    if (Settings.Instance.AIFriendlyMarking)
                    {
                        if (!Pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless))
                        {
                            DrawMark();
                        }
                    }
                }
                else
                {
                    if (Settings.Instance.AIMarking)
                    {
                        if (!Pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless))
                        {
                            DrawMark();
                        }
                    }
                }

                // Go through all hediffs at once
                for (int i =  Pawn.health.hediffSet.hediffs.Count - 1; i >= 0; i--)
                {
                    HediffDef def = Pawn.health.hediffSet.hediffs[i].def;
                    if (def == TorannMagicDefOf.TM_TechnoBitHD
                        && MagicData.MagicPowersT.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_TechnoBit).learned)
                    {
                        DrawTechnoBit();
                    }

                    if (def == TorannMagicDefOf.TM_DemonScornHD
                        || def == TorannMagicDefOf.TM_DemonScornHD_I
                        || def == TorannMagicDefOf.TM_DemonScornHD_II
                        || def == TorannMagicDefOf.TM_DemonScornHD_III)
                    {
                        DrawScornWings();
                    }
                }

                if (mageLightActive)
                {
                    DrawMageLight();
                }

                Enchantment.CompEnchant compEnchant = Pawn.GetComp<Enchantment.CompEnchant>();

                if (IsMagicUser && compEnchant?.enchantingContainer != null && compEnchant.enchantingContainer.Count > 0)
                {
                    DrawEnchantMark();
                }
            }
            base.PostDraw();
        }


        private void DrawTechnoBit()
        {
            if (bitFloatingDown)
            {
                if (bitOffset < .38f)
                {
                    bitFloatingDown = false;
                }
                bitOffset -= .001f;
            }
            else
            {
                if (bitOffset > .57f)
                {
                    bitFloatingDown = true;
                }
                bitOffset += .001f;
            }

            bitPosition = Pawn.Drawer.DrawPos;
            bitPosition.x -= .5f + Rand.Range(-.01f, .01f);
            bitPosition.z += bitOffset;
            bitPosition.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            Vector3 s = new Vector3(.35f, 1f, .35f);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(bitPosition, Quaternion.AngleAxis(0f, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, TM_RenderQueue.bitMat, 0);
        }

        private void DrawMageLight()
        {
            if (mageLightSet) return;
            
            Vector3 lightPos = Vector3.zero;

            lightPos = Pawn.Drawer.DrawPos;
            lightPos.x -= .5f;
            lightPos.z += .6f;

            lightPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            float angle = Rand.Range(0, 360);
            Vector3 s = new Vector3(.27f, .5f, .27f);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(lightPos, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, TM_RenderQueue.mageLightMat, 0);
        }
        
        public void DrawEnchantMark()
        {
            DrawMark(TM_RenderQueue.enchantMark, new Vector3(.5f, 1f, .5f), 0, -.2f);
        }

        public void DrawScornWings()
        {
            if (Pawn.Dead || Pawn.Downed) return;

            Vector3 vector = Pawn.Drawer.DrawPos;
            vector.y = Pawn.Rotation == Rot4.North ? AltitudeLayer.PawnState.AltitudeFor() : AltitudeLayer.Pawn.AltitudeFor();
            Vector3 s = new Vector3(3f, 3f, 3f);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(0f, Vector3.up), s);
            if (Pawn.Rotation == Rot4.South || Pawn.Rotation == Rot4.North)
            {
                Graphics.DrawMesh(MeshPool.plane10, matrix, TM_RenderQueue.scornWingsNS, 0);
            }
            if (Pawn.Rotation == Rot4.East)
            {
                Graphics.DrawMesh(MeshPool.plane10, matrix, TM_RenderQueue.scornWingsE, 0);
            }
            if (Pawn.Rotation == Rot4.West)
            {
                Graphics.DrawMesh(MeshPool.plane10, matrix, TM_RenderQueue.scornWingsW, 0);
            }
        }

        public static List<TMAbilityDef> MagicAbilities = null;

        private void SingleEvent()
        {
            doOnce = false;
        }

        private void DoOncePerLoad()
        {
            if (spell_FertileLands)
            {
                if (fertileLands.Count > 0)
                {
                    List<IntVec3> cellList = Constants.GetGrowthCells();
                    if (cellList.Count != 0)
                    {
                        for (int i = 0; i < fertileLands.Count; i++)
                        {
                            Constants.RemoveGrowthCell(fertileLands[i]);
                        }
                    }
                    Constants.SetGrowthCells(fertileLands);
                    RemovePawnAbility(TorannMagicDefOf.TM_FertileLands);
                    AddPawnAbility(TorannMagicDefOf.TM_DismissFertileLands);
                }
            }
            //to fix filtering of succubus abilities
            if(Pawn.story.traits.HasTrait(TorannMagicDefOf.Succubus))
            {
                for(int i = 0; i < MagicData.MagicPowersWD.Count; i++)
                {
                    MagicPower wd = MagicData.MagicPowersWD[i];
                    if (wd.learned && wd.abilityDef == TorannMagicDefOf.TM_SoulBond)
                    {
                        MagicData.MagicPowersSD.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_SoulBond).learned = true;
                    }
                    else if(wd.learned && wd.abilityDef == TorannMagicDefOf.TM_ShadowBolt)
                    {
                        MagicData.MagicPowersSD.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_ShadowBolt).learned = true;
                    }
                    else if (wd.learned && wd.abilityDef == TorannMagicDefOf.TM_Dominate)
                    {
                        MagicData.MagicPowersSD.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_Dominate).learned = true;
                    }
                }
            }
        }

        public override void CompTick()
        {
            if (Pawn == null) return;
            if (!Pawn.Spawned)
            {
                if (!Pawn.IsHashIntervalTick(600) || Pawn.Map != null || !IsMagicUser) return;
                if (!(AbilityData?.AllPowers?.Count > 0)) return;

                foreach (PawnAbility allPower in AbilityData.AllPowers)
                {
                    allPower.CooldownTicksLeft -= 600;
                    if (allPower.CooldownTicksLeft <= 0)
                    {
                        allPower.CooldownTicksLeft = 0;
                    }
                }

                return;
            }

            if (IsMagicUser && !Pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless) && !Pawn.IsWildMan())
            {
                if (!firstTick)
                {
                    PostInitializeTick();
                }
                if (doOnce)
                {
                    SingleEvent();
                }
                base.CompTick();
                age++;
                if(chainedAbilitiesList is { Count: > 0 })
                {
                    for(int i = 0; i < chainedAbilitiesList.Count; i++)
                    {
                        chainedAbilitiesList[i].expirationTicks--;
                        if(chainedAbilitiesList[i].expires && chainedAbilitiesList[i].expirationTicks <= 0)
                        {
                            RemovePawnAbility(chainedAbilitiesList[i].abilityDef);
                            chainedAbilitiesList.Remove(chainedAbilitiesList[i]);
                            break;
                        }
                    }
                }
                if (Mana != null)
                {
                    if (Find.TickManager.TicksGame % 4 == 0 && Pawn.CurJob != null && Pawn.CurJobDef == JobDefOf.DoBill && Pawn.CurJob.targetA != null && Pawn.CurJob.targetA.Thing != null)
                    {
                        DoArcaneForging();
                    }
                    if (Mana.CurLevel >= .99f * Mana.MaxLevel)
                    {
                        if (age > lastXPGain + magicXPRate)
                        {
                            MagicData.MagicUserXP++;
                            lastXPGain = age;
                        }
                    }
                    if (Find.TickManager.TicksGame % 30 == 0)
                    {
                        if (MagicUserXP > MagicUserXPTillNextLevel)
                        {
                            LevelUp();
                        }
                    }
                    if (Find.TickManager.TicksGame % 60 == 0)
                    {
                        if (Pawn.IsColonist)
                        {
                            if (!magicPowersInitializedForColonist) ResolveFactionChange();

                            ResolveEnchantments();
                            summonedMinions.Cleanup();
                            ResolveSustainers();
                            supportedUndead.Cleanup();
                            ResolveEffecter();
                            ResolveClassSkills();
                            ResolveSpiritOfLight();
                            ResolveChronomancerTimeMark();
                        }
                        else
                        {
                            magicPowersInitializedForColonist = false;
                        }
                    }

                    if (autocastTick < Find.TickManager.TicksGame)  //180 default
                    {
                        if (!Pawn.Dead && !Pawn.Downed && Pawn.Map != null && Pawn.story?.traits != null && MagicData != null && AbilityData != null && !Pawn.InMentalState)
                        {
                            if (Pawn.IsColonist)
                            {
                                autocastTick = Find.TickManager.TicksGame + (int)Rand.Range(.8f * Settings.Instance.autocastEvaluationFrequency, 1.2f * Settings.Instance.autocastEvaluationFrequency);
                                ResolveAutoCast();
                            }
                            else if(Settings.Instance.AICasting && (!Pawn.IsPrisoner || Pawn.IsFighting()) && (Pawn.guest != null && !Pawn.IsSlave))
                            {
                                float tickMult = Settings.Instance.AIAggressiveCasting ? 1f : 2f;
                                autocastTick = Find.TickManager.TicksGame + (int)(Rand.Range(.75f * Settings.Instance.autocastEvaluationFrequency, 1.25f * Settings.Instance.autocastEvaluationFrequency) * tickMult);
                                ResolveAIAutoCast();
                            }
                        }
                    }
                    if (!Pawn.IsColonist && Settings.Instance.AICasting && Settings.Instance.AIAggressiveCasting && Find.TickManager.TicksGame > nextAICastAttemptTick) //Aggressive AI Casting
                    {
                        nextAICastAttemptTick = Find.TickManager.TicksGame + Rand.Range(300, 500);
                        if (Pawn.jobs != null && Pawn.CurJobDef != TorannMagicDefOf.TMCastAbilitySelf && Pawn.CurJobDef != TorannMagicDefOf.TMCastAbilityVerb)
                        {
                            IEnumerable<AbilityUserAIProfileDef> enumerable = Pawn.EligibleAIProfiles();
                            if (enumerable != null)
                            {
                                foreach (AbilityUserAIProfileDef item in enumerable)
                                {
                                    if (item == null) continue;

                                    AbilityAIDef useThisAbility = null;
                                    if (item.decisionTree != null)
                                    {
                                        useThisAbility = item.decisionTree.RecursivelyGetAbility(Pawn);
                                    }
                                    if (useThisAbility == null) continue;

                                    ThingComp val = Pawn.AllComps.First(comp => comp.GetType() == item.compAbilityUserClass);
                                    if (val is not CompAbilityUser compAbilityUser) continue;

                                    PawnAbility pawnAbility = compAbilityUser.AbilityData.AllPowers.First((PawnAbility ability) => ability.Def == useThisAbility.ability);
                                    if (pawnAbility.CanCastPowerCheck(AbilityContext.AI, out _))
                                    {
                                        LocalTargetInfo target = useThisAbility.Worker.TargetAbilityFor(useThisAbility, Pawn);
                                        if (target.IsValid)
                                        {
                                            pawnAbility.UseAbility(AbilityContext.Player, target);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (Find.TickManager.TicksGame % overdriveFrequency == 0)
                {
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Technomancer) || (TM_ClassUtility.ClassHasAbility(TorannMagicDefOf.TM_Overdrive)))
                    {
                        ResolveTechnomancerOverdrive();
                    }
                }
                if (Find.TickManager.TicksGame % 299 == 0) //cache weapon damage for tooltip and damage calculations
                {
                    weaponDamage = GetSkillDamage(); // TM_Calc.GetSkillDamage(Pawn);
                }
                if (Find.TickManager.TicksGame % 601 == 0)
                {
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Warlock))
                    {
                        ResolveWarlockEmpathy();
                    }
                }
                if (Find.TickManager.TicksGame % 602 == 0)
                {
                    ResolveMagicUseEvents();
                }
                if (Find.TickManager.TicksGame % 2001 == 0)
                {
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Succubus))
                    {
                        ResolveSuccubusLovin();
                    }
                }
                if (deathRetaliating)
                {
                    DoDeathRetaliation();
                }
                else if (Find.TickManager.TicksGame % 67 == 0 && !Pawn.IsColonist && Pawn.Downed)
                {
                    DoDeathRetaliation();
                }
            }
            else if(ModsConfig.IdeologyActive)
            {
                if(Find.TickManager.TicksGame % 2501 == 0 && Pawn.story != null && Pawn.story.traits.HasTrait(TorannMagicDefOf.TM_Gifted))
                {
                    if (!Pawn.Inspired && Pawn.CurJobDef == JobDefOf.LayDown && Rand.Chance(.025f))
                    {
                        Pawn.mindState.inspirationHandler.TryStartInspiration(TorannMagicDefOf.ID_ArcanePathways);
                    }
                }
            }
        }

        private int deathRetaliationDelayCount;
        public void DoDeathRetaliation()
        {
            if (!Pawn.Downed || Pawn.Map == null || Pawn.IsPrisoner || Pawn.Faction == null || !Pawn.Faction.HostileTo(Faction.OfPlayerSilentFail))
            {
                deathRetaliating = false;
                canDeathRetaliate = false;
                deathRetaliationDelayCount = 0;
            }
            if (canDeathRetaliate && deathRetaliating)
            {
                ticksTillRetaliation--;
                if (deathRing == null || deathRing.Count < 1)
                {
                    deathRing = TM_Calc.GetOuterRing(Pawn.Position, 1f, 2f);
                }
                if (Find.TickManager.TicksGame % 6 == 0)
                {
                    Vector3 moteVec = deathRing.RandomElement().ToVector3Shifted();
                    moteVec.x += Rand.Range(-.4f, .4f);
                    moteVec.z += Rand.Range(-.4f, .4f);
                    float angle = (Quaternion.AngleAxis(90, Vector3.up) * TM_Calc.GetVector(moteVec, Pawn.DrawPos)).ToAngleFlat();
                    ThingDef mote = TorannMagicDefOf.Mote_Psi_Grayscale;
                    mote.graphicData.color = Color.white;
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_Psi_Grayscale, moteVec, Pawn.Map, Rand.Range(.25f, .6f), .1f, .05f, .05f, 0, Rand.Range(4f, 6f), angle, angle);
                }
                if (ticksTillRetaliation <= 0)
                {
                    canDeathRetaliate = false;
                    deathRetaliating = false;
                    TM_Action.CreateMagicDeathEffect(Pawn, Pawn.Position);
                }
            }
            else if (canDeathRetaliate)
            {
                if (deathRetaliationDelayCount >= 20 && Rand.Value < .04f)
                {
                    
                    deathRetaliating = true;
                    ticksTillRetaliation = Mathf.RoundToInt(Rand.Range(400, 1200) * Settings.Instance.deathRetaliationDelayFactor);
                    deathRing = TM_Calc.GetOuterRing(Pawn.Position, 1f, 2f);
                }
                else
                {
                    deathRetaliationDelayCount++;
                }
            }
        }

        public void PostInitializeTick()
        {
            bool flag = Pawn != null;
            if (flag)
            {
                bool spawned = Pawn.Spawned;
                if (spawned)
                {
                    bool flag2 = Pawn.story != null;
                    if (flag2)
                    {
                        Trait t = Pawn.story.traits.GetTrait(TorannMagicDefOf.TM_Possessed);
                        if (t != null && !Pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_SpiritPossessionHD))
                        {
                            Pawn.story.traits.RemoveTrait(t);
                        }
                        else
                        {
                            firstTick = true;
                            Initialize();
                            ResolveMagicTab();
                            ResolveMana();
                            DoOncePerLoad();
                        }
                    }
                }
            }
        }

        public bool IsMagicUser
        {
            get
            {
                if (Pawn?.story == null) return false;

                if (customClass != null) return true;
                if (customClass == null && customIndex == -2)
                {
                    customIndex = TM_ClassUtility.CustomClassIndexOfBaseMageClass(Pawn.story.traits.allTraits);
                    if (customIndex >= 0)
                    {
                        TM_CustomClass foundCustomClass = TM_ClassUtility.CustomClasses[customIndex];
                        if (!foundCustomClass.isMage)
                        {
                            customIndex = -1;
                            return false;
                        }
                        else
                        {
                            customClass = foundCustomClass;
                            return true;
                        }
                    }
                }
                //if (Pawn.story.traits.allTraits.Any(t => magicTraitIndexes.Contains(t.def.index) 
                //|| TM_Calc.IsWanderer(Pawn) 
                //|| (AdvancedClasses != null && AdvancedClasses.Count > 0)))
                bool hasMagicTrait = false;
                for (int i = 0; i < Pawn.story.traits.allTraits.Count; i++)
                {
                    if (!magicTraitIndexes.Contains(Pawn.story.traits.allTraits[i].def.index)) continue;

                    hasMagicTrait = true;
                    break;
                }

                if (hasMagicTrait || TM_Calc.IsWanderer(Pawn) || AdvancedClasses.Count > 0)
                {
                    return true;
                }
                if(TM_Calc.HasAdvancedClass(Pawn))
                {
                    bool hasMageAdvClass = false;
                    foreach(TM_CustomClass cc in TM_ClassUtility.GetAdvancedClassesForPawn(Pawn))
                    {
                        if(cc.isMage)
                        {
                            AdvancedClasses.Add(cc);
                            hasMageAdvClass = true;
                        }
                    }
                    if(hasMageAdvClass)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private Dictionary<int, int> cacheXPFL = new Dictionary<int, int>();
        public int GetXPForLevel(int lvl)
        {
            if (!cacheXPFL.ContainsKey(lvl))
            {
                IntVec2 c1 = new IntVec2(0, 40); 
                IntVec2 c2 = new IntVec2(5, 30);
                IntVec2 c3 = new IntVec2(15, 20); 
                IntVec2 c4 = new IntVec2(30, 10);
                IntVec2 c5 = new IntVec2(200, 0);

                int val = 0;

                for (int i = 0; i < lvl + 1; i++)
                {
                    val += (Mathf.Clamp(i, c1.x, c2.x - 1) * c1.z) + c1.z;
                    if (i >= c2.x)
                    {
                        val += (Mathf.Clamp(i, c2.x, c3.x - 1) * c2.z) + c2.z;
                    }
                    if (i >= c3.x)
                    {
                        val += (Mathf.Clamp(i, c3.x, c4.x - 1) * c3.z) + c3.z;
                    }
                    if (i >= c4.x)
                    {
                        val += (Mathf.Clamp(i, c4.x, c5.x - 1) * c4.z) + c4.z;
                    }
                }
                cacheXPFL.Add(lvl, val);
            }
            if (cacheXPFL.ContainsKey(lvl))
            {
                return cacheXPFL[lvl];
            }
            else
            {
                return 0;
            }
        }

        public int MagicUserLevel
        {
            get => MagicData.MagicUserLevel;
            set
            {
                if (value > MagicData.MagicUserLevel)
                {
                    MagicData.MagicAbilityPoints++;
                    if (MagicData.MagicUserXP < GetXPForLevel(value - 1))
                    {
                        MagicData.MagicUserXP = GetXPForLevel(value - 1);
                    }
                }
                MagicData.MagicUserLevel = value;
            }
        }

        public int MagicUserXP
        {
            get => MagicData.MagicUserXP;
            set => MagicData.MagicUserXP = value;
        }
        
        public float XPLastLevel => MagicUserLevel > 0 ? GetXPForLevel(MagicUserLevel - 1) : 0f;
        public float XPTillNextLevelPercent => (MagicData.MagicUserXP - XPLastLevel) / (MagicUserXPTillNextLevel - XPLastLevel);

        public int MagicUserXPTillNextLevel
        {
            get
            {
                if(MagicUserXP < XPLastLevel)
                {
                    MagicUserXP = (int)XPLastLevel;
                }
                return GetXPForLevel(MagicUserLevel);
            }
        }

        public void LevelUp(bool hideNotification = false)
        {
            if (!(Pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless) || Pawn.story.traits.HasTrait(TorannMagicDefOf.TM_Wayfarer)))
            {
                if (MagicUserLevel < (customClass?.maxMageLevel ?? 200))
                {
                    MagicUserLevel++;
                    if (hideNotification) return;

                    if (Pawn.IsColonist && Settings.Instance.showLevelUpMessage)
                    {
                        Messages.Message("TM_MagicLevelUp".Translate(parent.Label),
                            Pawn, MessageTypeDefOf.PositiveEvent);
                    }
                }
                else
                {
                    MagicUserXP = (int)XPLastLevel;
                }
            }
        }

        public void LevelUpPower(MagicPower power)
        {
            foreach (AbilityUser.AbilityDef current in power.TMabilityDefs)
            {
                RemovePawnAbility(current);
            }
            power.level++;
            AddPawnAbility(power.abilityDef);
            UpdateAbilities();
        }

        public Need_Mana Mana
        {
            get
            {
                if (!Pawn.DestroyedOrNull() && !Pawn.Dead)
                {
                    return Pawn.needs.TryGetNeed<Need_Mana>();
                }
                return null;
            }
        }

        public void ResolveFactionChange()
        {
            if (!colonistPowerCheck)
            {
                RemovePowers();
                spell_BattleHymn = false;
                RemovePawnAbility(TorannMagicDefOf.TM_BattleHymn);
                spell_Blizzard = false;
                RemovePawnAbility(TorannMagicDefOf.TM_Blizzard);
                spell_BloodMoon = false;
                RemovePawnAbility(TorannMagicDefOf.TM_BloodMoon);
                spell_EyeOfTheStorm = false;
                RemovePawnAbility(TorannMagicDefOf.TM_EyeOfTheStorm);
                spell_Firestorm = false;
                RemovePawnAbility(TorannMagicDefOf.TM_Firestorm);
                spell_FoldReality = false;
                RemovePawnAbility(TorannMagicDefOf.TM_FoldReality);
                spell_HolyWrath = false;
                RemovePawnAbility(TorannMagicDefOf.TM_HolyWrath);
                spell_LichForm = false;
                RemovePawnAbility(TorannMagicDefOf.TM_BattleHymn);
                spell_Meteor = false;
                RemovePawnAbility(TorannMagicDefOf.TM_Meteor);
                spell_OrbitalStrike = false;
                RemovePawnAbility(TorannMagicDefOf.TM_OrbitalStrike);
                spell_PsychicShock = false;
                RemovePawnAbility(TorannMagicDefOf.TM_PsychicShock);
                spell_RegrowLimb = false;
                spell_Resurrection = false;
                spell_Scorn = false;
                RemovePawnAbility(TorannMagicDefOf.TM_Scorn);
                spell_Shapeshift = false;
                spell_SummonPoppi = false;
                RemovePawnAbility(TorannMagicDefOf.TM_SummonPoppi);
                RemovePawnAbility(TorannMagicDefOf.TM_Recall);
                spell_Recall = false;
                RemovePawnAbility(TorannMagicDefOf.TM_LightSkipGlobal);
                RemovePawnAbility(TorannMagicDefOf.TM_LightSkipMass);
                RemovePawnAbility(TorannMagicDefOf.TM_SpiritOfLight);
                AssignAbilities();
            }
            magicPowersInitializedForColonist = true;
            colonistPowerCheck = true;
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
            if (MagicAbilities == null)
            {
                if (magicPowersInitialized == false && MagicData != null)
                {
                    MagicData.MagicUserLevel = 0;
                    MagicData.MagicAbilityPoints = 0;
                    AssignAbilities();
                    if (!Pawn.IsColonist)
                    {
                        InitializeSpell();
                        colonistPowerCheck = false;
                    }
                }
                magicPowersInitialized = true;
                UpdateAbilities();
            }
        }

        private void AssignAbilities()
        {
            
            float hardModeMasterChance = .35f;
            float masterChance = .05f;
            Pawn abilityUser = Pawn;
            bool flag2;
            List<TMAbilityDef> usedAbilities = new List<TMAbilityDef>();
            if (abilityUser?.story?.traits == null) return;

            if (customClass != null)
            {
                for (int z = 0; z < MagicData.AllMagicPowers.Count; z++)
                {
                    TMAbilityDef ability = (TMAbilityDef)MagicData.AllMagicPowers[z].abilityDef;
                    if (usedAbilities.Contains(ability)) continue;

                    usedAbilities.Add(ability);
                    if (customClass.classMageAbilities.Contains(ability))
                    {
                        MagicData.AllMagicPowers[z].learned = true;
                    }
                    //if (MagicData.AllMagicPowers[z] == MagicData.MagicPowersWD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SoulBond) ||
                    //MagicData.AllMagicPowers[z] == MagicData.MagicPowersWD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ShadowBolt) ||
                    //MagicData.AllMagicPowers[z] == MagicData.MagicPowersWD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Dominate))
                    //{
                    //    MagicData.AllMagicPowers[z].learned = false;
                    //}
                    if (MagicData.AllMagicPowers[z].requiresScroll)
                    {
                        MagicData.AllMagicPowers[z].learned = false;
                    }
                    if (!abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty) && !Rand.Chance(ability.learnChance))
                    {
                        MagicData.AllMagicPowers[z].learned = false;
                    }
                    if (MagicData.AllMagicPowers[z].learned)
                    {
                        if (ability.shouldInitialize)
                        {
                            AddPawnAbility(ability);
                        }
                        if (ability.childAbilities is { Count: > 0 })
                        {
                            for (int c = 0; c < ability.childAbilities.Count; c++)
                            {
                                if (ability.childAbilities[c].shouldInitialize)
                                {
                                    AddPawnAbility(ability.childAbilities[c]);
                                }
                            }
                        }
                    }
                }
                MagicPower branding = MagicData.AllMagicPowers.FirstOrDefault((MagicPower p) => p.abilityDef == TorannMagicDefOf.TM_Branding);
                if(branding is { learned: true } && abilityUser.story.traits.HasTrait(TorannMagicDefOf.TM_Golemancer))
                {
                    int count = 0;
                    while (count < 2)
                    {
                        TMAbilityDef tmpAbility = TM_Data.BrandList().RandomElement();
                        for (int i = 0; i < MagicData.AllMagicPowers.Count; i++)
                        {
                            TMAbilityDef ad = (TMAbilityDef)MagicData.AllMagicPowers[i].abilityDef;
                            if (!MagicData.AllMagicPowers[i].learned && ad == tmpAbility)
                            {
                                count++;
                                MagicData.AllMagicPowers[i].learned = true;
                                RemovePawnAbility(ad);
                                TryAddPawnAbility(ad);
                            }
                        }
                    }
                }
                if (customClass.classHediff != null)
                {
                    HealthUtility.AdjustSeverity(abilityUser, customClass.classHediff, customClass.hediffSeverity);
                }
            }
            else
            {
                //for (int z = 0; z < MagicData.AllMagicPowers.Count; z++)
                //{
                //    MagicData.AllMagicPowers[z].learned = false;
                //}
                flag2 = TM_Calc.IsWanderer(abilityUser);
                if (flag2)
                {
                    //Log.Message("Initializing Wanderer Abilities");
                    MagicData.ReturnMatchingMagicPower(TorannMagicDefOf.TM_Cantrips).learned = true;
                    magicData.ReturnMatchingMagicPower(TorannMagicDefOf.TM_WandererCraft).learned = true;
                    for (int i = 0; i < 3; i++)
                    {
                        MagicPower mp = MagicData.MagicPowersStandalone.RandomElement();
                        if (mp.abilityDef == TorannMagicDefOf.TM_TransferMana)
                        {
                            mp.learned = true;
                            spell_TransferMana = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_SiphonMana)
                        {
                            mp.learned = true;
                            spell_SiphonMana = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_SpellMending)
                        {
                            mp.learned = true;
                            spell_SpellMending = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_DirtDevil)
                        {
                            mp.learned = true;
                            spell_DirtDevil = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_Heater)
                        {
                            mp.learned = true;
                            spell_Heater = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_Cooler)
                        {
                            mp.learned = true;
                            spell_Cooler = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_PowerNode)
                        {
                            mp.learned = true;
                            spell_PowerNode = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_Sunlight)
                        {
                            mp.learned = true;
                            spell_Sunlight = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_SmokeCloud)
                        {
                            mp.learned = true;
                            spell_SmokeCloud = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_Extinguish)
                        {
                            mp.learned = true;
                            spell_Extinguish = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_EMP)
                        {
                            mp.learned = true;
                            spell_EMP = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_ManaShield)
                        {
                            mp.learned = true;
                            spell_ManaShield = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_Blur)
                        {
                            mp.learned = true;
                            spell_Blur = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_ArcaneBolt)
                        {
                            mp.learned = true;
                            spell_ArcaneBolt = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_LightningTrap)
                        {
                            mp.learned = true;
                            spell_LightningTrap = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_Invisibility)
                        {
                            mp.learned = true;
                            spell_Invisibility = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_MageLight)
                        {
                            mp.learned = true;
                            spell_MageLight = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_Ignite)
                        {
                            mp.learned = true;
                            spell_Ignite = true;
                        }
                        else if (mp.abilityDef == TorannMagicDefOf.TM_SnapFreeze)
                        {
                            mp.learned = true;
                            spell_SnapFreeze = true;
                        }
                        else
                        {
                            int rnd = Rand.RangeInclusive(0, 4);
                            switch (rnd)
                            {
                                case 0:
                                    MagicData.MagicPowersP.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Heal).learned = true;
                                    spell_Heal = true;
                                    break;
                                case 1:
                                    MagicData.MagicPowersA.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Blink).learned = true;
                                    spell_Blink = true;
                                    break;
                                case 2:
                                    MagicData.MagicPowersHoF.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Rainmaker).learned = true;
                                    spell_Rain = true;
                                    break;
                                case 3:
                                    MagicData.MagicPowersS.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SummonMinion).learned = true;
                                    spell_SummonMinion = true;
                                    break;
                                case 4:
                                    MagicData.MagicPowersA.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Teleport).learned = true;
                                    spell_Teleport = true;
                                    break;
                            }
                        }
                    }
                    if (!abilityUser.IsColonist)
                    {
                        spell_ArcaneBolt = true;
                        AddPawnAbility(TorannMagicDefOf.TM_ArcaneBolt);
                    }
                    InitializeSpell();
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.InnerFire);
                if (flag2)
                {
                    //Log.Message("Initializing Inner Fire Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_RayofHope);
                            MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_RayofHope).learned = true;
                        }
                        if (Rand.Chance(.6f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Firebolt);
                            MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Firebolt).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Fireclaw);
                            MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Fireclaw).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Fireball);
                            MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Fireball).learned = true;
                        }
                        MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Firestorm).learned = false;
                    }
                    else
                    {
                        MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_RayofHope).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_RayofHope);
                        MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Firebolt).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_Firebolt);
                        MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Fireclaw).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_Fireclaw);
                        MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Fireball).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_Fireball);
                        MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Firestorm).learned = false;

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                spell_Firestorm = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.HeartOfFrost);
                if (flag2)
                {
                    //Log.Message("Initializing Heart of Frost Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Soothe);
                            MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Soothe).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Icebolt);
                            MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Icebolt).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Snowball);
                            MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Snowball).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_FrostRay);
                            MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_FrostRay).learned = true;
                        }
                        if (Rand.Chance(.7f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Rainmaker);
                            MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Rainmaker).learned = true;
                            spell_Rain = true;
                        }
                        MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Blizzard).learned = false;
                    }
                    else
                    {
                        MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Soothe).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_Soothe);
                        MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Icebolt).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_Icebolt);
                        MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Snowball).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_Snowball);
                        MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_FrostRay).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_FrostRay);
                        MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Rainmaker).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_Rainmaker);
                        spell_Rain = true;
                        MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Blizzard).learned = false;

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                spell_Blizzard = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.StormBorn);
                if (flag2)
                {
                    //Log.Message("Initializing Storm Born Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_AMP);
                            MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_AMP).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_LightningBolt);
                            MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_LightningBolt).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_LightningCloud);
                            MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_LightningCloud).learned = true;
                        }
                        if (Rand.Chance(.2f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_LightningStorm);
                            MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_LightningStorm).learned = true;
                        }
                        MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EyeOfTheStorm).learned = false;
                    }
                    else
                    {
                        AddPawnAbility(TorannMagicDefOf.TM_AMP);
                        MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_AMP).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_LightningBolt);
                        MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_LightningBolt).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_LightningCloud);
                        MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_LightningCloud).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_LightningStorm);
                        MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_LightningStorm).learned = true;
                        MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EyeOfTheStorm).learned = false;

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                spell_EyeOfTheStorm = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Arcanist);
                if (flag2)
                {
                    //Log.Message("Initializing Arcane Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Shadow);
                            MagicData.MagicPowersA.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Shadow).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_MagicMissile);
                            MagicData.MagicPowersA.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_MagicMissile).learned = true;
                        }
                        if (Rand.Chance(.7f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Blink);
                            MagicData.MagicPowersA.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Blink).learned = true;
                            spell_Blink = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Summon);
                            MagicData.MagicPowersA.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Summon).learned = true;
                        }
                        if (Rand.Chance(.2f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Teleport);
                            MagicData.MagicPowersA.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Teleport).learned = true;
                            spell_Teleport = true;
                        }
                        MagicData.MagicPowersA.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_FoldReality).learned = false;
                    }
                    else
                    {
                        for(int i = 0; i < MagicData.MagicPowersA.Count; i++)
                        {
                            if (MagicData.magicPowerA[i].abilityDef != TorannMagicDefOf.TM_FoldReality)
                            {
                                MagicData.MagicPowersA[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_Shadow);
                        AddPawnAbility(TorannMagicDefOf.TM_MagicMissile);
                        AddPawnAbility(TorannMagicDefOf.TM_Blink);
                        AddPawnAbility(TorannMagicDefOf.TM_Summon);
                        AddPawnAbility(TorannMagicDefOf.TM_Teleport);  //Pending Redesign (graphics?)
                        spell_Blink = true;
                        spell_Teleport = true;

                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Paladin);
                if (flag2)
                {
                    //Log.Message("Initializing Paladin Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if(Rand.Chance(TorannMagicDefOf.TM_P_RayofHope.learnChance))
                        {
                            MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_P_RayofHope).learned = true;
                            AddPawnAbility(TorannMagicDefOf.TM_P_RayofHope);
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Heal);
                            MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Heal).learned = true;
                            spell_Heal = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Shield);
                            MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Shield).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_ValiantCharge);
                            MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ValiantCharge).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Overwhelm);
                            MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Overwhelm).learned = true;
                        }
                        MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_HolyWrath).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersP.Count; i++)
                        {
                            if (MagicData.MagicPowersP[i].abilityDef != TorannMagicDefOf.TM_HolyWrath)
                            {
                                MagicData.MagicPowersP[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_Heal);
                        AddPawnAbility(TorannMagicDefOf.TM_Shield);
                        AddPawnAbility(TorannMagicDefOf.TM_ValiantCharge);
                        AddPawnAbility(TorannMagicDefOf.TM_Overwhelm);
                        AddPawnAbility(TorannMagicDefOf.TM_P_RayofHope);
                        spell_Heal = true;

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                spell_HolyWrath = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Summoner);
                if (flag2)
                {
                    //Log.Message("Initializing Summoner Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_SummonMinion);
                            MagicData.MagicPowersS.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SummonMinion).learned = true;
                            spell_SummonMinion = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_SummonPylon);
                            MagicData.MagicPowersS.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SummonPylon).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_SummonExplosive);
                            MagicData.MagicPowersS.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SummonExplosive).learned = true;
                        }
                        if (Rand.Chance(.2f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_SummonElemental);
                            MagicData.MagicPowersS.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SummonElemental).learned = true;
                        }
                        MagicData.MagicPowersS.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SummonPoppi).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersS.Count; i++)
                        {
                            if (MagicData.MagicPowersS[i].abilityDef != TorannMagicDefOf.TM_SummonPoppi)
                            {
                                MagicData.MagicPowersS[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_SummonMinion);
                        AddPawnAbility(TorannMagicDefOf.TM_SummonPylon);
                        AddPawnAbility(TorannMagicDefOf.TM_SummonExplosive);
                        AddPawnAbility(TorannMagicDefOf.TM_SummonElemental);
                        spell_SummonMinion = true;

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                spell_SummonPoppi = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Druid);
                if (flag2)
                {
                    // Log.Message("Initializing Druid Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.6f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Poison);
                            MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Poison).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_SootheAnimal);
                            MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SootheAnimal).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Regenerate);
                            MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Regenerate).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_CureDisease);
                            MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_CureDisease).learned = true;
                        }
                        MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_RegrowLimb).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersD.Count; i++)
                        {
                            if (MagicData.MagicPowersD[i].abilityDef != TorannMagicDefOf.TM_RegrowLimb)
                            {
                                MagicData.MagicPowersD[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_Poison);
                        AddPawnAbility(TorannMagicDefOf.TM_SootheAnimal);
                        AddPawnAbility(TorannMagicDefOf.TM_Regenerate);
                        AddPawnAbility(TorannMagicDefOf.TM_CureDisease);
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Necromancer) || abilityUser.story.traits.HasTrait(TorannMagicDefOf.Lich);
                if (flag2)
                {
                    //Log.Message("Initializing Necromancer Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_RaiseUndead);
                            MagicData.MagicPowersN.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_RaiseUndead).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_DeathMark);
                            MagicData.MagicPowersN.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_DeathMark).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_FogOfTorment);
                            MagicData.MagicPowersN.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_FogOfTorment).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_ConsumeCorpse);
                            MagicData.MagicPowersN.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ConsumeCorpse).learned = true;
                        }
                        if (Rand.Chance(.2f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_CorpseExplosion);
                            MagicData.MagicPowersN.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_CorpseExplosion).learned = true;
                        }
                        MagicData.MagicPowersN.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_LichForm).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersN.Count; i++)
                        {
                            if (MagicData.MagicPowersN[i].abilityDef != TorannMagicDefOf.TM_LichForm)
                            {
                                MagicData.MagicPowersN[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_RaiseUndead);
                        AddPawnAbility(TorannMagicDefOf.TM_DeathMark);
                        AddPawnAbility(TorannMagicDefOf.TM_FogOfTorment);
                        AddPawnAbility(TorannMagicDefOf.TM_ConsumeCorpse);
                        AddPawnAbility(TorannMagicDefOf.TM_CorpseExplosion);

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                AddPawnAbility(TorannMagicDefOf.TM_DeathBolt);
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Priest);
                if (flag2)
                {
                    //Log.Message("Initializing Priest Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_AdvancedHeal);
                            MagicData.MagicPowersPR.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_AdvancedHeal).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Purify);
                            MagicData.MagicPowersPR.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Purify).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_HealingCircle);
                            MagicData.MagicPowersPR.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_HealingCircle).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_BestowMight);
                            MagicData.MagicPowersPR.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_BestowMight).learned = true;
                        }
                        MagicData.MagicPowersPR.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Resurrection).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersPR.Count; i++)
                        {
                            if (MagicData.MagicPowersPR[i].abilityDef != TorannMagicDefOf.TM_Resurrection)
                            {
                                MagicData.MagicPowersPR[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_AdvancedHeal);
                        AddPawnAbility(TorannMagicDefOf.TM_Purify);
                        AddPawnAbility(TorannMagicDefOf.TM_HealingCircle);
                        AddPawnAbility(TorannMagicDefOf.TM_BestowMight);
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.TM_Bard);
                if (flag2)
                {
                    //Log.Message("Initializing Priest Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (true)
                        {
                            MagicData.MagicPowersB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_BardTraining).learned = true;
                            MagicData.MagicPowersB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Inspire).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Entertain);
                            MagicData.MagicPowersB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Entertain).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Lullaby);
                            MagicData.MagicPowersB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Lullaby).learned = true;
                        }
                        MagicData.MagicPowersB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_BattleHymn).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersB.Count; i++)
                        {
                            if (MagicData.MagicPowersB[i].abilityDef != TorannMagicDefOf.TM_BattleHymn)
                            {
                                MagicData.MagicPowersB[i].learned = true;
                            }
                        }
                        //AddPawnAbility(TorannMagicDefOf.TM_BardTraining);
                        AddPawnAbility(TorannMagicDefOf.TM_Entertain);
                        //AddPawnAbility(TorannMagicDefOf.TM_Inspire);
                        AddPawnAbility(TorannMagicDefOf.TM_Lullaby);

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                spell_BattleHymn = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Succubus);
                if (flag2)
                {
                    //Log.Message("Initializing Succubus Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.7f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_SoulBond);
                            MagicData.MagicPowersSD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SoulBond).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_ShadowBolt);
                            MagicData.MagicPowersSD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ShadowBolt).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Dominate);
                            MagicData.MagicPowersSD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Dominate).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Attraction);
                            MagicData.MagicPowersSD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Attraction).learned = true;
                        }
                        MagicData.MagicPowersSD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Scorn).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersSD.Count; i++)
                        {
                            if (MagicData.MagicPowersSD[i].abilityDef != TorannMagicDefOf.TM_Scorn)
                            {
                                MagicData.MagicPowersSD[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_SoulBond);
                        AddPawnAbility(TorannMagicDefOf.TM_ShadowBolt);
                        AddPawnAbility(TorannMagicDefOf.TM_Dominate);
                        AddPawnAbility(TorannMagicDefOf.TM_Attraction);

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                spell_Scorn = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Warlock);
                if (flag2)
                {
                    //Log.Message("Initializing Succubus Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.7f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_SoulBond);
                            MagicData.MagicPowersWD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SoulBond).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_ShadowBolt);
                            MagicData.MagicPowersWD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ShadowBolt).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Dominate);
                            MagicData.MagicPowersWD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Dominate).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Repulsion);
                            MagicData.MagicPowersWD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Repulsion).learned = true;
                        }
                        MagicData.MagicPowersWD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_PsychicShock).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersWD.Count; i++)
                        {
                            if (MagicData.MagicPowersWD[i].abilityDef != TorannMagicDefOf.TM_PsychicShock)
                            {
                                MagicData.MagicPowersWD[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_SoulBond);
                        AddPawnAbility(TorannMagicDefOf.TM_ShadowBolt);
                        AddPawnAbility(TorannMagicDefOf.TM_Dominate);
                        AddPawnAbility(TorannMagicDefOf.TM_Repulsion);
                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                spell_PsychicShock = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Geomancer);
                if (flag2)
                {
                    //Log.Message("Initializing Heart of Geomancer Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Stoneskin);
                            MagicData.MagicPowersG.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Stoneskin).learned = true;
                        }
                        if (Rand.Chance(.6f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Encase);
                            MagicData.MagicPowersG.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Encase).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_EarthSprites);
                            MagicData.MagicPowersG.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EarthSprites).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_EarthernHammer);
                            MagicData.MagicPowersG.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EarthernHammer).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Sentinel);
                            MagicData.MagicPowersG.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Sentinel).learned = true;
                        }
                        MagicData.MagicPowersG.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Meteor).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersG.Count; i++)
                        {
                            if (!MagicData.MagicPowersG[i].abilityDef.defName.StartsWith("TM_Meteor"))
                            {
                                MagicData.MagicPowersG[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_Stoneskin);
                        AddPawnAbility(TorannMagicDefOf.TM_Encase);
                        AddPawnAbility(TorannMagicDefOf.TM_EarthSprites);
                        AddPawnAbility(TorannMagicDefOf.TM_EarthernHammer);
                        AddPawnAbility(TorannMagicDefOf.TM_Sentinel);

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                AddPawnAbility(TorannMagicDefOf.TM_Meteor);
                                spell_Meteor = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Technomancer);
                if (flag2)
                {
                    //Log.Message("Initializing Technomancer Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_TechnoShield);
                            MagicData.MagicPowersT.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_TechnoShield).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Sabotage);
                            MagicData.MagicPowersT.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Sabotage).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Overdrive);
                            MagicData.MagicPowersT.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Overdrive).learned = true;
                        }
                        MagicData.MagicPowersT.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_OrbitalStrike).learned = false;
                        if (Rand.Chance(.2f))
                        {
                            spell_OrbitalStrike = true;
                            MagicData.MagicPowersT.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_OrbitalStrike).learned = true;
                            InitializeSpell();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersT.Count; i++)
                        {
                            MagicData.MagicPowersT[i].learned = true;
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_TechnoShield);
                        AddPawnAbility(TorannMagicDefOf.TM_Sabotage);
                        AddPawnAbility(TorannMagicDefOf.TM_Overdrive);
                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                spell_OrbitalStrike = true;
                            }
                        }
                    }
                    MagicData.MagicPowersT.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_TechnoBit).learned = false;
                    MagicData.MagicPowersT.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_TechnoTurret).learned = false;
                    MagicData.MagicPowersT.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_TechnoWeapon).learned = false;
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.BloodMage);
                if (flag2)
                {
                    //Log.Message("Initializing Heart of Frost Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(1f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_BloodGift);
                            MagicData.MagicPowersBM.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_BloodGift).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_IgniteBlood);
                            MagicData.MagicPowersBM.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_IgniteBlood).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_BloodForBlood);
                            MagicData.MagicPowersBM.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_BloodForBlood).learned = true;
                        }
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_BloodShield);
                            MagicData.MagicPowersBM.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_BloodShield).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Rend);
                            MagicData.MagicPowersBM.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Rend).learned = true;
                        }
                        MagicData.MagicPowersBM.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_BloodMoon).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersBM.Count; i++)
                        {
                            if (!MagicData.MagicPowersBM[i].abilityDef.defName.StartsWith("TM_BloodMoon"))
                            {
                                MagicData.MagicPowersBM[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_BloodGift);
                        AddPawnAbility(TorannMagicDefOf.TM_IgniteBlood);
                        AddPawnAbility(TorannMagicDefOf.TM_BloodForBlood);
                        AddPawnAbility(TorannMagicDefOf.TM_BloodShield);
                        AddPawnAbility(TorannMagicDefOf.TM_Rend);
                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                AddPawnAbility(TorannMagicDefOf.TM_BloodMoon);
                                spell_BloodMoon = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Enchanter);
                if (flag2)
                {
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.5f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_EnchantedBody);
                            AddPawnAbility(TorannMagicDefOf.TM_EnchantedAura);
                            MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EnchantedAura).learned = true;
                            MagicData.MagicPowersE.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EnchantedBody).learned = true;
                            spell_EnchantedAura = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Transmutate);
                            MagicData.MagicPowersE.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Transmutate).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_EnchanterStone);
                            MagicData.MagicPowersE.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EnchanterStone).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_EnchantWeapon);
                            MagicData.MagicPowersE.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EnchantWeapon).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Polymorph);
                            MagicData.MagicPowersE.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Polymorph).learned = true;
                        }
                        MagicData.MagicPowersE.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Shapeshift).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersE.Count; i++)
                        {
                            if (MagicData.MagicPowersE[i].abilityDef != TorannMagicDefOf.TM_Shapeshift)
                            {
                                MagicData.MagicPowersE[i].learned = true;
                            }
                        }
                        MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EnchantedAura).learned = true;
                        AddPawnAbility(TorannMagicDefOf.TM_EnchantedBody);
                        AddPawnAbility(TorannMagicDefOf.TM_EnchantedAura);
                        spell_EnchantedAura = true;
                        AddPawnAbility(TorannMagicDefOf.TM_Transmutate);
                        AddPawnAbility(TorannMagicDefOf.TM_EnchanterStone);
                        AddPawnAbility(TorannMagicDefOf.TM_EnchantWeapon);
                        AddPawnAbility(TorannMagicDefOf.TM_Polymorph);
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Chronomancer);
                if (flag2)
                {
                    //Log.Message("Initializing Chronomancer Abilities");
                    if (abilityUser.IsColonist && !abilityUser.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty))
                    {
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_Prediction);
                            MagicData.MagicPowersC.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Prediction).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_AlterFate);
                            MagicData.MagicPowersC.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_AlterFate).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_AccelerateTime);
                            MagicData.MagicPowersC.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_AccelerateTime).learned = true;
                        }
                        if (Rand.Chance(.4f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_ReverseTime);
                            MagicData.MagicPowersC.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ReverseTime).learned = true;
                        }
                        if (Rand.Chance(.3f))
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_ChronostaticField);
                            MagicData.MagicPowersC.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ChronostaticField).learned = true;
                        }
                        MagicData.MagicPowersC.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Recall).learned = false;
                    }
                    else
                    {
                        for (int i = 0; i < MagicData.MagicPowersC.Count; i++)
                        {
                            if (MagicData.MagicPowersC[i].abilityDef == TorannMagicDefOf.TM_Recall)
                            {
                                MagicData.MagicPowersC[i].learned = true;
                            }
                        }
                        AddPawnAbility(TorannMagicDefOf.TM_Prediction);
                        AddPawnAbility(TorannMagicDefOf.TM_AlterFate);
                        AddPawnAbility(TorannMagicDefOf.TM_AccelerateTime);
                        AddPawnAbility(TorannMagicDefOf.TM_ReverseTime);
                        AddPawnAbility(TorannMagicDefOf.TM_ChronostaticField);

                        if (!abilityUser.IsColonist)
                        {
                            if ((Settings.Instance.AIHardMode && Rand.Chance(hardModeMasterChance)) || Rand.Chance(masterChance))
                            {
                                AddPawnAbility(TorannMagicDefOf.TM_Recall);
                                spell_Recall = true;
                            }
                        }
                    }
                }
                flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.ChaosMage);
                if (flag2)
                {
                    foreach (MagicPower current in MagicData.AllMagicPowers)
                    {
                        if (current.abilityDef != TorannMagicDefOf.TM_ChaosTradition)
                        {
                            current.learned = false;
                        }
                    }
                    MagicData.MagicPowersCM.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ChaosTradition).learned = true;
                    AddPawnAbility(TorannMagicDefOf.TM_ChaosTradition);
                    TM_Calc.AssignChaosMagicPowers(this, !abilityUser.IsColonist);
                }
            }
            AssignAdvancedClassAbilities(true);
        }

        public void AssignAdvancedClassAbilities(bool firstAssignment = false)
        {
            if (AdvancedClasses is { Count: > 0 })
            {
                for (int z = 0; z < MagicData.AllMagicPowers.Count; z++)
                {
                    TMAbilityDef ability = (TMAbilityDef)MagicData.AllMagicPowers[z].abilityDef;
                    foreach (TM_CustomClass cc in AdvancedClasses)
                    {
                        if (cc.classMageAbilities.Contains(ability))
                        {
                            MagicData.AllMagicPowers[z].learned = true;
                        }
                        if (MagicData.AllMagicPowers[z] == MagicData.MagicPowersWD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SoulBond) ||
                        MagicData.AllMagicPowers[z] == MagicData.MagicPowersWD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ShadowBolt) ||
                        MagicData.AllMagicPowers[z] == MagicData.MagicPowersWD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Dominate))
                        {
                            MagicData.AllMagicPowers[z].learned = false;
                        }
                        if (MagicData.AllMagicPowers[z].requiresScroll)
                        {
                            MagicData.AllMagicPowers[z].learned = false;
                        }
                        if (!Pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_Uncertainty) && !Rand.Chance(ability.learnChance))
                        {
                            MagicData.AllMagicPowers[z].learned = false;
                        }
                        if (MagicData.AllMagicPowers[z].learned)
                        {
                            if (ability.shouldInitialize)
                            {
                                AddPawnAbility(ability);
                            }
                            if (ability.childAbilities is { Count: > 0 })
                            {
                                for (int c = 0; c < ability.childAbilities.Count; c++)
                                {
                                    if (ability.childAbilities[c].shouldInitialize)
                                    {
                                        AddPawnAbility(ability.childAbilities[c]);
                                    }
                                }
                            }
                        }
                        if (cc.classHediff != null)
                        {
                            HealthUtility.AdjustSeverity(Pawn, customClass.classHediff, customClass.hediffSeverity);
                        }
                    }
                }
                MagicPower branding = MagicData.AllMagicPowers.FirstOrDefault((MagicPower p) => p.abilityDef == TorannMagicDefOf.TM_Branding);
                if (branding is { learned: true } && firstAssignment)
                {
                    int count = 0;
                    while (count < 2)
                    {
                        TMAbilityDef tmpAbility = TM_Data.BrandList().RandomElement();
                        for (int i = 0; i < MagicData.AllMagicPowers.Count; i++)
                        {
                            TMAbilityDef ad = (TMAbilityDef)MagicData.AllMagicPowers[i].abilityDef;
                            if (!MagicData.AllMagicPowers[i].learned && ad == tmpAbility)
                            {
                                count++;
                                MagicData.AllMagicPowers[i].learned = true;
                                RemovePawnAbility(ad);
                                TryAddPawnAbility(ad);
                            }
                        }
                    }
                }                
            }
        }

        private void InitializeCustomClassSpells()
        {
            for (int j = 0; j < MagicData.AllMagicPowers.Count; j++)
            {
                if (MagicData.AllMagicPowers[j].learned && !customClass.classMageAbilities.Contains(MagicData.AllMagicPowers[j].abilityDef))
                {
                    AddDistinctPawnAbility(MagicData.AllMagicPowers[j].abilityDef);
                }
            }
            if(recallSpell) AddDistinctPawnAbility(TorannMagicDefOf.TM_Recall);
        }

        private void InitializeNonCustomClassSpells()
        {
            Pawn pawn = Pawn;
            if (spell_Rain && !pawn.story.traits.HasTrait(TorannMagicDefOf.HeartOfFrost))
            {
                AddDistinctPawnAbility(TorannMagicDefOf.TM_Rainmaker);
            }
            if (spell_Blink && !pawn.story.traits.HasTrait(TorannMagicDefOf.Arcanist))
            {
                if (!pawn.story.traits.HasTrait(TorannMagicDefOf.ChaosMage))
                {
                    AddDistinctPawnAbility(TorannMagicDefOf.TM_Blink);
                }
                else
                {
                    for (int i = 0; i < chaosPowers.Count; i++)
                    {
                        if (chaosPowers[i].Ability != TorannMagicDefOf.TM_Blink
                            && chaosPowers[i].Ability != TorannMagicDefOf.TM_Blink_I
                            && chaosPowers[i].Ability != TorannMagicDefOf.TM_Blink_II
                            && chaosPowers[i].Ability != TorannMagicDefOf.TM_Blink_III) continue;

                        AddDistinctPawnAbility(TorannMagicDefOf.TM_Blink);
                        break;
                    }
                }
            }
            if (spell_Teleport && !pawn.story.traits.HasTrait(TorannMagicDefOf.Arcanist))
            {
                if (!(pawn.story.traits.HasTrait(TorannMagicDefOf.ChaosMage) && MagicData.MagicPowersA.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_Teleport).learned))
                {
                    AddDistinctPawnAbility(TorannMagicDefOf.TM_Teleport);
                }
            }

            if (spell_Heal && !pawn.story.traits.HasTrait(TorannMagicDefOf.Paladin))
            {
                if (!pawn.story.traits.HasTrait(TorannMagicDefOf.ChaosMage))
                {
                    AddDistinctPawnAbility(TorannMagicDefOf.TM_Heal);
                }
                else
                {
                    bool hasAbility = false;
                    for (int i = 0; i < chaosPowers.Count; i++)
                    {
                        if (chaosPowers[i].Ability == TorannMagicDefOf.TM_Heal)
                        {
                            hasAbility = true;
                        }
                    }
                    if (!hasAbility)
                    {
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_Heal);
                    }
                }
            }

            if (spell_SummonMinion && !pawn.story.traits.HasTrait(TorannMagicDefOf.Summoner))
            {
                if (!pawn.story.traits.HasTrait(TorannMagicDefOf.ChaosMage))
                {
                    AddDistinctPawnAbility(TorannMagicDefOf.TM_SummonMinion);
                }
                else
                {
                    for (int i = 0; i < chaosPowers.Count; i++)
                    {
                        if (chaosPowers[i].Ability != TorannMagicDefOf.TM_SummonMinion) continue;

                        AddDistinctPawnAbility(TorannMagicDefOf.TM_SummonMinion);
                        break;
                    }
                }
            }

            if (spell_FoldReality)
            {
                MagicData.ReturnMatchingMagicPower(TorannMagicDefOf.TM_FoldReality).learned = true;
                AddDistinctPawnAbility(TorannMagicDefOf.TM_FoldReality);
            }

            if (spell_Meteor)
            {
                MagicPower meteorPower = MagicData.MagicPowersG.FirstOrDefault(static mp => mp.abilityDef == TorannMagicDefOf.TM_Meteor)
                                         ?? MagicData.MagicPowersG.FirstOrDefault(static mp => mp.abilityDef == TorannMagicDefOf.TM_Meteor_I)
                                         ?? MagicData.MagicPowersG.FirstOrDefault(static mp => mp.abilityDef == TorannMagicDefOf.TM_Meteor_II)
                                         ?? MagicData.MagicPowersG.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_Meteor_III);
                switch (meteorPower.level)
                {
                    case 3:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_Meteor_III);
                        break;
                    case 2:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_Meteor_II);
                        break;
                    case 1:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_Meteor_I);
                        break;
                    default:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_Meteor);
                        break;
                }
            }
            if (spell_OrbitalStrike)
            {
                MagicPower OrbitalStrikePower = MagicData.magicPowerT.FirstOrDefault(static mp => mp.abilityDef == TorannMagicDefOf.TM_OrbitalStrike)
                                                ?? MagicData.magicPowerT.FirstOrDefault(static mp => mp.abilityDef == TorannMagicDefOf.TM_OrbitalStrike_I)
                                                ?? MagicData.magicPowerT.FirstOrDefault(static mp => mp.abilityDef == TorannMagicDefOf.TM_OrbitalStrike_II)
                                                ?? MagicData.magicPowerT.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_OrbitalStrike_III);
                switch (OrbitalStrikePower.level)
                {
                    case 3:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_OrbitalStrike_III);
                        break;
                    case 2:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_OrbitalStrike_II);
                        break;
                    case 1:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_OrbitalStrike_I);
                        break;
                    default:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_OrbitalStrike);
                        break;
                }
            }
            if (spell_BloodMoon)
            {
                MagicPower BloodMoonPower = MagicData.MagicPowersBM.FirstOrDefault(static mp => mp.abilityDef == TorannMagicDefOf.TM_BloodMoon)
                                            ?? MagicData.MagicPowersBM.FirstOrDefault(static mp => mp.abilityDef == TorannMagicDefOf.TM_BloodMoon_I)
                                            ?? MagicData.MagicPowersBM.FirstOrDefault(static mp => mp.abilityDef == TorannMagicDefOf.TM_BloodMoon_II)
                                            ?? MagicData.MagicPowersBM.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_BloodMoon_III);
                switch (BloodMoonPower.level)
                {
                    case 3:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_BloodMoon_III);
                        break;
                    case 2:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_BloodMoon_II);
                        break;
                    case 1:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_BloodMoon_I);
                        break;
                    default:
                        AddDistinctPawnAbility(TorannMagicDefOf.TM_BloodMoon);
                        break;
                }
            }

            if (spell_Recall)
            {
                AddDistinctPawnAbility(TorannMagicDefOf.TM_TimeMark);
                if (recallSpell)
                {
                    AddDistinctPawnAbility(TorannMagicDefOf.TM_Recall);
                }
            }

            if (spell_Heater) AddDistinctPawnAbility(TorannMagicDefOf.TM_Heater);
            if (spell_Cooler) AddDistinctPawnAbility(TorannMagicDefOf.TM_Cooler);
            if (spell_PowerNode) AddDistinctPawnAbility(TorannMagicDefOf.TM_PowerNode);
            if (spell_Sunlight) AddDistinctPawnAbility(TorannMagicDefOf.TM_Sunlight);
            if (spell_DryGround) AddDistinctPawnAbility(TorannMagicDefOf.TM_DryGround);
            if (spell_WetGround) AddDistinctPawnAbility(TorannMagicDefOf.TM_WetGround);
            if (spell_ChargeBattery) AddDistinctPawnAbility(TorannMagicDefOf.TM_ChargeBattery);
            if (spell_SmokeCloud) AddDistinctPawnAbility(TorannMagicDefOf.TM_SmokeCloud);
            if (spell_Extinguish) AddDistinctPawnAbility(TorannMagicDefOf.TM_Extinguish);
            if (spell_EMP) AddDistinctPawnAbility(TorannMagicDefOf.TM_EMP);
            if (spell_Blizzard) AddDistinctPawnAbility(TorannMagicDefOf.TM_Blizzard);
            if (spell_Firestorm) AddDistinctPawnAbility(TorannMagicDefOf.TM_Firestorm);
            if (spell_TransferMana) AddDistinctPawnAbility(TorannMagicDefOf.TM_TransferMana);
            if (spell_SiphonMana) AddDistinctPawnAbility(TorannMagicDefOf.TM_SiphonMana);
            if (spell_RegrowLimb) AddDistinctPawnAbility(TorannMagicDefOf.TM_RegrowLimb);
            if (spell_EyeOfTheStorm) AddDistinctPawnAbility(TorannMagicDefOf.TM_EyeOfTheStorm);
            if (spell_HeatShield) AddDistinctPawnAbility(TorannMagicDefOf.TM_HeatShield);
            if (spell_ManaShield) AddDistinctPawnAbility(TorannMagicDefOf.TM_ManaShield);
            if (spell_Blur) AddDistinctPawnAbility(TorannMagicDefOf.TM_Blur);
            if (spell_Resurrection) AddDistinctPawnAbility(TorannMagicDefOf.TM_Resurrection);
            if (spell_HolyWrath) AddDistinctPawnAbility(TorannMagicDefOf.TM_HolyWrath);
            if (spell_LichForm) AddDistinctPawnAbility(TorannMagicDefOf.TM_LichForm);
            if (spell_Flight) AddDistinctPawnAbility(TorannMagicDefOf.TM_Flight);
            if (spell_SummonPoppi) AddDistinctPawnAbility(TorannMagicDefOf.TM_SummonPoppi);
            if (spell_BattleHymn) AddDistinctPawnAbility(TorannMagicDefOf.TM_BattleHymn);
            if (spell_CauterizeWound) AddDistinctPawnAbility(TorannMagicDefOf.TM_CauterizeWound);
            if (spell_SpellMending) AddDistinctPawnAbility(TorannMagicDefOf.TM_SpellMending);
            if (spell_FertileLands) AddDistinctPawnAbility(TorannMagicDefOf.TM_FertileLands);
            if (spell_PsychicShock) AddDistinctPawnAbility(TorannMagicDefOf.TM_PsychicShock);
            if (spell_Scorn) AddDistinctPawnAbility(TorannMagicDefOf.TM_Scorn);
            if (spell_BlankMind) AddDistinctPawnAbility(TorannMagicDefOf.TM_BlankMind);
            if (spell_ShadowStep) AddDistinctPawnAbility(TorannMagicDefOf.TM_ShadowStep);
            if (spell_ShadowCall) AddDistinctPawnAbility(TorannMagicDefOf.TM_ShadowCall);
            if (spell_Teach) AddDistinctPawnAbility(TorannMagicDefOf.TM_TeachMagic);
            if (spell_EnchantedAura) AddDistinctPawnAbility(TorannMagicDefOf.TM_EnchantedAura);
            if (spell_Shapeshift) AddDistinctPawnAbility(TorannMagicDefOf.TM_Shapeshift);
            if (spell_ShapeshiftDW) AddDistinctPawnAbility(TorannMagicDefOf.TM_ShapeshiftDW);
            if (spell_DirtDevil) AddDistinctPawnAbility(TorannMagicDefOf.TM_DirtDevil);
            if (spell_MechaniteReprogramming) AddDistinctPawnAbility(TorannMagicDefOf.TM_MechaniteReprogramming);
            if (spell_ArcaneBolt) AddDistinctPawnAbility(TorannMagicDefOf.TM_ArcaneBolt);
            if (spell_LightningTrap) AddDistinctPawnAbility(TorannMagicDefOf.TM_LightningTrap);
            if (spell_Invisibility) AddDistinctPawnAbility(TorannMagicDefOf.TM_Invisibility);
            if (spell_BriarPatch) AddDistinctPawnAbility(TorannMagicDefOf.TM_BriarPatch);
            if (spell_MageLight) AddDistinctPawnAbility(TorannMagicDefOf.TM_MageLight);
            if (spell_SnapFreeze) AddDistinctPawnAbility(TorannMagicDefOf.TM_SnapFreeze);
            if (spell_Ignite) AddDistinctPawnAbility(TorannMagicDefOf.TM_Ignite);

            for (int j = 0; j < MagicData.MagicPowersCustomAll.Count; j++)
            {
                if (MagicData.MagicPowersCustomAll[j].learned)
                {
                    AddDistinctPawnAbility(MagicData.MagicPowersCustomAll[j].abilityDef);
                }
            }
        }

        public void InitializeSpell()
        {
            if (!IsMagicUser) return;

            if (customClass != null) InitializeCustomClassSpells();
            else InitializeNonCustomClassSpells();
        }

        public void RemovePowers(bool clearStandalone = true)
        {
            Pawn abilityUser = Pawn;
            if (magicPowersInitialized && MagicData != null)
            {
                bool flag2 = true;
                if (customClass != null)
                {
                    for (int i = 0; i < MagicData.AllMagicPowers.Count; i++)
                    {
                        MagicPower mp = MagicData.AllMagicPowers[i];
                        for (int j = 0; j < mp.TMabilityDefs.Count; j++)
                        {
                            TMAbilityDef tmad = mp.TMabilityDefs[j] as TMAbilityDef;
                            if(tmad.childAbilities is { Count: > 0 })
                            {
                                for(int k = 0; k < tmad.childAbilities.Count; k++)
                                {
                                    RemovePawnAbility(tmad.childAbilities[k]);
                                }
                            }                            
                            RemovePawnAbility(tmad);
                        }
                        mp.learned = false;
                    }
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.InnerFire);
                if (TM_Calc.IsWanderer(Pawn))
                {
                    spell_ArcaneBolt = false;
                    RemovePawnAbility(TorannMagicDefOf.TM_ArcaneBolt);
                }
                if (abilityUser.story.traits.HasTrait(TorannMagicDefOf.ChaosMage))
                {
                    foreach (MagicPower current in MagicData.AllMagicPowersForChaosMage)
                    {
                        if (current.abilityDef != TorannMagicDefOf.TM_ChaosTradition)
                        {
                            current.learned = false;
                            foreach (TMAbilityDef tmad in current.TMabilityDefs)
                            {
                                if (tmad.childAbilities is { Count: > 0 })
                                {
                                    for (int k = 0; k < tmad.childAbilities.Count; k++)
                                    {
                                        RemovePawnAbility(tmad.childAbilities[k]);
                                    }
                                }
                                RemovePawnAbility(tmad);
                            }
                        }
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_EnchantedAura);
                    RemovePawnAbility(TorannMagicDefOf.TM_NanoStimulant);
                    RemovePawnAbility(TorannMagicDefOf.TM_LightSkipMass);
                    RemovePawnAbility(TorannMagicDefOf.TM_LightSkipGlobal);
                    spell_EnchantedAura = false;
                    spell_ShadowCall = false;
                    spell_ShadowStep = false;
                    RemovePawnAbility(TorannMagicDefOf.TM_ShadowCall);
                    RemovePawnAbility(TorannMagicDefOf.TM_ShadowStep);

                }
                if (flag2)
                {
                    //Log.Message("Fixing Inner Fire Abilities");
                    foreach (MagicPower currentIF in MagicData.MagicPowersIF)
                    {
                        if (currentIF.abilityDef != TorannMagicDefOf.TM_Firestorm)
                        {
                            currentIF.learned = false;
                        }
                        RemovePawnAbility(currentIF.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_RayofHope_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_RayofHope_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_RayofHope_III);

                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.HeartOfFrost);
                if (flag2)
                {
                    //Log.Message("Fixing Heart of Frost Abilities");
                    foreach (MagicPower currentHoF in MagicData.MagicPowersHoF)
                    {
                        if (currentHoF.abilityDef != TorannMagicDefOf.TM_Blizzard)
                        {
                            currentHoF.learned = false;
                        }
                        RemovePawnAbility(currentHoF.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_Soothe_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Soothe_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Soothe_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_FrostRay_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_FrostRay_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_FrostRay_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.StormBorn);
                if (flag2)
                {
                    //Log.Message("Fixing Storm Born Abilities");                   
                    foreach (MagicPower currentSB in MagicData.MagicPowersSB)
                    {
                        if (currentSB.abilityDef != TorannMagicDefOf.TM_EyeOfTheStorm)
                        {
                            currentSB.learned = false;
                        }
                        RemovePawnAbility(currentSB.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_AMP_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_AMP_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_AMP_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Arcanist);
                if (flag2)
                {
                    //Log.Message("Fixing Arcane Abilities");
                    foreach (MagicPower currentA in MagicData.MagicPowersA)
                    {
                        if (currentA.abilityDef != TorannMagicDefOf.TM_FoldReality)
                        {
                            currentA.learned = false;
                        }
                        RemovePawnAbility(currentA.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_Shadow_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Shadow_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Shadow_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_MagicMissile_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_MagicMissile_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_MagicMissile_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_Blink_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Blink_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Blink_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_Summon_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Summon_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Summon_III);

                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Paladin);
                if (flag2)
                {
                    //Log.Message("Fixing Paladin Abilities");
                    foreach (MagicPower currentP in MagicData.MagicPowersP)
                    {
                        if (currentP.abilityDef != TorannMagicDefOf.TM_HolyWrath)
                        {
                            currentP.learned = false;
                        }
                        RemovePawnAbility(currentP.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_Shield_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Shield_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Shield_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Summoner);
                if (flag2)
                {
                    foreach (MagicPower currentS in MagicData.MagicPowersS)
                    {
                        if (currentS.abilityDef != TorannMagicDefOf.TM_SummonPoppi)
                        {
                            currentS.learned = false;
                        }
                        RemovePawnAbility(currentS.abilityDef);
                    }
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Druid);
                if (flag2)
                {
                    foreach (MagicPower currentD in MagicData.MagicPowersD)
                    {
                        if (currentD.abilityDef != TorannMagicDefOf.TM_RegrowLimb)
                        {
                            currentD.learned = false;
                        }
                        RemovePawnAbility(currentD.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_SootheAnimal_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_SootheAnimal_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_SootheAnimal_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Necromancer) || abilityUser.story.traits.HasTrait(TorannMagicDefOf.Lich);
                if (flag2)
                {
                    foreach (MagicPower currentN in MagicData.MagicPowersN)
                    {
                        if (currentN.abilityDef != TorannMagicDefOf.TM_LichForm)
                        {
                            currentN.learned = false;
                        }
                        RemovePawnAbility(currentN.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_DeathMark_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_DeathMark_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_DeathMark_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_ConsumeCorpse_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_ConsumeCorpse_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_ConsumeCorpse_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_CorpseExplosion_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_CorpseExplosion_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_CorpseExplosion_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_DeathBolt_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_DeathBolt_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_DeathBolt_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Priest);
                if (flag2)
                {
                    foreach (MagicPower currentPR in MagicData.MagicPowersPR)
                    {
                        if (currentPR.abilityDef != TorannMagicDefOf.TM_Resurrection)
                        {
                            currentPR.learned = false;
                        }
                        RemovePawnAbility(currentPR.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_HealingCircle_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_HealingCircle_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_HealingCircle_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_BestowMight_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_BestowMight_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_BestowMight_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.TM_Bard);
                if (flag2)
                {
                    foreach (MagicPower currentB in MagicData.MagicPowersB)
                    {
                        if (currentB.abilityDef != TorannMagicDefOf.TM_BattleHymn)
                        {
                            currentB.learned = false;
                        }
                        RemovePawnAbility(currentB.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_Lullaby_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Lullaby_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Lullaby_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Succubus);
                if (flag2)
                {
                    foreach (MagicPower currentSD in MagicData.MagicPowersSD)
                    {
                        if (currentSD.abilityDef != TorannMagicDefOf.TM_Scorn)
                        {
                            currentSD.learned = false;
                        }
                        RemovePawnAbility(currentSD.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_ShadowBolt_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_ShadowBolt_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_ShadowBolt_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_Attraction_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Attraction_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Attraction_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Warlock);
                if (flag2)
                {
                    foreach (MagicPower currentWD in MagicData.MagicPowersWD)
                    {
                        if (currentWD.abilityDef != TorannMagicDefOf.TM_PsychicShock)
                        {
                            currentWD.learned = false;
                        }
                        RemovePawnAbility(currentWD.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_Repulsion_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Repulsion_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Repulsion_III);
                }
                // flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Geomancer);
                if (flag2)
                {
                    foreach (MagicPower currentG in MagicData.MagicPowersG)
                    {
                        if (currentG.abilityDef == TorannMagicDefOf.TM_Meteor || currentG.abilityDef == TorannMagicDefOf.TM_Meteor_I || currentG.abilityDef == TorannMagicDefOf.TM_Meteor_II || currentG.abilityDef == TorannMagicDefOf.TM_Meteor_III)
                        {
                            currentG.learned = true;
                        }
                        else
                        {
                            currentG.learned = false;
                        }
                        RemovePawnAbility(currentG.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_Encase_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Encase_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Encase_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_Meteor_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Meteor_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Meteor_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Technomancer);
                if (flag2)
                {
                    foreach (MagicPower currentT in MagicData.MagicPowersT)
                    {
                        if (currentT.abilityDef == TorannMagicDefOf.TM_OrbitalStrike || currentT.abilityDef == TorannMagicDefOf.TM_OrbitalStrike_I || currentT.abilityDef == TorannMagicDefOf.TM_OrbitalStrike_II || currentT.abilityDef == TorannMagicDefOf.TM_OrbitalStrike_III)
                        {
                            currentT.learned = true;
                        }
                        else
                        {
                            currentT.learned = false;
                        }
                        RemovePawnAbility(currentT.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_OrbitalStrike_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_OrbitalStrike_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_OrbitalStrike_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.BloodMage);
                if (flag2)
                {
                    foreach (MagicPower currentBM in MagicData.MagicPowersBM)
                    {
                        if (currentBM.abilityDef == TorannMagicDefOf.TM_BloodMoon || currentBM.abilityDef == TorannMagicDefOf.TM_BloodMoon_I || currentBM.abilityDef == TorannMagicDefOf.TM_BloodMoon_II || currentBM.abilityDef == TorannMagicDefOf.TM_BloodMoon_III)
                        {
                            currentBM.learned = true;
                        }
                        else
                        {
                            currentBM.learned = false;
                        }
                        RemovePawnAbility(currentBM.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_Rend_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Rend_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Rend_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_BloodMoon_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_BloodMoon_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_BloodMoon_III);
                }
                //flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Enchanter);
                if (flag2)
                {

                    foreach (MagicPower currentE in MagicData.MagicPowersE)
                    {
                        if (currentE.abilityDef != TorannMagicDefOf.TM_Shapeshift)
                        {
                            currentE.learned = false;
                        }
                        RemovePawnAbility(currentE.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_Polymorph_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_Polymorph_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_Polymorph_III);
                    RemovePawnAbility(TorannMagicDefOf.TM_EnchantedAura);
                }
                // flag2 = abilityUser.story.traits.HasTrait(TorannMagicDefOf.Chronomancer);
                if (flag2)
                {
                    foreach (MagicPower currentC in MagicData.MagicPowersC)
                    {
                        if (currentC.abilityDef != TorannMagicDefOf.TM_Recall)
                        {
                            currentC.learned = false;
                        }
                        RemovePawnAbility(currentC.abilityDef);
                    }
                    RemovePawnAbility(TorannMagicDefOf.TM_ChronostaticField_I);
                    RemovePawnAbility(TorannMagicDefOf.TM_ChronostaticField_II);
                    RemovePawnAbility(TorannMagicDefOf.TM_ChronostaticField_III);
                }
                if (flag2)
                {
                    foreach (MagicPower currentShadow in MagicData.MagicPowersShadow)
                    {
                        RemovePawnAbility(currentShadow.abilityDef);
                    }
                }
                if (clearStandalone)
                {
                    foreach (MagicPower currentStandalone in MagicData.MagicPowersStandalone)
                    {
                        RemovePawnAbility(currentStandalone.abilityDef);
                    }
                }
            }
        }

        public override bool TryTransformPawn()
        {
            return IsMagicUser;
        }

        public bool TryAddPawnAbility(TMAbilityDef ability)
        {
            //add check to verify no ability is already added
            AddPawnAbility(ability);
            return true;
        }

        private void ClearPower(MagicPower current)
        {
            Log.Message("Removing ability: " + current.abilityDef.defName);
            RemovePawnAbility(current.abilityDef);
            UpdateAbilities();
        }

        public void ResetSkills()
        {
            MagicData.MagicPowerSkill_global_regen.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_regen_pwr").level = 0;
            MagicData.MagicPowerSkill_global_eff.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_eff_pwr").level = 0;
            MagicData.MagicPowerSkill_global_spirit.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_spirit_pwr").level = 0;
            for (int i = 0; i < MagicData.AllMagicPowersWithSkills.Count; i++)
            {
                MagicData.AllMagicPowersWithSkills[i].level = 0;
                MagicData.AllMagicPowersWithSkills[i].learned = false;
                MagicData.AllMagicPowersWithSkills[i].autocast = false;
                TMAbilityDef ability = (TMAbilityDef)MagicData.AllMagicPowersWithSkills[i].abilityDef;
                MagicPowerSkill mps = MagicData.GetSkill_Efficiency(ability);
                if (mps != null)
                {
                    mps.level = 0;
                }
                mps = MagicData.GetSkill_Power(ability);
                if (mps != null)
                {
                    mps.level = 0;
                }
                mps = MagicData.GetSkill_Versatility(ability);
                if (mps != null)
                {
                    mps.level = 0;
                }
            }
            for(int i = 0; i < MagicData.AllMagicPowers.Count; i++)
            {                
                for(int j = 0; j < MagicData.AllMagicPowers[i].TMabilityDefs.Count; j++)
                {
                    TMAbilityDef ability = (TMAbilityDef)MagicData.AllMagicPowers[i].TMabilityDefs[j];
                    RemovePawnAbility(ability);
                }
                MagicData.AllMagicPowers[i].learned = false;
                MagicData.AllMagicPowers[i].autocast = false;
            }
            MagicUserLevel = 0;
            MagicUserXP = 0;
            MagicData.MagicAbilityPoints = 0;
            AssignAbilities();
        }

        private void LoadPowers()
        {
            foreach (MagicPower currentA in MagicData.MagicPowersA)
            {
                //Log.Message("Removing ability: " + currentA.abilityDef.defName.ToString());
                currentA.level = 0;
                RemovePawnAbility(currentA.abilityDef);
            }
            foreach (MagicPower currentHoF in MagicData.MagicPowersHoF)
            {
                //Log.Message("Removing ability: " + currentHoF.abilityDef.defName.ToString());
                currentHoF.level = 0;
                RemovePawnAbility(currentHoF.abilityDef);
            }
            foreach (MagicPower currentSB in MagicData.MagicPowersSB)
            {
                //Log.Message("Removing ability: " + currentSB.abilityDef.defName.ToString());
                currentSB.level = 0;
                RemovePawnAbility(currentSB.abilityDef);
            }
            foreach (MagicPower currentIF in MagicData.MagicPowersIF)
            {
                //Log.Message("Removing ability: " + currentIF.abilityDef.defName.ToString());
                currentIF.level = 0;
                RemovePawnAbility(currentIF.abilityDef);
            }
            foreach (MagicPower currentP in MagicData.MagicPowersP)
            {
                //Log.Message("Removing ability: " + currentP.abilityDef.defName.ToString());
                currentP.level = 0;
                RemovePawnAbility(currentP.abilityDef);
            }
            foreach (MagicPower currentS in MagicData.MagicPowersS)
            {
                //Log.Message("Removing ability: " + currentP.abilityDef.defName.ToString());
                currentS.level = 0;
                RemovePawnAbility(currentS.abilityDef);
            }
            foreach (MagicPower currentD in MagicData.MagicPowersD)
            {
                //Log.Message("Removing ability: " + currentP.abilityDef.defName.ToString());
                currentD.level = 0;
                RemovePawnAbility(currentD.abilityDef);
            }
            foreach (MagicPower currentN in MagicData.MagicPowersN)
            {
                //Log.Message("Removing ability: " + currentP.abilityDef.defName.ToString());
                currentN.level = 0;
                RemovePawnAbility(currentN.abilityDef);
            }
        }

        public void RemoveTMagicHediffs()
        {
            List<Hediff> allHediffs = Pawn.health.hediffSet.hediffs;
            for (int i = 0; i < allHediffs.Count(); i++)
            {
                if (allHediffs[i].def.defName.StartsWith("TM_"))
                {
                    Pawn.health.RemoveHediff(allHediffs[i]);
                }
            }
        }

        public void RemoveAbilityUser()
        {
            RemovePowers();
            RemoveTMagicHediffs();
            RemoveTraits();
            magicData = null;
            Initialized = false;
        }

        public float ActualManaCost(TMAbilityDef magicDef)
        {
            float adjustedManaCost = magicDef.manaCost;
            if (magicDef.efficiencyReductionPercent != 0)
            {
                if (magicDef == TorannMagicDefOf.TM_EnchantedAura)
                {
                    adjustedManaCost *= 1f - (magicDef.efficiencyReductionPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_EnchantedBody).level);
                }
                else if (magicDef == TorannMagicDefOf.TM_ShapeshiftDW)
                {
                    adjustedManaCost *= 1f - (magicDef.efficiencyReductionPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_Shapeshift).level);
                }
                else if (magicDef == TorannMagicDefOf.TM_ShadowStep || magicDef == TorannMagicDefOf.TM_ShadowCall)
                {
                    adjustedManaCost *= 1f - (magicDef.efficiencyReductionPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_SoulBond).level);
                }
                else if( magicDef == TorannMagicDefOf.TM_LightSkipGlobal || magicDef == TorannMagicDefOf.TM_LightSkipMass)
                {
                    adjustedManaCost *= 1f - (magicDef.efficiencyReductionPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_LightSkip).level);
                }      
                else if(magicDef == TorannMagicDefOf.TM_SummonTotemEarth || magicDef == TorannMagicDefOf.TM_SummonTotemHealing || magicDef == TorannMagicDefOf.TM_SummonTotemLightning)
                {
                    adjustedManaCost *= 1f - (magicDef.efficiencyReductionPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_Totems).level);
                }
                else if (magicDef == TorannMagicDefOf.TM_Hex_CriticalFail || magicDef == TorannMagicDefOf.TM_Hex_Pain || magicDef == TorannMagicDefOf.TM_Hex_MentalAssault)
                {
                    adjustedManaCost *= 1f - (magicDef.efficiencyReductionPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_Hex).level);
                }
                else if(Pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless))
                {
                    CompAbilityUserMight compMight = Pawn.GetCompAbilityUserMight();
                    adjustedManaCost *= 1f - (magicDef.efficiencyReductionPercent * compMight.MightData.GetSkill_Efficiency(TorannMagicDefOf.TM_Mimic).level);
                }
                else
                {
                    MagicPowerSkill mps = MagicData.GetSkill_Efficiency(magicDef);
                    if (mps != null)
                    {
                        adjustedManaCost *= 1f - (magicDef.efficiencyReductionPercent * mps.level);
                    }
                }
            }

            if (Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_SyrriumSenseHD")))
            {
                adjustedManaCost *= .9f;
            }
            if (Math.Abs(mpCost - 1f) > TOLERANCE)
            {
                if (magicDef == TorannMagicDefOf.TM_Explosion)
                {
                    adjustedManaCost *= 1f - (1f - mpCost)/10f;
                }
                else
                {
                    adjustedManaCost *= mpCost;
                }
            }
            if (magicDef.abilityHediff != null && Pawn.health.hediffSet.HasHediff(magicDef.abilityHediff))
            {
                return 0f;
            }
            if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless))
            {
                adjustedManaCost = 0;
            }
            if (Pawn.story.traits.HasTrait(TorannMagicDefOf.ChaosMage) || (customClass != null && customClass.classMageAbilities.Contains(TorannMagicDefOf.TM_ChaosTradition)))
            {
                adjustedManaCost = adjustedManaCost * 1.2f;
            }

            if(Pawn.Map.gameConditionManager.ConditionIsActive(TorannMagicDefOf.TM_ManaStorm))
            {
                return Mathf.Max(adjustedManaCost *.5f, 0f);
            }
            return Mathf.Max(adjustedManaCost, (.5f * magicDef.manaCost));
        }

        public override List<HediffDef> IgnoredHediffs()
        {
            return new List<HediffDef>
            {
                TorannMagicDefOf.TM_MightUserHD
            };
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            Pawn pawn = Pawn;
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
            {
                Hediff hediff = pawn.health.hediffSet.hediffs[i];

                if (hediff.def == TorannMagicDefOf.TM_HediffInvulnerable)
                {
                    absorbed = true;
                    FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.ExplosionFlash, 10);
                    dinfo.SetAmount(0);
                    return;
                }
                if (hediff.def == TorannMagicDefOf.TM_HediffEnchantment_phantomShift && Rand.Chance(.2f))
                {
                    absorbed = true;
                    FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.ExplosionFlash, 8);
                    FleckMaker.ThrowSmoke(pawn.Position.ToVector3Shifted(), pawn.Map, 1.2f);
                    dinfo.SetAmount(0);
                    return;
                }
                if (hediff.def == TorannMagicDefOf.TM_HediffShield)
                {
                    float sev = hediff.Severity;
                    absorbed = true;
                    int actualDmg = 0;
                    float dmgAmt = dinfo.Amount;
                    float dmgToSev = 0.004f;

                    if (!pawn.IsColonist)
                    {
                        dmgToSev = Settings.Instance.AIHardMode ? 0.0025f : 0.003f;
                    }
                    sev -= dmgAmt * dmgToSev;
                    if (sev < 0)
                    {
                        actualDmg = Mathf.RoundToInt(Mathf.Abs(sev / dmgToSev));
                        BreakShield(pawn);
                    }
                    TM_Action.DisplayShieldHit(pawn, dinfo);
                    hediff.Severity = sev;
                    dinfo.SetAmount(actualDmg);

                    return;
                }
                if (hediff.def == TorannMagicDefOf.TM_DemonScornHD || hediff.def == TorannMagicDefOf.TM_DemonScornHD_I || hediff.def == TorannMagicDefOf.TM_DemonScornHD_II || hediff.def == TorannMagicDefOf.TM_DemonScornHD_III)
                {
                    float sev = hediff.Severity;
                    absorbed = true;
                    int actualDmg = 0;
                    float dmgAmt = dinfo.Amount;
                    float dmgToSev = 1f;

                    if (!pawn.IsColonist)
                    {
                        dmgToSev = Settings.Instance.AIHardMode ? 0.8f : 1f;
                    }
                    sev -= dmgAmt * dmgToSev;
                    if (sev < 0)
                    {
                        actualDmg = Mathf.RoundToInt(Mathf.Abs(sev / dmgToSev));
                        BreakShield(pawn);
                    }
                    TM_Action.DisplayShieldHit(pawn, dinfo);
                    hediff.Severity = sev;
                    dinfo.SetAmount(actualDmg);

                    return;
                }
                if (hediff.def == TorannMagicDefOf.TM_ManaShieldHD && damageMitigationDelayMS < age)
                {
                    float sev = Mana.CurLevel;
                    absorbed = true;
                    int actualDmg = 0;
                    float dmgAmt = dinfo.Amount;
                    float dmgToSev = 0.02f;
                    float maxDmg = 11f;
                    if (MagicData.MagicPowerSkill_Cantrips.First(static mps => mps.label == "TM_Cantrips_ver").level >= 3)
                    {
                        dmgToSev = 0.015f;
                        maxDmg = 14f;
                        if (MagicData.MagicPowerSkill_Cantrips.First(static mps => mps.label == "TM_Cantrips_ver").level >= 7)
                        {
                            dmgToSev = 0.012f;
                            maxDmg = 17f;
                        }
                    }
                    if (dmgAmt >= maxDmg)
                    {
                        actualDmg = Mathf.RoundToInt(dmgAmt - maxDmg);
                        sev -= maxDmg * dmgToSev;
                    }
                    else
                    {
                        sev -= dmgAmt * dmgToSev;
                    }
                    Mana.CurLevel = sev;
                    if (sev < 0)
                    {
                        actualDmg = Mathf.RoundToInt(Mathf.Abs(sev / dmgToSev));
                        BreakShield(pawn);
                        hediff.Severity = sev;
                        pawn.health.RemoveHediff(hediff);
                    }
                    TM_Action.DisplayShieldHit(pawn, dinfo);
                    damageMitigationDelayMS = age + 2;
                    dinfo.SetAmount(actualDmg);
                    pawn.TakeDamage(dinfo);
                    return;
                }
                if (hediff.def == TorannMagicDefOf.TM_LichHD && damageMitigationDelay < age)
                {
                    absorbed = true;
                    const int mitigationAmt = 4;
                    int dmgAmt = Mathf.RoundToInt(dinfo.Amount);
                    if (dmgAmt < mitigationAmt)
                    {
                        MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "TM_DamageAbsorbedAll".Translate());
                        return;
                    }
                    MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "TM_DamageAbsorbed".Translate(
                        dmgAmt, mitigationAmt));
                    int actualDmg = dmgAmt - mitigationAmt;

                    damageMitigationDelay = age + 6;
                    dinfo.SetAmount(actualDmg);
                    pawn.TakeDamage(dinfo);
                    return;
                }
                if (arcaneRes != 0 && resMitigationDelay < age)
                {
                    if (hediff.def == TorannMagicDefOf.TM_HediffEnchantment_arcaneRes)
                    {
                        if ((dinfo.Def.armorCategory != null && (dinfo.Def.armorCategory == TorannMagicDefOf.Dark || dinfo.Def.armorCategory == TorannMagicDefOf.Light)) || dinfo.Def.defName.Contains("TM_") || dinfo.Def.defName is "FrostRay" or "Snowball" or "Iceshard" or "Firebolt")
                        {
                            absorbed = true;
                            int actualDmg = Mathf.RoundToInt(dinfo.Amount / arcaneRes);
                            resMitigationDelay = age + 10;
                            dinfo.SetAmount(actualDmg);
                            pawn.TakeDamage(dinfo);
                            return;
                        }
                    }
                }
            }

            base.PostPreApplyDamage(dinfo, out absorbed);
        }

        private void BreakShield(Pawn pawn)
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
            FleckMaker.Static(pawn.TrueCenter(), pawn.Map, FleckDefOf.ExplosionFlash, 12f);
            for (int i = 0; i < 6; i++)
            {
                Vector3 loc = pawn.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                FleckMaker.ThrowDustPuff(loc, pawn.Map, Rand.Range(0.8f, 1.2f));
            }
        }

        public void DoArcaneForging()
        {
            if (Pawn.CurJob.targetA.Thing.def.defName == "TableArcaneForge")
            {
                ArcaneForging = true;
                Thing forge = Pawn.CurJob.targetA.Thing;
                if (Pawn.Position == forge.InteractionCell && Pawn.jobs.curDriver.CurToilIndex >= 10)
                {
                    if (Find.TickManager.TicksGame % 20 == 0)
                    {
                        if (Mana.CurLevel >= .1f)
                        {
                            Mana.CurLevel -= .025f;
                            MagicUserXP += 4;
                            FleckMaker.ThrowSmoke(forge.DrawPos, forge.Map, Rand.Range(.8f, 1.2f));
                        }
                        else
                        {
                            Pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                        }
                    }

                    ThingDef mote = null;
                    int rnd = Rand.RangeInclusive(0, 3);
                    switch (rnd)
                    {
                        case 0:
                            mote = TorannMagicDefOf.Mote_ArcaneFabricationA;
                            break;
                        case 1:
                            mote = TorannMagicDefOf.Mote_ArcaneFabricationB;
                            break;
                        case 2:
                            mote = TorannMagicDefOf.Mote_ArcaneFabricationC;
                            break;
                        case 3:
                            mote = TorannMagicDefOf.Mote_ArcaneFabricationD;
                            break;
                    }
                    Vector3 drawPos = forge.DrawPos;
                    drawPos.x += Rand.Range(-.05f, .05f);
                    drawPos.z += Rand.Range(-.05f, .05f);
                    TM_MoteMaker.ThrowGenericMote(mote, drawPos, forge.Map, Rand.Range(.25f, .4f), .02f, 0f, .01f, Rand.Range(-200, 200), 0, 0, forge.Rotation.AsAngle);
                }
            }
            else
            {
                ArcaneForging = false;
            }
        }

        public void ResolveMagicUseEvents()
        {
            int expiryTick = Find.TickManager.TicksGame - 150000;
            for (int i = MagicUsed.Count - 1; i >= 0; i--)
            {
                if (expiryTick > MagicUsed[i].eventTick) MagicUsed.RemoveAt(i);
            }
        }

        public void ResolveAutoCast()
        {
            bool flagCM = Pawn.story.traits.HasTrait(TorannMagicDefOf.ChaosMage);
            bool isCustom = customClass != null;
            if (Settings.Instance.autocastEnabled && Pawn.jobs != null && Pawn.CurJob != null && Pawn.CurJob.def != TorannMagicDefOf.TMCastAbilityVerb && Pawn.CurJob.def != TorannMagicDefOf.TMCastAbilitySelf &&
                Pawn.CurJob.def != JobDefOf.Ingest && Pawn.CurJob.def != JobDefOf.ManTurret && Pawn.GetPosture() == PawnPosture.Standing && !Pawn.CurJob.playerForced && !Pawn.Map.GameConditionManager.ConditionIsActive(TorannMagicDefOf.ManaDrain) && !Pawn.Map.GameConditionManager.ConditionIsActive(TorannMagicDefOf.TM_ManaStorm))
            {
                //Log.Message("pawn " + Pawn.LabelShort + " current job is " + Pawn.CurJob.def.defName);
                //non-combat (undrafted) spells
                bool castSuccess = false;
                if (Pawn.drafter != null && !Pawn.Drafted && Mana != null && Mana.CurLevelPercentage >= Settings.Instance.autocastMinThreshold)
                {
                    foreach (MagicPower mp in MagicData.MagicPowersCustomAll)
                    {
                        if (!mp.learned || !mp.autocast || mp.autocasting is not { magicUser: true, undrafted: true })
                            continue;

                        TMAbilityDef tmad = mp.TMabilityDefs[mp.level] as TMAbilityDef; // issues with index?
                        bool canUseWithEquippedWeapon = true;
                        bool canUseIfViolentAbility = !Pawn.story.DisabledWorkTagsBackstoryAndTraits.HasFlag(WorkTags.Violent) || !tmad.MainVerb.isViolent;
                        if(!TM_Calc.HasResourcesForAbility(Pawn, tmad))
                        {
                            continue;
                        }
                        if (canUseWithEquippedWeapon && canUseIfViolentAbility)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                            if (mp.autocasting.type == AutocastType.OnTarget && Pawn.CurJob.targetA != null && Pawn.CurJob.targetA.Thing != null)
                            {
                                LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, Pawn.CurJob.targetA);
                                if (localTarget != null && localTarget.IsValid)
                                {
                                    Thing targetThing = localTarget.Thing;
                                    if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                    {
                                        continue;
                                    }
                                    if (mp.autocasting.requiresLoS && !TM_Calc.HasLoSFromTo(Pawn.Position, targetThing, Pawn, mp.autocasting.minRange, ability.Def.MainVerb.range))
                                    {
                                        continue;
                                    }
                                    if (mp.autocasting.maxRange != 0f && mp.autocasting.maxRange < (Pawn.Position - targetThing.Position).LengthHorizontal)
                                    {
                                        continue;
                                    }
                                    bool TE = mp.autocasting.targetEnemy && targetThing.Faction != null && targetThing.Faction.HostileTo(Pawn.Faction);
                                    if (TE && targetThing is Pawn)
                                    {
                                        Pawn targetPawn = targetThing as Pawn;
                                        if (targetPawn.Downed || targetPawn.IsPrisonerInPrisonCell())
                                        {
                                            continue;
                                        }
                                    }
                                    bool TN = mp.autocasting.targetNeutral && targetThing.Faction != null && !targetThing.Faction.HostileTo(Pawn.Faction);
                                    bool TNF = mp.autocasting.targetNoFaction && targetThing.Faction == null;
                                    bool TF = mp.autocasting.targetFriendly && targetThing.Faction == Pawn.Faction;
                                    if (!(TE || TN || TF || TNF))
                                    {
                                        continue;
                                    }
                                    if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                    {
                                        continue;
                                    }
                                    AutoCast.MagicAbility_OnTarget.TryExecute(this, tmad, ability, mp, targetThing, mp.autocasting.minRange, out castSuccess);
                                }
                            }
                            if (mp.autocasting.type == AutocastType.OnSelf)
                            {
                                LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, Pawn.CurJob.targetA);
                                if (localTarget != null && localTarget.IsValid)
                                {
                                    Pawn targetThing = localTarget.Pawn;
                                    if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                    {
                                        continue;
                                    }
                                    if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                    {
                                        continue;
                                    }
                                    AutoCast.MagicAbility_OnSelf.Evaluate(this, tmad, ability, mp, out castSuccess);
                                }
                            }
                            if (mp.autocasting.type == AutocastType.OnCell && Pawn.CurJob.targetA != null)
                            {
                                LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, Pawn.CurJob.targetA);
                                if (localTarget != null && localTarget.IsValid)
                                {
                                    IntVec3 targetThing = localTarget.Cell;
                                    if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                    {
                                        continue;
                                    }
                                    if (mp.autocasting.requiresLoS && !TM_Calc.HasLoSFromTo(Pawn.Position, targetThing, Pawn, mp.autocasting.minRange, ability.Def.MainVerb.range))
                                    {
                                        continue;
                                    }
                                    if (mp.autocasting.maxRange != 0f && mp.autocasting.maxRange < (Pawn.Position - targetThing).LengthHorizontal)
                                    {
                                        continue;
                                    }
                                    if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                    {
                                        continue;
                                    }
                                    AutoCast.MagicAbility_OnCell.TryExecute(this, tmad, ability, mp, targetThing, mp.autocasting.minRange, out castSuccess);
                                }
                            }
                            if (mp.autocasting.type == AutocastType.OnNearby)
                            {
                                if(mp.autocasting.maxRange == 0f)
                                {
                                    mp.autocasting.maxRange = mp.abilityDef.MainVerb.range;
                                }
                                LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, Pawn.CurJob.targetA);
                                if (localTarget != null && localTarget.IsValid)
                                {
                                    Thing targetThing = localTarget.Thing;
                                    if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                    {
                                        continue;
                                    }
                                    if (mp.autocasting.requiresLoS && !TM_Calc.HasLoSFromTo(Pawn.Position, targetThing, Pawn, mp.autocasting.minRange, ability.Def.MainVerb.range))
                                    {
                                        continue;
                                    }
                                    if (mp.autocasting.maxRange != 0f && mp.autocasting.maxRange < (Pawn.Position - targetThing.Position).LengthHorizontal)
                                    {
                                        continue;
                                    }
                                    bool TE = mp.autocasting.targetEnemy && targetThing.Faction != null && targetThing.Faction.HostileTo(Pawn.Faction);
                                    if (TE && targetThing is Pawn)
                                    {
                                        Pawn targetPawn = targetThing as Pawn;
                                        if (targetPawn.Downed || targetPawn.IsPrisonerInPrisonCell())
                                        {
                                            continue;
                                        }
                                    }
                                    bool TN = mp.autocasting.targetNeutral && targetThing.Faction != null && !targetThing.Faction.HostileTo(Pawn.Faction);
                                    bool TNF = mp.autocasting.targetNoFaction && targetThing.Faction == null;
                                    bool TF = mp.autocasting.targetFriendly && targetThing.Faction == Pawn.Faction;
                                    if (!(TE || TN || TF || TNF))
                                    {
                                        continue;
                                    }
                                    if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                    {
                                        continue;
                                    }
                                    AutoCast.MagicAbility_OnTarget.TryExecute(this, tmad, ability, mp, targetThing, mp.autocasting.minRange, out castSuccess);
                                }
                            }
                            if (castSuccess) return;
                        }
                    }

                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Summoner) || flagCM || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersS.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SummonMinion);
                        if (magicPower is { learned: true, autocast: true } && summonedMinions.Count < 4)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_SummonMinion);
                            AutoCast.MagicAbility_OnSelfPosition.Evaluate(this, TorannMagicDefOf.TM_SummonMinion, ability, magicPower, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if ((Pawn.story.traits.HasTrait(TorannMagicDefOf.Chronomancer) || isCustom) && !recallSet)
                    {
                        if (AbilityData.Powers.Any(p => p.Def == TorannMagicDefOf.TM_TimeMark))
                        {
                            MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_TimeMark);
                            if (magicPower != null && (magicPower.learned || spell_Recall) && magicPower.autocast && !Pawn.CurJob.playerForced)
                            {
                                PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_TimeMark);
                                AutoCast.MagicAbility_OnSelfPosition.Evaluate(this, TorannMagicDefOf.TM_TimeMark, ability, magicPower, out castSuccess);
                                if (castSuccess) return;
                            }
                        }
                    }
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Arcanist) || flagCM || isCustom)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersA)
                        {
                            if (current?.abilityDef == null) continue;

                            foreach (TMAbilityDef tmad in current.TMabilityDefs)
                            {
                                if ((tmad == TorannMagicDefOf.TM_Summon || tmad == TorannMagicDefOf.TM_Summon_I || tmad == TorannMagicDefOf.TM_Summon_II || tmad == TorannMagicDefOf.TM_Summon_III) && !Pawn.CurJob.playerForced)
                                {
                                    //Log.Message("evaluating " + tmad.defName);
                                    MagicPower magicPower = MagicData.MagicPowersA.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                    if (magicPower is { learned: true, autocast: true })
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                        float minDistance = ActualManaCost(tmad) * 150;
                                        AutoCast.Summon.Evaluate(this, tmad, ability, magicPower, minDistance, out castSuccess);
                                        if (castSuccess) return;
                                    }
                                }
                                if ((tmad == TorannMagicDefOf.TM_Blink || tmad == TorannMagicDefOf.TM_Blink_I || tmad == TorannMagicDefOf.TM_Blink_II || tmad == TorannMagicDefOf.TM_Blink_III))
                                {
                                    MagicPower magicPower = MagicData.MagicPowersA.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                    if (magicPower is { learned: true, autocast: true })
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                        float minDistance = ActualManaCost(tmad) * 240;
                                        AutoCast.Blink.Evaluate(this, tmad, ability, magicPower, minDistance, out castSuccess);
                                        if (castSuccess)
                                        {
                                            return;
                                        }
                                    }
                                    if (flagCM && magicPower != null && spell_Blink && !magicPower.learned && magicPower.autocast)
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                        float minDistance = ActualManaCost(tmad) * 200;
                                        AutoCast.Blink.Evaluate(this, tmad, ability, magicPower, minDistance, out castSuccess);
                                        if (castSuccess) return;
                                    }
                                }
                            }
                        }
                    }
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Druid) || flagCM || isCustom)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersD)
                        {
                            if (current is not { abilityDef: { }, learned: true } || Pawn.CurJob.playerForced) continue;

                            if (current.abilityDef == TorannMagicDefOf.TM_Regenerate)
                            {
                                MagicPower magicPower = MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == current.abilityDef);
                                if (magicPower is { learned: true, autocast: true })
                                {
                                    PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Regenerate);
                                    MagicPowerSkill pwr = MagicData.MagicPowerSkill_Regenerate.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Regenerate_pwr");
                                    if (pwr.level == 0)
                                    {
                                        AutoCast.HediffHealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_Regenerate, ability, magicPower, HediffDef.Named("TM_Regeneration"), 10f, out castSuccess);
                                    }
                                    else if (pwr.level == 1)
                                    {
                                        AutoCast.HediffHealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_Regenerate, ability, magicPower, HediffDef.Named("TM_Regeneration_I"), 12f, out castSuccess);
                                    }
                                    else if (pwr.level == 2)
                                    {
                                        AutoCast.HediffHealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_Regenerate, ability, magicPower, HediffDef.Named("TM_Regeneration_II"), 14f, out castSuccess);
                                    }
                                    else
                                    {
                                        AutoCast.HediffHealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_Regenerate, ability, magicPower, HediffDef.Named("TM_Regeneration_III"), 16f, out castSuccess);
                                    }
                                    if (castSuccess) return;
                                }
                            }
                            if (current.abilityDef == TorannMagicDefOf.TM_CureDisease)
                            {
                                MagicPower magicPower = MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == current.abilityDef);
                                if (magicPower is { learned: true, autocast: true })
                                {
                                    PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_CureDisease);
                                    MagicPowerSkill ver = MagicData.MagicPowerSkill_CureDisease.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_CureDisease_ver");

                                    List<string> afflictionList = new List<string>();
                                    foreach (TM_CategoryHediff chd in HediffCategoryList.Named("TM_Category_Hediffs").diseases)
                                    {
                                        if (chd.requiredSkillName == "TM_CureDisease_ver" && chd.requiredSkillLevel <= ver.level)
                                        {
                                            afflictionList.Add(chd.hediffDefname);
                                        }
                                    }
                                    AutoCast.CureSpell.Evaluate(this, TorannMagicDefOf.TM_CureDisease, ability, magicPower, afflictionList, out castSuccess);
                                    if (castSuccess) return;
                                }
                            }
                            if (current.abilityDef == TorannMagicDefOf.TM_RegrowLimb && spell_RegrowLimb)
                            {
                                MagicPower magicPower = MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == current.abilityDef);
                                bool workPriorities = true;
                                if (Pawn.CurJob?.workGiverDef?.workType != null)
                                {
                                    workPriorities = Pawn.workSettings.GetPriority(Pawn.CurJob.workGiverDef.workType) >= Pawn.workSettings.GetPriority(TorannMagicDefOf.TM_Magic);
                                }
                                if (magicPower is { learned: true, autocast: true } && !Pawn.CurJob.playerForced && workPriorities)
                                {
                                    Area tArea = TM_Calc.GetSeedOfRegrowthArea(Pawn.Map, false);
                                    if (tArea != null)
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_RegrowLimb);
                                        AutoCast.OnTarget_Spell.TryExecute(this, TorannMagicDefOf.TM_RegrowLimb, ability, magicPower, tArea.ActiveCells.RandomElement(), 40, out castSuccess);
                                        if (castSuccess) return;
                                    }
                                }
                            }
                        }
                    }
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Paladin) || flagCM || isCustom)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersP)
                        {
                            if (current is { abilityDef: { }, learned: true } && !Pawn.CurJob.playerForced)
                            {
                                foreach(TMAbilityDef tmad in current.TMabilityDefs)
                                { 
                                    if (tmad == TorannMagicDefOf.TM_Heal)
                                    {
                                        MagicPower magicPower = MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                        if (magicPower is { learned: true, autocast: true })
                                        {
                                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                            AutoCast.HealSpell.Evaluate(this, tmad, ability, magicPower, out castSuccess);
                                            if (castSuccess) return;
                                        }
                                    }
                                    if ((tmad == TorannMagicDefOf.TM_Shield || tmad == TorannMagicDefOf.TM_Shield_I || tmad == TorannMagicDefOf.TM_Shield_II || tmad == TorannMagicDefOf.TM_Shield_III))
                                    {
                                        MagicPower magicPower = MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                        if (magicPower is { learned: true, autocast: true })
                                        {
                                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                            AutoCast.Shield.Evaluate(this, tmad, ability, magicPower, out castSuccess);
                                            if (castSuccess) return;
                                        }
                                    }                                        
                                }
                            }
                        }
                    }
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Priest) || flagCM || isCustom)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersPR)
                        {
                            if (current is { abilityDef: { }, learned: true } && !Pawn.CurJob.playerForced)
                            {
                                foreach (TMAbilityDef tmad in current.TMabilityDefs)
                                {
                                    if (tmad == TorannMagicDefOf.TM_AdvancedHeal)
                                    {
                                        MagicPower magicPower = MagicData.MagicPowersPR.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                        if (magicPower is { learned: true, autocast: true })
                                        {
                                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                            AutoCast.HealSpell.EvaluateMinSeverity(this, tmad, ability, magicPower, 1f, out castSuccess);
                                            if (castSuccess) return;
                                        }
                                    }
                                    if (tmad == TorannMagicDefOf.TM_Purify)
                                    {
                                        MagicPower magicPower = MagicData.MagicPowersPR.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                        if (magicPower is { learned: true, autocast: true })
                                        {
                                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Purify);
                                            MagicPowerSkill ver = MagicData.MagicPowerSkill_Purify.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Purify_ver");
                                            AutoCast.HealPermanentSpell.Evaluate(this, TorannMagicDefOf.TM_Purify, ability, magicPower, out castSuccess);
                                            if (castSuccess) return;
                                            List<string> afflictionList = new List<string>();
                                            afflictionList.Clear();
                                            foreach(TM_CategoryHediff chd in HediffCategoryList.Named("TM_Category_Hediffs").ailments)
                                            {
                                                if(chd.requiredSkillName == "TM_Purify_ver" && chd.requiredSkillLevel <= ver.level)
                                                {
                                                    afflictionList.Add(chd.hediffDefname);
                                                }
                                            }
                                            AutoCast.CureSpell.Evaluate(this, TorannMagicDefOf.TM_Purify, ability, magicPower, afflictionList, out castSuccess);
                                            if (castSuccess) return;
                                            List<string> addictionList = new List<string>();
                                            foreach (TM_CategoryHediff chd in HediffCategoryList.Named("TM_Category_Hediffs").addictions)
                                            {
                                                if (chd.requiredSkillName == "TM_Purify_ver" && chd.requiredSkillLevel <= ver.level)
                                                {
                                                    addictionList.Add(chd.hediffDefname);
                                                }
                                            }
                                            if (ver.level >= 3)
                                            {
                                                IEnumerable<ChemicalDef> enumerable = from def in DefDatabase<ChemicalDef>.AllDefs
                                                                                      where (true)
                                                                                      select def;
                                                foreach (ChemicalDef addiction in enumerable)
                                                {
                                                    if (addiction.defName != "ROMV_VitaeAddiction" && addiction != TorannMagicDefOf.Luciferium)
                                                    {
                                                        addictionList.AddDistinct(addiction.defName);
                                                    }
                                                }
                                            }
                                            AutoCast.CureAddictionSpell.Evaluate(this, TorannMagicDefOf.TM_Purify, ability, magicPower, addictionList, out castSuccess);
                                            if (castSuccess) return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Enchanter) || flagCM || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersE.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Transmutate);
                        bool workPriorities = true;
                        if (Pawn.CurJob?.workGiverDef?.workType != null)
                        {
                            workPriorities = Pawn.workSettings.GetPriority(Pawn.CurJob.workGiverDef.workType) >= Pawn.workSettings.GetPriority(TorannMagicDefOf.TM_Magic);
                        }
                        if (magicPower is { learned: true, autocast: true } && !Pawn.CurJob.playerForced && workPriorities)
                        {
                            Area tArea = TM_Calc.GetTransmutateArea(Pawn.Map, false);
                            if (tArea != null)
                            {
                                Thing transmuteThing = TM_Calc.GetTransmutableThingFromCell(tArea.ActiveCells.RandomElement(), Pawn, out _, out _, out _, out _, out _);
                                if (transmuteThing != null)
                                {
                                    PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Transmutate);
                                    AutoCast.OnTarget_Spell.TryExecute(this, TorannMagicDefOf.TM_Transmutate, ability, magicPower, transmuteThing, 50, out castSuccess);
                                    if (castSuccess) return;
                                }
                            }
                        }
                    }
                    if ((spell_MechaniteReprogramming && Pawn.story.traits.HasTrait(TorannMagicDefOf.Technomancer)) || flagCM || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_MechaniteReprogramming);
                        if (magicPower is { learned: true, autocast: true } && !Pawn.CurJob.playerForced)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_MechaniteReprogramming);
                            List<string> afflictionList = new List<string>();
                            foreach (TM_CategoryHediff chd in HediffCategoryList.Named("TM_Category_Hediffs").mechanites)
                            {
                                afflictionList.Add(chd.hediffDefname);                                
                            }
                            AutoCast.CureSpell.Evaluate(this, TorannMagicDefOf.TM_MechaniteReprogramming, ability, magicPower, afflictionList, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if (spell_Heal && !Pawn.story.traits.HasTrait(TorannMagicDefOf.Paladin) && !isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Heal);
                        if (magicPower.learned && magicPower.autocast && !Pawn.CurJob.playerForced)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Heal);
                            AutoCast.HealSpell.Evaluate(this, TorannMagicDefOf.TM_Heal, ability, magicPower, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if (spell_TransferMana || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_TransferMana);
                        if (magicPower.learned && magicPower.autocast && !Pawn.CurJob.playerForced)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_TransferMana);
                            AutoCast.TransferManaSpell.Evaluate(this, TorannMagicDefOf.TM_TransferMana, ability, magicPower, false, false, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if (spell_SiphonMana || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SiphonMana);
                        if (magicPower.learned && magicPower.autocast && !Pawn.CurJob.playerForced)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_SiphonMana);
                            AutoCast.TransferManaSpell.Evaluate(this, TorannMagicDefOf.TM_SiphonMana, ability, magicPower, false, true, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if (spell_CauterizeWound || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_CauterizeWound);
                        if (magicPower.learned && magicPower.autocast && !Pawn.CurJob.playerForced)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_CauterizeWound);
                            AutoCast.HealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_CauterizeWound, ability, magicPower, 40f, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if (spell_SpellMending || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SpellMending);
                        if (magicPower.learned && magicPower.autocast && !Pawn.CurJob.playerForced)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_SpellMending);
                            AutoCast.SpellMending.Evaluate(this, TorannMagicDefOf.TM_SpellMending, ability, magicPower, HediffDef.Named("SpellMendingHD"), out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if (spell_Teach || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_TeachMagic);
                        if (magicPower.learned && magicPower.autocast && !Pawn.CurJob.playerForced)
                        {
                            if (Pawn.CurJobDef.joyKind != null || Pawn.CurJobDef == JobDefOf.Wait_Wander || Pawn.CurJobDef == JobDefOf.GotoWander)
                            {
                                PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_TeachMagic);
                                AutoCast.Teach.Evaluate(this, TorannMagicDefOf.TM_TeachMagic, ability, magicPower, out castSuccess);
                                if (castSuccess) return;
                            }
                        }
                    }
                    if (spell_SummonMinion && !Pawn.story.traits.HasTrait(TorannMagicDefOf.Summoner) && !isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersS.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SummonMinion);
                        if (magicPower.learned && magicPower.autocast && !Pawn.CurJob.playerForced && summonedMinions.Count < 4)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_SummonMinion);
                            AutoCast.MagicAbility_OnSelfPosition.Evaluate(this, TorannMagicDefOf.TM_SummonMinion, ability, magicPower, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if (spell_DirtDevil || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_DirtDevil);
                        if (magicPower is { learned: true, autocast: true } && !Pawn.CurJob.playerForced && Pawn.GetRoom() != null)
                        {
                            float roomCleanliness = Pawn.GetRoom().GetStat(RoomStatDefOf.Cleanliness);

                            if (roomCleanliness < -3f)
                            {
                                PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_DirtDevil);
                                AutoCast.MagicAbility_OnSelfPosition.Evaluate(this, TorannMagicDefOf.TM_DirtDevil, ability, magicPower, out castSuccess);
                                if (castSuccess) return;
                            }
                        }
                    }
                    if (spell_Blink && !Pawn.story.traits.HasTrait(TorannMagicDefOf.Arcanist) && !flagCM && !isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersA.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Blink);
                        if (magicPower.autocast)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Blink);
                            float minDistance = ActualManaCost(TorannMagicDefOf.TM_Blink) * 200;
                            AutoCast.Blink.Evaluate(this, TorannMagicDefOf.TM_Blink, ability, magicPower, minDistance, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                }

                //combat (drafted) spells
                if (Pawn.drafter != null && Pawn.Drafted && Pawn.drafter.FireAtWill && Pawn.CurJob.def != JobDefOf.Goto && Mana != null && Mana.CurLevelPercentage >= Settings.Instance.autocastCombatMinThreshold)
                {
                    foreach (MagicPower mp in MagicData.MagicPowersCustom)
                    {
                        if (mp.learned && mp.autocast && mp.autocasting is { magicUser: true, drafted: true })
                        {
                            TMAbilityDef tmad = mp.TMabilityDefs[mp.level] as TMAbilityDef; // issues with index?
                            bool canUseWithEquippedWeapon = true;
                            bool canUseIfViolentAbility = !Pawn.story.DisabledWorkTagsBackstoryAndTraits.HasFlag(WorkTags.Violent) || !tmad.MainVerb.isViolent;
                            if (!TM_Calc.HasResourcesForAbility(Pawn, tmad))
                            {
                                continue;
                            }
                            if (canUseWithEquippedWeapon && canUseIfViolentAbility)
                            {
                                PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                if (mp.autocasting.type == AutocastType.OnTarget && Pawn.TargetCurrentlyAimingAt != null && Pawn.TargetCurrentlyAimingAt.Thing != null)
                                {
                                    LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, Pawn.TargetCurrentlyAimingAt);
                                    if (localTarget != null && localTarget.IsValid)
                                    {
                                        Thing targetThing = localTarget.Thing;
                                        if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.requiresLoS && !TM_Calc.HasLoSFromTo(Pawn.Position, targetThing, Pawn, mp.autocasting.minRange, ability.Def.MainVerb.range))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.maxRange != 0f && mp.autocasting.maxRange < (Pawn.Position - targetThing.Position).LengthHorizontal)
                                        {
                                            continue;
                                        }
                                        bool TE = mp.autocasting.targetEnemy && targetThing.Faction != null && targetThing.Faction.HostileTo(Pawn.Faction);
                                        if (TE && targetThing is Pawn)
                                        {
                                            Pawn targetPawn = targetThing as Pawn;
                                            if (targetPawn.Downed || targetPawn.IsPrisonerInPrisonCell())
                                            {
                                                continue;
                                            }
                                        }
                                        bool TN = mp.autocasting.targetNeutral && targetThing.Faction != null && !targetThing.Faction.HostileTo(Pawn.Faction);
                                        bool TNF = mp.autocasting.targetNoFaction && targetThing.Faction == null;
                                        bool TF = mp.autocasting.targetFriendly && targetThing.Faction == Pawn.Faction;
                                        if (!(TE || TN || TF || TNF))
                                        {
                                            continue;
                                        }
                                        if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                        {
                                            continue;
                                        }
                                        AutoCast.MagicAbility_OnTarget.TryExecute(this, tmad, ability, mp, targetThing, mp.autocasting.minRange, out castSuccess);
                                    }
                                }
                                if (mp.autocasting.type == AutocastType.OnSelf)
                                {
                                    LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, Pawn);
                                    if (localTarget != null && localTarget.IsValid)
                                    {
                                        Pawn targetThing = localTarget.Pawn;
                                        if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                        {
                                            continue;
                                        }
                                        if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                        {
                                            continue;
                                        }
                                        AutoCast.MagicAbility_OnSelf.Evaluate(this, tmad, ability, mp, out castSuccess);
                                    }
                                }
                                if (mp.autocasting.type == AutocastType.OnCell && Pawn.TargetCurrentlyAimingAt != null)
                                {
                                    LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, Pawn.TargetCurrentlyAimingAt);
                                    if (localTarget != null && localTarget.IsValid)
                                    {
                                        IntVec3 targetThing = localTarget.Cell;
                                        if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.requiresLoS && !TM_Calc.HasLoSFromTo(Pawn.Position, targetThing, Pawn, mp.autocasting.minRange, ability.Def.MainVerb.range))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.maxRange != 0f && mp.autocasting.maxRange < (Pawn.Position - targetThing).LengthHorizontal)
                                        {
                                            continue;
                                        }
                                        if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                        {
                                            continue;
                                        }
                                        AutoCast.MagicAbility_OnCell.TryExecute(this, tmad, ability, mp, targetThing, mp.autocasting.minRange, out castSuccess);
                                    }
                                }
                                if (mp.autocasting.type == AutocastType.OnNearby)
                                {
                                    LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, Pawn.TargetCurrentlyAimingAt);
                                    if (localTarget != null && localTarget.IsValid)
                                    {
                                        Thing targetThing = localTarget.Thing;
                                        if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.requiresLoS && !TM_Calc.HasLoSFromTo(Pawn.Position, targetThing, Pawn, mp.autocasting.minRange, ability.Def.MainVerb.range))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.maxRange != 0f && mp.autocasting.maxRange < (Pawn.Position - targetThing.Position).LengthHorizontal)
                                        {
                                            continue;
                                        }
                                        bool TE = mp.autocasting.targetEnemy && targetThing.Faction != null && targetThing.Faction.HostileTo(Pawn.Faction);
                                        if (TE && targetThing is Pawn)
                                        {
                                            Pawn targetPawn = targetThing as Pawn;
                                            if (targetPawn.Downed || targetPawn.IsPrisonerInPrisonCell())
                                            {
                                                continue;
                                            }
                                        }
                                        bool TN = mp.autocasting.targetNeutral && targetThing.Faction != null && !targetThing.Faction.HostileTo(Pawn.Faction);
                                        bool TNF = mp.autocasting.targetNoFaction && targetThing.Faction == null;
                                        bool TF = mp.autocasting.targetFriendly && targetThing.Faction == Pawn.Faction;
                                        if (!(TE || TN || TF || TNF))
                                        {
                                            continue;
                                        }
                                        if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                        {
                                            continue;
                                        }
                                        AutoCast.MagicAbility_OnTarget.TryExecute(this, tmad, ability, mp, targetThing, mp.autocasting.minRange, out castSuccess);
                                    }
                                }
                                if (castSuccess) return;
                            }
                        }
                    }

                    if ((Pawn.story.traits.HasTrait(TorannMagicDefOf.InnerFire) || flagCM || isCustom) && Pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.Violent)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersIF)
                        {
                            if (current is { abilityDef: { }, learned: true })
                            {
                                if (current.abilityDef == TorannMagicDefOf.TM_Firebolt)
                                {
                                    MagicPower magicPower = MagicData.MagicPowersIF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == current.abilityDef);
                                    if (magicPower is { learned: true, autocast: true })
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Firebolt);
                                        AutoCast.DamageSpell.Evaluate(this, TorannMagicDefOf.TM_Firebolt, ability, magicPower, out castSuccess);
                                        if (castSuccess) return;
                                    }
                                }
                            }
                        }
                    }
                    if ((Pawn.story.traits.HasTrait(TorannMagicDefOf.HeartOfFrost) || flagCM || isCustom) && Pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.Violent)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersHoF)
                        {
                            if (current is { abilityDef: { }, learned: true })
                            {
                                foreach (TMAbilityDef tmad in current.TMabilityDefs)
                                {
                                    if (tmad == TorannMagicDefOf.TM_Icebolt)
                                    {
                                        MagicPower magicPower = MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Icebolt);
                                        if (magicPower is { learned: true, autocast: true })
                                        {
                                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Icebolt);
                                            AutoCast.DamageSpell.Evaluate(this, TorannMagicDefOf.TM_Icebolt, ability, magicPower, out castSuccess);
                                            if (castSuccess) return;
                                        }
                                    }
                                    else if ((tmad == TorannMagicDefOf.TM_FrostRay || tmad == TorannMagicDefOf.TM_FrostRay_I || tmad == TorannMagicDefOf.TM_FrostRay_II || tmad == TorannMagicDefOf.TM_FrostRay_III))
                                    {
                                        MagicPower magicPower = MagicData.MagicPowersHoF.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                        if (magicPower is { learned: true, autocast: true })
                                        {
                                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                            AutoCast.DamageSpell.Evaluate(this, tmad, ability, magicPower, out castSuccess);
                                            if (castSuccess) return;
                                        }                                       
                                    }
                                }
                            }
                        }
                    }
                    if ((Pawn.story.traits.HasTrait(TorannMagicDefOf.StormBorn) || flagCM || isCustom) && Pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.Violent)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersSB)
                        {
                            if (current is not { abilityDef: { }, learned: true }) continue;
                            if (current.abilityDef != TorannMagicDefOf.TM_LightningBolt) continue;

                            MagicPower magicPower = MagicData.MagicPowersSB.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_LightningBolt);
                            if (magicPower is { learned: true, autocast: true })
                            {
                                PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_LightningBolt);
                                AutoCast.DamageSpell.Evaluate(this, TorannMagicDefOf.TM_LightningBolt, ability, magicPower, out castSuccess);
                                if (castSuccess) return;
                            }
                        }
                    }
                    if ((Pawn.story.traits.HasTrait(TorannMagicDefOf.Arcanist) || flagCM || isCustom) && Pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.Violent)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersA)
                        {
                            if (current is not { abilityDef: { }, learned: true }) continue;

                            foreach (TMAbilityDef tmad in current.TMabilityDefs)
                            {
                                if ((tmad == TorannMagicDefOf.TM_MagicMissile || tmad == TorannMagicDefOf.TM_MagicMissile_I || tmad == TorannMagicDefOf.TM_MagicMissile_II || tmad == TorannMagicDefOf.TM_MagicMissile_III))
                                {
                                    MagicPower magicPower = MagicData.MagicPowersA.First(mp => mp.abilityDef == tmad);
                                    if (magicPower is { learned: true, autocast: true })
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault(pa => pa.Def == tmad);
                                        AutoCast.DamageSpell.Evaluate(this, tmad, ability, magicPower, out castSuccess);
                                        if (castSuccess) return;
                                    }
                                }
                            }
                        }
                    }
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Druid) || flagCM || isCustom)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersD)
                        {
                            if (current is { abilityDef: { }, learned: true })
                            {
                                if (current.abilityDef == TorannMagicDefOf.TM_Poison && Pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.Violent)
                                {
                                    MagicPower magicPower = MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Poison);
                                    if (magicPower is { learned: true, autocast: true })
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Poison);
                                        AutoCast.HediffSpell.EvaluateMinRange(this, TorannMagicDefOf.TM_Poison, ability, magicPower, HediffDef.Named("TM_Poisoned_HD"), 10, out castSuccess);
                                        if (castSuccess) return;
                                    }
                                }
                                if (current.abilityDef == TorannMagicDefOf.TM_Regenerate)
                                {
                                    MagicPower magicPower = MagicData.MagicPowersD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Regenerate);
                                    if (magicPower is { learned: true, autocast: true })
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Regenerate);
                                        MagicPowerSkill pwr = MagicData.MagicPowerSkill_Regenerate.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Regenerate_pwr");
                                        if (pwr.level == 0)
                                        {
                                            AutoCast.HediffHealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_Regenerate, ability, magicPower, HediffDef.Named("TM_Regeneration"), 10f, out castSuccess);
                                            if (castSuccess) return;
                                        }
                                        else if (pwr.level == 1)
                                        {
                                            AutoCast.HediffHealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_Regenerate, ability, magicPower, HediffDef.Named("TM_Regeneration_I"), 12f, out castSuccess);
                                            if (castSuccess) return;
                                        }
                                        else if (pwr.level == 2)
                                        {
                                            AutoCast.HediffHealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_Regenerate, ability, magicPower, HediffDef.Named("TM_Regeneration_II"), 14f, out castSuccess);
                                            if (castSuccess) return;
                                        }
                                        else
                                        {
                                            AutoCast.HediffHealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_Regenerate, ability, magicPower, HediffDef.Named("TM_Regeneration_III"), 16f, out castSuccess);
                                            if (castSuccess) return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if ((Pawn.story.traits.HasTrait(TorannMagicDefOf.Succubus) || flagCM || isCustom) && Pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.Violent)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersSD)
                        {
                            if (current is { abilityDef: { }, learned: true })
                            {
                                foreach (TMAbilityDef tmad in current.TMabilityDefs)
                                {
                                    if ((tmad == TorannMagicDefOf.TM_ShadowBolt || tmad == TorannMagicDefOf.TM_ShadowBolt_I || tmad == TorannMagicDefOf.TM_ShadowBolt_II || tmad == TorannMagicDefOf.TM_ShadowBolt_III))
                                    {
                                        MagicPower magicPower = MagicData.MagicPowersSD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                        if (magicPower is { learned: true, autocast: true })
                                        {
                                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                            AutoCast.DamageSpell.Evaluate(this, tmad, ability, magicPower, out castSuccess);
                                            if (castSuccess) return;
                                        }                                        
                                    }
                                }
                            }
                        }
                    }
                    if ((Pawn.story.traits.HasTrait(TorannMagicDefOf.Warlock) || flagCM || isCustom) && Pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.Violent)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersWD)
                        {
                            if (current is { abilityDef: { }, learned: true })
                            {
                                foreach (TMAbilityDef tmad in current.TMabilityDefs)
                                {
                                    if ((tmad == TorannMagicDefOf.TM_ShadowBolt || tmad == TorannMagicDefOf.TM_ShadowBolt_I || tmad == TorannMagicDefOf.TM_ShadowBolt_II || tmad == TorannMagicDefOf.TM_ShadowBolt_III))
                                    {
                                        MagicPower magicPower = MagicData.MagicPowersWD.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                        if (magicPower is { learned: true, autocast: true })
                                        {
                                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                            AutoCast.DamageSpell.Evaluate(this, tmad, ability, magicPower, out castSuccess);
                                            if (castSuccess) return;
                                        }                                        
                                    }
                                }
                            }
                        }
                    }
                    if ((Pawn.story.traits.HasTrait(TorannMagicDefOf.Paladin) || flagCM || isCustom))
                    {
                        foreach (MagicPower current in MagicData.MagicPowersP)
                        {
                            if (current is not { abilityDef: { }, learned: true }) continue;

                            foreach (TMAbilityDef tmad in current.TMabilityDefs)
                            {
                                if (tmad == TorannMagicDefOf.TM_Heal)
                                {
                                    MagicPower magicPower = MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                    if (magicPower is { learned: true, autocast: true })
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                        AutoCast.HealSpell.Evaluate(this, tmad, ability, magicPower, out castSuccess);
                                        if (castSuccess) return;
                                    }
                                }
                                if ((tmad == TorannMagicDefOf.TM_Shield || tmad == TorannMagicDefOf.TM_Shield_I || tmad == TorannMagicDefOf.TM_Shield_II || tmad == TorannMagicDefOf.TM_Shield_III))
                                {
                                    MagicPower magicPower = MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == tmad);
                                    if (magicPower is { learned: true, autocast: true })
                                    {
                                        PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                        AutoCast.Shield.Evaluate(this, tmad, ability, magicPower, out castSuccess);
                                        if (castSuccess) return;
                                    }
                                }
                            }
                        }
                    }
                    if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Priest) || flagCM || isCustom)
                    {
                        foreach (MagicPower current in MagicData.MagicPowersPR)
                        {
                            if (current is not { abilityDef: { }, learned: true }) continue;
                            if (current.abilityDef != TorannMagicDefOf.TM_AdvancedHeal) continue;

                            MagicPower magicPower = MagicData.MagicPowersPR.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_AdvancedHeal);
                            if (magicPower is { learned: true, autocast: true })
                            {
                                PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_AdvancedHeal);
                                AutoCast.HealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_AdvancedHeal, ability, magicPower, 1f, out castSuccess);
                                if (castSuccess) return;
                            }
                        }
                    }
                    if ((spell_Heal && !Pawn.story.traits.HasTrait(TorannMagicDefOf.Paladin)))
                    {
                        MagicPower magicPower = MagicData.MagicPowersP.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Heal);
                        if (magicPower.autocast)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_Heal);
                            AutoCast.HealSpell.Evaluate(this, TorannMagicDefOf.TM_Heal, ability, magicPower, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if (spell_SiphonMana || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SiphonMana);
                        if (magicPower.learned && magicPower.autocast)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_SiphonMana);
                            AutoCast.TransferManaSpell.Evaluate(this, TorannMagicDefOf.TM_SiphonMana, ability, magicPower, true, true, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if (spell_CauterizeWound || isCustom)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_CauterizeWound);
                        if (magicPower.learned && magicPower.autocast)
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_CauterizeWound);
                            AutoCast.HealSpell.EvaluateMinSeverity(this, TorannMagicDefOf.TM_CauterizeWound, ability, magicPower, 40f, out castSuccess);
                            if (castSuccess) return;
                        }
                    }
                    if ((spell_ArcaneBolt || isCustom) && Pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.Violent)
                    {
                        MagicPower magicPower = MagicData.MagicPowersStandalone.FirstOrDefault<MagicPower>((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ArcaneBolt);
                        if (magicPower is { learned: true, autocast: true })
                        {
                            PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == TorannMagicDefOf.TM_ArcaneBolt);
                            AutoCast.DamageSpell.Evaluate(this, TorannMagicDefOf.TM_ArcaneBolt, ability, magicPower, out castSuccess);
                            // if (castSuccess) return;
                        }
                    }
                }
            }
        }

        public void ResolveAIAutoCast()
        {
            
            if (Settings.Instance.AICasting && Pawn.jobs != null && Pawn.CurJob != null && Pawn.CurJob.def != TorannMagicDefOf.TMCastAbilityVerb && Pawn.CurJob.def != TorannMagicDefOf.TMCastAbilitySelf &&
                Pawn.CurJob.def != JobDefOf.Ingest && Pawn.CurJob.def != JobDefOf.ManTurret && Pawn.GetPosture() == PawnPosture.Standing)
            {
                bool castSuccess = false;
                if (Mana != null && Mana.CurLevelPercentage >= Settings.Instance.autocastMinThreshold)
                {
                    foreach (MagicPower mp in MagicData.AllMagicPowersWithSkills)
                    {
                        if (mp.learned && mp.autocasting is { magicUser: true, AIUsable: true })
                        {                            
                            //try
                            //{                             
                            TMAbilityDef tmad = mp.TMabilityDefs[mp.level] as TMAbilityDef; // issues with index?
                            bool canUseWithEquippedWeapon = true;
                            bool canUseIfViolentAbility = !Pawn.story.DisabledWorkTagsBackstoryAndTraits.HasFlag(WorkTags.Violent) || !tmad.MainVerb.isViolent;
                            if (!TM_Calc.HasResourcesForAbility(Pawn, tmad))
                            {
                                continue;
                            }
                            if (canUseWithEquippedWeapon && canUseIfViolentAbility)
                            {
                                PawnAbility ability = AbilityData.Powers.FirstOrDefault((PawnAbility x) => x.Def == tmad);
                                LocalTargetInfo currentTarget = Pawn.TargetCurrentlyAimingAt != null ? Pawn.TargetCurrentlyAimingAt : (Pawn.CurJob != null ? Pawn.CurJob.targetA : null);
                                if (mp.autocasting.type == AutocastType.OnTarget && currentTarget != null)
                                {
                                    LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, currentTarget);
                                    if (localTarget != null && localTarget.IsValid)
                                    {
                                        Thing targetThing = localTarget.Thing;
                                        if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.requiresLoS && !TM_Calc.HasLoSFromTo(Pawn.Position, targetThing, Pawn, mp.autocasting.minRange, ability.Def.MainVerb.range))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.maxRange != 0f && mp.autocasting.maxRange < (Pawn.Position - targetThing.Position).LengthHorizontal)
                                        {
                                            continue;
                                        }
                                        bool TE = mp.autocasting.targetEnemy && targetThing.Faction != null && targetThing.Faction.HostileTo(Pawn.Faction);
                                        if(TE && targetThing is Pawn)
                                        {
                                            Pawn targetPawn = targetThing as Pawn;
                                            if(targetPawn.Downed || targetPawn.IsPrisoner)
                                            {
                                                continue;
                                            }
                                        }
                                        bool TN = mp.autocasting.targetNeutral && targetThing.Faction != null && !targetThing.Faction.HostileTo(Pawn.Faction);                                        
                                        if (TN && targetThing is Pawn)
                                        {
                                            Pawn targetPawn = targetThing as Pawn;
                                            if (targetPawn.Downed || targetPawn.IsPrisoner)
                                            {
                                                continue;
                                            }
                                            if (mp.abilityDef.MainVerb.isViolent && !targetPawn.InMentalState)
                                            {
                                                continue;
                                            }
                                        }
                                        bool TNF = mp.autocasting.targetNoFaction && targetThing.Faction == null;
                                        if (TNF && targetThing is Pawn)
                                        {
                                            Pawn targetPawn = targetThing as Pawn;
                                            if (targetPawn.Downed || targetPawn.IsPrisoner)
                                            {
                                                continue;
                                            }                                            
                                        }
                                        bool TF = mp.autocasting.targetFriendly && targetThing.Faction == Pawn.Faction;
                                        if (!(TE || TN || TF || TNF))
                                        {
                                            continue;
                                        }
                                        if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                        {
                                            continue;
                                        }
                                        AutoCast.MagicAbility_OnTarget.TryExecute(this, tmad, ability, mp, targetThing, mp.autocasting.minRange, out castSuccess);
                                    }
                                }
                                if (mp.autocasting.type == AutocastType.OnSelf)
                                {
                                    LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, Pawn);
                                    if (localTarget != null && localTarget.IsValid)
                                    {
                                        Pawn targetThing = localTarget.Pawn;
                                        if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                        {
                                            continue;
                                        }
                                        if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                        {
                                            continue;
                                        }
                                        AutoCast.MagicAbility_OnSelf.Evaluate(this, tmad, ability, mp, out castSuccess);
                                    }
                                }
                                if (mp.autocasting.type == AutocastType.OnCell && currentTarget != null)
                                {
                                    LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, currentTarget);
                                    if (localTarget != null && localTarget.IsValid)
                                    {
                                        IntVec3 targetThing = localTarget.Cell;
                                        if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.requiresLoS && !TM_Calc.HasLoSFromTo(Pawn.Position, targetThing, Pawn, mp.autocasting.minRange, ability.Def.MainVerb.range))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.maxRange != 0f && mp.autocasting.maxRange < (Pawn.Position - targetThing).LengthHorizontal)
                                        {
                                            continue;
                                        }
                                        if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                        {
                                            continue;
                                        }
                                        AutoCast.MagicAbility_OnCell.TryExecute(this, tmad, ability, mp, targetThing, mp.autocasting.minRange, out castSuccess);
                                    }
                                }
                                if (mp.autocasting.type == AutocastType.OnNearby)
                                {
                                    LocalTargetInfo localTarget = TM_Calc.GetAutocastTarget(Pawn, mp.autocasting, currentTarget);
                                    if (localTarget != null && localTarget.IsValid)
                                    {
                                        Thing targetThing = localTarget.Thing;
                                        if (!mp.autocasting.ValidType(mp.autocasting.GetTargetType, localTarget))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.requiresLoS && !TM_Calc.HasLoSFromTo(Pawn.Position, targetThing, Pawn, mp.autocasting.minRange, ability.Def.MainVerb.range))
                                        {
                                            continue;
                                        }
                                        if (mp.autocasting.maxRange != 0f && mp.autocasting.maxRange < (Pawn.Position - targetThing.Position).LengthHorizontal)
                                        {
                                            continue;
                                        }
                                        bool TE = mp.autocasting.targetEnemy && targetThing.Faction != null && targetThing.Faction.HostileTo(Pawn.Faction);
                                        if (TE && targetThing is Pawn)
                                        {
                                            Pawn targetPawn = targetThing as Pawn;
                                            if (targetPawn.Downed || targetPawn.IsPrisoner)
                                            {
                                                continue;
                                            }
                                        }
                                        bool TN = mp.autocasting.targetNeutral && targetThing.Faction != null && !targetThing.Faction.HostileTo(Pawn.Faction);
                                        if (TN && targetThing is Pawn)
                                        {
                                            Pawn targetPawn = targetThing as Pawn;
                                            if (targetPawn.Downed || targetPawn.IsPrisoner)
                                            {
                                                continue;
                                            }
                                            if (mp.abilityDef.MainVerb.isViolent && !targetPawn.InMentalState)
                                            {
                                                continue;
                                            }
                                        }
                                        bool TNF = mp.autocasting.targetNoFaction && targetThing.Faction == null;
                                        if (TNF && targetThing is Pawn)
                                        {
                                            Pawn targetPawn = targetThing as Pawn;
                                            if (targetPawn.Downed || targetPawn.IsPrisoner)
                                            {
                                                continue;
                                            }
                                        }
                                        bool TF = mp.autocasting.targetFriendly && targetThing.Faction == Pawn.Faction;
                                        if (!(TE || TN || TF || TNF))
                                        {
                                            continue;
                                        }
                                        if (!mp.autocasting.ValidConditions(Pawn, targetThing))
                                        {
                                            continue;
                                        }
                                        AutoCast.MagicAbility_OnTarget.TryExecute(this, tmad, ability, mp, targetThing, mp.autocasting.minRange, out castSuccess);
                                    }
                                }
                            }
                            //}
                            //catch
                            //{
                            //    Log.Message("no index found at " + mp.level + " for " + mp.abilityDef.defName);
                            //}
                        }
                        if (castSuccess) goto AIAutoCastExit;
                    }
                    AIAutoCastExit:;
                }
            }
        }

        private void ResolveSpiritOfLight()
        {
            if(SoL != null)
            {
                if(!spell_EqualizeLight)
                {
                    AddDistinctPawnAbility(TorannMagicDefOf.TM_SoL_Equalize);
                    spell_EqualizeLight = true;
                }
            }
            if(SoL == null)
            {
                if(spell_CreateLight || spell_EqualizeLight)
                {
                    RemovePawnAbility(TorannMagicDefOf.TM_SoL_Equalize);
                    spell_EqualizeLight = false;
                }
            }
        }

        private void ResolveEarthSpriteAction()
        {
            MagicPowerSkill magicPowerSkill = MagicData.MagicPowerSkill_EarthSprites.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_EarthSprites_pwr");
            //Log.Message("resolving sprites");
            earthSpriteMap ??= Pawn.Map;
            switch (earthSpriteType.Value)
            {
                //mining stone
                case 1:
                {
                    //Log.Message("stone");
                    Building mineTarget = earthSprites.GetFirstBuilding(earthSpriteMap);
                    nextEarthSpriteAction = Find.TickManager.TicksGame + Mathf.RoundToInt((300 * (1 - (.1f * magicPowerSkill.level))) / arcaneDmg);
                    TM_MoteMaker.ThrowGenericFleck(TorannMagicDefOf.SparkFlash, earthSprites.ToVector3Shifted(), earthSpriteMap, Rand.Range(2f, 5f), .05f, 0f, .1f, 0, 0f, 0f, 0f);
                    var mineable = mineTarget as Mineable;
                    const int num = 80;
                    if (mineable != null && mineTarget.HitPoints > num)
                    {
                        var dinfo = new DamageInfo(DamageDefOf.Mining, num, 0, -1f, Pawn);
                        mineTarget.TakeDamage(dinfo);

                        if (Rand.Chance(Settings.Instance.magicyteChance * 2))
                        {
                            Thing thing = ThingMaker.MakeThing(TorannMagicDefOf.RawMagicyte);
                            thing.stackCount = Rand.Range(8, 16);
                            GenPlace.TryPlaceThing(thing, earthSprites, earthSpriteMap, ThingPlaceMode.Near);
                        }
                    }
                    else if (mineable != null && mineTarget.HitPoints <= num)
                    {
                        mineable.DestroyMined(Pawn);
                    }

                    if (!mineable.DestroyedOrNull()) return;

                    IntVec3 oldEarthSpriteLoc = earthSprites;
                    Building newMineSpot;
                    if (earthSpritesInArea)
                    {
                        //Log.Message("moving in area");
                        List<IntVec3> spriteAreaCells = GenRadial.RadialCellsAround(oldEarthSpriteLoc, 6f, false).ToList();
                        spriteAreaCells.Shuffle();
                        for (int i = 0; i < spriteAreaCells.Count; i++)
                        {
                            IntVec3 intVec = spriteAreaCells[i];
                            newMineSpot = intVec.GetFirstBuilding(earthSpriteMap);
                            if (newMineSpot != null && !intVec.Fogged(earthSpriteMap) && TM_Calc.GetSpriteArea() != null && TM_Calc.GetSpriteArea().ActiveCells.Contains(intVec))
                            {
                                mineable = newMineSpot as Mineable;
                                if (mineable != null)
                                {
                                    earthSprites = intVec;
                                    //Log.Message("assigning");
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            IntVec3 intVec = earthSprites + GenAdj.AdjacentCells.RandomElement();
                            newMineSpot = intVec.GetFirstBuilding(earthSpriteMap);
                            if (newMineSpot != null)
                            {
                                mineable = newMineSpot as Mineable;
                                if (mineable != null)
                                {
                                    earthSprites = intVec;
                                    i = 20;
                                }
                            }
                        }
                    }

                    if (oldEarthSpriteLoc == earthSprites)
                    {
                        earthSpriteType.Set(0);
                        earthSprites = IntVec3.Invalid;
                        earthSpritesInArea = false;
                    }

                    break;
                }
                //transforming soil
                case 2:
                {
                    //Log.Message("earth");
                    nextEarthSpriteAction = Find.TickManager.TicksGame + Mathf.RoundToInt((24000 * (1 - (.1f * magicPowerSkill.level))) / arcaneDmg);
                    for (int m = 0; m < 4; m++)
                    {
                        TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_ThickDust, earthSprites.ToVector3Shifted(), earthSpriteMap, Rand.Range(.3f, .5f), Rand.Range(.2f, .3f), .05f, Rand.Range(.4f, .6f), Rand.Range(-20, 20), Rand.Range(.5f, 1f), Rand.Range(0, 360), Rand.Range(0, 360));
                    }
                    Map map = earthSpriteMap;
                    IntVec3 curCell = earthSprites;
                    TerrainDef terrain = curCell.GetTerrain(map);
                    if (Rand.Chance(.8f))
                    {
                        Thing thing = ThingMaker.MakeThing(TorannMagicDefOf.RawMagicyte);
                        thing.stackCount = Rand.Range(10, 20);
                        GenPlace.TryPlaceThing(thing, earthSprites, earthSpriteMap, ThingPlaceMode.Near);
                    }

                    if (!curCell.InBoundsWithNullCheck(map) || !curCell.IsValid || terrain == null) return;

                    switch (terrain.defName)
                    {
                        case "MarshyTerrain" or "Mud" or "Marsh":
                            map.terrainGrid.SetTerrain(curCell, terrain.driesTo);
                            break;
                        case "WaterShallow":
                            map.terrainGrid.SetTerrain(curCell, TerrainDef.Named("Marsh"));
                            break;
                        case "Ice":
                            map.terrainGrid.SetTerrain(curCell, TerrainDef.Named("Mud"));
                            break;
                        case "Soil":
                            map.terrainGrid.SetTerrain(curCell, TerrainDef.Named("SoilRich"));
                            break;
                        case "Sand":
                        case "Gravel":
                        case "MossyTerrain":
                            map.terrainGrid.SetTerrain(curCell, TerrainDef.Named("Soil"));
                            break;
                        case "SoftSand":
                            map.terrainGrid.SetTerrain(curCell, TerrainDef.Named("Sand"));
                            break;
                        default:
                            Log.Message("unable to resolve terraindef - resetting earth sprite parameters");
                            earthSprites = IntVec3.Invalid;
                            earthSpriteMap = null;
                            earthSpriteType.Set(0);
                            earthSpritesInArea = false;
                            break;
                    }

                    terrain = curCell.GetTerrain(map);  // Get terrain after initial transformation
                    if (terrain.defName != "SoilRich") return;

                    //look for new spot to transform
                    IntVec3 oldEarthSpriteLoc = earthSprites;
                    if (earthSpritesInArea)
                    {
                        //Log.Message("moving in area");
                        List<IntVec3> spriteAreaCells = GenRadial.RadialCellsAround(oldEarthSpriteLoc, 6f, false).ToList();
                        spriteAreaCells.Shuffle();
                        for (int i = 0; i < spriteAreaCells.Count; i++)
                        {
                            IntVec3 intVec = spriteAreaCells[i];
                            terrain = intVec.GetTerrain(map);
                            if (terrain.defName is not ("MarshyTerrain" or "Mud" or "Marsh" or "WaterShallow"
                                or "Ice" or "Sand" or "Gravel" or "Soil" or "MossyTerrain" or "SoftSand")) continue;
                            if (TM_Calc.GetSpriteArea() != null && TM_Calc.GetSpriteArea().ActiveCells.Contains(intVec)) //dont transform terrain underneath buildings
                            {
                                //Log.Message("assigning");
                                earthSprites = intVec;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            IntVec3 intVec = earthSprites + GenAdj.AdjacentCells.RandomElement();
                            terrain = intVec.GetTerrain(map);
                            if (terrain.defName is not ("MarshyTerrain" or "Mud" or "Marsh" or "WaterShallow"
                                or "Ice" or "Sand" or "Gravel" or "Soil" or "MossyTerrain" or "SoftSand")) continue;
                            if (intVec.GetFirstBuilding(earthSpriteMap) != null) continue; //dont transform terrain underneath buildings

                            earthSprites = intVec;
                            i = 20;
                        }
                    }

                    if (oldEarthSpriteLoc == earthSprites)
                    {
                        earthSpriteType.Set(0);
                        earthSpriteMap = null;
                        earthSprites = IntVec3.Invalid;
                        earthSpritesInArea = false;
                        //Log.Message("ending");
                    }

                    break;
                }
            }
        }

        public void ResolveEffecter()
        {
            if (PowerModifier <= 0) return;

            powerEffecter ??= EffecterDefOf.ProgressBar.Spawn();
            powerEffecter.EffectTick(Pawn, TargetInfo.Invalid);
            MoteProgressBar mote = ((SubEffecter_ProgressBar)powerEffecter.children[0]).mote;
            if (mote == null) return;

            mote.progress = Mathf.Clamp01((float)powerModifier / maxPower);
            mote.offsetZ = +0.85f;
        }

        public void ResolveSuccubusLovin()
        {
            if (Pawn.CurrentBed() != null && Pawn.ageTracker.AgeBiologicalYears > 17 && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_VitalityBoostHD")))
            {
                Pawn pawnInMyBed = TM_Calc.FindNearbyOtherPawn(Pawn, 1);
                if (pawnInMyBed != null)
                {
                    if (pawnInMyBed.CurrentBed() != null && pawnInMyBed.RaceProps.Humanlike && pawnInMyBed.ageTracker.AgeBiologicalYears > 17)
                    {
                        Job job = new Job(JobDefOf.Lovin, pawnInMyBed, Pawn.CurrentBed());
                        Pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        HealthUtility.AdjustSeverity(pawnInMyBed, HediffDef.Named("TM_VitalityDrainHD"), 8);
                        HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_VitalityBoostHD"), 6);
                    }
                }
            }
        }

        public void ResolveWarlockEmpathy()
        {
            //strange bug observed where other pawns will get the old offset of the previous pawn's offset unless other pawn has no empathy existing
            //in other words, empathy base mood effect seems to carry over from last otherpawn instead of using current otherpawn values
            if (!Rand.Chance(Pawn.GetStatValue(StatDefOf.PsychicSensitivity, false) - 1)) return;
            Pawn otherPawn = TM_Calc.FindNearbyOtherPawn(Pawn, 5);
            if (otherPawn is not { IsColonist: true }) return;
            if (!Rand.Chance(otherPawn.GetStatValue(StatDefOf.PsychicSensitivity, false) - .3f)) return;

            ThoughtHandler pawnThoughtHandler = Pawn.needs.mood.thoughts;
            List<Thought> pawnThoughts = new List<Thought>();
            pawnThoughtHandler.GetAllMoodThoughts(pawnThoughts);
            List<Thought> otherThoughts = new List<Thought>();
            otherPawn.needs.mood.thoughts.GetAllMoodThoughts(otherThoughts);
            float oldMemoryOffset = 0;
            if (Rand.Chance(.3f)) //empathy absorbed by warlock
            {
                ThoughtDef empathyThought = ThoughtDef.Named("WarlockEmpathy");
                List<Thought_Memory> memoryThoughts = Pawn.needs.mood.thoughts.memories.Memories;
                for (int i = 0; i < memoryThoughts.Count; i++)
                {
                    if (memoryThoughts[i].def.defName == "WarlockEmpathy")
                    {
                        oldMemoryOffset = memoryThoughts[i].MoodOffset();
                        if (oldMemoryOffset > 30)
                        {
                            oldMemoryOffset = 30;
                        }
                        else if (oldMemoryOffset < -30)
                        {
                            oldMemoryOffset = -30;
                        }
                        Pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(memoryThoughts[i].def);
                    }
                }
                Thought transferThought = otherThoughts.RandomElement();
                float newOffset = Mathf.RoundToInt(transferThought.CurStage.baseMoodEffect / 2);
                empathyThought.stages.First().baseMoodEffect = newOffset + oldMemoryOffset;

                Pawn.needs.mood.thoughts.memories.TryGainMemory(empathyThought);
                Vector3 drawPosOffset = Pawn.DrawPos;
                drawPosOffset.z += .3f;
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_ArcaneCircle, drawPosOffset, Pawn.Map, newOffset / 20, .2f, .1f, .1f, Rand.Range(100, 200), 0, 0, Rand.Range(0, 360));
            }
            else //empathy bleeding to other pawn
            {
                ThoughtDef empathyThought = ThoughtDef.Named("PsychicEmpathy");
                List<Thought_Memory> memoryThoughts = otherPawn.needs.mood.thoughts.memories.Memories;
                for (int i = 0; i < memoryThoughts.Count; i++)
                {
                    if (memoryThoughts[i].def.defName == "PsychicEmpathy")
                    {
                        oldMemoryOffset = memoryThoughts[i].CurStage.baseMoodEffect;
                        if (oldMemoryOffset > 30)
                        {
                            oldMemoryOffset = 30;
                        }
                        else if (oldMemoryOffset < -30)
                        {
                            oldMemoryOffset = -30;
                        }
                        otherPawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(memoryThoughts[i].def);
                    }
                }
                Thought transferThought = pawnThoughts.RandomElement();
                float newOffset = Mathf.RoundToInt(transferThought.CurStage.baseMoodEffect / 2);
                empathyThought.stages.First().baseMoodEffect = newOffset + oldMemoryOffset;

                otherPawn.needs.mood.thoughts.memories.TryGainMemory(empathyThought);
                Vector3 drawPosOffset = otherPawn.DrawPos;
                drawPosOffset.z += .3f;
                TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_ArcaneCircle, drawPosOffset, otherPawn.Map, newOffset / 20, .2f, .1f, .1f, Rand.Range(100, 200), 0, 0, Rand.Range(0, 360));
            }
        }

        public void ResolveTechnomancerOverdrive()
        {
            if (overdriveBuilding != null)
            {
                List<Pawn> odPawns = Constants.GetOverdrivePawnList();

                if (!odPawns.Contains(Pawn))
                {
                    odPawns.Add(Pawn);
                    Constants.SetOverdrivePawnList(odPawns);
                }
                Vector3 rndPos = overdriveBuilding.DrawPos;
                rndPos.x += Rand.Range(-.4f, .4f);
                rndPos.z += Rand.Range(-.4f, .4f);
                TM_MoteMaker.ThrowGenericFleck(TorannMagicDefOf.SparkFlash, rndPos, overdriveBuilding.Map, Rand.Range(.6f, .8f), .1f, .05f, .05f, 0, 0, 0, Rand.Range(0, 360));
                FleckMaker.ThrowSmoke(rndPos, overdriveBuilding.Map, Rand.Range(.8f, 1.2f));
                rndPos = overdriveBuilding.DrawPos;
                rndPos.x += Rand.Range(-.4f, .4f);
                rndPos.z += Rand.Range(-.4f, .4f);
                TM_MoteMaker.ThrowGenericFleck(TorannMagicDefOf.ElectricalSpark, rndPos, overdriveBuilding.Map, Rand.Range(.4f, .7f), .2f, .05f, .1f, 0, 0, 0, Rand.Range(0, 360));
                SoundInfo info = SoundInfo.InMap(new TargetInfo(overdriveBuilding.Position, overdriveBuilding.Map));
                info.pitchFactor = .4f;
                info.volumeFactor = .3f;
                SoundDefOf.TurretAcquireTarget.PlayOneShot(info);
                MagicPowerSkill damageControl = MagicData.MagicPowerSkill_Overdrive.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Overdrive_ver");
                if (Rand.Chance(.6f - (.06f * damageControl.level)))
                {
                    TM_Action.DamageEntities(overdriveBuilding, null, Rand.Range(3f, (7f - (1f * damageControl.level))), DamageDefOf.Burn, overdriveBuilding);
                }
                overdriveFrequency = 100 + (10 * damageControl.level);
                if (Rand.Chance(.4f))
                {
                    overdriveFrequency /= 2;
                }
                overdriveDuration--;
                if (overdriveDuration <= 0)
                {
                    if (odPawns.Contains(Pawn))
                    {
                        Constants.ClearOverdrivePawns();
                        odPawns.Remove(Pawn);
                        Constants.SetOverdrivePawnList(odPawns);
                    }
                    overdrivePowerOutput = 0;
                    overdriveBuilding = null;
                }
            }
        }

        public void ResolveChronomancerTimeMark()
        {
            //Log.Message("pawn " + Pawn.LabelShort + " recallset: " + recallSet + " expiration: " + recallExpiration + " / " + Find.TickManager.TicksGame + " recallSpell: " + recallSpell + " position: " + recallPosition);
            if(customClass != null && MagicData.MagicPowersC.FirstOrDefault((MagicPower x ) => x.abilityDef == TorannMagicDefOf.TM_Recall).learned && !MagicData.MagicPowersStandalone.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_TimeMark).learned)
            {
                MagicData.MagicPowersStandalone.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_TimeMark).learned = true;
                RemovePawnAbility(TorannMagicDefOf.TM_TimeMark);
                AddPawnAbility(TorannMagicDefOf.TM_TimeMark);
            }
            if (recallExpiration <= Find.TickManager.TicksGame)
            {
                recallSet = false;
            }
            if (recallSet && !recallSpell)
            {
                AddPawnAbility(TorannMagicDefOf.TM_Recall);
                recallSpell = true;
            }
            if (recallSpell && (!recallSet || recallPosition == default))
            {
                recallSpell = false;
                RemovePawnAbility(TorannMagicDefOf.TM_Recall);
            }
        }

        public void ResolveSustainers()
        {
            if(BrandPawns.Count > 0)
            {
                for(int i = 0; i < BrandPawns.Count; i++)
                {
                    Pawn p = BrandPawns[i];
                    if (p != null && (p.Destroyed || p.Dead))
                    {
                        BrandPawns.Remove(BrandPawns[i]);
                        BrandDefs.Remove(BrandDefs[i]);
                        break;
                    }
                }
                if(sigilSurging && Mana.CurLevel <= .01f)
                {
                    sigilSurging = false;
                }
            }

            if (stoneskinPawns.Count > 0)
            {
                for (int i = stoneskinPawns.Count - 1; i >= 0; i--)
                {
                    if (stoneskinPawns[i].DestroyedOrNull() || stoneskinPawns[i].Dead
                        || !stoneskinPawns[i].health.hediffSet.HasHediff(HediffDef.Named("TM_StoneskinHD")))
                    {
                        stoneskinPawns.RemoveAt(i);
                    }
                }
            }

            if (soulBondPawn.DestroyedOrNull() && (spell_ShadowStep || spell_ShadowCall))
            {
                soulBondPawn = null;
                spell_ShadowCall = false;
                spell_ShadowStep = false;
                RemovePawnAbility(TorannMagicDefOf.TM_ShadowCall);
                RemovePawnAbility(TorannMagicDefOf.TM_ShadowStep);
            }
            if (soulBondPawn != null)
            {
                if (spell_ShadowStep == false)
                {
                    spell_ShadowStep = true;
                    RemovePawnAbility(TorannMagicDefOf.TM_ShadowStep);
                    AddPawnAbility(TorannMagicDefOf.TM_ShadowStep);
                }
                if (spell_ShadowCall == false)
                {
                    spell_ShadowCall = true;
                    RemovePawnAbility(TorannMagicDefOf.TM_ShadowCall);
                    AddPawnAbility(TorannMagicDefOf.TM_ShadowCall);
                }
            }

            weaponEnchants.Cleanup();

            if (mageLightActive)
            {
                if (Pawn.Map == null && mageLightSet)
                {
                    mageLightActive = false;
                    mageLightThing = null;
                    mageLightSet = false;
                }
                Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_MageLightHD);
                if (hediff == null && !mageLightSet)
                {
                    HealthUtility.AdjustSeverity(Pawn, TorannMagicDefOf.TM_MageLightHD, .5f);
                }
                if (mageLightSet && mageLightThing == null)
                {
                    mageLightActive = false;
                }
            }
            else
            {
                Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_MageLightHD);
                if (hediff != null)
                {
                    Pawn.health.RemoveHediff(hediff);
                }
                if (!mageLightThing.DestroyedOrNull())
                {
                    mageLightThing.Destroy();
                    mageLightThing = null;
                }
                mageLightSet = false;
            }            
        }

        public void ResolveMana()
        {
            if (Mana != null) return;  // Exit if there is no Mana need

            Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_MagicUserHD);
            if (hediff == null)
            {
                hediff = HediffMaker.MakeHediff(TorannMagicDefOf.TM_MagicUserHD, Pawn);
                Pawn.health.AddHediff(hediff);
            }

            hediff.Severity = 1f;
        }
        public void ResolveMagicTab()
        {
            if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless)) return;
            
            InspectTabBase inspectTabsx = Pawn.GetInspectTabs().FirstOrDefault(itb => itb.labelKey == "TM_TabMagic");
            IEnumerable<InspectTabBase> inspectTabs = Pawn.GetInspectTabs();
            if (inspectTabs == null || !inspectTabs.Any()) return;
            if (inspectTabsx != null) return;
            
            try
            {
                Pawn.def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Magic)));
            }
            catch (Exception ex)
            {
                Log.Error(string.Concat(
                    "Could not instantiate inspector tab of type ",
                    typeof(ITab_Pawn_Magic),
                    ": ",
                    ex
                ));
            }
        }

        public void ResolveClassSkills()
        {
            bool flagCM = Pawn.story.traits.HasTrait(TorannMagicDefOf.ChaosMage);
            bool isCustom = customClass != null;

            if(isCustom && customClass.classHediff != null && !Pawn.health.hediffSet.HasHediff(customClass.classHediff))
            {
                HealthUtility.AdjustSeverity(Pawn, customClass.classHediff, customClass.hediffSeverity);                
            }

            if(Pawn.story.traits.HasTrait(TorannMagicDefOf.TM_CursedTD) && !Pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_CursedHD))
            {
                HealthUtility.AdjustSeverity(Pawn, TorannMagicDefOf.TM_CursedHD, .1f);
            }

            if (Pawn.story.traits.HasTrait(TorannMagicDefOf.BloodMage) || (isCustom && (customClass.classMageAbilities.Contains(TorannMagicDefOf.TM_BloodGift) || customClass.classHediff == TorannMagicDefOf.TM_BloodHD)))
            {
                if (!Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_BloodHD")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_BloodHD"), .1f);
                    for (int i = 0; i < 4; i++)
                    {
                        TM_MoteMaker.ThrowBloodSquirt(Pawn.DrawPos, Pawn.Map, Rand.Range(.5f, .8f));
                    }
                }
            }

            if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Chronomancer) || flagCM || (isCustom && customClass.classMageAbilities.Contains(TorannMagicDefOf.TM_Prediction)))
            {
                if (predictionIncidentDef != null && (predictionTick + 30) < Find.TickManager.TicksGame)
                {
                    predictionIncidentDef = null;
                    Find.Storyteller.incidentQueue.Clear();
                    //Log.Message("prediction failed to execute, clearing prediction");
                }
            }


            if(HexedPawns is { Count: <= 0 } && previousHexedPawns > 0)
            {
                //remove abilities
                previousHexedPawns = 0;
                RemovePawnAbility(TorannMagicDefOf.TM_Hex_Pain);
                RemovePawnAbility(TorannMagicDefOf.TM_Hex_MentalAssault);
                RemovePawnAbility(TorannMagicDefOf.TM_Hex_CriticalFail);
            }

            if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Enchanter) || flagCM || isCustom)
            {
                if (MagicData.MagicPowersE.First(mp => mp.abilityDef == TorannMagicDefOf.TM_EnchantedBody).learned && (spell_EnchantedAura == false || !MagicData.MagicPowersStandalone.First(mp => mp.abilityDef == TorannMagicDefOf.TM_EnchantedAura).learned))
                {
                    spell_EnchantedAura = true;
                    MagicData.MagicPowersStandalone.First(mp => mp.abilityDef == TorannMagicDefOf.TM_EnchantedAura).learned = true;
                    InitializeSpell();
                }

                if (MagicData.MagicPowerSkill_Shapeshift.First(mp => mp.label == "TM_Shapeshift_ver").level >= 3 && (spell_ShapeshiftDW != true || !MagicData.MagicPowersStandalone.First(mp => mp.abilityDef == TorannMagicDefOf.TM_ShapeshiftDW).learned))
                {
                    spell_ShapeshiftDW = true;
                    MagicData.MagicPowersStandalone.First(mp => mp.abilityDef == TorannMagicDefOf.TM_ShapeshiftDW).learned = true;
                    InitializeSpell();
                }
            }

            if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Technomancer) || flagCM || isCustom)
            {
                if (HasTechnoBit)
                {
                    if (!Pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_TechnoBitHD))
                    {
                        HealthUtility.AdjustSeverity(Pawn, TorannMagicDefOf.TM_TechnoBitHD, .5f);
                        Vector3 bitDrawPos = Pawn.DrawPos;
                        bitDrawPos.x -= .5f;
                        bitDrawPos.z += .45f;
                        for (int i = 0; i < 4; i++)
                        {
                            FleckMaker.ThrowSmoke(bitDrawPos, Pawn.Map, Rand.Range(.6f, .8f));
                        }
                    }
                }
                if (HasTechnoWeapon && Pawn.equipment?.Primary != null)
                {
                    if (Pawn.equipment.Primary.def.defName.Contains("TM_TechnoWeapon_Base") && Pawn.equipment.Primary.def.Verbs != null && Pawn.equipment.Primary.def.Verbs.FirstOrDefault().range < 2)
                    {
                        TM_Action.DoAction_TechnoWeaponCopy(Pawn, technoWeaponThing, technoWeaponThingDef, technoWeaponQC);
                    }

                    if (!Pawn.equipment.Primary.def.defName.Contains("TM_TechnoWeapon_Base") && (technoWeaponThing != null || technoWeaponThingDef != null))
                    {
                        technoWeaponThing = null;
                        technoWeaponThingDef = null;
                    }
                }
            }

            if (MagicUserLevel >= 20 && (spell_Teach == false || !MagicData.MagicPowersStandalone.First(mp => mp.abilityDef == TorannMagicDefOf.TM_TeachMagic).learned))
            {
                AddPawnAbility(TorannMagicDefOf.TM_TeachMagic);
                MagicData.MagicPowersStandalone.First(mp => mp.abilityDef == TorannMagicDefOf.TM_TeachMagic).learned = true;
                spell_Teach = true;
            }

            if ((Pawn.story.traits.HasTrait(TorannMagicDefOf.Geomancer) || flagCM || isCustom) && earthSpriteType.Value != 0 && earthSprites.IsValid)
            {
                if (nextEarthSpriteAction < Find.TickManager.TicksGame)
                {
                    ResolveEarthSpriteAction();
                }

                if (nextEarthSpriteMote < Find.TickManager.TicksGame)
                {
                    nextEarthSpriteMote += Rand.Range(7, 12);
                    Vector3 shiftLoc = earthSprites.ToVector3Shifted();
                    shiftLoc.x += Rand.Range(-.3f, .3f);
                    shiftLoc.z += Rand.Range(-.3f, .3f);
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_Twinkle, shiftLoc, Pawn.Map, Rand.Range(.6f, 1.4f), .15f, Rand.Range(.2f, .5f), Rand.Range(.2f, .5f), Rand.Range(-100, 100), Rand.Range(0f, .3f), Rand.Range(0, 360), 0);
                    if(Rand.Chance(.3f))
                    {
                        shiftLoc = earthSprites.ToVector3Shifted();
                        shiftLoc.x += Rand.Range(-.3f, .3f);
                        shiftLoc.z += Rand.Range(-.3f, .3f);
                        TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_GreenTwinkle, shiftLoc, Pawn.Map, Rand.Range(.6f, 1f), .15f, Rand.Range(.2f, .9f), Rand.Range(.5f, .9f), Rand.Range(-200, 200), Rand.Range(0f, .3f), Rand.Range(0, 360), 0);
                    }
                }
            }

            summonedSentinels.Cleanup();
            lightningTraps.Cleanup();

            if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Lich))
            {
                if (!Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_LichHD")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_LichHD"), .5f);
                }
                if (spell_Flight != true)
                {
                    RemovePawnAbility(TorannMagicDefOf.TM_DeathBolt);
                    AddPawnAbility(TorannMagicDefOf.TM_DeathBolt);
                    spell_Flight = true;
                    InitializeSpell();
                }
            }

            if (IsMagicUser && !Pawn.Dead && !Pawn.Downed)
            {
                if (Pawn.story.traits.HasTrait(TorannMagicDefOf.TM_Bard))
                {
                    MagicPowerSkill bardtraining_pwr = Pawn.GetCompAbilityUserMagic().MagicData.MagicPowerSkill_BardTraining.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_BardTraining_pwr");

                    List<Trait> traits = Pawn.story.traits.allTraits;
                    for (int i = 0; i < traits.Count; i++)
                    {
                        if (traits[i].def.defName == "TM_Bard")
                        {
                            if (traits[i].Degree != bardtraining_pwr.level)
                            {
                                traits.Remove(traits[i]);
                                Pawn.story.traits.GainTrait(new Trait(TorannMagicDefOf.TM_Bard, bardtraining_pwr.level));
                                FleckMaker.ThrowHeatGlow(Pawn.Position, Pawn.Map, 2);
                            }
                        }
                    }
                }

                if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Succubus) || Pawn.story.traits.HasTrait(TorannMagicDefOf.Warlock))
                {
                    if (soulBondPawn != null)
                    {
                        if (!soulBondPawn.Spawned)
                        {
                            RemovePawnAbility(TorannMagicDefOf.TM_SummonDemon);
                            spell_SummonDemon = false;
                        }
                        else if (soulBondPawn.health.hediffSet.HasHediff(HediffDef.Named("TM_DemonicPriceHD")))
                        {
                            if (spell_SummonDemon)
                            {
                                RemovePawnAbility(TorannMagicDefOf.TM_SummonDemon);
                                spell_SummonDemon = false;
                            }
                        }
                        else if (soulBondPawn.health.hediffSet.HasHediff(HediffDef.Named("TM_SoulBondMentalHD")) && soulBondPawn.health.hediffSet.HasHediff(HediffDef.Named("TM_SoulBondPhysicalHD")))
                        {
                            if (spell_SummonDemon == false)
                            {
                                AddPawnAbility(TorannMagicDefOf.TM_SummonDemon);
                                spell_SummonDemon = true;
                            }
                        }
                        else
                        {
                            if (spell_SummonDemon)
                            {
                                RemovePawnAbility(TorannMagicDefOf.TM_SummonDemon);
                                spell_SummonDemon = false;
                            }
                        }
                    }
                    else if (spell_SummonDemon)
                    {
                        RemovePawnAbility(TorannMagicDefOf.TM_SummonDemon);
                        spell_SummonDemon = false;
                    }
                }
            }

            if (IsMagicUser && !Pawn.Dead & !Pawn.Downed && (Pawn.story.traits.HasTrait(TorannMagicDefOf.TM_Bard) || (isCustom && customClass.classMageAbilities.Contains(TorannMagicDefOf.TM_Inspire))))
            {
                if (!Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_InspirationalHD")) && MagicData.MagicPowersB.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Inspire).learned)
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_InspirationalHD"), 0.95f);
                }
            }
        }

        public void ResolveEnchantments()
        {
            float _maxMP = 0;
            float _maxMPUpkeep = 0;
            float _mpRegenRate = 0;
            float _mpRegenRateUpkeep = 0;
            float _coolDown = 0;
            float _xpGain = 0;
            float _mpCost = 0;
            float _arcaneRes = 0;
            float _arcaneDmg = 0;
            bool _arcaneSpectre = false;
            bool _phantomShift = false;
            float _arcalleumCooldown = 0f;

            //Determine trait adjustments
            IEnumerable<DefModExtension_TraitEnchantments> traitEnum = Pawn.story.traits.allTraits
                .Select(t => t.def.GetModExtension<DefModExtension_TraitEnchantments>());
            foreach (DefModExtension_TraitEnchantments e in traitEnum)
            {
                if (e != null)
                {
                    _maxMP += e.maxMP;
                    _mpCost += e.mpCost;
                    _mpRegenRate += e.mpRegenRate;
                    _coolDown += e.magicCooldown;
                    _xpGain += e.xpGain;
                    _arcaneRes += e.arcaneRes;
                    _arcaneDmg += e.arcaneDmg;
                }
            }

            //Determine hediff adjustments
            foreach(Hediff hd in Pawn.health.hediffSet.hediffs)
            {
                if(hd.def.GetModExtension<DefModExtension_HediffEnchantments>() != null)
                {                    
                    foreach(HediffEnchantment hdStage in hd.def.GetModExtension<DefModExtension_HediffEnchantments>().stages)
                    {
                        if(hd.Severity >= hdStage.minSeverity && hd.Severity < hdStage.maxSeverity)
                        {
                            DefModExtension_TraitEnchantments e = hdStage.enchantments;
                            if (e != null)
                            {
                                _maxMP += e.maxMP;
                                _mpCost += e.mpCost;
                                _mpRegenRate += e.mpRegenRate;
                                _coolDown += e.magicCooldown;
                                _xpGain += e.xpGain;
                                _arcaneRes += e.arcaneRes;
                                _arcaneDmg += e.arcaneDmg;
                            }
                            break;
                        }
                    }
                }
            }

            List<Apparel> apparel = Pawn.apparel.WornApparel;
            if (apparel != null)
            {
                for (int i = 0; i < Pawn.apparel.WornApparelCount; i++)
                {
                    Enchantment.CompEnchantedItem item = apparel[i].GetComp<Enchantment.CompEnchantedItem>();
                    if (item is { HasEnchantment: true })
                    {
                        float enchantmentFactor = 1f;
                        if (item.MadeFromEnchantedStuff)
                        {
                            enchantmentFactor = item.EnchantedStuff.enchantmentBonusMultiplier;

                            float arcalleumFactor = item.EnchantedStuff.arcalleumCooldownPerMass;
                            float apparelWeight = apparel[i].def.GetStatValueAbstract(StatDefOf.Mass, apparel[i].Stuff);
                            if (apparel[i].Stuff.defName == "TM_Arcalleum")
                            {
                                _arcaneRes += .05f;
                            }
                            _arcalleumCooldown += (apparelWeight * (arcalleumFactor / 100));

                        }
                        _maxMP += item.maxMP * enchantmentFactor;
                        _mpRegenRate += item.mpRegenRate * enchantmentFactor;
                        _coolDown += item.coolDown * enchantmentFactor;
                        _xpGain += item.xpGain * enchantmentFactor;
                        _mpCost += item.mpCost * enchantmentFactor;
                        _arcaneRes += item.arcaneRes * enchantmentFactor;
                        _arcaneDmg += item.arcaneDmg * enchantmentFactor;

                        if (item.arcaneSpectre)
                        {
                            _arcaneSpectre = true;
                        }
                        if (item.phantomShift)
                        {
                            _phantomShift = true;
                        }
                    }
                }
            }
            if (Pawn.equipment?.Primary != null)
            {
                Enchantment.CompEnchantedItem item = Pawn.equipment.Primary.GetComp<Enchantment.CompEnchantedItem>();
                if (item is { HasEnchantment: true })
                {
                    float enchantmentFactor = 1f;
                    if (item.MadeFromEnchantedStuff)
                    {
                        Enchantment.CompProperties_EnchantedStuff compES = Pawn.equipment.Primary.Stuff.GetCompProperties<Enchantment.CompProperties_EnchantedStuff>();

                        float arcalleumFactor = compES.arcalleumCooldownPerMass;
                        if (Pawn.equipment.Primary.Stuff.defName == "TM_Arcalleum")
                        {
                            _arcaneDmg += .1f;
                        }
                        _arcalleumCooldown += (Pawn.equipment.Primary.def.GetStatValueAbstract(StatDefOf.Mass, Pawn.equipment.Primary.Stuff) * (arcalleumFactor / 100f));
                    }
                    else
                    {
                        _maxMP += item.maxMP * enchantmentFactor;
                        _mpRegenRate += item.mpRegenRate * enchantmentFactor;
                        _coolDown += item.coolDown * enchantmentFactor;
                        _xpGain += item.xpGain * enchantmentFactor;
                        _mpCost += item.mpCost * enchantmentFactor;
                        _arcaneRes += item.arcaneRes * enchantmentFactor;
                        _arcaneDmg += item.arcaneDmg * enchantmentFactor;
                    }
                }
                if (Pawn.equipment.Primary.def.defName == "TM_DefenderStaff")
                {
                    if (item_StaffOfDefender == false)
                    {
                        AddPawnAbility(TorannMagicDefOf.TM_ArcaneBarrier);
                        item_StaffOfDefender = true;
                    }
                }
                else
                {
                    if (item_StaffOfDefender)
                    {
                        RemovePawnAbility(TorannMagicDefOf.TM_ArcaneBarrier);
                        item_StaffOfDefender = false;
                    }
                }
            }
            CleanupSummonedStructures();

            //Determine active or sustained hediffs and abilities
            if(SoL != null)
            {
                _maxMPUpkeep += (TorannMagicDefOf.TM_SpiritOfLight.upkeepEnergyCost * (1 - (TorannMagicDefOf.TM_SpiritOfLight.upkeepEfficiencyPercent * MagicData.MagicPowerSkill_SpiritOfLight.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_SpiritOfLight_eff").level)));
                _mpRegenRateUpkeep += (TorannMagicDefOf.TM_SpiritOfLight.upkeepRegenCost * (1 - (TorannMagicDefOf.TM_SpiritOfLight.upkeepEfficiencyPercent * MagicData.MagicPowerSkill_SpiritOfLight.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_SpiritOfLight_eff").level)));
            }
            if (summonedLights.Count > 0)
            {
                _maxMPUpkeep += (summonedLights.Count * TorannMagicDefOf.TM_Sunlight.upkeepEnergyCost);
                _mpRegenRateUpkeep += (summonedLights.Count * TorannMagicDefOf.TM_Sunlight.upkeepRegenCost);
            }
            if (summonedHeaters.Count > 0)
            {
                _maxMPUpkeep += (summonedHeaters.Count * TorannMagicDefOf.TM_Heater.upkeepEnergyCost);
            }
            if (summonedCoolers.Count > 0)
            {
                _maxMPUpkeep += (summonedCoolers.Count * TorannMagicDefOf.TM_Cooler.upkeepEnergyCost);
            }
            if (summonedPowerNodes.Count > 0)
            {
                _maxMPUpkeep += (summonedPowerNodes.Count * TorannMagicDefOf.TM_PowerNode.upkeepEnergyCost);
                _mpRegenRateUpkeep += (summonedPowerNodes.Count * TorannMagicDefOf.TM_PowerNode.upkeepRegenCost);
            }
            if (weaponEnchants.Count > 0)
            {
                _maxMPUpkeep += (weaponEnchants.Count * ActualManaCost(TorannMagicDefOf.TM_EnchantWeapon));
            }
            if (stoneskinPawns.Count > 0)
            {
                _maxMPUpkeep += (stoneskinPawns.Count * (TorannMagicDefOf.TM_Stoneskin.upkeepEnergyCost - (.02f * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_Stoneskin).level)));
            }
            if (summonedSentinels.Count > 0)
            {
                MagicPowerSkill heartofstone = MagicData.MagicPowerSkill_Sentinel.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Sentinel_eff");

                if (heartofstone.level == 3)
                {
                    _maxMPUpkeep += (.15f * summonedSentinels.Count);
                }
                else
                {
                    _maxMPUpkeep += ((.2f - (.02f * heartofstone.level)) * summonedSentinels.Count);
                }
            }
            if(BrandPawns.Count > 0)
            {
                float brandCost = BrandPawns.Count * (TorannMagicDefOf.TM_Branding.upkeepRegenCost * (1f - (TorannMagicDefOf.TM_Branding.upkeepEfficiencyPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_Branding).level)));
                if(sigilSurging)
                {
                    brandCost *= (5f * (1f - (.1f * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_SigilSurge).level)));
                }
                if(sigilDraining)
                {
                    brandCost *= (1.5f * (1f - (.2f * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_SigilDrain).level)));
                }
                _mpRegenRateUpkeep += brandCost; 
            }
            if(livingWall.Value is { Spawned: true })
            {
                _maxMPUpkeep += (TorannMagicDefOf.TM_LivingWall.upkeepEnergyCost * (1f - (TorannMagicDefOf.TM_LivingWall.upkeepEfficiencyPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_LivingWall).level)));
            }
            //Bonded spirit animal
            if (bondedSpirit.Value != null)
            {
                _maxMPUpkeep += (TorannMagicDefOf.TM_GuardianSpirit.upkeepEnergyCost * (1f - (TorannMagicDefOf.TM_GuardianSpirit.upkeepEfficiencyPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_GuardianSpirit).level)));
                _mpRegenRateUpkeep += (TorannMagicDefOf.TM_GuardianSpirit.upkeepRegenCost * (1f - (TorannMagicDefOf.TM_GuardianSpirit.upkeepEfficiencyPercent * MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_GuardianSpirit).level)));
                if (bondedSpirit.Value.Dead || bondedSpirit.Value.Destroyed)
                {
                    bondedSpirit.Set(null);
                }
                else if (bondedSpirit.Value.Faction != null && bondedSpirit.Value.Faction != Pawn.Faction)
                {
                    bondedSpirit.Set(null);
                }
                else if (!bondedSpirit.Value.health.hediffSet.HasHediff(TorannMagicDefOf.TM_SpiritBondHD))
                {
                    HealthUtility.AdjustSeverity(bondedSpirit.Value, TorannMagicDefOf.TM_SpiritBondHD, .5f);
                }
                if(TorannMagicDefOf.TM_SpiritCrowR == GuardianSpiritType)
                {
                    Hediff hd = Pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_CrowInsightHD);
                    if(hd != null && Math.Abs(hd.Severity - (.5f + MagicData.GetSkill_Power(TorannMagicDefOf.TM_GuardianSpirit).level)) > TOLERANCE)
                    {
                        Pawn.health.RemoveHediff(hd);
                        HealthUtility.AdjustSeverity(Pawn, TorannMagicDefOf.TM_CrowInsightHD, .5f + MagicData.GetSkill_Power(TorannMagicDefOf.TM_GuardianSpirit).level);
                    }
                    else
                    {
                        HealthUtility.AdjustSeverity(Pawn, TorannMagicDefOf.TM_CrowInsightHD, .5f + MagicData.GetSkill_Power(TorannMagicDefOf.TM_GuardianSpirit).level);
                    }
                }
            }
            if (enchanterStones is { Count: > 0 })
            {
                for (int i = 0; i < enchanterStones.Count; i++)
                {
                    if (enchanterStones[i].DestroyedOrNull())
                    {
                        enchanterStones.Remove(enchanterStones[i]);
                    }
                }
                _maxMPUpkeep += (enchanterStones.Count * (TorannMagicDefOf.TM_EnchanterStone.upkeepEnergyCost * (1f - TorannMagicDefOf.TM_EnchanterStone.upkeepEfficiencyPercent * MagicData.MagicPowerSkill_EnchanterStone.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_EnchanterStone_eff").level)));
            }
            try
            {
                if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Druid) && fertileLands.Count > 0)
                {
                    _mpRegenRateUpkeep += TorannMagicDefOf.TM_FertileLands.upkeepRegenCost;
                }
            }
            catch
            {

            }
            if (Pawn.story.traits.HasTrait(TorannMagicDefOf.Lich))
            {
                if (spell_LichForm || (customClass != null && MagicData.ReturnMatchingMagicPower(TorannMagicDefOf.TM_LichForm).learned))
                {
                    RemovePawnAbility(TorannMagicDefOf.TM_LichForm);
                    MagicData.ReturnMatchingMagicPower(TorannMagicDefOf.TM_LichForm).learned = false;
                    spell_LichForm = false;
                }
                _maxMP += .5f;
                _mpRegenRate += .5f;
            }
            if (Pawn.Inspired && Pawn.Inspiration.def == TorannMagicDefOf.ID_ManaRegen)
            {
                _mpRegenRate += 1f;
            }
            if (recallSet)
            {
                _maxMPUpkeep += TorannMagicDefOf.TM_Recall.upkeepEnergyCost * (1 - (TorannMagicDefOf.TM_Recall.upkeepEfficiencyPercent * MagicData.MagicPowerSkill_Recall.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Recall_eff").level));
                _mpRegenRateUpkeep += TorannMagicDefOf.TM_Recall.upkeepRegenCost * (1 - (TorannMagicDefOf.TM_Recall.upkeepEfficiencyPercent * MagicData.MagicPowerSkill_Recall.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Recall_eff").level));
            }
            using (IEnumerator<Hediff> enumerator = Pawn.health.hediffSet.hediffs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Hediff rec = enumerator.Current;
                    TMAbilityDef ability = MagicData.GetHediffAbility(rec);
                    if (ability != null)
                    {
                        MagicPowerSkill skill = MagicData.GetSkill_Efficiency(ability);
                        int level = 0;
                        if (skill != null)
                        {
                            level = skill.level;
                        }
                        if (ability == TorannMagicDefOf.TM_EnchantedAura || ability == TorannMagicDefOf.TM_EnchantedBody)
                        {
                            level = MagicData.GetSkill_Efficiency(TorannMagicDefOf.TM_EnchantedBody).level;
                        }

                        _maxMPUpkeep += (ability.upkeepEnergyCost * (1f - (ability.upkeepEfficiencyPercent * level)));

                        if (ability == TorannMagicDefOf.TM_EnchantedAura || ability == TorannMagicDefOf.TM_EnchantedBody)
                        {
                            level = MagicData.GetSkill_Versatility(TorannMagicDefOf.TM_EnchantedBody).level;
                        }
                        _mpRegenRateUpkeep += (ability.upkeepRegenCost * (1f - (ability.upkeepEfficiencyPercent * level)));
                    }
                }
            }

            if (Pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_SS_SerumHD))
            {
                Hediff def = Pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_SS_SerumHD);
                _mpRegenRate -= .15f * def.CurStageIndex;
                _maxMP -= .25f;
                _arcaneRes += .15f * def.CurStageIndex;
                _arcaneDmg -= .1f * def.CurStageIndex;
            }

            //class and global modifiers
            _arcaneDmg += (.01f * MagicData.MagicPowerSkill_WandererCraft.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_WandererCraft_pwr").level);
            _arcaneRes += (.02f * MagicData.MagicPowerSkill_WandererCraft.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_WandererCraft_pwr").level);
            _mpCost -= (.01f * MagicData.MagicPowerSkill_WandererCraft.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_WandererCraft_eff").level);
            _xpGain += (.02f * MagicData.MagicPowerSkill_WandererCraft.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_WandererCraft_eff").level);
            _coolDown -= (.01f * MagicData.MagicPowerSkill_WandererCraft.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_WandererCraft_ver").level);
            _mpRegenRate += (.01f * MagicData.MagicPowerSkill_WandererCraft.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_WandererCraft_ver").level);
            _maxMP += (.02f * MagicData.MagicPowerSkill_WandererCraft.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_WandererCraft_ver").level);

            _maxMP += (.04f * MagicData.MagicPowerSkill_global_spirit.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_spirit_pwr").level);
            _mpRegenRate += (.05f * MagicData.MagicPowerSkill_global_regen.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_regen_pwr").level);
            _mpCost -= (.025f * MagicData.MagicPowerSkill_global_eff.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_eff_pwr").level);
            _arcaneRes += ((1f - Pawn.GetStatValue(StatDefOf.PsychicSensitivity, false)) / 2f);
            _arcaneDmg += ((Pawn.GetStatValue(StatDefOf.PsychicSensitivity, false) - 1f) / 4f);

            arcalleumCooldown = Mathf.Clamp(0f + _arcalleumCooldown,
                0f, Pawn.story.traits.HasTrait(TorannMagicDefOf.TM_BoundlessTD) ? .1f : .5f);

            float val = (1f - (.03f * MagicData.MagicPowerSkill_Cantrips.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Cantrips_eff").level));
            _maxMPUpkeep *= val;
            _mpRegenRateUpkeep *= val;

            //resolve upkeep costs
            _maxMP -= (_maxMPUpkeep);
            _mpRegenRate -= (_mpRegenRateUpkeep);

            //finalize
            maxMP = Mathf.Clamp(1f + _maxMP, 0f, 5f);
            mpRegenRate = 1f + _mpRegenRate;
            coolDown = Mathf.Clamp(1f + _coolDown, 0.25f, 10f);
            xpGain = Mathf.Clamp(1f + _xpGain, 0.01f, 5f);
            mpCost = Mathf.Clamp(1f + _mpCost, 0.1f, 5f);
            arcaneRes = Mathf.Clamp(1 + _arcaneRes, 0.01f, 5f);
            arcaneDmg = 1 + _arcaneDmg;

            if (IsMagicUser && !TM_Calc.IsCrossClass(Pawn, true))
            {
                if (Math.Abs(maxMP - 1f) > TOLERANCE && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_maxEnergy")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_maxEnergy"), .5f);
                }
                if (Math.Abs(mpRegenRate - 1f) > TOLERANCE && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_energyRegen")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_energyRegen"), .5f);
                }
                if (Math.Abs(coolDown - 1f) > TOLERANCE && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_coolDown")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_coolDown"), .5f);
                }
                if (Math.Abs(xpGain - 1f) > TOLERANCE && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_xpGain")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_xpGain"), .5f);
                }
                if (Math.Abs(mpCost - 1f) > TOLERANCE && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_energyCost")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_energyCost"), .5f);
                }
                if (Math.Abs(arcaneRes - 1f) > TOLERANCE && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_dmgResistance")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_dmgResistance"), .5f);
                }
                if (Math.Abs(arcaneDmg - 1f) > TOLERANCE && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_dmgBonus")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_dmgBonus"), .5f);
                }
                if(_arcalleumCooldown != 0 && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_arcalleumCooldown")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_arcalleumCooldown"), .5f);
                }
                if (_arcaneSpectre && !Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_arcaneSpectre")))
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_arcaneSpectre"), .5f);
                }
                else if(_arcaneSpectre == false && Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_arcaneSpectre")))
                {
                    Pawn.health.RemoveHediff(Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("TM_HediffEnchantment_arcaneSpectre")));
                }
                if (_phantomShift)
                {
                    HealthUtility.AdjustSeverity(Pawn, HediffDef.Named("TM_HediffEnchantment_phantomShift"), .5f);
                }
                else if (_phantomShift == false && Pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_HediffEnchantment_phantomShift")))
                {
                    Pawn.health.RemoveHediff(Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("TM_HediffEnchantment_phantomShift")));
                }               
            }
        }

        private void CleanupSummonedStructures()
        {
            summonedLights.Cleanup();
            summonedHeaters.Cleanup();
            summonedCoolers.Cleanup();
            summonedPowerNodes.Cleanup();
            lightningTraps.Cleanup();
        }

        private void loadMagicPowers(List<MagicPower> magicPowers)
        {
            if (magicPowers.NullOrEmpty()) return;
            foreach (MagicPower magicPower in magicPowers)
            {
                // We add scroll abilities in InitializeSpell() rather than here
                if (magicPower.abilityDef == null || magicPower.requiresScroll || !magicPower.learned) continue;
                AddPawnAbility(magicPower.abilityDef);
            }
        }

        public override void PostExposeData()
        {
            //base.PostExposeData();            
            Scribe_Values.Look<bool>(ref magicPowersInitialized, "magicPowersInitialized");
            Scribe_Values.Look<bool>(ref magicPowersInitializedForColonist, "magicPowersInitializedForColonist", true);
            Scribe_Values.Look<bool>(ref colonistPowerCheck, "colonistPowerCheck", true);
            Scribe_Values.Look<bool>(ref spell_Rain, "spell_Rain");
            Scribe_Values.Look<bool>(ref spell_Blink, "spell_Blink");
            Scribe_Values.Look<bool>(ref spell_Teleport, "spell_Teleport");
            Scribe_Values.Look<bool>(ref spell_Heal, "spell_Heal");
            Scribe_Values.Look<bool>(ref spell_Heater, "spell_Heater");
            Scribe_Values.Look<bool>(ref spell_Cooler, "spell_Cooler");
            Scribe_Values.Look<bool>(ref spell_PowerNode, "spell_PowerNode");
            Scribe_Values.Look<bool>(ref spell_Sunlight, "spell_Sunlight");
            Scribe_Values.Look<bool>(ref spell_DryGround, "spell_DryGround");
            Scribe_Values.Look<bool>(ref spell_WetGround, "spell_WetGround");
            Scribe_Values.Look<bool>(ref spell_ChargeBattery, "spell_ChargeBattery");
            Scribe_Values.Look<bool>(ref spell_SmokeCloud, "spell_SmokeCloud");
            Scribe_Values.Look<bool>(ref spell_Extinguish, "spell_Extinguish");
            Scribe_Values.Look<bool>(ref spell_EMP, "spell_EMP");
            Scribe_Values.Look<bool>(ref spell_Blizzard, "spell_Blizzard");
            Scribe_Values.Look<bool>(ref spell_Firestorm, "spell_Firestorm");
            Scribe_Values.Look<bool>(ref spell_EyeOfTheStorm, "spell_EyeOfTheStorm");
            Scribe_Values.Look<bool>(ref spell_SummonMinion, "spell_SummonMinion");
            Scribe_Values.Look<bool>(ref spell_TransferMana, "spell_TransferMana");
            Scribe_Values.Look<bool>(ref spell_SiphonMana, "spell_SiphonMana");
            Scribe_Values.Look<bool>(ref spell_RegrowLimb, "spell_RegrowLimb");
            Scribe_Values.Look<bool>(ref spell_ManaShield, "spell_ManaShield");
            Scribe_Values.Look<bool>(ref spell_FoldReality, "spell_FoldReality");
            Scribe_Values.Look<bool>(ref spell_Resurrection, "spell_Resurrection");
            Scribe_Values.Look<bool>(ref spell_HolyWrath, "spell_HolyWrath");
            Scribe_Values.Look<bool>(ref spell_LichForm, "spell_LichForm");
            Scribe_Values.Look<bool>(ref spell_Flight, "spell_Flight");
            Scribe_Values.Look<bool>(ref spell_SummonPoppi, "spell_SummonPoppi");
            Scribe_Values.Look<bool>(ref spell_BattleHymn, "spell_BattleHymn");
            Scribe_Values.Look<bool>(ref spell_FertileLands, "spell_FertileLands");
            Scribe_Values.Look<bool>(ref spell_CauterizeWound, "spell_CauterizeWound");
            Scribe_Values.Look<bool>(ref spell_SpellMending, "spell_SpellMending");
            Scribe_Values.Look<bool>(ref spell_PsychicShock, "spell_PsychicShock");
            Scribe_Values.Look<bool>(ref spell_Scorn, "spell_Scorn");
            Scribe_Values.Look<bool>(ref spell_Meteor, "spell_Meteor");
            Scribe_Values.Look<bool>(ref spell_Teach, "spell_Teach");
            Scribe_Values.Look<bool>(ref spell_OrbitalStrike, "spell_OrbitalStrike");
            Scribe_Values.Look<bool>(ref spell_BloodMoon, "spell_BloodMoon");
            Scribe_Values.Look<bool>(ref spell_Shapeshift, "spell_Shapeshift");
            Scribe_Values.Look<bool>(ref spell_ShapeshiftDW, "spell_ShapeshiftDW");
            Scribe_Values.Look<bool>(ref spell_Blur, "spell_Blur");
            Scribe_Values.Look<bool>(ref spell_BlankMind, "spell_BlankMind");
            Scribe_Values.Look<bool>(ref spell_DirtDevil, "spell_DirtDevil");
            Scribe_Values.Look<bool>(ref spell_ArcaneBolt, "spell_ArcaneBolt");
            Scribe_Values.Look<bool>(ref spell_LightningTrap, "spell_LightningTrap");
            Scribe_Values.Look<bool>(ref spell_Invisibility, "spell_Invisibility");
            Scribe_Values.Look<bool>(ref spell_BriarPatch, "spell_BriarPatch");
            Scribe_Values.Look<bool>(ref spell_MechaniteReprogramming, "spell_MechaniteReprogramming");
            Scribe_Values.Look<bool>(ref spell_Recall, "spell_Recall");
            Scribe_Values.Look<bool>(ref spell_MageLight, "spell_MageLight");
            Scribe_Values.Look<bool>(ref spell_SnapFreeze, "spell_SnapFreeze");
            Scribe_Values.Look<bool>(ref spell_Ignite, "spell_Ignite");
            Scribe_Values.Look<bool>(ref spell_HeatShield, "spell_HeatShield");
            Scribe_Values.Look<bool>(ref useTechnoBitToggle, "useTechnoBitToggle", true);
            Scribe_Values.Look<bool>(ref useTechnoBitRepairToggle, "useTechnoBitRepairToggle", true);
            Scribe_Values.Look<bool>(ref useElementalShotToggle, "useElementalShotToggle", true);
            Scribe_Values.Look<int>(ref powerModifier, "powerModifier");
            Scribe_Values.Look<int>(ref technoWeaponDefNum, "technoWeaponDefNum");
            Scribe_Values.Look<bool>(ref doOnce, "doOnce", true);
            Scribe_Values.Look<int>(ref predictionTick, "predictionTick");
            Scribe_Values.Look<int>(ref predictionHash, "predictionHash");
            Scribe_References.Look<Thing>(ref mageLightThing, "mageLightThing");
            Scribe_Values.Look<bool>(ref mageLightActive, "mageLightActive");
            Scribe_Values.Look<bool>(ref mageLightSet, "mageLightSet");
            Scribe_Values.Look<bool>(ref deathRetaliating, "deathRetaliating");
            Scribe_Values.Look<bool>(ref canDeathRetaliate, "canDeathRetaliate");
            Scribe_Values.Look<int>(ref ticksTillRetaliation, "ticksTillRetaliation", 600);
            Scribe_Defs.Look<IncidentDef>(ref predictionIncidentDef, "predictionIncidentDef");
            Scribe_References.Look<Pawn>(ref soulBondPawn, "soulBondPawn");
            //Scribe_References.Look<Thing>(ref technoWeaponThing, "technoWeaponThing");
            Scribe_Defs.Look<ThingDef>(ref technoWeaponThingDef, "technoWeaponThingDef");
            Scribe_Values.Look<QualityCategory>(ref technoWeaponQC, "technoWeaponQC");
            Scribe_References.Look<Thing>(ref enchanterStone, "enchanterStone");
            enchanterStones.Scribe("enchanterStones");
            summonedMinions.Scribe("summonedMinions");
            supportedUndead.Scribe("supportedUndead");
            summonedLights.Scribe("summonedLights");
            summonedPowerNodes.Scribe("summonedPowerNodes");
            summonedHeaters.Scribe("summonedHeaters");
            summonedSentinels.Scribe("summonedSentinels");
            stoneskinPawns.Scribe("stoneskinPawns");
            lightningTraps.Scribe("lightningTraps");
            weaponEnchants.Scribe("weaponEnchants");
            livingWall.Scribe("livingWall");

            bondedSpirit.Scribe("bondedSpirit");
            Scribe_Defs.Look<ThingDef>(ref guardianSpiritType, "guardianSpiritType");

            earthSpriteType.Scribe("earthSpriteType");
            Scribe_Values.Look<IntVec3>(ref earthSprites, "earthSprites");
            Scribe_References.Look<Map>(ref earthSpriteMap, "earthSpriteMap");
            Scribe_Values.Look<bool>(ref earthSpritesInArea, "earthSpritesInArea");
            Scribe_Values.Look<int>(ref nextEarthSpriteAction, "nextEarthSpriteAction");

            BrandPawns.Scribe("brands");
            Scribe_Collections.Look<HediffDef>(ref BrandDefs, "brandDefs", LookMode.Def);

            Scribe_Collections.Look<Pawn>(ref hexedPawns, "hexedPawns", LookMode.Reference);
            Scribe_Collections.Look<IntVec3>(ref fertileLands, "fertileLands", LookMode.Value);
            Scribe_Values.Look<float>(ref maxMP, "maxMP", 1f);
            Scribe_Values.Look<int>(ref lastChaosTraditionTick, "lastChaosTraditionTick");
            //Scribe_Collections.Look<TM_ChaosPowers>(ref chaosPowers, "chaosPowers", LookMode.Deep, new object[0]);
            //Recall variables 
            Scribe_Values.Look<bool>(ref recallSet, "recallSet");
            Scribe_Values.Look<bool>(ref recallSpell, "recallSpell");
            Scribe_Values.Look<int>(ref recallExpiration, "recallExpiration");
            Scribe_Values.Look<IntVec3>(ref recallPosition, "recallPosition");
            Scribe_References.Look<Map>(ref recallMap, "recallMap");
            Scribe_Collections.Look<string>(ref recallNeedDefnames, "recallNeedDefnames", LookMode.Value);
            Scribe_Collections.Look<float>(ref recallNeedValues, "recallNeedValues", LookMode.Value);
            Scribe_Collections.Look<Hediff>(ref recallHediffList, "recallHediffList", LookMode.Deep);
            Scribe_Collections.Look<float>(ref recallHediffDefSeverityList, "recallHediffSeverityList", LookMode.Value);
            Scribe_Collections.Look<int>(ref recallHediffDefTicksRemainingList, "recallHediffDefTicksRemainingList", LookMode.Value);
            Scribe_Collections.Look<Hediff_Injury>(ref recallInjuriesList, "recallInjuriesList", LookMode.Deep);
            Scribe_References.Look<FlyingObject_SpiritOfLight>(ref SoL, "SoL");
            Scribe_Values.Look<bool>(ref sigilSurging, "sigilSurging");
            Scribe_Values.Look<bool>(ref sigilDraining, "sigilDraining");
            Scribe_Deep.Look(ref magicWardrobe, "magicWardrobe");
            //
            Scribe_Deep.Look<MagicData>(ref magicData, "magicData", this);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Pawn abilityUser = Pawn;
                int index = TM_ClassUtility.CustomClassIndexOfBaseMageClass(abilityUser.story.traits.allTraits);
                if (index >= 0)
                {                   
                    customClass = TM_ClassUtility.CustomClasses[index];
                    customIndex = index;
                    LoadCustomClassAbilities(customClass);                    
                }                
                else
                {
                    for (int i = 0; i < abilityUser.story.traits.allTraits.Count; i++)
                    {
                        TraitDef traitDef = abilityUser.story.traits.allTraits[i].def;
                        if (traitDef == TorannMagicDefOf.InnerFire) loadMagicPowers(MagicData.MagicPowersIF);
                        else if (traitDef == TorannMagicDefOf.HeartOfFrost)  loadMagicPowers(MagicData.MagicPowersHoF);
                        else if (traitDef == TorannMagicDefOf.StormBorn) loadMagicPowers(MagicData.MagicPowersSB);
                        else if (traitDef == TorannMagicDefOf.Arcanist) loadMagicPowers(MagicData.MagicPowersA);
                        else if (traitDef == TorannMagicDefOf.Paladin) loadMagicPowers(MagicData.MagicPowersP);
                        else if (traitDef == TorannMagicDefOf.Summoner) loadMagicPowers(MagicData.MagicPowersS);
                        else if (traitDef == TorannMagicDefOf.Druid) loadMagicPowers(MagicData.MagicPowersD);
                        else if (traitDef == TorannMagicDefOf.Necromancer || traitDef == TorannMagicDefOf.Lich) loadMagicPowers(MagicData.MagicPowersN);
                        else if (traitDef == TorannMagicDefOf.Priest) loadMagicPowers(MagicData.MagicPowersPR);
                        else if (traitDef == TorannMagicDefOf.TM_Bard) loadMagicPowers(MagicData.MagicPowersB);
                        else if (traitDef == TorannMagicDefOf.Succubus) loadMagicPowers(MagicData.MagicPowersSD);
                        else if (traitDef == TorannMagicDefOf.Warlock) loadMagicPowers(MagicData.MagicPowersWD);
                        else if (traitDef == TorannMagicDefOf.Geomancer) loadMagicPowers(MagicData.MagicPowersG);
                        else if (traitDef == TorannMagicDefOf.Technomancer) loadMagicPowers(MagicData.MagicPowersT);
                        else if (traitDef == TorannMagicDefOf.BloodMage) loadMagicPowers(MagicData.MagicPowersBM);
                        else if (traitDef == TorannMagicDefOf.Enchanter) loadMagicPowers(MagicData.MagicPowersE);
                        else if (traitDef == TorannMagicDefOf.Chronomancer) loadMagicPowers(MagicData.MagicPowersC);
                    }
                    if (abilityUser.story.traits.HasTrait(TorannMagicDefOf.ChaosMage))
                    {
                        //Log.Message("Loading Chaos Mage Abilities");
                        MagicPower mpCM = MagicData.MagicPowersCM.First(mp => mp.abilityDef == TorannMagicDefOf.TM_ChaosTradition);
                        if (mpCM.learned)
                        {
                            AddPawnAbility(TorannMagicDefOf.TM_ChaosTradition);
                            chaosPowers = new List<TM_ChaosPowers>();
                            List<MagicPower> learnedList = new List<MagicPower>();
                            for (int i = 0; i < MagicData.AllMagicPowersForChaosMage.Count; i++)
                            {
                                MagicPower mp = MagicData.AllMagicPowersForChaosMage[i];
                                if (mp.learned)
                                {
                                    learnedList.Add(mp);
                                }
                            }
                            int count = learnedList.Count;
                            for (int i = 0; i < 5; i++)
                            {
                                if (i < count)
                                {
                                    chaosPowers.Add(new TM_ChaosPowers((TMAbilityDef)learnedList[i].GetAbilityDef(0), TM_ClassUtility.GetAssociatedMagicPowerSkill(this, learnedList[i])));
                                    foreach(MagicPower mp in learnedList)
                                    {
                                        for (int j = 0; j < mp.TMabilityDefs.Count; j++)
                                        {
                                            TMAbilityDef tmad = mp.TMabilityDefs[j] as TMAbilityDef;
                                            if(tmad.shouldInitialize)
                                            {
                                                int level = mp.level;
                                                if (mp.TMabilityDefs[level] == TorannMagicDefOf.TM_LightSkip)
                                                {
                                                    if (TM_Calc.GetSkillPowerLevel(Pawn, TorannMagicDefOf.TM_LightSkip) >= 1)
                                                    {
                                                        AddPawnAbility(TorannMagicDefOf.TM_LightSkipMass);
                                                    }
                                                    if (TM_Calc.GetSkillPowerLevel(Pawn, TorannMagicDefOf.TM_LightSkip) >= 2)
                                                    {
                                                        AddPawnAbility(TorannMagicDefOf.TM_LightSkipGlobal);
                                                    }
                                                }
                                                if (tmad == TorannMagicDefOf.TM_Hex && HexedPawns.Count > 0)
                                                {
                                                    RemovePawnAbility(TorannMagicDefOf.TM_Hex_CriticalFail);
                                                    RemovePawnAbility(TorannMagicDefOf.TM_Hex_MentalAssault);
                                                    RemovePawnAbility(TorannMagicDefOf.TM_Hex_Pain);
                                                    AddPawnAbility(TorannMagicDefOf.TM_Hex_CriticalFail);
                                                    AddPawnAbility(TorannMagicDefOf.TM_Hex_MentalAssault);
                                                    AddPawnAbility(TorannMagicDefOf.TM_Hex_Pain);
                                                }
                                                
                                                AddDistinctPawnAbility(tmad);
                                            }
                                            if(tmad.childAbilities is { Count: > 0 })
                                            {
                                                foreach(TMAbilityDef ad in tmad.childAbilities)
                                                {
                                                    if(ad.shouldInitialize)
                                                    {
                                                        AddDistinctPawnAbility(ad);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    chaosPowers.Add(new TM_ChaosPowers((TMAbilityDef)TM_Calc.GetRandomMagicPower(this).abilityDef, null));
                                }
                            }
                        }
                    }
                }
                if(TM_Calc.HasAdvancedClass(Pawn))
                {
                    List<TM_CustomClass> ccList = TM_ClassUtility.GetAdvancedClassesForPawn(Pawn);
                    foreach(TM_CustomClass cc in ccList)
                    {
                        if(cc.isMage)
                        {
                            AdvancedClasses.Add(cc);
                            LoadCustomClassAbilities(cc);
                        }
                    }                    
                }
                UpdateAutocastDef();
                InitializeSpell();
                //UpdateAbilities();
            }
        }

        public void LoadCustomClassAbilities(TM_CustomClass cc, Pawn fromPawn = null)
        {
            for (int i = 0; i < cc.classMageAbilities.Count; i++)
            {
                TMAbilityDef ability = cc.classMageAbilities[i];
                MagicData fromData = null;
                if (fromPawn != null)
                {
                   fromData = fromPawn.GetCompAbilityUserMagic().MagicData;
                }
                if (fromData != null)
                {
                    foreach (MagicPower fp in fromData.AllMagicPowers)
                    {
                        if (fp.learned && cc.classMageAbilities.Contains(fp.abilityDef))
                        {
                            MagicPower mp = MagicData.AllMagicPowers.FirstOrDefault((MagicPower x) => x.abilityDef == fp.TMabilityDefs[0]);                            
                            if (mp != null)
                            {
                                mp.learned = true;
                                mp.level = fp.level;
                            }
                        }
                    }
                }

                for (int j = 0; j < MagicData.AllMagicPowers.Count; j++)
                {
                    if (MagicData.AllMagicPowers[j] == MagicData.MagicPowersWD.First(mp => mp.abilityDef == TorannMagicDefOf.TM_SoulBond) ||
                            MagicData.AllMagicPowers[j] == MagicData.MagicPowersWD.First(mp => mp.abilityDef == TorannMagicDefOf.TM_ShadowBolt) ||
                            MagicData.AllMagicPowers[j] == MagicData.MagicPowersWD.First(mp => mp.abilityDef == TorannMagicDefOf.TM_Dominate))
                    {
                        MagicData.AllMagicPowers[j].learned = false;
                    }
                    
                    if (MagicData.AllMagicPowers[j].TMabilityDefs.Contains(cc.classMageAbilities[i]) && MagicData.AllMagicPowers[j].learned)
                    {
                        if (cc.classMageAbilities[i].shouldInitialize)
                        {
                            int level = MagicData.AllMagicPowers[j].level;                                                        
                            AddPawnAbility(MagicData.AllMagicPowers[j].TMabilityDefs[level]);
                            if (magicData.AllMagicPowers[j].TMabilityDefs[level] == TorannMagicDefOf.TM_LightSkip)
                            {
                                if (TM_Calc.GetSkillPowerLevel(Pawn, TorannMagicDefOf.TM_LightSkip) >= 1)
                                {
                                    AddPawnAbility(TorannMagicDefOf.TM_LightSkipMass);
                                }
                                if (TM_Calc.GetSkillPowerLevel(Pawn, TorannMagicDefOf.TM_LightSkip) >= 2)
                                {
                                    AddPawnAbility(TorannMagicDefOf.TM_LightSkipGlobal);
                                }
                            }
                            if (cc.classMageAbilities[i] == TorannMagicDefOf.TM_Hex && HexedPawns.Count > 0)
                            {
                                RemovePawnAbility(TorannMagicDefOf.TM_Hex_CriticalFail);
                                RemovePawnAbility(TorannMagicDefOf.TM_Hex_MentalAssault);
                                RemovePawnAbility(TorannMagicDefOf.TM_Hex_Pain);
                                AddPawnAbility(TorannMagicDefOf.TM_Hex_CriticalFail);
                                AddPawnAbility(TorannMagicDefOf.TM_Hex_MentalAssault);
                                AddPawnAbility(TorannMagicDefOf.TM_Hex_Pain);
                            }
                        }
                        if (ability.childAbilities is { Count: > 0 })
                        {
                            for (int c = 0; c < ability.childAbilities.Count; c++)
                            {
                                if (ability.childAbilities[c].shouldInitialize)
                                {
                                    AddPawnAbility(ability.childAbilities[c]);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddAdvancedClass(TM_CustomClass ac, Pawn fromPawn = null)
        {
            if (ac is not { isMage: true, isAdvancedClass: true }) return;

            Trait t = Pawn.story.traits.GetTrait(TorannMagicDefOf.TM_Possessed);
            if (t != null && !Pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_SpiritPossessionHD))
            {
                Pawn.story.traits.RemoveTrait(t);
                return;
            }
            if (!AdvancedClasses.Contains(ac))
            {
                AdvancedClasses.Add(ac);
            }
            else // clear all abilities and re-add
            {
                foreach (TMAbilityDef ability in ac.classMageAbilities)
                {
                    RemovePawnAbility(ability);
                    if (ability.childAbilities is { Count: > 0 })
                    {
                        foreach (TMAbilityDef cab in ability.childAbilities)
                        {
                            RemovePawnAbility(cab);
                        }
                    }
                }
            }

            MagicData fromData = fromPawn?.GetCompAbilityUserMagic().MagicData;
            if(fromData != null)
            {
                foreach(TMAbilityDef ability in ac.classMageAbilities)
                {
                    MagicPowerSkill mps_e = MagicData.GetSkill_Efficiency(ability);
                    MagicPowerSkill fps_e = fromData.GetSkill_Efficiency(ability);
                    if (mps_e != null && fps_e != null)
                    {
                        mps_e.level = fps_e.level;
                    }
                    MagicPowerSkill mps_p = MagicData.GetSkill_Power(ability);
                    MagicPowerSkill fps_p = fromData.GetSkill_Power(ability);
                    if (mps_p != null && fps_p != null)
                    {
                        mps_p.level = fps_p.level;
                    }
                    MagicPowerSkill mps_v = MagicData.GetSkill_Versatility(ability);
                    MagicPowerSkill fps_v = fromData.GetSkill_Versatility(ability);
                    if (mps_v != null && fps_v != null)
                    {
                        mps_v.level = fps_v.level;
                    }
                }
            }
            LoadCustomClassAbilities(ac, fromPawn);
        }

        public void RemoveAdvancedClass(TM_CustomClass ac)
        {
            for (int i = 0; i < ac.classMageAbilities.Count; i++)
            {
                TMAbilityDef ability = ac.classMageAbilities[i];

                for (int j = 0; j < MagicData.AllMagicPowers.Count; j++)
                {
                    MagicPower power = MagicData.AllMagicPowers[j];
                    if (power.abilityDef == ability)
                    {
                        if (magicData.AllMagicPowers[j].TMabilityDefs[power.level] == TorannMagicDefOf.TM_LightSkip)
                        {
                            if (TM_Calc.GetSkillPowerLevel(Pawn, TorannMagicDefOf.TM_LightSkip) >= 1)
                            {
                                RemovePawnAbility(TorannMagicDefOf.TM_LightSkipMass);
                            }
                            if (TM_Calc.GetSkillPowerLevel(Pawn, TorannMagicDefOf.TM_LightSkip) >= 2)
                            {
                                RemovePawnAbility(TorannMagicDefOf.TM_LightSkipGlobal);
                            }
                        }
                        if (ac.classMageAbilities[i] == TorannMagicDefOf.TM_Hex && HexedPawns.Count > 0)
                        {
                            RemovePawnAbility(TorannMagicDefOf.TM_Hex_CriticalFail);
                            RemovePawnAbility(TorannMagicDefOf.TM_Hex_MentalAssault);
                            RemovePawnAbility(TorannMagicDefOf.TM_Hex_Pain);
                        }                        
                        power.autocast = false;
                        power.learned = false;
                        power.level = 0;

                        if (ability.childAbilities is { Count: > 0 })
                        {
                            for (int c = 0; c < ability.childAbilities.Count; c++)
                            {
                                RemovePawnAbility(ability.childAbilities[c]);
                            }
                        }
                    }
                    RemovePawnAbility(ability);
                }
            }
            if (ac.isMage && ac.isAdvancedClass)
            {
                foreach (TMAbilityDef ability in ac.classMageAbilities)
                {
                    MagicPowerSkill mps_e = MagicData.GetSkill_Efficiency(ability);
                    if (mps_e != null)
                    {
                        mps_e.level = 0;
                    }
                    MagicPowerSkill mps_p = MagicData.GetSkill_Power(ability);
                    if (mps_p != null)
                    {
                        mps_p.level = 0;
                    }
                    MagicPowerSkill mps_v = MagicData.GetSkill_Versatility(ability);
                    if (mps_v != null)
                    {
                        mps_v.level = 0;
                    }
                }
            }
            if(AdvancedClasses.Contains(ac))
            {
                AdvancedClasses.Remove(ac);
            }
        }

        public void UpdateAutocastDef()
        {
            if (!IsMagicUser || MagicData?.MagicPowersCustom == null) return;

            foreach (MagicPower mp in MagicData.MagicPowersCustom)
            {
                foreach (TM_CustomPowerDef mpDef in TM_Data.CustomMagePowerDefs())
                {
                    if (mpDef.customPower.abilityDefs[0].ToString() != mp.GetAbilityDef(0).ToString()) continue;

                    if (mpDef.customPower.autocasting != null)
                    {
                        mp.autocasting = mpDef.customPower.autocasting;
                    }
                }
            }
        }

        private Dictionary<string, Command> gizmoCommands = new();
        public Command GetGizmoCommands(string key)
        {
            Command command = gizmoCommands.TryGetValue(key);
            if (command != null) return command;

            switch (key)
            {
                case "symbiosis":
                {
                    Command_Action itemSymbiosis = new Command_Action
                    {
                        action = delegate
                        {
                            TM_Action.RemoveSymbiosisCommand(Pawn);
                        },
                        Order = 61,
                        defaultLabel = TM_TextPool.TM_RemoveSymbiosis,
                        defaultDesc = TM_TextPool.TM_RemoveSymbiosisDesc,
                        icon = ContentFinder<Texture2D>.Get("UI/end_symbiosis"),
                    };
                    gizmoCommands.Add(key, itemSymbiosis);
                    return itemSymbiosis;
                }
                case "wanderer":
                {
                    Command_Action itemWanderer = new Command_Action
                    {
                        action = delegate
                        {
                            TM_Action.PromoteWanderer(Pawn);
                        },
                        Order = 51,
                        defaultLabel = TM_TextPool.TM_PromoteWanderer,
                        defaultDesc = TM_TextPool.TM_PromoteWandererDesc,
                        icon = ContentFinder<Texture2D>.Get("UI/wanderer"),
                    };
                    gizmoCommands.Add(key, itemWanderer);
                    return itemWanderer;
                }
                case "technoBit":
                {
                    string toggle = "bit_c";
                    string label = "TM_TechnoBitEnabled".Translate();
                    if (!useTechnoBitToggle)
                    {
                        toggle = "bit_off";
                        label = "TM_TechnoBitDisabled".Translate();
                    }
                    var item = new Command_Toggle
                    {
                        isActive = () => useTechnoBitToggle,
                        toggleAction = () =>
                        {
                            useTechnoBitToggle = !useTechnoBitToggle;
                        },
                        defaultLabel = label,
                        defaultDesc = "TM_TechnoBitToggleDesc".Translate(),
                        Order = -89,
                        icon = ContentFinder<Texture2D>.Get("UI/" + toggle)
                    };
                    gizmoCommands.Add(key, item);
                    return item;
                }
                case "technoRepair":
                {
                    string toggle_repair = "bit_repairon";
                    if (!useTechnoBitRepairToggle)
                    {
                        toggle_repair = "bit_repairoff";
                    }
                    var item_repair = new Command_Toggle
                    {
                        isActive = () => useTechnoBitRepairToggle,
                        toggleAction = () =>
                        {
                            useTechnoBitRepairToggle = !useTechnoBitRepairToggle;
                        },
                        defaultLabel = "TM_TechnoBitRepair".Translate(),
                        defaultDesc = "TM_TechnoBitRepairDesc".Translate(),
                        Order = -88,
                        icon = ContentFinder<Texture2D>.Get("UI/" + toggle_repair)
                    };
                    gizmoCommands.Add(key, item_repair);
                    return item_repair;
                }
                case "elementalShot":
                {
                    string toggle = "elementalshot";
                    if (!useElementalShotToggle)
                    {
                        toggle = "elementalshot_off";
                    }
                    var item = new Command_Toggle
                    {
                        isActive = () => useElementalShotToggle,
                        toggleAction = () =>
                        {
                            useElementalShotToggle = !useElementalShotToggle;
                        },
                        defaultLabel = "TM_TechnoWeapon_ver".Translate(),
                        defaultDesc = "TM_ElementalShotToggleDesc".Translate(),
                        Order = -88,
                        icon = ContentFinder<Texture2D>.Get("UI/" + toggle)
                    };
                    gizmoCommands.Add(key, item);
                    return item;
                }
            }
            return null;
        }
    }
}
