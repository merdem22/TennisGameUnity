using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance { get; private set; }

    public int playerScore = 0;
    public int opponentScore = 0;

    public GameObject ball; 
    public Vector3 ballDropPosition;
    public GameObject player;
    public GameObject opponent;


    //fading related.
    [SerializeField] private CanvasGroup fadeCanvasGroup; //the canvas group attached to the Ui for fading after scores.
    [SerializeField] private float fadeDuration;
    [SerializeField] private float waitInFadeDuration;
    private bool isFading = false;

    public bool lastShotPlayer = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ScorePoint(bool playerScored)
    {
        if (!isFading)
        {
            if (playerScored)
            {
                playerScore++;
                Debug.Log("Player Scored, current Player Score: " + playerScore);
            }
            else
            {
                opponentScore++;
                Debug.Log("Opponent Scored, current Opponent Score: " + opponentScore);
            }

            //start the fade and reset coroutine;
            StartCoroutine(FadeOutAndReset(playerScored)); //if playerScored than player shall serve.
        }
    }

    public void ResetState(bool playerSide)
    {
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();

        //initial state of lastShot is false, if player starts
        if (playerSide) { lastShotPlayer = false; }
        else { lastShotPlayer = true; }

        //reset the player positions.
        player.transform.position = new Vector3(0, 0, -references.courtLength / 4);
        opponent.transform.position = new Vector3(0, 0, references.courtLength / 4);

        Debug.Log("Resetting state...");
        if (playerSide)
        {
            ball.transform.position = ballDropPosition;
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }
        else
        {
            ball.transform.position = new Vector3(ballDropPosition.x, ballDropPosition.y, -ballDropPosition.z);
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }  
    }

    private IEnumerator FadeOutAndReset(bool playerSide)
    {
        isFading = true;

        yield return StartCoroutine(Fade(1f));

        //wait for a moment before resetting
        yield return new WaitForSeconds(waitInFadeDuration);

        //reset the game state
        ResetState(playerSide);

        //fade back to normal
        yield return StartCoroutine(Fade(0f));

        isFading = false;
    }


    //coroutine for fading in and out.
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }




}
