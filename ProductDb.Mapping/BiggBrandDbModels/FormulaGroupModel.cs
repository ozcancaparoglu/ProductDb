using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class FormulaGroupModel : EntityBaseModel
    {
        [Required(ErrorMessage = "*This field is mandotary")]
        public string Name { get; set; }
    }
}
