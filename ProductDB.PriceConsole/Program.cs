using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Mapping.AutoMapperConfigurations;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.StoreFormulaServices;

namespace ProductDB.PriceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var serviceProvider = new ServiceCollection()
                   .AddLogging()
                   .AddTransient<IPriceCalculatorService, PriceCalculatorService>()
                   .AddSingleton<IUnitOfWork, UnitOfWork>()
                   .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                   .AddSingleton<IAutoMapperConfiguration, AutoMapperConfiguration>()
                   .AddDbContext<BiggBrandsDbContext>(opts => opts.UseSqlServer("Data Source=10.0.0.40;Initial Catalog=BIGGBRANDSDB_V2;Integrated Security=False;Persist Security Info=False;User ID=biggbrands;Password=CkMjNH7s1"))
                   .BuildServiceProvider();
                var _priceCalcService = serviceProvider.GetService<IPriceCalculatorService>();

                //_priceCalcService.ProcessAllStorePrices(9, 32);

                var _last = _priceCalcService.GetLatestPriceLogRequest();
                if (_last != null)
                {
                    //_priceCalcService.DeleteAllResultsByStoreId(_last.StoreId);
                    Console.WriteLine("Started : " + DateTime.Now);
                    _last.OperationStartDate = DateTime.Now;
                    _priceCalcService.UpdatePricelog(_last);
                    Console.WriteLine("Updated Price Log: " + DateTime.Now);
                    _priceCalcService.ProcessAllStorePrices(_last.StoreId, _last.Id);
                    Console.WriteLine("Updated Prices: " + DateTime.Now);
                    _last.OperationCompleteDate = DateTime.Now;
                    Console.WriteLine("Updated Price Log Last: " + DateTime.Now);
                    _priceCalcService.UpdatePricelog(_last);
                    Console.WriteLine("Finished: " + DateTime.Now);
                }

                GC.Collect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
