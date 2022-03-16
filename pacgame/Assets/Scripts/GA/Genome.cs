using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Genome
{
    public List<int> vecBits = new List<int>(); // vector of binary digits representing genes.
    // indices 0 - 1 = ghost types. indices 2 - 4 = ghost speed. 
    public List<int> vecDecoded = new List<int>(); // vector of size 2. index 0 = color, index 1 = speed.
    public double fitScore = 0.0; // fitness value of genome
    public int speed; // speed deciphered from genes. 
    public int color; // color deciphered from genes, probably unnecessary

    public int age = 0;

    public double totalDistancePlayer = 0.0; // total distance from player, used to calculate average
    public bool playerCollide = false;

    public GameObject prefab;

    // Constructor, initialize Genome with random binary digits
    // 00 = Red, 01 = Pink, 10 = Blue, 11 = Orange
    public Genome(int num_bits, System.Random randomInstance) { // WORKS
        fitScore = 0;
        for (int i = 0; i < num_bits; i++) { // randomize binary digits 
            int bit = randomInstance.Next(2);
            vecBits.Add(bit);     
        }
    }

    // Constructor 2, initialize Genome with nothing
    public Genome() {
        fitScore = 0;
    }

    // Get - Set methods. Implement if necessary. 

}
