using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum e_type { nothing, damage, status };

public class BattleEffect
{
    // These values are defined when stored in a skill
    private e_type type;
    private float modifier;
    private string message;
    // These values are only defined when it is selected in battle
    private Participant target;
    private Participant user;
    private int turnstamp;

    public BattleEffect()
    {
        this.type = e_type.nothing;
        this.modifier = 0f;
        this.message = "This is an effect that works";
        this.target = null;
        this.user = null;
        this.turnstamp = 0;
    }

    public BattleEffect(e_type type, float modifier, string message)
    {
        this.type = type;
        this.modifier = modifier;
        this.message = message;
        this.target = null;
        this.user = null;
        this.turnstamp = 0;
    }


    public e_type Type { get => type; set => type = value; }
    public Participant Target { get => target; set => target = value; }
    public Participant User { get => user; set => user = value; }
    public string Message { get => message; set => message = value; }
    public int Turnstamp { get => turnstamp; set => turnstamp = value; }
    public float Modifier { get => modifier; set => modifier = value; }


    public void select(Participant user, Participant target, int turnstamp)
    {
        User = user;
        Target = target;
        Turnstamp = turnstamp;
    }

    public void run()
    {
        Debug.Log(Message);
        switch (Type)
        {
            case (e_type.damage):
                target.Current_hp = (int) (target.Current_hp - modifier);
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

