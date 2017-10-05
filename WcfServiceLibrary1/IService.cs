using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract(Name = "WeatherService", Namespace = "http://www.mycompany.com/weather")]

    public interface IWheatherService
    {
        [OperationContract]
        Task<WheatherInfo> GetWhetherInfo(string city);
        [OperationContract]
        Task<string> GetWhetherInfoSerialized(string city);
        [OperationContract]
        void SendEmailAsync(string city, string receiversAddress);
    }

}
