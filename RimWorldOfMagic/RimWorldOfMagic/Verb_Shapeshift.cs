using RimWorld;
using System;
using Verse;
using Verse.Sound;

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TorannMagic
{
    public class Verb_Shapeshift : VFECore.Abilities.Verb_CastAbility  
    {
        float arcaneDmg = 1f;
        public int verVal;
        public int pwrVal;
        public int effVal;

        private int min;
        private int max;

        private int duration = 1800;

        protected override bool TryCastShot()
        {
            Pawn casterPawn = base.CasterPawn;

            CompAbilityUserMagic comp = casterPawn.GetCompAbilityUserMagic();
            verVal = comp.MagicData.MagicPowerSkill_Shapeshift.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Shapeshift_ver").level;
            pwrVal = comp.MagicData.MagicPowerSkill_Shapeshift.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Shapeshift_pwr").level;
            effVal = comp.MagicData.MagicPowerSkill_Shapeshift.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_Shapeshift_eff").level;
            duration = Mathf.RoundToInt((duration + (360 * effVal))*comp.arcaneDmg);
            bool flag = casterPawn != null && !casterPawn.Dead;
            if (flag)
            {
                
                CompPolymorph compPoly = casterPawn.GetComp<CompPolymorph>();
                if (compPoly != null && compPoly.Original != null && compPoly.TicksLeft > 0)
                {
                    compPoly.Temporary = true;
                    compPoly.TicksLeft = 0;
                }
                else
                {
                    FactionDef fDef = TorannMagicDefOf.TM_SummonedFaction;
                    if (casterPawn.Faction != null)
                    {
                        fDef = casterPawn.Faction.def;
                    }
                    SpawnThings spawnThing = new SpawnThings();
                    spawnThing.factionDef = fDef;
                    spawnThing.spawnCount = 1;
                    spawnThing.temporary = false;

                    GetPolyMinMax(casterPawn);

                    spawnThing = TM_Action.AssignRandomCreatureDef(spawnThing, min, max);
                    if (spawnThing.def == null || spawnThing.kindDef == null)
                    {
                        spawnThing.def = ThingDef.Named("Rat");
                        spawnThing.kindDef = PawnKindDef.Named("Rat");
                        Log.Message("random creature was null");
                    }

                    Pawn polymorphedPawn = TM_Action.PolymorphPawn(CasterPawn, casterPawn, casterPawn, spawnThing, casterPawn.Position, true, duration, casterPawn.Faction);

                    if (effVal >= 3)
                    {
                        polymorphedPawn.GetComp<CompPolymorph>().Temporary = false;
                    }

                    FleckMaker.ThrowSmoke(polymorphedPawn.DrawPos, casterPawn.Map, 2);
                    FleckMaker.ThrowMicroSparks(polymorphedPawn.DrawPos, casterPawn.Map);
                    FleckMaker.ThrowHeatGlow(polymorphedPawn.Position, casterPawn.Map, 2);
                    //caster.DeSpawn();

                    HealthUtility.AdjustSeverity(polymorphedPawn, HediffDef.Named("TM_ShapeshiftHD"), .5f + (1f * pwrVal));

                }
            }
            return true;
        }

        private void GetPolyMinMax(Pawn pawn)
        {
            if (verVal >= 3)
            {
                min = Mathf.RoundToInt(200 * arcaneDmg);
                max = Mathf.RoundToInt(500 * arcaneDmg);
            }
            else if (verVal >= 2)
            {
                min = Mathf.RoundToInt(150 * arcaneDmg);
                max = Mathf.RoundToInt(420 * arcaneDmg);
            }
            else if (verVal >= 1)
            {
                min = Mathf.RoundToInt(120 * arcaneDmg);
                max = Mathf.RoundToInt(320 * arcaneDmg);
            }
            else
            {
                min = Mathf.RoundToInt(80 * arcaneDmg);
                max = Mathf.RoundToInt(250 * arcaneDmg);
            }
        }
    }
}
