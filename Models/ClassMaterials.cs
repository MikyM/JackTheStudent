using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public partial class ClassMaterials
    {
        public int IdMaterial { get; set; }
        public string Class { get; set; }
        public string Materials { get; set; }
        public string LogBy { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
