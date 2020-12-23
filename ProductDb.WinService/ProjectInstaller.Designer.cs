namespace ProductDb.WinService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProductDbLogoWinService = new System.ServiceProcess.ServiceProcessInstaller();
            this.ProductDbWinService = new System.ServiceProcess.ServiceInstaller();
            // 
            // ProductDbLogoWinService
            // 
            this.ProductDbLogoWinService.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ProductDbLogoWinService.Password = null;
            this.ProductDbLogoWinService.Username = null;
            // 
            // ProductDbWinService
            // 
            this.ProductDbWinService.Description = "Product Db Logo Request Service";
            this.ProductDbWinService.DisplayName = "Product Db Logo Request Service";
            this.ProductDbWinService.ServiceName = "ProductDbLogoService";
            this.ProductDbWinService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ProductDbLogoWinService,
            this.ProductDbWinService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ProductDbLogoWinService;
        private System.ServiceProcess.ServiceInstaller ProductDbWinService;
    }
}