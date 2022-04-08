using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * PlayerController class contains information and controls for player character.
*/
public class PlayerController : MonoBehaviour
{
    private MovementController moveController; // MovementController class attached
    public string nextDirection; // next immediate direction

    /**
     * Start is called before the first call to Update. 
     * This method runs once per scene.
     * Occurs at the very start of the game, the first frame.  
    */
    void Start()
    {
        // Obtain reference to attached MovementController class
        moveController = GetComponent<MovementController>(); 
    }

    /** 
     * Update is called once per frame. 
     * Continually checks for player input and updates the player's next direction.
    */
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

        // Player movement is simulated through MovementController class
        moveController.SetDirection(this.nextDirection);
    }

    
    /**
     * Retrieves player's next node. 
     * @return The node player will move to 
    */
    public GameObject GetPlayerNextNode()
    {
        return moveController.nextNode;
    }
}
