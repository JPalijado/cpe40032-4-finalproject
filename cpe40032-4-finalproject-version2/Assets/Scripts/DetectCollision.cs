using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{

    private PlayerHealth playerHealth;
    private GameManager gameManager;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detects if the player was hit by an enemy projectile
        if (other.CompareTag("EnemyProjectile"))
        {
            playerHealth.RecieveDamage(other.GetComponent<EnemyProjectile>().damage);
            Destroy(other.gameObject);
            gameManager.RockEffect(transform.position);
        }
    }
}
