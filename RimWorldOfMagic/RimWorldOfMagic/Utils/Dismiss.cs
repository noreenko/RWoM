namespace TorannMagic.Utils;

public abstract class Dismiss<T>
{
    protected bool DismissSpellLearned
    {
        set
        {
            if (value)
            {
                if (!dismissSpellLearned) abilityUser.GiveAbility(abilityDef);
            }
            else
            {
                if (dismissSpellLearned) abilityUser.RemoveAbility(abilityDef);
            }
            dismissSpellLearned = value;
        }
    }
    private bool dismissSpellLearned;
    private readonly TMAbilityDef abilityDef;
    private readonly CompAbilityUserTMBase abilityUser;

    public Dismiss(CompAbilityUserTMBase _abilityUser, TMAbilityDef _abilityDef)
    {
        abilityUser = _abilityUser;
        abilityDef = _abilityDef;
    }
}
