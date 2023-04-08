using Verse;

namespace TorannMagic;

public class MagicPowerSkill : IExposable
{
    public string label;
    public string desc;
    public int level;
    public int levelMax;
    public int costToLevel = 1;

    public MagicPowerSkill(string newLabel, string newDesc)
    {
        label = newLabel;
        desc = newDesc;
        level = 0;

        if(label is "TM_HolyWrath_ver" or "TM_HolyWrath_pwr" or "TM_Sentinel_pwr" or "TM_EnchanterStone_ver" or "TM_Polymorph_ver" or "TM_AlterFate_pwr" or "TM_LightSkip_pwr" or "TM_RuneCarving_pwr" || label.Contains("TM_BardTraining") || label.Contains("TM_Shapeshift") || label.Contains("TM_ChaosTradition"))
        {
            costToLevel = 2;
        }

        if (newLabel == "TM_Firebolt_pwr")
        {
            levelMax = 6;
        }
        else if (newLabel is "TM_global_regen_pwr" or "TM_global_eff_pwr" or "TM_EarthSprites_pwr" or "TM_Prediction_pwr" or "TM_GuardianSpirit_pwr" or "TM_Golemancy_pwr" or "TM_Golemancy_eff" or "TM_Golemancy_ver")
        {
            levelMax = 5;
        }
        else if (newLabel is "TM_Blink_eff" or "TM_Summon_eff" or "TM_AdvancedHeal_pwr" or "TM_AdvancedHeal_ver" or "TM_HealingCircle_pwr")
        {
            levelMax = 4;
        }
        else if (newLabel == "TM_global_spirit_pwr")
        {
            levelMax = 50;
        }
        else if (newLabel is "TM_TechnoBit_pwr" or "TM_TechnoBit_ver" or "TM_TechnoBit_eff" or "TM_TechnoTurret_pwr" or "TM_TechnoTurret_ver" or "TM_TechnoTurret_eff" or "TM_TechnoWeapon_pwr" or "TM_TechnoWeapon_ver" or "TM_TechnoWeapon_eff" or "TM_Cantrips_pwr" or "TM_Cantrips_eff" or "TM_Cantrips_ver" or "TM_Totems_pwr" or "TM_Totems_eff" or "TM_Totems_ver" or "TM_SpiritOfLight_pwr" or "TM_SpiritOfLight_eff" or "TM_SpiritOfLight_ver" or "TM_Cantrips_pwr" or "TM_Cantrips_eff" or "TM_Cantrips_ver")
        {
            levelMax = 15;
        }
        else if (newLabel is "TM_WandererCraft_pwr" or "TM_WandererCraft_eff" or "TM_WandererCraft_ver")
        {
            levelMax = 30;
        }
        else if (newLabel is "TM_Sentinel_pwr" or "TM_LightSkip_pwr")
        {
            levelMax = 2;
        }
        else
        {
            levelMax = 3;
        }
    }

    public void ExposeData()
    {
        Scribe_Values.Look<string>(ref label, "label", "default");
        Scribe_Values.Look<string>(ref desc, "desc", "default");
        Scribe_Values.Look<int>(ref level, "level");
        Scribe_Values.Look<int>(ref costToLevel, "costToLevel", 1);
        Scribe_Values.Look<int>(ref levelMax, "levelMax");
    }

}
