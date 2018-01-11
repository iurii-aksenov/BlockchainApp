using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BlockchainLibrary
{
    [DataContract]
    public class Block
    {
        public Block()
        {
        }

        public Block(int index, DateTime timeStamp, int proof, byte[] previousHash)
            : this(index, timeStamp, proof, previousHash, null)
        {
        }

        public Block(int index, DateTime timeStamp, int proof, byte[] previousHash,
            IEnumerable<Transaction> transactions)
        {
            Transactions = transactions as List<Transaction> ?? new List<Transaction>();
            Index = index;
            TimeStamp = timeStamp;
            Proof = proof;
            PreviousHash = previousHash;
        }

        [DataMember]
        public int Index { get; set; }

        [DataMember]
        public DateTime TimeStamp { get; set; }

        [DataMember]
        public List<Transaction> Transactions { get; set; }

        [DataMember]
        public int Proof { get; set; }

        [DataMember]
        public byte[] PreviousHash { get; set; }

        public override string ToString()
        {
            var transactions = Transactions.Select(x => x.ToString());

            return $"{nameof(Index)}: {Index};\n" +
                   $"{nameof(TimeStamp)}: {TimeStamp};\n" +
                   $"{nameof(Proof)}: {Proof};\n" +
                   $"{nameof(Transactions)}:" +
                   string.Join("\n", transactions) +
                   $"\n" +
                   $"{nameof(PreviousHash)}: {Convert.ToBase64String(PreviousHash)};";
        }
    }
}