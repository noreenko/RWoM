using RimWorld;
using System;
using Verse;
using Verse.Sound;

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TorannMagic
{
    public class Verb_ShapeshiftDW : VFECore.Abilities.Verb_CastAbility  
    {
        float arcaneDmg = 1f;
        public int verVal = 0;
        public int pwrVal = 0;
        public int effVal = 0;

        private int duration = 1800;

        protected override bool TryCastShot()
        {
            Pawn caster = base.CasterPawn;

            CompAbilityUserMagic comp = caster.GetCompAbilityUserMagic();
            verVal = comp.MagicData.MagicPowerSkill_Shapeshift.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Shapeshift_ver").level;
            pwrVal = comp.MagicData.MagicPowerSkill_Shapeshift.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Shapeshift_pwr").level;
            effVal = comp.MagicData.MagicPowerSkill_Shapeshift.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Shapeshift_eff").level;
            duration = Mathf.RoundToInt((duration + (360 * effVal))*comp.arcaneDmg);
            bool flag = caster != null && !caster.Dead;
            if (flag)
            {
                
                CompPolymorph compPoly = caster.GetComp<CompPolymorph>();
                if (compPoly != null && compPoly.Original != null && compPoly.TicksLeft > 0)
                {
                    compPoly.Temporary = true;
                    compPoly.TicksLeft = 0;
                }
                else
                {
                    FactionDef fDef = TorannMagicDefOf.TM_SummonedFaction;
                    if (caster.Faction != null)
                    {
                        fDef = caster.Faction.def;
                    }
                    SpawnThings spawnThing = new SpawnThings();
                    spawnThing.factionDef = fDef;
                    spawnThing.spawnCount = 1;
                    spawnThing.temporary = false;

                    spawnThing.kindDef = PawnKindDef.Named("TM_Dire_Wolf");
                    spawnThing.def = ThingDef.Named("TM_Dire_WolfR");
                    if (spawnThing.def == null || spawnThing.kindDef == null)
                    {
                        spawnThing.def = ThingDef.Named("Rat");
                        spawnThing.kindDef = PawnKindDef.Named("Rat");
                        Log.Message("random creature was null");
                    }

                    Pawn polymorphedPawn = TM_Action.PolymorphPawn(CasterPawn, caster, caster, spawnThing, caster.Position, true, duration, caster.Faction);

                    if (effVal >= 3)
                    {
                        polymorphedPawn.GetComp<CompPolymorph>().Temporary = false;
                    }

                    FleckMaker.ThrowSmoke(polymorphedPawn.DrawPos, caster.Map, 2);
                    FleckMaker.ThrowMicroSparks(polymorphedPawn.DrawPos, caster.Map);
                    FleckMaker.ThrowHeatGlow(polymorphedPawn.Position, caster.Map, 2);
                    //caster.DeSpawn();

                    HealthUtility.AdjustSeverity(polymorphedPawn, HediffDef.Named("TM_ShapeshiftHD"), .5f + (1f * pwrVal));

                }
            }
            return true;
        }
        
    }
}
