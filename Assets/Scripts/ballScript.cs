using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballScript : MonoBehaviour
{
    public float maxHitForce;
    public float spinMultiplier;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("tennisRacket"))
        {
            Vector3 racketVelocity = collision.gameObject.GetComponent<racketScript>().getVelocity(); //this won't work, bc racket is a kinematic so it doesn't have pyhsics.
            Vector3 hitForce = (racketVelocity * maxHitForce);

            rigidbody.AddForce(hitForce, ForceMode.Impulse);
        }
    }
}
