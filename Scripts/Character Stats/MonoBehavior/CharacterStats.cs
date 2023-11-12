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
        }//ֱ�Ӷ���ScriptableObject�������ֵ

        set
        {
            characterData.maxHealth = value;//value�����ⲿ��������Եĸ�ֵ �������벻��ֱ����characterData�Ĵ������ֵ ֻ�ܼ�ӵ��� CharacterStasts��get������  get������������ʵ� set�Ƕ�ȡ�����ű����洢��ֵ���е�ֵ ����˵����������� ֻ����һ���н������
        }


    }//�ȿ��Ա�д�룬�ֿ��Ա���ȡ   c#���Ը�ֵ
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
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);//��������ȥ������
        
        currentHealth = Mathf.Max(currentHealth - damage,0);
        

        if(attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");//��Characterstates�л�ȡ�����Ϳ��Բ����ڸ��������
        }

    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if(isCritical)
        {
            coreDamage*=attackData.criticalMultiplier;
            //Debug.Log("������" + coreDamage);
        }
        return (int)coreDamage;
    }


    #endregion
}







