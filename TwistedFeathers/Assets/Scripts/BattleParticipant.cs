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
        private List<BattleEffect> statuses;
        private List<BattleEffect> buffs;

        protected BattleParticipant() : base()
        {
            this.max_hp = 50;
            this.current_hp = 50;
            this.defense = 0.0f;
            this.dodge = 0.0f;
            this.statuses = new List<BattleEffect>();
            this.buffs = new List<BattleEffect>();

        }

        protected BattleParticipant(p_type type, string name) : base(type, name)
        {
            this.max_hp = 50;
            this.current_hp = 50;
            this.defense = 0.0f;
            this.dodge = 0.0f;
            this.statuses = new List<BattleEffect>();
            this.buffs = new List<BattleEffect>();
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
        public List<BattleEffect> Buffs { get => buffs; set => buffs = value; }

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