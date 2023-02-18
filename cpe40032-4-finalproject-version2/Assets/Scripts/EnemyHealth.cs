using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public Slider healthSlider;
    public int healthPoints;
    public int currentHealth;
    public bool enemyIsDead = false;
    public bool isBossDead = false;
    private Enemy enemy;
    private GameManager gameManager;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        healthSlider.maxValue = healthPoints;
        healthSlider.value = healthPoints;
        currentHealth = healthPoints;
    }

    public void HitEnemy(int damage)
    {
        // Deal damage to enemy
        currentHealth -= damage;
        healthSlider.value = currentHealth;

        // Death animation
        if (currentHealth <= 0)
        {
            enemyIsDead = true;
            healthSlider.gameObject.SetActive(false);
            Destroy(gameObject, 2.5f);

            if (enemy.type == EnemyType.Boss)
            {
                // Boss death effect
                gameManager.BossDeathEffect(transform.position);
            }
            else if (enemy.type == EnemyType.Titan)
            {
                // Titan death effect
                gameManager.TitanDeath();
            }
            else
            {
                // Normal enemies effect
                gameManager.EnemyDeath();
            }
        }
    }
}