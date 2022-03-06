using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This version of enemy controller sets all enemies to be located in one of the normal nodes on the map. Tested in Scene Game 3. 

// if changing enemy types, probably cna instantiate them as prefabs. 
// if changing speed, can do 8r + 2b + 3p + 5o or something idk  
public class EnemyController2 : MonoBehaviour
{
    // red = -1.6, -4
    // blue = 1.69, -4
    // orange = -1.6, -2
    // pink = 1.69, -2

    /*string[] modes = {"chase", "scatter", "scared", "win"}; // win state = ghost on same tile as pacman, level controller steps in
    int current_mode = 1;

    public string nextDirection;
    public string destination;*/

   /* public enum GhostNodePositions
    {
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        moveToPath
    }

    public GhostNodePositions ghostNodePositions;
    */

    public enum GhostColor
    {
        red,
        blue,
        pink,
        orange
    }

    public GhostColor ghostColor;

    // has to be public 
    // public GameObject ghostNodeLeft;
    // public GameObject ghostNodeRight;
    // public GameObject ghostNodeCenter;
    // public GameObject ghostNodeStart;
    public GameObject startingNode; // Set in inspector 
    // public bool leaveHome = false;

    public GameObject player;
    private MovementController movementController;

    public bool playerCaught;
    public GameObject gameManagerObj;
    public GameManager gameManager;

    // public float distanceFromPlayer;

    void Awake() 
    {
        gameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gameManagerObj.GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player"); // change this to the one in level controller
        movementController = gameObject.GetComponent<MovementController>();
        // movementController.isGhost = true;
        

        /*
        if (ghostColor == GhostColor.red) 
        {
            ghostNodePositions = GhostNodePositions.startNode;
            startingNode = ghostNodeStart;
        }

        else if (ghostColor == GhostColor.blue) 
        {
            ghostNodePositions = GhostNodePositions.leftNode;
            startingNode = ghostNodeLeft;
        }

        else if (ghostColor == GhostColor.pink) 
        {
            ghostNodePositions = GhostNodePositions.centerNode;
            startingNode = ghostNodeCenter;
        }

        else if (ghostColor == GhostColor.orange) 
        {
            ghostNodePositions = GhostNodePositions.rightNode;
            startingNode = ghostNodeRight;
        }*/

    }

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Enemy Controller Running");
        startingNode = GameObject.FindGameObjectWithTag("StartNode");
        movementController.nextNode = startingNode;
        // leaveHome = true; // maybe control this from level controller later, or ask if ui is at respwan state
        // movementController.nextDirection = "right";
        playerCaught = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.state == GameManager.GameStates.win || gameManager.state == GameManager.GameStates.gameOver) {
            // loop ensures that ghosts will stop moving when playthrough ends
            movementController.nextNode = null; // will give you an error
        }
        /*if (transform.position == player.transform.position)
        {
            playerCaught = true; 
            gameManager.state = GameManager.GameStates.gameOver;
        }*/

        /*if (movementController.nextNode == player.GetComponent<PlayerController>().GetPlayerNextNode())
        {
            current_mode = 3; // win, trigger UI
        }*/
        // if (ghostNodePositions == GhostNodePositions.moveToPath)
        // {
            if (ghostColor == GhostColor.red) 
            {
                Vector2 target = player.transform.position;
                string nextDirection = GetClosestDirection(target);
                movementController.SetDirection(nextDirection);
            }
            
            else if (ghostColor == GhostColor.blue) 
            {
                string targetDirection = player.GetComponent<MovementController>().currDirection;
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

                GameObject redGhost = GameObject.FindGameObjectWithTag("InvisibleRedGhost");
                float xDistance = targetPosition.x - redGhost.transform.position.x;
                float yDistance = targetPosition.y - redGhost.transform.position.y;

                Vector2 blueTarget = new Vector2(targetPosition.x + xDistance, targetPosition.y + yDistance);
                string nextDirection = GetClosestDirection(blueTarget);
                movementController.SetDirection(nextDirection);
            }

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

            else if (ghostColor == GhostColor.orange)
            {
                float distance = Math.Abs(Vector2.Distance(player.transform.position, transform.position));
                if (distance < 8)
                {
                    Vector2 target = player.transform.position;
                    string nextDirection = GetClosestDirection(target);
                    movementController.SetDirection(nextDirection);
                }

                else
                {
                    GameObject target = GameObject.FindGameObjectWithTag("OrangeScatter");
                    string nextDirection = GetClosestDirection(target.transform.position);
                    movementController.SetDirection(nextDirection);
                }
            }
        // }   

        /* else if (ghostNodePositions == GhostNodePositions.respawning)
        {
            Debug.Log("RESPWAN");
        }

        else {
            if (leaveHome) 
            {
                if (ghostNodePositions ==  GhostNodePositions.leftNode)
                {
                    ghostNodePositions = GhostNodePositions.centerNode;
                    movementController.nextNode = ghostNodeCenter;
                }

                if (ghostNodePositions == GhostNodePositions.rightNode)
                {
                    ghostNodePositions = GhostNodePositions.centerNode;
                    movementController.nextNode = ghostNodeCenter;
                }

                if (ghostNodePositions == GhostNodePositions.centerNode)
                {
                    ghostNodePositions = GhostNodePositions.startNode;
                    movementController.nextNode = ghostNodeStart;
                }

                if (ghostNodePositions == GhostNodePositions.startNode)
                {
                    ghostNodePositions = GhostNodePositions.moveToPath;
                    // movementController.nextDirection = "right"; // temp, change to GetPlayerDirection? 
                }
            }
        }*/
    }

    public string GetClosestDirection(Vector2 targetPosition) 
    {
        float shortestDistance = 0;
        string nextDirection = "";
        
        // get player position
        // Vector2 targetPosition = player.transform.position;
        string prevDirection = movementController.currDirection; // this is so that ghosts can't reverse

        NodeController nodeController = movementController.nextNode.GetComponent<NodeController>();

        if (nodeController.up && prevDirection != "down") 
        {
            GameObject nodeUp = nodeController.nodeUp;
            float distance = Vector2.Distance(nodeUp.transform.position, targetPosition);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                nextDirection = "up";
            }
        }

        if (nodeController.left && prevDirection != "right") 
        {
            GameObject nodeLeft = nodeController.nodeLeft;
            float distance = Vector2.Distance(nodeLeft.transform.position, targetPosition);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                nextDirection = "left";
            }
        }

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
        // Debug.Log(shortestDistance);

        return nextDirection;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && movementController.isGhost == true)
        {
            playerCaught = true; 
            gameManager.state = GameManager.GameStates.gameOver;
        }
    }
};