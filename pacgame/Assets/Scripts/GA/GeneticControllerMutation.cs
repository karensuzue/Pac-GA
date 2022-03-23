// Genetic Controller with Dynamic Mutation Rate. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// SAVED = Made static variable in GeneticData.cs, retain information after reloading scene
public class GeneticControllerMutation : MonoBehaviour
{
    // Population:
    private List<Genome> vecPopulation = new List<Genome>(); // SAVE TO GENETICDATA
    private int popSize = 4; // population size
    
    // Chromosome Representation:
    // 0 0 | 0 0 0
    // color | speed
    private int chromLength = 5; // number of bits per chromosome (vecBits size)
    private int speedChromLength = 3; // number of bits per speed chromosome
    private int colorChromLength = 2; // number of bits per color(type) chromosome
    private int speedIndexStart = 2; // speed starts at index 2 of the Genome bit list
    
    // Mating Rates:
    private float mutationRate = 0.5f;
    private float crossoverRate = 0.7f;

    // Fitness:
    private double bestFitness; // best fitness score
    private double worstFitness; 
    private double totalFitness; // total fitness score of the current population
    private double averageFitness;
    private Genome fittestGenome; // fittest genome of the population
    private Genome worstGenome; 
    private int fittestGenomeIndex; // index to access the fittest genome in the population
    private int worstGenomeIndex; 
    
    // Game and Prefabs:
    private int generation; // current generation, increases each time Start() is run
    private bool gameRunning; // true if game is currently running
    private List<GameObject> generatedGhosts = new List<GameObject>(); // ghosts instantiated on screen
    bool prefabsCleared; // true if prefabs are all cleared

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

    // GameManager:
    public GameObject gameManagerObj; // drag and drop in Inspector
    public GameManager gameManager;
    // public float timeComplete; // 1 playthrough completion time

    // GeneticData, where things get saved:
    public GameObject geneticDataObj; // Drop in Inspector
    public GeneticData geneticData;

    // Timer-related:
    private static System.Random random = new System.Random(); // helps generate random numbers
    private double nextUpdate = 0.0; // update "interval" in seconds, used for UpdateEverySecond
    public int intervalCount = 0; // number of 1-second intervals, for calculating average. 
            // Similar to nextUpdate but stops updating once playthrough ends. 
    
    // The player!
    public GameObject player;
    // public bool fitnessCalculated;

    // Export data to .csv file
    CSVWriter csv;

    void Awake()
    {
        // initialization goes here
        gameManager = gameManagerObj.GetComponent<GameManager>();
        geneticData = geneticDataObj.GetComponent<GeneticData>();
        gameRunning = false;

        // get "saves" from GeneticData
        generation = geneticData.GetGeneration();
        vecPopulation = geneticData.GetVecPopulation();
        intervalCount = geneticData.GetIntervalCount();

        player = GameObject.FindGameObjectWithTag("Player");

        if (generation < 1) {
            csv = new CSVWriter();
            geneticData.SetCSVWriter(csv);
        }
        else {
            csv = geneticData.GetCSVWriter();
        }
         // once instantiated in geneticData, doesn't need to be resaved
    }

    void Start() {
        gameRunning = true;
        // fitnessCalculated = false;

        generation += 1;
        geneticData.SetGeneration(generation); // send to GeneticData

        // If this is the first generation, initialize the population
        if (generation == 1) { 
            CreateStartingPopulation();

            for (int i = 0; i < popSize; i++) { // Go through all genomes in population
                List<int> decoded = Decode(vecPopulation[i].vecBits); // Decode, returns [color, speed]
                vecPopulation[i].vecDecoded = decoded; // Send to the current genome what's decoded
                vecPopulation[i].color = decoded[0]; // Send to current genome it's decoded color
                vecPopulation[i].speed = decoded[1]; // Send to current genome it's decoded speed

                InstantiateGhostPrefab(vecPopulation[i].vecDecoded); // Instantiate prefab, one ghost added to generatedGhosts
                // Send to current genome, gain access to the ghost object on screen
                vecPopulation[i].prefab = generatedGhosts[i]; // WORKS
                    
            }
            UpdateGenomeAge();
        }
        // all genomes +1 in age, age defined by number of generations survived
        // in first generation: age gets updated after initializing the population
        // in the following generations: age gets updated before recalculating fitness
          // age starts at 1

        // If this is not the first generation, run Epoch
        if (generation > 1) {
            Epoch();
        }

        // fitnessCalculated == true; // true
        intervalCount = 0; // reset intervalCount. Has to be done after fitness is calculated to prevent NaN
        // fitnessCalculated = false;

        for (int i = 0; i < vecPopulation.Count; i++) {
            // FOR DEBUG: Set enemy controller fitness = genome fitness to observe
            EnemyController2 enemyCtrl = vecPopulation[i].prefab.GetComponent<EnemyController2>();
            enemyCtrl.fitness = vecPopulation[i].fitScore;
            Debug.Log("enemyctrl fitness");
            Debug.Log(enemyCtrl.fitness);
        }


        geneticData.SetVecPopulation(vecPopulation); // any changes to vecPopulation has to be resaved to GeneticData   
        
        prefabsCleared = false; // switch to keep Update from constantly trying to clear prefabs (even when there's none)
    }

   
    async void Update()
    {
        // Loop that runs in fixed 1 second interval, used for updating average distance from player in one playthrough
        if (Time.time >= nextUpdate) {
            nextUpdate = Mathf.FloorToInt(Time.time)+1;
            if (gameRunning == true) {
                intervalCount += 1; // WORKS 
            }
            UpdateEverySecond();
        }

        for (int i = 0; i < vecPopulation.Count; i++) {
            // the ghost that collides with the player first will have its EnemyController's playerCaught set to true
            // simply find that ghost, then set playerCollide to true for the Genome object
            // it is possible for multiple ghosts to have this boolean value checked if they overlapped
            GameObject currentPrefab = vecPopulation[i].prefab;
            EnemyController2 enemyController = currentPrefab.GetComponent<EnemyController2>();
            if (enemyController.playerCaught == true) {
                vecPopulation[i].playerCollide = true;
            }
        }

        GameManager.GameStates state = gameManager.state; // current game state
        if (state == GameManager.GameStates.win || state == GameManager.GameStates.gameOver) {
            // if game won or over
            gameRunning = false; // game no longer running, paused
            geneticData.SetIntervalCount(intervalCount);
        }

        // Debug.Log("OLD TIME SCORE");
        // Debug.Log(geneticData.GetShortestPlayTime());
        if (state == GameManager.GameStates.win) { // PROBABLY WORKS
            // update shortest play time highscore
            if (gameManager.endTime < geneticData.GetShortestPlayTime()) { 
                // if the end time of this playthrough is shorter than the previous highscore, update
                geneticData.SetShortestPlayTime(gameManager.endTime);
            }
        }

        // Debug.Log("NEW TIME SCORE");
        // Debug.Log(geneticData.GetShortestPlayTime());
    }

    private void UpdateEverySecond() { // WORKS
        // UpdateEverySecond is called once per second 
        if (gameRunning == true) {
            // continue adding distance until playthrough ends 
            for (int i = 0; i < vecPopulation.Count; i++) {
                GameObject currentPrefab = vecPopulation[i].prefab;
                double distance = Vector2.Distance(currentPrefab.transform.position, player.transform.position);
                vecPopulation[i].totalDistancePlayer += distance;
            }

            // Debug.Log("TOTAL DISTANCE DEBUG");
            // Debug.Log(vecPopulation[1].totalDistancePlayer);
        }
    }
    
    // PRIVATE METHODS
    private void CreateStartingPopulation() { // WORKS
        vecPopulation = new List<Genome>();
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

        return decodedGenome; // returns [int color, int speed]
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
            generatedGhosts.RemoveAt(i); // has to be removed, empty for next generation
            vecPopulation[i].prefab = null;
            Destroy(current);
        }
    }

    /*
    private async void DestroyGhostPrefabs2() { // for debug purposes, WORKS
        for (int i = 0; i < vecPopulation.Count; i++) {
            GameObject current = vecPopulation[i].prefab;
            vecPopulation[i].prefab = null;
            Destroy(current);
        }
    }
    */

    private double FitnessFunction(Genome gen) { // WORKS
        // updates fitness score for one individual 
        double fitness = 0;
        double average = gen.totalDistancePlayer / intervalCount; // average distance away from player per second
        
        if (gen.speed == 0) { // if ghost stays still, this prevents player from completing the game
            fitness -= 30; // remove this completely
        }

        if (gen.playerCollide == true) { // if ghost collides with player
            fitness += 20; // encourages more aggression
            if (gameManager.endTime < 5.5) { // if this ghost kills player too quickly, deduct
                fitness -= 30; // encourages more passivity

            }
        }

        if (gameManager.state == GameManager.GameStates.win) {
            if (gameManager.endTime < geneticData.GetShortestPlayTime()) { // if player has a new time high score, deduct points for all ghosts
                fitness -= 5; // more aggression
            }
        }
        
        // maximum distance from player is about 40.361 (two opposite diagonal corners of the map)
        // maximum fitness deducted is about 5.361
        if (average > 20) { // if average distance from playe is greater than threshold
            fitness -= (average - 20); // the further the distance, the greater the deduction
            // fitness -= Math.Pow(average - 35, 2); // aggressively encourages ghost to move towards player more
        }

        return fitness;
    }

    private void CalculatePopulationFitness() { // WORKS
        // assigns fitness score to all members of the population
        for (int i = 0; i < vecPopulation.Count; i++) {
            double fitness = FitnessFunction(vecPopulation[i]);
            vecPopulation[i].fitScore = fitness;
            vecPopulation[i].fitScoreOld = fitness;
        }
    }

    private void ScaleFitnessScores() {
        // scale to prevent quick convergence and ensure population diversity 
        // Sigma scaling method
        // directly changes the fitness scores of each member of the population

        // calculate the standard deviation
        double runningTotal = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            runningTotal += Math.Pow((vecPopulation[i].fitScore - averageFitness), 2);
        }
        Debug.Log("runningtotal");
        Debug.Log(runningTotal);
        double variance = runningTotal / popSize;
        double standev = Math.Sqrt(variance);

        Debug.Log("standev");
        Debug.Log(standev);

        // loop through population, reassign fitness scores
        for (int i = 0; i < vecPopulation.Count; i++) {
            double oldFitness = vecPopulation[i].fitScore;
            // vecPopulation[i].fitScoreOld = oldFitness; // for CSVWriter
            vecPopulation[i].fitScore = (oldFitness - averageFitness) / (2 * standev);
        }

        // recalculate best, worst, average, total fitness scores
        CalcBestWorstAvTotFitness();
    }

    private void CalcBestWorstAvTotFitness() {
        // use to calculate or recalculate the best/worst/average/total fitness values
        fittestGenomeIndex = BestGenomeFinder(); // index of the fittest genome
        fittestGenome = vecPopulation[fittestGenomeIndex]; // fittest Genome object
        bestFitness = fittestGenome.fitScore; // fitness score of the fittest genome

        worstGenomeIndex = WorstGenomeFinder(); // index of the worst genome
        worstGenome = vecPopulation[worstGenomeIndex]; // worst fitness Genome object
        worstFitness = worstGenome.fitScore; // fitness score of the worst genome

        // calculate total fitness
        double total = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            total += vecPopulation[i].fitScore;
        }
        totalFitness = total;

        // calculate average fitness
        averageFitness = total / vecPopulation.Count;
    }

    // choose one good genome to bring over?
    // check crossover on all genomes in population? or use selection functions to choose out two genomes for this purpose
    // 
    private Genome RouletteWheelSelection() {
        // Doesnt guarantee the best is selected
        // Those with higher fitness have a higher chance
        // System.Random rand = new System.Random();

        // random number between 0 and totalFitness
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

    /*
    private Genome SUSSelection() {
        // Stochastic Universal Sampling
        // better for small population
        // relies on fitness being nonnegative
        // sigma scaling can give negative fitness score
        // but doesnt work well if elitism is employed, for this reason discarded
        int selectedGenomeIndex = 0;
        double sum = 0;

        int numToAdd
        return vecPopulation[selectedGenomeIndex];
    }*/

    private int BestGenomeFinder() {
        // 1 best genome goes to the next generation
        // Used for elitism, good with Roulette Wheel Selection
        // returns the index of the best genome
        int bestGenomeIndex = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            if (vecPopulation[i].fitScore > vecPopulation[bestGenomeIndex].fitScore) {
                bestGenomeIndex = i;
            }
        }
        return bestGenomeIndex;
    }

    private int WorstGenomeFinder() {
        // returns the index of the worst genome
        int worstIndex = 0;
        for (int i = 0; i < vecPopulation.Count; i++) {
            if (vecPopulation[i].fitScore < vecPopulation[worstIndex].fitScore) {
                if (vecPopulation[i].age > 1) { 
                    // gives chance for diversity/innovation, only select as worst if age is > 1
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
            Debug.Log("age");
            Debug.Log(vecPopulation[i].age);
        }
    }

    private void Mutate(ref List<int> genomeBits) {
        for (int i = 0; i < chromLength; i++) {
            // flip?
            if ((float)random.NextDouble() < mutationRate) {
                if (genomeBits[i] == 0) { genomeBits[i] = 1; }
                if (genomeBits[i] == 1) { genomeBits[i] = 0; }
            }
        }
    }

    private void Crossover(ref List<int> mom, ref List<int> dad, ref List<int> child1, ref List<int> child2) { // in this case only one child is produced instead of 2 to ensure that there is always four ghosts in the game
        if (((float)random.NextDouble() > crossoverRate) || (mom == dad)) { // float uses less memory, double not needed in this case?
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


    // PUBLIC METHODS 
    public void Epoch() {
        // if (gameRunning == false) { // if game is at "Play Again?" screen
            // Update population fitness
            CalculatePopulationFitness(); 
            // Update best, worst, total, and average fitness scores
            CalcBestWorstAvTotFitness(); 
            // Scale fitness scores, also update BWTA scores in the process
            // Updated fitness will be used for selection
            ScaleFitnessScores(); 

            // fitnessCalculated = true; 
            csv.WriteData(vecPopulation, generation);

            // Destroy prefabs on screen
            if (generatedGhosts.Count > 0 && prefabsCleared == false) {
                DestroyGhostPrefabs(); // destroy after ghost collision with player is checked
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
            vecPopulation.RemoveAt(fittestGenomeIndex); // can now remove as RWS needs the original, unmodified population
            
            // Add second fittest Genome
            int secondfittestIndex = BestGenomeFinder(); // Genome 4
            newPop.Add(vecPopulation[secondfittestIndex]);
            

            // Probabilistically select 2 members to add to the next generation based on their fitness
            // Probability (1-r) * p, r being the fraction of the population to be replaced by
            // Crossover (0.5) and p being the number of hypotheses to be included in the population.
            // Genome selectedGenome1 = RouletteWheelSelection();
            // newPop.Add(selectedGenome1);
            // vecPopulation.Remove(selectedGenome1);

            // Genome selectedGenome2 = RouletteWheelSelection();
            // newPop.Add(selectedGenome2);
            // vecPopulation.Remove(selectedGenome2);

            // For now age is not considered (implemented when picking out worst genome)

            // Replace old population with new population
            vecPopulation = newPop;

            // Incrementing generation is done in Start

            // Decode, place information in respective Genome objects
            for (int i = 0; i < popSize; i++) { // Go through all genomes in population
                List<int> decoded = Decode(vecPopulation[i].vecBits); // Decode, returns [color, speed]
                vecPopulation[i].vecDecoded = decoded; // Send to the current genome what's decoded
                vecPopulation[i].color = decoded[0]; // Send to current genome it's decoded color
                vecPopulation[i].speed = decoded[1]; // Send to current genome it's decoded speed

                // Instantiate prefab, one ghost added to generatedGhosts
                InstantiateGhostPrefab(vecPopulation[i].vecDecoded);
                prefabsCleared = false; // new prefabs instantiated, so false

                // Send to current genome, gain access to the ghost object on screen
                vecPopulation[i].prefab = generatedGhosts[i];
            }

            UpdateGenomeAge();
        // }
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
