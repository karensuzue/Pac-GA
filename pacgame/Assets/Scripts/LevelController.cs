using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private float timeTotal;
    public float score;
    private int ghostTotal = 4;

    private int lives = 3;

    private GameObject[] pellets;
    public int pelletCount;
    public GameObject player; // set back to private after done 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        pellets = GameObject.FindGameObjectsWithTag("Pellet");
        pelletCount = pellets.Length;
        // if pelletCount == 0, then next round 
        // taking too long = reduce
        // too many deaths = reduce
    }
    public float GetScore()
    {
        return score;
    }

    public float GetTime()
    {
        return timeTotal;
    }
}
