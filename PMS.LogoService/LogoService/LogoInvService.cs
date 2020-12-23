using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LogoInvoiceSrv;
using PMS.Service.LoggingService;

namespace PMS.LogoService.LogoService
{
    public class LogoInvService : ILogoInvService
    {
        PostBoxServiceClient postBoxServiceClient;
        public LogoInvService()
        {
            postBoxServiceClient = new PostBoxServiceClient(
                PostBoxServiceClient.EndpointConfiguration.PostBoxServiceEndpoint);
        }

        public async Task<LoginResponse> ConnectService(string CompanyCode)
        {
            var LoginType = new LoginType
            {
                passWord = "EUDD56K8",
                userName = "7430132605"
            };
            return await Login(LoginType);
        }

        public async Task<eFaturaWebServiceResultType> GetValidateGIBUser(string sessionID, string[] paramList = null)
        {
            //GetValidateGIBUser("1", new string[] { "VKN=123456789" ,"DOCUMENTTYPE=0"});
            return await postBoxServiceClient.GetValidateGIBUserAsync(sessionID, paramList);
        }

        public async Task<LoginResponse> Login(LoginType login)
        {
            var req = new LoginRequest(login);
            return await postBoxServiceClient.LoginAsync(req);
        }

        public async Task LogOut(string sessionID)
        {
            await postBoxServiceClient.LogoutAsync(sessionID);
        }

    }
}
