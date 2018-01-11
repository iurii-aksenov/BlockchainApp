using System;
using System.Runtime.Serialization;

namespace BlockchainLibrary
{
    [DataContract]
    public class Transaction
    {
        public Transaction()
        {
        }

        public Transaction(byte[] sender, byte[] recipient, int amount)
        {
            Sender = sender;
            Recipient = recipient;
            Amount = amount;
        }

        [DataMember]
        public byte[] Sender { get; set; }

        [DataMember]
        public byte[] Recipient { get; set; }

        [DataMember]
        public int Amount { get; set; }

        public override string ToString()
        {
            return $"{nameof(Sender)}: {Convert.ToBase64String(Sender)}; " +
                   $"{nameof(Recipient)}:{Convert.ToBase64String(Recipient)}; " +
                   $"{nameof(Amount)}:{Amount};";
        }
    }
}