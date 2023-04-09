using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;


namespace TorannMagic.TMDefs
{
    public class TM_CustomPower
    {
        //Abilities
        public List<VFECore.Abilities.AbilityDef> abilityDefs;

        //Skills
        public List<TM_CustomSkill> skills;

        //Autocast features
        public TM_Autocast autocasting;

        //Application
        public bool forMage = false;
        public bool forFighter = false;
        public int maxLevel;
        public int learnCost;
        public bool requiresScroll;
        public bool chaosMageUseable = false;
        public int costToLevel;

    }
}
