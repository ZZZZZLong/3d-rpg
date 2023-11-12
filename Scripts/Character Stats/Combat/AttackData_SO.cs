using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack",menuName = "Attack/Attack Data")]//在左键点击创建时可以看见
public class AttackData_SO : ScriptableObject
{
    public float attackRange;

    public float skillRange;

    public float coolDown;

    public float minDamage;

    public float maxDamage;

    public float criticalMultiplier;//爆伤

    public float criticalChance;//暴击率

}
    

