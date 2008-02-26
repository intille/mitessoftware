using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections;


namespace AXML
{
    public class Reader
    {
        public const string DEFAULT_XSD_FILE = "..\\..\\NeededFiles\\Master\\ActivityLabelsRealtime.xsd";    
        public const string DEFAULT_XML_FILE = "ActivityLabelsRealtime.xml";

        private string xmlFile;
        private string xsdFile;
        private System.Windows.Forms.Form caller;


        public Reader(string dataDirectory, System.Windows.Forms.Form caller)
        {
            this.xmlFile = dataDirectory + "\\" + DEFAULT_XML_FILE;
            this.xsdFile = DEFAULT_XSD_FILE;
            this.caller = caller;

        }

        public Annotation parse()
        {
            Annotation annotation = new Annotation();
            XmlDocument dom = new XmlDocument();
            dom.Load(this.xmlFile);
            XmlNode xNode=dom.DocumentElement;

            if ( (xNode.Name == Constants.ANNOTATION_ELEMENT) && (xNode.HasChildNodes))
            {
                
                foreach (XmlNode iNode in xNode.ChildNodes)
                {
                    //Console.WriteLine(iNode.Name);

                    //parsing file information
                    if (iNode.Name == Constants.FILE_INFO_ELEMENT)
                    {                        
                        foreach (XmlNode jNode in iNode.ChildNodes)
                        {
                            //Console.WriteLine(jNode.Name);

                            foreach (XmlAttribute jAttribute in jNode.Attributes)
                            {
                                if (jAttribute.Name == Constants.NAME_ATTRIBUTE)
                                {
                                    if (jNode.Name == Constants.SUBJECT_ELEMENT)
                                    {
                                        annotation.FileInfo.Subject = jAttribute.Value;
                                    }
                                    else if (jNode.Name == Constants.OBSERVER_ELEMENT)
                                    {                                        
                                        annotation.FileInfo.Observer = jAttribute.Value;
                                    }
                                    else if (jNode.Name == Constants.LOCATION_ELEMENT)
                                    {
                                        annotation.FileInfo.Location = jAttribute.Value;
                                    }
                                    else if (jNode.Name == Constants.NOTES_ELEMENT)
                                    {
                                        annotation.FileInfo.Notes = jAttribute.Value;
                                    }                                    
                                }
                            }


                          
                        }
                    }
                        //parsing labels
                    else if (iNode.Name == Constants.LABELS_ELEMENT)
                    {

                        //parsing the categories
                        foreach (XmlNode jNode in iNode.ChildNodes)
                        {
                            //double frequency = 1500;
                            if (jNode.Name == Constants.CATEGORY_ELEMENT)
                            {
                                Category category = new Category();

                                //parsing category attributes
                                foreach (XmlAttribute jAttribute in jNode.Attributes)
                                {
                                   if (jAttribute.Name == Constants.NAME_ATTRIBUTE)
                                   {
                                            category.Name=jAttribute.Value;
                                   }else if (jAttribute.Name == Constants.PROPERTY_ATTRIBUTE)
                                   {
                                            category.Property=jAttribute.Value;
                                   }
                                }
          
                                //parsing category labels
                                foreach (XmlNode kNode in jNode.ChildNodes)
                                {
                                    foreach (XmlAttribute kAttribute in kNode.Attributes)
                                    {
                                        if (kAttribute.Name == Constants.NAME_ATTRIBUTE)
                                        {
                                            Label newlabel=new Label(kAttribute.Value, category.Name);
                                            //newlabel.InitializeTone(this.caller.Handle.ToInt32(),frequency);
                                            category.Labels.Add(newlabel);
                                            //frequency += 200;
                                        }
                                    }
                                }

                                annotation.Categories.Add(category);
                            }
                        }

                    }
                }
            }

            return annotation;            
        }

        public bool validate()
        {

            XmlSchemaSet sc = new XmlSchemaSet();
            // Add the schema to the collection.

            sc.Add("urn:mites-schema", this.xsdFile);
            // Set the validation settings.    
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            // Create the XmlReader object.
            XmlReader reader = XmlReader.Create(this.xmlFile, settings);
            // Parse the file. 
            while (reader.Read())
            {
                ;
            }
            return true;


        }

        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            throw new Exception(e.Message);
        }
    }


}
