using Newtonsoft.Json;
using PMS.Mapping;
using RestSharp;
using System;
using System.Collections.Generic;

namespace PMS.Dummy
{
    class Program
    {
        static void Main(string[] args)
        {
            var RequestURL = $"http://localhost:57247/api/order/postorder";
            IRestClient client = new RestClient(RequestURL);

            var req = new RestRequest(Method.POST);



            var data = new OrderModel();
            data.BillingAddress = "deneem adres";
            data.BillingAddressName = "deneme";
            data.BillingCity = "deneem city";
            data.BillingCountry = "türkiye";
            data.BillingEmail = "deneme@deneme.com";
            data.BillingIdentityNumber = "12134521458";
            data.BillingPostalCode = "123";
            data.BillingTaxNumber = "123";
            data.BillingTaxOffice = "deneme office";
            data.BillingTelNo1 = "12313";
            data.BillingTelNo2 = "1231";
            data.BillingTelNo3 = "212121";
            data.BillingTown = "town";
            data.CollectionViaCreditCard = 140;
            data.CollectionViaTransfer = 200;
            data.CreateDate = DateTime.Now;
            data.CreatedBy = 1;
            data.Explanation1 = "denem";
            data.OrderNo = "12313";
            data.ProjectCode = "45";
            data.OrderItems = new List<OrderItemModel>()
            {
                new OrderItemModel(){ Desi = 10, Currency = "TL", SKU = "DENEME SKU", Price = 10, Quantity = 200,IARSent = true,EmailSent = true, IsInChequeManage = true},
                new OrderItemModel(){ Desi = 10, Currency = "TL", SKU = "DENEME SKU2", Price = 20, Quantity = 300,IARSent = true,EmailSent = true, IsInChequeManage = true},
            };

            //req.AddHeader("content-type", "application/json");
            var jsonObject = JsonConvert.SerializeObject(data);
            //req.AddParameter("application/json", jsonObject , ParameterType.RequestBody);

            //var result = client.Execute(req);


        }
    }
}
