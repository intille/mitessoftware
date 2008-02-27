using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SimpleLogger
{

    public class Logger
    {
        public const string LOGGER_DIRECTORY = "..\\..\\..\\..\\MITesSRC\\AllSensors\\NeededFiles\\log\\";
        
        public enum Priority : int
        {
            OFF = 200,
            DEBUG = 100,
            INFO = 75,
            WARN = 50,
            ERROR = 25,
            FATAL = 0
        }

        private StreamWriter sw;
        private string logDirectory;
        private Priority logLevel;

        public void Debug(string message)
        {
            Append(message, Priority.DEBUG);
        }

        public void Info(string message)
        {
            Append(message, Priority.INFO);
        }

        public void Warn(string message)
        {
            Append(message, Priority.WARN);
        }

        public void Error(string message)
        {
            Append(message, Priority.ERROR);
        }

        public void Fatal(string message)
        {
            Append(message, Priority.FATAL);
        }

        // constructor for static resources
        public Logger(string logDirectory, Priority logLevel)
        {


            this.logDirectory = logDirectory;
            this.logLevel = logLevel;

            // if the file doesn't exist, create it
            if (!File.Exists(logDirectory + "\\log-" + DateTime.Now.ToString("yyyy-MM-dd")+".txt"))
            {
                FileStream fs = File.Create(logDirectory + "\\log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                fs.Close();
            }

            // open up the streamwriter for writing..
            sw = File.AppendText(logDirectory + "\\log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
        }

        public void Dispose()
        {
            sw.Close();
        }


        public void Append(string message, Priority level)
        {            
            if ((int) this.logLevel >= (int)level)
            {

                try
                {
                    lock (sw)
                    {
                        sw.Write("\r\nLog Entry : ");
                        sw.WriteLine("{0} : ", DateTime.Now.ToString("hh:mm:ss"));
                        sw.WriteLine("  :");
                        sw.WriteLine("  :{0}", message);
                        sw.WriteLine("-------------------------------");
                        sw.Flush();
                    }
                }
                catch
                {
                    // do nothing
                }
            }
        }
    }
}


 

