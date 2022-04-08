using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * EnemyController2 class used for "Game GA" Scene.
 * Does not implement ghost starting nodes. 
 * Controls for enemy/ghost movement patterns. 
*/ 
public class EnemyController2 : MonoBehaviour
{
    // Grouping of ghost colors. 
    public enum GhostColor
    {
        red,
        blue,
        pink,
        orange
    }
    public GhostColor ghostColor;

    // The ghost's chosen starting node
    public GameObject startingNode; // Set in inspector 
    // Reference to player GameObject in Hierarchy. 
    public GameObject player;
    // MovementController class attached to ghost GameObject
    private MovementController movementController;
    public bool playerCaught; // True if player is caught by ghost. 
    public GameObject gameManagerObj; // GameManager GameObject object.
    public GameManager gameManager; // GameManager class
   
    /** 
     * Awake is the first method called when a Scene is loaded.
     * This method runs once per scene, and lasts until a Scene is unloaded. 
     * For initializing variables or states before the game starts.
    */
    void Awake() 
    {
        // Obtain GameManager class from its respective GameObject
        gameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gameManagerObj.GetComponent<GameManager>();

        // Find the player's GameObject in the hierarchy
        player = GameObject.FindGameObjectWithTag("Player");
         // MovementController class attached to ghost GameObject
        movementController = gameObject.GetComponent<MovementController>();
    }

    /**
     * Start is called before the first call to Update. 
     * This method runs once per scene.
     * Occurs at the very start of the game, the first frame.  
    */
    void Start()
    {
        // Find initial starting location on map
        startingNode = GameObject.FindGameObjectWithTag("StartNode");
        // Set nextNode of MovementController class to 
        movementController.nextNode = startingNode;
        playerCaught = false;
    }

    /** 
     * Update is called once per frame. Continually checks and updates the ghost's next direction.
    */
    void Update()
    {
        // Ensures that ghosts will stop moving when playthrough ends
        if (gameManager.state == GameManager.GameStates.win || gameManager.state == GameManager.GameStates.gameOver) {
           
            movementController.nextNode = null;
        }

        // RED GHOST MOVEMENT
        if (ghostColor == GhostColor.red) 
        {
            // Player position is target position
            Vector2 target = player.transform.position;
            // The best direction to reach target thus far
            string nextDirection = GetClosestDirection(target);
            // Set direction
            movementController.SetDirection(nextDirection);
        }

        // BLUE GHOST MOVEMENT
        else if (ghostColor == GhostColor.blue) 
        {
            // Player's current direction
            string targetDirection = player.GetComponent<MovementController>().currDirection;

            // Player's current position, used to calculate target position
            Vector2 targetPosition = player.transform.position;   
            if (targetDirection == "left")
            {
                targetPosition.x -= 2;
            }

            if (targetDirection == "right")
            {
                targetPosition.x += 2;
            }

            if (targetDirection == "up")
            {
                targetPosition.y += 2;
            }

            if (targetDirection == "down")
            {
                targetPosition.y -= 2;
            }

            // Find red ghost GameObject in Hierarchy
            GameObject redGhost = GameObject.FindGameObjectWithTag("InvisibleRedGhost");
            float xDistance = targetPosition.x - redGhost.transform.position.x;
            float yDistance = targetPosition.y - redGhost.transform.position.y;

            // Finalized target position for blue ghost
            Vector2 blueTarget = new Vector2(targetPosition.x + xDistance, targetPosition.y + yDistance);
            string nextDirection = GetClosestDirection(blueTarget);
            movementController.SetDirection(nextDirection);
        }

        // PINK GHOST MOVEMENT
        else if (ghostColor == GhostColor.pink) 
        {
            string targetDirection = player.GetComponent<MovementController>().currDirection;
            Vector2 targetPosition = player.transform.position;

            if (targetDirection == "left") 
            {
                targetPosition.x -= 2;
            }

            else if (targetDirection == "right")
            {
                targetPosition.x += 2;
            }

            else if (targetDirection == "up")
            {
                targetPosition.y += 2;
            }

            else if (targetDirection == "down")
            {
                targetPosition.y -= 2;
            }

            string nextDirection = GetClosestDirection(targetPosition);
            movementController.SetDirection(nextDirection);
        }

        // ORANGE GHOST MOVEMENT
        else if (ghostColor == GhostColor.orange)
        {
            // Find distance between player and self (ghost) in the current frame 
            float distance = Math.Abs(Vector2.Distance(player.transform.position, transform.position));
            
            // If distance is smaller than 8 units
            if (distance < 8)
            {
                // Orange ghost behaves like red ghost
                Vector2 target = player.transform.position;
                string nextDirection = GetClosestDirection(target);
                movementController.SetDirection(nextDirection);
            }

            else
            {
                // Target position is set to that of a node outside of the map
                GameObject target = GameObject.FindGameObjectWithTag("OrangeScatter");
                string nextDirection = GetClosestDirection(target.transform.position);
                movementController.SetDirection(nextDirection);
            }
        }
    }

    /**
     * Retrieves direction that is closest to a given target.
     * @param targetPosition 2D Vector holding target position 
     * @return A string detailing the next direction to move to 
    */
    public string GetClosestDirection(Vector2 targetPosition) 
    {
        // Variables to keep track of shortest distance and direction
        float shortestDistance = 0;
        string nextDirection = "";
        
        // Get ghost's current direction, prevent ghost from reversing
        string prevDirection = movementController.currDirection;

        // NodeController class obtained from the ghost's nextNode 
        NodeController nodeController = movementController.nextNode.GetComponent<NodeController>();

        // If there exists a node above nextNode and the previous direction is not 'down'
        if (nodeController.up && prevDirection != "down") 
        {
            // Obtain node above nextNode
            GameObject nodeUp = nodeController.nodeUp;
            // Calculate distance between nodeUp and target
            float distance = Vector2.Distance(nodeUp.transform.position, targetPosition);

            // Update shortest distance and next direction if distance is the smallest thus far
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                nextDirection = "up";
            }
        }

        // If there exists a node to the left of nextNode and the previous direction is not 'right'
        if (nodeController.left && prevDirection != "right") 
        {
            // Obtain node to the left
            GameObject nodeLeft = nodeController.nodeLeft;
            // Calculate distance between target and nodeLeft
            float distance = Vector2.Distance(nodeLeft.transform.position, targetPosition);

            // Check and update shortest distance
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                nextDirection = "left";
            }
        }

        // If there exists a node below nextNode and the previous direction is not 'up'
        if (nodeController.down && prevDirection != "up") 
        {
            GameObject nodeDown = nodeController.nodeDown;
            float distance = Vector2.Distance(nodeDown.transform.position, targetPosition);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                nextDirection = "down";
            }
        }

        // If there exists a node to the right of nextNode and the previous direction is not 'left'
        if (nodeController.right && prevDirection != "left") 
        {
            GameObject nodeRight = nodeController.nodeRight;
            float distance = Vector2.Distance(nodeRight.transform.position, targetPosition);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                nextDirection = "right";
            }
        }

        // Return shortest direction in string format
        return nextDirection;
    }

    /**
     * Called when an object collides with ghost
     * @param other Collider object attached to GameObject that collided with ghost
    */
    void OnTriggerEnter2D(Collider2D other)
    {
        // If other GameObject is player
        if (other.tag == "Player" && movementController.isGhost == true)
        {
            // Player is caught
            playerCaught = true; 
            // Game state is game over
            gameManager.state = GameManager.GameStates.gameOver;
        }
    }
};