using Verse;
using RimWorld;
using System.Collections.Generic;
using VFECore.Abilities;
using UnityEngine;

namespace TorannMagic.Enchantment
{
    public class CompEnchantedItem : ThingComp
    {
        public List<VFECore.Abilities.Ability> MagicAbilities = new();

        public List<Trait> SoulOrbTraits = new();

        public CompAbilityUserMagic CompAbilityUserMagicTarget = null;

        public CompProperties_EnchantedItem Props => (CompProperties_EnchantedItem)props;

        public CompProperties_EnchantedStuff EnchantedStuff => parent.Stuff.GetCompProperties<CompProperties_EnchantedStuff>();

        public bool MadeFromEnchantedStuff
        {
            get
            {
                if (parent != null && parent.def.MadeFromStuff && parent.Stuff != null && parent.Stuff.GetCompProperties<CompProperties_EnchantedStuff>() != null)
                {                 
                    return EnchantedStuff.isEnchanted;
                }
                return false;
            }
        }

        public HediffDef GetEnchantedStuff_HediffDef
        {
            get
            {
                if(MadeFromEnchantedStuff && EnchantedStuff != null)
                {
                    return EnchantedStuff.appliedHediff;
                }
                return null;
            }
        }

        public Pawn WearingPawn
        {
            get
            {
                Apparel ap = parent as Apparel;
                if(ap != null)
                {
                    if(ap.Wearer != null)
                    {
                        return ap.Wearer;
                    }
                }
                ThingWithComps twc = parent as ThingWithComps;
                if(twc != null)
                {
                    Pawn_EquipmentTracker p_et = twc.ParentHolder as Pawn_EquipmentTracker;
                    if(p_et != null && p_et.pawn != null)
                    {
                        return p_et.pawn;
                    }
                }
                return null;
            }
        }

        public void GetOverlayGraphic()
        {
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            Pawn pawn = parent as Pawn;

            if(initialized && !abilitiesInitialized)
            {
                InitializeAbilities(parent);
            }

            if (!initialized)
            {
                hasEnchantment = Props.hasEnchantment;
                if(!hasEnchantment)
                {
                    hasEnchantment = MadeFromEnchantedStuff;
                }

                arcaneDmg = Props.arcaneDmg;
                arcaneDmgTier = Props.arcaneDmgTier;
                arcaneRes = Props.arcaneRes;
                arcaneResTier = Props.arcaneResTier;

                maxMP = Props.maxMP;
                maxMPTier = Props.maxMPTier;
                mpRegenRate = Props.mpRegenRate;
                mpRegenRateTier = Props.mpRegenRateTier;
                coolDown = Props.coolDown;
                coolDownTier = Props.coolDownTier;
                mpCost = Props.mpCost;
                mpCostTier = Props.mpCostTier;
                xpGain = Props.xpGain;
                xpGainTier = Props.xpGainTier;

                if(MadeFromEnchantedStuff && EnchantedStuff != null)
                {
                    maxMP += EnchantedStuff.maxEnergyOffset;
                    mpRegenRate += EnchantedStuff.energyRegenOffset;
                    coolDown += EnchantedStuff.cooldownOffset;
                    mpCost += EnchantedStuff.energyCostOffset;
                    xpGain += EnchantedStuff.xpGainOffset;
                    arcaneRes += EnchantedStuff.arcaneResOffset;
                    arcaneDmg += EnchantedStuff.arcaneDmgOffset;
                }

                healthRegenRate = Props.healthRegenRate;

                enchantmentAction = Props.enchantedAction;
                arcaneSpectre = Props.arcaneSpectre;
                phantomShift = Props.phantomShift;
                arcalleumCooldown = Props.arcalleumCooldown;
                enchantmentThought = Props.enchantmentThought;

                skillTier = Props.skillTier;

                hediff = Props.hediff;
                hediffSeverity = Props.hediffSeverity;

                if (parent.def.tickerType == TickerType.Rare)
                {
                    Find.TickManager.RegisterAllTickabilityFor(parent);
                }

                if(parent.def.tickerType == TickerType.Never)
                {
                    parent.def.tickerType = TickerType.Rare;
                    Find.TickManager.RegisterAllTickabilityFor(parent);
                }

                if(Props.hasAbility && !abilitiesInitialized)
                {
                    InitializeAbilities(parent as Apparel);
                }

                if(parent.def == TorannMagicDefOf.TM_MagicArtifact_MagicEssence && magicEssence == 0)
                {
                    magicEssence = Rand.Range(200, 500);
                }
                if(parent.def == TorannMagicDefOf.TM_MagicArtifact_MightEssence && mightEssence == 0)
                {
                    mightEssence = Rand.Range(200, 500);
                }

                initialized = true;
            }
        }        

        private void InitializeAbilities(ThingWithComps abilityThing)
        {
            if (abilityThing is Apparel abilityApparel)
            {
                if (abilityApparel.Wearer == null) return;

                CompAbilitiesApparel comp = abilityApparel.TryGetComp<CompAbilitiesApparel>();
                comp?.Notify_Equipped(abilityApparel.Wearer);
                abilitiesInitialized = true;
            }
            else
            {
                if (abilityThing == null) return;

                CompAbilitiesApparel comp = abilityThing.TryGetComp<CompAbilitiesApparel>();
                comp?.Notify_Equipped(WearingPawn);
                abilitiesInitialized = true;
            }
            
        }

        public override void CompTickRare()
        {
            if (hediff != null)
            {
                Apparel artifact = parent as Apparel;                
                if (artifact != null)
                {
                    if (artifact.Wearer != null)
                    {                       
                        //Log.Message("" + artifact.LabelShort + " has holding owner " + artifact.Wearer.LabelShort);
                        if (artifact.Wearer.health.hediffSet.GetFirstHediffOfDef(hediff, false) != null)
                        {

                        }
                        else
                        {                            
                            HealthUtility.AdjustSeverity(artifact.Wearer, hediff, hediffSeverity);
                            HediffComp_EnchantedItem hdc = artifact.Wearer.health.hediffSet.GetFirstHediffOfDef(hediff, false).TryGetComp<HediffComp_EnchantedItem>();
                            if (hdc != null)
                            {
                                hdc.enchantedItem = artifact;
                            }
                            //HediffComp_EnchantedItem comp = diff.TryGetComp<HediffComp_EnchantedItem>();
                        }
                    }
                }
                Thing primary = parent as Thing;
                if (primary != null && primary.ParentHolder is Pawn_EquipmentTracker pet)
                {
                    if (pet.pawn != null && pet.pawn.equipment != null && pet.pawn.equipment.Primary == primary)
                    {
                        if (pet.pawn.health.hediffSet.GetFirstHediffOfDef(hediff, false) != null)
                        {

                        }
                        else
                        {
                            HealthUtility.AdjustSeverity(pet.pawn, hediff, hediffSeverity);
                            HediffComp_EnchantedItem hdc = pet.pawn.health.hediffSet.GetFirstHediffOfDef(hediff, false).TryGetComp<HediffComp_EnchantedItem>();
                            if (hdc != null)
                            {
                                hdc.enchantedWeapon = primary;
                            }
                        }
                    }
                }                
            }
            if (Props.hasAbility && !abilitiesInitialized)
            {
                Apparel artifact = parent as Apparel;
                if (artifact != null)
                {
                    if (artifact.Wearer != null)
                    {
                        //Log.Message("" + artifact.LabelShort + " has holding owner " + artifact.Wearer.LabelShort);
                        InitializeAbilities(artifact);                        
                    }

                    MagicAbilities = artifact.GetComp<CompAbilitiesApparel>().GivenAbilities;
                    //MagicAbilities = new List<AbilityDef>();
                    //MagicAbilities.Clear();
                    // abilities;
                }
                ThingWithComps primary = parent;
                if (primary != null && primary.ParentHolder is Pawn_EquipmentTracker pet)
                {
                    if (pet.pawn != null && pet.pawn.equipment != null && pet.pawn.equipment.Primary == primary)
                    {
                        InitializeAbilities(primary);
                    }
                    MagicAbilities = primary.GetComp<CompAbilitiesApparel>().GivenAbilities;
                }
            }
            if (GetEnchantedStuff_HediffDef != null)
            {
                if (WearingPawn != null)
                {
                    hediffStuff.Clear();
                    List<Apparel> wornApparel = WearingPawn.apparel.WornApparel;
                    for (int i = 0; i < wornApparel.Count; i++)
                    {
                        CompEnchantedItem itemComp = wornApparel[i].TryGetComp<CompEnchantedItem>();
                        if(itemComp != null && itemComp.GetEnchantedStuff_HediffDef != null)
                        {
                            int hdCount = GetStuffCount_Hediff(itemComp.EnchantedStuff.appliedHediff);
                            if (hdCount >= itemComp.EnchantedStuff.applyHediffAtCount)
                            {
                                if(WearingPawn.health.hediffSet.HasHediff(itemComp.EnchantedStuff.appliedHediff))
                                {
                                    Hediff hd = WearingPawn.health.hediffSet.GetFirstHediffOfDef(itemComp.EnchantedStuff.appliedHediff);
                                    if(hd.Severity < (hdCount * itemComp.EnchantedStuff.severityPerCount))
                                    {
                                        WearingPawn.health.RemoveHediff(hd);
                                        HealthUtility.AdjustSeverity(WearingPawn, itemComp.EnchantedStuff.appliedHediff, hdCount * itemComp.EnchantedStuff.severityPerCount);
                                    }
                                }
                                else
                                {
                                    HealthUtility.AdjustSeverity(WearingPawn, itemComp.EnchantedStuff.appliedHediff, hdCount * itemComp.EnchantedStuff.severityPerCount);
                                }
                            }
                        }
                    }
                    if (WearingPawn.equipment != null && WearingPawn.equipment.Primary != null && !EnchantedStuff.apparelOnly)
                    {
                        ThingWithComps eq = WearingPawn.equipment.Primary;
                        CompEnchantedItem itemComp = eq.TryGetComp<CompEnchantedItem>();
                        if (itemComp != null && itemComp.GetEnchantedStuff_HediffDef != null)
                        {
                            int hdCount = GetStuffCount_Hediff(itemComp.EnchantedStuff.appliedHediff);
                            if (hdCount >= itemComp.EnchantedStuff.applyHediffAtCount)
                            {
                                if (WearingPawn.health.hediffSet.HasHediff(itemComp.EnchantedStuff.appliedHediff))
                                {
                                    Hediff hd = WearingPawn.health.hediffSet.GetFirstHediffOfDef(itemComp.EnchantedStuff.appliedHediff);
                                    if (hd.Severity < (hdCount * itemComp.EnchantedStuff.severityPerCount))
                                    {
                                        WearingPawn.health.RemoveHediff(hd);
                                        HealthUtility.AdjustSeverity(WearingPawn, itemComp.EnchantedStuff.appliedHediff, hdCount * itemComp.EnchantedStuff.severityPerCount);
                                    }
                                }
                                else
                                {
                                    HealthUtility.AdjustSeverity(WearingPawn, itemComp.EnchantedStuff.appliedHediff, hdCount * itemComp.EnchantedStuff.severityPerCount);
                                }
                            }
                        }
                    }

                }
            }
            base.CompTickRare();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            bool flag = parent.def.tickerType == TickerType.Never;
            if (flag)
            {
                //parent.def.tickerType = TickerType.Rare;
                //Find.TickManager.RegisterAllTickabilityFor(parent);
            }
            base.PostSpawnSetup(respawningAfterLoad);
            
        }

        private Dictionary<HediffDef, int> hediffStuff = new Dictionary<HediffDef, int>();
        public int GetStuffCount_Hediff(HediffDef hd)
        {
            if (!hediffStuff.ContainsKey(hd))
            {
                hediffStuff.Add(hd, 1);
            }
            else
            {
                int count = 0;
                hediffStuff.TryGetValue(hd, out count);
                if(count != 0)
                {
                    hediffStuff.SetOrAdd(hd, count + 1);
                }
            }
            return hediffStuff[hd];
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref maxMP, "maxMP", 0, false);
            Scribe_Values.Look<float>(ref mpRegenRate, "mpRegenRateP", 0, false);
            Scribe_Values.Look<float>(ref coolDown, "coolDown", 0, false);
            Scribe_Values.Look<float>(ref mpCost, "mpCost", 0, false);
            Scribe_Values.Look<float>(ref xpGain, "xpGain", 0, false);
            Scribe_Values.Look<float>(ref arcaneRes, "arcaneRes", 0, false);
            Scribe_Values.Look<float>(ref arcaneDmg, "arcaneDmg", 0, false);
            Scribe_Values.Look<float>(ref necroticEnergy, "necroticEnergy", 0f, false);
            Scribe_Values.Look<bool>(ref arcaneSpectre, "arcaneSpectre", false, false);
            Scribe_Values.Look<bool>(ref phantomShift, "phantomShift", false, false);
            //Scribe_Deep.Look<EnchantmentAction>(ref enchantmentAction, "enchantmentAction", new object[0]);
            Scribe_Defs.Look<ThoughtDef>(ref enchantmentThought, "enchantmentThought");
            Scribe_Values.Look<float>(ref arcalleumCooldown, "arcalleumCooldown", 0f, false);
            Scribe_Values.Look<EnchantmentTier>(ref maxMPTier, "maxMPTier", (EnchantmentTier)0, false);
            Scribe_Values.Look<EnchantmentTier>(ref mpRegenRateTier, "mpRegenRateTier", (EnchantmentTier)0, false);
            Scribe_Values.Look<EnchantmentTier>(ref coolDownTier, "coolDownTier", (EnchantmentTier)0, false);
            Scribe_Values.Look<EnchantmentTier>(ref mpCostTier, "mpCostTier", (EnchantmentTier)0, false);
            Scribe_Values.Look<EnchantmentTier>(ref xpGainTier, "xpGainTier", (EnchantmentTier)0, false);
            Scribe_Values.Look<EnchantmentTier>(ref arcaneResTier, "arcaneResTier", (EnchantmentTier)0, false);
            Scribe_Values.Look<EnchantmentTier>(ref arcaneDmgTier, "arcaneDmgTier", (EnchantmentTier)0, false);
            Scribe_Values.Look<bool>(ref hasEnchantment, "hasEnchantment", false, false);
            Scribe_Values.Look<bool>(ref initialized, "initialized", false, false);
            Scribe_Collections.Look<Trait>(ref SoulOrbTraits, "SoulOrbTraits", LookMode.Deep);
            Scribe_Values.Look<int>(ref mightEssence, "mightEssence", 0, false);
            Scribe_Values.Look<int>(ref magicEssence, "magicEssence", 0, false);
            //Props.ExposeData();
        }

        public override string GetDescriptionPart()
        {
            string text = string.Empty;
            if (Props.MagicAbilities.Count == 1)
            {
                text += "Item Ability:";
            }
            else
            {
                if (Props.MagicAbilities.Count > 1)
                {
                    text += "Item Abilities:";
                }
            }
            foreach (TMAbilityDef current in Props.MagicAbilities)
            {
                text += "\n\n";
                text = text + current.label.CapitalizeFirst() + " - ";
                text += current.description;  // TODO Jecstools: This may need to be Ability.GetDescriptionForPawn();
            }
            bool flag3 = SoulOrbTraits != null && SoulOrbTraits.Count > 0;
            if (flag3)
            {
                text += "Absorbed Traits:";
                foreach (Trait current in SoulOrbTraits)
                {
                    text += "\n";
                    text = text + current.LabelCap;
                }

            }
            bool flag4 = necroticEnergy != 0;
            if(flag4)
            {
                text += "Necrotic Energy: " + NecroticEnergy.ToString("N1");
            }
            bool flag5 = mightEssence != 0;
            if(flag5)
            {
                text += "Might Essence: " + mightEssence;
            }
            bool flag6 = magicEssence != 0;
            if (flag6)
            {
                text += "Magic Essence: " + magicEssence;
            }
            return text;
        }

        private bool initialized = false;
        private bool abilitiesInitialized = false;
        private bool hasEnchantment = false;

        public EnchantmentTier maxMPTier;
        public EnchantmentTier mpRegenRateTier;
        public EnchantmentTier coolDownTier;
        public EnchantmentTier mpCostTier;
        public EnchantmentTier xpGainTier;
        public EnchantmentTier arcaneResTier;
        public EnchantmentTier arcaneDmgTier;

        //Magic Stats (%)
        public float maxMP = 0;
        public float mpRegenRate = 0;
        public float coolDown = 0;
        public float mpCost = 0;
        public float xpGain = 0;

        public float arcaneRes = 0;
        public float arcaneDmg = 0;

        //Might Stats (%)

        //Common Stats (%)        

        public float healthRegenRate = 0;
        private float necroticEnergy = 0f;

        //Special Abilities
        public EnchantmentAction enchantmentAction = null;
        public EnchantmentTier skillTier = EnchantmentTier.Skill;
        public bool arcaneSpectre = false;
        public bool phantomShift = false;

        public float arcalleumCooldown = 0f;

        public int mightEssence = 0;
        public int magicEssence = 0;

        //Hediffs
        public HediffDef hediff = null;
        public float hediffSeverity = 0f;

        //Abilities

        //Thoughts
        public ThoughtDef enchantmentThought = null;

        public float NecroticEnergy
        {
            get
            {
                return Mathf.Clamp(necroticEnergy, 0f, 100f);
            }
            set
            {
                necroticEnergy = Mathf.Clamp(value, 0f, 100f);
            }
        }

        private float StuffMultiplier
        {
            get
            {
                //if(parent.Stuff != null && parent.Stuff.defName == "TM_Manaweave")
                //{
                //    return 120f;
                //}
                if(parent.Stuff != null && MadeFromEnchantedStuff && EnchantedStuff.enchantmentBonusMultiplier != 1f)
                {
                    return 100f * EnchantedStuff.enchantmentBonusMultiplier;
                }
                else
                {
                    return 100f;
                }
            }
        }

        public string MaxMPLabel
        {
            get
            {
                return "TM_MaxMPLabel".Translate(
                    maxMP * StuffMultiplier
                );
            }
        }

        public string MPRegenRateLabel
        {
            get
            {
                return "TM_MPRegenRateLabel".Translate(
                    mpRegenRate * StuffMultiplier
                );
            }
        }

        public string CoolDownLabel
        {
            get
            {
                return "TM_CoolDownLabel".Translate(
                    coolDown * StuffMultiplier
                );
            }
        }

        public string MPCostLabel
        {
            get
            {
                return "TM_MPCostLabel".Translate(
                    mpCost * StuffMultiplier
                );
            }
        }

        public string XPGainLabel
        {
            get
            {
                return "TM_XPGainLabel".Translate(
                    xpGain * StuffMultiplier
                );
            }
        }

        public string ArcaneResLabel
        {
            get
            {
                return "TM_ArcaneResLabel".Translate(
                    arcaneRes * StuffMultiplier
                );
            }
        }

        public string ArcaneDmgLabel
        {
            get
            {
                return "TM_ArcaneDmgLabel".Translate(
                    arcaneDmg * StuffMultiplier
                );
            }
        }

        public string ArcaneSpectreLabel
        {
            get
            {
                return "TM_ArcaneSpectre".Translate();
            }
        }

        public string PhantomShiftLabel
        {
            get
            {
                return "TM_PhantomShift".Translate();
            }
        }

        public string ArcalleumCooldownLabel
        {
            get
            {
                return "TM_ArcalleumCooldown".Translate(
                    arcalleumCooldown);
            }
        }

        public string EnchantmentActionLabel
        {
            get
            {
                if (enchantmentAction.type == EnchantmentActionType.ApplyHediff)
                {
                    return "TM_EA_Hediff".Translate(
                        enchantmentAction.actionLabel,
                        enchantmentAction.hediffDef.label,
                        enchantmentAction.hediffChance * 100f);
                }
                if(enchantmentAction.type == EnchantmentActionType.ApplyDamage)
                {
                    return "TM_EA_Damage".Translate(
                        enchantmentAction.actionLabel,
                        enchantmentAction.damageAmount - enchantmentAction.damageVariation,
                        enchantmentAction.damageAmount + enchantmentAction.damageVariation,
                        enchantmentAction.damageDef.label,
                        enchantmentAction.damageChance * 100f);
                }
                return "";
            }
        }

        public string HediffLabel
        {
            get
            {
                return hediff.LabelCap;
            }
        }

        public bool HasMagic
        {
            get
            {
                return MagicAbilities.Count > 0;
            }
        }

        public EnchantmentTier SetTier(float mod)
        {
            if (mod < 0)
            {
                return EnchantmentTier.Negative;
            }
            if (mod <= .05f)
            {
                return EnchantmentTier.Minor;
            }
            else if (mod <= .1f)
            {
                return EnchantmentTier.Standard;
            }
            else if (mod <= .15f)
            {
                return EnchantmentTier.Major;
            }
            else if (mod > .15f)
            {
                return EnchantmentTier.Crafted;
            }
            else
            {
                return EnchantmentTier.Undefined;
            }
        }

        public bool HasEnchantment
        {
            get
            {
                return hasEnchantment;
            }
            set
            {
                hasEnchantment = value;
            }
        }        
    }
}
