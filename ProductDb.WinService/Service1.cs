using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Timers;

namespace ProductDb.WinService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        Timer timerCA = new Timer();
        Timer timerPrice = new Timer();
        Logger Logger = null;
        string LogoServiceURL = string.Empty;

        /// <summary>
        ///  Redis Caching Product
        /// </summary>
        public string LogoStockRedisCache = string.Empty;

        string StoreServiceURL = string.Empty;
        string LogoProductStocksWarehouseTypeURL = string.Empty;
        string CalculatePriceServiceURL = string.Empty;
        string ChannelAdvisorOrder = string.Empty;
        string ChannelAdvisorQuantityUpdate = string.Empty;
        string ChannelAdvisorQuantityUpdate2 = string.Empty;
        string ChannelAdvisorQuantityUpdate3 = string.Empty;
        string JoomQuantityUpdate = string.Empty;
        int IntervalTime = 0;
        int IntervalTimeCA = 0;
        int IntervalTimePrice = 0;
        int TimeOut = 0;
        public Service1()
        {
            InitializeComponent();
            Logger = Logger.GetLogger;
            IntervalTime = Convert.ToInt32(ConfigurationManager.AppSettings.Get("IntervalTime"));
            IntervalTimeCA = Convert.ToInt32(ConfigurationManager.AppSettings.Get("IntervalTimeCA"));
            IntervalTimePrice = Convert.ToInt32(ConfigurationManager.AppSettings.Get("IntervalTimePrice"));
            LogoServiceURL = ConfigurationManager.AppSettings.Get("LogoRemoteServiceURL");
            StoreServiceURL = ConfigurationManager.AppSettings.Get("StoreRemoteServiceURL");
            ChannelAdvisorOrder = ConfigurationManager.AppSettings.Get("ChannelAdvisorOrderURL");
            ChannelAdvisorQuantityUpdate = ConfigurationManager.AppSettings.Get("ChannelAdvisorQuantityUpdateURL");
            ChannelAdvisorQuantityUpdate2 = ConfigurationManager.AppSettings.Get("ChannelAdvisorQuantityUpdateURL2");
            ChannelAdvisorQuantityUpdate3 = ConfigurationManager.AppSettings.Get("ChannelAdvisorQuantityUpdateURL3");
            JoomQuantityUpdate = ConfigurationManager.AppSettings.Get("JoomQuantityUpdateURL");
            TimeOut = Convert.ToInt32(ConfigurationManager.AppSettings.Get("TimeOut"));
            LogoProductStocksWarehouseTypeURL = ConfigurationManager.AppSettings.Get("LogoProductStocksWarehouseTypeURL");

            LogoStockRedisCache = ConfigurationManager.AppSettings.Get("LogoRedisStockCache");

            CalculatePriceServiceURL = ConfigurationManager.AppSettings.Get("CalculatePriceServiceURL");
        }

        protected override void OnStart(string[] args)
        {
            Logger.WriteLog("Service is started...");
//#if DEBUG
//            //base.RequestAdditionalTime(600000); // 10 minutes timeout for startup
//            Debugger.Launch(); // launch and attach debugger
//#endif
            UpdateProductDbStock(LogoServiceURL);
            UpdateProductDbStock(StoreServiceURL);
            UpdateProductDbStock(LogoProductStocksWarehouseTypeURL);
            UpdateProductDbPrices(CalculatePriceServiceURL);
            //UpdateChannelAdvisor(ChannelAdvisorOrder);
            //UpdateChannelAdvisor(ChannelAdvisorQuantityUpdate);
            //UpdateChannelAdvisor(ChannelAdvisorQuantityUpdate2);
            //UpdateChannelAdvisor(ChannelAdvisorQuantityUpdate3);
            //UpdateChannelAdvisor(JoomQuantityUpdate);
            timer.Elapsed += new ElapsedEventHandler(ElapsedTime);
            timerCA.Elapsed += new ElapsedEventHandler(CAElapsedTime);
            timerPrice.Elapsed += new ElapsedEventHandler(PriceElapsedTime);

            timer.Interval = IntervalTime;
            timer.Enabled = true;

            timerCA.Interval = IntervalTimeCA;
            timerCA.Enabled = true;

            timerPrice.Interval = IntervalTimePrice;
            timerPrice.Enabled = true;
        }
        public void onDebug()
        {
            OnStart(null);
        }
        private void CAElapsedTime(object sender, ElapsedEventArgs e)
        {
            //UpdateChannelAdvisor(ChannelAdvisorOrder);
            //UpdateChannelAdvisor(ChannelAdvisorQuantityUpdate);
            //UpdateChannelAdvisor(ChannelAdvisorQuantityUpdate2);
            //UpdateChannelAdvisor(ChannelAdvisorQuantityUpdate3);
            //UpdateChannelAdvisor(JoomQuantityUpdate);
        }
        private void ElapsedTime(object sender, ElapsedEventArgs e)
        {
            UpdateProductDbStock(LogoServiceURL);
            UpdateProductDbStock(StoreServiceURL);
            UpdateProductDbStock(LogoProductStocksWarehouseTypeURL);
            // Redis Caching
            //UpdateProductDbStock(LogoStockRedisCache);

        }

        private void PriceElapsedTime(object sender, ElapsedEventArgs e)
        {
            UpdateProductDbPrices(CalculatePriceServiceURL);
        }

        void UpdateProductDbStock(string RemoteServiceURL)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(RemoteServiceURL);
                request.Method = "POST";
                request.ContentLength = 0;
                request.Timeout = TimeOut;
                var response = (HttpWebResponse)request.GetResponse();

                string message = response.StatusCode == HttpStatusCode.OK ? "ProductDb Stocks Updated" : "Failed Description :" + response.StatusDescription + " Remnote Url" + RemoteServiceURL;
                Logger.WriteLog(message);
                response.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.Message.ToString() + " Remnote Url"  + RemoteServiceURL);
            }
        }

        void UpdateProductDbPrices(string remoteServiceUrl)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(remoteServiceUrl);
                request.Method = "POST";
                request.ContentLength = 0;
                request.Timeout = TimeOut;
                var response = (HttpWebResponse)request.GetResponse();

                string message = response.StatusCode == HttpStatusCode.OK ? "Prices Updated" : "Failed Description : " + response.StatusDescription + " Remnote Url : " + remoteServiceUrl;
                Logger.WriteLog(message);
                response.Close();

            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.Message.ToString() + " Remnote Url " + remoteServiceUrl);
            }
        }

        void UpdateChannelAdvisor(string ServiceURL)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(ServiceURL);
                request.Method = "POST";
                request.ContentLength = 0;
                request.Timeout = TimeOut;
                var response = (HttpWebResponse)request.GetResponse();

                string message = response.StatusCode == HttpStatusCode.OK ? "Channel Advisor ProductInfo Updated" : "Failed Description :" + response.StatusDescription;
                Logger.WriteLog(message);
                response.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.Message.ToString());
            }
        }

        protected override void OnStop()
        {
            Logger.WriteLog("Service is stoped...");
        }
    }
}
