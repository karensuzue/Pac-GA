using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public bool left = false; //switch all to priv after done with debug 
    public bool right = false;
    public bool up = false;
    public bool down = false;

    public bool pellet = true;
    public bool energizer = false;

    public GameObject nodeLeft;
    public GameObject nodeRight;
    public GameObject nodeUp;
    public GameObject nodeDown;

    // private BoxCollider2D boxCollider;

    // Start is called before the first frame update

    void Awake()
    {
        // boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {

        RaycastHit2D[] hits;

        hits = Physics2D.RaycastAll(transform.position, Vector2.up);
        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Mathf.Abs(hits[i].point.y - transform.position.y);
            if ((distance <= 1) && (hits[i].collider.gameObject.layer == 9))
            {
                up = true;
                nodeUp = hits[i].collider.gameObject;
            } 

        }

        hits = Physics2D.RaycastAll(transform.position, -Vector2.up);
        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Mathf.Abs(hits[i].point.y - transform.position.y);
            if ((distance <= 1) && (hits[i].collider.gameObject.layer == 9))
            {
                down = true;
                nodeDown = hits[i].collider.gameObject;
            }
        }

        hits = Physics2D.RaycastAll(transform.position, Vector2.left);
        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Mathf.Abs(hits[i].point.x - transform.position.x);
            if ((distance <= 1) && (hits[i].collider.gameObject.layer == 9))
            {
                left = true;
                nodeLeft = hits[i].collider.gameObject;
            }
        }

        hits = Physics2D.RaycastAll(transform.position, Vector2.right);
        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Mathf.Abs(hits[i].point.x - transform.position.x);
            if ((distance <= 1) && (hits[i].collider.gameObject.layer == 9))
            {
                right = true;
                nodeRight = hits[i].collider.gameObject;
            }
        }

        if (pellet)
        {
            GameObject child = transform.GetChild(0).gameObject;
            if (child.layer == 11) // if energizer
            {
                energizer = true;
            }
        }
        

    }

    public GameObject GetNextNode(string direction) // getter as all variables will be set to priv
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
