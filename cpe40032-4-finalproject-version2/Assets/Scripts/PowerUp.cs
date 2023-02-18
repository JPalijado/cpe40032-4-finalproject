using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { None, Rage, Laser, Grenade, Shield, Slow, Radiate }

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
    private float rotationSpeed = 100.0f;
    private float spawnDuration = 10;
    private float waitTimer;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive)
        {
            // Rotates the powerup to add visual effect
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

            // The powerup will only be visible for 10 seconds;
            if (waitTimer >= spawnDuration)
            {
                Destroy(gameObject);
                gameManager.gameAudio.PlayOneShot(gameManager.despawnSound, 0.5f);
                waitTimer = 0;
            }
            else
            {
                waitTimer += 1 * Time.deltaTime;
            }
        }

        // Destroys the powerup after the level is done
        if (!gameManager.spawnAgain)
        {
            Destroy(gameObject);
        }
    }

    // Spawns another powerup if the current powerup spawned inside an obstacle
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Environment"))
        {
            Destroy(gameObject);
            gameManager.SpawnRandomPowerup();
        }
    }
}
