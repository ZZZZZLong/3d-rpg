using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    private NavMeshAgent agent;
    private Animator anim;   //定义两个组件

    private CharacterStats characterStats;//获得数值脚本，就能在这个脚本中运用到数值
    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();//初始化调用两个组件
        characterStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        mousemanager.instance.OnMouseCliked += MoveTotarget;//往事件里添加订阅事件的方法;OnMouseCliked是一个事件 理解成一个集合;
        mousemanager.instance.OnEnemyCliked += EventAttack;
    }

    

    private void Update()
    {
        //if (characterStats.currentHealth == 0)
        //  isDead = true;
        isDead = characterStats.currentHealth == 0;//更简单的代码 ctrl k c 多行代码注释 ctrl k u 取消多行代码注释
        SwitchAnimation(); 
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);//设置Animator组件里的变量Speed 通过agent不同的速度调用不同的动画
        anim.SetBool("Death", isDead);
    }
    public void MoveTotarget(Vector3 target)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = target;//将得到的参数传入agent的目的地三维函数
        //事件:1 在一个空物体上挂脚本 2 在脚本上实现单例模式 构造参数为三维坐标的事件OnMouseCliked 3  
    }
    private void EventAttack(GameObject target)
    {
        agent.isStopped = false;    
        if(target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;

            StartCoroutine(MoveToAttackTarget());//挂上携程方程
        }
    }

    IEnumerator MoveToAttackTarget()//在函数里执行while方法会导致人物在while中循环完成后才会到下一帧，会出现瞬移的情况
    {
        transform.LookAt(attackTarget.transform);

        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;//在这里休息一帧,下一帧再次执行命令
        }

        agent.isStopped = true;

        if(lastAttackTime < 0 )
        {
            anim.SetBool("Critical", characterStats.isCritical);
            
            anim.SetTrigger("Attack");
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //Animation Event

    void Hit()
    {
        var targetStats = attackTarget.GetComponent<CharacterStats>();

        targetStats.TakeDamage(characterStats, targetStats);

    }

}

