using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excel=Microsoft.Office.Interop.Excel; 

// Data Exporter takes data gained from GA playthroughs, saves them in a .csv file. 
public class DataExporter : MonoBehaviour
{
    class StartExcel {
        string path = ""; 
        Excel.Workbook book = null; 
        Excel.Application app = new Excel.Application();
        Excel.Worksheet sheet = null;

        public StartExcel(string path, int sheet) {
            this.path = path;
            this.app.Visible = false;
            this.book = Excel.Workbooks.Open(path);
            this.sheet = book.Worksheets[sheet];

        }

        public FormatTable() {
            // add the columns and row names here 
        }

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
