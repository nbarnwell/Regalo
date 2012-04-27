using System;
using Regalo.Core;

namespace Regalo.RavenDB.Tests.Unit.DomainModel.Customers
{
    public class AccountManager : AggregateRoot
    {
        private DateTime _startDate;

         public void Employ(DateTime startDate)
         {
             // Check the start date against known rules

             Record(new Employed(Guid.NewGuid(), startDate));
         }

        private void Apply(Employed evt)
        {
            Id = evt.AggregateId;
            _startDate = evt.StartDate;
        }
    }
}