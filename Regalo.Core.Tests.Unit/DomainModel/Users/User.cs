using System;

namespace Regalo.Core.Tests.Unit.DomainModel.Users
{
    public class User : AggregateRoot
    {
        private string _password;

        public void Register()
        {
            Record(new UserRegistered(Guid.NewGuid()));
        }

        public void ChangePassword(string newpassword)
        {
            if (string.IsNullOrWhiteSpace(newpassword)) throw new InvalidOperationException("New password cannot be empty or whitespace.");
            if (newpassword == _password) throw new InvalidOperationException("New password cannot be the same as the old password.");
         
            Record(new UserChangedPassword(newpassword));
        }

        private void Apply(UserRegistered evt)
        {
            Id = evt.AggregateId;
        }

        private void Apply(UserChangedPassword evt)
        {
            _password = evt.NewPassword;
        }
    }
}