using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public abstract class BattleParticipant : Participant
    {
        private int max_hp;
        private int current_hp;
        private float defense;
        private float dodge;
        private List<KeyValuePair<string, BattleEffect>> statuses;
        private Dictionary<string, Skill> passiveSkills;
        private Dictionary<string, Skill> utilitySkills;
        private Dictionary<string, Skill> attackSkills;



        protected BattleParticipant() : base()
        {
            this.max_hp = 50;
            this.current_hp = 50;
            this.defense = 0.0f;
            this.dodge = 0.0f;
            this.statuses = new List<KeyValuePair<string, BattleEffect>>();

        }

        protected BattleParticipant(p_type type, string name) : base(type, name)
        {
            this.max_hp = 50;
            this.current_hp = 50;
            this.defense = 0.0f;
            this.dodge = 0.0f;
            this.statuses = new List<KeyValuePair<string, BattleEffect>>();
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

        public List<KeyValuePair<string, BattleEffect>> Statuses
        {
            get => statuses;
            set => statuses = value;
        }
        public Dictionary<string, Skill> PassiveSkills { get => passiveSkills; set => passiveSkills = value; }
        public Dictionary<string, Skill> UtilitySkills { get => utilitySkills; set => utilitySkills = value; }
        public Dictionary<string, Skill> AttackSkills { get => attackSkills; set => attackSkills = value; }

        public void addPassive(Skill passive)
        {
            this.passiveSkills.Add(passive.Name, passive);
        }
        public void addUtility(Skill util)
        {
            this.utilitySkills.Add(util.Name, util);
        }
        public void addAttack(Skill attack)
        {
            this.attackSkills.Add(attack.Name, attack);
        }
    }
}