using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public enum cond_type
    {
        None,
        Environment,
        Target,
        User
    }

    public class Conditional
    {
        // What the entity conditional will be looking at
        private cond_type check_type;
        // What aspect of the entity the conditional will be looking at
        private stat_type check_stat;
        // The amount the conditional is comparing against
        private float check_amount;
        // The direction of the comparison; > 0  is greater than, < 0 is less than, == 0 is equal to
        private int check_compare;
        //This is an option to invert the conditional
        private bool check_invert;

        public bool isCond(BattleEffect battleEffect)
        {
            bool conditional = true;
            switch (this.check_type)
            {
                case cond_type.Environment:
                    break;
                case cond_type.Target:
                    foreach (BattleParticipant bp in battleEffect.Target)
                    {
                        if (check_compare > 0)
                        {
                            conditional = bp.getStat(check_stat) > check_amount;
                        }
                        else if (check_amount < 0)
                        {
                            conditional = bp.getStat(check_stat) < check_amount;
                        }
                        else
                        {
                            conditional = bp.getStat(check_stat) == check_amount;
                        }
                    }
                    break;
                case cond_type.User:
                    BattleParticipant user = (BattleParticipant) battleEffect.User;
                    if (check_compare > 0)
                    {
                        conditional = user.getStat(check_stat) > check_amount;
                    }
                    else if (check_amount < 0)
                    {
                        conditional = user.getStat(check_stat) < check_amount;
                    }
                    else
                    {
                        conditional = user.getStat(check_stat) == check_amount;
                    }
                    break;
                default:
                    Debug.LogError("Conditional Effect: invalid conditional type");
                    break;
            }

            if (check_invert)
            {
                conditional = !conditional;
            }
            return conditional;
        }

    }
}


