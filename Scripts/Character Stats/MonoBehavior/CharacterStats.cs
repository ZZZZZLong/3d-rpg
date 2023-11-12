using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;

    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;
     #region read from CharacterData_SO
    public int MaxHealth
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else
                return 0;
        }//直接读到ScriptableObject类里面的值

        set
        {
            characterData.maxHealth = value;//value代表外部对这个属性的赋值 其他代码不能直接用characterData的代码里的值 只能间接的用 CharacterStasts的get来访问  get是外界用来访问的 set是读取其他脚本（存储数值）中的值 所以说在这个代码内 只是做一个中介的作用
        }


    }//既可以被写入，又可以被读取   c#属性赋值
    public int currentHealth
    {
        get
        {
            if (characterData != null)

                return characterData.currentHealth;

            else
            {
                return 0;
            }
        }

        set
        {
            characterData.currentHealth = value;
        }


    }

    public int baseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;

            else
            {
                return 0;

            }
        }
        set
        {
            characterData.baseDefence = value;
        }
    }
    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            else
                return 0;
        }

        set
        {
            characterData.currentDefence = value;
        }
    
  }
    #endregion

     #region read from AttackData_SO
    public float attackRange 
    {
        get
        {
            if (attackData!= null)
                return attackData.attackRange;
            else
                return 0;
        }
        set
        {
            attackData.attackRange = value;
        }
    }

    public float skillRange
    {
        get
        {
            if (attackData != null)
                return attackData.skillRange;
            else
                return 0;
        }
        set
        {
            attackData.skillRange = value;
        }
    }

    public float coolDown
    {
        get
        {
            if (attackData != null)
                return attackData.skillRange;
            else
                return 0;
        }
        set
        {
            attackData.skillRange = value;
        }
    }

    public float minDamge
    {
        get
        {
            if (attackData != null)
                return attackData.skillRange;
            else
                return 0;
        }
        set
        {
            attackData.skillRange = value;
        }
    }

    public float maxDamge
    {
        get
        {
            if (attackData != null)
                return attackData.skillRange;
            else
                return 0;
        }
        set
        {
            attackData.skillRange = value;
        }
    }

    public float criticalMultiplier
    {
        get
        {
            if (attackData != null)
                return attackData.skillRange;
            else
                return 0;
        }
        set
        {
            attackData.skillRange = value;
        }
    }

    public float criticalChance
    {
        get
        {
            if (attackData != null)
                return attackData.skillRange;
            else
                return 0;
        }
        set
        {
            attackData.skillRange = value;
        }
    }
    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats attacker,CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);//攻击力减去防御力
        
        currentHealth = Mathf.Max(currentHealth - damage,0);
        

        if(attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");//在Characterstates中获取动画就可以不用在各个人物的
        }

    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if(isCritical)
        {
            coreDamage*=attackData.criticalMultiplier;
            //Debug.Log("暴击：" + coreDamage);
        }
        return (int)coreDamage;
    }


    #endregion
}







