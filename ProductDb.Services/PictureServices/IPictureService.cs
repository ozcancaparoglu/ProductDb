using ProductDb.Mapping.BiggBrandDbModels;
using System.Collections.Generic;

namespace ProductDb.Services.PictureServices
{
    public interface IPictureService
    {
        ICollection<PicturesModel> AllPicturesWithSku(string sku);
        PicturesModel AddNewPictures(PicturesModel model);
        ICollection<PicturesModel> AllActivePictures(int id);
        ICollection<PicturesModel> AllPictures(int id);
        PicturesModel EditPictures(Mapping.BiggBrandDbModels.PicturesModel model);
        int DeletePicture(PicturesModel model);
        PicturesModel PicturesById(int id);
        void SetState(int objectId);
        string ArrangePictureName(string sku,string extension);
        PictureConfiguration PictureConfiguration();
    }
}