using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAilments
{
    public enum Buffs
    {
        DEFENSE,
        ATTACK
    }
    public enum Debuffs
    {
        DEFENSE,
        ATTACK
    }

    public const float DefenseBuffRate = 20.0f;
    public const float AttackBuffRate = 15.0f;

    public const float DefenseDebuffRate = 10.0f;
    public const float AttackDebuffRate = 10.0f;
}
