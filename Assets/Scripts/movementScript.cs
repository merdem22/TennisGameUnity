using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementScript : MonoBehaviour
{

    CharacterController controller;
    public float movementSpeed;
    [SerializeField] Animator animator;

    private void Awake()
    {
        //gameManager.Instance.player = this.gameObject;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    
    void Update()
    {

        /*if (Input.GetButtonDown("Fire1")) 
        {
            //animator.SetTrigger("Hit");
        }*/

        //get input.
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        //set the animator params.
        animator.SetFloat("moveX", x);
        animator.SetFloat("moveY", y);

        Vector3 toMoveVector = new Vector3(x, 0, y);
        controller.Move(toMoveVector * Time.deltaTime * movementSpeed);


        //adjusting the animation speed with respesct to velocity.
        float speed = controller.velocity.magnitude;
        float speedRatio = speed / movementSpeed;
        float animatorModifier = Mathf.Lerp(1, 2, speedRatio); //animation ranges (1-2)x
        animator.speed = animatorModifier;
    }
}
