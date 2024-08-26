using System;

namespace OBDC.Core.Models
{
    public class PlayerAccount
    {
        public Guid AccountId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
