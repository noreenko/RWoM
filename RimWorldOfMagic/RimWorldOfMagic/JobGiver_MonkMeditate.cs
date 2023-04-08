using System;
using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace TorannMagic;

public class JobGiver_MonkMeditate : ThinkNode_JobGiver
{
    int verVal = 0;
    public override float GetPriority(Pawn pawn)
    {
        Hediff chiHD = pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_ChiHD);
        CompAbilityUserMight comp = pawn.GetCompAbilityUserMight();
        if (chiHD == null)
        {
            return 0f;
        }
        if(comp == null)
        {
            return 0f;
        }
        if ((int)chiHD.Severity > 80)
        {
            return 0f;
        }
        if (Find.TickManager.TicksGame < comp.allowMeditateTick)
        {
            return 0f;
        }
        TimeAssignmentDef timeAssignmentDef;
        if (pawn.RaceProps.Humanlike)
        {
            timeAssignmentDef = ((pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything);
        }
        else
        {
            int num = GenLocalDate.HourOfDay(pawn);
            timeAssignmentDef = ((num >= 7 && num <= 21) ? TimeAssignmentDefOf.Anything : TimeAssignmentDefOf.Sleep);
        }
        float curLevel = chiHD.Severity;
        if (timeAssignmentDef == TimeAssignmentDefOf.Anything)
        {
            return curLevel switch
            {
                < 30.0f => 8f,
                < 70.0f => 4f,
                _ => 0f
            };
        }
        if (timeAssignmentDef == TimeAssignmentDefOf.Work)
        {
            return 0f;
        }
        if (timeAssignmentDef == TimeAssignmentDefOf.Joy)
        {
            if (curLevel < 70f)
            {
                return 8f;
            }
            return 0f;
        }
        if (timeAssignmentDef == TimeAssignmentDefOf.Sleep)
        {
            return curLevel switch
            {
                < 50 when verVal >= 3 => 8f,
                < 70 when verVal >= 2 => 6f,
                _ => 0f
            };
        }
        throw new NotImplementedException();
    }

    protected override Job TryGiveJob(Pawn pawn)
    {
        if (pawn is { Map: { }, health.hediffSet: { } } && pawn.health.hediffSet.HasHediff(TorannMagicDefOf.TM_ChiHD) && !pawn.Drafted)
        {
            if (pawn.InBed() || HealthAIUtility.ShouldSeekMedicalRest(pawn) || pawn.GetPosture() != PawnPosture.Standing)
            {
                return null;
            }
            Need_Joy curJoy = pawn.needs.joy;
            if(curJoy == null)
            {
                return null;
            }
            if(curJoy.CurLevel >= .8f)
            {
                return null;
            }
            if(pawn.CurJob != null && pawn.CurJob.playerForced)
            {
                return null;
            }
            if(pawn.timetable != null && !(pawn.timetable.CurrentAssignment.allowJoy && pawn.timetable.CurrentAssignment.allowRest))
            {
                return null;
            }
            CompAbilityUserMight comp = pawn.GetCompAbilityUserMight();
            if (comp != null)
            {
                MightPower mightPower = comp.MightData.MightPowersM.FirstOrDefault<MightPower>((MightPower x) => x.abilityDef == TorannMagicDefOf.TM_Meditate);

                if (mightPower == null)
                {
                    return null;
                }

                if (!mightPower.AutoCast)
                {
                    return null;
                }

                Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_ChiHD);
                VFECore.Abilities.Ability ability = comp.LearnedAbilities.FirstOrDefault(a => a.def == TorannMagicDefOf.TM_Meditate);

                if (ability.cooldown > Find.TickManager.TicksGame || hediff.Severity >= 70)
                {
                    return null;
                }

                Building_Bed building_Bed = pawn.ownership.OwnedBed;
                if (building_Bed != null)
                {
                    if (building_Bed.GetRoom() != null && !building_Bed.GetRoom().PsychologicallyOutdoors)
                    {
                        List<IntVec3> roomCells = building_Bed.GetRoom().Cells.ToList();
                        for (int i = 0; i < roomCells.Count; i++)
                        {
                            if (roomCells[i].IsValid && roomCells[i].Walkable(pawn.Map) && roomCells[i].GetFirstBuilding(pawn.Map) == null)
                            {
                                return new Job(TorannMagicDefOf.JobDriver_TM_Meditate, roomCells[i]);
                            }
                        }
                    }

                }
                return new Job(TorannMagicDefOf.JobDriver_TM_Meditate, pawn.Position);
            }
        }
        return null;
    }


}
