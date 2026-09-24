using RealtimeChat.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Contracts.Repositories.EFRepositories
{
    public interface IChatMessageRepository : IRepository<ChatMessage>
    {

    }
}
