using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack",menuName = "Attack/Attack Data")]//������������ʱ���Կ���
public class AttackData_SO : ScriptableObject
{
    public float attackRange;

    public float skillRange;

    public float coolDown;

    public float minDamage;

    public float maxDamage;

    public float criticalMultiplier;//����

    public float criticalChance;//������

}
    

