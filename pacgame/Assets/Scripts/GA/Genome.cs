using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * Class represents a genome in the population.
*/
public class Genome
{
    // List of binary digits representing genes
    // indices 0 - 1 = ghost types. indices 2 - 4 = ghost speed
    public List<int> vecBits = new List<int>(); 
    // Vector of size 2 containing decoded genes: index 0 = color, index 1 = speed
    public List<int> vecDecoded = new List<int>(); 
    // Fitness value of genome
    public double fitScore = 0.0; 
    // Helper variable used to retain original/unscaled fitness score
    public double fitScoreOld = 0.0;
    // Speed deciphered from genes
    public int speed; 
    // Color deciphered from genes
    public int color; 
    // Genome age (according to how many generations it has survived)
    public int age = 0;
    // Total distance from player, used to calculate average
    public double totalDistancePlayer = 0.0; 
    public bool playerCollide = false; 
    public bool closestToPlayer = false; // For fitness function

    // GameObject instance associated with the genome
    public GameObject prefab;

    /**
     * Constructor, initialize Genome with random binary digits
     * 00 = Red, 01 = Pink, 10 = Blue, 11 = Orange
     * @param num_bits Number of bits per genome
     * @param randomInstance Random generator
    */
    public Genome(int num_bits, System.Random randomInstance) { // WORKS
        fitScore = 0;
        for (int i = 0; i < num_bits; i++) { // randomize binary digits 
            int bit = randomInstance.Next(2);
            vecBits.Add(bit);     
        }
    }

    /**
     * Constructor 2, initializes Genome
    */
    public Genome() {
        fitScore = 0;
    }
}
