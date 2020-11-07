using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public partial class Exams
    {
        public int IdExam { get; set; }
        public string Class { get; set; }
        public DateTime Date { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
        public string Materials { get; set; }
    }
}
