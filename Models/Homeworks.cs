using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public partial class Homeworks
    {
        public int IdHomework { get; set; }
        public string Class { get; set; }
        public string LogBy { get; set; }
        public string AdditionalInfo { get; set; }
        public string Materials { get; set; }
    }
}
