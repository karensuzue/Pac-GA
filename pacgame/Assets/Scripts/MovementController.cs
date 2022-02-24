using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject nextNode; // given in inspector
    // private GameObject teleportR;
    // private GameObject teleportL;
    public float speed; // switch to private later

    public string currDirection = ""; // switch to private after done with debuggin
    public string nextDirection = "";

    public bool isGhost = false;

    private NodeController nodeController;

    void Start()
    {
        // teleportL = GameObject.FindGameObjectWithTag("TeleportL");
        // teleportR = GameObject.FindGameObjectWithTag("TeleportR");
    }

    // Update is called once per frame
    void FixedUpdate() // handles the movement
    {
        transform.position = Vector2.MoveTowards(transform.position, nextNode.transform.position,
         speed * Time.deltaTime);
    }

    void Update()
    {
        nodeController = nextNode.GetComponent<NodeController>();

        if (nodeController.pellet && gameObject.tag == "Player")
        {
            nodeController.pellet = false;
            GameObject child = nextNode.transform.GetChild(0).gameObject;
            child.SetActive(false);

            if (nodeController.energizer)
            {
                nodeController.energizer = false;
            }

            GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
            // Debug.Log(pellets.Length);

            GameObject[] energizers = GameObject.FindGameObjectsWithTag("Energizer");
            // Debug.Log(energizers.Length);
        }

        if (transform.position.x == nextNode.transform.position.x && transform.position.y == nextNode.transform.position.y)
        {
            // making sure that pacman is right on the node and not somewhere else (to prevent it from getting stuck)
            // only when it is right on the middle of the node shoudl we change the direction
            GameObject tempNode = nodeController.GetNextNode(nextDirection);
            if (tempNode != null)
            {
                nextNode = tempNode;
                currDirection = nextDirection;
            }            

            /*else if (tempNode == null && currDirection != "")
            {
                if (currDirection != "")
                {
                    tempNode = nodeController.GetNextNode(currDirection);
                    nextNode = tempNode;
                }
                
            }*/
            
           
        }
    }

    public void SetDirection(string direction)
    {
        nextDirection = direction;
    }

}
