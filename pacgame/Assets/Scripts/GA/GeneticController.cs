using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeneticController : MonoBehaviour
{
    private List<Genome> vecPopulation = new List<Genome>();
    private int popSize; // population size
    private int geneLength; 
    private int chromLength = 5; // number of bits per chromosome (vecBits size)
    private int speedChromLength = 3; // number of bits per speed chromosome
    private int colorChromLength = 2; // number of bits per color(type) chromosome
    private int speedIndexStart = 2; // speed starts at index 2 of the Genome bit list
    
    private double mutationRate;
    private double crossoverRate;

    private double bestFitness; // best fitness score
    private Genome bestGenome;

    private int generation; // current generation
    private bool busy; // true if current run in progress

    public GameObject gameManagerObj; // drag and drop in Inspector
    private int ghostTotal;
    private List<GameObject> generatedGhosts = new List<GameObject>();

    // 00 or 0 = Red, 01 or 1 = Pink, 10 or 2 = Blue, 11 or 3 = Orange
    public GameObject redPrefab;
    public GameObject bluePrefab;
    public GameObject pinkPrefab;
    public GameObject orangePrefab;
    

    void Awake()
    {
        // initialization goes here
        ghostTotal = gameManagerObj.GetComponent<GameManager>().ghostTotal;

    }
    void Start() {
        // test to see if genome initialization works 
        Genome genom = new Genome(chromLength);
        for (int i = 0; i < chromLength; i++) {
            Debug.Log(genom.vecBits[i]);   
        }

        List<int> genomDecoded = Decode(genom.vecBits);
        Debug.Log("Genome Decoded:");
        Debug.Log(genomDecoded[0]);
        Debug.Log(genomDecoded[1]);


        CreateStartingPopulation();
        Debug.Log("Starting Population:");
        for (int i = 0; i < ghostTotal; i++) {
            Debug.Log(vecPopulation[i]);
            Debug.Log("Genome" + i.ToString());
            for (int j = 0; j < chromLength; i++) {
                Debug.Log(genom.vecBits[j]);
            }
        }

        
    }

    // Start is called before the first frame update
    void FixedUpdate()
    {
          
    }

    // Methods needed to implement:
    // Crossover (Single point)
    // Mutation
    // Selection (Roulette Wheel)
    // Calculate fitness score for entire population
    // Decipher speed from genome.
    // Decipher color/ghost type from genome. (CAN group with other function to form one decode function)
    // create start function
    // convert bin to decimal to use for decode

    /*
    private void Mutate(ref List<Genome> vecBits) {

    }

    private void Crossover(ref List<int> mom, ref List<int> dad, List<int> baby1, List<int> baby2) {
        // we use single-point crossover
    }

    private Genome RouletteWheelSelection() {

    }


    private void CalculatePopulationFitness() {

    }

    private void Epoch() {

    }

    private void Reset() {

    }*/

    private void CreateStartingPopulation() { // WORKS
        for (int i = 0; i < ghostTotal; i++) { // initialize 4 ghosts
            Genome genome = new Genome(chromLength);
            vecPopulation.Add(genome);
        }   
    }

    private List<int> Decode(List<int> bits) { // WORKS
        // decodes genes, returns a list containing the ghost color and speed
        // Input genome's list of bits
        // Outputs a size 2 list containing the numbers representing color + speed
        List<int> decodedGenome = new List<int>();

        List<int> colorBits = new List<int>();
        List<int> speedBits = new List<int>();
        for (int i = 0; i < colorChromLength; i++) { // convert binary to integer for color
            colorBits.Add(bits[i]);
        }

        for (int i = speedIndexStart; i < chromLength; i++) {
            speedBits.Add(bits[i]);
        }

        decodedGenome.Add(Convert.ToInt32(string.Join("", colorBits), 2));
        decodedGenome.Add(Convert.ToInt32(string.Join("", speedBits), 2));

        return decodedGenome;
    }

    /*
    private void InstantiateGhostPrefabs() {
        GameObject ghostPrefab = Instantiate<GameObject>();

        

    }
    
    private void DestroyGhostPrefabs() {

    }*/

    // Accessor methods

    public int GetGeneration() {
        return generation;
    }

    public Genome GetFittestGenome() {
        return bestGenome;
    }

    public double GetFittestFitnest() {
        return bestFitness;
    }

}
