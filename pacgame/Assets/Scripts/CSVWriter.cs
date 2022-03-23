using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVWriter
{
    public string filename = "";
    public List<Genome> genomeList;
    
    public TextWriter tw;

    public CSVWriter() {
        // Initialize table
        filename = Application.dataPath + "/data.csv";
        tw = new StreamWriter(filename, false);
        tw.WriteLine("Generation, Color, Speed, Fit, ScaledFit, Age");
        tw.Close();
    }

    public void WriteData(List<Genome> genomeList, int generation) {
        tw = new StreamWriter(filename, true);
        int gen = generation - 1;
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
