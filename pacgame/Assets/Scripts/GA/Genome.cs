using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Genome
{
    public List<int> vecBits = new List<int>(); // vector of binary digits representing genes.
    // indices 0 - 1 = ghost types. indices 2 - 4 = ghost speed. 
    public double fitScore; // fitness value of genome
    public float speed; // speed deciphered from genes. 
    public string color; // color deciphered from genes, probably unnecessary

    public GameObject prefab; // maybe needed

    // Constructor, initialize Genome with random binary digits
    // 00 = Red, 01 = Pink, 10 = Blue, 11 = Orange
    public Genome(int num_bits) { // WORKS
        fitScore = 0;
        System.Random rndm = new System.Random();
        for (int i = 0; i < num_bits; i++) { // randomize binary digits 
            int bit = rndm.Next(2);
            vecBits.Add(bit);     
        }
    }

    // Get - Set methods. Implement if necessary. 

}
