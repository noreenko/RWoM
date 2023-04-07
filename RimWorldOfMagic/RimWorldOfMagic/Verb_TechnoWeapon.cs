using Verse;


namespace TorannMagic
{
    public class Verb_TechnoWeapon : VFECore.Abilities.Verb_CastAbility
    {
        protected override bool TryCastShot()
        {
            Pawn caster = base.CasterPawn;
            Thing thing = currentTarget.Cell.GetFirstItem(caster.Map);
            TM_Action.DoAction_TechnoWeaponCopy(caster, thing);
            
            burstShotsLeft = 0;
            return true;
        }
    }
}
