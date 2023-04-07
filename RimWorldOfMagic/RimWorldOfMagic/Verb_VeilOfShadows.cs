using Verse;
using UnityEngine;


namespace TorannMagic
{
    public class Verb_VeilOfShadows : VFECore.Abilities.Verb_CastAbility
    {
        private int verVal;
        private int pwrVal;

        protected override bool TryCastShot()
        {
            Pawn casterPawn = CasterPawn;

            if (casterPawn is { Dead: false })
            {
                CompAbilityUserMight comp = casterPawn.GetCompAbilityUserMight();
                verVal = TM_Calc.GetSkillVersatilityLevel(casterPawn, ability.def as TMAbilityDef);
                pwrVal = TM_Calc.GetSkillPowerLevel(casterPawn, ability.def as TMAbilityDef);

                HealthUtility.AdjustSeverity(casterPawn, TorannMagicDefOf.TM_ShadowCloakHD, .2f + (comp.mightPwr * verVal));

                HediffComp_Disappears hdComp = casterPawn.health.hediffSet.GetFirstHediffOfDef(TorannMagicDefOf.TM_ShadowCloakHD).TryGetComp<HediffComp_Disappears>();
                if(hdComp != null)
                {
                    hdComp.ticksToDisappear = 600 + (60 * pwrVal);
                }

                ThingDef fog = TorannMagicDefOf.Fog_Shadows;
                fog.gas.expireSeconds.min = 10 + pwrVal;
                fog.gas.expireSeconds.max = 11  + pwrVal;
                GenExplosion.DoExplosion(casterPawn.Position, casterPawn.Map, 3f + (.3f * verVal), TMDamageDefOf.DamageDefOf.TM_Toxin, casterPawn, 0, 0, TMDamageDefOf.DamageDefOf.TM_Toxin.soundExplosion, null, null, null, fog, 1f, 1, null, false, null, 0f, 0);

                for (int i = 0; i < 6; i++)
                {
                    Vector3 rndPos = casterPawn.DrawPos;
                    rndPos.x += Rand.Range(-1.5f, 1.5f);
                    rndPos.z += Rand.Range(-1.5f, 1.5f);
                    TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_ShadowCloud, rndPos, casterPawn.Map, Rand.Range(1f, 1.8f), .6f, .05f, Rand.Range(.6f, .8f), Rand.Range(-40, 40), Rand.Range(2, 3f), Rand.Range(0, 360), Rand.Range(0, 360));
                }
            }
            return true;
        }
    }
}
