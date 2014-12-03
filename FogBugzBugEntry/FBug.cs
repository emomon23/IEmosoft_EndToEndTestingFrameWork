using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEmosoft.FogBugzBugEntry
{
    public class FBug
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string ImageFilePath { get; set; }
        public string Project { get; set; }
        public string Category { get; set; }
    }
}
