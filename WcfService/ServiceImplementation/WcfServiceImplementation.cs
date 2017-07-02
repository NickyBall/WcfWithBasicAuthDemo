using System;
using System.ServiceModel;
using WcfService.Contract;

namespace WcfService.ServiceImplementation
{
    [ServiceBehavior(Namespace = "https://samplewcf.dev.com/")]
    public class WcfServiceImplementation : IWcfService
    {
        public WcfServiceImplementation()
        {

        }

        public string GetRamdomGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
