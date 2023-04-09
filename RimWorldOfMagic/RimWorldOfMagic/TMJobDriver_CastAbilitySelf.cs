﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

using UnityEngine;


namespace TorannMagic
{
    public class TMJobDriver_CastAbilitySelf : VFECore.Abilities.JobDriver_CastAbilityOnce
    {
        public VFECore.Abilities.Verb_CastAbility verb = new VFECore.Abilities.Verb_CastAbility();
        bool cooldownFlag = false;
        bool energyFlag;
        bool validCastFlag;
        private bool wildCheck;
        private int duration;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);
            verb = pawn.CurJob.verbToUse as VFECore.Abilities.Verb_CastAbility;
           
            //if (Context == AbilityContext.Player)
            //{
            Find.Targeter.targetingSource = verb;
            //}
            if (verb?.ability?.def is TMAbilityDef tmAbility)
            {
                CompAbilityUserMight compMight = pawn.GetCompAbilityUserMight();
                CompAbilityUserMagic compMagic = pawn.GetCompAbilityUserMagic();
                if (tmAbility.manaCost > 0 && pawn.story?.traits != null && !pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless))
                {
                    if (pawn.Map.gameConditionManager.ConditionIsActive(TorannMagicDefOf.TM_ManaStorm))
                    {
                        int amt = Mathf.RoundToInt(compMagic.ActualManaCost(tmAbility) * 100f);
                        if (amt > 5)
                        {
                            pawn.Map.weatherManager.eventHandler.AddEvent(new TM_WeatherEvent_MeshFlash(Map, pawn.Position, TM_MatPool.blackLightning, TMDamageDefOf.DamageDefOf.TM_Arcane, pawn, amt, Mathf.Clamp((float)amt / 5f, 1f, 5f)));
                        }
                    }
                    if (compMagic != null && compMagic.Mana != null)
                    {
                        if (compMagic.ActualManaCost(tmAbility) > compMagic.Mana.CurLevel)
                        {
                            energyFlag = true;
                        }
                    }
                    else
                    {
                        energyFlag = true;
                    }
                }
                if (tmAbility.staminaCost > 0)
                {
                    if (compMight != null && compMight.Stamina != null)
                    {
                        if (compMight.ActualStaminaCost(tmAbility) > compMight.Stamina.CurLevel)
                        {
                            energyFlag = true;
                        }
                    }
                    else
                    {
                        energyFlag = true;
                    }
                }
            }
            validCastFlag = cooldownFlag || energyFlag;
            //yield return Toils_Combat.CastVerb(TargetIndex.A, false);
            Toil toil1 = new Toil()
            {
                initAction = () => {
                    pawn.pather.StopDead();
                },
                defaultCompleteMode = ToilCompleteMode.Instant                
            };
            yield return toil1;
            //
            Toil combatToil = new Toil();
            combatToil.initAction = delegate
            {
                verb = combatToil.actor.jobs.curJob.verbToUse as VFECore.Abilities.Verb_CastAbility;
                if (verb != null && verb.verbProps != null)
                {
                    try
                    {
                        duration = (int)((verb.verbProps.warmupTime * 60) * pawn.GetStatValue(StatDefOf.AimingDelayFactor, false));
                    }
                    catch
                    {
                        duration = (int)(verb.verbProps.warmupTime * 60);
                    }
                    LocalTargetInfo target = combatToil.actor.jobs.curJob.GetTarget(TargetIndex.A);
                    if (target != null && !validCastFlag)
                    {
                        if (pawn.IsColonist)
                        {
                            verb.TryStartCastOn(target);
                        }
                        else
                        {
                            duration = 0;
                            verb.WarmupComplete();
                        }
                    }
                }
                else
                {
                    EndJobWith(JobCondition.Errored);
                }
            };
            combatToil.tickAction = delegate
            {
                if (pawn.Downed)
                {
                    EndJobWith(JobCondition.InterruptForced);
                }
                if (Find.TickManager.TicksGame % 12 == 0)
                {
                    TM_MoteMaker.ThrowCastingMote(pawn.DrawPos, pawn.Map, Rand.Range(1.2f, 2f));                    
                }

                duration--;
                if (!wildCheck && duration <= 6)
                {
                    wildCheck = true;
                    if (pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(TorannMagicDefOf.ChaosMage) && Rand.Chance(.1f))
                    {
                        TM_Action.DoWildSurge(pawn, pawn.GetCompAbilityUserMagic(), (MagicAbility)verb.ability, (TMAbilityDef)verb.ability.def, TargetA);
                        EndJobWith(JobCondition.InterruptForced);
                    }
                }
            };
            combatToil.AddFinishAction(delegate
            {
                if (duration <= 5 && !pawn.DestroyedOrNull() && !pawn.Dead && !pawn.Downed)
                {
                    pawn.ClearReservationsForJob(job);
                }
            });
            combatToil.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
            pawn.ClearReservationsForJob(job);
            yield return combatToil;
        }
        
    }
}
