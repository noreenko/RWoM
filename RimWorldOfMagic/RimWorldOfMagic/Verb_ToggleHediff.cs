using RimWorld;
using Verse;

namespace TorannMagic
{
    public class Verb_ToggleHediff : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            Pawn casterPawn = CasterPawn;

            if (casterPawn is { Dead: false })
            {
                TMAbilityDef abilityDef = (TMAbilityDef)ability.def;
                if (abilityDef?.abilityHediff != null)
                {
                    HediffDef hdDef = abilityDef.abilityHediff;
                    if (casterPawn.health.hediffSet.HasHediff(hdDef))
                    {
                        Hediff hd = casterPawn.health.hediffSet.GetFirstHediffOfDef(hdDef);
                        casterPawn.health.RemoveHediff(hd);
                    }
                    else
                    {
                        HealthUtility.AdjustSeverity(casterPawn, hdDef, hdDef.initialSeverity);
                        if (casterPawn.Map != null)
                        {
                            FleckMaker.ThrowLightningGlow(casterPawn.DrawPos, casterPawn.Map, 1f);
                            FleckMaker.ThrowDustPuff(casterPawn.Position, casterPawn.Map, 1f);
                        }
                    }

                    CompAbilityUserMagic magicComp = casterPawn.GetCompAbilityUserMagic();
                    if(magicComp?.MagicData != null)
                    {
                        MagicPower mp = magicComp.MagicData.ReturnMatchingMagicPower(abilityDef);
                        if(mp != null)
                        {
                            mp.autocast = casterPawn.health.hediffSet.HasHediff(hdDef);
                        }
                    }
                    CompAbilityUserMight mightComp = casterPawn.GetCompAbilityUserMight();
                    if (mightComp?.MightData != null)
                    {
                        MightPower mp = mightComp.MightData.ReturnMatchingMightPower(abilityDef);
                        if (mp != null)
                        {
                            mp.autocast = casterPawn.health.hediffSet.HasHediff(hdDef);
                        }
                    }

                }
                else
                {
                    Log.Warning("Unrecognized ability or no hediff assigned for this ability.");
                }
                
            }
            return true;
        }
    }
}
