using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TwistedFeathers;

namespace TwistedFeathers
{
    public enum p_type
    {
        none,
        player,
        enemy,
        environment,
        weather
    }

    // Specific Type
    public enum s_type
    {
        None,
        Rogue,
        Fighter,
        Mage,
        Necromancer,
        Thief,
        Swamp,
        Desert
    }

    public abstract class Participant
    {
        protected p_type type;
        //private string name;
        protected s_type name;
        protected float attack;
        protected float accuracy;
        protected Skill[] skillTree;
        protected List<Skill> skills;
        public GameObject myPrefab;
        public GameObject me;

        protected Participant()
        {
            this.type = p_type.none;
            this.name = s_type.None;
            this.attack = 0f;
            this.accuracy = 0f;
            this.skills = new List<Skill>();
        }

        protected Participant(p_type type, s_type name)
        {
            this.type = type;
            this.name = name;
            this.attack = 0f;
            this.accuracy = 0f;
            this.skills = new List<Skill>();
        }
        protected Participant(p_type type, s_type name, List<Skill> skills)
        {
            this.type = type;
            this.name = name;
            this.attack = 0f;
            this.accuracy = 0f;
            this.skills = skills;
        }

        public p_type Type
        {
            get => type;
            set => type = value;
        }

        public List<Skill> Skills
        {
            get => skills;
            set => skills = value;
        }

        public Skill[] SkillTree
        {
            get => skillTree;
            set => skillTree = value;
        }

        public s_type Name
        {
            get => name;
            set => name = value;
        }
        public float Attack { get => attack; set => attack = value; }
        public float Accuracy { get => accuracy; set => accuracy = value; }

        public void AddSkill(Skill new_skill)
        {
            this.skills.Add(new_skill);
        }


    }
}

