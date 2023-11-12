using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data",menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]

    public int maxHealth;//最大

    public int currentHealth;//当前

    public int baseDefence;

    public int currentDefence;
    //攻击更加复杂    需要创建额外的ScriptableObject的类//通过一个脚本创建多种数值 这就是ScriptableObject的好处
}
