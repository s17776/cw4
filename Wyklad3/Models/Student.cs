using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyklad3.Models
{
    public class Student
    {
        public string IndexNumber { get; set; }
      //  [Required]
      //  [MaxLength(10)]
        public string FirstName { get; set; }
      //  [Required]
       // [MaxLength(255)]
        public string LastName { get; set; }

        public DateTime Birthdate { get; set; }

      //  [Required]
        public string Studies { get; set; }

    
    }
}
