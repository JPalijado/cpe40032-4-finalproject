using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationIndicator : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        // Reads the game manager script
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // Points the pointer to the location indicator
        gameManager.locationArrow.transform.LookAt(gameObject.transform);
    }
}
