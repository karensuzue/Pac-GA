using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticData : MonoBehaviour
{
    // Class retains data across each playthrough
    // Static variables 

    public static List<Genome> vecPopulation = new List<Genome>();
    public static int generation = 0; // current generation
    public static int intervalCount = 0;
    public static double shortestPlayTime = 100; 
    public static CSVWriter csv;       

    void Start() {
        // Debug.Log(generation);
    }

    // GET METHODS
    public int GetGeneration() { return generation; }

    public List<Genome> GetVecPopulation() { return vecPopulation; }

    public double GetShortestPlayTime() { return shortestPlayTime; }

    public int GetIntervalCount() { return intervalCount; }
    public CSVWriter GetCSVWriter() { return csv; }

    // SET METHODS
    public void SetGeneration(int gen) { generation = gen; }

    public void SetVecPopulation(List<Genome> pop) { vecPopulation = pop; }

    public void SetShortestPlayTime(double time) { shortestPlayTime = time; } 

    public void SetIntervalCount(int count) { intervalCount = count; }

    public void SetCSVWriter(CSVWriter c) { csv = c; }
}
