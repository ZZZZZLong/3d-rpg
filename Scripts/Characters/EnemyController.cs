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

    public bool isGuard;//是不是一个站桩怪

    private float speed;

    private GameObject attackTarget;

    public float lookAtTime; 

    private float remainLookAtTime;//计时器，在代码中需要变化和还原，要用到公开和私立的两种变量；

    private float lastAttackTime;

    private Quaternion guardRotation;

    [Header("Patrol State")]

    public float patrolRange;

    private Vector3 wayPoint;

    private Vector3 guardPos;

    //配合动画的布尔值
    
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
        lastAttackTime = -Time.deltaTime;//计时器
    }

    private void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);//把动画的布尔值和代码关联在一起
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
        //如果发现Player切换到追击状态
        switch(enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                
                if(transform.position != guardPos)//不在站桩位置 获得地点后前往站桩位置
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                     
                        isWalk = false;//站桩位置和当前位置相减的平方,归位
                        transform.rotation = Quaternion.Lerp(transform.rotation,guardRotation,0.01f);//Lerp(最开始的角度,旋转后的角度，速度数值 速度越快数值越大【0-1】)
                    }
                }
                
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;//乘法比除法开销更小
                //判断是否走到了随机巡逻点;
                if(Vector3.Distance(wayPoint,transform.position)<= agent.stoppingDistance)//性能开销比SqrMagnitude更大
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
                //1.追 2. 拉脱回上一个状态 3.在攻击范围内攻击 4.配合动画 
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
                    
                }//如果没发现敌人就脱离战斗
                else
                {
                    isFollow = true;
                    agent.isStopped = false;

                    agent.destination = attackTarget.transform.position;
                    
                }
                
                //攻击范围检测
                if (TargetiInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;//当随机的数小于暴击率的时候就会产生暴击
                        //执行攻击
                        Attack();
                        
                    }

                }


                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                agent.enabled = false;//后面会修改这个方法

                Destroy(gameObject,2f);//延迟两秒
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if(TargetiInAttackRange())
        {
            //近身攻击动画；
            anim.SetTrigger("Attack");
            
        }
        if (TargetInSkillRange())
        {
            //远程攻击动画
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

        remainLookAtTime = lookAtTime;//计时器的用法
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x+randomX,guardPos.y,guardPos.z+randomZ);

        //wayPoint = randomPoint;//不能这么简单的就得到这个点，可能会卡死
        NavMeshHit hit;//返回信息
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position:transform.position;//在范围内是否能得到一个可以移动的点，避免了卡死在墙内的情况
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

