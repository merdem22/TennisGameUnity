using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballThrowerScript : MonoBehaviour
{
    public GameObject tennisBallPreset;
    public GameObject barrelEnd;
    public float throwBallInterval;
    public float throwSpeed;
    private float secondsToThrowBall;
    void Start()
    {
        secondsToThrowBall = throwBallInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (secondsToThrowBall <= 0f)
        {
            GameObject tennisBallInstance = Instantiate(tennisBallPreset, barrelEnd.transform.position, barrelEnd.transform.rotation);
            tennisBallInstance.GetComponent<Rigidbody>().velocity = Vector3.forward * throwSpeed;
            secondsToThrowBall = throwBallInterval;
        }
        secondsToThrowBall -= Time.deltaTime;
    }
}
