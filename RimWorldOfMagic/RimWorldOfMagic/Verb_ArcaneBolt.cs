using Verse;

using UnityEngine;
using System.Linq;

namespace TorannMagic
{
    class Verb_ArcaneBolt : VFECore.Abilities.Verb_CastAbility  
    {
        protected override bool TryCastShot()
        {
            bool result = false;
            Pawn pawn = this.CasterPawn;
            CompAbilityUserMagic comp = pawn.GetCompAbilityUserMagic();
            int burstCountMin = 1;
            
            
            if (pawn.GetCompAbilityUserMagic().MagicData.MagicPowerSkill_Cantrips.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Cantrips_pwr").level >= 2)
            {
                burstCountMin++;
                if (pawn.GetCompAbilityUserMagic().MagicData.MagicPowerSkill_Cantrips.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Cantrips_pwr").level >= 7)
                {
                    burstCountMin++;
                }
            }
            if (!pawn.IsColonist && ModOptions.Settings.Instance.AIHardMode)
            {
                burstCountMin = 3;
            }

            Map map = this.CasterPawn.Map;
            IntVec3 targetVariation = this.currentTarget.Cell;
            targetVariation.x += Mathf.RoundToInt(Rand.Range(-.05f, .05f) * Vector3.Distance(pawn.DrawPos, this.currentTarget.CenterVector3));// + Rand.Range(-1f, 1f));
            targetVariation.z += Mathf.RoundToInt(Rand.Range(-.05f, .05f) * Vector3.Distance(pawn.DrawPos, this.currentTarget.CenterVector3));// + Rand.Range(-1f, 1f));
            this.TryLaunchProjectile(this.verbProps.defaultProjectile, targetVariation);
            this.burstShotsLeft--;
            //Log.Message("burst shots left " + this.burstShotsLeft);
            float burstCountFloat = (float)(15f - this.burstShotsLeft);
            float mageLevelFloat = (float)(burstCountMin + (comp.MagicUserLevel/10f));
            result = Rand.Chance(mageLevelFloat - burstCountFloat);            
            return result;
        }
    }
}
