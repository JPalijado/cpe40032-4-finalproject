using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public GameObject heartBeatPanel;
    public Slider healthSlider;
    public int healthPoints = 10;
    public bool isDead = false;
    public int increaseHealthPoints = 5;
    public int currentHealth;
    private float nextBeat;
    private float beatInterval = 1;
    private float waitTimer;
    public bool isPanelPlayed;

    private PlayerController playerController;
    private GameManager gameManager;
    private Animator playerAnim;

    void Start()
    {
        healthSlider.maxValue = healthPoints;
        healthSlider.value = healthPoints;
        currentHealth = healthPoints;

        playerAnim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // Plays a heart beat sound effect
        if (currentHealth <= (healthPoints / 3) && gameManager.aliveEnemyCount != 0)
        {
            heartBeatPanel.gameObject.SetActive(true);
            if (Time.time > nextBeat && gameManager.isGameActive)
            {
                nextBeat = Time.time + beatInterval;
                gameManager.gameAudio.PlayOneShot(gameManager.heartBeatSound, 4);
            }
        }
        else
        {
            heartBeatPanel.gameObject.SetActive(false);
        }
    }

    public void RecieveDamage(int damage)
    {
        // The player will recieve damage from enemies
        if (!playerController.isShielded)
        {
            currentHealth -= damage;
            healthSlider.value = currentHealth;
            gameManager.gameAudio.PlayOneShot(gameManager.playerHitSound, 2);

            // Plays game over
            if (currentHealth <= 0)
            {
                healthSlider.gameObject.SetActive(false);
                gameManager.GameOver();
                playerAnim.SetBool("Death_b", true);
                playerAnim.SetInteger("DeathType_int", 1);
            }
        }
        else
        {
            gameManager.ShieldHitSoundEffect();
        }
    }

    public void ResetHealth()
    {
        // Regens the player health into full
        currentHealth = healthPoints;
        healthSlider.value = currentHealth;
        gameManager.RegenEffect();
    }

    public void IncreaseHealth()
    {
        // Increase the player heath after every level
        healthPoints += increaseHealthPoints;
        healthSlider.maxValue = healthPoints;
        currentHealth = healthPoints;
        healthSlider.value = currentHealth;
        gameManager.RegenEffect();
    }
}