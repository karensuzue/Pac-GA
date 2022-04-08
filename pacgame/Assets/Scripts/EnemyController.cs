using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * EnemyController class used for "Game" Scene.
 * Controls for enemy/ghost movement patterns. 
*/
public class EnemyController : MonoBehaviour
{
    // Group of positions for ghost starting nodes
    public enum GhostNodePositions
    {
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        moveToPath
    }
    public GhostNodePositions ghostNodePositions;

    // Grouping of ghost colors
    public enum GhostColor
    {
        red,
        blue,
        pink,
        orange
    }
    public GhostColor ghostColor;

    // Ghost Starting Nodes, obtained through Hierarchy 
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public GameObject startingNode; // The ghost's chosen starting node, set in Awake
    public bool leaveHome = false; // Bool set to true only when ghost is leaving starting nodes/spawn area

    public GameObject player; // Reference to player GameObject instance in Hierarchy
    private MovementController movementController; // MovementController class of ghost GameObject

    public bool playerCaught; // True if player is caught by ghost
    public GameObject gameManagerObj; // GameManager GameObject object
    public GameManager gameManager; // GameManager class


    /** 
     * Awake is the first method called when a Scene is loaded.
     * This method runs once per scene, and lasts until a Scene is unloaded. 
     * For initializing variables or states before the game starts.
    */
    void Awake() 
    {
        // Obtain GameManager class from its respective GameObject
        gameManager = gameManagerObj.GetComponent<GameManager>(); 
        // Find the player's GameObject instance
        player = GameObject.FindGameObjectWithTag("Player"); 
        // MovementController class attached to ghost GameObject
        movementController = gameObject.GetComponent<MovementController>(); 
        movementController.isGhost = true;

        // Red ghost starts at start node of the spawning area
        if (ghostColor == GhostColor.red) 
        {
            // set spawn position to startNode
            ghostNodePositions = GhostNodePositions.startNode;
            startingNode = ghostNodeStart; 
        }

        // Blue ghost starts at left node of the spawning area
        else if (ghostColor == GhostColor.blue) 
        {
            // set spawn position to leftNode
            ghostNodePositions = GhostNodePositions.leftNode; 
            startingNode = ghostNodeLeft;
        }

        // Pink ghost starts at center node of the spawning area
        else if (ghostColor == GhostColor.pink) 
        {
            // set spawn position to centerNode
            ghostNodePositions = GhostNodePositions.centerNode;
            startingNode = ghostNodeCenter;
        }

        // Orange ghost starts at right node of the spawning area
        else if (ghostColor == GhostColor.orange) 
        {
            // set spawn position to rightNode
            ghostNodePositions = GhostNodePositions.rightNode;
            startingNode = ghostNodeRight;
        }

    }

    /**
     * Start is called before the first call to Update. 
     * This method runs once per scene.
     * Occurs at the very start of the game, the first frame.  
    */
    void Start()
    {
        // Set nextNode of MovementController class to one of the spawn locations above
        movementController.nextNode = startingNode;
        leaveHome = true;
        playerCaught = false;
    }

    /** 
     * Update is called once per frame. Continually checks and updates the ghost's next direction.
    */
    void Update()
    {
        // If ghost has exit starting/spawn area.
        if (ghostNodePositions == GhostNodePositions.moveToPath)
        {
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

                // Find red ghost GameObject instance
                GameObject redGhost = GameObject.FindGameObjectWithTag("RedGhost"); 
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

        // If ghosts are in respawning mode
        else if (ghostNodePositions == GhostNodePositions.respawning)
        {
            Debug.Log("RESPAWN");
        }

        // If ghosts are still in starting/spawn area
        else {
            // If ghost can leave spawn area
            if (leaveHome) 
            {
                // If ghost is currently at the left side of the spawn area, move to center
                if (ghostNodePositions ==  GhostNodePositions.leftNode)
                {
                    ghostNodePositions = GhostNodePositions.centerNode;
                    movementController.nextNode = ghostNodeCenter;
                }

                // If ghost is currently at the right side of the spawn area, move to center
                if (ghostNodePositions == GhostNodePositions.rightNode)
                {
                    ghostNodePositions = GhostNodePositions.centerNode;
                    movementController.nextNode = ghostNodeCenter;
                }

                // If ghost is currently at the center node, move to 'start' node located outside of spawn area
                if (ghostNodePositions == GhostNodePositions.centerNode)
                {
                    ghostNodePositions = GhostNodePositions.startNode;
                    movementController.nextNode = ghostNodeStart;
                }

                // If ghost is currently at the start node, move to maze path. 
                if (ghostNodePositions == GhostNodePositions.startNode)
                {
                    ghostNodePositions = GhostNodePositions.moveToPath;
                }
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
        if (other.tag == "Player")
        {
            // Player is caught
            playerCaught = true; 
            // Game state is game over
            gameManager.state = GameManager.GameStates.gameOver;
        }
    }
};