using System;

namespace JackTheStudent.Models
{
    public partial class ClassMaterial
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public string Link { get; set; }
        public string ShortenedLink { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
