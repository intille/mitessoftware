using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections;

namespace SOXML
{
    public class Reader
    {

        public const string DEFAULT_XML_FILE = "SensorCalibration.xml";
        private string xmlFile;

        public Reader(string dataDirectory)
        {
            this.xmlFile = dataDirectory + "\\" + DEFAULT_XML_FILE;
        }

        public Hashtable parse()
        {
            Hashtable calibrations = new Hashtable();
            XmlDocument dom = new XmlDocument();
            dom.Load(this.xmlFile);
            XmlNode xNode = dom.DocumentElement;
            int acceleration_index = 0;

            if ((xNode.Name == Constants.SENSORS_ELEMENT) && (xNode.HasChildNodes))
            {
                //Sensor nodes
                foreach (XmlNode iNode in xNode.ChildNodes)
                {



                    //parsing file information
                    if (iNode.Name == Constants.SENSOR_ELEMENT)
                    {
                        SensorCalibration calibration = new SensorCalibration();

                        foreach (XmlAttribute iAttribute in iNode.Attributes)
                        {
                            //read nodes attributes
                            if (iAttribute.Name == Constants.ID_ATTRIBUTE)
                            {
                                calibration.ID = Convert.ToInt32(iAttribute.Value);
                            }
                        }

                        //Orientations
                        foreach (XmlNode jNode in iNode.ChildNodes)
                        {
                            SensorOrientation orientation = new SensorOrientation();

                            foreach (XmlAttribute jAttribute in jNode.Attributes)
                            {
                                //read nodes attributes
                                if ((jNode.Name == Constants.ORIENTATION_ELEMENT) && (jAttribute.Name == Constants.ID_ATTRIBUTE))
                                {
                                    orientation.ID = Convert.ToInt32(jAttribute.Value);
                                }
                                else if ((jNode.Name == Constants.ORIENTATION_ELEMENT) && (jAttribute.Name == Constants.DESCRIPTION_ATTRIBUTE))
                                {
                                    orientation.Description = jAttribute.Value;
                                }
                            }

                            foreach (XmlNode kNode in jNode.ChildNodes)
                            {
                                if (kNode.Name == Constants.ACCELERATIONS_ELEMENT)
                                {

                                    acceleration_index = 0;
                                    foreach (XmlNode rNode in kNode.ChildNodes)
                                    {
                                        SensorAcceleration acceleration = new SensorAcceleration();

                                        foreach (XmlAttribute rAttribute in rNode.Attributes)
                                        {
                                            if ((rNode.Name == Constants.ACCELERATION_ELEMENT) && (rAttribute.Name == Constants.MINX_ATTRIBUTE))
                                            {
                                                acceleration.MinX = Convert.ToInt32(rAttribute.Value);
                                            }
                                            else if ((rNode.Name == Constants.ACCELERATION_ELEMENT) && (rAttribute.Name == Constants.MAXX_ATTRIBUTE))
                                            {
                                                acceleration.MaxX = Convert.ToInt32(rAttribute.Value);
                                            }
                                            else if ((rNode.Name == Constants.ACCELERATION_ELEMENT) && (rAttribute.Name == Constants.MINY_ATTRIBUTE))
                                            {
                                                acceleration.MinY = Convert.ToInt32(rAttribute.Value);
                                            }
                                            else if ((rNode.Name == Constants.ACCELERATION_ELEMENT) && (rAttribute.Name == Constants.MAXY_ATTRIBUTE))
                                            {
                                                acceleration.MaxY = Convert.ToInt32(rAttribute.Value);
                                            }
                                            else if ((rNode.Name == Constants.ACCELERATION_ELEMENT) && (rAttribute.Name == Constants.MINZ_ATTRIBUTE))
                                            {
                                                acceleration.MinZ = Convert.ToInt32(rAttribute.Value);
                                            }
                                            else if ((rNode.Name == Constants.ACCELERATION_ELEMENT) && (rAttribute.Name == Constants.MAXZ_ATTRIBUTE))
                                            {
                                                acceleration.MaxZ = Convert.ToInt32(rAttribute.Value);
                                            }
                                            else if ((rNode.Name == Constants.ACCELERATION_ELEMENT) && (rAttribute.Name == Constants.DIRECTION_ATTRIBUTE))
                                            {
                                                acceleration.Direction = rAttribute.Value;
                                            }

                                        }

                                        orientation.Accelerations[acceleration_index] = acceleration;
                                        acceleration_index++;
                                    }
                                }
                                else
                                {
                                    foreach (XmlAttribute kAttribute in kNode.Attributes)
                                    {

                                        if ((kNode.Name == Constants.IMAGE_ELEMENT) && (kAttribute.Name == Constants.FILE_ATTRIBUTE))
                                        {
                                            orientation.ImageFile = kAttribute.Value;
                                        }
                                        else if ((kNode.Name == Constants.AVERAGE_ELEMENT) && (kAttribute.Name == Constants.X_ATTRIBUTE))
                                        {
                                            orientation.X = Convert.ToDouble(kAttribute.Value);
                                        }
                                        else if ((kNode.Name == Constants.AVERAGE_ELEMENT) && (kAttribute.Name == Constants.Y_ATTRIBUTE))
                                        {
                                            orientation.Y = Convert.ToDouble(kAttribute.Value);
                                        }
                                        else if ((kNode.Name == Constants.AVERAGE_ELEMENT) && (kAttribute.Name == Constants.Z_ATTRIBUTE))
                                        {
                                            orientation.Z = Convert.ToDouble(kAttribute.Value);
                                        }

                                        else
                                        {

                                        }

                                    }
                                }
                            }

                            calibration.Orientations[orientation.ID] = orientation;
                        }

                        calibrations.Add(calibration.ID, calibration); 
                    }
                }
            }

            return calibrations;
        }
    }
}
