using System;

namespace JackTheStudent.Models
{
    public partial class TeamsLink
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public string Date { get; set; }
        public string Link { get; set; }
        public string ShortenedLink { get; set; }
        public string ClassType { get; set; }
        public string GroupId { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
