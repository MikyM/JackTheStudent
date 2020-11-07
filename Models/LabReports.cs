using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public partial class LabReports
    {
        public int IdLabReport { get; set; }
        public string Class { get; set; }
        public string LogBy { get; set; }
        public string AdditionalInfo { get; set; }
        public string Materials { get; set; }
    }
}
