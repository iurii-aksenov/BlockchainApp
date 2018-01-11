using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BlockchainServer.DTO
{
    [DataContract]
    public class TotalNodesDTO
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public List<string> Nodes { get; set; }
    }
}
