using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public partial class DickAppointment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Length { get; set; }
        public string Circumference { get; set; }
        public string Width { get; set; }
        public DateTime Date { get; set; }
    }
}
