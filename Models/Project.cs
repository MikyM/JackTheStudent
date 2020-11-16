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
        public string ClassShortName { get; set; }
        public DateTime Date { get; set; }
        public string GroupId { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
        public ICollection<GroupProjectMember> GroupProjectMembers { get; set; }

        public async Task<List<GroupProjectMember>> GetParticipants()
        {
            string participantsString = String.Empty; 
            using (var db = new JackTheStudentContext()){  
                var participants = db.GroupProjectMember
                                        .Where( x => x.ProjectId == this.Id)
                                        .ToList();
                return participants;
            }
        }
    }
}