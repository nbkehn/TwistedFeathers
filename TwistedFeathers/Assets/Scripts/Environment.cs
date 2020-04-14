using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwistedFeathers;

namespace TwistedFeathers
{
    public class Environment : Participant
    {
        public GameObject miniIcon;
        public int envListIndex;
        private List<KeyValuePair<int, Skill>> skills;

        public Environment() : base(p_type.environment, s_type.None)
        {

        }

        public Environment(s_type name, GameObject Icon) : base(p_type.environment, name)
        {
            this.skills = new List<KeyValuePair<int, Skill>>();
            miniIcon = Icon;
            Debug.Log(miniIcon);
            if (name == s_type.Swamp)
            {
                envListIndex = 1;
            }

            if (name == s_type.Desert)
            {
                envListIndex = 0;
            }
        }

        public Environment(s_type name, GameObject Icon, List<KeyValuePair<int, Skill>> skills) : base(p_type.environment, name)
        {
            this.skills = skills;
            miniIcon = Icon;
            Debug.Log(miniIcon);
            if (name == s_type.Swamp)
            {
                envListIndex = 1;
            }

            if (name == s_type.Desert)
            {
                envListIndex = 0;
            }
        }

        public new List<KeyValuePair<int, Skill>> Skills { get => skills; set => skills = value; }
    }
}
