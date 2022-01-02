using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("General Settings")]
    private Rigidbody playerRb;
    public float speed = 5.0f;
    private GameObject focalPoint;
    public PowerUpType currentPowerUp = PowerUpType.None; // This is used to help determine which
    // logic to enable for the player when a power up is collected

    [Header("PowerUp: Pushback")]
    public bool hasPowerup;
    private float powerUpStrenght = 15.0f;
    public GameObject powerupIndicator;


    [Header("PowerUp: Rockets")]
    public GameObject rocketPrefab;
    private GameObject tmpRocket; // will be used for spawning in the homing rockets
    private Coroutine powerupCountdown;

    [Header("PowerUp: Smash")]
    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    bool smashing = false;
    float floorY;



    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
        }

        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType; // Appeal to enum
            Destroy(other.gameObject);
            powerupIndicator.gameObject.SetActive(true);

            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerUpCountdownRoutine());

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);
            enemyRigidbody.AddForce(awayFromPlayer * powerUpStrenght, ForceMode.Impulse);
            Debug.Log("Player collided with: " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
        }
    }

    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            // launch the missiles from above the player, to stop the collision from pushing us back
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity); 
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }


    IEnumerator PowerUpCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        currentPowerUp = PowerUpType.None; // Return to None type of PowerUp
        powerupIndicator.gameObject.SetActive(false);
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();
        //Store the y position before taking off
        floorY = transform.position.y;
        //Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;
        while (Time.time < jumpTime)
        {
            //move the player up while still keeping their x velocity.
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }
        //Now move the player down
        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }
        //Cycle through all enemies.
        for (int i = 0; i < enemies.Length; i++)
        {
            //Apply an explosion force that originates from our position.
            if (enemies[i] != null)
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
        }
        //We are no longer smashing, so set the boolean to false
        smashing = false;
    }

}
