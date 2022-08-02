using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorannMagic
{
    public static class Utils
    {
        /// <summary>
        /// Performence friendly way to check for multible Traits
        /// </summary>
        /// <param name="pawntraits"></param>
        /// <param name="checktraitdefs"></param>
        /// <returns></returns>
        public static bool HasAnyTraitsinArray(List<Trait> pawntraits, TraitDef[] checktraitdefs)
        {
            foreach (var trait in pawntraits)
            {
                var tdef = trait.def;
                for (int i = 0; i < checktraitdefs.Length; i++)
                {
                    if (tdef == checktraitdefs[i]) return true;
                }
            }
            return false;
        }
    }
}