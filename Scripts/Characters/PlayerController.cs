using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    private NavMeshAgent agent;
    private Animator anim;   //�����������

    private CharacterStats characterStats;//�����ֵ�ű�������������ű������õ���ֵ
    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();//��ʼ�������������
        characterStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        mousemanager.instance.OnMouseCliked += MoveTotarget;//���¼�����Ӷ����¼��ķ���;OnMouseCliked��һ���¼� ����һ������;
        mousemanager.instance.OnEnemyCliked += EventAttack;
    }

    

    private void Update()
    {
        //if (characterStats.currentHealth == 0)
        //  isDead = true;
        isDead = characterStats.currentHealth == 0;//���򵥵Ĵ��� ctrl k c ���д���ע�� ctrl k u ȡ�����д���ע��
        SwitchAnimation(); 
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);//����Animator�����ı���Speed ͨ��agent��ͬ���ٶȵ��ò�ͬ�Ķ���
        anim.SetBool("Death", isDead);
    }
    public void MoveTotarget(Vector3 target)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = target;//���õ��Ĳ�������agent��Ŀ�ĵ���ά����
        //�¼�:1 ��һ���������Ϲҽű� 2 �ڽű���ʵ�ֵ���ģʽ �������Ϊ��ά������¼�OnMouseCliked 3  
    }
    private void EventAttack(GameObject target)
    {
        agent.isStopped = false;    
        if(target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;

            StartCoroutine(MoveToAttackTarget());//����Я�̷���
        }
    }

    IEnumerator MoveToAttackTarget()//�ں�����ִ��while�����ᵼ��������while��ѭ����ɺ�Żᵽ��һ֡�������˲�Ƶ����
    {
        transform.LookAt(attackTarget.transform);

        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;//��������Ϣһ֡,��һ֡�ٴ�ִ������
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

