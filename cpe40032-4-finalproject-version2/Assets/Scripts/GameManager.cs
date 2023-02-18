using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private PlayerController playerControllerScript;
    private PlayerHealth playerHealthScript;

    // For GameObjects
    public List<GameObject> enemies;
    public List<GameObject> powerups;
    public GameObject locIndicatorPrefarb;
    public GameObject bossPrefab;
    public GameObject president;
    public GameObject helicopterPrefab;
    public GameObject player;
    public GameObject gun;
    public GameObject backgroundAudio;

    // For User Interface
    public GameObject titleScreen;
    public GameObject difficultyScreen;
    public GameObject settingsScreen;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject missionCompleteScreen;
    public GameObject levelClearScreen;
    public GameObject settingReturn;
    public GameObject pauseSettingReturn;
    public GameObject darkPanel;
    public GameObject powerupText;
    public GameObject locationArrow;
    public GameObject introText;
    public GameObject controlScreen;
    public GameObject bossText;

    // For Lighting
    public GameObject lvl1Light;
    public GameObject lvl2Light;
    public GameObject lvl3Light;
    public GameObject lvl4Light;
    public GameObject lvl5Light;
    public GameObject playerLight;

    // For Spawning Enemies
    public int spawnedEnemy;
    public int maxSpawnedEnemy;
    public int inreaseEnemyCount;
    public float spawnEnemyInterval = 5.0f;
    public int aliveEnemyCount;
    private int minRate;
    private int maxRate;
    public int spawnRate;
    public bool isSpawning;
    private float waitTimer;
    private float spawnPosY = -35.4438f;
    public Vector3[] spawnPositions;

    // For level
    private float levelInterval = 10;
    private float nextLevelStart;
    public bool nextLevel;
    public bool spawnAgain;
    public bool isLevelDone;
    public bool isGameStarted;
    public bool isGameActive;
    private bool isGamePaused;
    public int level;

    // For PowerUps
    private float powerupWaitTimer;
    public float powerupSpawnInterval = 10;
    private float powerupYPos = -34.7f;

    // For Spawning Other GameObjects
    private bool isLocIndicatorSpawned;
    public bool isBossSpawned;
    private bool isProceedPlayed;

    // Visual Effects
    public ParticleSystem explosionEffects;
    public ParticleSystem bloodEffects;
    public ParticleSystem sparkEffects;
    public ParticleSystem grenadeEffects;
    public ParticleSystem rockEffects;
    public ParticleSystem hitEffects;
    public ParticleSystem poofEffects;
    public ParticleSystem slowEffects;
    public ParticleSystem regenEffects;
    public ParticleSystem bossDeathEffects;
    public ParticleSystem bossSpawnEffects;
    public ParticleSystem muzzleEffects;

    // Sound Effects 
    public AudioSource gameAudio;
    public AudioClip bulletSound;
    public AudioClip bulletHitSound;
    public AudioClip laserSound;
    public AudioClip laserHitSound;
    public AudioClip laserChargingSound;
    public AudioClip grenadeSound;
    public AudioClip grenadeIndicatorSound;
    public AudioClip explosionSound;
    public AudioClip radiateSound;
    public AudioClip radiateHitSound;
    public AudioClip rageSound;
    public AudioClip shieldSound;
    public AudioClip shieldHitSound;
    public AudioClip slowSound;
    public AudioClip undoSlowSound;
    public AudioClip regenSound;
    public AudioClip kamikazeSound;
    public AudioClip rockThrowSound;
    public AudioClip rockHitSound;
    public AudioClip playerHitSound;
    public AudioClip BossDeathSound;
    public AudioClip BossSpawnSound;
    public AudioClip heartBeatSound;
    public AudioClip despawnSound;
    public AudioClip gameOverSound;
    public AudioClip introSound;
    public AudioClip endingSound;
    public AudioClip proceedSound;
    public AudioClip doneSound;
    public AudioClip enemyDeathSound;
    public AudioClip titanDeathSound;

    void Start()
    {
        playerControllerScript = player.GetComponent<PlayerController>();
        playerHealthScript = player.GetComponent<PlayerHealth>();
        gameAudio = GetComponent<AudioSource>();
    }

    public void SelectDifficulty()
    {
        // Disables the screens that are not needed
        titleScreen.gameObject.SetActive(false);
        settingReturn.gameObject.SetActive(false);

        // Displays the difficulty screen for difficulty selection
        difficultyScreen.gameObject.SetActive(true);
        isGameStarted = false;
        isGamePaused = false;
        gameAudio.Play();
    }

    public void Settings()
    {
        titleScreen.gameObject.SetActive(false);
        settingsScreen.gameObject.SetActive(true);
        settingReturn.gameObject.SetActive(true);
    }

    public void PauseSettings()
    {
        pauseScreen.gameObject.SetActive(false);
        settingsScreen.gameObject.SetActive(true);
        pauseSettingReturn.gameObject.SetActive(true);
    }
    public void ReturnPause()
    {
        // Enables the pause screen
        pauseScreen.gameObject.SetActive(true);

        // Disables not needed UI
        settingsScreen.gameObject.SetActive(false);
        pauseSettingReturn.gameObject.SetActive(false);
    }

    public void Return()
    {
        // Enables the title screen
        titleScreen.gameObject.SetActive(true);

        // Disables not needed UI
        difficultyScreen.gameObject.SetActive(false);
        settingsScreen.gameObject.SetActive(false);
        settingReturn.gameObject.SetActive(false);
        controlScreen.gameObject.SetActive(false);
    }

    public void Resume()
    {
        // Disables not needed UI
        pauseScreen.gameObject.SetActive(false);
        darkPanel.gameObject.SetActive(false);

        // Continues the game
        isGameActive = true;
        isGamePaused = false;
        gameAudio.UnPause();
    }

    public void Controls()
    {
        // Disables the title screen
        titleScreen.gameObject.SetActive(false);
        // Displays the controls screen
        controlScreen.gameObject.SetActive(true);
    }

    public void StartGame(int maxSpawn, int increment, int minSpawnRate, int maxSpawnRate)
    {
        // Disables the title screen
        difficultyScreen.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
        // Sets the booleans
        isGameActive = true;
        isGameStarted = true;
        isSpawning = true;
        isBossSpawned = false;

        // Sets the parameters
        maxSpawnedEnemy = maxSpawn;
        inreaseEnemyCount = increment;
        minRate = minSpawnRate;
        maxRate = maxSpawnRate;
        level = 1;
        spawnedEnemy = 0;

        // Plays the intro
        gameAudio.PlayOneShot(introSound, 2);

        // Activates gameobjects
        playerHealthScript.healthSlider.gameObject.SetActive(true);
        gun.gameObject.SetActive(true);

        // Displays the objective
        StartCoroutine(IntroText());
    }

    void Update()
    {
        // Counts the remaining enemies
        aliveEnemyCount = FindObjectsOfType<Enemy>().Length;

        CheckLevel();

        if (spawnedEnemy >= maxSpawnedEnemy && isGameActive)
        {
            isSpawning = false;

            // Checks if the level is done
            if (aliveEnemyCount == 0)
            {
                LimitPlayerPosition();

                // Increments the level
                if (Time.time > nextLevelStart)
                {
                    nextLevelStart = Time.time + levelInterval;
                    spawnAgain = false;
                    spawnedEnemy = 0;
                    level++;
                    maxSpawnedEnemy += inreaseEnemyCount;
                    powerupSpawnInterval--;
                    playerHealthScript.IncreaseHealth();
                    isLocIndicatorSpawned = false;
                }
            }
        }

        ProceedNextLevel();

        // Spawns enemies with interval
        if (isGameActive && isSpawning && spawnAgain)
        {
            if (waitTimer >= spawnEnemyInterval)
            {
                SpawnEnemy();

                waitTimer = 0f;
            }
            else
            {
                waitTimer += 1 * Time.deltaTime;
            }
        }

        // Spawn powerup each interval
        if (aliveEnemyCount != 0 && isGameActive)
        {
            if (powerupWaitTimer >= powerupSpawnInterval)
            {
                SpawnRandomPowerup();
                powerupWaitTimer = 0;
            }
            else
            {
                powerupWaitTimer += 1 * Time.deltaTime;
            }
        }

        //Check if the user has pressed the P key
        if (Input.GetKeyDown(KeyCode.P) && isGameActive)
        {
            isGameActive = false;
            isGamePaused = true;
            pauseScreen.gameObject.SetActive(true);
        }
        CheckPause();
    }
    
    // Method that pauses the game
    void CheckPause()
    {
        if (isGamePaused)
        {
            gameAudio.Pause();
            darkPanel.gameObject.SetActive(true);
            powerupText.gameObject.SetActive(false);
        }
        else
        {
            darkPanel.gameObject.SetActive(false);
            powerupText.gameObject.SetActive(true);
        }

        if (!isGameActive)
        {
            powerupText.gameObject.SetActive(false);
            bossText.gameObject.SetActive(false);
        }
    }

    public void GameOver()
    {
        StartCoroutine(GameOverScreen());
        // Set the gameactive as false
        isGameActive = false;
        gameAudio.PlayOneShot(gameOverSound, 4);
        gun.gameObject.SetActive(false);
        backgroundAudio.GetComponent<AudioSource>().Stop();
    }

    public void MissionComplete()
    {
        // Displays the mission complete screen
        missionCompleteScreen.gameObject.SetActive(true);
        // Set the gameactive as false
        isGameActive = false;
        gameAudio.PlayOneShot(endingSound, 4);
        backgroundAudio.GetComponent<AudioSource>().Stop();
    }

    public void RestartGame()
    {
        // Plays the game from the beginning
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Checks if the player can proceed to the next level
    public void ProceedNextLevel()
    {
        if (!spawnAgain && isGameActive)
        {
            levelClearScreen.gameObject.SetActive(true);
            locationArrow.gameObject.SetActive(true);
            playerLight.gameObject.SetActive(true);
            if (!isProceedPlayed)
            {
                gameAudio.PlayOneShot(doneSound, 2);
                isProceedPlayed = true;
            }
        }
        else
        {
            levelClearScreen.gameObject.SetActive(false);
            locationArrow.gameObject.SetActive(false);
            playerLight.gameObject.SetActive(false);
            isProceedPlayed = false;
        }
    }

    // Spawn enemies on the amp
    private void SpawnEnemy()
    {
        spawnRate = Random.Range(minRate, maxRate);
        for (int i = 0; i < spawnRate; i++)
        {
            // Spawns the random enemy
            int index = Random.Range(0, enemies.Count);
            Instantiate(enemies[index], RandomSpawnPos(spawnPositions), enemies[index].transform.rotation);
        }
        spawnedEnemy += spawnRate;
    }

    // Spawn the boss on the map
    private void SpawnBoss()
    {
        Vector3 bossSpawnPos = new Vector3(-34.5f, spawnPosY, -112.5f);
        Instantiate(bossPrefab, bossSpawnPos, bossPrefab.transform.rotation);
        Instantiate(bossSpawnEffects, bossSpawnPos, bossSpawnEffects.transform.rotation);
        gameAudio.PlayOneShot(BossSpawnSound, 4);
        bossText.gameObject.SetActive(true);
    }

    // Generates a random spawn position index
    private Vector3 RandomSpawnPos(Vector3[] pos)
    {
        int spawnIndex = Random.Range(0, pos.Length);
        return pos[spawnIndex];
    }

    private Vector3 GenerateRandomSpawnPointTop(float spawnPosXLeft, float spawnPosXRight, float spawnPosZ)
    {
        float spawnPosX = Random.Range(spawnPosXLeft, spawnPosXRight);
        Vector3 randomSpawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        return randomSpawnPos;
    }

    private Vector3 GenerateRandomSpawnPointDown(float spawnPosXLeft, float spawnPosXRight, float spawnPosZ)
    {
        float spawnPosX = Random.Range(spawnPosXLeft, spawnPosXRight);
        Vector3 randomSpawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        return randomSpawnPos;
    }

    private Vector3 GenerateRandomSpawnPointLeft(float spawnPosX, float spawnPosZTop, float spawnPosZDown)
    {
        float spawnPosZ = Random.Range(spawnPosZDown, spawnPosZTop);
        Vector3 randomSpawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        return randomSpawnPos;
    }

    private Vector3 GenerateRandomSpawnPointRight(float spawnPosX, float spawnPosZTop, float spawnPosZDown)
    {
        float spawnPosZ = Random.Range(spawnPosZDown, spawnPosZTop);
        Vector3 randomSpawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        return randomSpawnPos;
    }

    // Check the level and adjust the parameters accordingly
    public void CheckLevel()
    {
        if (level == 1)
        {
            spawnPositions = new Vector3[1];
            spawnPositions[0] = GenerateRandomSpawnPointTop(-145, -100, -107);

            isSpawning = true;
            spawnAgain = true;

            lvl1Light.gameObject.SetActive(true);
        }
        else
        {
            lvl1Light.gameObject.SetActive(false);
        }

        if (level == 2)
        {
            spawnPositions = new Vector3[2];
            spawnPositions[0] = GenerateRandomSpawnPointTop(-146, -106, -47);
            spawnPositions[1] = GenerateRandomSpawnPointDown(-145, -100, -125);

            Vector3 spawnPos = new Vector3(-123.6f, powerupYPos, -97.75f);
            SpawnLocIndicator(spawnPos);
            isSpawning = true;
            lvl2Light.gameObject.SetActive(true);
        }
        else
        {
            lvl2Light.gameObject.SetActive(false);
        }

        if (level == 3)
        {
            spawnPositions = new Vector3[3];
            spawnPositions[0] = GenerateRandomSpawnPointLeft(-121.5f, -67, -74);
            spawnPositions[1] = GenerateRandomSpawnPointRight(-55, -67, -70);
            spawnPositions[2] = GenerateRandomSpawnPointDown(-96, -89.5f, -91);

            Vector3 spawnPos = new Vector3(-92.5f, powerupYPos, -71);
            SpawnLocIndicator(spawnPos);
            isSpawning = true;
            lvl3Light.gameObject.SetActive(true);
        }
        else
        {
            lvl3Light.gameObject.SetActive(false);
        }

        if (level == 4)
        {
            spawnPositions = new Vector3[4];
            spawnPositions[0] = GenerateRandomSpawnPointLeft(-121.5f, -107, -118);
            spawnPositions[1] = GenerateRandomSpawnPointRight(-63.5f, -107, -118);
            spawnPositions[2] = GenerateRandomSpawnPointDown(-96, -89.5f, -145);
            spawnPositions[3] = GenerateRandomSpawnPointTop(-96, -89.5f, -82);

            Vector3 spawnPos = new Vector3(-92.5f, powerupYPos, -112);
            SpawnLocIndicator(spawnPos);
            isSpawning = true;
            lvl4Light.gameObject.SetActive(true);
        }
        else
        {
            lvl4Light.gameObject.SetActive(false);
        }

        if (level == 5)
        {
            Vector3 spawnPos = new Vector3(-48f, powerupYPos, -112.5f);
            SpawnLocIndicator(spawnPos);
            isSpawning = false;

            if (spawnAgain)
            {
                president.gameObject.SetActive(true);
                bossText.gameObject.SetActive(true);

                if (!isBossSpawned)
                {
                    SpawnBoss();
                    isBossSpawned = true;
                }

                if (aliveEnemyCount == 0)
                {
                    bossText.gameObject.SetActive(false);
                }
            }

            lvl5Light.gameObject.SetActive(true);
        }
        else
        {
            lvl5Light.gameObject.SetActive(false);
        }
    }

    // Spawns enemies beside the boss
    public void SpawnMinions()
    {
        spawnPositions = new Vector3[1];
        spawnPositions[0] = GenerateRandomSpawnPointRight(-10, -107, -118);
        SpawnEnemy();
    }

    // Generates a random position for the powerup
    private Vector3 GenerateRandomPowerupSpawnPoint()
    {
        float xRangeLeft = playerControllerScript.xRangeLeft;
        float xRangeRight = playerControllerScript.xRangeRight;
        float zRangeDown = playerControllerScript.zRangeDown;
        float zRangeTop = playerControllerScript.zRangeTop;

        // Generate random position
        float xRange = Random.Range(xRangeLeft, xRangeRight);
        float zRange = Random.Range(zRangeDown, zRangeTop);

        Vector3 randomPowerupSpawnPos = new Vector3(xRange, powerupYPos, zRange);
        return randomPowerupSpawnPos;
    }

    // Spawns a random powerup on the map
    public void SpawnRandomPowerup()
    {
        int powerupIndex = Random.Range(0, powerups.Count);
        Instantiate(powerups[powerupIndex], GenerateRandomPowerupSpawnPoint(), powerups[powerupIndex].transform.rotation);
    }

    // Spawns the location indicator for the next level
    private void SpawnLocIndicator(Vector3 pos)
    {
        if (isLocIndicatorSpawned == false)
        {
            Instantiate(locIndicatorPrefarb, pos, locIndicatorPrefarb.transform.rotation);
            isLocIndicatorSpawned = true;
        }
    }

    // Set position boundaries for the player
    private void LimitPlayerPosition()
    {
        playerControllerScript.xRangeLeft = -161.5f;
        playerControllerScript.xRangeRight = 18;
        playerControllerScript.zRangeDown = -142;
        playerControllerScript.zRangeTop = -63;
    }

    // Spawns the helicopter
    public void SpawnHelicopter()
    {
        Vector3 heliSpawnPos = new Vector3(-35, 10, -112.5f);
        Instantiate(helicopterPrefab, heliSpawnPos, helicopterPrefab.transform.rotation);
    }

    // Hides the president from the map
    public void DespawnPresident()
    {
        president.gameObject.SetActive(false);
    }

    // Effects
    // Plays the kamikaze explosion effect
    public void KamikazeExplosion(Vector3 pos)
    {
        Instantiate(explosionEffects, pos + new Vector3(0, 0.5f, 0), transform.rotation);
        gameAudio.PlayOneShot(kamikazeSound, 2);
    }

    // Plays the bullet hit effect
    public void BloodEffect(Vector3 pos)
    {
        Instantiate(bloodEffects, pos + new Vector3(0, 0.5f, 0), player.transform.rotation);
        gameAudio.PlayOneShot(bulletHitSound, 0.04f);
    }

    // Plays the laser hit effect
    public void SparkEffect(Vector3 pos)
    {
        Instantiate(sparkEffects, pos + new Vector3(0, 0.5f, 0), transform.rotation);
        gameAudio.PlayOneShot(laserHitSound, 0.4f);
    }

    // Plays the grenade hit effect
    public void GrenadeEffect(Vector3 pos)
    {
        Instantiate(grenadeEffects, pos + new Vector3(0, 0.5f, 0), transform.rotation);
        gameAudio.PlayOneShot(explosionSound, 2);
    }

    // Plays the rock hit effect
    public void RockEffect(Vector3 pos)
    {
        Instantiate(rockEffects, pos + new Vector3(0, 0.5f, 0), transform.rotation);
        gameAudio.PlayOneShot(rockHitSound, 0.4f);
    }

    // Plays the enemy melee hit effect
    public void HitEffect(Vector3 pos)
    {
        Instantiate(hitEffects, pos + new Vector3(0, 0.5f, 0), transform.rotation);
    }

    // Plays the radiate hit effect
    public void PoofEffect(Vector3 pos)
    {
        Instantiate(poofEffects, pos + new Vector3(0, 0.5f, 0), transform.rotation);
        gameAudio.PlayOneShot(radiateHitSound, 1.2f);
    }

    // Plays the slow effect
    public void SlowEffect(Vector3 pos)
    {
        Instantiate(slowEffects, pos + new Vector3(0, 0.5f, 0), transform.rotation);
    }

    // Plays the gunshot sound effect
    public void GunShot()
    {
        gameAudio.PlayOneShot(bulletSound, 0.4f);
    }

    // Plays the laser sound effect
    public void LaserSoundEffect()
    {
        gameAudio.PlayOneShot(laserSound, 2);
    }

    // Plays the grenade sound effect
    public void GrenadeSoundEffect()
    {
        gameAudio.PlayOneShot(grenadeSound, 2);
    }

    // Plays the undo slow effect
    public void UndoSlowSoundEffect()
    {
        gameAudio.PlayOneShot(undoSlowSound, 2);
    }

    // Plays the block effect
    public void ShieldHitSoundEffect()
    {
        gameAudio.PlayOneShot(shieldHitSound, 2);
    }

    // Plays the regen effect
    public void RegenEffect()
    {
        Instantiate(regenEffects, player.transform.position + new Vector3(0, 0.5f, 0), regenEffects.transform.rotation);
    }

    // Plays the boss death effect
    public void BossDeathEffect(Vector3 pos)
    {
        gameAudio.PlayOneShot(BossDeathSound, 2);
        Instantiate(bossDeathEffects, pos + new Vector3(0, 0.5f, 0), transform.rotation);
    }

    // Plays enemy death sound effect
    public void EnemyDeath()
    {
        gameAudio.PlayOneShot(enemyDeathSound, 0.5f);
    }

    // Plays titan death sound effect
    public void TitanDeath()
    {
        gameAudio.PlayOneShot(titanDeathSound, 0.5f);
    }

    // Plays the muzzle effect
    public void MuzzleEffect(Transform spawner)
    {
        var muzzleVFX = Instantiate(muzzleEffects, spawner.transform.position, Quaternion.identity);
        muzzleVFX.transform.forward = spawner.transform.forward;
    }

    IEnumerator GameOverScreen()
    {
        yield return new WaitForSeconds(4);
        // Displays the gameover screen
        gameOverScreen.gameObject.SetActive(true);
    }

    IEnumerator IntroText()
    {
        // Displays the intro text after 7 seconds
        yield return new WaitForSeconds(7);
        introText.gameObject.SetActive(true);
        // Hides it after 10 seconds
        yield return new WaitForSeconds(10);
        introText.gameObject.SetActive(false);
    }
}