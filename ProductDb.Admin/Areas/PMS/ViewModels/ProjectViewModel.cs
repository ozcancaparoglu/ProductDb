using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.Common.Dto;
namespace ProductDb.Admin.Areas.PMS.ViewModels
{
    public class ProjectViewModel
    {
        public int? firmNo { get; set; }
        public IEnumerable<Project> Projects { get; set; }
    }
}
