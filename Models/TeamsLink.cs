using System;
using System.Collections.Generic;
using System.Linq;

namespace JackTheStudent.Models
{
    public partial class TeamsLink
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public string ClassShortName { get; set; }
        public string Date { get; set; }
        public string Link { get; set; }
        public string ClassType { get; set; }
        public string GroupId { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }

        public string GetFullClassType() 
        {
            return JackTheStudent.Program.classTypeList.Where(c => c.ShortName == this.Class).Select(c => c.Name).FirstOrDefault();
        }
        public string GetFullClassName() 
        {
            return JackTheStudent.Program.classList.Where(c => c.ShortName == this.ClassType).Select(c => c.Name).FirstOrDefault();
        }
    }
}
