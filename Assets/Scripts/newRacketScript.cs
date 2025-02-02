using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newRacketScript : MonoBehaviour
{
    // Start is called before the first frame update

    
    public float timeAfterRelease;
    public float releaseTimeCap;


    public float inaccuracy;
    public float timingForceWeight;
    private Vector3 mouseLocation;



    public float minForce;
    public float maxForce;
    public float maxHoldDuration;
    private float holdDuration;

    public float maxHitAngle;

    [SerializeField] Animator animator;
    [SerializeField] private LayerMask netLayerMask;
    [SerializeField] private float lowShotYOffset;
    [SerializeField] private float highShotYOffset;
    [SerializeField] private float lowShotForceMultiplier;
    [SerializeField] private float highShotForceMultiplier;

    //weights of automatically hiting harder when the ball comes fast.
    [SerializeField] private float minSpeedForceMultiplier;
    [SerializeField] private float maxSpeedForceMultiplier;

    //weights of automatically hiting more upright when the the distance from the net is far.
    [SerializeField] private float minYOffsetMagnifier;
    [SerializeField] private float maxYOffsetMagnifier;

    [SerializeField] private Slider holdPowerSlider;
    [SerializeField] private RectTransform sliderRectTransform;
    [SerializeField] private Transform sliderPositionTransform;
    [SerializeField] private float hitThreshold;
    [SerializeField] private float animMaxTriggerWait;

    private bool isLowShot = true; //indicates that the shot is going to be a lowshot (fire1), if it is false then it is going to indicate a high shot (fire2) 



    //animation related.
    private bool triggerAvailable = true;
    private float triggerMaxWait = 0.5f; //this should be set from the script, 
    private float triggerWait;
    private float animTriggerWait;
    private float hitReleaseAnimSpeed;




    private void Awake()
    {
        triggerWait = triggerMaxWait;
        animTriggerWait = animMaxTriggerWait;
    }

    void Update()
    {
        if (!triggerAvailable)
        {
            triggerWait -= Time.deltaTime;
            if (triggerWait <= 0) { triggerAvailable = true; triggerWait = triggerMaxWait; }
        }


        //Debug.Log("holdDuration: "  + holdDuration);
        mouseLocation = references.GetCursorLocationOnGround();

        if (Input.GetButton("Fire1") || Input.GetButton("Fire2"))
        {
            if (Input.GetButton("Fire1")) { isLowShot = true; }
            else { isLowShot = false; }

            timeAfterRelease = 0;

            if (holdDuration < maxHoldDuration) { holdDuration += Time.deltaTime; }
            else { holdDuration = maxHoldDuration; }

            // Update the slider value to reflect hold duration
            holdPowerSlider.value = holdDuration / maxHoldDuration;

        }
        else
        {
            timeAfterRelease += Time.deltaTime;
            GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            holdDuration -= 5 * Time.deltaTime;

            if (holdDuration / maxHoldDuration > hitThreshold && animTriggerWait <= 0.1f)
            {
                animator.SetTrigger("Hit");
                animTriggerWait = animMaxTriggerWait;
            }
            if (holdDuration <= 0)
            {
                holdDuration = 0;
            }

            // Update the slider value to reflect hold duration
            holdPowerSlider.value = holdDuration / maxHoldDuration;
        }
        animTriggerWait -= Time.deltaTime;
        if (animTriggerWait <= 0) { animTriggerWait = 0;}
        animator.SetFloat("hold", holdDuration / maxHoldDuration);
        hitReleaseAnimSpeed = holdDuration / maxHoldDuration + 1;
        animator.SetFloat("hitSpeed", hitReleaseAnimSpeed); // You can use this to drive a parameter in a blend tree, if needed
        Vector3 screenPos = Camera.main.WorldToScreenPoint(sliderPositionTransform.position);
        sliderRectTransform.position = screenPos;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("tennisBall") && !gameManager.Instance.lastShotPlayer)
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
            gameManager.Instance.lastShotPlayer = true;
            Rigidbody ballRb = collider.gameObject.GetComponent<Rigidbody>();
            ballRb.velocity = new Vector3(0f, 0f, ballRb.velocity.z); //this is on testing.
            float speedMagnifier = speedHitMagnifier(collider.GetComponent<Rigidbody>().velocity.magnitude);
            ballRb.AddForce(getForceVector() * speedMagnifier, ForceMode.Impulse);
            enemyAI.ballObject = collider.gameObject;

            //camera shake
            float joltIntensity = holdDuration / maxHoldDuration;
            references.mainCamera.GetComponent<cameraScript>().Jolt(joltIntensity);
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
        Vector3 inAccuracyVector = inaccuracy * new Vector3(Random.Range(-timingWeight, timingWeight), Random.Range(-timingWeight, timingWeight) / 2, Random.Range(-timingWeight, timingWeight));
        Vector3 direction = (getVectorToCursorDirection() + inAccuracyVector).normalized;

        RaycastHit hit;

        //determining y component (we want it to be atleast as high as the net on good shots.) 
        if (Physics.Raycast(new Vector3(transform.position.x, 0.1f, transform.position.z), direction, out hit, Mathf.Infinity, netLayerMask))
        {
            Debug.Log("Adjusting y.");
            Vector3 hitPoint = hit.point;
            float yMagniifer = getYOffsetMagnifier(direction);
            Debug.Log("y magnifier: " + yMagniifer);
            if (isLowShot)
            {
                hitPoint.y = hit.collider.bounds.max.y + (lowShotYOffset * yMagniifer);
                Debug.Log("Is a lowshot!");
            }
            else
            {
                hitPoint.y = hit.collider.bounds.max.y + (highShotYOffset * yMagniifer);
                Debug.Log("Is a highshot!");
            }
            direction = (hitPoint - transform.position).normalized;
        }

        //clamping
        Vector3 forward = Vector3.forward;
        float angle = Vector3.Angle(new Vector3(forward.x, direction.y, forward.z), direction);

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
            return getHoldDurationForce() * lowShotForceMultiplier;
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


    private float getYOffsetMagnifier(Vector3 hitDirection)
    {
        RaycastHit hit;
        Vector3 raycastPosition = new Vector3(transform.position.x, 0f, transform.position.z); //gotta set y component to zero for the ray to hit the collider.
        Vector3 raycastDirection = new Vector3(hitDirection.x, 0f, hitDirection.z);
        float distanceToNet = 0f;
        if (Physics.Raycast(raycastPosition, raycastDirection, out hit, float.PositiveInfinity, netLayerMask))
        {
            distanceToNet = Vector3.Distance(hit.point, transform.position);
            Debug.Log("raycast for opponent y offset hit");
        }

        float hypothenus = Mathf.Sqrt(references.courtLength / 2 * references.courtLength / 2 + references.courtWidth * references.courtWidth);
        float normalizedDistance = Mathf.InverseLerp(0, hypothenus, distanceToNet); //longest distance possible is hyptohenus.
        return Mathf.Lerp(minYOffsetMagnifier, maxYOffsetMagnifier, normalizedDistance);
    }

    private float speedHitMagnifier(float speed)
    {
        float normalizedSpeed = Mathf.InverseLerp(0, references.maxBallSpeed, speed);
        return Mathf.Lerp(minSpeedForceMultiplier, maxSpeedForceMultiplier, normalizedSpeed);
    }

}
