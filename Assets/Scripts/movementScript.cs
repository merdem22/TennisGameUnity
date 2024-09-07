using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementScript : MonoBehaviour
{

    CharacterController controller;
    public float movementSpeed;

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

        //get input.
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 toMoveVector = new Vector3(x, 0, y);

        controller.Move(toMoveVector * Time.deltaTime * movementSpeed);

    }
}
