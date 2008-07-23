using System;
using System.Collections.Generic;
using System.Text;
using MITesFeatures;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //Generates an Arff file from a PlaceLab format file
           // Extractor.toARFF(@"..\NeededFiles\SamplePLFormat",
             //   "..\\NeededFiles\\Master\\", 3);

            Extractor.toARFF(@"C:\MITesData\SamplePLFormat",
                "..\\NeededFiles\\Master\\", 3);
        }
    }
}
