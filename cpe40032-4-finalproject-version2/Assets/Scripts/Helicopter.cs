using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    private float ydown = -35.4f;
    private float speed = 7;
    public bool comingUp = false;
    public GameObject president;
    public GameObject player;

    private GameManager gameManager;

    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // The helicopter will descend
        if (transform.position.y > ydown && comingUp == false)
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
        
        // The helicopter will takeoff
        if (comingUp == true)
        {
           transform.Translate(Vector3.up * Time.deltaTime * speed);
           gameManager.DespawnPresident();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // Runs the mission complete method
        if (other.CompareTag("Player"))
        {
            comingUp = true;
            player.gameObject.SetActive(false);
            gameManager.MissionComplete();
        }
    }
}
