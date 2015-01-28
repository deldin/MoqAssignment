using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Ista.FileServices.Service
{
    [RunInstaller(true)]
    public partial class ServiceHostInstaller : Installer
    {
        private readonly ServiceProcessInstaller serviceProcess;
        private readonly ServiceInstaller service;
        
        public ServiceHostInstaller()
        {
            InitializeComponent();

            serviceProcess = new ServiceProcessInstaller();
            service = new ServiceInstaller();

            serviceProcess.Account = ServiceAccount.LocalSystem;
            service.StartType = ServiceStartMode.Automatic;
            service.DisplayName = "Ista Global File Services";
            service.ServiceName = "IstaFileServices";

            Installers.Add(service);
            Installers.Add(serviceProcess);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            var name = GetContextParameter("name");
            var displayname = GetContextParameter("displayname");
            var description = GetContextParameter("description");

            if (!string.IsNullOrWhiteSpace(name))
                service.ServiceName = name;
            if (!string.IsNullOrWhiteSpace(displayname))
                service.DisplayName = displayname;
            if (!string.IsNullOrWhiteSpace(description))
                service.Description = description;

            serviceProcess.Account = ServiceAccount.LocalSystem;
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            var serviceName = GetContextParameter("name");
            if (!string.IsNullOrWhiteSpace(serviceName))
                service.ServiceName = serviceName;
        }

        private string GetContextParameter(string key)
        {
            if (!Context.Parameters.ContainsKey(key))
                return string.Empty;

            return Context.Parameters[key] ?? string.Empty;
        }
    }
}
