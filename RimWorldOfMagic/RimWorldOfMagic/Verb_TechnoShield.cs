﻿using RimWorld;
using Verse;
using Verse.Sound;

namespace TorannMagic
{
    public class Verb_TechnoShield : VFECore.Abilities.Verb_CastAbility
    {
        
        int pwrVal;
        int verVal;
        CompAbilityUserMagic comp;

        bool validTarg;
        //Used for non-unique abilities that can be used with shieldbelt
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.Thing != null && targ.Thing == this.caster)
            {
                return this.verbProps.targetParams.canTargetSelf;
            }
            if (targ.IsValid && targ.CenterVector3.InBoundsWithNullCheck(base.CasterPawn.Map) && !targ.Cell.Fogged(base.CasterPawn.Map) && targ.Cell.Walkable(base.CasterPawn.Map))
            {
                if ((root - targ.Cell).LengthHorizontal < this.verbProps.range)
                {
                    ShootLine shootLine;
                    validTarg = this.TryFindShootLineFromTo(root, targ, out shootLine);
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
            Pawn caster = base.CasterPawn;
            Pawn pawn = this.currentTarget.Thing as Pawn;

            comp = caster.GetCompAbilityUserMagic();
            pwrVal = TM_Calc.GetSkillPowerLevel(caster, ability.def as TMAbilityDef);
            verVal = TM_Calc.GetSkillVersatilityLevel(caster, ability.def as TMAbilityDef);

            if (pawn != null)
            {
                ApplyTechnoShield(pawn);                
            }            
            return true;
        }

        public void ApplyTechnoShield(Pawn pawn)
        {
            ApplyHediffs(pawn);
            SoundInfo info = SoundInfo.InMap(new TargetInfo(pawn.Position, pawn.Map, false), MaintenanceType.None);
            info.pitchFactor = .7f;
            SoundDefOf.EnergyShield_Reset.PlayOneShot(info);
            FleckMaker.ThrowLightningGlow(pawn.DrawPos, pawn.Map, 1.5f);
            TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_TechnoShield, pawn.DrawPos, pawn.Map, .3f, .2f, 0, .2f, Rand.Range(-500, 500), 0, 0, Rand.Range(0, 360));
            TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_TechnoShield, pawn.DrawPos, pawn.Map, .6f, .2f, .1f, .1f, Rand.Range(-500, 500), 0, 0, Rand.Range(0, 360));
            TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_TechnoShield, pawn.DrawPos, pawn.Map, 1f, .2f, .2f, .05f, Rand.Range(-500, 500), 0, 0, Rand.Range(0, 360));
        }

        private void ApplyHediffs(Pawn target)
        {
            if (this.pwrVal == 3)
            {
                HealthUtility.AdjustSeverity(target, TorannMagicDefOf.TM_TechnoShieldHD_III, (110 + (30 * verVal))*comp.arcaneDmg);
            }
            else if(this.pwrVal == 2)
            {
                HealthUtility.AdjustSeverity(target, TorannMagicDefOf.TM_TechnoShieldHD_II, (110 + (30 * verVal))*comp.arcaneDmg);
            }
            else if(this.pwrVal == 1)
            {
                HealthUtility.AdjustSeverity(target, TorannMagicDefOf.TM_TechnoShieldHD_I, (110 + (30 * verVal))*comp.arcaneDmg);
            }
            else
            {
                HealthUtility.AdjustSeverity(target, TorannMagicDefOf.TM_TechnoShieldHD, (110 + (30 * verVal))*comp.arcaneDmg);
            }
        }
    }
}
