using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public partial class DickAppointments
    {
        public int IdDickAppointment { get; set; }
        public string Name { get; set; }
        public string Length { get; set; }
        public string Circumference { get; set; }
        public string Width { get; set; }
        public DateTime Date { get; set; }
    }
}
