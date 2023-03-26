using UnityEngine;
using Verse;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using RimWorld;
using System;
using TorannMagic.ModOptions;

namespace TorannMagic
{
    public static class TM_Data
    {
        public static List<ThingDef> SpellList()
        {
            IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                               where (def.defName.Contains("SpellOf_"))
                                               select def;
            return enumerable.ToList();
        }

        public static List<ThingDef> MasterSpellList()
        {
            List<ThingDef> masterSpellList = new List<ThingDef>();
            masterSpellList.Add(TorannMagicDefOf.SpellOf_Firestorm);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_Blizzard);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_EyeOfTheStorm);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_RegrowLimb);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_FoldReality);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_Resurrection);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_HolyWrath);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_LichForm);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_SummonPoppi);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_BattleHymn);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_PsychicShock);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_Scorn);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_Meteor);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_OrbitalStrike);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_BloodMoon);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_Shapeshift);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_Recall);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_SpiritOfLight);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_GuardianSpirit);
            masterSpellList.Add(TorannMagicDefOf.SpellOf_LivingWall);
            return masterSpellList;
        }

        public static List<ThingDef> RestrictedAbilities
        {
            get
            {
                List<ThingDef> restricted = new List<ThingDef>();
                restricted.Add(TorannMagicDefOf.SpellOf_BattleHymn);
                restricted.Add(TorannMagicDefOf.SpellOf_BlankMind);
                restricted.Add(TorannMagicDefOf.SpellOf_Blizzard);
                restricted.Add(TorannMagicDefOf.SpellOf_BloodMoon);
                restricted.Add(TorannMagicDefOf.SpellOf_BriarPatch);
                restricted.Add(TorannMagicDefOf.SpellOf_CauterizeWound);
                restricted.Add(TorannMagicDefOf.SpellOf_ChargeBattery);
                restricted.Add(TorannMagicDefOf.SpellOf_DryGround);
                restricted.Add(TorannMagicDefOf.SpellOf_EyeOfTheStorm);
                restricted.Add(TorannMagicDefOf.SpellOf_FertileLands);
                restricted.Add(TorannMagicDefOf.SpellOf_Firestorm);
                restricted.Add(TorannMagicDefOf.SpellOf_FoldReality);
                restricted.Add(TorannMagicDefOf.SpellOf_HeatShield);
                restricted.Add(TorannMagicDefOf.SpellOf_HolyWrath);
                restricted.Add(TorannMagicDefOf.SpellOf_LichForm);
                restricted.Add(TorannMagicDefOf.SpellOf_MechaniteReprogramming);
                restricted.Add(TorannMagicDefOf.SpellOf_Meteor);
                restricted.Add(TorannMagicDefOf.SpellOf_OrbitalStrike);
                restricted.Add(TorannMagicDefOf.SpellOf_Overdrive);
                restricted.Add(TorannMagicDefOf.SpellOf_PsychicShock);
                restricted.Add(TorannMagicDefOf.SpellOf_Recall);
                restricted.Add(TorannMagicDefOf.SpellOf_RegrowLimb);
                restricted.Add(TorannMagicDefOf.SpellOf_Resurrection);
                restricted.Add(TorannMagicDefOf.SpellOf_Sabotage);
                restricted.Add(TorannMagicDefOf.SpellOf_Scorn);
                restricted.Add(TorannMagicDefOf.SpellOf_Shapeshift);
                restricted.Add(TorannMagicDefOf.SpellOf_SummonPoppi);
                restricted.Add(TorannMagicDefOf.SpellOf_TechnoShield);
                restricted.Add(TorannMagicDefOf.SpellOf_WetGround);
                restricted.Add(TorannMagicDefOf.SpellOf_SpiritOfLight);
                restricted.Add(TorannMagicDefOf.SpellOf_GuardianSpirit);
                restricted.Add(TorannMagicDefOf.SpellOf_Discord);
                restricted.Add(TorannMagicDefOf.SpellOf_ShieldOther);
                restricted.AddRange(RestrictedAbilitiesXML);
                return restricted;
            }
        }

        public static List<ThingDef> RestrictedAbilitiesXML
        {
            get
            {
                IEnumerable<TMAbilityDef> enumerable = from def in DefDatabase<TMAbilityDef>.AllDefs
                                                   where (def.restrictedAbility)
                                                   select def;
                List<ThingDef> xmlRestrictedAbilities = new List<ThingDef>();
                xmlRestrictedAbilities.Clear();
                foreach(TMAbilityDef d in enumerable)
                {
                    if(d.restrictedAbility && d.learnItem != null)
                    {
                        xmlRestrictedAbilities.Add(d.learnItem);
                    }
                }
                return xmlRestrictedAbilities.ToList();
            }
        }

        public static List<ThingDef> StandardSpellList()
        {
            return SpellList().Except(MasterSpellList()).ToList();
        }

        public static List<ThingDef> StandardSkillList()
        {
            IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                               where (def.defName.Contains("SkillOf_"))
                                               select def;
            return enumerable.ToList();
        }

        public static List<ThingDef> FighterBookList()
        {
            List<ThingDef> fighterBookList = new List<ThingDef>();
            fighterBookList.Add(TorannMagicDefOf.BookOfGladiator);
            fighterBookList.Add(TorannMagicDefOf.BookOfBladedancer);
            fighterBookList.Add(TorannMagicDefOf.BookOfDeathKnight);
            fighterBookList.Add(TorannMagicDefOf.BookOfFaceless);
            fighterBookList.Add(TorannMagicDefOf.BookOfPsionic);
            fighterBookList.Add(TorannMagicDefOf.BookOfRanger);
            fighterBookList.Add(TorannMagicDefOf.BookOfSniper);
            fighterBookList.Add(TorannMagicDefOf.BookOfMonk);
            fighterBookList.Add(TorannMagicDefOf.BookOfCommander);
            fighterBookList.Add(TorannMagicDefOf.BookOfSuperSoldier);
            foreach(TMDefs.TM_CustomClass cc in TM_ClassUtility.CustomClasses)
            {
                if (cc.isFighter && cc.fullScript != null)
                {
                    fighterBookList.Add(cc.fullScript);
                }
            }
            return fighterBookList;
        }

        public static List<ThingDef> MageBookList()
        {
            IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                               where (def.defName.Contains("BookOf"))
                                               select def;

            enumerable = enumerable.Except(MageTornScriptList());
            return enumerable.Except(FighterBookList()).ToList();
        }

        public static List<ThingDef> AllBooksList()
        {
            IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                               where (def.defName.Contains("BookOf"))
                                               select def;

            return enumerable.Except(MageTornScriptList()).ToList();
        }

        public static List<ThingDef> MageTornScriptList()
        {
            IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                               where (def.defName.Contains("Torn_BookOf"))
                                               select def;

            return enumerable.ToList();
        }

        public static List<TraitDef> EnabledMagicTraits
        // DEPRECATED. Use TM_ClassUtility.EnabledMagicTraits instead. This is just here to prevent save breaking for ArcanePathway.
        {
            get
            {
                List<TraitDef> magicTraits = new List<TraitDef>();

                if (Settings.Arcanist.isEnabled) { magicTraits.Add(TorannMagicDefOf.Arcanist); }
                if (Settings.FireMage.isEnabled) { magicTraits.Add(TorannMagicDefOf.InnerFire); }
                if (Settings.IceMage.isEnabled) { magicTraits.Add(TorannMagicDefOf.HeartOfFrost); }
                if (Settings.LitMage.isEnabled) { magicTraits.Add(TorannMagicDefOf.StormBorn); }
                if (Settings.Druid.isEnabled) { magicTraits.Add(TorannMagicDefOf.Druid); }
                if (Settings.Priest.isEnabled) { magicTraits.Add(TorannMagicDefOf.Priest); }
                if (Settings.Necromancer.isEnabled) { magicTraits.Add(TorannMagicDefOf.Necromancer); }
                if (Settings.Technomancer.isEnabled) { magicTraits.Add(TorannMagicDefOf.Technomancer); }
                if (Settings.Geomancer.isEnabled) { magicTraits.Add(TorannMagicDefOf.Geomancer); }
                if (Settings.Demonkin.isEnabled) { magicTraits.Add(TorannMagicDefOf.Warlock); }
                if (Settings.Demonkin.isEnabled) { magicTraits.Add(TorannMagicDefOf.Succubus); }
                if (Settings.ChaosMage.isEnabled) { magicTraits.Add(TorannMagicDefOf.ChaosMage); }
                if (Settings.Paladin.isEnabled) { magicTraits.Add(TorannMagicDefOf.Paladin); }
                if (Settings.Summoner.isEnabled) { magicTraits.Add(TorannMagicDefOf.Summoner); }
                if (Settings.Bard.isEnabled) { magicTraits.Add(TorannMagicDefOf.TM_Bard); }
                if (Settings.Chronomancer.isEnabled) { magicTraits.Add(TorannMagicDefOf.Chronomancer); }
                if (Settings.Enchanter.isEnabled) { magicTraits.Add(TorannMagicDefOf.Enchanter); }
                if (Settings.BloodMage.isEnabled) { magicTraits.Add(TorannMagicDefOf.BloodMage); }
                if (Settings.Brightmage.isEnabled) { magicTraits.Add(TorannMagicDefOf.TM_Brightmage); }
                if (Settings.Shaman.isEnabled) { magicTraits.Add(TorannMagicDefOf.TM_Shaman); }
                if (Settings.Golemancer.isEnabled) { magicTraits.Add(TorannMagicDefOf.TM_Golemancer); }
                foreach (TMDefs.TM_CustomClass cc in TM_ClassUtility.CustomClasses)
                {
                    if (cc.isMage && !magicTraits.Contains(cc.classTrait) && Settings.Instance.CustomClass[cc.classTrait.ToString()])
                    {
                        magicTraits.Add(cc.classTrait);
                    }
                }
                return magicTraits;
            }
        }

        public static List<TMAbilityDef> BrandList()
        {
            return new List<TMAbilityDef>
            {
                TorannMagicDefOf.TM_AwarenessBrand,
                TorannMagicDefOf.TM_EmotionBrand,
                TorannMagicDefOf.TM_FitnessBrand,
                TorannMagicDefOf.TM_ProtectionBrand,
                TorannMagicDefOf.TM_SiphonBrand,
                TorannMagicDefOf.TM_VitalityBrand
            };
        }

        private static HashSet<ThingDef> magicFociSet;
        public static HashSet<ThingDef> MagicFociList()
        {
            return magicFociSet ??= DefDatabase<ThingDef>.AllDefs.Where(static def =>
                WeaponCategoryList.Named("TM_Category_MagicalFoci").weaponDefNames.Contains(def.defName)).ToHashSet();
        }

        public static List<string> CustomWeaponCategoryList(string listDefName)
        {
            List<string> customWeaponDefNames = new List<string>();
            foreach(WeaponCategoryList wcl in DefDatabase<WeaponCategoryList>.AllDefs.Where(list => list.defName == listDefName))
            {
                customWeaponDefNames.AddRange(wcl.weaponDefNames);
            }
            return customWeaponDefNames;
        }

        private static HashSet<ThingDef> bowSet;
        public static HashSet<ThingDef> BowSet()
        {
            return bowSet ??= DefDatabase<ThingDef>.AllDefs.Where(static def =>
                WeaponCategoryList.Named("TM_Category_Bows").weaponDefNames.Contains(def.defName)).ToHashSet();
        }

        private static HashSet<ThingDef> pistolSet;
        public static HashSet<ThingDef> PistolSet()
        {
            return pistolSet ??= DefDatabase<ThingDef>.AllDefs.Where(static def =>
                WeaponCategoryList.Named("TM_Category_Pistols").weaponDefNames.Contains(def.defName)).ToHashSet();
        }

        private static HashSet<ThingDef> rifleSet;
        public static HashSet<ThingDef> RifleSet()
        {
            return rifleSet ??= DefDatabase<ThingDef>.AllDefs.Where(static def =>
                WeaponCategoryList.Named("TM_Category_Rifles").weaponDefNames.Contains(def.defName)).ToHashSet();
        }

        private static HashSet<ThingDef> shotgunSet;
        public static HashSet<ThingDef> ShotgunSet()
        {
            return shotgunSet ??= DefDatabase<ThingDef>.AllDefs.Where(static def =>
                WeaponCategoryList.Named("TM_Category_Shotguns").weaponDefNames.Contains(def.defName)).ToHashSet();
        }

        private static HashSet<HediffDef> ailmentSet;
        public static HashSet<HediffDef> AilmentSet()
        {
            return ailmentSet ??= DefDatabase<HediffDef>.AllDefs.Where(static def =>
                HediffCategoryList.Named("TM_Category_Hediffs").ailments.Any(hediff =>
                    hediff.hediffDefname == def.defName ||
                    hediff.containsDefnameString && def.defName.Contains(hediff.hediffDefname))
            ).ToHashSet();
        }

        private static HashSet<HediffDef> addictionSet;
        public static HashSet<HediffDef> AddictionSet()
        {
            return addictionSet ??= DefDatabase<HediffDef>.AllDefs.Where(static def =>
                HediffCategoryList.Named("TM_Category_Hediffs").addictions.Any(hediff =>
                    hediff.hediffDefname == def.defName ||
                    hediff.containsDefnameString && def.defName.Contains(hediff.hediffDefname))
                ).ToHashSet();
        }

        private static HashSet<HediffDef> mechaniteSet;
        public static HashSet<HediffDef> MechaniteSet()
        {
            return mechaniteSet ??= DefDatabase<HediffDef>.AllDefs.Where(static def =>
                HediffCategoryList.Named("TM_Category_Hediffs").mechanites.Any(hediff =>
                    hediff.hediffDefname == def.defName ||
                    hediff.containsDefnameString && def.defName.Contains(hediff.hediffDefname))
            ).ToHashSet();
        }

        private static HashSet<HediffDef> diseaseSet;
        public static HashSet<HediffDef> DiseaseSet()
        {
            return diseaseSet ??= DefDatabase<HediffDef>.AllDefs.Where(static def =>
                HediffCategoryList.Named("TM_Category_Hediffs").diseases.Any(hediff =>
                    hediff.hediffDefname == def.defName ||
                    hediff.containsDefnameString && def.defName.Contains(hediff.hediffDefname))
            ).ToHashSet();
        }

        private static TM_CustomPowerDef[] customFighterPowerDefs;
        public static TM_CustomPowerDef[] CustomFighterPowerDefs()
        {
            return customFighterPowerDefs ??= DefDatabase<TM_CustomPowerDef>.AllDefs.Where(static def =>
                def.customPower is { forFighter: true }).ToArray();
        }

        private static TM_CustomPowerDef[] customMagePowerDefs;
        public static IEnumerable<TM_CustomPowerDef> CustomMagePowerDefs()
        {
            return customMagePowerDefs ??= DefDatabase<TM_CustomPowerDef>.AllDefs.Where(static def =>
                def.customPower is { forMage: true }).ToArray();
        }

    }
}
