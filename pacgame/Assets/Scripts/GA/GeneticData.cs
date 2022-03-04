using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticData : MonoBehaviour
{
    // Class retains data across each playthrough
    // Static variables 

    public static List<Genome> vecPopulation = new List<Genome>();
    public static int generation = 0; // current generation

    void Start() {
        Debug.Log(generation);
    }

    // GET METHODS
    public int GetGeneration() { return generation; }

    public List<Genome> GetVecPopulation() { return vecPopulation; }

    // SET METHODS
    public void SetGeneration(int gen) { generation = gen; }

    public void SetVecPopulation(List<Genome> pop) { vecPopulation = pop; }
}
