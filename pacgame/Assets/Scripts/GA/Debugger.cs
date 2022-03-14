using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    // Start is called before the first frame update
    private static System.Random random = new System.Random();
    async void Start()
    {
        Genome test1 = new Genome(5, random);
        Genome test2 = new Genome(5, random);

        Genome test3 = new Genome();
        Genome test4 = new Genome();
        Crossover(ref test1.vecBits, ref test2.vecBits, ref test3.vecBits, ref test4.vecBits);

        /*
        Debug.Log("MOM");
        for (int i = 0; i < 5; i++) {
            Debug.Log(test1.vecBits[i]);
        }

        Debug.Log("DAD");
        for (int i = 0; i < 5; i++) {
            Debug.Log(test2.vecBits[i]);
        }
        */

        Debug.Log("BABY1");
        for (int i = 0; i < 5; i++) {
            Debug.Log(test3.vecBits[i]);
        }

        /*Debug.Log("BABY2");
        for (int i = 0; i < 5; i++) {
            Debug.Log(test4.vecBits[i]);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void Mutate(ref List<int> genomeBits) {
        for (int i = 0; i < 5; i++) {
            // flip?
            if ((float)random.NextDouble() < 0.001f) {
                if (genomeBits[i] == 0) { genomeBits[i] = 1; }
                if (genomeBits[i] == 1) { genomeBits[i] = 0; }
            }
        }
    }

    private void Crossover(ref List<int> mom, ref List<int> dad, ref List<int> child1, ref List<int> child2) { // in this case only one child is produced instead of 2 to ensure that there is always four ghosts in the game
        if (((float)random.NextDouble() > 1f) || (mom == dad)) { // float uses less memory, double not needed in this case?
            child1 = mom; 
            child2 = dad;
            return;
        }

        else {
            int crossPoint = random.Next(5); // random point chosen to swap
            for (int i = 0; i < crossPoint; i++) {
                child1.Add(mom[i]);
            }

            for (int i = crossPoint; i < 5; i++) {
                child1.Add(dad[i]);
            }

            for (int i = 0; i < crossPoint; i++) {
                child2.Add(dad[i]);
            }

            for (int i = crossPoint; i < 5; i++) {
                child2.Add(mom[i]);  
            }
        }
    }
}
