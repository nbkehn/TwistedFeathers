using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TwistedFeathers
{
    public abstract class BattleParticipant : Participant
    {
        private int max_hp;
        private int current_hp;
        private float defense;
        private float dodge;
        private Dictionary<string, Skill> passiveSkills;
        private Dictionary<string, Skill> utilitySkills;
        private Dictionary<string, Skill> attackSkills;
        private List<BattleEffect> statuses;
        private List<BattleEffect> buffs;

        protected BattleParticipant() : base()
        {
            this.max_hp = 50;
            this.current_hp = 50;
            this.defense = 0.0f;
            this.dodge = 0.0f;
            this.passiveSkills = new Dictionary<string, Skill>();
            this.utilitySkills = new Dictionary<string, Skill>();
            this.attackSkills = new Dictionary<string, Skill>();
            this.statuses = new List<BattleEffect>();
            this.buffs = new List<BattleEffect>();
            LoadSkillTree();

        }

        protected BattleParticipant(p_type type, s_type name) : base(type, name)
        {
            this.max_hp = 50;
            this.current_hp = 50;
            this.defense = 0.0f;
            this.dodge = 0.0f;
            this.passiveSkills = new Dictionary<string, Skill>();
            this.utilitySkills = new Dictionary<string, Skill>();
            this.attackSkills = new Dictionary<string, Skill>();
            this.statuses = new List<BattleEffect>();
            this.buffs = new List<BattleEffect>();
            LoadSkillTree();
        }

        public int Max_hp
        {
            get => max_hp;
            set => max_hp = value;
        }

        public int Current_hp
        {
            get => current_hp;
            set => current_hp = value;
        }

        public float Defense
        {
            get => defense;
            set => defense = value;
        }

        public float Dodge
        {
            get => dodge;
            set => dodge = value;
        }

        public List<BattleEffect> Statuses
        {
            get => statuses;
            set => statuses = value;
        }
        public Dictionary<string, Skill> PassiveSkills { get => passiveSkills; set => passiveSkills = value; }
        public Dictionary<string, Skill> UtilitySkills { get => utilitySkills; set => utilitySkills = value; }
        public Dictionary<string, Skill> AttackSkills { get => attackSkills; set => attackSkills = value; }

        public void addPassive(Skill passive)
        {
            if (!this.passiveSkills.ContainsKey(passive.Name))
            {
                this.passiveSkills.Add(passive.Name, passive);
            }
        }
        public void addUtility(Skill util)
        {
            if (!this.utilitySkills.ContainsKey(util.Name))
            {
                this.utilitySkills.Add(util.Name, util);
            }
        }

        public void addAttack(Skill attack)
        {
            if (!this.attackSkills.ContainsKey(attack.Name))
            {
                this.attackSkills.Add(attack.Name, attack);
            }
        }

        public List<BattleEffect> Buffs { get => buffs; set => buffs = value; }

        public float getStat(stat_type type)
        {
            switch (type)
            {
                case stat_type.HP:
                    return this.Current_hp;
                    break;
                case stat_type.Attack:
                    return this.Attack;
                    break;
                case stat_type.Defense:
                    return this.Defense;
                    break;
                case stat_type.Accuracy:
                    return this.Accuracy;
                    break;
                case stat_type.Dodge:
                    return this.Dodge;
                    break;
                default:
                    Debug.LogError("BattleParticipant getStat: invalid stat type");
                    return 0;
                    break;
            }
        }

        public float getStat(string type)
        {
            switch (type)
            {
                case "hp":
                    return this.Current_hp;
                    break;
                case "attack":
                    return this.Attack;
                    break;
                case "defense":
                    return this.Defense;
                    break;
                case "accuracy":
                    return this.Accuracy;
                    break;
                case "dodge":
                    return this.Dodge;
                    break;
                default:
                    Debug.LogError("BattleParticipant getStat: invalid stat type");
                    return 0;
                    break;
            }
        }

        public new void LoadSkillTree()
        {
            string path = Application.streamingAssetsPath + "/Scripts/SkillEditor/Data/" + name.ToString() + ".json";

            string dataAsJson;
            if (File.Exists(path))
            {
                // Read the json from the file into a string
                dataAsJson = File.ReadAllText(path);

                // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
                SkillTree skillData = JsonUtility.FromJson<SkillTree>(dataAsJson);

                // Store the SkillTree as an array of Skill
                skillTree = new Skill[skillData.skilltree.Length];
                skillTree = skillData.skilltree;

                for (int i = 0; i < skillTree.Length; i++)
                {
                    if (skillTree[i].Dependency > -1)
                    {
                        skillTree[i].Pre_req = skillTree[skillTree[i].Dependency];
                    }
                    skillTree[i].User_type = this.type;
                    if (skillTree[i].Selected)
                    {
                        this.skills.Add(skillTree[i]);
                        if (skillTree[i].SkillType == Skill_Type.Attack)
                        {
                            this.AttackSkills.Add(skillTree[i].Name, skillTree[i]);
                            Debug.Log("New Attack Skill: " + skillTree[i].Name);
                        }
                        else if (skillTree[i].SkillType == Skill_Type.Utility)
                        {
                            this.UtilitySkills.Add(skillTree[i].Name, skillTree[i]);
                            Debug.Log("New Utility Skill: " + skillTree[i].Name);
                        }
                        else if (skillTree[i].SkillType == Skill_Type.Passive)
                        {
                            //TODO CODE FOR APPLYING PASSIVE HERE
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("A skill tree does not exist for: " + name.ToString());
            }
        }

        /*
         * Resets stats back to their default values
         * For use after a battle
         */
        public void resetStats()
        {
            this.current_hp = this.max_hp;
            this.Attack = 0;
            this.defense = 0;
            this.Accuracy = 0;
            this.dodge = 0;
        }

        public string displayStatuses()
        {
            string message = "Active Status Effects:";
            foreach (BattleEffect status in Statuses)
            {
                message += "\n\tName: " + status.Specifier + "\n\tDuration: " + status.Duration;
            }
            return message;
        }

        public string displayBuffs()
        {
            string message = "Active Buffs and Debuffs";
            foreach (BattleEffect buff in Buffs)
            {
                message += "\n\tStat: " + buff.Specifier + "\n\tModifier: " + (buff.Modifier * -100) + "%\n\tDuration: " + buff.Duration;
            }
            return message;
        }
    }
}