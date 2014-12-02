using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JiraBugEntry
{
    public class JiraInstance
    {
        public JiraInstance()
        {
            string fileName = @"C:\Projects\Data.dat";
            string tagName = "LokiJira";

            string[] filelines = File.ReadAllLines(fileName);
            bool gotIt = false;

            foreach (string line in filelines)
            {
                if (line.Contains(tagName))
                {
                    gotIt = true;
                    string [] values = line.Split(',');

                    this.Username = values[1];
                    this.Password = values[2];
                    this.Url = values[3];
                    break;
                }
            }

            if (!gotIt)
            {
                throw new Exception(string.Format("Unable to '{0}' in file {1}", tagName, fileName));
            }
        }

        public string Url { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
