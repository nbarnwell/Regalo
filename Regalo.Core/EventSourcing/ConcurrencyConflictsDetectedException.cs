using System;
using System.Collections.Generic;

namespace Regalo.Core.EventSourcing
{
    public class ConcurrencyConflictsDetectedException : Exception
    {
        public IEnumerable<ConcurrencyConflict> Conflicts { get; private set; }

        public ConcurrencyConflictsDetectedException(IEnumerable<ConcurrencyConflict> conflicts)
        {
            if (conflicts == null) throw new ArgumentNullException("conflicts");
            Conflicts = conflicts;
        }
    }
}