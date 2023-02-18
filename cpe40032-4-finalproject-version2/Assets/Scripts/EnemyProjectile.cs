using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed;
    public int damage;

    void Update()
    {
        // Moves the projectiles forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        CheckBoundaries();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy the enemy projectile when colliding with the environment
        if (other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }

    // Destroys the projectile if it is not in the game view anymore
    void CheckBoundaries()
    {
        if (transform.position.x > 32.5)
        {
            Destroy(gameObject);
        }

        if (transform.position.x < -178.5)
        {
            Destroy(gameObject);
        }

        if (transform.position.z > -38)
        {
            Destroy(gameObject);
        }

        if (transform.position.z < -150)
        {
            Destroy(gameObject);
        }
    }
}
