using System;

namespace ContactManager.Domain
{
    public class BaseEntity
    {
        public long Id { get; set; }

        public int Version { get; set; }
    }
}
