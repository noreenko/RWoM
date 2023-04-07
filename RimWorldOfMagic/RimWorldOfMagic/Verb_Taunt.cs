using Verse;
using Verse.Sound;

namespace TorannMagic
{
    public class Verb_Taunt : VFECore.Abilities.Verb_CastAbility
    {
        float radius = 15f;
        float tauntChance = .6f;
        int targetsMax = 5;

        protected override bool TryCastShot()
        {
            Pawn caster = base.CasterPawn;
            Pawn pawn = this.currentTarget.Thing as Pawn;

            bool flag = pawn != null && !pawn.Dead;
            if (flag)
            {
                CompAbilityUserMight comp = caster.GetCompAbilityUserMight();
                int verVal = TM_Calc.GetSkillVersatilityLevel(caster, ability.def as TMAbilityDef);
                int pwrVal = TM_Calc.GetSkillPowerLevel(caster, ability.def as TMAbilityDef);
                radius += (2f * verVal);
                tauntChance += (pwrVal * .05f);
                targetsMax += pwrVal;

                SoundInfo info = SoundInfo.InMap(new TargetInfo(caster.Position, caster.Map, false), MaintenanceType.None);
                if(this.CasterPawn.gender == Gender.Female)
                {
                    info.pitchFactor = Rand.Range(1.1f, 1.3f); 
                }
                else
                {
                    info.pitchFactor = Rand.Range(.7f, .9f);
                }
                TorannMagicDefOf.TM_Roar.PlayOneShot(info);
                Effecter RageWave = TorannMagicDefOf.TM_RageWaveED.Spawn();
                RageWave.Trigger(new TargetInfo(caster.Position, caster.Map, false), new TargetInfo(caster.Position, caster.Map, false));
                RageWave.Cleanup();
                TM_Action.SearchAndTaunt(caster, this.radius, targetsMax, tauntChance);                
            }

            return true;
        }        
    }
}
