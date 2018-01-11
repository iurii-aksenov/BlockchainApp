using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using BlockchainLibrary;

namespace BlockchainServer.DTO
{
    [DataContract]
    public class ResolvedConflictsDTO
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public List<Block> Chain { get; set; }
    }
}
