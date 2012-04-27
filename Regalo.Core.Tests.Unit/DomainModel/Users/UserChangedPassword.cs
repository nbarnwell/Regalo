using System;

namespace Regalo.Core.Tests.Unit.DomainModel.Users
{
    public class UserChangedPassword : Event
    {
        public string NewPassword { get; private set; }

        public UserChangedPassword(string newpassword)
        {
            if (newpassword == null) throw new ArgumentNullException("newpassword");

            NewPassword = newpassword;
        }

        public bool Equals(UserChangedPassword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.NewPassword, NewPassword);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(UserChangedPassword)) return false;
            return Equals((UserChangedPassword)obj);
        }

        public override int GetHashCode()
        {
            return (NewPassword != null ? NewPassword.GetHashCode() : 0);
        }

        public static bool operator ==(UserChangedPassword left, UserChangedPassword right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserChangedPassword left, UserChangedPassword right)
        {
            return !Equals(left, right);
        }
    }
}