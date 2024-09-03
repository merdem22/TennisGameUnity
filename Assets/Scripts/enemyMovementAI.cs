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
    [SerializeField] private GameObject characterReference;



    public static GameObject ballObject;
    private Rigidbody ballRigidbody;
    private Vector3 targetPosition;


    //court size related.
    [SerializeField] private GameObject court; //we require the length and the size of the court.
    private float courtWidth;
    private float courtLength;
    private float courtGroundLevel; 
    private float netPosition = 0f;

    private opponentHitAi referenceToHitScript;
    private bool hitAvailable = true; //value known from hit script.

    void Start()
    { 
        courtWidth = court.transform.localScale.x;
        courtLength = court.transform.localScale.z;
        courtGroundLevel = court.transform.position.y + court.transform.localScale.y / 2; 

        Debug.Log("Court width:" + courtWidth);
        Debug.Log("Court length: " + courtLength);

        referenceToHitScript = GetComponentInChildren<opponentHitAi>();
    }

    
    void Update()
    {
        hitAvailable = referenceToHitScript.hitAvailable;
        
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

        //Debug.Log("Distance of ball to player: " + distance);

        //we want the enemy to be more precise and small in its anticipations for the ball when its close.
        float anticipationTime = getAnticipationTime();
        //Debug.Log("anticipationTime:" + anticipationTime);
        Vector3 anticipatedPosition = ballPosition + (anticipationTime * ballVelocity);

        
        //Vector3 anticipatedPosition = getAccurateBallPrediction();

        //determine if the ball is on the side of the enemy.
        bool ballOnAISide = ((transform.position.z > netPosition && anticipatedPosition.z > netPosition && anticipatedPosition.z < courtLength/2) ||
                            (transform.position.z < netPosition && anticipatedPosition.z < netPosition && anticipatedPosition.z > -courtLength/2)) &&
                            (courtWidth/2 > anticipatedPosition.x && -courtWidth/2 < anticipatedPosition.x);
                           
        //Debug.Log("bool (Ball is on AI side): " + ballOnAISide);
        if (ballOnAISide && hitAvailable)
        {
            targetPosition = new Vector3(anticipatedPosition.x, transform.position.y, anticipatedPosition.z);
        }
        else //ball is on the other side so its best to move to a neutral position.
        {
            float neutralZ = transform.position.z > netPosition ? courtLength * 0.25f : -courtLength * 0.25f;
            //lets add bias due to the location of the character.

            
            //float characterBias = characterReference.transform.position.x;
            targetPosition = new Vector3(0f, transform.position.y, neutralZ);
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
