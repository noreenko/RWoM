﻿using Verse;
namespace TorannMagic
{
    public class Verb_ChaosTradition : VFECore.Abilities.Verb_CastAbility
    {
        private int verVal;
        private int pwrVal;
        private int effVal;

        private int gRegen;
        private int gEff;
        private int gSpirit;

        protected override bool TryCastShot()
        {
            CompAbilityUserMagic comp = CasterPawn.GetCompAbilityUserMagic();            

            if (CasterPawn != null && !CasterPawn.Downed && comp != null && comp.MagicData != null)
            {                
                pwrVal = comp.MagicData.MagicPowerSkill_ChaosTradition.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_ChaosTradition_pwr").level;
                verVal = comp.MagicData.MagicPowerSkill_ChaosTradition.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_ChaosTradition_ver").level;
                effVal = comp.MagicData.MagicPowerSkill_ChaosTradition.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_ChaosTradition_eff").level;

                gRegen = comp.MagicData.MagicPowerSkill_global_regen.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_regen_pwr").level;
                gEff = comp.MagicData.MagicPowerSkill_global_eff.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_eff_pwr").level;
                gSpirit = comp.MagicData.MagicPowerSkill_global_spirit.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_spirit_pwr").level;

                TM_Action.ClearSustainedMagicHediffs(comp);                
                TM_Calc.AssignChaosMagicPowers(comp);

                if(effVal >= 3)
                {
                    HealthUtility.AdjustSeverity(CasterPawn, TorannMagicDefOf.TM_ChaosTraditionHD, 8f);
                }
                if(effVal >= 2)
                {
                    float manaReMod = 1f;
                    if(Find.TickManager.TicksGame - comp.lastChaosTraditionTick < 57500)
                    {
                        manaReMod = (float)(Find.TickManager.TicksGame - comp.lastChaosTraditionTick) / 57500f;
                    }
                    comp.Mana.CurLevel += .4f * comp.mpRegenRate * manaReMod;
                }
                if(effVal >= 1)
                { 
                    HealthUtility.AdjustSeverity(CasterPawn, TorannMagicDefOf.TM_ChaoticMindHD, 24f);
                }

                comp.MagicData.MagicAbilityPoints -= ((2*(pwrVal + verVal + effVal)) + gSpirit + gRegen + gEff);
                comp.MagicData.MagicPowerSkill_ChaosTradition.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_ChaosTradition_pwr").level = pwrVal;
                comp.MagicData.MagicPowerSkill_ChaosTradition.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_ChaosTradition_ver").level = verVal;
                comp.MagicData.MagicPowerSkill_ChaosTradition.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_ChaosTradition_eff").level = effVal;

                comp.MagicData.MagicPowerSkill_global_regen.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_regen_pwr").level = gRegen;
                comp.MagicData.MagicPowerSkill_global_eff.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_eff_pwr").level = gEff;
                comp.MagicData.MagicPowerSkill_global_spirit.FirstOrDefault((MagicPowerSkill x) => x.label == "TM_global_spirit_pwr").level = gSpirit;

                if(comp.MagicData.MagicAbilityPoints < 0)
                {
                    comp.MagicData.MagicAbilityPoints = 0;
                }

                ClearSpellRemnants(comp);
                comp.lastChaosTraditionTick = Find.TickManager.TicksGame;
            }
            else
            {
                Log.Warning("failed to TryCastShot");
            }

            burstShotsLeft = 0;
            return false;
        }  
        
        public void ClearSpellRemnants(CompAbilityUserMagic comp)
        {
            if (comp != null)
            {
                if (comp.Pawn.equipment != null && comp.Pawn.equipment.Primary != null && comp.Pawn.equipment.Primary.def.defName.Contains("TM_TechnoWeapon_Base"))
                {                        
                    comp.technoWeaponThing = null;
                    comp.technoWeaponThingDef = null;
                    comp.technoWeaponDefNum = -1;
                    if(!comp.Pawn.equipment.Primary.DestroyedOrNull())
                    {
                        comp.Pawn.equipment.Primary.Destroy(DestroyMode.Vanish);
                    }
                }

                if (comp.earthSprites != IntVec3.Invalid || comp.earthSpriteType.Value != 0)
                {
                    comp.earthSpriteType.Set(0);
                    comp.earthSpriteMap = null;
                    comp.earthSprites = IntVec3.Invalid;
                    comp.earthSpritesInArea = false;
                }

                if (comp.stoneskinPawns != null && comp.stoneskinPawns.Count > 0)
                {
                    for (int i = 0; i < comp.stoneskinPawns.Count; i++)
                    {
                        if (comp.stoneskinPawns[i].health.hediffSet.HasHediff(TorannMagicDefOf.TM_StoneskinHD))
                        {
                            Hediff hd = comp.stoneskinPawns[i].health?.hediffSet?.GetFirstHediffOfDef(TorannMagicDefOf.TM_StoneskinHD);
                            if (hd != null)
                            {
                                comp.stoneskinPawns[i].health.RemoveHediff(hd);
                            }
                        }                        
                    }
                }

                if(comp.weaponEnchants != null && comp.weaponEnchants.Count > 0)
                {
                    for(int i = 0; i < comp.weaponEnchants.Count; i++)
                    {
                        Verb_DispelEnchantWeapon.RemoveExistingEnchantment(comp.weaponEnchants[i]);
                    }
                    comp.weaponEnchants.Clear();
                    comp.RemoveAbility(TorannMagicDefOf.TM_DispelEnchantWeapon);
                }

                if (comp.enchanterStones != null && comp.enchanterStones.Count > 0)
                {
                    for (int i = 0; i < comp.enchanterStones.Count; i++)
                    {
                        if(comp.enchanterStones[i].Map != null)
                        {
                            TM_Action.TransmutateEffects(comp.enchanterStones[i].Position, comp.Pawn);
                        }
                        comp.enchanterStones[i].Destroy(DestroyMode.Vanish);
                    }
                    comp.enchanterStones.Clear();
                    comp.RemoveAbility(TorannMagicDefOf.TM_DismissEnchanterStones);
                }

                if (comp.summonedSentinels != null && comp.summonedSentinels.Count > 0)
                {
                    for (int i = 0; i < comp.summonedSentinels.Count; i++)
                    {
                        if (comp.summonedSentinels[i].Map != null)
                        {
                            TM_Action.TransmutateEffects(comp.summonedSentinels[i].Position, comp.Pawn);
                        }
                        comp.summonedSentinels[i].Destroy(DestroyMode.Vanish);
                    }
                    comp.summonedSentinels.Clear();
                    comp.RemoveAbility(TorannMagicDefOf.TM_ShatterSentinel);
                }

                if (comp.soulBondPawn != null)
                {
                    comp.soulBondPawn = null;
                }
            }
        }

    }
}
