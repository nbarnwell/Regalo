using System;

namespace Regalo.Core.Tests.DomainModel.Users
{
    public class UserChangedPassword : Event
    {
        public UserChangedPassword(string newpassword)
        {
            if (newpassword == null) throw new ArgumentNullException("newpassword");

            NewPassword = newpassword;
        }

        public string NewPassword { get; private set; }
    }
}