using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputHandler _input;

    [SerializeField]
    private bool RotateTowardMouse;

    [SerializeField]
    private float MovementSpeed = 7;
    [SerializeField]
    private float RotationSpeed;

    [SerializeField]
    private Camera Camera;

    public GameObject[] projectilePrefabs;
    public Transform projectileSpawnPoint;
    private int projectileIndex = 0;

    private Animator playerAnim;
    private PlayerHealth playerHealth;
    public float fireRate = 0.1f;
    private float nextFire;

    public float xRangeLeft;
    public float xRangeRight;
    public float zRangeTop;
    public float zRangeDown;
    private float yLimit = -35.4438f;

    public bool proceedLevel = false;
    public bool proceedBossLevel = false;

    private GameManager gameManager;

    public PowerUpType currentPowerUp = PowerUpType.None;
    public bool hasPowerup = false;
    private Coroutine powerupCountdown;
    public bool isShielded = false;
    public bool isSlowed = false;
    public bool isRadiating = false;

    // Effect booleans
    private bool isLaserEffectPlayed = false;
    private bool isExplosiveEffectPlayed = false;
    private bool isRageEffectPlayed = false;
    private bool isShieldEffectPlayed = false;
    private bool isSlowEffectPlayed = false;
    private bool isUndoSlowEffectPlayed = false;
    private bool fromSlow = false;
    private bool isRadiateEffectPlayed = false;

    // Effects
    public ParticleSystem laserEffects;
    public ParticleSystem explosiveEffects;
    public ParticleSystem rageEffects;
    public ParticleSystem shieldEffects;
    public ParticleSystem radiateEffects;
    public ParticleSystem slowEffects;

    // Powerup Texts
    public GameObject explosiveText;
    public GameObject laserText;
    public GameObject radiateText;
    public GameObject rageText;
    public GameObject shieldText;
    public GameObject slowText;

    private void Awake()
    {
        _input = GetComponent<InputHandler>();
        playerAnim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive)
        {
            // For player controls and movement
            var targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);
            var movementVector = MoveTowardTarget(targetVector);

            if (!RotateTowardMouse)
            {
                RotateTowardMovementVector(movementVector);
            }
            if (RotateTowardMouse)
            {
                RotateFromMouseVector();
            }
            if (Input.GetKey(KeyCode.Mouse0) && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                SpawnProjectile();
                ProjectileSoundEffects();
            }
        }

        CheckLevel();
        CheckPowerUp();

        // Resets the proceedLevel boolean
        if (gameManager.spawnedEnemy == 0)
        {
            proceedLevel = false;
        }

        // Limits the postion of the player within the map
        LimitPosition();

        // Resets the player's position if the game is not started yet
        if (gameManager.isGameStarted == false)
        {
            ResetPostion();
        }
    }

    private void RotateFromMouseVector()
    {
        // The player will look at the mouse pointer
        Ray ray = Camera.ScreenPointToRay(_input.MousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }

    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        // Calculates the target vector of the movement
        var speed = MovementSpeed * Time.deltaTime;

        targetVector = Quaternion.Euler(0, Camera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
        return targetVector;
    }

    private void RotateTowardMovementVector(Vector3 movementDirection)
    {
        // Rotate with movement
        if (movementDirection.magnitude == 0) { return; }
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, RotationSpeed);
    }

    void SpawnProjectile()
    {
        // Spawns the projectile into the map
        Instantiate(projectilePrefabs[projectileIndex], projectileSpawnPoint.position, projectileSpawnPoint.transform.rotation);
        gameManager.MuzzleEffect(projectileSpawnPoint);
    }

    void ProjectileSoundEffects()
    {
        // Plays the sound effect for each projectile
        if (projectileIndex == 0)
        {
            gameManager.GunShot();
        }

        if (projectileIndex == 1)
        {
            gameManager.LaserSoundEffect();
        }

        if (projectileIndex == 2)
        {
            gameManager.GrenadeSoundEffect();
        }
    }

    void CheckLevel()
    {

        if (gameManager.level == 1)
        {
            // Sets the boundaries
            xRangeLeft = -145;
            xRangeRight = -106;
            zRangeDown = -142;
            zRangeTop = -125;
        }

        if (gameManager.level == 2 && gameManager.spawnAgain)
        {
            // Sets the boundaries
            xRangeLeft = -145;
            xRangeRight = -106;
            zRangeDown = -102;
            zRangeTop = -79.5f;
        }

        if (gameManager.level == 3 && gameManager.spawnAgain)
        {
            // Sets the boundaries
            xRangeLeft = -100;
            xRangeRight = -79.5f;
            zRangeDown = -84;
            zRangeTop = -62;
        }

        if (gameManager.level == 4 && gameManager.spawnAgain)
        {
            // Sets the boundaries
            xRangeLeft = -100;
            xRangeRight = -79;
            zRangeDown = -126.5f;
            zRangeTop = -102.5f;
        }

        if (gameManager.level == 5)
        {
            if (gameManager.spawnAgain)
            {
                // Sets the boundaries
                xRangeLeft = -60;
                xRangeRight = -33;
                zRangeDown = -118;
                zRangeTop = -107;

                // Adjust the zRange top and points the arrow to the president
                if (gameManager.aliveEnemyCount == 0)
                {
                    zRangeTop = -87.5f;

                    // Find the president
                    gameManager.locationArrow.transform.LookAt(gameManager.president.transform);
                    gameManager.locationArrow.gameObject.SetActive(true);
                }
            }
        }
    }

    void CheckPowerUp()
    {
        if (currentPowerUp == PowerUpType.None)
        {
            projectileIndex = 0;
            fireRate = 0.1f;
            MovementSpeed = 5;
            isShielded = false;
            isSlowed = false;
            isRadiating = false;
        }

        // Launch lasers
        if (currentPowerUp == PowerUpType.Laser && !playerHealth.isDead)
        {
            projectileIndex = 1;
            fireRate = 0.2f;
            MovementSpeed = 5;
            isShielded = false;
            isSlowed = false;
            isRadiating = false;

            laserText.gameObject.SetActive(true);
            laserEffects.gameObject.SetActive(true);
            if (!isLaserEffectPlayed)
            {
                laserEffects.Play();
                laserEffects.GetComponent<AudioSource>().PlayOneShot(gameManager.laserChargingSound, 3);
                isLaserEffectPlayed = true;
            }
        }
        else
        {
            laserEffects.GetComponent<AudioSource>().Stop();
            laserText.gameObject.SetActive(false);
            laserEffects.gameObject.SetActive(false);
            isLaserEffectPlayed = false;
        }

        // Launch grenades
        if (currentPowerUp == PowerUpType.Grenade && !playerHealth.isDead)
        {
            projectileIndex = 2;
            fireRate = 0.5f;
            MovementSpeed = 5;
            isShielded = false;
            isSlowed = false;
            isRadiating = false;

            explosiveText.gameObject.SetActive(true);
            explosiveEffects.gameObject.SetActive(true);
            if (!isExplosiveEffectPlayed)
            {
                explosiveEffects.Play();
                explosiveEffects.GetComponent<AudioSource>().PlayOneShot(gameManager.grenadeIndicatorSound, 3);
                isExplosiveEffectPlayed = true;
            }
        }
        else
        {
            explosiveText.gameObject.SetActive(false);
            explosiveEffects.gameObject.SetActive(false);
            explosiveEffects.GetComponent<AudioSource>().Stop();
            isExplosiveEffectPlayed = false;
        }

        // Increases the fire rate and the movement speed of the player
        if (currentPowerUp == PowerUpType.Rage && !playerHealth.isDead)
        {
            fireRate = 0.05f;
            MovementSpeed = 10;
            projectileIndex = 0;
            isShielded = false;
            isSlowed = false;
            isRadiating = false;
            rageEffects.gameObject.SetActive(true);
            rageText.gameObject.SetActive(true);
            if (isRageEffectPlayed == false)
            {
                rageEffects.Play();
                rageEffects.GetComponent<AudioSource>().PlayOneShot(gameManager.rageSound, 1.2f);
                isRageEffectPlayed = true;
            }
        }
        else
        {
            rageEffects.GetComponent<AudioSource>().Stop();
            rageEffects.gameObject.SetActive(false);
            rageText.gameObject.SetActive(false);
            isRageEffectPlayed = false;
        }

        // The player will not take any damage
        if (currentPowerUp == PowerUpType.Shield && !playerHealth.isDead)
        {
            isShielded = true;
            projectileIndex = 0;
            fireRate = 0.1f;
            MovementSpeed = 5;
            isSlowed = false;
            isRadiating = false;
            shieldEffects.gameObject.SetActive(true);
            shieldText.gameObject.SetActive(true);
            if (isShieldEffectPlayed == false)
            {
                shieldEffects.Play();
                shieldEffects.GetComponent<AudioSource>().PlayOneShot(gameManager.shieldSound, 2);
                isShieldEffectPlayed = true;
            }
        }
        else
        {
            shieldEffects.GetComponent<AudioSource>().Stop();
            shieldEffects.gameObject.SetActive(false);
            shieldText.gameObject.SetActive(false);
            isShieldEffectPlayed = false;
        }

        // The player will freeze the enemies
        if (currentPowerUp == PowerUpType.Slow && !playerHealth.isDead)
        {
            isSlowed = true;
            projectileIndex = 0;
            fireRate = 0.1f;
            MovementSpeed = 5;
            isShielded = false;
            isRadiating = false;
            slowEffects.gameObject.SetActive(true);
            slowText.gameObject.SetActive(true);
            if (isSlowEffectPlayed == false)
            {
                slowEffects.Play();
                slowEffects.GetComponent<AudioSource>().PlayOneShot(gameManager.slowSound, 2);
                isSlowEffectPlayed = true;
                fromSlow = true;
                isUndoSlowEffectPlayed = false;
            }
        }
        else
        {
            slowEffects.GetComponent<AudioSource>().Stop();
            slowEffects.gameObject.SetActive(false);
            slowText.gameObject.SetActive(false);
            isSlowEffectPlayed = false;
            if (isUndoSlowEffectPlayed == false && fromSlow == true)
            {
                gameManager.UndoSlowSoundEffect();
                isUndoSlowEffectPlayed = true;
                fromSlow = false;
            }
        }

        // The health of the enemies will decrease each second
        if (currentPowerUp == PowerUpType.Radiate && !playerHealth.isDead)
        {
            isRadiating = true;
            projectileIndex = 0;
            fireRate = 0.1f;
            MovementSpeed = 5;
            isShielded = false;
            isSlowed = false;
            radiateEffects.gameObject.SetActive(true);
            radiateText.gameObject.SetActive(true);
            if (isRadiateEffectPlayed == false)
            {
                radiateEffects.Play();
                radiateEffects.GetComponent<AudioSource>().PlayOneShot(gameManager.radiateSound, 2);
                isRadiateEffectPlayed = true;
            }
        }
        else
        {
            radiateEffects.GetComponent<AudioSource>().Stop();
            radiateEffects.gameObject.SetActive(false);
            radiateText.gameObject.SetActive(false);
            isRadiateEffectPlayed = false;
        }
    }

    void LimitPosition()
    {
        // Limits the player position on x, y, and z axis
        if (transform.position.x < xRangeLeft)
        {
            transform.position = new Vector3(xRangeLeft, transform.position.y, transform.position.z);
        }
        if (transform.position.x > xRangeRight)
        {
            transform.position = new Vector3(xRangeRight, transform.position.y, transform.position.z);
        }
        if (transform.position.z < zRangeDown)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zRangeDown);
        }
        if (transform.position.z > zRangeTop)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zRangeTop);
        }
        if (transform.position.y < yLimit)
        {
            transform.position = new Vector3(transform.position.x, yLimit, transform.position.z);
        }
    }

    void ResetPostion()
    {
        // Reset the postion of the player
        transform.position = new Vector3(-135, -35.4438f, -139);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Camera.gameObject.transform.position = new Vector3(-135, -25, -144);
    }

    public void OnTriggerEnter(Collider other)
    {
        // Proceed level if the player picked the location indicator
        if (other.CompareTag("LocIndicator"))
        {
            Destroy(other.gameObject);
            gameManager.spawnAgain = true;
            proceedLevel = true;
            gameManager.gameAudio.PlayOneShot(gameManager.proceedSound, 2);
        }

        if (other.CompareTag("Powerup"))
        {
            // Activates the powerup for a short time
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            Destroy(other.gameObject);

            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }

        if (other.CompareTag("Regen"))
        {
            // Regenerates player health
            Destroy(other.gameObject);
            playerHealth.ResetHealth();
            gameManager.gameAudio.PlayOneShot(gameManager.regenSound, 1.2f);
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        while(gameManager.isGameActive)
        {
            // Serves as the countdown of the powerup
            yield return new WaitForSeconds(8);
            hasPowerup = false;
            // Returns the currentPowerUp to none
            currentPowerUp = PowerUpType.None;
        }
    }
}