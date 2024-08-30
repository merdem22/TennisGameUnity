using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class enemyAI : MonoBehaviour
{
    //movement related.
    [SerializeField] private float movementSpeed;
    [SerializeField] private float anticipationTime;
    public static GameObject ballObject;
    private Rigidbody ballRigidbody;
    private Vector3 targetPosition;


    //court size related.
    [SerializeField] private GameObject court; //we require the length and the size of the court.
    private float courtWidth;
    private float courtLength;
    private float netPosition = 0f;

    void Start()
    { 
        courtWidth = court.transform.localScale.x;
        courtLength = court.transform.localScale.z;
        Debug.Log("Court width:" + courtWidth);
        Debug.Log("Court length: " + courtLength);
    }

    
    void Update()
    {
        if (ballObject != null)
        {
            positionForBall();
            moveToTarget();
        }
        
    }

    private void positionForBall()
    {
        ballRigidbody = ballObject.GetComponent<Rigidbody>();
        Vector3 ballPosition = ballObject.transform.position;
        Vector3 ballVelocity = ballRigidbody.velocity;


        Vector3 anticipatedPosition = ballPosition + (ballVelocity * anticipationTime);

        //determine if the ball is on the side of the enemy.
        bool ballOnAISide = (transform.position.z > netPosition && anticipatedPosition.z > netPosition) ||
                            (transform.position.z < netPosition && anticipatedPosition.z < netPosition);
        Debug.Log("bool (Ball is on AI side): " + ballOnAISide);
        if (ballOnAISide)
        {
            targetPosition = new Vector3(anticipatedPosition.x, transform.position.y, anticipatedPosition.z);
        }
        else //ball is on the other side so its best to move to a neutral position.
        {
            float neutralZ = transform.position.z > netPosition ? courtLength * 0.75f : -courtLength * 0.75f;
            targetPosition = new Vector3(0, transform.position.y, neutralZ);
        }

        // Clamp the target position within the court boundaries
        targetPosition.x = Mathf.Clamp(targetPosition.x, -courtWidth / 2, courtWidth / 2);
        targetPosition.z = Mathf.Clamp(targetPosition.z, netPosition, courtLength / 2);
    }

    private void moveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
    }
}
