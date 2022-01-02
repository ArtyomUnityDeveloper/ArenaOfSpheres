using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    private Transform target;
    private float speed = 15.0f;
    private bool homing;
    private float rocketStrength = 15.0f;
    private float aliveTimer = 5.0f;


    public void Fire(Transform newTarget)
    {
        target = newTarget; // Was: target = homingTarget;
        homing = true;
        Destroy(gameObject, aliveTimer);
    }



    // Update is called once per frame
    void Update()
    {

        if (homing && target != null) // In if statement - the code which moves the rocket to target
        {
            Vector3 moveDirection = (target.transform.position - transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
            transform.LookAt(target);
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (target != null)
        {
            if (col.gameObject.CompareTag(target.tag)) // Here we compare the tag of the colliding object with the tag of the target
            {
                Rigidbody targetRigidbody = col.gameObject.GetComponent<Rigidbody>(); // If they match, we get the rigidbody of the target
                Vector3 away = -col.contacts[0].normal; // We then use the normal of the
                // collision contact to determine which direction to push the target in
                targetRigidbody.AddForce(away * rocketStrength, ForceMode.Impulse); // Finally we apply the force to the
                // target and destroy the missile
                Destroy(gameObject);
            }
        }
    }

}
