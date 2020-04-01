using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
        private float attack;
        private float accuracy;
        private Skill[] skillTree;
        private List<Skill> skills;
        public GameObject myPrefab;
        public GameObject me;

        protected Participant()
        {
            this.type = p_type.none;
            this.name = "";
            this.attack = 0f;
            this.accuracy = 0f;
            this.skills = new List<Skill>();
        }

        protected Participant(p_type type, string name)
        {
            this.type = type;
            this.name = name;
            this.attack = 0f;
            this.accuracy = 0f;
            this.skills = new List<Skill>();
        }
        protected Participant(p_type type, string name, List<Skill> skills)
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

        public string Name
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

        public void LoadSkillTree(string path)
        {
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
                    }
                }
            }
        }
    }
}

