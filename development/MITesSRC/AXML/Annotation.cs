using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;


namespace AXML
{
    public class Annotation
    {

        public const string DEFAULT_OUTPUT_FILE = "AnnotationIntervals";
        public const string DEFAULT_OUTPUT_CSV_FILE = "AnnotationIntervals";
        private string output_file;
        private string output_csv_file;
        private FileInformation fileInfo;
        private ArrayList categories;
        private ArrayList data;
        private string dataDirectory;
        private string fileSuffix;

        public Annotation()
        {

            this.fileInfo = new FileInformation();
            this.categories = new ArrayList();
            this.data = new ArrayList();
            fileSuffix = DateTime.Now.ToString("-yyyy-MM-dd-HH-mm-ss");
            this.output_file = DEFAULT_OUTPUT_FILE + ".xml";
            this.output_csv_file = DEFAULT_OUTPUT_CSV_FILE + ".csv";
        }

        public Annotation(string output_file,string output_csv_file){
               
            this.fileInfo=new FileInformation();
            this.categories = new ArrayList();
            this.data = new ArrayList();
            this.output_file = output_file;
            this.output_csv_file = output_csv_file;
        }

        public string DataDirectory
        {
            get
            {
                return this.dataDirectory;
            }
            set
            {
                this.dataDirectory = value;
            }
        }
        public string OutputFile
        {
            get
            {
                return this.output_file;
            }
        }

        public string OutputCSVFile
        {
            get
            {
                return this.output_csv_file;
            }
        }
        public FileInformation FileInfo
        {
            get
            {
                return this.fileInfo;
            }
        }

        public ArrayList Categories
        {
            get
            {
                return this.categories;
            }
        }

        public ArrayList Data
        {
            get
            {
                return this.data;
            }
        }

        public string ToXML()
        {
            string xml = "";

            xml += "<?xml version='1.0'?>\n";
            xml += "<" + Constants.ANNOTATION_ELEMENT + " xmlns=\"urn:mites-schema\">\n";
            xml += this.fileInfo.ToXML();
            xml += "<" + Constants.LABELS_ELEMENT + ">\n";
            foreach (Category category in this.categories)
            {
                xml += category.ToXML();
            }
            xml += "</" + Constants.LABELS_ELEMENT + ">\n";
            xml += "<" + Constants.DATA_ELEMENT + ">\n";
            foreach (AnnotatedRecord annotatedRecord in this.data)
            {
                xml += annotatedRecord.ToXML();
            }
            xml += "</" + Constants.DATA_ELEMENT + ">\n";
            xml += "</"+Constants.ANNOTATION_ELEMENT+">\n";
            return xml;
        }

        public void ToXMLFile()
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter(this.dataDirectory+"\\"+this.output_file);

            // write a line of text to the file
            tw.WriteLine(ToXML());

            // close the stream
            tw.Close();
        }

        public void ToCSVFile()
        {
            string csv = "";

            csv += Constants.STARTTIME_ATTRIBUTE + "," + Constants.ENDTIME_ATTRIBUTE + "," + Constants.COLOR_ATTRIBUTE;
            foreach (Category category in this.categories)
            {
                csv += "," + category.Name;
            }
            csv += "\n";
            foreach (AnnotatedRecord annotatedRecord in this.data)
            {
                csv += annotatedRecord.ToCSV()+"\n";
            }

            // create a writer and open the file
            TextWriter tw = new StreamWriter(this.dataDirectory + "\\" + this.output_csv_file);

            // write a line of text to the file
            tw.WriteLine(csv);

            // close the stream
            tw.Close();
        }
    }
}
