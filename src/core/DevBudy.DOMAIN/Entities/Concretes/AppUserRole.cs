using DevBudy.DOMAIN.Entities.Abstracts;
using DevBudy.DOMAIN.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.DOMAIN.Entities.Concretes
{
    public class AppUserRole : IdentityUserRole<int>, IEntity
    {
        public AppUserRole()
        {
            CreatedDate = DateTime.Now;
            DataStatus = DataStatus.Inserted;
            ActiveStatus = ActiveStatus.Active;
        }
        public int ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DataStatus DataStatus { get; set; }
        public ActiveStatus ActiveStatus { get; set; }

        //Relational Properties
        public virtual AppUser User { get; set; }
        public virtual AppRole Role { get; set; }
    }
}
