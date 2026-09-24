using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeChat.Application.Common.Exceptions
{
    public sealed class MessageQuotaExceededException : Exception
    {
        public MessageQuotaExceededException() : base("The hourly message quota has been exceeded.")
        {
        }
    }
}
