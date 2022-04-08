using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The NodeController class contains information for a single node GameObject on the maze.
 * Helps keep track of valid paths on the map, which are formed from a collection of nodes
*/
public class NodeController : MonoBehaviour
{
    // Keep track of nearby nodes in four cardinal directions
    public bool left = false; // false if node doesn't exist
    public bool right = false;
    public bool up = false;
    public bool down = false;

    // References to nearby node GameObjects are stored here
    public GameObject nodeLeft; 
    public GameObject nodeRight;
    public GameObject nodeUp;
    public GameObject nodeDown;


    // Does node contain pellet or energizer? 
    public bool pellet = true; // True by default
    public bool energizer = false;

    /**
     * Start is called before the first call to Update. 
     * This method runs once per scene.
     * Occurs at the very start of the game, the first frame.  
    */
    void Start()
    {
        // Raycast in all four directions to check if nodes exist nearby
        RaycastHit2D[] hits;

        hits = Physics2D.RaycastAll(transform.position, Vector2.up); // towards north
        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Mathf.Abs(hits[i].point.y - transform.position.y);
            // Nodes are recognized through layer 9 and a distance of 1
            if ((distance <= 1) && (hits[i].collider.gameObject.layer == 9))
            {
                up = true;
                nodeUp = hits[i].collider.gameObject;
            } 

        }

        hits = Physics2D.RaycastAll(transform.position, -Vector2.up); // towards south
        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Mathf.Abs(hits[i].point.y - transform.position.y);
            if ((distance <= 1) && (hits[i].collider.gameObject.layer == 9))
            {
                down = true;
                nodeDown = hits[i].collider.gameObject;
            }
        }

        hits = Physics2D.RaycastAll(transform.position, Vector2.left); // towards west
        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Mathf.Abs(hits[i].point.x - transform.position.x);
            if ((distance <= 1) && (hits[i].collider.gameObject.layer == 9))
            {
                left = true;
                nodeLeft = hits[i].collider.gameObject;
            }
        }

        hits = Physics2D.RaycastAll(transform.position, Vector2.right); // towards east
        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Mathf.Abs(hits[i].point.x - transform.position.x);
            if ((distance <= 1) && (hits[i].collider.gameObject.layer == 9))
            {
                right = true;
                nodeRight = hits[i].collider.gameObject;
            }
        }

        if (pellet) // If pellet exists in node 
        {
            // Check for energizer
            GameObject child = transform.GetChild(0).gameObject;
            if (child.tag == "Energizer") 
            {
                energizer = true;
            }
            else
            {
                energizer = false;
            }
        }
        

    }

    /**
     * Retrieves next node to move to.
     * @param direction A string holding target direction.
     * @return A nearby node GameObject.
    */
    public GameObject GetNextNode(string direction)
    {
        GameObject nextNode = null;

        if (direction == "left" && left)
        {
            nextNode = nodeLeft;
        }

        if (direction == "right" && right)
        {
            nextNode = nodeRight;
        }
        
        if (direction == "up" && up)
        {
            nextNode = nodeUp;
        }

        if (direction == "down" && down)
        {
            nextNode = nodeDown;
        }

        return nextNode;
    }

}
