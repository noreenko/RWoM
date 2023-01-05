using Verse;

namespace TorannMagic
{
    internal static class MagicUserUtility
    {
        public static Need_Mana GetMana(Pawn pawn)
        {
            return pawn.GetCompAbilityUserMagic()?.Mana;
        }

        public static CompAbilityUserMagic GetMagicUser(Pawn pawn)
        {
            return pawn.GetCompAbilityUserMagic();
        }
    }
}
