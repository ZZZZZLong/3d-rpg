using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates{GUARD,PATROL,CHASE,DEAD }
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;
    
    private NavMeshAgent agent;

    private Animator anim;

    private Collider coll;

    private CharacterStats characterStats;

    [Header("Basic Settings")]

    public float sightRadius;

    public bool isGuard;//�ǲ���һ��վ׮��

    private float speed;

    private GameObject attackTarget;

    public float lookAtTime; 

    private float remainLookAtTime;//��ʱ�����ڴ�������Ҫ�仯�ͻ�ԭ��Ҫ�õ�������˽�������ֱ�����

    private float lastAttackTime;

    private Quaternion guardRotation;

    [Header("Patrol State")]

    public float patrolRange;

    private Vector3 wayPoint;

    private Vector3 guardPos;

    //��϶����Ĳ���ֵ
    
    bool isWalk;

    bool isChase;

    bool isFollow;

    bool isDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;

        guardPos = transform.position;

        guardRotation = transform.rotation;

        remainLookAtTime = lookAtTime;
                                           
        
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
    }
    private void Update()
    {
        if (characterStats.currentHealth == 0)
            isDead = true;

        SwitchStates();
        SwitchAnimation();
        lastAttackTime = -Time.deltaTime;//��ʱ��
    }

    private void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);//�Ѷ����Ĳ���ֵ�ʹ��������һ��
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }
    private void SwitchStates()
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            
        }
        //�������Player�л���׷��״̬
        switch(enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                
                if(transform.position != guardPos)//����վ׮λ�� ��õص��ǰ��վ׮λ��
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                     
                        isWalk = false;//վ׮λ�ú͵�ǰλ�������ƽ��,��λ
                        transform.rotation = Quaternion.Lerp(transform.rotation,guardRotation,0.01f);//Lerp(�ʼ�ĽǶ�,��ת��ĽǶȣ��ٶ���ֵ �ٶ�Խ����ֵԽ��0-1��)
                    }
                }
                
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;//�˷��ȳ���������С
                //�ж��Ƿ��ߵ������Ѳ�ߵ�;
                if(Vector3.Distance(wayPoint,transform.position)<= agent.stoppingDistance)//���ܿ�����SqrMagnitude����
                {
                    isWalk = false;
                    if(remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE:
                //1.׷ 2. ���ѻ���һ��״̬ 3.�ڹ�����Χ�ڹ��� 4.��϶��� 
                isWalk = false;
                isChase = true;

                agent.speed = speed;
                
                if (!FoundPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard){
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        enemyStates = EnemyStates.PATROL;
                    }
                    
                }//���û���ֵ��˾�����ս��
                else
                {
                    isFollow = true;
                    agent.isStopped = false;

                    agent.destination = attackTarget.transform.position;
                    
                }
                
                //������Χ���
                if (TargetiInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        //�����ж�
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;//���������С�ڱ����ʵ�ʱ��ͻ��������
                        //ִ�й���
                        Attack();
                        
                    }

                }


                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                agent.enabled = false;//������޸��������

                Destroy(gameObject,2f);//�ӳ�����
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if(TargetiInAttackRange())
        {
            //������������
            anim.SetTrigger("Attack");
            
        }
        if (TargetInSkillRange())
        {
            //Զ�̹�������
            anim.SetTrigger("Skill");
        }
    }



    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders) 
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }


    bool TargetiInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position,transform.position)<= characterStats.attackData.attackRange;

        else return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else return false;
    }

    void GetNewWayPoint()
    {

        remainLookAtTime = lookAtTime;//��ʱ�����÷�
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x+randomX,guardPos.y,guardPos.z+randomZ);

        //wayPoint = randomPoint;//������ô�򵥵ľ͵õ�����㣬���ܻῨ��
        NavMeshHit hit;//������Ϣ
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position:transform.position;//�ڷ�Χ���Ƿ��ܵõ�һ�������ƶ��ĵ㣬�����˿�����ǽ�ڵ����
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRadius);  
    }

    //Animation Event
    void Hit()
    {
        if(attackTarget!=null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}

