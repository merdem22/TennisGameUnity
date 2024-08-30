using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class velocityCalculator : MonoBehaviour //this class helps to get the velocities of the objects that don't have a rigidbody, the racket is a trigger and i need its velocity so this is useful.
{
    private Vector3 previousLocation;
    private Vector3 velocity;
    public float smoothingFactor = 0.5f;

    void Start()
    {
        previousLocation = transform.position;
    }

    void Update()
    {
        //get raw velocity.
        Vector3 rawVelocity = (transform.position - previousLocation) / Time.deltaTime;
        //apply smoothing (useful if there are instant big changes in velocity).
        velocity = Vector3.Lerp(velocity, rawVelocity, smoothingFactor);

        previousLocation = transform.position;
    }

    public Vector3 getVelocity()
    {
        return velocity;
    }
}
