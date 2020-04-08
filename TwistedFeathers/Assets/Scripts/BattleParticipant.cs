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

        protected BattleParticipant(p_type type, s_type name) : base(type, name)
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