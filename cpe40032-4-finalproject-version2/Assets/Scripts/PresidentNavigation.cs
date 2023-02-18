using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PresidentNavigation : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent navMeshAgent;
    private Animator presAnim;
    private float pickupRange = 3;
    private float distance;
    private float movementSpeed = 5;

    private GameManager gameManager;

    public bool gotPresident = false;
    public bool spawnedHeli = false;

    public AudioSource presAudio;
    private AudioSource audioHeli;
    public AudioClip gotPresidentSound;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").GetComponent<Transform>();
        presAnim = GetComponent<Animator>();
        audioHeli = GameObject.Find("AudioHeli").GetComponent<AudioSource>();
        presAudio = GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = movementSpeed;

        // Calculates the distance between the president and the player
        distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < pickupRange)
        {
            // Signals that the player has the president
            gotPresident = true;
        }

        if (gotPresident == true)
        {
            // Set destination to the player
            navMeshAgent.destination = target.transform.position;

            // Disables the pointer
            gameManager.locationArrow.gameObject.SetActive(false);

            // Spawns the helicopter
            if (spawnedHeli == false)
            {
                gameManager.SpawnHelicopter();
                audioHeli.Play();
                presAudio.PlayOneShot(gotPresidentSound, 3);
                spawnedHeli = true;
            }
        }

        if (distance <= navMeshAgent.stoppingDistance || gotPresident == false)
        {
            // Stops the president
            navMeshAgent.isStopped = true;
            presAnim.SetFloat("Speed_f", 0);
        }
        else
        {
            navMeshAgent.isStopped = false;
            // Walking animation
            presAnim.SetFloat("Speed_f", 1);
            presAnim.SetFloat("Speed_Multiplier", 1);
        }
    }
}
