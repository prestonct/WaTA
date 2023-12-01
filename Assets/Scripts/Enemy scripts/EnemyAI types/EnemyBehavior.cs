using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
/// <summary>
/// Parent Enemy Behavior script
/// Script to define all the stuff enemy AI should always do regardless of enemy type
/// </summary>

public class EnemyBehavior : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform Player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;
    public float patrolRange = 20f;
    private Vector3 spawnPoint;
    public float ChaseRange = 40f;
    bool isReturningHome;
    public float fleeDelay = 2f;
    public GameObject Coin;
    private AudioManager audioManager;

    //for static enemies
    public bool isStatic = true;

    //Patrolling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    //attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    //States
    public float sightRange, attackRange, fleeRange;
    public bool playerInSightRange, playerInAttackRange, playerInFleeRange, isStunned;
    public const float stunTime = 0.5f; // set to 0 if no stun

    private ParticleSystem particles;
    private float stunTimer = 0;
    private float voiceLineTimer = 0;
    private float voiceCooldown = 10;
    private BotState botState = BotState.Patrolling;
    private BotState lastState = BotState.Patrolling;

    enum BotState
    {
        Patrolling,
        Chasing,
        Attacking,
        Fleeing
    }

    protected virtual void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    private void Update()
    {
        if(voiceLineTimer >= voiceCooldown)
        {
            voiceLineTimer = voiceCooldown;
        }
        else
        {
            voiceLineTimer += Time.deltaTime;
        }
        if(isStunned)
        {
            if(stunTimer > stunTime)
            {
                isStunned = false;
                stunTimer = 0;
                // no longer stunned
            }
            else
            {
                stunTimer += Time.deltaTime;
                return;
            }
        }
        //check for sight, attack, and flee range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInFleeRange = Physics.CheckSphere(transform.position, fleeRange, whatIsPlayer);

        float distanceToSpawn = Vector3.Distance(transform.position, spawnPoint);
        if (isReturningHome && distanceToSpawn > 0.25f*ChaseRange) return;
        if (isReturningHome && distanceToSpawn < 0.25f*ChaseRange) isReturningHome = false;
        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
        if (playerInFleeRange && alreadyAttacked) Fleeing();

    }

    private void Awake()
    {
        spawnPoint = transform.position;
        Player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        particles = GetComponentInChildren<ParticleSystem>();
    }
    private void Patrolling()
    {
        if (botState != BotState.Patrolling)
        {
            lastState = botState;
            botState = BotState.Patrolling;
        }
        Debug.Log("Patrolling...");
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        if (audioManager != null && voiceLineTimer >= voiceCooldown)
        {
            int rand = UnityEngine.Random.Range(0, 10);
            if(rand <= 5)
            {
                audioManager.OnBotIdleSounds(transform.position);
            }
            else
            {
                audioManager.OnBotPatrollingSounds(transform.position);
            }
            voiceLineTimer = 0;
        }

        //find a new walkPoint that is never more than +- X away from spawnPoint - limited patrol range
        walkPoint = spawnPoint + new Vector3(Random.Range(-patrolRange, patrolRange), 0f, Random.Range(-patrolRange, patrolRange));

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (botState != BotState.Chasing)
        {
            lastState = botState;
            botState = BotState.Chasing;

            if (audioManager != null && voiceLineTimer >= voiceCooldown)
            {
                voiceLineTimer = 0;
                audioManager.OnBotLocatesTargetSounds(transform.position);
            }
        }
        Debug.Log("Chasing...");
        if (isStatic == true)
        {
            return;
        }
        float distanceToSpawn = Vector3.Distance(transform.position, spawnPoint);

        if (distanceToSpawn > ChaseRange)
        {
            ReturnHome();
            return;
        }
        agent.SetDestination(Player.position);
    }

    private void ReturnHome()
    {
        isReturningHome = true;
        agent.SetDestination(spawnPoint);
        if (audioManager != null && voiceLineTimer >= voiceCooldown)
        {
            voiceLineTimer = 0;
            audioManager.OnBotLosesTargetSounds(transform.position);
        }
    }


    public virtual void AttackPlayer()
    {
        Debug.Log("Attacking...");
        if (botState != BotState.Attacking)
        {
            lastState = botState;
            botState = BotState.Attacking;

            if (audioManager != null && voiceLineTimer >= voiceCooldown)
            {
                voiceLineTimer = 0;
                audioManager.OnBotAttackSounds(transform.position);
            }
        }

        agent.SetDestination(transform.position);
        transform.LookAt(Player.position + new Vector3(0f, 1.3f, 0f));
    }
    private void Fleeing()
    {
        if (botState != BotState.Fleeing)
        {
            lastState = botState;
            botState = BotState.Fleeing;
        }
        StartCoroutine(FleeAfterDelay(fleeDelay));
    }

    IEnumerator FleeAfterDelay(float fleeDelay)
    {
        yield return new WaitForSeconds(fleeDelay);
        Debug.Log("Fleeing...");
        //set enemy to move straight back from player
        //vector math to move back in a straight line
        Vector3 fleeDirection = transform.position - Player.position;
        fleeDirection.Normalize();
        agent.SetDestination(transform.position + fleeDirection * (attackRange - .2f * attackRange));
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        particles.Play();
        if(stunTime > 0)
        {
            isStunned = true;
            stunTimer = 0;
        }
        int rand = UnityEngine.Random.Range(0, 10);
        if (rand < 5 && voiceLineTimer >= voiceCooldown)
        {
            voiceLineTimer = 0;
            audioManager.OnBotStruckQuips(transform.position);
        }
        audioManager.OnBotStruckSounds(transform.position);

        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
            Instantiate(Coin, transform.position, Quaternion.identity);
        }
    }
    private void DestroyEnemy()
    {
        int rand = UnityEngine.Random.Range(0, 10);
        if (rand < 4)
        {
            audioManager.OnWillowKillSounds(Player.position);
        }
        Destroy(gameObject);
        //create a coin game object
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, fleeRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if(other.gameObject.layer == LayerMask.NameToLayer("Player Attack"))
        {
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            TakeDamage(playerAttack.damage);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other == null) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player Attack"))
        {
            PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            if (playerAttack.canHitMultipleTimes)
            {
                // this damage is taken every physics update, not likely to be useful but maybe for certain powers
                TakeDamage(playerAttack.damage);
            }
        }
    }
}
