using RimWorld;
using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_BrandSiphon : VFECore.Abilities.Verb_CastAbility
    {
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
            Pawn casterPawn = CasterPawn;
            
            if(casterPawn != null && CurrentTarget is { HasThing: true, Thing: Pawn hitPawn })
            {
                if (hitPawn is { RaceProps.Humanlike: true } && !TM_Calc.IsUndead(hitPawn) && hitPawn != casterPawn)
                {
                    CompAbilityUserMagic casterComp = casterPawn.GetCompAbilityUserMagic();
                    CompAbilityUserMagic targetComp = hitPawn.GetCompAbilityUserMagic();
                    if (casterComp != null && targetComp != null && targetComp.Mana != null && targetComp.IsMagicUser && hitPawn.health != null && hitPawn.health.hediffSet != null)
                    {
                        TM_Action.UpdateBrand(hitPawn, casterPawn, casterComp, TorannMagicDefOf.TM_SiphonBrandHD);

                        UpdateHediffComp(hitPawn);
                        DoBrandingEffect(hitPawn);
                    }
                    else
                    {
                        Messages.Message("TM_InvalidTarget".Translate(CasterPawn.LabelShort, ability.def.label), MessageTypeDefOf.RejectInput);
                    }
                }
                else
                {
                    Messages.Message("TM_InvalidTarget".Translate(CasterPawn.LabelShort, ability.def.label), MessageTypeDefOf.RejectInput);
                }
            }
            else
            {
                Messages.Message("TM_InvalidTarget".Translate(CasterPawn.LabelShort, ability.def.label), MessageTypeDefOf.RejectInput);
            }

            return false;
        }

        private void UpdateHediffComp(Pawn hitPawn)
        {
            Hediff hd = hitPawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_SiphonBrandHD);
            HediffComp_BrandingBase hdc = hd?.TryGetComp<HediffComp_BrandingBase>();
            if (hdc != null)
            {
                hdc.BranderPawn = CasterPawn;
            }
        }

        private void DoBrandingEffect(Pawn hitPawn)
        {
            if (hitPawn?.Map != null)
            {
                TargetInfo ti = new TargetInfo(hitPawn.Position, hitPawn.Map);
                TM_MoteMaker.MakeOverlay(ti, TorannMagicDefOf.TM_Mote_PsycastAreaEffect, hitPawn.Map, Vector3.zero, 1f, 0f, .1f, .4f, 1.2f, -3f);
            }
        }
    }
}
