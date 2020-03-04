using UnityEngine;

public enum EffectType { None, Damage, Status, Buff }

[System.Serializable]
public class BattleEffect
{
    private bool show;

    [SerializeField]
    private EffectType type;

    [SerializeField]
    private float modifier;

    [SerializeField]
    private float duration;

    [SerializeField]
    private string specifier;

    public bool Show { get => show; set => show = value; }
    public EffectType Type { get => type; set => type = value; }
    public float Modifier { get => modifier; set => modifier = value; }
    public float Duration { get => duration; set => duration = value; }
    public string Specifier { get => specifier; set => specifier = value; }

}
