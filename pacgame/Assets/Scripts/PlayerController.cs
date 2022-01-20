using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController moveController;
    public string nextDirection;
    // Start is called before the first frame update
    void Start()
    {
        moveController = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            nextDirection = "up";
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            nextDirection = "left";
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            nextDirection = "down";
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            nextDirection = "right";
        }

        moveController.nextDirection = this.nextDirection;
    }
}
