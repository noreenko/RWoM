using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Noise;

namespace TorannMagic.Utils
{
    /*
     * This class stores information about which Pawns are which classes, whether they are spawned, etc. It uses
     * the harmony patches below the class to achieve this state tracking.
     */
    public static class TM_PawnTracker
    {
        // Keep track of when the pawn despawned so we can calculate cooldowns when they come back
        public static readonly Dictionary<Pawn, int> DeSpawnedPawnTickTracker = new Dictionary<Pawn, int>();

        // For simplicity, use this not fully optimized function to prevent bugs that would be hard to find
        public static void ResolveComps(Pawn pawn)
        {
            ResolveMagicComp(pawn.TryGetComp<CompAbilityUserMagic>());
            ResolveMightComp(pawn.TryGetComp<CompAbilityUserMight>());
        }
        public static void ResolveMagicComp(CompAbilityUserMagic magicComp)
        {
            if (magicComp == null) return;
            if (magicComp.SetIsMagicUser())
            {
                if (magicComp.Pawn.Spawned && !magicComp.Pawn.IsWildMan() &&
                    !magicComp.Pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless))
                {
                    magicComp.tickConditionsMet = true;
                }
                else
                {
                    magicComp.tickConditionsMet = false;
                }
            }
            else
            {
                magicComp.tickConditionsMet = false;
            }
        }

        public static void ResolveMightComp(CompAbilityUserMight mightComp)
        {
            if (mightComp == null) return;
            if (mightComp.SetIsMightUser())
            {
                if (mightComp.Pawn.Spawned && !mightComp.Pawn.NonHumanlikeOrWildMan())
                {
                    mightComp.tickConditionsMet = true;
                }
                else
                {
                    mightComp.tickConditionsMet = false;
                }
            }
            else
            {
                mightComp.tickConditionsMet = false;
            }
        }
    }


    // ====== Harmony Patches ======================================================================================

    [HarmonyPatch(typeof(TraitSet), nameof(TraitSet.GainTrait))]
    class TM_PawnTracker__GainTrait__Postfix
    {
        static void Postfix(Pawn ___pawn)
        {
            TM_PawnTracker.ResolveComps(___pawn);
        }
    }

    [HarmonyPatch(typeof(TraitSet), nameof(TraitSet.RemoveTrait))]
    class TM_PawnTracker__RemoveTrait__Postfix
    {
        static void Postfix(Pawn ___pawn)
        {
            TM_PawnTracker.ResolveComps(___pawn);
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
    class TM_PawnTracker__SpawnSetup__Postfix
    {
        static void Postfix(Pawn __instance)
        {
            var magicComp = __instance.TryGetComp<CompAbilityUserMagic>();
            TM_PawnTracker.ResolveMagicComp(magicComp);
            var mightComp = __instance.TryGetComp<CompAbilityUserMight>();
            TM_PawnTracker.ResolveMightComp(mightComp);

            int tickDeSpawned = TM_PawnTracker.DeSpawnedPawnTickTracker.TryGetValue(__instance, -1);
            if (tickDeSpawned == -1) return;

            int ticksPassed = Find.TickManager.TicksGame - tickDeSpawned;
            for (int i = 0; i < magicComp.AbilityData.AllPowers.Count; i++)
            {
                magicComp.AbilityData.AllPowers[i].CooldownTicksLeft = Math.Max(
                    magicComp.AbilityData.AllPowers[i].CooldownTicksLeft - ticksPassed, 0
                );
            }
            for (int i = 0; i < mightComp.AbilityData.AllPowers.Count; i++)
            {
                mightComp.AbilityData.AllPowers[i].CooldownTicksLeft = Math.Max(
                    mightComp.AbilityData.AllPowers[i].CooldownTicksLeft - ticksPassed, 0
                );
            }

            TM_PawnTracker.DeSpawnedPawnTickTracker.Remove(__instance);
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
    class TM_PawnTracker__DeSpawn__Postfix
    {
        static void Postfix(Pawn __instance)
        {
            TM_PawnTracker.ResolveComps(__instance);
            TM_PawnTracker.DeSpawnedPawnTickTracker[__instance] = Find.TickManager.TicksGame;
        }
    }

    // This is how pawns gain/lose WildMan status
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.ChangeKind))]
    class TM_PawnTracker__ChangeKind__Postfix
    {
        static void Postfix(Pawn __instance)
        {
            TM_PawnTracker.ResolveComps(__instance);
        }
    }
}
