using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BlockchainServer.DTO
{
    [DataContract]
    public class TransactionDTO
    {
        [DataMember]
        public byte[] Sender { get; set; }

        [DataMember]
        public byte[] Recipient { get; set; }

        [DataMember]
        public int Amount { get; set; }
    }
}
