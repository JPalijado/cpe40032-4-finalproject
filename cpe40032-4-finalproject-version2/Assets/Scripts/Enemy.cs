using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Zombie, Kamikaze, Boulder, Blip, Titan, Boss}

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public EnemyType type;

    private EnemyNavigation enemyNavigation;
    private EnemyHealth enemyHealth;
    private GameManager gameManager;
    private GameObject target;

    private float attackTime;
    public bool isAttacking = false;
    public GameObject[] enemyProjectile;
    public Transform enemyProjectileSpawnPoint;

    private float attackRange;
    private float distance;
    private float attackSpeed;

    // For Kamikaze
    public float explosionTime;
    private float chargingTime;
    private int damage;
    private bool isDetonationPlayed;
    private AudioSource kamikazeAudio;

    // For Radiation Powerup
    public float radiationRadius = 8;
    private float nextHit;
    private float hitInterval = 1;
    private int radiationDamage = 1;

    // For Boss
    private float bossMeleeHealth = 100;
    private float bossRageHealth = 50;
    private float bossRageMovementSpeed = 5;
    private bool isMelee = false;
    public bool approachPlayer = false;
    private float nextSpawn;
    private float spawnInterval = 3;

    //Effect
    public ParticleSystem bossEffect;
    public GameObject kamikazeEffect;

    public void Start()
    {
        enemyNavigation = GetComponent<EnemyNavigation>();
        enemyHealth = GetComponent<EnemyHealth>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        kamikazeAudio = GameObject.Find("AudioKamikaze").GetComponent<AudioSource>();
        type = enemyType;
        target = GameObject.Find("Player");
    }

    public void Update()
    {
        // Calculates the distance between the enemy and the player
        distance = Vector3.Distance(transform.position, enemyNavigation.target.transform.position);

        Radiation();
        DisableEffects();

        // Checks the type of the enemy and set parameters accordingly
        if (enemyHealth.enemyIsDead == false && gameManager.isGameActive && enemyNavigation.isSlowed == false)
        {
            if (type == EnemyType.Zombie)
            {
                attackRange = 1.5f;
                damage = 1;
                attackSpeed = 1;
                approachPlayer = true;

                if (distance <= attackRange)
                {
                    attackTime += Time.deltaTime;
                    isAttacking = true;
                    
                    if (attackTime >= attackSpeed)
                    {
                        attackTime = 0;
                        transform.LookAt(target.gameObject.transform);
                        target.GetComponent<PlayerHealth>().RecieveDamage(damage);
                        gameManager.HitEffect(target.transform.position);
                    }
                }
                else
                {
                    isAttacking = false;
                }
            }

            if (type == EnemyType.Kamikaze)
            {
                attackRange = 3.0f;
                damage = 4;
                chargingTime = 1;
                approachPlayer = true;

                if (distance <= attackRange)
                {
                    explosionTime += Time.deltaTime;
                    isAttacking = true;

                    // Plays the detonation cue
                    if (!isDetonationPlayed)
                    {
                        isDetonationPlayed = true;
                        kamikazeAudio.Play();
                    }

                    // Explodes the kamikaze
                    if (explosionTime > chargingTime)
                    {
                        Destroy(gameObject);
                        target.GetComponent<PlayerHealth>().RecieveDamage(damage);
                        gameManager.KamikazeExplosion(transform.position);
                    }
                }
                else
                {
                    isAttacking = false;
                    isDetonationPlayed = false;
                    explosionTime = 0;
                    kamikazeAudio.Stop();
                }
            }

            if (type == EnemyType.Boulder)
            {
                attackRange = 8.0f;
                attackSpeed = 1;
                approachPlayer = true;

                if (distance <= attackRange)
                {
                    transform.LookAt(target.gameObject.transform);
                    attackTime += Time.deltaTime;
                    isAttacking = true;

                    if (attackTime >= attackSpeed)
                    {
                        attackTime = 0;
                        Instantiate(enemyProjectile[0], enemyProjectileSpawnPoint.position, enemyProjectileSpawnPoint.transform.rotation);
                        gameManager.gameAudio.PlayOneShot(gameManager.rockThrowSound, 0.4f);
                    }
                }
                else
                {
                    isAttacking = false;
                }
            }

            if (type == EnemyType.Titan)
            {
                attackRange = 1.5f;
                damage = 3;
                attackSpeed = 1;
                approachPlayer = true;

                if (distance <= attackRange)
                {
                    attackTime += Time.deltaTime;
                    isAttacking = true;
                    transform.LookAt(target.gameObject.transform);

                    if (attackTime >= attackSpeed)
                    {
                        attackTime = 0;
                        target.GetComponent<PlayerHealth>().RecieveDamage(damage);
                        gameManager.HitEffect(target.transform.position);
                    }
                }
                else
                {
                    isAttacking = false;
                }
            }

            if (type == EnemyType.Boss)
            {
                if (isMelee == false)
                {
                    attackSpeed = 2;
                    transform.LookAt(target.gameObject.transform);
                    attackTime += Time.deltaTime;
                    isAttacking = true;
                    bossEffect.Play();
                    if (attackTime >= attackSpeed)
                    {
                        attackTime = 0;
                        Instantiate(enemyProjectile[0], enemyProjectileSpawnPoint.position, enemyProjectileSpawnPoint.transform.rotation);
                        gameManager.gameAudio.PlayOneShot(gameManager.rockThrowSound, 0.4f);
                    }
                    else
                    {
                        isAttacking = false;
                    }
                }

                // The boss will spawn speical enemies if it's still alive 
                if (Time.time > nextSpawn)
                {
                    nextSpawn = Time.time + spawnInterval;
                    gameManager.SpawnMinions();
                }

                if (enemyHealth.currentHealth <= bossRageHealth)
                {
                    damage = 10;
                    enemyNavigation.speed = bossRageMovementSpeed;
                }

                if (enemyHealth.currentHealth <= bossMeleeHealth)
                {
                    isMelee = true;
                    approachPlayer = true;

                    attackRange = 1.5f;
                    damage = 5;
                    attackSpeed = 1;

                    if (distance <= attackRange)
                    {
                        attackTime += Time.deltaTime;
                        isAttacking = true;
                            
                        if (attackTime >= attackSpeed)
                        {
                            attackTime = 0;
                            transform.LookAt(target.gameObject.transform);
                            target.GetComponent<PlayerHealth>().RecieveDamage(damage);
                            gameManager.HitEffect(target.transform.position);
                        }
                    }
                    else
                    {
                        isAttacking = false;
                    }
                }
            }

        }
    }

    private void Radiation()
    {
        // Decrease the health of the enemy by 1 each second if the enemy is near the player
        if (target.GetComponent<PlayerController>().isRadiating == true && target.GetComponent<PlayerHealth>().isDead == false && enemyHealth.enemyIsDead == false)
        {
            if (distance <= radiationRadius)
            {
                Debug.Log("Radiating");
                nextHit += Time.deltaTime;
                if (nextHit >= hitInterval)
                {
                    nextHit = 0;
                    enemyHealth.HitEnemy(radiationDamage);
                    gameManager.PoofEffect(transform.position);
                }
            }
        }
    }

    // Disables the effects on kamikaze
    private void DisableEffects()
    {
        if (enemyHealth.enemyIsDead)
        {
            if (type == EnemyType.Kamikaze)
            {
                kamikazeEffect.gameObject.SetActive(false);
                kamikazeAudio.Stop();
            }
        }
    }
}
