using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/**
 * CSVWriter class writes data to .csv file.  
*/
public class CSVWriter
{
    public string filename = "";
    public TextWriter tw;

    /**
     * Constructor, initialize table
    */
    public CSVWriter() {
        filename = Application.dataPath + "/data.csv";
        tw = new StreamWriter(filename, false); // NOTE: APPEND SET TO FALSE. 
            // Even though all CSVWriter objects write to the same path, append=false will lead to
            // data being written over. It is thus necessary to use the same CSVWriter object 
            // throughout a training session. 
        tw.WriteLine("Generation, Color, Speed, Fit, ScaledFit, Age"); // Headers for table columns
        tw.Close();
    }

    /**
     * Appends data to .csv file
     * @param genomeList Current population 
     * @param generation Current generation
    */
    public void WriteData(List<Genome> genomeList, int generation) {
        tw = new StreamWriter(filename, true); // NOTE: APPEND SET TO TRUE 
        int gen = generation - 1;

        // Writes additional data to file
        for (int i = 0; i < genomeList.Count; i++) {
            Genome current = genomeList[i];
            int age = current.age - 1;
            tw.WriteLine(gen.ToString() + "," + current.color.ToString() + "," + 
            current.speed.ToString() + "," + current.fitScoreOld.ToString() + "," + 
            current.fitScore.ToString() + "," + age.ToString());
        }

        tw.Close();
    }        
}
