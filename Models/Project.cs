using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace JackTheStudent.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public Boolean IsGroup { get; set; }
        public string Class { get; set; }
        public DateTime Date { get; set; }
        public string GroupId { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
        public ICollection<GroupProjectMember> GroupProjectMembers { get; set; }

        public List<GroupProjectMember> GetParticipants()
        {
            if (this.IsGroup) {
                return this.GroupProjectMembers.ToList();
            } else {
                return null;
            }
        }

        public void SetParticipants(List<GroupProjectMember> members)
        {
            if (this.IsGroup) {
                this.GroupProjectMembers = members;
            } else {
                this.GroupProjectMembers = null;
            } 
        }

        public GroupProjectMember GetParticipant(int participantId) 
        {
            if(this.IsGroup) {
                return this.GroupProjectMembers.Where(m => m.Id == participantId).FirstOrDefault();
            } else {
                return null;
            }
        }

        public void AddParticipant(GroupProjectMember participant) 
        {
            if(this.IsGroup) {
                this.GroupProjectMembers.Add(participant);
            }
        }

        public void AddParticipants(List<GroupProjectMember> participants) 
        {
            if(this.IsGroup) {
                this.GroupProjectMembers.Concat(participants);
            }
        }

    }
}