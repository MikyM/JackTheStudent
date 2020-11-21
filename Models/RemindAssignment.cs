using System.ComponentModel.DataAnnotations.Schema;

namespace JackTheStudent.Models
{
    public partial class RoleAssignment
    {
        public int Id { get; set; }
        public long _RoleId { get; set; }
        public long _MessageId { get; set; }

        [NotMapped]
        public ulong RoleId
        {
            get { 
                unchecked   {
                    return (ulong) _RoleId;
                }
            }

            set {
                unchecked {
                    _RoleId = (long)value;
                }
            }
        }
        [NotMapped]
        public ulong MessageId
        {
            get {
                unchecked {
                    return (ulong) _MessageId;
                }
            }

            set {
                unchecked {
                    _MessageId = (long)value;
                }
            }
        }
    }
}