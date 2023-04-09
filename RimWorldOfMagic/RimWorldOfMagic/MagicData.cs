
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using TorannMagic.TMDefs;

namespace TorannMagic
{
    public class MagicData : IExposable
    {
        private Pawn magicPawn;
        private int magicUserLevel;
        private int magicAbilityPoints;
        private int magicUserXP = 1;
        private int ticksToLearnMagicXP = -1;
        public bool initialized;
        private int ticksAffiliation;
        private bool isNecromancer;
        private int dominationCount;

        public List<MagicPower> magicPowerCustom;
        public List<MagicPower> MagicPowersCustom  //supports customs abilities
        {
            get
            {
                if (magicPowerCustom != null) return magicPowerCustom;

                TM_CustomPowerDef[] customPowerDefs = TM_Data.CustomMagePowerDefs();
                magicPowerCustom = new List<MagicPower>();
                foreach (TM_CustomPowerDef powerDef in customPowerDefs)
                {
                    bool newPower = false;
                    List<VFECore.Abilities.AbilityDef> abilityList = powerDef.customPower.abilityDefs;
                    bool requiresScroll = powerDef.customPower.requiresScroll;
                    MagicPower mp = new MagicPower(abilityList, requiresScroll)
                    {
                        learnCost = powerDef.customPower.learnCost,
                        costToLevel = powerDef.customPower.costToLevel,
                        autocasting = powerDef.customPower.autocasting
                    };
                    if (!magicPowerCustom.Any(a => a.GetAbilityDef(0) == mp.GetAbilityDef(0)))
                    {
                        newPower = true;
                    }
                    bool hasSkills = false;
                    if (powerDef.customPower.skills != null)
                    {
                        foreach (TM_CustomSkill skill in powerDef.customPower.skills)
                        {
                            if (skill == null) continue;

                            MagicPowerSkill mps = new MagicPowerSkill(skill.label, skill.description)
                            {
                                levelMax = skill.levelMax,
                                costToLevel = skill.costToLevel
                            };
                            if (!MagicPowerSkill_Custom.Any(b => b.label == mps.label) && !AllMagicPowerSkills.Any(b => b.label == mps.label))
                            {
                                MagicPowerSkill_Custom.Add(mps);
                            }
                            hasSkills = true;
                        }
                    }
                    if (newPower)
                    {
                        if (hasSkills)
                        {
                            magicPowerCustom.Add(mp);
                        }
                        else if (!MagicPowersCustomStandalone.Any((a => a.GetAbilityDef(0) == mp.GetAbilityDef(0))))
                        {
                            MagicPowersCustomStandalone.Add(mp);
                        }
                    }
                    if (hasSkills && powerDef.customPower.chaosMageUseable && !AllMagicPowersForChaosMage.Contains(mp))
                    {
                        AllMagicPowersForChaosMage.Add(mp);
                    }
                }
                allMagicPowerSkillsList = null; //force rediscovery and caching to include custom defs
                return magicPowerCustom;
            }
        }

        public List<MagicPower> magicPowerCustomStandalone;
        public List<MagicPower> MagicPowersCustomStandalone => magicPowerCustomStandalone ??= new List<MagicPower>(); //supports customs abilities

        private List<MagicPower> magicPowerCustomAll;
        public List<MagicPower> MagicPowersCustomAll
        {
            get
            {
                if (magicPowerCustomAll != null) return magicPowerCustomAll;

                magicPowerCustomAll = new List<MagicPower>();
                magicPowerCustomAll.AddRange(MagicPowersCustom);
                magicPowerCustomAll.AddRange(MagicPowersCustomStandalone);
                return magicPowerCustomAll;
            }
        }

        public List<MagicPowerSkill> magicPowerSkill_Custom;
        public List<MagicPowerSkill> MagicPowerSkill_Custom => magicPowerSkill_Custom ??= new List<MagicPowerSkill>();

        public List<MagicPower> magicPowerIF;
        public List<MagicPower> magicPowerHoF;
        public List<MagicPower> magicPowerSB;
        public List<MagicPower> magicPowerA;
        public List<MagicPower> magicPowerP;
        public List<MagicPower> magicPowerS;
        public List<MagicPower> magicPowerD;
        public List<MagicPower> magicPowerN;
        public List<MagicPower> magicPowerPR;
        public List<MagicPower> magicPowerB;
        public List<MagicPower> magicPowerWD;
        public List<MagicPower> magicPowerSD;
        public List<MagicPower> magicPowerG;
        public List<MagicPower> magicPowerT;
        public List<MagicPower> magicPowerBM;
        public List<MagicPower> magicPowerE;
        public List<MagicPower> magicPowerC;
        public List<MagicPower> magicPowerW;
        public List<MagicPower> magicPowerCM;
        public List<MagicPower> magicPowerShadow;
        public List<MagicPower> magicPowerBrightmage;
        public List<MagicPower> magicPowerShaman;
        public List<MagicPower> magicPowerGolemancer;

        public List<MagicPower> magicPowerStandalone;

        public List<MagicPowerSkill> magicPowerSkill_global_regen;
        public List<MagicPowerSkill> magicPowerSkill_global_eff;
        public List<MagicPowerSkill> magicPowerSkill_global_spirit;

        public List<MagicPowerSkill> magicPowerSkill_WandererCraft;
        public List<MagicPowerSkill> magicPowerSkill_Cantrips;

        public List<MagicPowerSkill> magicPowerSkill_RayofHope;
        public List<MagicPowerSkill> magicPowerSkill_Firebolt;
        public List<MagicPowerSkill> magicPowerSkill_Fireball;
        public List<MagicPowerSkill> magicPowerSkill_Fireclaw;
        public List<MagicPowerSkill> magicPowerSkill_Firestorm;

        public List<MagicPowerSkill> magicPowerSkill_Soothe;
        public List<MagicPowerSkill> magicPowerSkill_Rainmaker;
        public List<MagicPowerSkill> magicPowerSkill_Icebolt;
        public List<MagicPowerSkill> magicPowerSkill_FrostRay;
        public List<MagicPowerSkill> magicPowerSkill_Snowball;
        public List<MagicPowerSkill> magicPowerSkill_Blizzard;

        public List<MagicPowerSkill> magicPowerSkill_AMP;
        public List<MagicPowerSkill> magicPowerSkill_LightningBolt;
        public List<MagicPowerSkill> magicPowerSkill_LightningCloud;
        public List<MagicPowerSkill> magicPowerSkill_LightningStorm;
        public List<MagicPowerSkill> magicPowerSkill_EyeOfTheStorm;

        public List<MagicPowerSkill> magicPowerSkill_Shadow;
        public List<MagicPowerSkill> magicPowerSkill_Blink;
        public List<MagicPowerSkill> magicPowerSkill_Summon;
        public List<MagicPowerSkill> magicPowerSkill_MagicMissile;
        public List<MagicPowerSkill> magicPowerSkill_Teleport;
        public List<MagicPowerSkill> magicPowerSkill_FoldReality;

        public List<MagicPowerSkill> magicPowerSkill_P_RayofHope;
        public List<MagicPowerSkill> magicPowerSkill_Heal;
        public List<MagicPowerSkill> magicPowerSkill_Shield;
        public List<MagicPowerSkill> magicPowerSkill_ValiantCharge;
        public List<MagicPowerSkill> magicPowerSkill_Overwhelm;
        public List<MagicPowerSkill> magicPowerSkill_HolyWrath;

        public List<MagicPowerSkill> magicPowerSkill_SummonMinion;
        public List<MagicPowerSkill> magicPowerSkill_SummonPylon;
        public List<MagicPowerSkill> magicPowerSkill_SummonExplosive;
        public List<MagicPowerSkill> magicPowerSkill_SummonElemental;
        public List<MagicPowerSkill> magicPowerSkill_SummonPoppi;

        public List<MagicPowerSkill> magicPowerSkill_Poison;
        public List<MagicPowerSkill> magicPowerSkill_SootheAnimal;
        public List<MagicPowerSkill> magicPowerSkill_Regenerate;
        public List<MagicPowerSkill> magicPowerSkill_CureDisease;
        public List<MagicPowerSkill> magicPowerSkill_RegrowLimb;

        public List<MagicPowerSkill> magicPowerSkill_RaiseUndead;
        public List<MagicPowerSkill> magicPowerSkill_DeathMark;
        public List<MagicPowerSkill> magicPowerSkill_FogOfTorment;
        public List<MagicPowerSkill> magicPowerSkill_ConsumeCorpse;
        public List<MagicPowerSkill> magicPowerSkill_CorpseExplosion;
        public List<MagicPowerSkill> magicPowerSkill_LichForm;
        public List<MagicPowerSkill> magicPowerSkill_DeathBolt;

        public List<MagicPowerSkill> magicPowerSkill_AdvancedHeal;
        public List<MagicPowerSkill> magicPowerSkill_Purify;
        public List<MagicPowerSkill> magicPowerSkill_HealingCircle;
        public List<MagicPowerSkill> magicPowerSkill_BestowMight;
        public List<MagicPowerSkill> magicPowerSkill_Resurrection;

        public List<MagicPowerSkill> magicPowerSkill_BardTraining;
        public List<MagicPowerSkill> magicPowerSkill_Entertain;
        public List<MagicPowerSkill> magicPowerSkill_Inspire;
        public List<MagicPowerSkill> magicPowerSkill_Lullaby;
        public List<MagicPowerSkill> magicPowerSkill_BattleHymn;

        public List<MagicPowerSkill> magicPowerSkill_SoulBond;
        public List<MagicPowerSkill> magicPowerSkill_ShadowBolt;
        public List<MagicPowerSkill> magicPowerSkill_Dominate;
        public List<MagicPowerSkill> magicPowerSkill_Repulsion;
        public List<MagicPowerSkill> magicPowerSkill_Attraction;
        public List<MagicPowerSkill> magicPowerSkill_Scorn;
        public List<MagicPowerSkill> magicPowerSkill_PsychicShock;
        //public List<MagicPowerSkill> magicPowerSkill_SummonDemon;

        public List<MagicPowerSkill> magicPowerSkill_Stoneskin;
        public List<MagicPowerSkill> magicPowerSkill_Encase;
        public List<MagicPowerSkill> magicPowerSkill_EarthSprites;
        public List<MagicPowerSkill> magicPowerSkill_EarthernHammer;
        public List<MagicPowerSkill> magicPowerSkill_Sentinel;
        public List<MagicPowerSkill> magicPowerSkill_Meteor;

        public List<MagicPowerSkill> magicPowerSkill_TechnoBit;
        public List<MagicPowerSkill> magicPowerSkill_TechnoTurret;
        public List<MagicPowerSkill> magicPowerSkill_TechnoWeapon;
        public List<MagicPowerSkill> magicPowerSkill_TechnoShield;
        public List<MagicPowerSkill> magicPowerSkill_Sabotage;
        public List<MagicPowerSkill> magicPowerSkill_Overdrive;
        public List<MagicPowerSkill> magicPowerSkill_OrbitalStrike;

        public List<MagicPowerSkill> magicPowerSkill_BloodGift;         //BloodGift & BloodSacrifice
        public List<MagicPowerSkill> magicPowerSkill_IgniteBlood;
        public List<MagicPowerSkill> magicPowerSkill_BloodForBlood;
        public List<MagicPowerSkill> magicPowerSkill_BloodShield;
        public List<MagicPowerSkill> magicPowerSkill_Rend;
        public List<MagicPowerSkill> magicPowerSkill_BloodMoon;

        public List<MagicPowerSkill> magicPowerSkill_EnchantedBody;  //EnchantedBody & EnchantedAura
        public List<MagicPowerSkill> magicPowerSkill_Transmutate;
        public List<MagicPowerSkill> magicPowerSkill_EnchanterStone;
        public List<MagicPowerSkill> magicPowerSkill_EnchantWeapon;
        public List<MagicPowerSkill> magicPowerSkill_Polymorph;
        public List<MagicPowerSkill> magicPowerSkill_Shapeshift;

        public List<MagicPowerSkill> magicPowerSkill_Prediction;
        public List<MagicPowerSkill> magicPowerSkill_AlterFate;
        public List<MagicPowerSkill> magicPowerSkill_AccelerateTime;
        public List<MagicPowerSkill> magicPowerSkill_ReverseTime;
        public List<MagicPowerSkill> magicPowerSkill_ChronostaticField;
        public List<MagicPowerSkill> magicPowerSkill_Recall;

        public List<MagicPowerSkill> magicPowerSkill_ChaosTradition;

        public List<MagicPowerSkill> magicPowerSkill_ShadowWalk;

        public List<MagicPowerSkill> magicPowerSkill_LightLance;
        public List<MagicPowerSkill> magicPowerSkill_Sunfire;
        public List<MagicPowerSkill> magicPowerSkill_LightBurst;
        public List<MagicPowerSkill> magicPowerSkill_LightSkip;
        public List<MagicPowerSkill> magicPowerSkill_Refraction;
        public List<MagicPowerSkill> magicPowerSkill_SpiritOfLight;

        public List<MagicPowerSkill> magicPowerSkill_Totems;            //healing totem, hex/buff totem, elemental totem
        public List<MagicPowerSkill> magicPowerSkill_ChainLightning;
        public List<MagicPowerSkill> magicPowerSkill_Enrage;
        public List<MagicPowerSkill> magicPowerSkill_Hex;
        public List<MagicPowerSkill> magicPowerSkill_SpiritWolves;
        public List<MagicPowerSkill> magicPowerSkill_GuardianSpirit;

        public List<MagicPowerSkill> magicPowerSkill_Golemancy;
        public List<MagicPowerSkill> magicPowerSkill_RuneCarving;
        public List<MagicPowerSkill> magicPowerSkill_Branding;
        public List<MagicPowerSkill> magicPowerSkill_SigilSurge;
        public List<MagicPowerSkill> magicPowerSkill_SigilDrain;
        public List<MagicPowerSkill> magicPowerSkill_LivingWall;

        public List<MagicPowerSkill> MagicPowerSkill_global_regen =>
            magicPowerSkill_global_regen ??= new List<MagicPowerSkill>
            {
                new("TM_global_regen_pwr", "TM_global_regen_pwr_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_global_eff =>
            magicPowerSkill_global_eff ??= new List<MagicPowerSkill>
            {
                new("TM_global_eff_pwr", "TM_global_eff_pwr_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_global_spirit =>
            magicPowerSkill_global_spirit ??= new List<MagicPowerSkill>
            {
                new("TM_global_spirit_pwr", "TM_global_spirit_pwr_desc")
            };

        public List<MagicPower> MagicPowersStandalone =>
            magicPowerStandalone ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_TransferMana),
                new(TorannMagicDefOf.TM_SiphonMana),
                new(TorannMagicDefOf.TM_SpellMending),
                new(TorannMagicDefOf.TM_CauterizeWound),
                new(TorannMagicDefOf.TM_TeachMagic),
                new(TorannMagicDefOf.TM_EnchantedAura),
                new(TorannMagicDefOf.TM_MechaniteReprogramming),
                new(TorannMagicDefOf.TM_DirtDevil),
                new(TorannMagicDefOf.TM_Heater),
                new(TorannMagicDefOf.TM_Cooler),
                new(TorannMagicDefOf.TM_PowerNode),
                new(TorannMagicDefOf.TM_Sunlight),
                new(TorannMagicDefOf.TM_DryGround),
                new(TorannMagicDefOf.TM_WetGround),
                new(TorannMagicDefOf.TM_ChargeBattery),
                new(TorannMagicDefOf.TM_SmokeCloud),
                new(TorannMagicDefOf.TM_Extinguish),
                new(TorannMagicDefOf.TM_EMP),
                new(TorannMagicDefOf.TM_ManaShield),
                new(TorannMagicDefOf.TM_ArcaneBarrier),
                new(TorannMagicDefOf.TM_Flight),
                new(TorannMagicDefOf.TM_FertileLands),
                new(TorannMagicDefOf.TM_Blur),
                new(TorannMagicDefOf.TM_BlankMind),
                new(TorannMagicDefOf.TM_ArcaneBolt),
                new(TorannMagicDefOf.TM_LightningTrap),
                new(TorannMagicDefOf.TM_Invisibility),
                new(TorannMagicDefOf.TM_BriarPatch),
                new(TorannMagicDefOf.TM_TimeMark),
                new(TorannMagicDefOf.TM_MageLight),
                new(TorannMagicDefOf.TM_NanoStimulant),
                new(TorannMagicDefOf.TM_Ignite),
                new(TorannMagicDefOf.TM_SnapFreeze),
                new(TorannMagicDefOf.TM_ShapeshiftDW),
                new(TorannMagicDefOf.TM_LightSkipMass),
                new(TorannMagicDefOf.TM_LightSkipGlobal),
                new(TorannMagicDefOf.TM_HeatShield),
            }; //spells needed for magicpower reference during autocast

        public List<MagicPower> MagicPowersW =>
            magicPowerW ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_WandererCraft),
                new(TorannMagicDefOf.TM_Cantrips),
            };

        public List<MagicPowerSkill> MagicPowerSkill_WandererCraft =>
            magicPowerSkill_WandererCraft ??= new List<MagicPowerSkill>
            {
                new("TM_WandererCraft_pwr", "TM_WandererCraft_pwr_desc"),
                new("TM_WandererCraft_eff", "TM_WandererCraft_eff_desc"),
                new("TM_WandererCraft_ver", "TM_WandererCraft_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Cantrips =>
            magicPowerSkill_Cantrips ??= new List<MagicPowerSkill>
            {
                new("TM_Cantrips_pwr", "TM_Cantrips_pwr_desc"),
                new("TM_Cantrips_eff", "TM_Cantrips_eff_desc"),
                new("TM_Cantrips_ver", "TM_Cantrips_ver_desc")
            };

        public List<MagicPower> MagicPowersIF =>
            magicPowerIF ??= new List<MagicPower>
            {
                new(
                    TorannMagicDefOf.TM_RayofHope,
                    TorannMagicDefOf.TM_RayofHope_I,
                    TorannMagicDefOf.TM_RayofHope_II,
                    TorannMagicDefOf.TM_RayofHope_III
                ),
                new(TorannMagicDefOf.TM_Firebolt),
                new(TorannMagicDefOf.TM_Fireclaw),
                new(TorannMagicDefOf.TM_Fireball),
                new(true, TorannMagicDefOf.TM_Firestorm),
            };

        public List<MagicPowerSkill> MagicPowerSkill_RayofHope =>
            magicPowerSkill_RayofHope ??= new List<MagicPowerSkill>
            {
                new("TM_RayofHope_eff", "TM_RayofHope_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Firebolt =>
            magicPowerSkill_Firebolt ??= new List<MagicPowerSkill>
            {
                new("TM_Firebolt_pwr", "TM_Firebolt_pwr_desc"),
                new("TM_Firebolt_eff", "TM_Firebolt_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Fireball =>
            magicPowerSkill_Fireball ??= new List<MagicPowerSkill>
            {
                new("TM_Fireball_pwr", "TM_Fireball_pwr_desc"),
                new("TM_Fireball_eff", "TM_Fireball_eff_desc"),
                new("TM_Fireball_ver", "TM_Fireball_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Fireclaw =>
            magicPowerSkill_Fireclaw ??= new List<MagicPowerSkill>
            {
                new("TM_Fireclaw_pwr", "TM_Fireclaw_pwr_desc"),
                new("TM_Fireclaw_eff", "TM_Fireclaw_eff_desc"),
                new("TM_Fireclaw_ver", "TM_Fireclaw_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Firestorm =>
            magicPowerSkill_Firestorm ??= new List<MagicPowerSkill>
            {
                new("TM_Firestorm_pwr", "TM_Firestorm_pwr_desc"),
                new("TM_Firestorm_eff", "TM_Firestorm_eff_desc"),
                new("TM_Firestorm_ver", "TM_Firestorm_ver_desc")
            };

        public List<MagicPower> MagicPowersHoF =>
            magicPowerHoF ??= new List<MagicPower>
            {
                new(
                    TorannMagicDefOf.TM_Soothe,
                    TorannMagicDefOf.TM_Soothe_I,
                    TorannMagicDefOf.TM_Soothe_II,
                    TorannMagicDefOf.TM_Soothe_III
                ),
                new(TorannMagicDefOf.TM_Rainmaker),
                new(TorannMagicDefOf.TM_Icebolt),
                new(
                    TorannMagicDefOf.TM_FrostRay,
                    TorannMagicDefOf.TM_FrostRay_I,
                    TorannMagicDefOf.TM_FrostRay_II,
                    TorannMagicDefOf.TM_FrostRay_III
                ),
                new(TorannMagicDefOf.TM_Snowball),
                new(true, TorannMagicDefOf.TM_Blizzard),
            };

        public List<MagicPowerSkill> MagicPowerSkill_Soothe =>
            magicPowerSkill_Soothe ??= new List<MagicPowerSkill>
            {
                new("TM_Soothe_eff", "TM_Soothe_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Icebolt =>
            magicPowerSkill_Icebolt ??= new List<MagicPowerSkill>
            {
                new("TM_Icebolt_pwr", "TM_Icebolt_pwr_desc"),
                new("TM_Icebolt_eff", "TM_Icebolt_eff_desc"),
                new("TM_Icebolt_ver", "TM_Icebolt_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_FrostRay =>
            magicPowerSkill_FrostRay ??= new List<MagicPowerSkill>
            {
                new("TM_FrostRay_eff", "TM_FrostRay_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Snowball =>
            magicPowerSkill_Snowball ??= new List<MagicPowerSkill>
            {
                new("TM_Snowball_pwr", "TM_Snowball_pwr_desc"),
                new("TM_Snowball_eff", "TM_Snowball_eff_desc"),
                new("TM_Snowball_ver", "TM_Snowball_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Rainmaker =>
            magicPowerSkill_Rainmaker ??= new List<MagicPowerSkill>
            {
                new("TM_Rainmaker_eff", "TM_Rainmaker_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Blizzard =>
            magicPowerSkill_Blizzard ??= new List<MagicPowerSkill>
            {
                new("TM_Blizzard_pwr", "TM_Blizzard_pwr_desc"),
                new("TM_Blizzard_eff", "TM_Blizzard_eff_desc"),
                new("TM_Blizzard_ver", "TM_Blizzard_ver_desc")
            };

        public List<MagicPower> MagicPowersSB =>
            magicPowerSB ??= new List<MagicPower>
            {
                new(
                    TorannMagicDefOf.TM_AMP,
                    TorannMagicDefOf.TM_AMP_I,
                    TorannMagicDefOf.TM_AMP_II,
                    TorannMagicDefOf.TM_AMP_III
                ),
                new(TorannMagicDefOf.TM_LightningBolt),
                new(TorannMagicDefOf.TM_LightningCloud),
                new(TorannMagicDefOf.TM_LightningStorm),
                new(true, TorannMagicDefOf.TM_EyeOfTheStorm),
            };

        public List<MagicPowerSkill> MagicPowerSkill_AMP =>
            magicPowerSkill_AMP ??= new List<MagicPowerSkill>
            {
                new("TM_AMP_eff", "TM_AMP_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_LightningBolt =>
            magicPowerSkill_LightningBolt ??= new List<MagicPowerSkill>
            {
                new("TM_LightningBolt_pwr", "TM_LightningBolt_pwr_desc"),
                new("TM_LightningBolt_eff", "TM_LightningBolt_eff_desc"),
                new("TM_LightningBolt_ver", "TM_LightningBolt_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_LightningCloud =>
            magicPowerSkill_LightningCloud ??= new List<MagicPowerSkill>
            {
                new("TM_LightningCloud_pwr", "TM_LightningCloud_pwr_desc"),
                new("TM_LightningCloud_eff", "TM_LightningCloud_eff_desc"),
                new("TM_LightningCloud_ver", "TM_LightningCloud_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_LightningStorm =>
            magicPowerSkill_LightningStorm ??= new List<MagicPowerSkill>
            {
                new("TM_LightningStorm_pwr", "TM_LightningStorm_pwr_desc"),
                new("TM_LightningStorm_eff", "TM_LightningStorm_eff_desc"),
                new("TM_LightningStorm_ver", "TM_LightningStorm_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_EyeOfTheStorm =>
            magicPowerSkill_EyeOfTheStorm ??= new List<MagicPowerSkill>
            {
                new("TM_EyeOfTheStorm_pwr", "TM_EyeOfTheStorm_pwr_desc"),
                new("TM_EyeOfTheStorm_eff", "TM_EyeOfTheStorm_eff_desc"),
                new("TM_EyeOfTheStorm_ver", "TM_EyeOfTheStorm_ver_desc")
            };

        public List<MagicPower> MagicPowersA =>
            magicPowerA ??= new List<MagicPower>
            {
                new(
                    TorannMagicDefOf.TM_Shadow,
                    TorannMagicDefOf.TM_Shadow_I,
                    TorannMagicDefOf.TM_Shadow_II,
                    TorannMagicDefOf.TM_Shadow_III
                ),
                new(
                    TorannMagicDefOf.TM_MagicMissile,
                    TorannMagicDefOf.TM_MagicMissile_I,
                    TorannMagicDefOf.TM_MagicMissile_II,
                    TorannMagicDefOf.TM_MagicMissile_III
                ),
                new(
                    TorannMagicDefOf.TM_Blink,
                    TorannMagicDefOf.TM_Blink_I,
                    TorannMagicDefOf.TM_Blink_II,
                    TorannMagicDefOf.TM_Blink_III
                ),
                new(
                    TorannMagicDefOf.TM_Summon,
                    TorannMagicDefOf.TM_Summon_I,
                    TorannMagicDefOf.TM_Summon_II,
                    TorannMagicDefOf.TM_Summon_III
                ),
                new(TorannMagicDefOf.TM_Teleport),
                new(true, TorannMagicDefOf.TM_FoldReality),
            };

        public List<MagicPowerSkill> MagicPowerSkill_Shadow =>
            magicPowerSkill_Shadow ??= new List<MagicPowerSkill>
            {
                new("TM_Shadow_eff", "TM_Shadow_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_MagicMissile =>
            magicPowerSkill_MagicMissile ??= new List<MagicPowerSkill>
            {
                new("TM_MagicMissile_eff", "TM_MagicMissile_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Blink =>
            magicPowerSkill_Blink ??= new List<MagicPowerSkill>
            {
                new("TM_Blink_eff", "TM_Blink_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Summon =>
            magicPowerSkill_Summon ??= new List<MagicPowerSkill>
            {
                new("TM_Summon_eff", "TM_Summon_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Teleport =>
            magicPowerSkill_Teleport ??= new List<MagicPowerSkill>
            {
                new("TM_Teleport_pwr", "TM_Teleport_pwr_desc"),
                new("TM_Teleport_eff", "TM_Teleport_eff_desc"),
                new("TM_Teleport_ver", "TM_Teleport_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_FoldReality =>
            magicPowerSkill_FoldReality ??= new List<MagicPowerSkill>
            {
                new("TM_FoldReality_eff", "TM_FoldReality_eff_desc")
            };

        bool hasPaladinBuff;
        public List<MagicPower> MagicPowersP
        {
            get
            {
                magicPowerP ??= new List<MagicPower>
                {
                    new(
                        TorannMagicDefOf.TM_P_RayofHope,
                        TorannMagicDefOf.TM_P_RayofHope_I,
                        TorannMagicDefOf.TM_P_RayofHope_II,
                        TorannMagicDefOf.TM_P_RayofHope_III
                    ),
                    new(TorannMagicDefOf.TM_Heal),
                    new(
                        TorannMagicDefOf.TM_Shield,
                        TorannMagicDefOf.TM_Shield_I,
                        TorannMagicDefOf.TM_Shield_II,
                        TorannMagicDefOf.TM_Shield_III
                    ),
                    new(TorannMagicDefOf.TM_ValiantCharge),
                    new(TorannMagicDefOf.TM_Overwhelm),
                    new(true, TorannMagicDefOf.TM_HolyWrath),
                };

                if (hasPaladinBuff) return magicPowerP;
                if(magicPowerP.Count >= 6)
                {
                    hasPaladinBuff = true;
                }
                if (hasPaladinBuff) return magicPowerP;

                MagicPower pBuff = new MagicPower(new List<VFECore.Abilities.AbilityDef>
                {
                    TorannMagicDefOf.TM_P_RayofHope,
                    TorannMagicDefOf.TM_P_RayofHope_I,
                    TorannMagicDefOf.TM_P_RayofHope_II,
                    TorannMagicDefOf.TM_P_RayofHope_III
                });
                List<MagicPower> oldList = new List<MagicPower> { pBuff };
                oldList.AddRange(magicPowerP);
                magicPowerP = oldList;
                hasPaladinBuff = true;
                return magicPowerP;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_P_RayofHope =>
            magicPowerSkill_P_RayofHope ??= new List<MagicPowerSkill>
            {
                new("TM_P_RayofHope_eff", "TM_P_RayofHope_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Heal =>
            magicPowerSkill_Heal ??= new List<MagicPowerSkill>
            {
                new("TM_Heal_pwr", "TM_Heal_pwr_desc"),
                new("TM_Heal_eff", "TM_Heal_eff_desc"),
                new("TM_Heal_ver", "TM_Heal_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Shield =>
            magicPowerSkill_Shield ??= new List<MagicPowerSkill>
            {
                new("TM_Shield_eff", "TM_Shield_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_ValiantCharge =>
            magicPowerSkill_ValiantCharge ??= new List<MagicPowerSkill>
            {
                new("TM_ValiantCharge_pwr", "TM_ValiantCharge_pwr_desc"),
                new("TM_ValiantCharge_eff", "TM_ValiantCharge_eff_desc"),
                new("TM_ValiantCharge_ver", "TM_ValiantCharge_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Overwhelm =>
            magicPowerSkill_Overwhelm ??= new List<MagicPowerSkill>
            {
                new("TM_Overwhelm_pwr", "TM_Overwhelm_pwr_desc"),
                new("TM_Overwhelm_eff", "TM_Overwhelm_eff_desc"),
                new("TM_Overwhelm_ver", "TM_Overwhelm_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_HolyWrath =>
            magicPowerSkill_HolyWrath ??= new List<MagicPowerSkill>
            {
                new("TM_HolyWrath_pwr", "TM_HolyWrath_pwr_desc"),
                new("TM_HolyWrath_eff", "TM_HolyWrath_eff_desc"),
                new("TM_HolyWrath_ver", "TM_HolyWrath_ver_desc")
            };

        public List<MagicPower> MagicPowersS =>
            magicPowerS ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_SummonMinion),
                new(TorannMagicDefOf.TM_SummonPylon),
                new(TorannMagicDefOf.TM_SummonExplosive),
                new(TorannMagicDefOf.TM_SummonElemental),
                new(true, TorannMagicDefOf.TM_SummonPoppi),
            };

        public List<MagicPowerSkill> MagicPowerSkill_SummonMinion =>
            magicPowerSkill_SummonMinion ??= new List<MagicPowerSkill>
            {
                new("TM_SummonMinion_pwr", "TM_SummonMinion_pwr_desc"),
                new("TM_SummonMinion_eff", "TM_SummonMinion_eff_desc"),
                new("TM_SummonMinion_ver", "TM_SummonMinion_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_SummonPylon =>
            magicPowerSkill_SummonPylon ??= new List<MagicPowerSkill>
            {
                new("TM_SummonPylon_pwr", "TM_SummonPylon_pwr_desc"),
                new("TM_SummonPylon_eff", "TM_SummonPylon_eff_desc"),
                new("TM_SummonPylon_ver", "TM_SummonPylon_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_SummonExplosive =>
            magicPowerSkill_SummonExplosive ??= new List<MagicPowerSkill>
            {
                new("TM_SummonExplosive_pwr", "TM_SummonExplosive_pwr_desc"),
                new("TM_SummonExplosive_eff", "TM_SummonExplosive_eff_desc"),
                new("TM_SummonExplosive_ver", "TM_SummonExplosive_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_SummonElemental =>
            magicPowerSkill_SummonElemental ??= new List<MagicPowerSkill>
            {
                new("TM_SummonElemental_pwr", "TM_SummonElemental_pwr_desc"),
                new("TM_SummonElemental_eff", "TM_SummonElemental_eff_desc"),
                new("TM_SummonElemental_ver", "TM_SummonElemental_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_SummonPoppi =>
            magicPowerSkill_SummonPoppi ??= new List<MagicPowerSkill>
            {
                new("TM_SummonPoppi_pwr", "TM_SummonPoppi_pwr_desc"),
                new("TM_SummonPoppi_eff", "TM_SummonPoppi_eff_desc"),
                new("TM_SummonPoppi_ver", "TM_SummonPoppi_ver_desc")
            };

        public List<MagicPower> MagicPowersD =>
            magicPowerD ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_Poison),
                new(
                    TorannMagicDefOf.TM_SootheAnimal,
                    TorannMagicDefOf.TM_SootheAnimal_I,
                    TorannMagicDefOf.TM_SootheAnimal_II,
                    TorannMagicDefOf.TM_SootheAnimal_III
                ),
                new(TorannMagicDefOf.TM_Regenerate),
                new(TorannMagicDefOf.TM_CureDisease),
                new(true, TorannMagicDefOf.TM_RegrowLimb),
            };

        public List<MagicPowerSkill> MagicPowerSkill_Poison =>
            magicPowerSkill_Poison ??= new List<MagicPowerSkill>
            {
                new("TM_Poison_pwr", "TM_Poison_pwr_desc"),
                new("TM_Poison_eff", "TM_Poison_eff_desc"),
                new("TM_Poison_ver", "TM_Poison_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_SootheAnimal =>
            magicPowerSkill_SootheAnimal ??= new List<MagicPowerSkill>
            {
                new("TM_SootheAnimal_pwr", "TM_SootheAnimal_pwr_desc"),
                new("TM_SootheAnimal_eff", "TM_SootheAnimal_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Regenerate =>
            magicPowerSkill_Regenerate ??= new List<MagicPowerSkill>
            {
                new("TM_Regenerate_pwr", "TM_Regenerate_pwr_desc"),
                new("TM_Regenerate_eff", "TM_Regenerate_eff_desc"),
                new("TM_Regenerate_ver", "TM_Regenerate_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_CureDisease =>
            magicPowerSkill_CureDisease ??= new List<MagicPowerSkill>
            {
                new("TM_CureDisease_pwr", "TM_CureDisease_pwr_desc"),
                new("TM_CureDisease_eff", "TM_CureDisease_eff_desc"),
                new("TM_CureDisease_ver", "TM_CureDisease_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_RegrowLimb =>
            magicPowerSkill_RegrowLimb ??= new List<MagicPowerSkill>
            {
                new("TM_RegrowLimb_eff", "TM_RegrowLimb_eff_desc")
            };

        public List<MagicPower> MagicPowersN
        {
            get
            {
                bool flag = magicPowerN == null;
                if (flag)
                {
                    magicPowerN = new List<MagicPower>
                    {
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_RaiseUndead
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_DeathMark,
                            TorannMagicDefOf.TM_DeathMark_I,
                            TorannMagicDefOf.TM_DeathMark_II,
                            TorannMagicDefOf.TM_DeathMark_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_FogOfTorment
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_ConsumeCorpse,
                            TorannMagicDefOf.TM_ConsumeCorpse_I,
                            TorannMagicDefOf.TM_ConsumeCorpse_II,
                            TorannMagicDefOf.TM_ConsumeCorpse_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_CorpseExplosion,
                            TorannMagicDefOf.TM_CorpseExplosion_I,
                            TorannMagicDefOf.TM_CorpseExplosion_II,
                            TorannMagicDefOf.TM_CorpseExplosion_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_LichForm
                        },true),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_DeathBolt,
                            TorannMagicDefOf.TM_DeathBolt_I,
                            TorannMagicDefOf.TM_DeathBolt_II,
                            TorannMagicDefOf.TM_DeathBolt_III
                        }),
                    };
                }
                return magicPowerN;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_RaiseUndead
        {
            get
            {
                bool flag = magicPowerSkill_RaiseUndead == null;
                if (flag)
                {
                    magicPowerSkill_RaiseUndead = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_RaiseUndead_pwr", "TM_RaiseUndead_pwr_desc"),
                        new MagicPowerSkill("TM_RaiseUndead_eff", "TM_RaiseUndead_eff_desc"),
                        new MagicPowerSkill("TM_RaiseUndead_ver", "TM_RaiseUndead_ver_desc")
                    };
                }
                return magicPowerSkill_RaiseUndead;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_DeathMark
        {
            get
            {
                bool flag = magicPowerSkill_DeathMark == null;
                if (flag)
                {
                    magicPowerSkill_DeathMark = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_DeathMark_pwr", "TM_DeathMark_pwr_desc"),
                        new MagicPowerSkill("TM_DeathMark_eff", "TM_DeathMark_eff_desc"),
                        new MagicPowerSkill("TM_DeathMark_ver", "TM_DeathMark_ver_desc")
                    };
                }
                return magicPowerSkill_DeathMark;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_FogOfTorment
        {
            get
            {
                bool flag = magicPowerSkill_FogOfTorment == null;
                if (flag)
                {
                    magicPowerSkill_FogOfTorment = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_FogOfTorment_pwr", "TM_FogOfTorment_pwr_desc"),
                        new MagicPowerSkill("TM_FogOfTorment_eff", "TM_FogOfTorment_eff_desc"),
                        new MagicPowerSkill("TM_FogOfTorment_ver", "TM_FogOfTorment_ver_desc")
                    };
                }
                return magicPowerSkill_FogOfTorment;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_ConsumeCorpse
        {
            get
            {
                bool flag = magicPowerSkill_ConsumeCorpse == null;
                if (flag)
                {
                    magicPowerSkill_ConsumeCorpse = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_ConsumeCorpse_eff", "TM_ConsumeCorpse_eff_desc"),
                        new MagicPowerSkill("TM_ConsumeCorpse_ver", "TM_ConsumeCorpse_ver_desc")
                    };
                }
                return magicPowerSkill_ConsumeCorpse;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_CorpseExplosion
        {
            get
            {
                bool flag = magicPowerSkill_CorpseExplosion == null;
                if (flag)
                {
                    magicPowerSkill_CorpseExplosion = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_CorpseExplosion_pwr", "TM_CorpseExplosion_pwr_desc"),
                        new MagicPowerSkill("TM_CorpseExplosion_eff", "TM_CorpseExplosion_eff_desc"),
                        new MagicPowerSkill("TM_CorpseExplosion_ver", "TM_CorpseExplosion_ver_desc")
                    };
                }
                return magicPowerSkill_CorpseExplosion;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_LichForm
        {
            get
            {
                bool flag = magicPowerSkill_LichForm == null;
                if (flag)
                {
                    magicPowerSkill_LichForm = new List<MagicPowerSkill>
                    {

                    };
                }
                return magicPowerSkill_LichForm;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_DeathBolt
        {
            get
            {
                bool flag = magicPowerSkill_DeathBolt == null;
                if (flag)
                {
                    magicPowerSkill_DeathBolt = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_DeathBolt_pwr", "TM_DeathBolt_pwr_desc"),
                        new MagicPowerSkill("TM_DeathBolt_eff", "TM_DeathBolt_eff_desc"),
                        new MagicPowerSkill("TM_DeathBolt_ver", "TM_DeathBolt_ver_desc")
                    };
                }
                return magicPowerSkill_DeathBolt;
            }
        }

        public List<MagicPower> MagicPowersPR
        {
            get
            {
                bool flag = magicPowerPR == null;
                if (flag)
                {
                    magicPowerPR = new List<MagicPower>
                    {
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_AdvancedHeal
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Purify
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_HealingCircle,
                            TorannMagicDefOf.TM_HealingCircle_I,
                            TorannMagicDefOf.TM_HealingCircle_II,
                            TorannMagicDefOf.TM_HealingCircle_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_BestowMight,
                            TorannMagicDefOf.TM_BestowMight_I,
                            TorannMagicDefOf.TM_BestowMight_II,
                            TorannMagicDefOf.TM_BestowMight_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Resurrection
                        },true),
                    };
                }
                return magicPowerPR;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_AdvancedHeal
        {
            get
            {
                bool flag = magicPowerSkill_AdvancedHeal == null;
                if (flag)
                {
                    magicPowerSkill_AdvancedHeal = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_AdvancedHeal_pwr", "TM_AdvancedHeal_pwr_desc"),
                        new MagicPowerSkill("TM_AdvancedHeal_eff", "TM_AdvancedHeal_eff_desc"),
                        new MagicPowerSkill("TM_AdvancedHeal_ver", "TM_AdvancedHeal_ver_desc")
                    };
                }
                return magicPowerSkill_AdvancedHeal;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Purify
        {
            get
            {
                bool flag = magicPowerSkill_Purify == null;
                if (flag)
                {
                    magicPowerSkill_Purify = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Purify_pwr", "TM_Purify_pwr_desc"),
                        new MagicPowerSkill("TM_Purify_eff", "TM_Purify_eff_desc"),
                        new MagicPowerSkill("TM_Purify_ver", "TM_Purify_ver_desc")
                    };
                }
                return magicPowerSkill_Purify;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_HealingCircle
        {
            get
            {
                bool flag = magicPowerSkill_HealingCircle == null;
                if (flag)
                {
                    magicPowerSkill_HealingCircle = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_HealingCircle_pwr", "TM_HealingCircle_pwr_desc"),
                        new MagicPowerSkill("TM_HealingCircle_eff", "TM_HealingCircle_eff_desc"),
                        new MagicPowerSkill("TM_HealingCircle_ver", "TM_HealingCircle_ver_desc")
                    };
                }
                return magicPowerSkill_HealingCircle;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_BestowMight
        {
            get
            {
                bool flag = magicPowerSkill_BestowMight == null;
                if (flag)
                {
                    magicPowerSkill_BestowMight = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_BestowMight_eff", "TM_BestowMight_eff_desc")
                    };
                }
                return magicPowerSkill_BestowMight;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Resurrection
        {
            get
            {
                bool flag = magicPowerSkill_Resurrection == null;
                if (flag)
                {
                    magicPowerSkill_Resurrection = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Resurrection_eff", "TM_Resurrection_eff_desc"),
                        new MagicPowerSkill("TM_Resurrection_ver", "TM_Resurrection_ver_desc")
                    };
                }
                return magicPowerSkill_Resurrection;
            }
        }

        public List<MagicPower> MagicPowersB
        {
            get
            {
                bool flag = magicPowerB == null;
                if (flag)
                {
                    magicPowerB = new List<MagicPower>
                    {
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_BardTraining
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Entertain
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Inspire
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Lullaby,
                            TorannMagicDefOf.TM_Lullaby_I,
                            TorannMagicDefOf.TM_Lullaby_II,
                            TorannMagicDefOf.TM_Lullaby_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_BattleHymn
                        },true),
                    };
                }
                return magicPowerB;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_BardTraining
        {
            get
            {
                bool flag = magicPowerSkill_BardTraining == null;
                if (flag)
                {
                    magicPowerSkill_BardTraining = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_BardTraining_pwr", "TM_BardTraining_pwr_desc")
                    };
                }
                return magicPowerSkill_BardTraining;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Entertain
        {
            get
            {
                bool flag = magicPowerSkill_Entertain == null;
                if (flag)
                {
                    magicPowerSkill_Entertain = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Entertain_pwr", "TM_Entertain_pwr_desc"),
                        new MagicPowerSkill("TM_Entertain_ver", "TM_Entertain_ver_desc")
                    };
                }
                return magicPowerSkill_Entertain;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Inspire
        {
            get
            {
                bool flag = magicPowerSkill_Inspire == null;
                if (flag)
                {
                    magicPowerSkill_Inspire = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Inspire_pwr", "TM_Inspire_pwr_desc"),
                        new MagicPowerSkill("TM_Inspire_ver", "TM_Inspire_ver_desc")
                    };
                }
                return magicPowerSkill_Inspire;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Lullaby
        {
            get
            {
                bool flag = magicPowerSkill_Lullaby == null;
                if (flag)
                {
                    magicPowerSkill_Lullaby = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Lullaby_pwr", "TM_Lullaby_pwr_desc"),
                        new MagicPowerSkill("TM_Lullaby_eff", "TM_Lullaby_eff_desc"),
                        new MagicPowerSkill("TM_Lullaby_ver", "TM_Lullaby_ver_desc")
                    };
                }
                return magicPowerSkill_Lullaby;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_BattleHymn
        {
            get
            {
                bool flag = magicPowerSkill_BattleHymn == null;
                if (flag)
                {
                    magicPowerSkill_BattleHymn = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_BattleHymn_pwr", "TM_BattleHymn_pwr_desc"),
                        new MagicPowerSkill("TM_BattleHymn_eff", "TM_BattleHymn_eff_desc"),
                        new MagicPowerSkill("TM_BattleHymn_ver", "TM_BattleHymn_ver_desc")
                    };
                }
                return magicPowerSkill_BattleHymn;
            }
        }

        public List<MagicPower> MagicPowersWD
        {
            get
            {
                bool flag = magicPowerWD == null;
                if (flag)
                {
                    magicPowerWD = new List<MagicPower>
                    {
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_SoulBond
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_ShadowBolt,
                            TorannMagicDefOf.TM_ShadowBolt_I,
                            TorannMagicDefOf.TM_ShadowBolt_II,
                            TorannMagicDefOf.TM_ShadowBolt_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Dominate
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Repulsion,
                            TorannMagicDefOf.TM_Repulsion_I,
                            TorannMagicDefOf.TM_Repulsion_II,
                            TorannMagicDefOf.TM_Repulsion_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_PsychicShock
                        },true),
                        //new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        //{
                        //    TorannMagicDefOf.TM_SummonDemon
                        //}),
                    };
                }
                return magicPowerWD;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_SoulBond
        {
            get
            {
                bool flag = magicPowerSkill_SoulBond == null;
                if (flag)
                {
                    magicPowerSkill_SoulBond = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_SoulBond_pwr", "TM_SoulBond_pwr_desc"),
                        new MagicPowerSkill("TM_SoulBond_eff", "TM_SoulBond_eff_desc"),
                        new MagicPowerSkill("TM_SoulBond_ver", "TM_SoulBond_ver_desc")
                    };
                }
                return magicPowerSkill_SoulBond;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_ShadowBolt
        {
            get
            {
                bool flag = magicPowerSkill_ShadowBolt == null;
                if (flag)
                {
                    magicPowerSkill_ShadowBolt = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_ShadowBolt_pwr", "TM_ShadowBolt_pwr_desc"),
                        new MagicPowerSkill("TM_ShadowBolt_eff", "TM_ShadowBolt_eff_desc"),
                        new MagicPowerSkill("TM_ShadowBolt_ver", "TM_ShadowBolt_ver_desc")
                    };
                }
                return magicPowerSkill_ShadowBolt;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Dominate
        {
            get
            {
                bool flag = magicPowerSkill_Dominate == null;
                if (flag)
                {
                    magicPowerSkill_Dominate = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Dominate_pwr", "TM_Dominate_pwr_desc"),
                        new MagicPowerSkill("TM_Dominate_eff", "TM_Dominate_eff_desc"),
                        new MagicPowerSkill("TM_Dominate_ver", "TM_Dominate_ver_desc")
                    };
                }
                return magicPowerSkill_Dominate;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Repulsion
        {
            get
            {
                if (magicPowerSkill_Repulsion == null)
                {
                    magicPowerSkill_Repulsion = new List<MagicPowerSkill>
                    {
                        new ("TM_Repulsion_pwr", "TM_Repulsion_pwr_desc"),
                        new ("TM_Repulsion_eff", "TM_Repulsion_eff_desc"),
                        new ("TM_Repulsion_ver", "TM_Repulsion_ver_desc")
                    };
                }
                return magicPowerSkill_Repulsion;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_PsychicShock
        {
            get
            {
                bool flag = magicPowerSkill_PsychicShock == null;
                if (flag)
                {
                    magicPowerSkill_PsychicShock = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_PsychicShock_pwr", "TM_PsychicShock_pwr_desc"),
                        new MagicPowerSkill("TM_PsychicShock_eff", "TM_PsychicShock_eff_desc"),
                        new MagicPowerSkill("TM_PsychicShock_ver", "TM_PsychicShock_ver_desc")
                    };
                }
                return magicPowerSkill_PsychicShock;
            }
        }

        public List<MagicPower> MagicPowersSD
        {
            get
            {
                bool flag = magicPowerSD == null;
                if (flag)
                {
                    magicPowerSD = new List<MagicPower>
                    {
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_SoulBond
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_ShadowBolt,
                            TorannMagicDefOf.TM_ShadowBolt_I,
                            TorannMagicDefOf.TM_ShadowBolt_II,
                            TorannMagicDefOf.TM_ShadowBolt_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Dominate
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Attraction,
                            TorannMagicDefOf.TM_Attraction_I,
                            TorannMagicDefOf.TM_Attraction_II,
                            TorannMagicDefOf.TM_Attraction_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Scorn
                        },true),
                        //new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        //{
                        //    TorannMagicDefOf.TM_SummonDemon
                        //}),
                    };
                }
                return magicPowerSD;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Attraction
        {
            get
            {
                bool flag = magicPowerSkill_Attraction == null;
                if (flag)
                {
                    magicPowerSkill_Attraction = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Attraction_pwr", "TM_Attraction_pwr_desc"),
                        new MagicPowerSkill("TM_Attraction_eff", "TM_Attraction_eff_desc"),
                        new MagicPowerSkill("TM_Attraction_ver", "TM_Attraction_ver_desc")
                    };
                }
                return magicPowerSkill_Attraction;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Scorn
        {
            get
            {
                bool flag = magicPowerSkill_Scorn == null;
                if (flag)
                {
                    magicPowerSkill_Scorn = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Scorn_pwr", "TM_Scorn_pwr_desc"),
                        new MagicPowerSkill("TM_Scorn_eff", "TM_Scorn_eff_desc"),
                        new MagicPowerSkill("TM_Scorn_ver", "TM_Scorn_ver_desc")
                    };
                }
                return magicPowerSkill_Scorn;
            }
        }

        public List<MagicPower> MagicPowersG
        {
            get
            {
                bool flag = magicPowerG == null;
                if (flag)
                {
                    magicPowerG = new List<MagicPower>
                    {
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Stoneskin
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Encase,
                            TorannMagicDefOf.TM_Encase_I,
                            TorannMagicDefOf.TM_Encase_II,
                            TorannMagicDefOf.TM_Encase_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_EarthSprites
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_EarthernHammer
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Sentinel
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Meteor,
                            TorannMagicDefOf.TM_Meteor_I,
                            TorannMagicDefOf.TM_Meteor_II,
                            TorannMagicDefOf.TM_Meteor_III
                        },true),
                    };
                }
                return magicPowerG;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Stoneskin
        {
            get
            {
                bool flag = magicPowerSkill_Stoneskin == null;
                if (flag)
                {
                    magicPowerSkill_Stoneskin = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Stoneskin_pwr", "TM_Stoneskin_pwr_desc"),
                        new MagicPowerSkill("TM_Stoneskin_eff", "TM_Stoneskin_eff_desc"),
                        new MagicPowerSkill("TM_Stoneskin_ver", "TM_Stoneskin_ver_desc")

                    };
                }
                return magicPowerSkill_Stoneskin;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Encase
        {
            get
            {
                bool flag = magicPowerSkill_Encase == null;
                if (flag)
                {
                    magicPowerSkill_Encase = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Encase_pwr", "TM_Encase_pwr_desc"),
                        new MagicPowerSkill("TM_Encase_eff", "TM_Encase_eff_desc"),
                        new MagicPowerSkill("TM_Encase_ver", "TM_Encase_ver_desc")
                    };
                }
                return magicPowerSkill_Encase;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_EarthSprites
        {
            get
            {
                bool flag = magicPowerSkill_EarthSprites == null;
                if (flag)
                {
                    magicPowerSkill_EarthSprites = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_EarthSprites_pwr", "TM_EarthSprites_pwr_desc"),
                        new MagicPowerSkill("TM_EarthSprites_eff", "TM_EarthSprites_eff_desc"),
                        new MagicPowerSkill("TM_EarthSprites_ver", "TM_EarthSprites_ver_desc")
                    };
                }
                return magicPowerSkill_EarthSprites;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_EarthernHammer
        {
            get
            {
                bool flag = magicPowerSkill_EarthernHammer == null;
                if (flag)
                {
                    magicPowerSkill_EarthernHammer = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_EarthernHammer_pwr", "TM_EarthernHammer_pwr_desc" ),
                        new MagicPowerSkill("TM_EarthernHammer_eff", "TM_EarthernHammer_eff_desc" ),
                        new MagicPowerSkill("TM_EarthernHammer_ver", "TM_EarthernHammer_ver_desc" )
                    };
                }
                return magicPowerSkill_EarthernHammer;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Sentinel
        {
            get
            {
                bool flag = magicPowerSkill_Sentinel == null;
                if (flag)
                {
                    magicPowerSkill_Sentinel = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Sentinel_pwr", "TM_Sentinel_pwr_desc"),
                        new MagicPowerSkill("TM_Sentinel_eff", "TM_Sentinel_eff_desc"),
                        new MagicPowerSkill("TM_Sentinel_ver", "TM_Sentinel_ver_desc")
                    };
                }
                return magicPowerSkill_Sentinel;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Meteor
        {
            get
            {
                bool flag = magicPowerSkill_Meteor == null;
                if (flag)
                {
                    magicPowerSkill_Meteor = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Meteor_ver", "TM_Meteor_ver_desc"),
                        new MagicPowerSkill("TM_Meteor_eff", "TM_Meteor_eff_desc")
                    };
                }
                return magicPowerSkill_Meteor;
            }
        }

        public List<MagicPower> MagicPowersT
        {
            get
            {
                bool flag = magicPowerT == null;
                if (flag)
                {
                    magicPowerT = new List<MagicPower>
                    {
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_TechnoBit
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_TechnoTurret
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_TechnoWeapon
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_TechnoShield
                        },true),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Overdrive
                        },true),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Sabotage
                        },true),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_OrbitalStrike,
                            TorannMagicDefOf.TM_OrbitalStrike_I,
                            TorannMagicDefOf.TM_OrbitalStrike_II,
                            TorannMagicDefOf.TM_OrbitalStrike_III
                        },true),
                    };
                }
                return magicPowerT;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_TechnoBit
        {
            get
            {
                bool flag = magicPowerSkill_TechnoBit == null;
                if (flag)
                {
                    magicPowerSkill_TechnoBit = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_TechnoBit_pwr", "TM_TechnoBit_pwr_desc"),
                        new MagicPowerSkill("TM_TechnoBit_eff", "TM_TechnoBit_eff_desc"),
                        new MagicPowerSkill("TM_TechnoBit_ver", "TM_TechnoBit_ver_desc")
                    };
                }
                return magicPowerSkill_TechnoBit;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_TechnoTurret
        {
            get
            {
                bool flag = magicPowerSkill_TechnoTurret == null;
                if (flag)
                {
                    magicPowerSkill_TechnoTurret = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_TechnoTurret_pwr", "TM_TechnoTurret_pwr_desc"),
                        new MagicPowerSkill("TM_TechnoTurret_eff", "TM_TechnoTurret_eff_desc"),
                        new MagicPowerSkill("TM_TechnoTurret_ver", "TM_TechnoTurret_ver_desc")
                    };
                }
                return magicPowerSkill_TechnoTurret;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_TechnoWeapon
        {
            get
            {
                bool flag = magicPowerSkill_TechnoWeapon == null;
                if (flag)
                {
                    magicPowerSkill_TechnoWeapon = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_TechnoWeapon_pwr", "TM_TechnoWeapon_pwr_desc"),
                        new MagicPowerSkill("TM_TechnoWeapon_eff", "TM_TechnoWeapon_eff_desc"),
                        new MagicPowerSkill("TM_TechnoWeapon_ver", "TM_TechnoWeapon_ver_desc")
                    };
                }
                return magicPowerSkill_TechnoWeapon;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_TechnoShield
        {
            get
            {
                bool flag = magicPowerSkill_TechnoShield == null;
                if (flag)
                {
                    magicPowerSkill_TechnoShield = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_TechnoShield_pwr", "TM_TechnoShield_pwr_desc"),
                        new MagicPowerSkill("TM_TechnoShield_eff", "TM_TechnoShield_eff_desc"),
                        new MagicPowerSkill("TM_TechnoShield_ver", "TM_TechnoShield_ver_desc")
                    };
                }
                return magicPowerSkill_TechnoShield;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Sabotage
        {
            get
            {
                bool flag = magicPowerSkill_Sabotage == null;
                if (flag)
                {
                    magicPowerSkill_Sabotage = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Sabotage_pwr", "TM_Sabotage_pwr_desc"),
                        new MagicPowerSkill("TM_Sabotage_eff", "TM_Sabotage_eff_desc"),
                        new MagicPowerSkill("TM_Sabotage_ver", "TM_Sabotage_ver_desc")
                    };
                }
                return magicPowerSkill_Sabotage;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Overdrive
        {
            get
            {
                bool flag = magicPowerSkill_Overdrive == null;
                if (flag)
                {
                    magicPowerSkill_Overdrive = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Overdrive_pwr", "TM_Overdrive_pwr_desc"),
                        new MagicPowerSkill("TM_Overdrive_eff", "TM_Overdrive_eff_desc"),
                        new MagicPowerSkill("TM_Overdrive_ver", "TM_Overdrive_ver_desc")
                    };
                }
                return magicPowerSkill_Overdrive;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_OrbitalStrike
        {
            get
            {
                bool flag = magicPowerSkill_OrbitalStrike == null;
                if (flag)
                {
                    magicPowerSkill_OrbitalStrike = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_OrbitalStrike_pwr", "TM_OrbitalStrike_pwr_desc"),
                        new MagicPowerSkill("TM_OrbitalStrike_eff", "TM_OrbitalStrike_eff_desc"),
                        new MagicPowerSkill("TM_OrbitalStrike_ver", "TM_OrbitalStrike_ver_desc")
                    };
                }
                return magicPowerSkill_OrbitalStrike;
            }
        }

        public List<MagicPower> MagicPowersBM
        {
            get
            {
                bool flag = magicPowerBM == null;
                if (flag)
                {
                    magicPowerBM = new List<MagicPower>
                    {
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_BloodGift
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_IgniteBlood
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_BloodForBlood
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_BloodShield
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Rend,
                            TorannMagicDefOf.TM_Rend_I,
                            TorannMagicDefOf.TM_Rend_II,
                            TorannMagicDefOf.TM_Rend_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_BloodMoon,
                            TorannMagicDefOf.TM_BloodMoon_I,
                            TorannMagicDefOf.TM_BloodMoon_II,
                            TorannMagicDefOf.TM_BloodMoon_III
                        }, true),
                    };
                }
                return magicPowerBM;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_BloodGift
        {
            get
            {
                bool flag = magicPowerSkill_BloodGift == null;
                if (flag)
                {
                    magicPowerSkill_BloodGift = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_BloodGift_pwr", "TM_BloodGift_pwr_desc"),
                        new MagicPowerSkill("TM_BloodGift_eff", "TM_BloodGift_eff_desc"),
                        new MagicPowerSkill("TM_BloodGift_ver", "TM_BloodGift_ver_desc")

                    };
                }
                return magicPowerSkill_BloodGift;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_IgniteBlood
        {
            get
            {
                bool flag = magicPowerSkill_IgniteBlood == null;
                if (flag)
                {
                    magicPowerSkill_IgniteBlood = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_IgniteBlood_pwr", "TM_IgniteBlood_pwr_desc"),
                        new MagicPowerSkill("TM_IgniteBlood_eff", "TM_IgniteBlood_eff_desc"),
                        new MagicPowerSkill("TM_IgniteBlood_ver", "TM_IgniteBlood_ver_desc")
                    };
                }
                return magicPowerSkill_IgniteBlood;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_BloodForBlood
        {
            get
            {
                bool flag = magicPowerSkill_BloodForBlood == null;
                if (flag)
                {
                    magicPowerSkill_BloodForBlood = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_BloodForBlood_pwr", "TM_BloodForBlood_pwr_desc" ),
                        new MagicPowerSkill("TM_BloodForBlood_eff", "TM_BloodForBlood_eff_desc" ),
                        new MagicPowerSkill("TM_BloodForBlood_ver", "TM_BloodForBlood_ver_desc" )
                    };
                }
                return magicPowerSkill_BloodForBlood;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_BloodShield
        {
            get
            {
                bool flag = magicPowerSkill_BloodShield == null;
                if (flag)
                {
                    magicPowerSkill_BloodShield = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_BloodShield_pwr", "TM_BloodShield_pwr_desc"),
                        new MagicPowerSkill("TM_BloodShield_eff", "TM_BloodShield_eff_desc"),
                        new MagicPowerSkill("TM_BloodShield_ver", "TM_BloodShield_ver_desc")
                    };
                }
                return magicPowerSkill_BloodShield;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Rend
        {
            get
            {
                bool flag = magicPowerSkill_Rend == null;
                if (flag)
                {
                    magicPowerSkill_Rend = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_Rend_pwr", "TM_Rend_pwr_desc"),
                        new MagicPowerSkill("TM_Rend_eff", "TM_Rend_eff_desc"),
                        new MagicPowerSkill("TM_Rend_ver", "TM_Rend_ver_desc")
                    };
                }
                return magicPowerSkill_Rend;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_BloodMoon
        {
            get
            {
                bool flag = magicPowerSkill_BloodMoon == null;
                if (flag)
                {
                    magicPowerSkill_BloodMoon = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_BloodMoon_pwr", "TM_BloodMoon_pwr_desc"),
                        new MagicPowerSkill("TM_BloodMoon_eff", "TM_BloodMoon_eff_desc"),
                        new MagicPowerSkill("TM_BloodMoon_ver", "TM_BloodMoon_ver_desc")
                    };
                }
                return magicPowerSkill_BloodMoon;
            }
        }

        public List<MagicPower> MagicPowersE
        {
            get
            {
                bool flag = magicPowerE == null;
                if (flag)
                {
                    magicPowerE = new List<MagicPower>
                    {
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_EnchantedBody
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Transmutate
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_EnchanterStone
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_EnchantWeapon
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Polymorph,
                            TorannMagicDefOf.TM_Polymorph_I,
                            TorannMagicDefOf.TM_Polymorph_II,
                            TorannMagicDefOf.TM_Polymorph_III
                        }),
                        new MagicPower(new List<VFECore.Abilities.AbilityDef>
                        {
                            TorannMagicDefOf.TM_Shapeshift
                        }, true),
                    };
                }
                return magicPowerE;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_EnchantedBody
        {
            get
            {
                bool flag = magicPowerSkill_EnchantedBody == null;
                if (flag)
                {
                    magicPowerSkill_EnchantedBody = new List<MagicPowerSkill>
                    {
                        new MagicPowerSkill("TM_EnchantedBody_pwr", "TM_EnchantedBody_pwr_desc"),
                        new MagicPowerSkill("TM_EnchantedBody_eff", "TM_EnchantedBody_eff_desc"),
                        new MagicPowerSkill("TM_EnchantedBody_ver", "TM_EnchantedBody_ver_desc")

                    };
                }
                return magicPowerSkill_EnchantedBody;
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_Transmutate
        {
            get
            {
                return magicPowerSkill_Transmutate ??= new List<MagicPowerSkill>
                {
                    new("TM_Transmutate_pwr", "TM_Transmutate_pwr_desc"),
                    new("TM_Transmutate_eff", "TM_Transmutate_eff_desc"),
                    new("TM_Transmutate_ver", "TM_Transmutate_ver_desc")
                };
            }
        }
        public List<MagicPowerSkill> MagicPowerSkill_EnchanterStone =>
            magicPowerSkill_EnchanterStone ??= new List<MagicPowerSkill>
            {
                new("TM_EnchanterStone_eff", "TM_EnchanterStone_eff_desc"),
                new("TM_EnchanterStone_ver", "TM_EnchanterStone_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_EnchantWeapon =>
            magicPowerSkill_EnchantWeapon ??= new List<MagicPowerSkill>
            {
                new("TM_EnchantWeapon_pwr", "TM_EnchantWeapon_pwr_desc"),
                new("TM_EnchantWeapon_eff", "TM_EnchantWeapon_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Polymorph =>
            magicPowerSkill_Polymorph ??= new List<MagicPowerSkill>
            {
                new("TM_Polymorph_pwr", "TM_Polymorph_pwr_desc"),
                new("TM_Polymorph_eff", "TM_Polymorph_eff_desc"),
                new("TM_Polymorph_ver", "TM_Polymorph_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Shapeshift =>
            magicPowerSkill_Shapeshift ??= new List<MagicPowerSkill>
            {
                new("TM_Shapeshift_pwr", "TM_Shapeshift_pwr_desc"),
                new("TM_Shapeshift_eff", "TM_Shapeshift_eff_desc"),
                new("TM_Shapeshift_ver", "TM_Shapeshift_ver_desc")
            };

        public List<MagicPower> MagicPowersC =>
            magicPowerC ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_Prediction),
                new(TorannMagicDefOf.TM_AlterFate),
                new(TorannMagicDefOf.TM_AccelerateTime),
                new(TorannMagicDefOf.TM_ReverseTime),
                new(
                    TorannMagicDefOf.TM_ChronostaticField,
                    TorannMagicDefOf.TM_ChronostaticField_I,
                    TorannMagicDefOf.TM_ChronostaticField_II,
                    TorannMagicDefOf.TM_ChronostaticField_III
                ),
                new(true, TorannMagicDefOf.TM_Recall),
            };

        public List<MagicPowerSkill> MagicPowerSkill_Prediction =>
            magicPowerSkill_Prediction ??= new List<MagicPowerSkill>
            {
                new("TM_Prediction_pwr", "TM_Prediction_pwr_desc"),
                new("TM_Prediction_eff", "TM_Prediction_eff_desc"),
                new("TM_Prediction_ver", "TM_Prediction_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_AlterFate =>
            magicPowerSkill_AlterFate ??= new List<MagicPowerSkill>
            {
                new("TM_AlterFate_pwr", "TM_AlterFate_pwr_desc"),
                new("TM_AlterFate_eff", "TM_AlterFate_eff_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_AccelerateTime =>
            magicPowerSkill_AccelerateTime ??= new List<MagicPowerSkill>
            {
                new("TM_AccelerateTime_pwr", "TM_AccelerateTime_pwr_desc"),
                new("TM_AccelerateTime_eff", "TM_AccelerateTime_eff_desc"),
                new("TM_AccelerateTime_ver", "TM_AccelerateTime_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_ReverseTime =>
            magicPowerSkill_ReverseTime ??= new List<MagicPowerSkill>
            {
                new("TM_ReverseTime_pwr", "TM_ReverseTime_pwr_desc"),
                new("TM_ReverseTime_eff", "TM_ReverseTime_eff_desc"),
                new("TM_ReverseTime_ver", "TM_ReverseTime_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_ChronostaticField =>
            magicPowerSkill_ChronostaticField ??= new List<MagicPowerSkill>
            {
                new("TM_ChronostaticField_pwr", "TM_ChronostaticField_pwr_desc"),
                new("TM_ChronostaticField_eff", "TM_ChronostaticField_eff_desc"),
                new("TM_ChronostaticField_ver", "TM_ChronostaticField_ver_desc")
            };

        public List<MagicPowerSkill> MagicPowerSkill_Recall =>
            magicPowerSkill_Recall ??= new List<MagicPowerSkill>
            {
                new("TM_Recall_pwr", "TM_Recall_pwr_desc"),
                new("TM_Recall_eff", "TM_Recall_eff_desc"),
                new("TM_Recall_ver", "TM_Recall_ver_desc")
            };

        public List<MagicPower> MagicPowersCM =>
            magicPowerCM ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_ChaosTradition),
            };

        public List<MagicPowerSkill> MagicPowerSkill_ChaosTradition =>
            magicPowerSkill_ChaosTradition ??= new List<MagicPowerSkill>
            {
                new("TM_ChaosTradition_pwr", "TM_ChaosTradition_pwr_desc"),
                new("TM_ChaosTradition_eff", "TM_ChaosTradition_eff_desc"),
                new("TM_ChaosTradition_ver", "TM_ChaosTradition_ver_desc")
            };

        public List<MagicPower> MagicPowersShadow =>
            magicPowerShadow ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_ShadowWalk),
            };

        public List<MagicPowerSkill> MagicPowerSkill_ShadowWalk =>
            magicPowerSkill_ShadowWalk ??= new List<MagicPowerSkill>
            {
                new("TM_ShadowWalk_pwr", "TM_ShadowWalk_pwr_desc"), // applies invisibility and duration of invisibility
                new("TM_ShadowWalk_eff", "TM_ShadowWalk_eff_desc"), // reduces mana cost
                new("TM_ShadowWalk_ver", "TM_ShadowWalk_ver_desc") // heals and can invis target
            };

        public List<MagicPower> MagicPowersBrightmage =>
            magicPowerBrightmage ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_LightLance),
                new(
                    TorannMagicDefOf.TM_Sunfire,
                    TorannMagicDefOf.TM_Sunfire_I,
                    TorannMagicDefOf.TM_Sunfire_II,
                    TorannMagicDefOf.TM_Sunfire_III
                ),
                new(TorannMagicDefOf.TM_LightBurst),
                new(TorannMagicDefOf.TM_LightSkip),
                new(TorannMagicDefOf.TM_Refraction),
                new(true, TorannMagicDefOf.TM_SpiritOfLight),
            };

        public List<MagicPowerSkill> MagicPowerSkill_LightLance =>
            magicPowerSkill_LightLance ??= new List<MagicPowerSkill>
            {
                new("TM_LightLance_pwr", "TM_LightLance_pwr_desc"), //damage
                new("TM_LightLance_eff", "TM_LightLance_eff_desc"), //mana cost
                new("TM_LightLance_ver", "TM_LightLance_ver_desc") //beam width and duration
            };

        public List<MagicPowerSkill> MagicPowerSkill_Sunfire =>
            magicPowerSkill_Sunfire ??= new List<MagicPowerSkill>
            {
                new("TM_Sunfire_pwr", "TM_Sunfire_pwr_desc"), //damage
                new("TM_Sunfire_eff", "TM_Sunfire_eff_desc"), //mana cost
                new("TM_Sunfire_ver", "TM_Sunfire_ver_desc") //lance count
            };

        public List<MagicPowerSkill> MagicPowerSkill_LightBurst =>
            magicPowerSkill_LightBurst ??= new List<MagicPowerSkill>
            {
                new("TM_LightBurst_pwr", "TM_LightBurst_pwr_desc"), //hediff severity
                new("TM_LightBurst_eff", "TM_LightBurst_eff_desc"), //mana cost
                new("TM_LightBurst_ver", "TM_LightBurst_ver_desc") //effects mechanoids, redirects bullets to another target or "spray"
            };

        public List<MagicPowerSkill> MagicPowerSkill_LightSkip =>
            magicPowerSkill_LightSkip ??= new List<MagicPowerSkill>
            {
                new("TM_LightSkip_pwr", "TM_LightSkip_pwr_desc"), //3 tiers, self->aoe->global (2pts per)
                new("TM_LightSkip_eff", "TM_LightSkip_eff_desc") //mana cost, light requirement
            };

        public List<MagicPowerSkill> MagicPowerSkill_Refraction =>
            magicPowerSkill_Refraction ??= new List<MagicPowerSkill>
            {
                new("TM_Refraction_pwr", "TM_Refraction_pwr_desc"), //+friendly accuracy; -hostile accuracy
                new("TM_Refraction_eff", "TM_Refraction_eff_desc"), //mana cost
                new("TM_Refraction_ver", "TM_Refraction_ver_desc") //duration
            };

        public List<MagicPowerSkill> MagicPowerSkill_SpiritOfLight =>
            magicPowerSkill_SpiritOfLight ??= new List<MagicPowerSkill>
            {
                new("TM_SpiritOfLight_pwr", "TM_SpiritOfLight_pwr_desc"), //increases damage of abilities, reduces delay between abilities
                new("TM_SpiritOfLight_eff", "TM_SpiritOfLight_eff_desc"), //decreased upkeep cost
                new("TM_SpiritOfLight_ver", "TM_SpiritOfLight_ver_desc") //increased light energy gain and increased max capacity
            };

        public List<MagicPower> MagicPowersShaman =>
            magicPowerShaman ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_Totems),
                new(TorannMagicDefOf.TM_ChainLightning),
                new(TorannMagicDefOf.TM_Enrage),
                new(
                    TorannMagicDefOf.TM_Hex,
                    TorannMagicDefOf.TM_Hex_I,
                    TorannMagicDefOf.TM_Hex_II,
                    TorannMagicDefOf.TM_Hex_III
                ),
                new(
                    TorannMagicDefOf.TM_SpiritWolves,
                    TorannMagicDefOf.TM_SpiritWolves_I,
                    TorannMagicDefOf.TM_SpiritWolves_II,
                    TorannMagicDefOf.TM_SpiritWolves_III
                ),
                new(true, TorannMagicDefOf.TM_GuardianSpirit),
            };

        public List<MagicPowerSkill> MagicPowerSkill_Totems =>
            magicPowerSkill_Totems ??= new List<MagicPowerSkill>
            {
                new("TM_Totems_pwr", "TM_Totems_pwr_desc"), // power of totems
                new("TM_Totems_eff", "TM_Totems_eff_desc"), // mana cost
                new("TM_Totems_ver", "TM_Totems_ver_desc") // duration
            };

        public List<MagicPowerSkill> MagicPowerSkill_ChainLightning =>
            magicPowerSkill_ChainLightning ??= new List<MagicPowerSkill>
            {
                new("TM_ChainLightning_pwr", "TM_ChainLightning_pwr_desc"), // damage
                new("TM_ChainLightning_eff", "TM_ChainLightning_eff_desc"), // reduces mana cost
                new("TM_ChainLightning_ver", "TM_ChainLightning_ver_desc") // number of forks, fork count
            };

        public List<MagicPowerSkill> MagicPowerSkill_Enrage =>
            magicPowerSkill_Enrage ??= new List<MagicPowerSkill>
            {
                new("TM_Enrage_pwr", "TM_Enrage_pwr_desc"), // severity
                new("TM_Enrage_eff", "TM_Enrage_eff_desc"), // reduces mana cost
                new("TM_Enrage_ver", "TM_Enrage_ver_desc") // application count
            };

        public List<MagicPowerSkill> MagicPowerSkill_Hex =>
            magicPowerSkill_Hex ??= new List<MagicPowerSkill>
            {
                new("TM_Hex_pwr", "TM_Hex_pwr_desc"), // severity, what other effects are available after hexing
                new("TM_Hex_eff", "TM_Hex_eff_desc"), // reduces mana cost
                new("TM_Hex_ver", "TM_Hex_ver_desc") // appication count, other stuff related to hex actions
            };

        public List<MagicPowerSkill> MagicPowerSkill_SpiritWolves =>
            magicPowerSkill_SpiritWolves ??= new List<MagicPowerSkill>
            {
                new("TM_SpiritWolves_pwr", "TM_SpiritWolves_pwr_desc"), // damage
                new("TM_SpiritWolves_eff", "TM_SpiritWolves_eff_desc"), // reduces mana cost
                new("TM_SpiritWolves_ver", "TM_SpiritWolves_ver_desc") // width
            };

        public List<MagicPowerSkill> MagicPowerSkill_GuardianSpirit =>
            magicPowerSkill_GuardianSpirit ??= new List<MagicPowerSkill>
            {
                new("TM_GuardianSpirit_pwr", "TM_GuardianSpirit_pwr_desc"), // power of GuardianSpirit
                new("TM_GuardianSpirit_eff", "TM_GuardianSpirit_eff_desc"), // mana cost
                new("TM_GuardianSpirit_ver", "TM_GuardianSpirit_ver_desc") // effect radius, effects
            };

        public List<MagicPower> MagicPowersGolemancer =>
            magicPowerGolemancer ??= new List<MagicPower>
            {
                new(TorannMagicDefOf.TM_Golemancy),
                new(TorannMagicDefOf.TM_RuneCarving),
                new(TorannMagicDefOf.TM_Branding),
                new(TorannMagicDefOf.TM_SigilSurge),
                new(TorannMagicDefOf.TM_SigilDrain),
                new(true, TorannMagicDefOf.TM_LivingWall),
            };

        public List<MagicPowerSkill> MagicPowerSkill_Golemancy =>
            magicPowerSkill_Golemancy ??= new List<MagicPowerSkill>
            {
                new("TM_Golemancy_pwr", "TM_Golemancy_pwr_desc"), // power of golems, 15 ranks
                new("TM_Golemancy_eff", "TM_Golemancy_eff_desc"), // mana cost to upkeep a golem, 15 ranks
                new("TM_Golemancy_ver", "TM_Golemancy_ver_desc") // abilities and skills available to a golem, 15 ranks
            };

        public List<MagicPowerSkill> MagicPowerSkill_RuneCarving =>
            magicPowerSkill_RuneCarving ??= new List<MagicPowerSkill>
            {
                new("TM_RuneCarving_pwr", "TM_RuneCarving_pwr_desc"), // efficiency boost to parts, 3 ranks, 2 pt level cost
                new("TM_RuneCarving_eff", "TM_RuneCarving_eff_desc"), // returns 10% mana per skill level
                new("TM_RuneCarving_ver", "TM_RuneCarving_ver_desc") // increases chance of success by 5%
            };

        public List<MagicPowerSkill> MagicPowerSkill_Branding =>
            magicPowerSkill_Branding ??= new List<MagicPowerSkill>
            {
                new("TM_Branding_pwr", "TM_Branding_pwr_desc"), // severity, 5 ranks
                new("TM_Branding_eff", "TM_Branding_eff_desc") // reduces upkeep cost, 5 ranks
            };

        public List<MagicPowerSkill> MagicPowerSkill_SigilSurge =>
            magicPowerSkill_SigilSurge ??= new List<MagicPowerSkill>
            {
                new("TM_SigilSurge_pwr", "TM_SigilSurge_pwr_desc"), // severity boost when active
                new("TM_SigilSurge_eff", "TM_SigilSurge_eff_desc"), // reduces mana upkeep while active
                new("TM_SigilSurge_ver", "TM_SigilSurge_ver_desc") // reduces 'feedback' effects on golemancer (pain)
            };

        public List<MagicPowerSkill> MagicPowerSkill_SigilDrain =>
            magicPowerSkill_SigilDrain ??= new List<MagicPowerSkill>
            {
                new("TM_SigilDrain_pwr", "TM_SigilDrain_pwr_desc"), // bonus of drain effects on golemancer
                new("TM_SigilDrain_eff", "TM_SigilDrain_eff_desc"), // reduces mana upkeep cost
                new("TM_SigilDrain_ver", "TM_SigilDrain_ver_desc") // reduces feedback effects on other pawn (pain)
            };

        public List<MagicPowerSkill> MagicPowerSkill_LivingWall =>
            magicPowerSkill_LivingWall ??= new List<MagicPowerSkill>
            {
                new("TM_LivingWall_pwr", "TM_LivingWall_pwr_desc"), // power of living walls abilities
                new("TM_LivingWall_eff", "TM_LivingWall_eff_desc"), // mana upkeep cost
                new("TM_LivingWall_ver", "TM_LivingWall_ver_desc") // movement and action quickness
            };

        public int MagicUserLevel
        {
            get => magicUserLevel;
            set => magicUserLevel = value;
        }

        public int MagicUserXP
        {
            get => magicUserXP;
            set => magicUserXP = value;
        }

        public int MagicAbilityPoints
        {
            get => magicAbilityPoints;
            set => magicAbilityPoints = value;
        }

        public MagicPowerSkill GetMagicPowerSkillWithSuffix(TMAbilityDef ability, Dictionary<TMAbilityDef, MagicPowerSkill> cache, string suffix)
        {
            if (cache.ContainsKey(ability)) return cache[ability];

            string defName = ability.defName;
            char[] trim = { '_', 'I', 'V', 'X' };
            defName = defName.TrimEnd(trim) + suffix;
            for (int i = 0; i < AllMagicPowerSkills.Count; i++)
            {
                MagicPowerSkill mps = AllMagicPowerSkills[i];
                if (!mps.label.Contains(defName)) continue;

                cache.Add(ability, mps);
                return mps;
            }
            foreach (TM_CustomPowerDef powerDef in TM_Data.CustomMagePowerDefs())
            {
                for (int j = 0; j < powerDef.customPower.abilityDefs.Count; j++)
                {
                    if (ability.defName != powerDef.customPower.abilityDefs[j].ToString()) continue;

                    for (int k = 0; k < AllMagicPowerSkills.Count; k++)
                    {
                        MagicPowerSkill mps = AllMagicPowerSkills[k];
                        foreach (TM_CustomSkill cs in powerDef.customPower.skills)
                        {
                            if (!cs.label.EndsWith(suffix) || cs.label != mps.label) continue;

                            cache.Add(ability, mps);
                            return mps;
                        }
                    }
                }
            }
            cache.Add(ability, null);
            return null;
        }

        private Dictionary<TMAbilityDef, MagicPowerSkill> skillEfficiency = new();
        public MagicPowerSkill GetSkill_Efficiency(TMAbilityDef ability)
        {
            return GetMagicPowerSkillWithSuffix(ability, skillEfficiency, "_eff");
        }

        private Dictionary<TMAbilityDef, MagicPowerSkill> skillVersatility = new();
        public MagicPowerSkill GetSkill_Versatility(TMAbilityDef ability)
        {
            return GetMagicPowerSkillWithSuffix(ability, skillVersatility, "_ver");
        }

        private Dictionary<TMAbilityDef, MagicPowerSkill> skillPower = new();
        public MagicPowerSkill GetSkill_Power(TMAbilityDef ability)
        {
            return GetMagicPowerSkillWithSuffix(ability, skillPower, "_pwr");
        }

        private Dictionary<HediffDef, TMAbilityDef> hediffAbility = new();
        public TMAbilityDef GetHediffAbility(Hediff hd)
        {
            if (hediffAbility.ContainsKey(hd.def)) return hediffAbility[hd.def];

            for (int i = AllMagicPowers.Count - 1; i >= 0; i--)
            {
                if (AllMagicPowers[i].abilityDef is not TMAbilityDef ability) continue;
                if (ability.abilityHediff == null || ability.abilityHediff != hd.def) continue;

                hediffAbility.Add(hd.def, ability);
                return ability;
            }
            hediffAbility.Add(hd.def, null);
            return null;
        }

        private int uniquePowersCount;
        public int GetUniquePowersWithSkillsCount(List<TM_CustomClass> customClassList)
        {
            List<TMAbilityDef> abilities = new();
            foreach (TM_CustomClass customClass in customClassList)
            {
                for (int i = 0; i < customClass.classMageAbilities.Count; i++)
                {
                    bool unique = true;
                    for (int j = 0; j < abilities.Count; j++)
                    {
                        if (customClass.classMageAbilities[i].defName.Contains(abilities[j].defName))
                        {
                            unique = false;
                        }
                    }
                    if (unique)
                    {
                        abilities.Add(customClass.classMageAbilities[i]);
                    }
                }
            }
            uniquePowersCount = abilities.Count;
            return uniquePowersCount;
        }

        public MagicPower ReturnMatchingMagicPower(TMAbilityDef def)
        {
            for (int i = 0; i < AllMagicPowers.Count; i++)
            {
                if (!AllMagicPowers[i].TMabilityDefs.Contains(def)) continue;

                MagicPower soulBond = MagicPowersWD.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_SoulBond);
                MagicPower shadowBolt = MagicPowersWD.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_ShadowBolt);
                MagicPower dominate = MagicPowersWD.First(static mp => mp.abilityDef == TorannMagicDefOf.TM_Dominate);
                if (AllMagicPowers[i] != soulBond && AllMagicPowers[i] != shadowBolt && AllMagicPowers[i] != dominate)
                    return AllMagicPowers[i];
            }
            return null;
        }

        List<MagicPower> allMagicPowersForChaosMageList = new();
        public List<MagicPower> AllMagicPowersForChaosMage
        {
            get
            {
                if (allMagicPowersForChaosMageList is { Count: > 0 }) return allMagicPowersForChaosMageList;

                List<MagicPower> tmpList = new List<MagicPower>();
                allMagicPowersForChaosMageList = new List<MagicPower>();
                allMagicPowersForChaosMageList.Clear();
                allMagicPowersForChaosMageList.AddRange(MagicPowersW);
                tmpList.Add(MagicPowersIF.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Firestorm));
                allMagicPowersForChaosMageList.AddRange(MagicPowersIF.Except(tmpList));
                tmpList.Clear();
                tmpList.Add(MagicPowersHoF.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Rainmaker));
                tmpList.Add(MagicPowersHoF.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Blizzard));
                allMagicPowersForChaosMageList.AddRange(MagicPowersHoF.Except(tmpList));
                tmpList.Clear();
                tmpList.Add(MagicPowersSB.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_EyeOfTheStorm));
                allMagicPowersForChaosMageList.AddRange(MagicPowersSB.Except(tmpList));
                tmpList.Clear();
                tmpList.Add(MagicPowersA.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_FoldReality));
                allMagicPowersForChaosMageList.AddRange(MagicPowersA.Except(tmpList));
                tmpList.Clear();
                tmpList.Add(MagicPowersP.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_HolyWrath));
                allMagicPowersForChaosMageList.AddRange(MagicPowersP.Except(tmpList));
                tmpList.Clear();
                allMagicPowersForChaosMageList.AddRange(MagicPowersS.Except(MagicPowersS.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SummonPoppi)));
                tmpList.Add(MagicPowersD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_RegrowLimb));
                allMagicPowersForChaosMageList.AddRange(MagicPowersD.Except(tmpList));
                tmpList.Clear();
                tmpList.Add(MagicPowersN.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_RaiseUndead));
                tmpList.Add(MagicPowersN.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_LichForm));
                tmpList.Add(MagicPowersN.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_DeathBolt));
                tmpList.Add(MagicPowersN.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_DeathBolt_I));
                tmpList.Add(MagicPowersN.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_DeathBolt_II));
                tmpList.Add(MagicPowersN.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_DeathBolt_III));
                allMagicPowersForChaosMageList.AddRange(MagicPowersN.Except(tmpList));
                tmpList.Clear();
                allMagicPowersForChaosMageList.AddRange(MagicPowersPR.Except(MagicPowersPR.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Resurrection)));
                tmpList.Add(MagicPowersB.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_BardTraining));
                tmpList.Add(MagicPowersB.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Inspire));
                tmpList.Add(MagicPowersB.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Entertain));
                tmpList.Add(MagicPowersB.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_BattleHymn));
                allMagicPowersForChaosMageList.AddRange(MagicPowersB.Except(tmpList));
                tmpList.Clear();
                allMagicPowersForChaosMageList.AddRange(MagicPowersWD.Except(MagicPowersWD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_PsychicShock)));
                tmpList.Add(MagicPowersSD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SoulBond));
                tmpList.Add(MagicPowersSD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ShadowBolt));
                tmpList.Add(MagicPowersSD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Dominate));
                tmpList.Add(MagicPowersSD.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Scorn));
                allMagicPowersForChaosMageList.AddRange(MagicPowersSD.Except(tmpList));
                tmpList.Clear();
                allMagicPowersForChaosMageList.AddRange(MagicPowersG.Except(MagicPowersG.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Meteor)));
                tmpList.Add(MagicPowersT.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_OrbitalStrike));
                tmpList.Add(MagicPowersT.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_OrbitalStrike_I));
                tmpList.Add(MagicPowersT.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_OrbitalStrike_II));
                tmpList.Add(MagicPowersT.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_OrbitalStrike_III));
                allMagicPowersForChaosMageList.AddRange(MagicPowersT.Except(tmpList));
                tmpList.Clear();
                allMagicPowersForChaosMageList.AddRange(MagicPowersE.Except(MagicPowersE.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Shapeshift)));
                allMagicPowersForChaosMageList.AddRange(MagicPowersC.Except(MagicPowersC.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Recall)));
                allMagicPowersForChaosMageList.Add((MagicPowersShadow.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_ShadowWalk)));
                allMagicPowersForChaosMageList.AddRange(MagicPowersBrightmage.Except(MagicPowersBrightmage.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_SpiritOfLight)));
                tmpList.Add(MagicPowersShaman.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_GuardianSpirit));
                //tmpList.Add(MagicPowersShaman.FirstOrDefault((MagicPower x) => x.abilityDef == TorannMagicDefOf.TM_Totems));
                allMagicPowersForChaosMageList.AddRange(MagicPowersShaman.Except(tmpList));
                tmpList.Clear();
                return allMagicPowersForChaosMageList;
            }
        }

        List<MagicPower> allMagicPowersList = new();
        public List<MagicPower> AllMagicPowers
        {
            get
            {
                if (allMagicPowersList is { Count: > 0 }) return allMagicPowersList;

                allMagicPowersList = new List<MagicPower>();
                allMagicPowersList.AddRange(AllMagicPowersWithSkills);
                allMagicPowersList.AddRange(MagicPowersStandalone);
                allMagicPowersList.AddRange(MagicPowersCustomStandalone);
                return allMagicPowersList;
            }
        }

        List<MagicPower> allMagicPowersWithSkillsList = new();
        public List<MagicPower> AllMagicPowersWithSkills
        {
            get
            {
                if (allMagicPowersWithSkillsList is { Count: > 0 }) return allMagicPowersWithSkillsList;

                allMagicPowersWithSkillsList = new List<MagicPower>();
                allMagicPowersWithSkillsList.AddRange(MagicPowersCustom);
                allMagicPowersWithSkillsList.AddRange(MagicPowersCM);
                allMagicPowersWithSkillsList.AddRange(MagicPowersW);
                allMagicPowersWithSkillsList.AddRange(MagicPowersC);
                allMagicPowersWithSkillsList.AddRange(MagicPowersE);
                allMagicPowersWithSkillsList.AddRange(MagicPowersBM);
                allMagicPowersWithSkillsList.AddRange(MagicPowersIF);
                allMagicPowersWithSkillsList.AddRange(MagicPowersHoF);
                allMagicPowersWithSkillsList.AddRange(MagicPowersSB);
                allMagicPowersWithSkillsList.AddRange(MagicPowersA);
                allMagicPowersWithSkillsList.AddRange(MagicPowersP);
                allMagicPowersWithSkillsList.AddRange(MagicPowersPR);
                allMagicPowersWithSkillsList.AddRange(MagicPowersS);
                allMagicPowersWithSkillsList.AddRange(MagicPowersD);
                allMagicPowersWithSkillsList.AddRange(MagicPowersN);
                allMagicPowersWithSkillsList.AddRange(MagicPowersB);
                allMagicPowersWithSkillsList.AddRange(MagicPowersSD);
                allMagicPowersWithSkillsList.AddRange(MagicPowersWD);
                allMagicPowersWithSkillsList.AddRange(MagicPowersG);
                allMagicPowersWithSkillsList.AddRange(MagicPowersT);
                allMagicPowersWithSkillsList.AddRange(MagicPowersShadow);
                allMagicPowersWithSkillsList.AddRange(MagicPowersBrightmage);
                allMagicPowersWithSkillsList.AddRange(MagicPowersShaman);
                allMagicPowersWithSkillsList.AddRange(MagicPowersGolemancer);
                return allMagicPowersWithSkillsList;
            }
        }

        List<MagicPowerSkill> allMagicPowerSkillsList = new();
        public List<MagicPowerSkill> AllMagicPowerSkills
        {
            get
            {
                if (allMagicPowerSkillsList is { Count: > 0 }) return allMagicPowerSkillsList;

                allMagicPowerSkillsList = new List<MagicPowerSkill>();
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_AccelerateTime);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_AdvancedHeal);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_AlterFate);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_AMP);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Attraction);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_BardTraining);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_BattleHymn);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_BestowMight);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Blink);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Blizzard);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_BloodForBlood);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_BloodGift);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_BloodMoon);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_BloodShield);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Cantrips);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_ChaosTradition);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_ChronostaticField);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_ConsumeCorpse);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_CorpseExplosion);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_CureDisease);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_DeathBolt);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_DeathMark);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Dominate);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_EarthernHammer);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_EarthSprites);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Encase);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_EnchantedBody);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_EnchanterStone);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_EnchantWeapon);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Entertain);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_EyeOfTheStorm);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Fireball);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Firebolt);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Fireclaw);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Firestorm);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_FogOfTorment);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_FoldReality);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_FrostRay);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_global_eff);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_global_regen);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_global_spirit);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Heal);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_HealingCircle);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_HolyWrath);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Icebolt);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_IgniteBlood);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Inspire);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_LichForm);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_LightningBolt);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_LightningCloud);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_LightningStorm);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Lullaby);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_MagicMissile);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Meteor);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_OrbitalStrike);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Overdrive);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Overwhelm);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Poison);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Polymorph);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Prediction);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_PsychicShock);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Purify);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Rainmaker);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_RaiseUndead);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_RayofHope);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_P_RayofHope);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Recall);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Regenerate);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_RegrowLimb);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Rend);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Repulsion);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Resurrection);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_ReverseTime);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Sabotage);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Scorn);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Sentinel);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Shadow);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_ShadowBolt);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Shapeshift);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Shield);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Snowball);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Soothe);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SootheAnimal);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SoulBond);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Stoneskin);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Summon);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SummonElemental);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SummonExplosive);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SummonMinion);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SummonPoppi);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SummonPylon);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_TechnoBit);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_TechnoShield);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_TechnoTurret);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_TechnoWeapon);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Teleport);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Transmutate);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_ValiantCharge);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_WandererCraft);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_ShadowWalk);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_LightLance);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Sunfire);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_LightBurst);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_LightSkip);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Refraction);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SpiritOfLight);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Totems);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_ChainLightning);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Enrage);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Hex);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SpiritWolves);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_GuardianSpirit);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Golemancy);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_RuneCarving);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Branding);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SigilSurge);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_SigilDrain);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_LivingWall);
                allMagicPowerSkillsList.AddRange(MagicPowerSkill_Custom);
                return allMagicPowerSkillsList;
            }
        }

        public void ResetAllSkills()
        {
            List<MagicPowerSkill> mps = AllMagicPowerSkills;
            for(int i = 0; i < mps.Count; i++)
            {
                mps[i].level = 0;
            }
        }

        public MagicData()
        {
        }

        public MagicData(CompAbilityUserMagic newUser)
        {
            magicPawn = newUser.Pawn;
        }

        public void ExposeData()
        {
            Scribe_References.Look<Pawn>(ref magicPawn, "magicPawn");
            Scribe_Values.Look<int>(ref magicUserLevel, "magicUserLevel");
            Scribe_Values.Look<int>(ref magicUserXP, "magicUserXP");
            Scribe_Values.Look<bool>(ref initialized, "initialized");
            Scribe_Values.Look<int>(ref magicAbilityPoints, "magicAbilityPoints");
            Scribe_Values.Look<int>(ref ticksToLearnMagicXP, "ticksToLearnMagicXP", -1);
            Scribe_Values.Look<int>(ref ticksAffiliation, "ticksAffiliation", -1);
            Scribe_Values.Look<int>(ref dominationCount, "dominationCount");
            Scribe_Values.Look<bool>(ref isNecromancer, "isNecromancer");
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_global_eff, "magicPowerSkill_global_eff", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_global_regen, "magicPowerSkill_global_regen", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_global_spirit, "magicPowerSkill_global_spirit", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerStandalone, "magicPowerStandalone", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerCustom, "magicPowerCustom", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerCustomStandalone, "magicPowerCustomStandalone", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Custom, "magicPowerSkill_Custom", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerW, "magicPowerW", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_WandererCraft, "magicPowerSkill_WandererCraft", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Cantrips, "magicPowerSkill_Cantrips", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerIF, "magicPowerIF", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_RayofHope, "magicPowerSkill_RayofHope", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Fireball, "magicPowerSkill_Fireball", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Firebolt, "magicPowerSkill_Firebolt", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Fireclaw, "magicPowerSkill_Fireclaw", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Firestorm, "magicPowerSkill_Firestorm", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerHoF, "magicPowerHoF", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Soothe, "magicPowerSkill_Soothe", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Icebolt, "magicPowerSkill_Icebolt", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_FrostRay, "magicPowerSkill_FrostRay", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Snowball, "magicPowerSkill_Snowball", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Rainmaker, "magicPowerSkill_Rainmaker", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Blizzard, "magicPowerSkill_Blizzard", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerSB, "magicPowerSB", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_AMP, "magicPowerSkill_AMP", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_LightningBolt, "magicPowerSkill_LightningBolt", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_LightningCloud, "magicPowerSkill_LightningCloud", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_LightningStorm, "magicPowerSkill_LightningStorm", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_EyeOfTheStorm, "magicPowerSkill_EyeOfTheStorm", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerA, "magicPowerA", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Shadow, "magicPowerSkill_Shadow", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_MagicMissile, "magicPowerSkill_MagicMissile", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Blink, "magicPowerSkill_Blink", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Summon, "magicPowerSkill_Summon", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Teleport, "magicPowerSkill_Teleport", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_FoldReality, "magicPowerSkill_FoldReality", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerP, "magicPowerP", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_P_RayofHope, "magicPowerSkill_P_RayofHope", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Heal, "magicPowerSkill_Heal", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Shield, "magicPowerSkill_Shield", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_ValiantCharge, "magicPowerSkill_ValiantCharge", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Overwhelm, "magicPowerSkill_Overwhelm", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_HolyWrath, "magicPowerSkill_HolyWrath", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerS, "magicPowerS", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SummonMinion, "magicPowerSkill_SummonMinion", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SummonPylon, "magicPowerSkill_SummonPylon", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SummonExplosive, "magicPowerSkill_SummonExplosive", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SummonElemental, "magicPowerSkill_SummonElemental", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SummonPoppi, "magicPowerSkill_SummonPoppi", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerD, "magicPowerD", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Poison, "magicPowerSkill_Poison", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SootheAnimal, "magicPowerSkill_SootheAnimal", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Regenerate, "magicPowerSkill_Regenerate", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_CureDisease, "magicPowerSkill_CureDisease", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_RegrowLimb, "magicPowerSkill_RegrowLimb", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerN, "magicPowerN", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_RaiseUndead, "magicPowerSkill_RaiseUndead", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_DeathMark, "magicPowerSkill_DeathMark", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_FogOfTorment, "magicPowerSkill_FogOfTorment", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_ConsumeCorpse, "magicPowerSkill_ConsumeCorpse", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_CorpseExplosion, "magicPowerSkill_CorpseExplosion", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_LichForm, "magicPowerSkill_LichForm", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_DeathBolt, "magicPowerSkill_DeathBolt", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerPR, "magicPowerPR", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_AdvancedHeal, "magicPowerSkill_AdvancedHeal", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Purify, "magicPowerSkill_Purify", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_HealingCircle, "magicPowerSkill_HealingCircle", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_BestowMight, "magicPowerSkill_BestowMight", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Resurrection, "magicPowerSkill_Resurrection", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerB, "magicPowerB", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_BardTraining, "magicPowerSkill_BardTraining", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Entertain, "magicPowerSkill_Entertain", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Inspire, "magicPowerSkill_Inspire", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Lullaby, "magicPowerSkill_Lullaby", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_BattleHymn, "magicPowerSkill_BattleHymn", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerWD, "magicPowerWD", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerSD, "magicPowerSD", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SoulBond, "magicPowerSkill_SoulBond", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_ShadowBolt, "magicPowerSkill_ShadowBolt", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Dominate, "magicPowerSkill_Dominate", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Repulsion, "magicPowerSkill_Repulsion", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Attraction, "magicPowerSkill_Attraction", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Scorn, "magicPowerSkill_Scorn", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_PsychicShock, "magicPowerSkill_PsychicShock", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerG, "magicPowerG", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Stoneskin, "magicPowerSkill_Stoneskin", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Encase, "magicPowerSkill_Encase", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_EarthSprites, "magicPowerSkill_EarthSprites", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_EarthernHammer, "magicPowerSkill_EarthernHammer", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Sentinel, "magicPowerSkill_Sentinel", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Meteor, "magicPowerSkill_Meteor", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerT, "magicPowerT", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_TechnoBit, "magicPowerSkill_TechnoBit", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_TechnoTurret, "magicPowerSkill_TechnoTurret", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_TechnoWeapon, "magicPowerSkill_TechnoWeapon", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_TechnoShield, "magicPowerSkill_TechnoShield", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Sabotage, "magicPowerSkill_Sabotage", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Overdrive, "magicPowerSkill_Overdrive", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_OrbitalStrike, "magicPowerSkill_OrbitalStrike", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerBM, "magicPowerBM", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_BloodGift, "magicPowerSkill_BloodGift", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_IgniteBlood, "magicPowerSkill_IgniteBlood", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_BloodForBlood, "magicPowerSkill_BloodForBlood", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_BloodShield, "magicPowerSkill_BloodShield", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Rend, "magicPowerSkill_Rend", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_BloodMoon, "magicPowerSkill_BloodMoon", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerE, "magicPowerE", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_EnchantedBody, "magicPowerSkill_EnchantedBody", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Transmutate, "magicPowerSkill_Transmutate", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_EnchanterStone, "magicPowerSkill_EnchanterStone", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_EnchantWeapon, "magicPowerSkill_EnchantWeapon", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Polymorph, "magicPowerSkill_Polymorph", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Shapeshift, "magicPowerSkill_Shapeshift", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerC, "magicPowerC", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Prediction, "magicPowerSkill_Prediction", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_AlterFate, "magicPowerSkill_AlterFate", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_AccelerateTime, "magicPowerSkill_AccelerateTime", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_ReverseTime, "magicPowerSkill_ReverseTime", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_ChronostaticField, "magicPowerSkill_ChronostaticField", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Recall, "magicPowerSkill_Recall", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerCM, "magicPowerCM", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_ChaosTradition, "magicPowerSkill_ChaosTradition", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerShadow, "magicPowerShadow", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_ShadowWalk, "magicPowerSkill_ShadowWalk", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerBrightmage, "magicPowerBrightmage", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_LightLance, "magicPowerSkill_LightLance", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Sunfire, "magicPowerSkill_Sunfire", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_LightBurst, "magicPowerSkill_LightBurst", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_LightSkip, "magicPowerSkill_LightSkip", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Refraction, "magicPowerSkill_Refraction", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SpiritOfLight, "magicPowerSkill_SpiritOfLight", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerShaman, "magicPowerShaman", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Totems, "magicPowerSkill_Totems", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_ChainLightning, "magicPowerSkill_ChainLightning", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Enrage, "magicPowerSkill_Enrage", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Hex, "magicPowerSkill_Hex", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SpiritWolves, "magicPowerSkill_SpiritWolves", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_GuardianSpirit, "magicPowerSkill_GuardianSpirit", LookMode.Deep);
            Scribe_Collections.Look<MagicPower>(ref magicPowerGolemancer, "magicPowerGolemancer", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Golemancy, "magicPowerSkill_Golemancy", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_RuneCarving, "magicPowerSkill_RuneCarving", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_Branding, "magicPowerSkill_Branding", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SigilSurge, "magicPowerSkill_SigilSurge", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_SigilDrain, "magicPowerSkill_SigilDrain", LookMode.Deep);
            Scribe_Collections.Look<MagicPowerSkill>(ref magicPowerSkill_LivingWall, "magicPowerSkill_LivingWall", LookMode.Deep);
        }
    }
}
