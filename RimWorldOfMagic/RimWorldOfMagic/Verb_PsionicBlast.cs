using Verse;

using UnityEngine;
using System.Linq;

namespace TorannMagic
{
    class Verb_PsionicBlast : VFECore.Abilities.Verb_CastAbility
    {
        bool validTarg;
        private int verVal;

        protected override bool TryCastShot()
        {
            bool result = false;
            Pawn pawn = CasterPawn;
            verVal = TM_Calc.GetSkillVersatilityLevel(pawn, ability.def as TMAbilityDef);
            Map map = CasterPawn.Map;
            IntVec3 targetVariation = currentTarget.Cell;
            targetVariation.x += Mathf.RoundToInt(Rand.Range(-.1f, .1f) * Vector3.Distance(pawn.DrawPos, currentTarget.CenterVector3) + Rand.Range(-1f, 1f)) ;
            targetVariation.z += Mathf.RoundToInt(Rand.Range(-.1f, .1f) * Vector3.Distance(pawn.DrawPos, currentTarget.CenterVector3) + Rand.Range(-1f, 1f));
            float angle = (Quaternion.AngleAxis(90, Vector3.up) * GetVector(pawn.Position, targetVariation)).ToAngleFlat();
            Vector3 drawPos = pawn.DrawPos + (GetVector(pawn.Position, targetVariation) * .5f);
            TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_PsiBlastStart, drawPos, pawn.Map, Rand.Range(.4f, .6f), Rand.Range(.0f, .05f), .1f, .2f, 0, 0, 0, angle); //throw psi blast start
            TM_MoteMaker.ThrowGenericMote(TorannMagicDefOf.Mote_PsiBlastEnd, drawPos, pawn.Map, Rand.Range(.4f, .8f), Rand.Range(.0f, .1f), .2f, .3f, 0, Rand.Range(1f, 1.5f), angle, angle); //throw psi blast end 
            TryLaunchProjectile(verbProps.defaultProjectile, targetVariation);
            float psiEnergy = 0;
            if (pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_PsionicHD"), false))
            {
                psiEnergy = CasterPawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("TM_PsionicHD"), false).Severity;
                HealthUtility.AdjustSeverity(pawn, HediffDef.Named("TM_PsionicHD"), -(4 - verVal));
            }
            result = Rand.Chance(((verVal*4)+psiEnergy)/100);            
            return result;
        }

        public Vector3 GetVector(IntVec3 center, IntVec3 objectPos)
        {
            Vector3 heading = (objectPos - center).ToVector3();
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;
            return direction;
        }
    }
}
