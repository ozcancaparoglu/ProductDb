using LogoInvoiceSrv;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.LogoService.LogoService
{
    public interface ILogoInvService
    {
        Task<LoginResponse> ConnectService(string CompanyCode);
        Task<LoginResponse> Login(LoginType login);
        Task LogOut(string sessionID);
        Task<eFaturaWebServiceResultType> GetValidateGIBUser(string sessionID, string[] paramList = null);
    }
}
