using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using WcfClient = WcfServiceClient.SampleServiceRef.WcfServiceClient;

namespace WcfServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WcfClient client = new WcfClient();
            client.ClientCredentials.UserName.UserName = "validusername";
            client.ClientCredentials.UserName.Password = "validPassword";
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            // Add the below only in DEV machines. 
            // **** DO NOT ADD THIS IN PRODUCTION ****
            System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                return true;
            };

            var response = client.GetRamdomGuid();
            Console.WriteLine(response);
        }
    }
}