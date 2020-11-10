using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace JackTheStudent.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public Boolean isGroup { get; set; }
        public string Class { get; set; }
        public DateTime Date { get; set; }
        public string GroupId { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
        public string Materials { get; set; }
        public ICollection<GroupProjectMember> GroupProjectMembers { get; set; }

        public async Task<string> GetParticipantsString()
        {
            string participantsString = String.Empty; 
            using (var db = new JackTheStudentContext()){  
                var participants = db.GroupProjectMember
                                        .Where( x => x.ProjectId == this.Id)
                                        .ToList();
                    foreach (GroupProjectMember participant in participants) {
                        participantsString = participantsString + participant.Member + ", ";
                    }
            participantsString = $"\nMembers: {participantsString.Substring(0, participantsString.Length-2)}"; 
            }
        return participantsString;
        }
    }
}