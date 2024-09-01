using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class enemyAI : MonoBehaviour
{
    //movement related.
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxAnticipationTime;
    [SerializeField] private float minAnticipationTime;
    [SerializeField] private float closenessRange;
    public static GameObject ballObject;
    private Rigidbody ballRigidbody;
    private Vector3 targetPosition;


    //court size related.
    [SerializeField] private GameObject court; //we require the length and the size of the court.
    private float courtWidth;
    private float courtLength;
    private float courtGroundLevel; 
    private float netPosition = 0f;
    private bool ballIsClose = false;

    void Start()
    { 
        courtWidth = court.transform.localScale.x;
        courtLength = court.transform.localScale.z;
        courtGroundLevel = court.transform.position.y + court.transform.localScale.y / 2; 

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

        float distance = Mathf.Abs((transform.position - ballPosition).magnitude);

        if (distance < closenessRange) { ballIsClose = true; }
        else { ballIsClose = false; }

        Debug.Log("Distance of ball to player: " + distance);
        Debug.Log("ball is close:" + ballIsClose);

        //we want the enemy to be more precise and small in its anticipations for the ball when its close.
        float anticipationTime = getAnticipationTime();
        Debug.Log("anticipationTime:" + anticipationTime);
        Vector3 anticipatedPosition = ballPosition + (anticipationTime * ballVelocity);

        
        //Vector3 anticipatedPosition = getAccurateBallPrediction();

        //determine if the ball is on the side of the enemy.
        bool ballOnAISide = (transform.position.z > netPosition && anticipatedPosition.z > netPosition && anticipatedPosition.z < courtLength) ||
                            (transform.position.z < netPosition && anticipatedPosition.z < netPosition && anticipatedPosition.z > -courtLength);
                           
        Debug.Log("bool (Ball is on AI side): " + ballOnAISide);
        if (ballOnAISide)
        {
            targetPosition = new Vector3(anticipatedPosition.x, transform.position.y, anticipatedPosition.z);
        }
        else //ball is on the other side so its best to move to a neutral position.
        {
            float neutralZ = transform.position.z > netPosition ? courtLength * 0.25f : -courtLength * 0.25f ;
            targetPosition = new Vector3(0, transform.position.y, neutralZ);
        }

        // Clamp the target position within the court boundaries
        targetPosition.x = Mathf.Clamp(targetPosition.x, -courtWidth / 2, courtWidth / 2);
        targetPosition.z = Mathf.Clamp(targetPosition.z, netPosition, courtLength/2);
    }

    private void moveToTarget()
    {
        // Smoothly interpolate between the current position and the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
    }

    //this should be used when the ball gets close to the character.
    private Vector3 getAccurateBallPrediction() //not sure if i am going to use this.
    {
        Vector3 ballPosition = ballObject.transform.position;
        Vector3 ballVelocity = ballRigidbody.velocity;

        // physics :O  
        float g = (Physics.gravity).magnitude; //hope this is 9.81 on default.
        float h = (ballPosition.y - courtGroundLevel);

        float maxHeightReached = h + (ballVelocity.y * ballVelocity.y) / 2 * g;
        float timeToFall = (ballVelocity.y / g) + Mathf.Sqrt(2 * maxHeightReached / g); //total time left on air = (time to reach max height + time to fall from max height)

        return ballPosition + (timeToFall * ballVelocity);

    }

    //sets anticipation time based on the distance from ball to player, smoother this way, it graudally decreases as the ball gets closer to the palyer.
    private float getAnticipationTime()
    {
        
        float distanceToBall = Vector3.Distance(transform.position, ballObject.transform.position);


        // Define the maximum and minimum distances where the anticipation time will vary
        float maxDistance = courtLength / 2;
        float minDistance = 1f;
        float anticipationTime;
        
        anticipationTime = Mathf.Lerp(minAnticipationTime, maxAnticipationTime, (distanceToBall - minDistance) / (maxDistance - minDistance));

        // Clamp the anticipation time to ensure it stays within the defined range
        anticipationTime = Mathf.Clamp(anticipationTime, minAnticipationTime, maxAnticipationTime);
        return anticipationTime;
    }


}
