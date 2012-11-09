using System;

namespace Regalo.Core.Tests.DomainModel.Users
{
    public class User : AggregateRoot
    {
        private string _password;

        /// <summary>
        /// Created to support a very specific test. Typical domain objects would *never* have public getters or setters!
        /// </summary>
        public int ChangeCount { get; private set; }

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

        private void Apply(object evt)
        {
            ChangeCount++;
        }

        private void Apply(UserRegistered evt)
        {
            Id = evt.UserId;
        }

        private void Apply(UserChangedPassword evt)
        {
            _password = evt.NewPassword;
        }
    }
}