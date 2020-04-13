using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public enum SkillType { None, Passive, Utility, Attack}

    public class Skill
    {
        private string name;
        private string description;
        private p_type user_type;
        private SkillType skill_type;
        bool unlocked;
        int level_req;
        Skill pre_req;
        private List<BattleEffect> effect;
        private List<BattleEffect> passive;

        public Skill()
        {
            this.name = "";
            this.description = null;
            this.user_type = p_type.none;
            this.unlocked = false;
            this.level_req = 0;
            this.pre_req = null;
            this.effect = new List<BattleEffect>();
            this.passive = new List<BattleEffect>();
        }

        public Skill(string name, string description, p_type user_type, List<BattleEffect> effect)
        {
            this.name = name;
            this.description = description;
            this.user_type = user_type;
            this.unlocked = false;
            this.level_req = 0;
            this.pre_req = null;
            this.effect = effect;
            this.passive = new List<BattleEffect>();
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public p_type User_type
        {
            get => user_type;
            set => user_type = value;
        }

        public List<BattleEffect> Effect
        {
            get => effect;
            set => effect = value;
        }

        public bool Unlocked
        {
            get => unlocked;
            set => unlocked = value;
        }

        public int Level_req
        {
            get => level_req;
            set => level_req = value;
        }

        public Skill Pre_req
        {
            get => pre_req;
            set => pre_req = value;
        }
        public List<BattleEffect> Passive { get => passive; set => passive = value; }
        public SkillType Skill_type { get => skill_type; set => skill_type = value; }

        // This method gets called when the skill is gained, it applies all the passives
        void onGain()
        {
            
        }

        // This method gets called if/when the skill is lost, it removes all the passives
        void onLose()
        {

        }
    }

}
