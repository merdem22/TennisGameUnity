using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballScript : MonoBehaviour
{
    public float maxHitForce;
    [SerializeField] private float maxBallSpeed;
    private Rigidbody rb;
    

    // Start is called before the first frame update

    void Awake()
    {
        references.maxBallSpeed = maxBallSpeed;
        rb = GetComponent<Rigidbody>();
        
    }

    private void Update()
    {
        //limit the ball's max speed.
        if (rb.velocity.magnitude > maxBallSpeed) { rb.velocity = rb.velocity.normalized * maxBallSpeed; } 
    }

}
