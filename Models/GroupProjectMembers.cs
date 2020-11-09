using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public partial class GroupProjectMember
    {
        public int Id { get; set; }
        public string Member { get; set; }
        public Project Project { get; set; }
    }
}