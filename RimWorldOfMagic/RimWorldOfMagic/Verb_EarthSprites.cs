using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using UnityEngine;
using Verse.Sound;


namespace TorannMagic
{
    public class Verb_EarthSprites : VFECore.Abilities.Verb_CastAbility
    {

        private int verVal;
        private int effVal;

        bool validTarg;
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map))
            {
                if ((root - targ.Cell).LengthHorizontal < verbProps.range)
                {
                    validTarg = true;
                }
                else
                {
                    //out of range
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
            CompAbilityUserMagic comp = CasterPawn.GetCompAbilityUserMagic();
            effVal = TM_Calc.GetSkillEfficiencyLevel(CasterPawn, ability.def as TMAbilityDef);
            verVal = TM_Calc.GetSkillVersatilityLevel(CasterPawn, ability.def as TMAbilityDef);

            if(!(comp.maxMP < .6f - (.07f * verVal)))
            {
                if(currentTarget.IsValid && currentTarget.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map))
                {
                    Building isBuilding = null;
                    TerrainDef terrain = null;
                    isBuilding = currentTarget.Cell.GetFirstBuilding(CasterPawn.Map);
                    terrain = currentTarget.Cell.GetTerrain(CasterPawn.Map);
                    //Log.Message("terrain type is " + terrain.defName);
                    if (isBuilding != null)
                    {
                        var mineable = isBuilding as Mineable;
                        if (mineable != null)
                        {
                            Area spriteArea = TM_Calc.GetSpriteArea();
                            if(spriteArea != null && spriteArea.ActiveCells != null && spriteArea.ActiveCells.Contains(currentTarget.Cell))
                            {
                                comp.earthSpritesInArea = true;
                            }
                            comp.earthSprites = currentTarget.Cell;
                            comp.earthSpriteType.Set(1);
                            comp.earthSpriteMap = CasterPawn.Map;
                            comp.nextEarthSpriteAction = Find.TickManager.TicksGame + 300;
                        }
                        else
                        {
                            Messages.Message("TM_InvalidTarget".Translate(
                                CasterPawn.LabelShort,
                                "Earth Sprites"
                            ), MessageTypeDefOf.RejectInput);
                        }
                    }
                    else if (terrain is { defName: "MarshyTerrain" or "Mud" or "Marsh" or "WaterShallow" or "Ice" or "Sand" or "Gravel" or "Soil" or "MossyTerrain" or "SoftSand" })
                    {
                        Area spriteArea = TM_Calc.GetSpriteArea();
                        if (spriteArea != null && spriteArea.ActiveCells != null && spriteArea.ActiveCells.Contains(currentTarget.Cell))
                        {
                            comp.earthSpritesInArea = true;
                        }
                        comp.earthSprites = currentTarget.Cell;
                        comp.earthSpriteType.Set(2);
                        comp.earthSpriteMap = CasterPawn.Map;
                        comp.nextEarthSpriteAction = Find.TickManager.TicksGame + 20000;
                    }
                    else if(terrain != null && !terrain.defName.Contains("water") && !terrain.defName.Contains("Water"))
                    {
                        ShatterTerrain(currentTarget.Cell, terrain);
                    }
                    else
                    {
                        Messages.Message("TM_InvalidTarget".Translate(
                            CasterPawn.LabelShort,
                            "Earth Sprites"
                        ), MessageTypeDefOf.RejectInput);
                    }
                }
                else
                {
                    Messages.Message("TM_InvalidTarget".Translate(
                        CasterPawn.LabelShort,
                        "Earth Sprites"
                    ), MessageTypeDefOf.RejectInput);
                }
            }
            else
            {
                Messages.Message("TM_NotEnoughMaxMana".Translate(
                    CasterPawn.LabelShort,
                    "Earth Sprites"
                ), MessageTypeDefOf.RejectInput);
            }

            burstShotsLeft = 0;
            return false;
        }

        public void ShatterTerrain(IntVec3 center, TerrainDef terrainDef)
        {
            List<IntVec3> cellList = GenRadial.RadialCellsAround(center, 2f, true).ToList();
            Building bldg = null;
            TerrainDef terrain = null;
            for (int i = 0; i < cellList.Count; i++)
            {
                IntVec3 cell = cellList[i];                
                bldg = cell.GetFirstBuilding(CasterPawn.Map);
                terrain = cell.GetTerrain(CasterPawn.Map);
                if (cell.InBoundsWithNullCheck(CasterPawn.Map) && bldg == null && terrain == terrainDef)
                {
                    CasterPawn.Map.terrainGrid.SetTerrain(cell, TerrainDef.Named("Gravel"));
                    FleckMaker.ThrowSmoke(cell.ToVector3Shifted(), CasterPawn.Map, Rand.Range(.8f, 1.2f));
                    Vector3 moteDirection = TM_Calc.GetVector(center, cell);
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_Rubble, cell.ToVector3Shifted(), CasterPawn.Map, Rand.Range(.3f, .6f), .2f, .02f, .05f, Rand.Range(-100, 100), Rand.Range(2f, 4f), (Quaternion.AngleAxis(90, Vector3.up) * moteDirection).ToAngleFlat(), 0);
                    TM_MoteMaker.ThrowGenericFleck(FleckDefOf.Smoke, cell.ToVector3Shifted(), CasterPawn.Map, Rand.Range(.9f, 1.2f), .3f, .02f, Rand.Range(.25f, .4f), Rand.Range(-100, 100), Rand.Range(2f, 4f), (Quaternion.AngleAxis(90, Vector3.up) * moteDirection).ToAngleFlat(), 0);
                }
                bldg = null;
            }
            TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_EarthCrack, center.ToVector3Shifted(), CasterPawn.Map, Rand.Range(2f, 2.3f), .2f, .25f, 1.5f, 0, 0f, 0f, Rand.Range(0, 360));
            TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_EarthCrack, center.ToVector3Shifted(), CasterPawn.Map, Rand.Range(3f, 3.5f), .2f, .25f, 1.7f, 0, 0f, 0f, Rand.Range(0, 360));
            Find.CameraDriver.shaker.DoShake(5f);
            SoundInfo info = SoundInfo.InMap(new TargetInfo(center, CasterPawn.Map, false), MaintenanceType.None);
            info.pitchFactor = .3f;
            info.volumeFactor = 1.9f;
            SoundDef.Named("PunchThroughRoofMetal").PlayOneShot(info);
        }
    }
}
