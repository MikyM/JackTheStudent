using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MySql.Data.EntityFrameworkCore.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace JackTheStudent.Models
{
    public partial class Class
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
    }
}
