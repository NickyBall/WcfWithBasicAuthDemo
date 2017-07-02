using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService.Contract
{
    [ServiceContract(Namespace = "https://samplewcf.dev.com/")]
    [XmlSerializerFormat]
    public interface IWcfService
    {
        [OperationContract]
        string GetRamdomGuid();
    }
}
