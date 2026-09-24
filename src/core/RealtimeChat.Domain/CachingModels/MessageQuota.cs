using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Domain.CachingModels
{
    public class MessageQuota
    {
        public int ReaminingMessage { get; set; }
        public TimeSpan ExpireAt { get; set; }

        public MessageQuota()
        {

        }

        public MessageQuota(int quota, TimeSpan expireAt)
        {
            ReaminingMessage = quota;
            ExpireAt = expireAt;
        }
    }
}
