using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.LogoService.LogoModels
{
    public class LogoResponseModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public bool isGIBUSER { get; set; }
        public int statusValue { get; set; }
        public int LogicalRef { get; set; }
    }
}
