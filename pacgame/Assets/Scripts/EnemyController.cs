using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // red = -1.6, -4
    // blue = 1.69, -4
    // orange = -1.6, -2
    // pink = 1.69, -2

    string[] modes = {"chase", "scatter", "scared", "win"}; // win state = ghost on same tile as pacman, level controller steps in
    int current_mode = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
