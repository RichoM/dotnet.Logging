using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Logging
{
    public class Logger
    {
        // INFO(Richo): The default logger writes only to the Console
        private static Logger current = new Logger(null);
        public static Logger Current
        {
            get { return current; }
            set { current = value; }
        }

        public static void Log(object o)
        {
            Log("{0}", o);
        }

        public static void Log(string format, params object[] args)
        {
            if (Current == null)
            {
                Console.WriteLine("Logger instance not configured");
            }
            else
            {
                Current.Write(format, args);
            }
        }

        private string path;
        private object locker = new object();

        public Logger(string path)
        {
            this.path = path;
        }

        public void Write(string format, params object[] args)
        {
            try
            {
                /*
                INFO(Richo): string.Format prints empty string for null args, 
                but I want null objects to be logged as "null".
                */
                object[] notNullArgs = args.Select((each) => each == null ? "null" : each).ToArray();
                lock (locker)
                {
                    if (path != null)
                    {
                        ValidateDirectoryPath();
                        using (StreamWriter writer = File.AppendText(path))
                        {
                            writer.WriteLine();
                            writer.Write(DateTime.Now);
                            writer.Write(": ");
                            writer.WriteLine(string.Format(format, notNullArgs));
                        }
                    }
                    Console.Write(DateTime.Now);
                    Console.Write(": ");
                    Console.WriteLine(format, notNullArgs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ValidateDirectoryPath()
        {
            string dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
    }
}
