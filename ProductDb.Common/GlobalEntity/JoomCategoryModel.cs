using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class JoomCategoryModel
    {
        public string Id { get; set; }
        public string Name{ get; set; }
        public string Path { get; set; }
        public string ParentId { get; set; }
    }
}
