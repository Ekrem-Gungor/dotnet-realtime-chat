using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.DOMAIN.Enums
{
    public enum MessageType
    {
        Text = 1,
        Image = 2,
        Video = 3,
        File = 4,
        Location = 5,
        Contact = 6,
        Poll = 7,
        Reaction = 8,
        SystemNotification = 9,
        Unknown = 10
    }
}
