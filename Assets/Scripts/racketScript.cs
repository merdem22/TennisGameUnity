using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class racketScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject rotationPivot;
    public float maxRotation;
    public float rotationSpeed;
    public float releaseSpeed;
   
    private float previousRotation;
    private float currentRotation = 0f;

    private Vector3 linearVelocity;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        previousRotation = currentRotation;

        if (Input.GetButton("Fire1"))
        {
            currentRotation = Mathf.MoveTowards(currentRotation, maxRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            currentRotation = Mathf.MoveTowards(currentRotation, 0f, releaseSpeed * Time.deltaTime);
        }
        //apply rotation
        transform.RotateAround(rotationPivot.transform.position, Vector3.up, currentRotation - transform.localRotation.eulerAngles.y);

        float angularVelocity = (currentRotation - previousRotation) * Mathf.Deg2Rad / Time.deltaTime;

        // Calculate length from pivot to center to calculate linear velocity.
        Vector3 radius = transform.position - rotationPivot.transform.position;

        // Calculate linear velocity
        linearVelocity = Vector3.Cross(Vector3.up * angularVelocity, radius);

        Debug.Log("x:" + getVelocity().x);
        Debug.Log("y:" + getVelocity().y);
        Debug.Log("z:" + getVelocity().z);

    }

    public Vector3 getVelocity()
    {
        CharacterController controller = GetComponentInParent<CharacterController>();
        return controller.velocity + linearVelocity; //velocity of due to character + linear velocity due to rotation
    
    }

}
