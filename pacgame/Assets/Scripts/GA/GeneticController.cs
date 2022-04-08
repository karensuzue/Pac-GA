using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * GeneticController class handles evolution. 
*/
public class GeneticController : MonoBehaviour
{
    // List of genomes, population:
    private List<Genome> vecPopulation = new List<Genome>();
    // Population size
    private int popSize = 4; 
    
    /**
     * Chromosome Representation:
     *  0 0 | 0 0 0
     *  color | speed
     */

    // Number of bits per chromosome (vecBits size)
    private int chromLength = 5; 
    // Number of bits per speed chromosome
    private int speedChromLength = 3; 
    // Number of bits per color(type) chromosome
    private int colorChromLength = 2; 
    // Speed starts at index 2 of the Genome bit list
    private int speedIndexStart = 2; 
    
    // Mating Rates:
    private float mutationRate = 0.001f;
    private float crossoverRate = 0.7f;

    // Fitness:
    private double bestFitness; // Best fitness score
    private double worstFitness; 
    private double totalFitness; // Total fitness score of the current population
    private double averageFitness;
    private Genome fittestGenome; // Fittest genome object of the population
    private Genome worstGenome; 
    private int fittestGenomeIndex; // Index to access the fittest genome object in the population
    private int worstGenomeIndex; 
    
    // Game and Prefabs:
    private int generation; // Current generation, increases each time Start() is run
    private bool gameRunning; // True if game is currently running
    private List<GameObject> generatedGhosts = new List<GameObject>(); // Ghosts instantiated on screen
    bool prefabsCleared; // True if prefabs are all cleared

    // Prefabs to be instantiated on screen
    // 00 or 0 = Red, 01 or 1 = Pink, 10 or 2 = Blue, 11 or 3 = Orange
    public GameObject redPrefab;
    public GameObject bluePrefab;
    public GameObject pinkPrefab;
    public GameObject orangePrefab;

    // Original placements of ghosts in game.
    // Ensures instantiation placement is correct. 
    // These are disabled objects in the hierarchy.  
    public GameObject redOg; 
    public GameObject pinkOg;
    public GameObject blueOg;
    public GameObject orangeOg;

    // GameManager instance:
    public GameObject gameManagerObj; // Drop in Inspector
    public GameManager gameManager;

    // GeneticData, where information across runs get saved:
    public GameObject geneticDataObj; // Drop in Inspector
    public GeneticData geneticData;

    // Timer-related:
    private double nextUpdate = 0.0; // Update "interval" in seconds, used for UpdateEverySecond
    public int intervalCount = 0; // Number of 1-second intervals, for calculating average. 
            // Similar to nextUpdate but stops updating once playthrough ends. 
    
    // Random instance:
    private static System.Random random = new System.Random(); // Helps generate random numbers

    // The player:
    public GameObject player;

    // Export data to .csv file
    CSVWriter csv;

    /** 
     * Awake is the first method called when a Scene is loaded.
     * This method runs once per scene, and lasts until a Scene is unloaded. 
     * For initializing variables or states before the game starts.
    */
    void Awake()
    {
        gameManager = gameManagerObj.GetComponent<GameManager>();
        geneticData = geneticDataObj.GetComponent<GeneticData>();
        gameRunning = false;

        // Get "saves" from GeneticData
        generation = geneticData.GetGeneration();
        vecPopulation = geneticData.GetVecPopulation();
        intervalCount = geneticData.GetIntervalCount();

        // Find player instance
        player = GameObject.FindGameObjectWithTag("Player");

        // Before evolution starts, set new CSVWriter
        if (generation < 1) {
            csv = new CSVWriter();
            geneticData.SetCSVWriter(csv);
        }
        else {
            // Get existing CSVWriter
            csv = geneticData.GetCSVWriter();
        }
    }

    /**
     * Start is called before the first call to Update. 
     * This method runs once per scene.
     * Occurs at the very start of the game, the first frame.  
    */
    void Start() {
        gameRunning = true;

        generation += 1;
        geneticData.SetGeneration(generation); // send to GeneticData

        // If this is the first generation, initialize the population
        if (generation == 1) { 
            CreateStartingPopulation();

            // Go through all genomes in population
            for (int i = 0; i < popSize; i++) { 
                // Decode, returns [color, speed]
                List<int> decoded = Decode(vecPopulation[i].vecBits); 
                // Send to the current genome what's decoded
                vecPopulation[i].vecDecoded = decoded; 
                // Send to current genome it's decoded color
                vecPopulation[i].color = decoded[0];
                // Send to current genome it's decoded speed
                vecPopulation[i].speed = decoded[1]; 
                // Instantiate prefab, one ghost instance added to generatedGhosts
                InstantiateGhostPrefab(vecPopulation[i].vecDecoded); 
                // Send to current genome, gain access to the ghost instance screen
                vecPopulation[i].prefab = generatedGhosts[i];
                    
            }

            // Update age for all genomes 
            UpdateGenomeAge();
        }

        // If this is not the first generation, run Epoch
        if (generation > 1) {
            Epoch();
        }
        
        // Reset intervalCount
        intervalCount = 0; 

        // Any changes to vecPopulation has to be resaved to GeneticData   
        geneticData.SetVecPopulation(vecPopulation); 
        
        // Switch to keep Update from constantly trying to clear prefabs (even when there's none)
        prefabsCleared = false; 
    }

    /** 
     * Update is called once per frame.
    */
    void Update()
    {
        // Loop that runs in fixed 1 second interval, used for updating average distance from player in one playthrough
        if (Time.time >= nextUpdate) {
            nextUpdate = Mathf.FloorToInt(Time.time)+1;
            if (gameRunning == true) {
                intervalCount += 1;
            }
            UpdateEverySecond();
        }

        // Determine genome that collided with player
        // It is possible for multiple genomes to have playerCollide set to true
        for (int i = 0; i < vecPopulation.Count; i++) {
            GameObject currentPrefab = vecPopulation[i].prefab;
            EnemyController2 enemyController = currentPrefab.GetComponent<EnemyController2>();
            if (enemyController.playerCaught == true) {
                vecPopulation[i].playerCollide = true;
            }
        }

        // Current game state
        GameManager.GameStates state = gameManager.state; 
        // If game won or over
        if (state == GameManager.GameStates.win || state == GameManager.GameStates.gameOver) {
            gameRunning = false; // game no longer running, paused
            geneticData.SetIntervalCount(intervalCount);
        }

        // If game won
        if (state == GameManager.GameStates.win) {
            // Update shortest play time highscore
            if (gameManager.endTime < geneticData.GetShortestPlayTime()) { 
                // If end time of this playthrough is shorter than the shortest, update
                geneticData.SetShortestPlayTime(gameManager.endTime);
            }
        }
    }

    /** 
     * UpdateEverySecond is called once per second.
    */
    private void UpdateEverySecond() {
        // UpdateEverySecond is called once per second 
        if (gameRunning == true) {
            // Continue adding distance until playthrough ends 
            for (int i = 0; i < vecPopulation.Count; i++) {
                GameObject currentPrefab = vecPopulation[i].prefab;
                double distance = Vector2.Distance(currentPrefab.transform.position, player.transform.position);
                vecPopulation[i].totalDistancePlayer += distance;
            }
        }
    }
    

    /** 
     * PRIVATE METHODS
    */ 

    /**
     * Initializes population with random genomes.
    */ 
    private void CreateStartingPopulation() {
        vecPopulation = new List<Genome>();
        for (int i = 0; i < popSize; i++) {
            Genome genome = new Genome(chromLength, random);
            vecPopulation.Add(genome);
        }   
    }

    /**
     * Decode genome bit list.
     * @param bits List of bits, genome representation.
     * @return A list of size 2 containing decoded genes (color and speed) in int form.
    */ 
    private List<int> Decode(List<int> bits) { 
        List<int> decodedGenome = new List<int>();

        // Split bit list into two parts
        List<int> colorBits = new List<int>();
        List<int> speedBits = new List<int>();
        for (int i = 0; i < colorChromLength; i++) { 
            colorBits.Add(bits[i]);
        }

        for (int i = speedIndexStart; i < chromLength; i++) {
            speedBits.Add(bits[i]);
        }

        // Convert binary to integer
        decodedGenome.Add(Convert.ToInt32(string.Join("", colorBits), 2));
        decodedGenome.Add(Convert.ToInt32(string.Join("", speedBits), 2));

        return decodedGenome; // returns [int color, int speed]
    }

    /**
     * Instantiate ghost GameObjects on screen.
     * @param decodedChrom List containing decoded genes for a single Genome.
    */ 
    private void InstantiateGhostPrefab(List<int> decodedChrom) {
        // 00 or 0 = Red, 01 or 1 = Pink, 10 or 2 = Blue, 11 or 3 = Orange
        int color = decodedChrom[0];
        int speed = decodedChrom[1];

        GameObject prefab;
        MovementController movementController;

        if (color == 0) { // red 
            prefab = Instantiate(redPrefab);
            // Makes sure that ghost is placed at the right location on map
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
    
    /**
     * Destroy ghost GameObjects on screen to reset for next run/generation.
    */ 
    private void DestroyGhostPrefabs() {
        for (int i = 0; i < generatedGhosts.Count; i++) {
            GameObject current = generatedGhosts[i];
            generatedGhosts.RemoveAt(i);
            vecPopulation[i].prefab = null;
            Destroy(current);
        }
    }

    /**
     * Calculates fitness score for one genome. 
     * @param gen Genome object.
     * @return The fitness score.
    */ 
    private double FitnessFunction(Genome gen) {
        double fitness = 0;
        double average = gen.totalDistancePlayer / intervalCount;

        // If ghost stays still, this prevents player from completing the game
        if (gen.speed == 0) { 
            fitness -= 300; // High deduction, removes this ghost completely
        }

        // 3 objectives:
        // 1. Kill player, but only after 6 seconds into the game (min = 0, max = 10, weight = 1)
        // 2. If player wins, the genome with closest average distance gets an extra 10 points
            // (min = 0, max = 10, weight = 1.5)
        // 3. Average distance maintained throughout the game is larger than thresholds
            // (min = -658.902, max = 36, weight = 1)

        double obj1 = 0;
        double obj2 = 0;
        double obj3 = 0;

        // Objective 1
        double obj1Min = 0;
        double obj1Max = 10;
        if (gen.playerCollide == true) { // If ghost collides with player
            obj1 += 10;
            // If ghost collides with player too quickly 
            if (gameManager.endTime <= 6) {
                obj1 -= (10 - gameManager.endTime);
            }
        }

        // Objective 2
        // Checking for closest distance is done in CalculatePopulationFitness() 
        double obj2Min = 0;
        double obj2Max = 10;
        if (gameManager.state == GameManager.GameStates.win) {
            if (gen.closestToPlayer == true) {
                obj2 += 10;
            }
        }

        // Objective 3
        // Maximum distance from player is about 40.361 (from top and bottom diagonal corners)
        // Quadratic-based approach to objective 3
        // y = -(x - 8)(x - 20)
        // 8 and 20 are thresholds, ideally enemies have to stay between this range
        // Vertex (highest score/max) = (14, 36)
        // End points: (0, -160) and (40.361, -658.902) (the latter's y being min)

        double obj3Min = -658.902;
        double obj3Max = 36;
        obj3 = -1 * (average - 8) * (average - 20);

        fitness += ((obj1 - obj1Min) / (obj1Max - obj1Min))
            + 1.5 * ((obj2 - obj2Min) / (obj2Max - obj2Min))
            + ((obj3 - obj3Min) / (obj3Max - obj3Min));

        return fitness;
    }
    
    
    /**
     * Calculates fitness scores for entire population.
     * Also checks for genome closest to player on average.
    */ 
    private void CalculatePopulationFitness() {
        int bestIndex = -1; // Index of the genome closest to the player
        double bestAvg = 8000; // Best (smallest) average distance from player

        // Update closest genome
        for (int i = 0; i < vecPopulation.Count; i++) {
            Genome current = vecPopulation[i];
            double average = current.totalDistancePlayer / intervalCount;
            if (average < bestAvg) {
                bestAvg = average;
                bestIndex = i;
            }
        }
        if (bestIndex != -1) {
            vecPopulation[bestIndex].closestToPlayer = true;
        }
        
        // Calculate fitness scores
        for (int i = 0; i < vecPopulation.Count; i++) {
            double fitness = FitnessFunction(vecPopulation[i]);
            vecPopulation[i].fitScore = fitness;
            vecPopulation[i].fitScoreOld = fitness; // Retain raw fitness
        }
    }

    /**
     * Sigma scaling to prevent quick convergence.
     * Directly changes the fitness scores of each member of the population.
    */
    private void ScaleFitnessScores() {
        // Calculate the standard deviation
        double runningTotal = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            runningTotal += Math.Pow((vecPopulation[i].fitScore - averageFitness), 2);
        }

        double variance = runningTotal / popSize;
        double standev = Math.Sqrt(variance);

        if (standev == 0) {
            for (int i = 0; i < vecPopulation.Count; i++) {
                vecPopulation[i].fitScore = 0;
            }
        }
        else {
            // Loop through population, reassign fitness scores
            for (int i = 0; i < vecPopulation.Count; i++) {
                double oldFitness = vecPopulation[i].fitScore;
                vecPopulation[i].fitScore = (oldFitness - averageFitness) / (2 * standev);
            }
        }
        // Recalculate best, worst, average, total fitness scores
        CalcBestWorstAvTotFitness();
    }

    /**
     * Calculate best, worst, average, total fitness scores.
    */
    private void CalcBestWorstAvTotFitness() {
        fittestGenomeIndex = BestGenomeFinder(); // index of the fittest genome
        fittestGenome = vecPopulation[fittestGenomeIndex]; // fittest Genome object
        bestFitness = fittestGenome.fitScore; // fitness score of the fittest genome

        worstGenomeIndex = WorstGenomeFinder(); // index of the worst genome
        worstGenome = vecPopulation[worstGenomeIndex]; // worst fitness Genome object
        worstFitness = worstGenome.fitScore; // fitness score of the worst genome

        // Calculate total fitness
        double total = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            total += vecPopulation[i].fitScore;
        }
        totalFitness = total;

        // Calculate average fitness
        averageFitness = total / vecPopulation.Count;
    }

    /**
     * Roulette wheel selection.
     * Select genes for mating purposes.
     * Those with higher fitness have a higher change of being selected.
     * Doesn't guarantee that the best is selected, should be used with elitism.
     * @return Selected genome.
    */ 
    private Genome RouletteWheelSelection() {
        double slice = random.NextDouble() * (totalFitness); 

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

    /**
     * Finds the best genome, used for elitism.
     * returns The index of the best genome in the population list.
    */ 
    private int BestGenomeFinder() {
        int bestGenomeIndex = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            if (vecPopulation[i].fitScore > vecPopulation[bestGenomeIndex].fitScore) {
                bestGenomeIndex = i;
            }
        }
        return bestGenomeIndex;
    }

    /**
     * Finds the worst genome.
     * returns The index of the worst genome in the population list.
    */ 
    private int WorstGenomeFinder() {
        int worstIndex = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            if (vecPopulation[i].fitScore < vecPopulation[worstIndex].fitScore) {
                if (vecPopulation[i].age > 1) { 
                    // Gives chance for diversity/innovation, only select as worst if age is > 1
                    worstIndex = i;
                }
            }
        }

        return worstIndex;
    }

    /**
     * Increase ages of all population members by 1. 
    */ 
    private void UpdateGenomeAge() {
        for (int i = 0; i < vecPopulation.Count; i++) {
            vecPopulation[i].age += 1;
        }
    }

    /**
     * Mutation operator.
     * @param genomeBits List of bits for genome representation.
    */ 
    private void Mutate(ref List<int> genomeBits) {
        for (int i = 0; i < chromLength; i++) {
            // flip?
            if ((float)random.NextDouble() < mutationRate) {
                if (genomeBits[i] == 0) { genomeBits[i] = 1; }
                if (genomeBits[i] == 1) { genomeBits[i] = 0; }
            }
        }
    }
    
    /**
     * Crossover operator.
     * @param mom List of bits for parent genome.
     * @param dad List of bits for parent genome.
     * @param child1 List of bits for child genome.
     * @param child2 List of bits for child genome.
    */ 
    private void Crossover(ref List<int> mom, ref List<int> dad, ref List<int> child1, ref List<int> child2) {
        if (((float)random.NextDouble() > crossoverRate) || (mom == dad)) {
            child1 = mom; 
            child2 = dad;
            return;
        }

        else {
            int crossPoint = random.Next(chromLength); // random point chosen to swap
            for (int i = 0; i < crossPoint; i++) {
                child1.Add(mom[i]);
            }

            for (int i = crossPoint; i < chromLength; i++) {
                child1.Add(dad[i]);
            }

            for (int i = 0; i < crossPoint; i++) {
                child2.Add(dad[i]);
            }

            for (int i = crossPoint; i < chromLength; i++) {
                child2.Add(mom[i]);
            }
        }
    }


    /** 
     * PUBLIC METHODS 
    */
    
    /**
     * Epoch loop, performs evolutionary processes.
    */ 
    public void Epoch() {
        // Update population fitness
        CalculatePopulationFitness(); 
        // Update best, worst, total, and average fitness scores
        CalcBestWorstAvTotFitness(); 
        // Scale fitness scores, also update BWTA scores in the process
        // Updated fitness will be used for selection
        ScaleFitnessScores();

        // Write data to .csv file
        csv.WriteData(vecPopulation, generation);

        // Destroy prefabs on screen
        if (generatedGhosts.Count > 0 && prefabsCleared == false) {
            DestroyGhostPrefabs();
            prefabsCleared = true;
        }

        // Create new population
        List<Genome> newPop = new List<Genome>();

        // Crossover time! Produce 2 children genomes
        Genome mom = RouletteWheelSelection();
        Genome dad = RouletteWheelSelection();
        Genome baby1 = new Genome();
        Genome baby2 = new Genome();
        Crossover(ref mom.vecBits, ref dad.vecBits, ref baby1.vecBits, ref baby2.vecBits);

        // Mutate children
        Mutate(ref baby1.vecBits);
        Mutate(ref baby2.vecBits);

        // Add children to new population
        newPop.Add(baby1); // Genome 1
        newPop.Add(baby2); // Genome 2

        // ELITISM
        // Add fittest Genome
        newPop.Add(fittestGenome); // Genome 3
        vecPopulation.RemoveAt(fittestGenomeIndex);
            
        // Add second fittest Genome
        int secondfittestIndex = BestGenomeFinder(); // Genome 4
        newPop.Add(vecPopulation[secondfittestIndex]);

        // Replace old population with new population
        vecPopulation = newPop;

        // Incrementing generation is done in Start

        // Decode, place information in respective Genome objects
        for (int i = 0; i < popSize; i++) { // Go through all genomes in population
            // Decode, returns [color, speed]
            List<int> decoded = Decode(vecPopulation[i].vecBits); 
            // Send to the current genome what's decoded
            vecPopulation[i].vecDecoded = decoded;
            // Send to current genome it's decoded color
            vecPopulation[i].color = decoded[0]; 
            // Send to current genome it's decoded speed
            vecPopulation[i].speed = decoded[1]; 

            // Instantiate prefab, one ghost added to generatedGhosts
            InstantiateGhostPrefab(vecPopulation[i].vecDecoded);
            prefabsCleared = false; // new prefabs instantiated, so false

            // Send to current genome, gain access to the ghost instance on screen
            vecPopulation[i].prefab = generatedGhosts[i];

            // Set totalDistancePlayer back to 0
            vecPopulation[i].totalDistancePlayer = 0.0;
        }

        UpdateGenomeAge();
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
