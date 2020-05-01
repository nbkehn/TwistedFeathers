using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwistedFeathers
{

    //TODO Implement BattleEffect as a dynamic to solve specifier enum issue

    public enum e_type
    {
        nothing,
        damage,
        status,
        buff
    }

    public enum stat_type
    {
        nothing,
        HP,
        Attack,
        Defense,
        Accuracy,
        Dodge

    }

    public enum status_type
    {
        nothing,
        Poison,
        Burn,
        Stun
    }

    public enum target_type
    {
        None,
        Self,
        Ally,
        Enemy,
        AllAllies,
        AllEnemies,
        All
    }

    [System.Serializable]
    public class BattleEffect
    {

        // This field is used by the skill editor only
        private bool show;

        // Other fields
        [SerializeField]
        private e_type type;

        [SerializeField]
        private float modifier;

        [SerializeField]
        private int duration;

        [SerializeField]
        private string specifier;

        [SerializeField]
        private int turnstamp;

        [SerializeField]
        private target_type targetType;

        public bool Show { get => show; set => show = value; }
        public e_type Type { get => type; set => type = value; }
        public float Modifier { get => modifier; set => modifier = value; }
        public int Duration { get => duration; set => duration = value; }
        public string Specifier { get => specifier; set => specifier = value; }
        public int Turnstamp { get => turnstamp; set => turnstamp = value; }
        public target_type TargetType { get => targetType; set => targetType = value; }


        // These values are only defined when it is selected in battle
        public List<BattleParticipant> Target { get; set; }
        public Participant User { get; set; }
        // These values are defined when stored in a skill
        /*Whether or not the effect will show up in the forecast*/
        public bool Visible { get; set; }
        public string SkillName { get; set; }
        public int UID { get; set; }
        public List<Conditional> Conditions { get; set; }
        public FillSkills fs;

        // Copy Constructor
        public BattleEffect(BattleEffect effect)
        {
            this.Type = effect.Type;
            this.Target = effect.Target;
            this.User = effect.User;
            this.Specifier = effect.Specifier;
            this.Turnstamp = effect.Turnstamp;
            this.Modifier = effect.Modifier;
            this.Duration = effect.Duration;
            this.Visible = effect.Visible;
            this.SkillName = effect.SkillName;
            this.UID = 0;
            this.Conditions = effect.Conditions;
            this.targetType = effect.targetType;
        }

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
            this.UID = 0;
            this.Conditions = new List<Conditional>();
            this.targetType = target_type.None;
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
            this.UID = 0;
            this.Conditions = new List<Conditional>();
            this.targetType = target_type.None;
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
            this.UID = 0;
            this.Conditions = new List<Conditional>();
            this.targetType = target_type.None;
        }

        public BattleEffect(e_type type, float modifier, int duration, string specifier, target_type target)
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
            this.UID = 0;
            this.Conditions = new List<Conditional>();
            this.targetType = target;
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
            this.UID = 0;
            this.Conditions = new List<Conditional>();
            this.targetType = target_type.None;
        }

        public BattleEffect(string skill_name, e_type type, float modifier, int duration, string specifier, List<BattleParticipant> target, Participant user, int turnstamp)
        {
            this.SkillName = skill_name;
            this.Type = type;
            this.Modifier = modifier;
            this.Duration = duration;
            this.Specifier = specifier;
            this.Target = target;
            this.User = user;
            this.Turnstamp = turnstamp;
            this.Visible = true;
            this.UID = 0;
            this.Conditions = new List<Conditional>();
            this.targetType = target_type.None;
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
            this.UID = 0;
            this.Conditions = new List<Conditional>();
            this.targetType = target_type.None;
        }

        private bool areCondMet()
        {
            foreach (Conditional condition in Conditions)
            {
                if (!condition.isCond(this))
                {
                    return false;
                    break;
                }
            }

            return true;

        }


        public bool select(Participant user, List<BattleParticipant> target, int turnstamp, string skill_name)
        {
            User = user;
            Target = target;
            Turnstamp += turnstamp;
            SkillName = skill_name;
            UID = Random.Range(0, Int32.MaxValue);
            return areCondMet();
        }

        public void addPassive(BattleParticipant user)
        {
            applyChanges(user, Modifier);
        }

        public void removePassive(BattleParticipant user)
        {
            applyChanges(user, -Modifier);
        }

        //Utility method for adding and removing passives
        private void applyChanges(BattleParticipant user, float value)
        {
            switch (Specifier)
            {
                case ("Dodge"):
                    user.Dodge += value;
                    break;
                case ("Defense"):
                    user.Defense += value;
                    break;
                default:
                    Debug.LogError("Error: Invalid passive specified");
                    break;
            }
        }

        public void run()
        {


            bool check_hit = true;
            foreach (BattleParticipant tar in Target)
            {
                check_hit = true;
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
                            BattleParticipant bp = null;
                            switch (Specifier)
                            {
                                case ("lifesteal"):
                                    bp = (BattleParticipant)User;
                                    bp.Current_hp = Math.Min(bp.Max_hp, bp.Current_hp + (int)(damage*.2));
                                    break;
                                case ("recoil"):
                                    System.Random rand = new System.Random();
                                    if (rand.Next(10) < 2)
                                    {
                                        bp = (BattleParticipant)User;
                                        bp.Current_hp = bp.Current_hp - 20;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case (e_type.buff):
                            switch (Specifier)
                            {
                                case ("attack"):
                                    tar.Attack += Modifier;
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
                                case ("vulnerable"):
                                    tar.Defense += -1;
                                    break;
                                case ("HealEnemy"):
                                    GameObject.Find("CombatManager").GetComponent<CombatManager>().healEnemy((int) Modifier);
                                    break;
                                case "FearCurse":
                                    // Change inequality to adjust percentage chance of occuring
                                    if(Random.Range(0.0f, 1.0f) < 0.5f)
                                    {
                                        fs = GameObject.Find("OpenSkills").GetComponent<FillSkills>();
                                        fs.DisableSkill = Random.Range(0, 4);
                                    }
                                    break;
                                case "FearCurse2":
                                    fs = GameObject.Find("OpenSkills").GetComponent<FillSkills>();
                                    fs.DisableSkill = Random.Range(0, 4);
                                    break;
                                case "Paralysis":
                                    if (Random.Range(0.0f, 1.0f) < 0.2f)
                                    {
                                        fs = GameObject.Find("OpenSkills").GetComponent<FillSkills>();
                                        fs.DisableAllSkills = true;
                                    }
                                    break;
                                case "Resurrect":
                                    GameObject.Find("CombatManager").GetComponent<CombatManager>().resurrectEnemy();
                                    break;
                                default:
                                    Debug.LogError("Error: Invalid stat buff specified");
                                    break;
                            }
                            if (Duration > 0)
                            {
                                tar.Buffs.Add(new BattleEffect("", e_type.buff, -Modifier, 0, Specifier, new List<BattleParticipant>() { tar }, tar, Turnstamp + Duration));
                            }
                            break;
                        case (e_type.status):
                            if (Visible)
                            {
                                BattleEffect status_effect = new BattleEffect(this);
                                status_effect.Visible = false;
                                status_effect.Target = new List<BattleParticipant>(){tar};
                                status_effect.User = tar;
                                tar.Statuses.Add(status_effect);
                            }
                            else
                            {
                                switch (Specifier)
                                {
                                    case "Poison":
                                        tar.Current_hp -= 3;
                                        break;
                                    case "Bleed":
                                        tar.Current_hp -= 5;
                                        break;
                                    case "Burn":
                                        break;
                                    case "Stun":
                                        break;
                                    default:
                                        Debug.LogError("Error: Invalid status effect specified");
                                        break;
                                }

                                this.duration -= 1;
                            }
                            break;
                        case (e_type.nothing):
                            // do nothing
                            break;
                        default:
                            //This is where special/unique effects need to be handled
                            Debug.LogError("Error: Invalid effect type specified");
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
            else if (x.Modifier != y.Modifier)
            {
                return x.Modifier.CompareTo(y.Modifier);
            }
            else if (x.Specifier != y.Specifier)
            {
                return x.Specifier.CompareTo(y.Specifier);
            }
            else if (x.UID != y.UID)
            {
                return x.UID.CompareTo(y.UID);
            }
            else
            {
                return 0;
            }
        }
    }
}

