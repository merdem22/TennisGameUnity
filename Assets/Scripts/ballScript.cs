using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballScript : MonoBehaviour //checks for ball bounces, as well.
{
    public float maxHitForce;
    [SerializeField] private float maxBallSpeed;
    private Rigidbody rb;


    [SerializeField] private GameObject court;
    [SerializeField] private Transform net;
  

    private int playerSideBounces = 0;
    private int opponentSideBounces = 0;


    private float courtWidth;
    private float courtLength;
    private float netPosition;



    void Awake()
    {
        //gameManager.Instance.ball = this.gameObject;
        //gameManager.Instance.ballDropPosition = transform.position;
        references.maxBallSpeed = maxBallSpeed;
        rb = GetComponent<Rigidbody>();
    }


    private void Start()
    {
        courtWidth = references.courtWidth;
        courtLength = references.courtLength;
        netPosition = net.position.z;
    }

    private void Update()
    {
        Vector3 ballPosition = transform.position;
        bool outOfBounds = (ballPosition.z > courtLength / 2 || ballPosition.z < -courtLength / 2 || ballPosition.x > courtWidth/2 || ballPosition.x < -courtWidth/2);

        if (outOfBounds) //handle out of bounds, scoring.
        {
            if (gameManager.Instance.lastShotPlayer)
            {
                if (opponentSideBounces > 0) { gameManager.Instance.ScorePoint(true); }
                else { gameManager.Instance.ScorePoint(false); }
            }
            else
            {
                if (playerSideBounces > 0) { gameManager.Instance.ScorePoint(false); }
                else { gameManager.Instance.ScorePoint(true); }
            }
            
        }


        //limit the ball's max speed.
        if (rb.velocity.magnitude > maxBallSpeed) { rb.velocity = rb.velocity.normalized * maxBallSpeed; }


        //rset bounces on every net crossing.
        if (transform.position.z < netPosition && opponentSideBounces > 0)
        {
            resetBounces();
        }
        else if (transform.position.z > netPosition && playerSideBounces > 0)
        {
            resetBounces();
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "ground")
        {
            Vector3 ballPosition = transform.position;

            if (ballPosition.z < netPosition)
            {
                playerSideBounces++;
                Debug.Log("bounce on playerSide: " + playerSideBounces);
                opponentSideBounces = 0;
                if (playerSideBounces >= 2)
                {
                    resetBounces();
                    gameManager.Instance.ScorePoint(false);
                }
            }
            else
            {
                opponentSideBounces++;
                Debug.Log("bounce on opponentSide: " + opponentSideBounces);
                playerSideBounces = 0;
                if (opponentSideBounces >= 2)
                {
                    resetBounces();
                    gameManager.Instance.ScorePoint(true);
                }
            }

        }
    }


    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("tennisRacket") || other.CompareTag("enemyBody"))
        {
            resetBounces();
        }
    }
    


    private void resetBounces()
    {
        playerSideBounces = 0;
        opponentSideBounces = 0;
    }

}
