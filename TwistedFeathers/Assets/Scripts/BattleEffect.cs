using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{


    public enum e_type
    {
        nothing,
        damage,
        status,
        buff
    };

    public class BattleEffect
    {
        // These values are defined when stored in a skill
        private e_type type;
        private float modifier;

        private string specifier;

        // These values are only defined when it is selected in battle
        private BattleParticipant target;
        private BattleParticipant user;
        private int turnstamp;

        public BattleEffect()
        {
            this.type = e_type.nothing;
            this.modifier = 0f;
            this.specifier = "This is an effect that works";
            this.target = null;
            this.user = null;
            this.turnstamp = 0;
        }

        public BattleEffect(e_type type, float modifier, string specifier)
        {
            this.type = type;
            this.modifier = modifier;
            this.specifier = specifier;
            this.target = null;
            this.user = null;
            this.turnstamp = 0;
        }


        public e_type Type
        {
            get => type;
            set => type = value;
        }

        public BattleParticipant Target
        {
            get => target;
            set => target = value;
        }

        public BattleParticipant User
        {
            get => user;
            set => user = value;
        }

        public string Message
        {
            get => specifier;
            set => specifier = value;
        }

        public int Turnstamp
        {
            get => turnstamp;
            set => turnstamp = value;
        }

        public float Modifier
        {
            get => modifier;
            set => modifier = value;
        }


        public void select(BattleParticipant user, BattleParticipant target, int turnstamp)
        {
            User = user;
            Target = target;
            Turnstamp = turnstamp;
        }

        public void run()
        {
            if (user.Type != target.Type)
            {
                float random_dodge = Random.Range(0.0f, 1.0f);
                if (target.Dodge >= random_dodge)
                {
                    //Miss!
                }
            }

            Debug.Log(Message);
            switch (Type)
            {
                case (e_type.damage):
                    // Missing flat mod additions
                    float damage = modifier + (modifier * user.Attack) - (modifier * target.Defense);
                    target.Current_hp = (int) (target.Current_hp - damage);
                    break;
                case (e_type.status):
                    target.Statuses.Add(new KeyValuePair<string, BattleEffect>(specifier, this));
                    break;
                default:
                    //This is where special/unique effects need to be handled
                    break;
            }
        }

    }


    public class EffectComparator : IComparer<BattleEffect>
    {
        public int Compare(BattleEffect x, BattleEffect y)
        {

            if (x.Turnstamp != y.Turnstamp)
            {
                return x.Turnstamp.CompareTo(y.Turnstamp);
            }
            else if (x.User.Type != y.User.Type)
            {
                return x.User.Type.CompareTo(y.User.Type);
            }
            else if (x.Type != y.Type)
            {
                return x.Type.CompareTo(y.Type);
            }
            else
            {
                return 0;
            }
        }
    }
}

