using System;
using System.Collections.Generic;

namespace JackTheStudent.Models
{
    public class User 
    {
        public long _Id { get; set; }
        public int Points { get; set; } = 0;
        
        public ulong Id{
            get{
                unchecked
                {
                    return (ulong)_Id;
                }
            }
            set{
                unchecked
                {
                    _Id =(long)value;
                }
            }
        }

        public int GetPoints()
        {
        return this.Points;
        }

        public void AddPoints(int Points)
        {
        this.Points += Points;
        }
    }
}