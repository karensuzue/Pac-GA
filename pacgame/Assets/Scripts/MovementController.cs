using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * MovementController class used for simulating character movement.
 * Keeps track of next directions. 
*/
public class MovementController : MonoBehaviour
{
    public GameObject nextNode; // Initialize through Inspector
    public float speed; // Character speed

    public string currDirection = ""; // Current direction
    public string nextDirection = ""; // Next direction

    public bool isGhost = false; // true if character is ghost/enemy type

    // NodeController class attached to node GameObject instance. 
    private NodeController nodeController; 

    /** 
     * FixedUpdate is called at a fixed interval independent of frame rate. 
     * Handles movement
    */
    void FixedUpdate()
    {
        // Transform posiiton of character to simulate movement 
        transform.position = Vector2.MoveTowards(transform.position, nextNode.transform.position,
         speed * Time.deltaTime);
    }

    /** 
     * Update is called once per frame.
     * Handles changes in direction.
    */
    void Update()
    {
        // Reference NodeController class of next node to move to
        nodeController = nextNode.GetComponent<NodeController>();

        // If next node has pellet and current character is player
        if (nodeController.pellet && gameObject.tag == "Player")
        {
            // disable pellet
            nodeController.pellet = false; 
            GameObject child = nextNode.transform.GetChild(0).gameObject;
            child.SetActive(false);

            // if energizer, disable energizer
            if (nodeController.energizer)
            {
                nodeController.energizer = false;
            }
        }

        // Makes sure that character is directly on top of node 
        // Only when character is right on the middle of node should we change direction
        if (transform.position.x == nextNode.transform.position.x && transform.position.y == nextNode.transform.position.y)
        {
            // Get node for next direction
            GameObject tempNode = nodeController.GetNextNode(nextDirection);

            // If node exists for such direction
            if (tempNode != null) 
            {
                nextNode = tempNode; // Finalize next node
                currDirection = nextDirection; // Update current direction
            }                      
           
        }
    }

    /**
     * Set next direction
     * @param direction A string holding target direction
    */
    public void SetDirection(string direction)
    {
        nextDirection = direction;
    }

}
