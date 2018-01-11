using BlockchainLibrary;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BlockchainServer.DTO
{
    [DataContract]
    public class MineDTO
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public int Index { get; set; }

        [DataMember]
        public List<Transaction> Transactions { get; set; }

        [DataMember]
        public int Proof { get; set; }

        [DataMember]
        public byte[] PreviousHash { get; set; }
    }
}