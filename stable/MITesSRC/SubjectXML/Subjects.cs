using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace SubjectXML
{
    public class Subjects
    {
        private Subject subject;
        private string dataDirectory;


        public const string DEFAULT_SUBJECT_FILE = "SubjectData.xml";
        

        public Subjects(string dataDirectory)
        {
            this.subject = new Subject();
            this.dataDirectory = dataDirectory;
           

        }

        public Subject FirstSubject
        {
            get
            {
                return this.subject;
            }
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

        public string ToXML()
        {
            string xml = "";

            xml += "<?xml version='1.0'?>\n";
            xml = "<" + Constants.SUBJECTDATA_ELEMENT + ">\n";
            xml += this.subject.ToXML();
            xml += "</" + Constants.SUBJECTDATA_ELEMENT + ">\n";
            return xml;
        }

        public void ToXMLFile()
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter(this.dataDirectory + "\\" + DEFAULT_SUBJECT_FILE);

            // write a line of text to the file
            tw.WriteLine(ToXML());

            // close the stream
            tw.Close();
        }
    }
}
