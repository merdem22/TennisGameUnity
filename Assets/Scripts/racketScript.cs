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
    public float timeAfterRelease;
    public float releaseTimeCap;



    public float inaccuracy;
    public float timingForceWeight;
    public float maxAngleInDegrees;

    private float currentRotation = 0f;
    private Vector3 mouseLocation;
    


    public float minForce;
    public float maxForce;
    public float maxHoldDuration;
    private float holdDuration;

    public float maxHitAngle;

    [SerializeField] private LayerMask netLayerMask;
    [SerializeField] private float lowShotYOffset;
    [SerializeField] private float highShotYOffset;
    [SerializeField] private float lowShotForceMultiplier;
    [SerializeField] private float highShotForceMultiplier;


    private bool isLowShot = true; //indicates that the shot is going to be a lowshot (fire1), if it is false then it is going to indicate a high shot (fire2) 

    
    private bool triggerAvailable = true;
    private float triggerMaxWait = 0.5f; //this should be set from the script, 
    private float triggerWait;

    private void Awake()
    {
        triggerWait = triggerMaxWait;
    }

    void Update()
    {
        if (!triggerAvailable)
        {
            triggerWait -= Time.deltaTime;
            if (triggerWait <= 0) {triggerAvailable = true; triggerWait = triggerMaxWait;}
        }


        //Debug.Log("holdDuration: "  + holdDuration);
        mouseLocation = references.GetCursorLocationOnGround();

        if (Input.GetButton("Fire1") || Input.GetButton("Fire2"))
        {
            if (Input.GetButton("Fire1")) { isLowShot = true; }
            else { isLowShot = false;}

            timeAfterRelease = 0;

            if (holdDuration < maxHoldDuration) { holdDuration += Time.deltaTime;}
            else { holdDuration = maxHoldDuration;}

            currentRotation = Mathf.MoveTowards(currentRotation, maxRotation, rotationSpeed * Time.deltaTime);
            GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        else
        {
            timeAfterRelease += Time.deltaTime;
            currentRotation = Mathf.MoveTowards(currentRotation, 0f, releaseSpeed * Time.deltaTime);
            GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            holdDuration -= Time.deltaTime;
            if (holdDuration <= 0)
            {
                holdDuration = 0;
            }
        }
        //apply rotation
        transform.RotateAround(rotationPivot.transform.position, Vector3.up, currentRotation - transform.localRotation.eulerAngles.y);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("tennisBall"))
        {
            if (triggerAvailable)
            {
                HitBall(other);
                triggerAvailable = false;
            }
            
        }
    }


    //helper methods.

    private void HitBall(Collider collider)
    {
        if (RacketCanHit())
        {
            collider.gameObject.GetComponent<Rigidbody>().AddForce(getForceVector(), ForceMode.Impulse);
            enemyAI.ballObject = collider.gameObject;
        }
    }

    private bool RacketCanHit() //returns a bool that is true if the timing to hit the ball is correct.
    {
        return (timeAfterRelease != 0 && timeAfterRelease < releaseTimeCap);
    }

    private float getTimingMetric() //returns a float value (0, 1) based on the timing of the hit. (0 is the best possible value, 1 is the worst)
    {
        return timeAfterRelease / releaseTimeCap;
    }

    //returns the direction to the cursor from the location of the racket.
    private Vector3 getVectorToCursorDirection()
    {
        Vector3 racketToCursor = (references.GetCursorLocationOnGround() - transform.position);
        racketToCursor.y = 0;
        return racketToCursor.normalized;
    }

    private float getHoldDurationForce()
    {
        float range = maxForce - minForce;
        float prevDuration = holdDuration;
        return (minForce + (range / maxHoldDuration) * prevDuration);
        
    }

    private Vector3 getForceDirection()
    {
        //we will have to clamp this eventually (we don't want to be shooting behind us :D)
        float timingWeight = getTimingMetric();
        Vector3 inAccuracyVector = inaccuracy * new Vector3(Random.Range(-timingWeight, timingWeight), Random.Range(-timingWeight, timingWeight)/2, Random.Range(-timingWeight, timingWeight));
        Vector3 direction = (getVectorToCursorDirection() + inAccuracyVector).normalized;

        RaycastHit hit;

        //determining y component (we want it to be atleast as high as the net on good shots.) 
        if (Physics.Raycast(new Vector3(transform.position.x, 0.1f, transform.position.z), direction, out hit, Mathf.Infinity, netLayerMask))
        {
            Debug.Log("Adjusting y.");
            Vector3 hitPoint = hit.point;
            if (isLowShot)
            {
                hitPoint.y = hit.collider.bounds.max.y + lowShotYOffset;
                Debug.Log("Is a lowshot!");
            }
            else
            {
                hitPoint.y = hit.collider.bounds.max.y + highShotYOffset;
                Debug.Log("Is a highshot!");
            }
            direction = (hitPoint - transform.position).normalized;
        }
        
        //clamping
        Vector3 forward = Vector3.forward;
        float angle = Vector3.Angle(new Vector3 (forward.x, direction.y, forward.z), direction);

        if (angle > maxHitAngle)
        {
            Debug.Log("Clamping done.");
            Vector3 clampedDirection = Vector3.RotateTowards(forward, direction, Mathf.Deg2Rad * maxHitAngle, 0f);
            direction = clampedDirection.normalized;
        }
        return direction;
    }

    private float getForceMagnitude()
    {
        if (isLowShot)
        {
            return getHoldDurationForce() *  lowShotForceMultiplier;
        }
        else
        {
            return getHoldDurationForce() * highShotForceMultiplier;
        }
    }

    private Vector3 getForceVector()
    {
        return getForceDirection() * getForceMagnitude();
    }

    
}
