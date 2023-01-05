using System;
using System.Linq;
using AbilityUser;
using RimWorld;
using TorannMagic.Utils;
using UnityEngine;
using Verse;
using System.Collections.Generic;
using AbilityUserAI;
using HarmonyLib;
using TorannMagic.ModOptions;
using TorannMagic.TMDefs;

namespace TorannMagic
{
    public abstract class CompAbilityUserTMBase : CompAbilityUser
    {
        // TODO - This should be moved into customClass. This will make the checks clearer (customClass != null) and allow us to load in immediately instead of checking every time if set.
        public int customIndex = -2;

        /*
         * These Tick offsets are used so expensive calls aren't happening all on the same tick. PostSpawnSetup is used
         * as the trigger so we can guarantee ThingIdNumber has been set. They correspond to the TickModulo variables
         * found within TM_TickManager
         */
        protected int tickOffset6;
        protected int tickOffset20;
        protected int tickOffset30;
        protected int tickOffset60;
        protected int tickOffset67;
        protected int tickOffset300;
        protected int tickOffset600;

        private int deSpawnTick = -1;  // Keep track of when we DeSpawn for cooldowns

        // Cache variables
        public bool IsFaceless;  // There are a lot of exceptions that occur when a pawn has the faceless trait

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            tickOffset6 = Pawn.GetHashCode() % 6;
            tickOffset20 = Pawn.GetHashCode() % 20;
            tickOffset30 = Pawn.GetHashCode() % 30;
            tickOffset60 = Pawn.GetHashCode() % 60;
            tickOffset67 = Pawn.GetHashCode() % 67;
            tickOffset300 = Pawn.GetHashCode() % 300;
            tickOffset600 = Pawn.GetHashCode() % 600;

            // Handle cooldowns on spawn
            if (deSpawnTick != -1)
            {
                int ticksPassed = Find.TickManager.TicksGame - deSpawnTick;
                for (int i = 0; i < AbilityData.AllPowers.Count; i++)
                {
                    AbilityData.AllPowers[i].CooldownTicksLeft = Math.Max(
                        AbilityData.AllPowers[i].CooldownTicksLeft - ticksPassed, 0
                    );
                }
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            deSpawnTick = Find.TickManager.TicksGame;
        }

        public TM_CustomClass customClass = null;
        private List<TM_CustomClass> advClasses;
        public List<TM_CustomClass> AdvancedClasses
        {
            get => advClasses ?? (advClasses = TM_ClassUtility.GetAdvancedClassesForPawn(Pawn));
            set => advClasses = value;
        }

        protected int age = -1;
        public bool tickConditionsMet;  // Set in TM_PawnTracker to know if we should tick in CompTick or not

        protected int autocastTick = 0;
        protected int nextAICastAttemptTick = 0;

        public bool canDeathRetaliate = false;
        protected bool deathRetaliating = false;
        protected int ticksTillRetaliation = 600;

        public float arcalleumCooldown = 0f;
        public float arcaneRes = 1;
        public float coolDown = 1;
        public float xpGain = 1;

        public float weaponDamage = 1f;
        public float weaponCritChance;

        public List<TM_CustomClass> CombinedCustomClasses
        {
            get
            {
                List<TM_CustomClass> combinedCustomClasses = new List<TM_CustomClass>();
                combinedCustomClasses.AddRange(AdvancedClasses);
                if (customClass != null)
                {
                    combinedCustomClasses.Add(customClass);
                }
                return combinedCustomClasses;
            }
        }

        public List<TMAbilityDef> CombinedCustomAbilities
        {
            get
            {
                List<TMAbilityDef> combinedCustomAbilities = new List<TMAbilityDef>();
                if (customClass != null)
                {
                    combinedCustomAbilities.AddRange(customClass.classFighterAbilities);
                }
                if (AdvancedClasses.Count > 0)
                {
                    foreach (TM_CustomClass cc in AdvancedClasses)
                    {
                        combinedCustomAbilities.AddRange(cc.classFighterAbilities);
                    }
                }
                return combinedCustomAbilities;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref deSpawnTick, "deSpawnTick", -1);  // Currently don't save cooldowns, but this will be necessary if we decide to

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                IsFaceless = Pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless);
            }
        }

        public bool CustomClassHasAbility(TMAbilityDef ability)
        {
            return customClass != null && customClass.classAbilities.Contains(ability);
        }

        private static readonly SimpleCache<string, Material> traitCache = new SimpleCache<string, Material>(5);

        protected void DrawMark(Material material, Vector3 scale, float xOffset = 0, float yOffset = 0)
        {
            Vector3 vector = Pawn.Drawer.DrawPos;
            vector.x += .45f + xOffset;
            vector.z += .45f + yOffset;
            vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            const float angle = 0f;

            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), scale);

            Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
        }

        // Scan for a trait and draw mark if there is one that applies. If you know the trait, use the specific DrawMark above
        protected void DrawMark()
        {
            Material material = traitCache.GetOrCreate(Pawn.ThingID, () =>
            {
                for (int i = 0; i < Pawn.story.traits.allTraits.Count; i++)
                {
                    var iconInfo = TraitIconMap.TryGetValue(Pawn.story.traits.allTraits[i].def);
                    if (iconInfo != null) return iconInfo.IconMaterial;
                }
                return null;
            }, 5);

            if (material != null)
            {
                if (customClass != null)
                {
                    material.color = customClass.classIconColor;                    
                }
                DrawMark(material, new Vector3(.28f, 1f, .28f));
            }
        }

        public float GetSkillDamage(float strFactor)
        {
            if (Pawn.equipment?.Primary == null)
                return Pawn.GetStatValue(StatDefOf.MeleeDPS, false) * strFactor;

            if(Pawn.equipment.Primary.def.IsMeleeWeapon)
            {
                weaponCritChance = TM_Calc.GetWeaponCritChance(Pawn.equipment.Primary);
                return TM_Calc.GetSkillDamage_Melee(Pawn, strFactor);
            }
            weaponCritChance = 0f;
            return TM_Calc.GetSkillDamage_Range(Pawn, strFactor);
        }

        protected void handleAggressiveAICasting()
        {
            if (Pawn.IsColonist || !Settings.Instance.AICasting || !Settings.Instance.AIAggressiveCasting ||
                Find.TickManager.TicksGame <= nextAICastAttemptTick) return;

            nextAICastAttemptTick = Find.TickManager.TicksGame + Rand.Range(300, 500);
            if (Pawn.jobs == null || Pawn.CurJobDef == TorannMagicDefOf.TMCastAbilitySelf ||
                Pawn.CurJobDef == TorannMagicDefOf.TMCastAbilityVerb) return;

            IEnumerable<AbilityUserAIProfileDef> enumerable = Pawn.EligibleAIProfiles();
            foreach (AbilityUserAIProfileDef item in enumerable)
            {
                AbilityAIDef useThisAbility = item?.decisionTree?.RecursivelyGetAbility(Pawn);
                if (useThisAbility == null) continue;

                ThingComp val = Pawn.AllComps.First(comp => comp.GetType() == item.compAbilityUserClass);
                if (!(val is CompAbilityUser compAbilityUser)) continue;

                PawnAbility pawnAbility = compAbilityUser.AbilityData.AllPowers.First(ability => ability.Def == useThisAbility.ability);
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
