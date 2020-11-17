using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public partial class ClassMaterial
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public string ClassShortName { get; set; }
        public string Link { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
