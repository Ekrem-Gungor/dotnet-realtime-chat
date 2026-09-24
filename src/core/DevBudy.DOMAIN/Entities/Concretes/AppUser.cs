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
    public class AppUser : IdentityUser<int>, IEntity
    {
        public AppUser()
        {
            CreatedDate = DateTime.Now;
            DataStatus = DataStatus.Inserted;
            ActiveStatus = ActiveStatus.Active;
            IsOnline = false;
        }
        public int ID { get; set; }
        public Guid? ActivationCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime LastLogout { get; set; }
        public DateTime LastActivityTime { get; set; }
        public bool IsPersonalDataConsentGiven { get; set; }
        public bool IsOnline { get; set; }
        public DataStatus DataStatus { get; set; }
        public ActiveStatus ActiveStatus { get; set; }

        // Relational Properties
        public virtual AppUserProfile Profile { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
