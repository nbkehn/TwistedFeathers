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
        // These values are only defined when it is selected in battle
        public e_type Type { get; set; }
        public List<BattleParticipant> Target { get; set; }
        public Participant User { get; set; }
        // These values are defined when stored in a skill
        public string Specifier { get; set; }
        public int Turnstamp { get; set; }
        public float Modifier { get; set; }
        public int Duration { get; set; }
        /*Whether or not the effect will show up in the forecast*/
        public bool Visible { get; set; }
        public string SkillName { get; set; }

        public BattleEffect()
        {
            this.SkillName = ""; 
            this.Type = e_type.nothing;
            this.Modifier = 0f;
            this.Duration = 0;
            this.Specifier = "DEFAULT SPECIFIER";
            this.Target = null;
            this.User = null;
            this.Turnstamp = 0;
            this.Visible = true;
        }

        public BattleEffect(e_type type, float modifier, string specifier)
        {
            this.SkillName = "";
            this.Type = type;
            this.Modifier = modifier;
            this.Duration = 0;
            this.Specifier = specifier;
            this.Target = null;
            this.User = null;
            this.Turnstamp = 0;
            this.Visible = true;
        }

        public BattleEffect(e_type type, float modifier, int duration, string specifier)
        {
            this.SkillName = "";
            this.Type = type;
            this.Modifier = modifier;
            this.Duration = duration;
            this.Specifier = specifier;
            this.Target = null;
            this.User = null;
            this.Turnstamp = 0;
            this.Visible = true;
        }

        public BattleEffect(e_type type, float modifier, int duration, string specifier, int turnstamp)
        {
            this.SkillName = "";
            this.Type = type;
            this.Modifier = modifier;
            this.Duration = duration;
            this.Specifier = specifier;
            this.Target = null;
            this.User = null;
            this.Turnstamp = turnstamp;
            this.Visible = true;
        }

        public BattleEffect(string skill_name, e_type type, float modifier, int duration, string specifier, List<BattleParticipant> target, Participant user, int turnstamp, bool visible)
        {
            this.SkillName = skill_name;
            this.Type = type;
            this.Modifier = modifier;
            this.Duration = duration;
            this.Specifier = specifier;
            this.Target = target;
            this.User = user;
            this.Turnstamp = turnstamp;
            this.Visible = visible;
        }


        public void select(Participant user, List<BattleParticipant> target, int turnstamp, string skill_name)
        {
            Debug.Log("Turnstamp" + Turnstamp);
            User = user;
            Target = target;
            Turnstamp = turnstamp;
            SkillName = skill_name;
        }

        public void run(SortedSet<BattleEffect> pq)
        {
            bool check_hit = true;
            foreach (BattleParticipant tar in Target)
            {
                //Check to see if effect actually hits target
                if (User.Type != tar.Type)
                {
                    float random_dodge = Random.Range(0.0f, 1.0f);
                    if ((tar.Dodge-User.Accuracy) >= random_dodge)
                    {
                        //Miss!
                        check_hit = false;
                    }
                }

                if (check_hit)
                {
                    switch (Type)
                    {
                        case (e_type.damage):
                            // Missing flat mod additions
                            float damage = Modifier + (Modifier * User.Attack) - (Modifier * tar.Defense);
                            tar.Current_hp = (int)(tar.Current_hp - damage);
                            break;
                        case (e_type.buff):
                            switch (Specifier)
                            {
                                case ("attack"):
                                    User.Attack += Modifier;
                                    break;
                                case ("defense"):
                                    tar.Defense += Modifier;
                                    break;
                                case ("accuracy"):
                                    tar.Accuracy += Modifier;
                                    break;
                                case ("dodge"):
                                    tar.Dodge += Modifier;
                                    break;
                                default:
                                    break;
                            }
                            if (Duration > 0)
                            {
                                pq.Add(new BattleEffect("", e_type.buff, -Modifier, 0, Specifier, new List<BattleParticipant>() { tar }, tar, Turnstamp + Duration, false));
                            }
                            break;
                        case (e_type.status):
                            tar.Statuses.Add(new KeyValuePair<string, BattleEffect>(Specifier, this));
                            break;
                        default:
                            //This is where special/unique effects need to be handled
                            break;
                    }
                }
                else
                {
                    //Miss case
                    Debug.Log(User.Name + " Missed!");
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

