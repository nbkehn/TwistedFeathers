using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public enum Skill_Type
    {
        None,
        Attack,
        Utility,
        Passive
    }

    [System.Serializable]
    public class Skill
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private int id;
        [SerializeField]
        private int dependency;
        [SerializeField]
        private string description;
        [SerializeField]
        private Skill_Type skillType;
        private p_type user_type;
        [SerializeField]
        private bool unlocked;
        [SerializeField]
        private bool selected;
        [SerializeField]
        private int level_req;
        private Skill pre_req;
        [SerializeField]
        private List<BattleEffect> effects;
        [SerializeField]
        private List<BattleEffect> passives;

        // For use by skill editor only
        public Skill(int id)
        {
            this.ID = id;
            this.effects = new List<BattleEffect>();
            this.passives = new List<BattleEffect>();
        }

        public Skill()
        {
            this.name = "";
            this.description = null;
            this.user_type = p_type.none;
            this.unlocked = false;
            this.level_req = 0;
            this.pre_req = null;
            this.effects = new List<BattleEffect>();
            this.passives = new List<BattleEffect>();
        }

        public Skill(string name, string description, p_type user_type, List<BattleEffect> effects)
        {
            this.name = name;
            this.description = description;
            this.user_type = user_type;
            this.unlocked = false;
            this.level_req = 0;
            this.pre_req = null;
            this.effects = effects;
            this.passives = new List<BattleEffect>();
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int ID
        {
            get => id;
            set => id = value;
        }

        public int Dependency
        {
            get => dependency;
            set => dependency = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public Skill_Type SkillType
        {
            get => skillType;
            set => skillType = value;
        }

        public p_type User_type
        {
            get => user_type;
            set => user_type = value;
        }

        public List<BattleEffect> Effects
        {
            get => effects;
            set => effects = value;
        }

        public bool Unlocked
        {
            get => unlocked;
            set => unlocked = value;
        }

        public bool Selected
        {
            get => selected;
            set => selected = value;
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
      
        public List<BattleEffect> Passives { get => passives; set => passives = value; }

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
