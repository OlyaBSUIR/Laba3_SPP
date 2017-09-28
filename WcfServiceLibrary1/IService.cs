using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary1
{
    [ServiceContract(Name = "WhetherService",
        Namespace = "http://www.mycompany.com/whether/2010/05/24")]

    public interface IWheatherService
    {
        [OperationContract]
        Task<WheatherInfo> GetWhetherInfo(string city);
        [OperationContract]
        Task<string> GetWhetherInfoSerialized(string city);
    }

}
