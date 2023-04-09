using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

using Verse;
using Verse.Sound;


namespace TorannMagic
{
    public class Verb_Elixir : VFECore.Abilities.Verb_CastAbility
    {
        bool validTarg;
        //Used specifically for non-unique verbs that ignore LOS (can be used with shield belt)
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ != null && targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
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
            bool flag = false;

            Map map = CasterPawn.Map;

            Pawn hitPawn = currentTarget.Thing as Pawn;
            Pawn casterPawn = CasterPawn;

            int verVal = TM_Calc.GetSkillVersatilityLevel(casterPawn, ability.def as TMAbilityDef, false);
            int pwrVal = TM_Calc.GetSkillPowerLevel(casterPawn, ability.def as TMAbilityDef, false);

            try
            {
                if (!hitPawn.DestroyedOrNull() & !hitPawn.Dead && hitPawn.Spawned && map != null && hitPawn.health != null && hitPawn.health.hediffSet != null)
                {
                    HealthUtility.AdjustSeverity(hitPawn, TorannMagicDefOf.TM_HerbalElixirHD, .7f + (.1f * pwrVal));
                    HediffComp_HerbalElixir hdhe = hitPawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_HerbalElixirHD).TryGetComp<HediffComp_HerbalElixir>();
                    if(hdhe != null)
                    {
                        hdhe.pwrVal = pwrVal;
                        hdhe.verVal = verVal;
                    }
                    SoundInfo info = SoundInfo.InMap(new TargetInfo(casterPawn.Position, casterPawn.Map));
                    TorannMagicDefOf.TM_Powerup.PlayOneShot(info);
                }
                else
                {
                    Messages.Message("TM_InvalidTarget".Translate(casterPawn.LabelShort, ability.def.label), MessageTypeDefOf.NeutralEvent);
                }
            }
            catch (NullReferenceException)
            {
                //ex
            }
            return false;
        }
    }
}
