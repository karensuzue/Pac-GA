using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class retains data across each playthrough. It does this through static variables.
 * Necessary due to loss of data once a Scene is unloaded.
 * Our genetic algorithm relies on data being transferred across such Scenes 
 *     to make appropriate adjustments.
*/
public class GeneticData : MonoBehaviour
{
    public static List<Genome> vecPopulation = new List<Genome>(); // The current population 
    public static int generation = 0; // The current generation
    public static int intervalCount = 0; // Helper variable for GeneticController to keep track of 1-second intervals
    public static double shortestPlayTime = 3000; // Shortest play time of a single run
    public static CSVWriter csv; // Helper variable to retain CSVWriter object 
        // through each Scene reload and avoid previous data being written over


    /**
     * GET METHODS
     * Can be called by other classes to obtain information.
    */
    public int GetGeneration() { return generation; } 
    public List<Genome> GetVecPopulation() { return vecPopulation; }
    public double GetShortestPlayTime() { return shortestPlayTime; }
    public int GetIntervalCount() { return intervalCount; }
    public CSVWriter GetCSVWriter() { return csv; }


    /** 
     * SET METHODS
     * Can be called by other classes to re-save information.
    */
    public void SetGeneration(int gen) { generation = gen; }
    public void SetVecPopulation(List<Genome> pop) { vecPopulation = pop; }
    public void SetShortestPlayTime(double time) { shortestPlayTime = time; } 
    public void SetIntervalCount(int count) { intervalCount = count; }
    public void SetCSVWriter(CSVWriter c) { csv = c; }
}
