using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Common.Dto
{
    public class LogoResponseDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public bool isGIBUSER { get; set; }
        public int statusValue { get; set; }
    }
}
