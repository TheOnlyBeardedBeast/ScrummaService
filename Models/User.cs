using System;

namespace ScrummaService.Models
{
    public enum RoleEnum
    {
        User,
        Observer
    }

    public class User
    {
        public int Group { get; set; }
        public string ConnectionId { get; set; }

        public string UserName { get; set; }

        public float? Vote { get; set; }

        public RoleEnum Role { get; set; } = RoleEnum.User;
    }
}