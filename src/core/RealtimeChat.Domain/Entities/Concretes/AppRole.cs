using RealtimeChat.Domain.Entities.Abstracts;
using RealtimeChat.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Domain.Entities.Concretes
{
    public class AppRole : IdentityRole<int>, IEntity
    {
        public AppRole()
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
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
    }
}
