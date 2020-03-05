using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public enum p_type
    {
        none,
        player,
        enemy,
        environment,
        weather
    };

    public abstract class Participant
    {
        private p_type type;
        private string name;
        private List<Skill> skills;
        public GameObject myPrefab;
        public GameObject me;

        protected Participant()
        {
            this.type = p_type.none;
            this.name = "";
            this.skills = new List<Skill>();
        }

        protected Participant(p_type type, string name)
        {
            this.type = type;
            this.name = name;
            this.skills = new List<Skill>();
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

        public string Name
        {
            get => name;
            set => name = value;
        }

        public void AddSkill(Skill new_skill)
        {
            this.skills.Add(new_skill);
        }
    }
}

