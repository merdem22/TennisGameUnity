using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opponentHitAi : MonoBehaviour
{



    //magnitude related
    [SerializeField] private float minHitForceMultiplier;
    [SerializeField] private float maxHitForceMultiplier;

    

    //direction related
    [SerializeField] private float maxHitAngle;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform opponentTransform;
    [SerializeField] private Transform courtTransform;
    
    [SerializeField] private float biasMagnitude;
    [SerializeField] LayerMask netLayerMask;
    [SerializeField] private float minYOffset;
    [SerializeField] private float maxYOffset;

    [SerializeField] private float minSpeedForceMultiplier;
    [SerializeField] private float maxSpeedForceMultiplier;


    

    private float courtWidth;
    private float courtLength;

    //initially it should be true.
    public bool hitAvailable = true; //enemyMovement script wants to know its value therefore, it is made public.

    //serves the same purpose as the player's triggerMaxWait, locks the trigger so it can only be used once within a period of time.
    private float triggerMaxWait = 1.5f;
    private float curTriggerWait;


    void Awake()
    {
        courtWidth = courtTransform.localScale.x;
        courtLength = courtTransform.localScale.z;
        references.courtWidth = courtWidth;
        references.courtLength = courtLength;


        curTriggerWait = triggerMaxWait;
    }

    void Update()
    {

        //Debug.Log("curTriggerWait: " + curTriggerWait);
        //Debug.Log("hitAvailable: " + hitAvailable);

        if (!hitAvailable)
        {
            curTriggerWait -= Time.deltaTime;
            if (curTriggerWait <= 0) { hitAvailable = true;}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("tennisBall") && gameManager.Instance.lastShotPlayer)
       {
            if (hitAvailable)
            {
                gameManager.Instance.lastShotPlayer = false;
                hitBall(other);
                hitAvailable = false;
                curTriggerWait = triggerMaxWait;
            }
            
       }
    }

    private void hitBall(Collider other)
    {
        Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();

        Vector3 hitDirection = calculateHitDirection();
        hitDirection = ClampHitDirection(hitDirection);

        //this is also on testing.
        ballRigidbody.velocity = new Vector3(ballRigidbody.velocity.x, 0f, ballRigidbody.velocity.z);
        float speedForce = speedHitMagnifier(ballRigidbody.velocity.magnitude);
        


        float hitForce = calculateHitForce(hitDirection);
        //Debug.Log("calculated hit force: " + hitForce);
        hitDirection.y = calculateYOffset(hitDirection);
        //Debug.Log("calculated y offset: " + hitDirection.y);

        

        
        if (ballRigidbody != null)
        {
            ballRigidbody.AddForce(hitForce * hitDirection * speedForce, ForceMode.Impulse);
        }
        
        
    }

    private Vector3 ClampHitDirection(Vector3 direction)
    {
        Vector3 forward = transform.forward;
        float angle = Vector3.Angle(forward, direction);

        if (angle > maxHitAngle)
        {
            direction = Vector3.RotateTowards(forward, direction, Mathf.Deg2Rad * maxHitAngle, 0f);
        }

        return direction.normalized;
    }

    private Vector3 calculateHitDirection()
    {
        /*
        Vector3 opponentToCourtCenter = (courtCenter - opponentTransform.position).normalized;
        Vector3 playerToCourtCenter = (courtCenter - playerTransform.position).normalized;
        */

        float spaceOnLeft = courtWidth / 2 + playerTransform.position.x;
        float spaceOnRight = courtWidth / 2 - playerTransform.position.x;

        float bias = (spaceOnRight - spaceOnLeft) * biasMagnitude;


        Vector3 targetPosition = playerTransform.position + new Vector3(bias, 0, 0);
        
        if (targetPosition.x > courtWidth / 2) { targetPosition.x = courtWidth / 2; }
        if (targetPosition.x < -courtWidth/2) { targetPosition.x = -courtWidth / 2; }

        Vector3 targetDirection = targetPosition - opponentTransform.position;

        targetDirection = ClampHitDirection(targetDirection.normalized);
        return targetDirection.normalized;
    }

    private float calculateHitForce(Vector3 hitDirection)
    {
        RaycastHit hit;
        Vector3 raycastPosition = new Vector3(opponentTransform.position.x, 0f, opponentTransform.position.z); //gotta set y component to zero for the ray to hit the collider.
        Vector3 raycastDirection = new Vector3(hitDirection.x, 0f, hitDirection.z);
        float distanceToNet = 0f;
        if (Physics.Raycast(raycastPosition, raycastDirection, out hit, float.PositiveInfinity, netLayerMask))
        {
            distanceToNet = Vector3.Distance(hit.point, opponentTransform.position);
            //Debug.Log("raycast for opponent hitforce hit");
        }
        float hypothenus = Mathf.Sqrt(references.courtLength / 2 * references.courtLength / 2 + references.courtWidth * references.courtWidth);
        float normalizedDistance = Mathf.InverseLerp(0, hypothenus, distanceToNet);
        return Mathf.Lerp(minHitForceMultiplier, maxHitForceMultiplier, normalizedDistance);
    }

    private float calculateYOffset(Vector3 hitDirection)
    {
        RaycastHit hit;
        Vector3 raycastPosition = new Vector3(opponentTransform.position.x, 0f, opponentTransform.position.z); //gotta set y component to zero for the ray to hit the collider.
        Vector3 raycastDirection = new Vector3(hitDirection.x, 0f, hitDirection.z);
        float distanceToNet = 0f;
        if (Physics.Raycast(raycastPosition, raycastDirection, out hit, float.PositiveInfinity, netLayerMask))
        {
            distanceToNet = Vector3.Distance(hit.point, opponentTransform.position);
            //Debug.Log("raycast for opponent y offset hit");
        }
        float hypothenus = Mathf.Sqrt(references.courtLength / 2 * references.courtLength / 2 + references.courtWidth * references.courtWidth);
        float normalizedDistance = Mathf.InverseLerp(0, hypothenus, distanceToNet);
        return Mathf.Lerp(minYOffset, maxYOffset, normalizedDistance);
    }

    private float speedHitMagnifier(float speed)
    {
        float normalizedSpeed = Mathf.InverseLerp(0, references.maxBallSpeed, speed);
        return Mathf.Lerp(minSpeedForceMultiplier, maxSpeedForceMultiplier, normalizedSpeed);
    }

}
