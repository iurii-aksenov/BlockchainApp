using BlockchainLibrary;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BlockchainServer.DTO
{
    [DataContract]
    public class ChainDTO
    {
        [DataMember]
        public List<Block> Chain { get; set; }

        [DataMember]
        public int Lngth { get; set; }
    }
}