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
    private bool gameRunning; // true if game is currently running
    private int ghostTotal;
    private List<GameObject> generatedGhosts = new List<GameObject>();

    // 00 or 0 = Red, 01 or 1 = Pink, 10 or 2 = Blue, 11 or 3 = Orange
    public GameObject redPrefab;
    public GameObject bluePrefab;
    public GameObject pinkPrefab;
    public GameObject orangePrefab;

    /*
    private enum GhostColors
    {
        red,
        pink,
        blue,
        orange
    }
    private GhostColors ghostColor;
    */

    public GameObject gameManagerObj; // drag and drop in Inspector
    public GameManager gameManager;

    // public GameObject ghostsOriginal; // drag and drop in Inspector
    /*
    public GameObject ghostStart; // drag and drop in Inspector
    public GameObject ghostStartLeft; // public for debug purpose
    public GameObject ghostStartRight;
    public GameObject ghostStartCenter;
    public GameObject ghostStartStart;
    */

    public GameObject redOg; // original placements of ghosts in game to ensure instantiation placement is correct. Comes from disabled objects in hierarchy instead of prefab. 
    public GameObject pinkOg;
    public GameObject blueOg;
    public GameObject orangeOg;

    private GameObject prefabTest;
    private GameObject prefabTest2;
    private GameObject prefabTest3;
    private GameObject prefabTest4;

    void Awake()
    {
        // initialization goes here
        gameManager = gameManagerObj.GetComponent<GameManager>();
        /*
        ghostStartCenter = ghostStart.transform.GetChild(0).gameObject;
        ghostStartLeft = ghostStart.transform.GetChild(1).gameObject;
        ghostStartRight = ghostStart.transform.GetChild(2).gameObject;
        ghostStartStart = ghostStart.transform.GetChild(3).gameObject;
        */
        ghostTotal = gameManager.ghostTotal;

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

        InstantiateGhostPrefab(genomDecoded);

        /*
        CreateStartingPopulation();
        Debug.Log("Test one from population");
        for (int i = 0; i < chromLength; i++) {
            Debug.Log(vecPopulation[0].vecBits[i]);   
        }*/
        // GameObject redOg = GameObject.FindGameObjectWithTag("RedGhost");
        
        /*
        prefabTest = Instantiate(redPrefab);
        prefabTest.transform.position = redOg.transform.position;
        // prefabTest.GetComponent<MovementController>().nextNode = ghostStart;
        Debug.Log(redOg.transform.position);
        Debug.Log(prefabTest.transform.position);

        prefabTest2 = Instantiate(pinkPrefab);
        prefabTest2.transform.position = pinkOg.transform.position;

        prefabTest3 = Instantiate(bluePrefab);
        prefabTest3.transform.position = blueOg.transform.position;

        prefabTest4 = Instantiate(orangePrefab);
        prefabTest4.transform.position = orangeOg.transform.position;
        */

        // GameObject obj = GameObject.FindGameObjectWithTag("RedGhost");
        // prefab.transform.position = obj.transform.position;
        // prefab.transform.position = ;
        // GameObject prefab2 = Instantiate(pinkPrefab);
        // prefab2.transform.position = ghostStartRight.transform.position;
        

        // CreateStartingPopulation();
        // gameRunning = true;
        
    }

   
    void FixedUpdate()
    {
        // GameManager.GameStates state = gameManager.state;

        // if (state == GameManager.GameStates.win || state == GameManager.GameStates.gameOver) {
            // gameRunning = false;
        // }

        // Epoch();  


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

    private void Reset() {

    }*/

    private void CreateStartingPopulation() { // WORKS
        for (int i = 0; i < ghostTotal; i++) { // initialize 4 ghosts
            Genome genome = new Genome(chromLength);
            vecPopulation.Add(genome);
        }   
    }

    private List<int> Decode(List<int> bits) { // WORKS
        // decodes genes, returns a list containing numbers for ghost color and speed
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

    private void NumberToColorMap(List<int> decodedChrom) {
        // maps first number of decoded chromosome to color

    }

    private void InstantiateGhostPrefab(List<int> decodedChrom) {
        // instantiate ghost prefab for a decode genome ([color, speed])
        // GameObject ghostPrefab = Instantiate<GameObject>();
        // 00 or 0 = Red, 01 or 1 = Pink, 10 or 2 = Blue, 11 or 3 = Orange
        int color = decodedChrom[0];
        int speed = decodedChrom[1];

        GameObject prefab;
        MovementController movementController;

        if (color == 0) { // red 
            prefab = Instantiate(redPrefab);
            prefab.transform.position = redOg.transform.position;
            movementController = prefab.GetComponent<MovementController>();
            movementController.speed = speed;
            generatedGhosts.Add(prefab);
        }
        else if (color == 1) { // pink 
            prefab = Instantiate(pinkPrefab);
            prefab.transform.position = pinkOg.transform.position;
            movementController = prefab.GetComponent<MovementController>();
            movementController.speed = speed;
            generatedGhosts.Add(prefab);
        }
        else if (color == 2) { // blue
            prefab = Instantiate(bluePrefab);
            prefab.transform.position = blueOg.transform.position;
            movementController = prefab.GetComponent<MovementController>();
            movementController.speed = speed;
            generatedGhosts.Add(prefab);
        }
        else if (color == 3) { // orange
            prefab = Instantiate(orangePrefab);
            prefab.transform.position = orangeOg.transform.position;
            movementController = prefab.GetComponent<MovementController>();
            movementController.speed = speed;
            generatedGhosts.Add(prefab);
        }
    }
    
    private void DestroyGhostPrefabs() {
        // destroys all ghost prefabs on screen
        for (int i = 0; i < ghostTotal; i++) {
            generatedGhosts.Remove(generatedGhosts[i]);
        }
    }


    private void Epoch() {
        if (gameRunning == false) {
            // calculate fitness score
            // perform operations 
            // loop through population, decode each, after decoding one you instantiate the ghost prefab right away
        }

    }
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
