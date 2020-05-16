using System;

namespace Ch.Common.CommonModel
{
    public class BaseEntity
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
