using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// if changing enemy types, probably cna instantiate them as prefabs. 
// if changing speed, can do 8r + 2b + 3p + 5o or something idk  
public class EnemyController : MonoBehaviour
{
    // red = -1.6, -4
    // blue = 1.69, -4
    // orange = -1.6, -2
    // pink = 1.69, -2

    /*string[] modes = {"chase", "scatter", "scared", "win"}; // win state = ghost on same tile as pacman, level controller steps in
    int current_mode = 1;

    public string nextDirection;
    public string destination;*/

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

    public enum GhostColor
    {
        red,
        blue,
        pink,
        orange
    }

    public GhostColor ghostColor;

    // has to be public 
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    // can be made private 
    public GameObject startingNode;
    public bool leaveHome = false;

    private GameObject player;
    private MovementController movementController;

    void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player"); // change this to the one in level controller
        movementController = gameObject.GetComponent<MovementController>();
        movementController.isGhost = true;

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
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        movementController.nextNode = startingNode;
        leaveHome = true; // maybe control this from level controller later, or ask if ui is at respwan state
        // movementController.nextDirection = "right";
    }

    // Update is called once per frame
    void Update()
    {
        /*if (movementController.nextNode == player.GetComponent<PlayerController>().GetPlayerNextNode())
        {
            current_mode = 3; // win, trigger UI
        }*/
        if (ghostNodePositions == GhostNodePositions.moveToPath)
        {
            if (ghostColor == GhostColor.red) {
                Vector2 target = player.transform.position;
                string nextDirection = GetClosestDirection(target);
                movementController.SetDirection(nextDirection);
            }
            
            //if (ghostColor == GhostColor.blue) {
                // Vector2 target = 
            //}

            if (ghostColor == GhostColor.pink) {

                Vector2 target = player.GetComponent<MovementController>().nextNode.transform.position * 4;
                string nextDirection = GetClosestDirection(target);
                movementController.SetDirection(nextDirection);
            }
        }   

        else if (ghostNodePositions == GhostNodePositions.respawning)
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
        }
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

        return nextDirection;
    }
};