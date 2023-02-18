using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 40.0f;
    public float playerPosZ;
    public float zRange = 50.0f;
    public float playerPosX;
    public float xRange = 50.0f;
    public int damage;
    public bool isLaser = false;
    public bool isGrenade = false;

    private float explosionRadius = 3;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (gameManager.isGameActive)
        {
            // Moves the projectiles forward
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            Transform playerPos = GameObject.Find("Player").GetComponent<Transform>();

            playerPosZ = playerPos.transform.position.z;
            playerPosX = playerPos.transform.position.x;

            // Destroys the game object upon reaching the boundaries
            if ((transform.position.x > playerPosX + xRange) || (transform.position.x < playerPosX - xRange) || (transform.position.z > playerPosZ + zRange) || (transform.position.z < playerPosZ - zRange))
            {
                Destroy(gameObject);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Environment")) 
        {
            // The laser will not destroy itself when colliding with the environment
            if (!isLaser)
            {
                Destroy(gameObject);
            }
            // Apply the explosion damage of the grenade
            ExplosionDamage();
        }

        if (other.CompareTag("EnemyProjectile"))
        {
            // The grenade will destroy the projectile from the enemy
            if (isGrenade)
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
            // Apply the explosion damage of the grenade
            ExplosionDamage();
        }

        if (other.CompareTag("Enemy"))
        {
            if(!other.GetComponent<EnemyHealth>().enemyIsDead)
            {
                if (!isLaser && !isGrenade)
                {
                    // Blood effect for bullet
                    gameManager.BloodEffect(transform.position);
                }
                // For grenade
                if (isGrenade == false)
                {
                    other.GetComponent<EnemyHealth>().HitEnemy(damage);
                }
                // The laser will not destroy itself when colliding with an enemy
                if (!isLaser)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameManager.SparkEffect(transform.position);
                }
                // Apply the explosion damage of the grenade
                ExplosionDamage();
            }
        }
    }

    private void ExplosionDamage()
    {
        // If the projectile is a grenade, damage the enemies within the explosion radius
        if (isGrenade == true)
        {
            gameManager.GrenadeEffect(transform.position);
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider c in colliders)
            {
                if (c.CompareTag("Enemy"))
                {
                    c.GetComponent<EnemyHealth>().HitEnemy(damage);
                }
            }
        }
    }
}
