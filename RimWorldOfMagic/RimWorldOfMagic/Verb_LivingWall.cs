using System.Collections.Generic;
using RimWorld;

using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_LivingWall : VFECore.Abilities.Verb_CastAbility
    {
        private int verVal;

        bool validTarg;
        //Used for non-unique abilities that can be used with shieldbelt
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.Thing != null && targ.Thing == caster)
            {
                return verbProps.targetParams.canTargetSelf;
            }
            if (targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map))
            {
                if ((root - targ.Cell).LengthHorizontal < verbProps.range)
                {
                    return true;
                }
                validTarg = false;
            }
            else
            {
                validTarg = false;
            }
            return validTarg;
        }

        protected override bool TryCastShot()
        {
            Pawn casterPawn = CasterPawn;
            //Pawn pawn = currentTarget.Thing as Pawn;
            CompAbilityUserMagic comp = casterPawn.GetCompAbilityUserMagic();
            verVal = TM_Calc.GetSkillVersatilityLevel(casterPawn, ability.def as TMAbilityDef);
            //verVal = TM_Calc.GetMagicSkillLevel(caster, comp.MagicData.MagicPowerSkill_LivingWall, "TM_LivingWall", "_ver", true);
            //pwrVal = TM_Calc.GetMagicSkillLevel(caster, comp.MagicData.MagicPowerSkill_LivingWall, "TM_LivingWall", "_pwr", true);
            if (comp == null) return true;

            List<Thing> tList = currentTarget.Cell.GetThingList(casterPawn.Map);
            if(tList is { Count: > 0 })
            {
                bool wallDetected = false;
                foreach(Thing t in tList)
                {
                    if(t.Faction == casterPawn.Faction && TM_Calc.IsWall(t))
                    {
                        if(comp.livingWall.Value != null && !comp.livingWall.Value.DestroyedOrNull())
                        {
                            comp.livingWall.Value.Destroy();
                        }
                        FleckMaker.ThrowLightningGlow(t.DrawPos, casterPawn.Map, 1f);
                        Thing launchedThing = new Thing
                        {
                            def = TorannMagicDefOf.FlyingObject_LivingWall
                        };
                        FlyingObject_LivingWall flyingObject = (FlyingObject_LivingWall)GenSpawn.Spawn(TorannMagicDefOf.FlyingObject_LivingWall, t.Position, CasterPawn.Map);
                        List<Vector3> path = new List<Vector3>();
                        Vector3 newVec = t.Position.ToVector3Shifted();
                        path.Add(newVec);
                        path.Add(newVec);
                        flyingObject.ExactLaunch(null, 0, false, path, casterPawn, newVec, t, launchedThing, 15+3*verVal, 0);
                        comp.livingWall.Set(flyingObject);
                        wallDetected = true;
                        break;
                    }
                }
                if(!wallDetected)
                {
                    Messages.Message("TM_InvalidTarget".Translate(casterPawn.LabelShort, ability.def.label), MessageTypeDefOf.NegativeEvent);
                    MoteMaker.ThrowText(casterPawn.DrawPos, casterPawn.Map, "Living Wall: Cast Failed");
                }
            }
            else
            {
                Messages.Message("TM_InvalidTarget".Translate(casterPawn.LabelShort, ability.def.label), MessageTypeDefOf.NegativeEvent);
                MoteMaker.ThrowText(casterPawn.DrawPos, casterPawn.Map, "Living Wall: Cast Failed");
            }

            return true;
        }
    }
}
