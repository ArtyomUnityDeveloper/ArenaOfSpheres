using UnityEngine;
public enum PowerUpType { None, Pushback, Rockets, Smash }
public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType; // Allows you to choose powerUp type in the corresponding component
}
