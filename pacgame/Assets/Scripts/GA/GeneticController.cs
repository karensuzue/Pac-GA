using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// SAVED = Made static variable in GeneticData.cs, retain information after reloading scene
public class GeneticController : MonoBehaviour
{
    private List<Genome> vecPopulation = new List<Genome>(); // SAVED
    private int popSize = 4; // population size
    private int chromLength = 5; // number of bits per chromosome (vecBits size)
    private int speedChromLength = 3; // number of bits per speed chromosome
    private int colorChromLength = 2; // number of bits per color(type) chromosome
    private int speedIndexStart = 2; // speed starts at index 2 of the Genome bit list
    
    private float mutationRate = 0.001f;
    private float crossoverRate = 0.7f;

    private double bestFitness; // best fitness score
    private double worstFitness; 
    private double totalFitness; // total fitness score of the current population
    private Genome fittestGenome; // fittest genome of the population
    private Genome worstGenome; 
    private int fittestGenomeIndex;
    private int worstGenomeIndex; 

    private int generation; // current generation
    private bool gameRunning; // true if game is currently running
    private List<GameObject> generatedGhosts = new List<GameObject>(); // ghosts instantiated on screen

    // 00 or 0 = Red, 01 or 1 = Pink, 10 or 2 = Blue, 11 or 3 = Orange
    public GameObject redPrefab;
    public GameObject bluePrefab;
    public GameObject pinkPrefab;
    public GameObject orangePrefab;

    public GameObject redOg; // original placements of ghosts in game to ensure instantiation placement is correct. Comes from disabled objects in hierarchy instead of prefab. 
    public GameObject pinkOg;
    public GameObject blueOg;
    public GameObject orangeOg;

    public GameObject gameManagerObj; // drag and drop in Inspector
    public GameManager gameManager;

    public GameObject geneticDataObj;
    public GeneticData geneticData;

    private static System.Random random = new System.Random();

    void Awake()
    {
        // initialization goes here
        gameManager = gameManagerObj.GetComponent<GameManager>();
        geneticData = geneticDataObj.GetComponent<GeneticData>();
        gameRunning = false;

        generation = geneticData.GetGeneration();
        vecPopulation = geneticData.GetVecPopulation();
    }

    void Start() {       
        gameRunning = true;
        generation += 1;
        geneticData.SetGeneration(generation);
        if (generation == 1) {
            CreateStartingPopulation();
            for (int i = 0; i < popSize; i++) { // Decode loop
                List<int> decoded = Decode(vecPopulation[i].vecBits);
                vecPopulation[i].vecDecoded = decoded;
                vecPopulation[i].color = decoded[0]; // might not be necessary
                vecPopulation[i].speed = decoded[1]; // might not be necessary 
                InstantiateGhostPrefab(vecPopulation[i].vecDecoded);
            }

            geneticData.SetVecPopulation(vecPopulation); // save current population
        }

        if (generation > 1) {
            Epoch();
        }

        UpdateGenomeAge(); 
        geneticData.SetVecPopulation(vecPopulation); // any changes to vecPopulation has to be resaved   
        
    }

   
    void Update()
    {
        GameManager.GameStates state = gameManager.state;

        if (state == GameManager.GameStates.win || state == GameManager.GameStates.gameOver) {
            // if game won or over
            gameRunning = false; // game no longer running, paused
            if (generatedGhosts.Count > 0) {
                DestroyGhostPrefabs();
                 // move to next generation
            }
        }
    }
    
    // PRIVATE METHODS
    private void CreateStartingPopulation() { // WORKS
        for (int i = 0; i < popSize; i++) { // initialize 4 ghosts
            Genome genome = new Genome(chromLength, random);
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

    private void InstantiateGhostPrefab(List<int> decodedChrom) { // WORKS 
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
    
    private void DestroyGhostPrefabs() { // WORKS
        // destroys all ghost prefabs on screen, prepare for next one
        for (int i = 0; i < generatedGhosts.Count; i++) {
            GameObject current = generatedGhosts[i];
            generatedGhosts.RemoveAt(i);
            Destroy(current);
        }
    }

    private double FitnessFunction(List<int> bits) {
        // updates fitness score to one individual 
        double fitness = 0;



        return fitness;
    }

    private void CalculatePopulationFitness() {
        // assigns fitness score to all members of the population

    }

    private void ScaleFitnessScore() {
        // scale to prevent quick convergence and ensure population diversity 

    }

    // choose one good genome to bring over?
    // check crossover on all genomes in population? or use selection functions to choose out two genomes for this purpose
    // 
    private Genome RouletteWheelSelection() {
        // doesnt guarantee the best is selected, but those with higher fitness have a higher chance
        // System.Random rand = new System.Random();
        double slice = random.NextDouble() * (totalFitness); // random number between 0 and totalFitness

        int selectedGenomeIndex = 0;
        double currentTotal = 0;

        for (int i = 0; i < vecPopulation.Count; i++) {
            currentTotal += vecPopulation[i].fitScore;

            if (currentTotal > slice) {
                selectedGenomeIndex = i;
                break;
            }

        }
        return vecPopulation[selectedGenomeIndex];
    }

    private Genome SUSSelection() {
        // Stochastic Universal Sampling
        // better for small population
        // relies on fitness being nonnegative
        // sigma scaling can give negative fitness score
        int selectedGenomeIndex = 0;
        if (worstFitness < 0) {

        }
        return vecPopulation[selectedGenomeIndex];
    }

    private int BestGenomeFinder() {
        int bestGenomeIndex = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            if (vecPopulation[i].fitScore > vecPopulation[bestGenomeIndex].fitScore) {
                bestGenomeIndex = i;
            }
        }
        return bestGenomeIndex;
    }

    private int WorstGenomeFinder() {
        // removes the lowest performing genome in the population and returns it, good for updating worst genome
        // add an age variable where it only removes the genome if its been there for at least 1-2 runs 
        int worstIndex = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            if (vecPopulation[i].fitScore < vecPopulation[worstIndex].fitScore) {
                if (generation == 0 || vecPopulation[i].age > 1) { // gives chance for diversity/innovation
                    worstIndex = i;
                }
            }
        }

        return worstIndex;
    }

    private void UpdateGenomeAge() {
        // increases ages of all population members by 1
        for (int i = 0; i < vecPopulation.Count; i++) {
            vecPopulation[i].age += 1;
        }
    }

    /*private void Mutate(ref List<int> genomeBits) {
        for (int i = 0; i < chromLength; i++) {
            // flip?
            if ((float)random.NextDouble() < mutationRate) {
                genomeBits[i] = !genomeBits[i];
            }
        }
    }*/

    private void Crossover(ref List<int> mom, ref List<int> dad, ref List<int> child) { // in this case only one child is produced instead of 2 to ensure that there is always four ghosts in the game
        // System.Random rand = new System.Random();
        // List<int> child = new List<int>();
        // modify this so that two best genomes are always chosen 
        if (((float)random.NextDouble() > crossoverRate) || (mom == dad)) { // float uses less memory, double not needed in this case?
            int choice = random.Next(2); // because only one child is produced rather than two, choose one child randomly
            if (choice == 0) { child = mom; }
            if (choice == 1) { child = dad; }
            return;
        }

        else {
            int crossPoint = random.Next(chromLength); // random point chosen to swap
            int choice = random.Next(2);
            if (choice == 0) {
                for (int i = 0; i < crossPoint; i++) {
                    child.Add(mom[i]);
                }

                for (int i = crossPoint; i < chromLength; i++) {
                    child.Add(dad[i]);
                }
            }

            if (choice == 1) {
                for (int i = 0; i < crossPoint; i++) {
                    child.Add(dad[i]);
                }

                for (int i = crossPoint; i < chromLength; i++) {
                    child.Add(mom[i]);
                }
            }
        }
    }


    // PUBLIC METHODS 
    public void Epoch() {
        if (gameRunning == false) {
            // calculate fitness score
            // perform operations 
            // loop through population, decode each, after decoding one you instantiate the ghost prefab right away
        }

    }


    // ACCESSOR METHODS 
    public int GetGeneration() {
        return generation;
    }

    public Genome GetFittestGenome() {
        return fittestGenome;
    }
    
    public Genome GetWorstGenome() {
        return worstGenome;
    }

    public double GetBestFitnest() {
        return bestFitness;
    }

    public double GetWorstFitness() {
        return worstFitness;
    }

}
