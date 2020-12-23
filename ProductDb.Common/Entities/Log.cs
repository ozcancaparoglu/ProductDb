using System;

namespace ProductDb.Common.Entities
{
    public class Log
    {
        public string ExceptionMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Method { get; set; }
    }
}
