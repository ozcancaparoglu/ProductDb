using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Services.SupplierServices
{
    public interface ISupplierService
    {
        ICollection<SupplierModel> AllActiveSuppliers();
        ICollection<SupplierModel> AllSuppliers();
        SupplierModel SupplierById(int id);
        SupplierModel AddNewSupplier(SupplierModel model);
        SupplierModel EditSupplier(SupplierModel model);
        void SetState(int objectId);
    }
}
