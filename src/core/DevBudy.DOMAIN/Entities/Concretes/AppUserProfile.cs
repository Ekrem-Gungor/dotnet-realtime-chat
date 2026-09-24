using DevBudy.DOMAIN.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.DOMAIN.Entities.Concretes
{
    public class AppUserProfile : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        //public Gender Gender { get; set; }

        // Relational Properties

        public virtual AppUser AppUser { get; set; }
    }
}
