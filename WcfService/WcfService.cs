using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.ServiceModel;
using System.Globalization;
using WcfService.Contract;
using WcfService.ServiceImplementation;
using System.ServiceModel.Security;
using System.ServiceModel.Description;

namespace WcfService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class WcfService : StatelessService
    {
        public WcfService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            yield return new ServiceInstanceListener(context => this.CreateWcfCommunicationListener(context));
        }

        private ICommunicationListener CreateWcfCommunicationListener(StatelessServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("WcfServiceEndpoint");
            int port = endpointConfig.Port;
            var scheme = endpointConfig.Protocol.ToString();

            var binding = new BasicHttpsBinding()
            {
                Namespace = "https://samplewcf.dev.com/"
            };

            // Setting the security mode to Transport and credentialType to Basic'
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/wcfservice", scheme, host, port);
            var listener = new WcfCommunicationListener<IWcfService>(
                serviceContext: context,
                wcfServiceObject: new WcfServiceImplementation(),
                listenerBinding: binding,
                address: new EndpointAddress(uri)
            );

            listener.ServiceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator
                = new CustomUserNamePasswordValidator();
            listener.ServiceHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode 
                = UserNamePasswordValidationMode.Custom;

            // Adding the behaviour to generate flat WSDL without any WSDL imports 
            foreach (var endpoint in listener.ServiceHost.Description.Endpoints)
            {
                endpoint.EndpointBehaviors.Add(new FlatWsdl());
            }

            // Check to see if the service host already has a ServiceMetadataBehavior
            ServiceMetadataBehavior smb = listener.ServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
            // If not, add one
            if (smb == null)
            {
                smb = new ServiceMetadataBehavior();
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                smb.HttpsGetEnabled = true;
                smb.HttpsGetUrl = new Uri(uri);
                listener.ServiceHost.Description.Behaviors.Add(smb);
            }

            return listener;
        }
    }
}
