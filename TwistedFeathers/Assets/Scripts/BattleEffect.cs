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

    public enum stat_type
    {
        nothing,
        attack,
        defense,
        accuracy,
        dodge
    }

    public class BattleEffect
    {
        // These values are defined when stored in a skill
        private e_type type;
        private float modifier;
        private float duration;
        private string specifier;

        // These values are only defined when it is selected in battle
        private List<BattleParticipant> target;
        private Participant user;
        private int turnstamp;
        /*Whether or not the effect will show up in the forecast*/
        private bool visible;

        public BattleEffect()
        {
            this.type = e_type.nothing;
            this.modifier = 0f;
            this.Duration = 0f;
            this.specifier = "This is an effect that works";
            this.target = null;
            this.user = null;
            this.turnstamp = 0;
            this.visible = true;
        }

        public BattleEffect(e_type type, float modifier, string specifier)
        {
            this.type = type;
            this.modifier = modifier;
            this.Duration = 0f;
            this.specifier = specifier;
            this.target = null;
            this.user = null;
            this.turnstamp = 0;
            this.visible = true;
        }

        public BattleEffect(e_type type, float modifier, float duration, string specifier, List<BattleParticipant> target, Participant user, int turnstamp, bool visible)
        {
            this.type = type;
            this.modifier = modifier;
            this.duration = duration;
            this.specifier = specifier;
            this.target = target;
            this.user = user;
            this.turnstamp = turnstamp;
            this.visible = visible;
        }

        public e_type Type
        {
            get => type;
            set => type = value;
        }

        public List<BattleParticipant> Target
        {
            get => target;
            set => target = value;
        }

        public Participant User
        {
            get => user;
            set => user = value;
        }

        public string Specifier
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
        public float Duration { get => duration; set => duration = value; }
        public bool Visible { get => visible; set => visible = value; }

        public void select(BattleParticipant user, List<BattleParticipant> target, int turnstamp)
        {
            User = user;
            Target = target;
            Turnstamp = turnstamp;
        }

        public void run(SortedSet<BattleEffect> pq)
        {
            bool check_hit = true;
            foreach (BattleParticipant tar in target)
            {
                //Check to see if effect actually hits target
                if (user.Type != tar.Type)
                {
                    float random_dodge = Random.Range(0.0f, 1.0f);
                    if ((tar.Dodge-user.Accuracy) >= random_dodge)
                    {
                        //Miss!
                        check_hit = false;
                    }
                }

                if (check_hit)
                {
                    Debug.Log(Specifier);
                    switch (Type)
                    {
                        case (e_type.damage):
                            // Missing flat mod additions
                            float damage = modifier + (modifier * user.Attack) - (modifier * tar.Defense);
                            tar.Current_hp = (int)(tar.Current_hp - damage);
                            break;
                        case (e_type.buff):
                            switch (Specifier)
                            {
                                case ("attack"):
                                    user.Attack += modifier;
                                    break;
                                case ("defense"):
                                    tar.Defense += modifier;
                                    break;
                                case ("accuracy"):
                                    tar.Accuracy += modifier;
                                    break;
                                case ("dodge"):
                                    tar.Dodge += modifier;
                                    break;
                                default:
                                    break;
                            }
                            if (duration > 0)
                            {
                                pq.Add(new BattleEffect(e_type.buff, -modifier, 0f, specifier, new List<BattleParticipant>() { tar }, tar, (int) (turnstamp + duration), false));
                            }
                            break;
                        case (e_type.status):
                            tar.Statuses.Add(new KeyValuePair<string, BattleEffect>(specifier, this));
                            break;
                        default:
                            //This is where special/unique effects need to be handled
                            break;
                    }
                }
                else
                {
                    //Miss case
                    Debug.Log(user.Name + " Missed!");
                }
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

