using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;

using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_RegrowLimb : VFECore.Abilities.Verb_CastAbility
    {

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            bool validTarg;
            if (targ.Thing != null && targ.Thing == caster)
            {
                return verbProps.targetParams.canTargetSelf;
            }
            if (targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
            {
                if ((root - targ.Cell).LengthHorizontal < verbProps.range)
                {
                    validTarg = TryFindShootLineFromTo(root, targ, out _);
                }
                else
                {
                    validTarg = false;
                }
            }
            else
            {
                validTarg = false;
            }
            return validTarg;
        }

        protected override bool TryCastShot()
        {

            CellRect cellRect = CellRect.CenteredOn(currentTarget.Cell, 1);
            cellRect.ClipInsideMap(CasterPawn.Map);
            IntVec3 centerCell = cellRect.CenterCell;
            Map map = CasterPawn.Map;

            if ((centerCell.IsValid && centerCell.Standable(map)))
            {
                AbilityUser.SpawnThings tempThing = new SpawnThings();
                tempThing.def = ThingDef.Named("SeedofRegrowth");
                Verb_RegrowLimb.SingleSpawnLoop(tempThing, centerCell, map);
            }
            else
            {
                Messages.Message("InvalidSummon".Translate(), MessageTypeDefOf.RejectInput);
            }
            return false;
        }

        public static void SingleSpawnLoop(SpawnThings spawnables, IntVec3 position, Map map)
        {
            if (spawnables.def != null)
            {
                ThingDef def = spawnables.def;
                ThingDef stuff = null;
                bool madeFromStuff = def.MadeFromStuff;
                if (madeFromStuff)
                {
                    stuff = ThingDefOf.WoodLog;
                }
                Thing thing = ThingMaker.MakeThing(def, stuff);
                GenPlace.TryPlaceThing(thing, position, map, ThingPlaceMode.Near);
            }
        }
    }
}
