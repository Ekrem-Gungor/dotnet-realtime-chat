using RealtimeChat.Contracts.Repositories.EFRepositories;
using RealtimeChat.Domain.Entities.Concretes;
using RealtimeChat.Persistence.ContextClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Persistence.Repositories.EFRepositories
{
    public class AppUserProfileRepository : Repository<AppUserProfile>, IAppUserProfileRepository
    {
        public AppUserProfileRepository(RealtimeChatDbContext db) : base(db)
        {

        }
    }
}
