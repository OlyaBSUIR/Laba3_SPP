using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WcfServiceLibrary1
{
    [DataContract]
    public class WheatherInfo
    {
        [DataMember]
        public string temperature { get; set; }
        [DataMember]
        public string speed { get; set; }
        [DataMember]
        public string pressure { get; set; }
        [DataMember]
        public string lastUpdate { get; set; }
        [DataMember]
        public string humadity { get; set; }

    }
}
