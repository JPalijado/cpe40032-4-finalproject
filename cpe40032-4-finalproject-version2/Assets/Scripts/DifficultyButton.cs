using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;

    public int maxSpawn;
    public int incrementSpawn;
    public int minRate;
    public int maxRate;

    void Start()
    {
        // Initializes components
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void SetDifficulty()
    {
        // Changes the difficulty of the game
        gameManager.StartGame(maxSpawn, incrementSpawn, minRate, maxRate);
    }
}