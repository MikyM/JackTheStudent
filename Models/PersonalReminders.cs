using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public partial class PersonalReminders
    {
        public int IdReminder { get; set; }
        public string About { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
        public string Materials { get; set; }
    }
}
