using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum e_type { nothing, damage, status };

public class BattleEffect
{
    private e_type type;
    private Participant target;
    private Participant user;
    private string message;
    private int turnstamp;

    public BattleEffect()
    {
        this.type = e_type.nothing;
        this.target = null;
        this.user = null;
        this.message = "This is an effect that works";
        this.turnstamp = 0;
    }

    public BattleEffect(string message)
    {
        this.type = e_type.nothing;
        this.target = null;
        this.user = null;
        this.message = message;
        this.turnstamp = 0;
    }

    public BattleEffect(e_type type, string message, int turnstamp)
    {
        this.type = type;
        this.target = null;
        this.user = null;
        this.message = message;
        this.turnstamp = turnstamp;
    }

    public BattleEffect(e_type type, Participant target, Participant user, string message, int turnstamp)
    {
        this.type = type;
        this.target = target;
        this.user = user;
        this.message = message;
        this.Turnstamp = turnstamp;
    }

    public e_type Type { get => type; set => type = value; }
    public Participant Target { get => target; set => target = value; }
    public Participant User { get => user; set => user = value; }
    public string Message { get => message; set => message = value; }
    public int Turnstamp { get => turnstamp; set => turnstamp = value; }
}


public class EffectComparator : IComparer<BattleEffect>
{
    public int Compare(BattleEffect x, BattleEffect y)
    {

        if (x.Turnstamp != y.Turnstamp)
        {
            return x.Turnstamp.CompareTo(y.Turnstamp);
        }
        //else if (x.User.Type != y.User.Type)
        //{
        //    return x.User.Type < y.User.Type ? 1 : 0;
        //}
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


public class BattleEffect_B : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
