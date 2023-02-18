using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    private GameObject player;
    public NavMeshAgent navMeshAgent;
    private EnemyHealth enemyHealth;
    private PlayerHealth playerHealth;
    private Enemy enemy;
    private Animator enemyAnim;
    public float speed;
    public float animSpeed;
    public Transform target;
    public bool isSlowed;

    public float movementSpeed;

    private float waitTimer;
    private float spawnSlowEffectInterval = 1;

    private GameManager gameManager;

    void Start()
    {
        enemyAnim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemy = GetComponent<Enemy>();
        player = GameObject.Find("Player");
        target = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<PlayerHealth>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = movementSpeed;
        isSlowed = player.GetComponent<PlayerController>().isSlowed;

        // Slows the speed of the enemy
        if (isSlowed == true && enemyHealth.enemyIsDead == false)
        {
            if (Time.time > waitTimer)
            {
                waitTimer = spawnSlowEffectInterval + Time.time;
                gameManager.SlowEffect(transform.position);
            }
        }

        if (enemy.isAttacking == false && playerHealth.isDead == false && enemy.approachPlayer == true && gameManager.isGameActive)
        {
            navMeshAgent.destination = target.transform.position;
            // Walking animation
            enemyAnim.SetFloat("Speed_f", speed);
            enemyAnim.SetFloat("Speed_Multiplier", animSpeed);
        }

        if (enemyHealth.enemyIsDead)
        {
            navMeshAgent.isStopped = true;
            // Death animation
            enemyAnim.SetBool("Death_b", true);
            enemyAnim.SetInteger("DeathType_int", 1);
        }

        if (!gameManager.isGameActive|| isSlowed)
        {
            navMeshAgent.isStopped = true;
            // Animation to idle
            enemyAnim.SetFloat("Speed_f", 0);
        }

        if (enemy.isAttacking)
        {
            navMeshAgent.isStopped = true;
            // Change the animation to attacking animation
            enemyAnim.SetFloat("Speed_f", 0);
        }
    }
}
