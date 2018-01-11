using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;

namespace BlockchainLibrary
{
    //TODO переписать под .Core, просто изменить платформу ;) - done
    //TODO убрать обращение к посторонним ресурсам


    public class Blockchain : IBlockchain
    {
        private List<Transaction> _currentTransactions;

        public Blockchain()
        {
            Chain = new List<Block>();
            Nodes = new HashSet<string>();
            _currentTransactions = new List<Transaction>();
            CreateNewBlock(100, new[] {Convert.ToByte(1)});
        }

        public HashSet<string> Nodes { get; }

        public List<Block> Chain { get; private set; }

        public Block LastBlock => Chain[Chain.Count - 1];

        /// <summary>
        ///     Add a new node to the list of nodes
        /// </summary>
        /// <param name="address">Address of node. Eg. http://192.168.0.1:5001</param>
        public void RegisterNode(string address)
        {
            var uri = new Uri(address);
            Nodes.Add($"{uri.Host}:{uri.Port}"); // 192.168.0.1:5001
        }

        /// <summary>
        ///     Determine if a given blockchain is valid
        /// </summary>
        /// <param name="chain">A blockchain</param>
        /// <returns>True if valid, False if not</returns>
        public bool IsValidChain(List<Block> chain)
        {
            var lastBlock = chain[0];
            var currentIndex = 1;

            while (currentIndex < chain.Count)
            {
                var block = chain[currentIndex];
                Console.WriteLine($"{lastBlock}");
                Console.WriteLine($"{block}");
                Console.WriteLine("\n------------------\n");
                //проверяем на корректность хеша этого блока
                if (block.PreviousHash != Hash(lastBlock))
                    return false;
                // проеряем что алгоритм proof of work коректен
                if (!IsValidProof(lastBlock.Proof, block.Proof))
                    return false;
                lastBlock = block;
                currentIndex++;
            }
            return true;
        }

        /// <summary>
        ///     This is our Consensus Algorithm, it resolves conflicts
        ///     by replacing our chain with the longest one in the network
        /// </summary>
        /// <returns>True if our chain was replaced, False if not</returns>
        public bool ResolveConflicts()
        {
            var neighbours = Nodes;
            var newChain = new List<Block>();
            // ищем цепочки длиннее наших
            var maxLength = Chain.Count;

            var neighbourLength = -1;
            var neighbourChain = new List<Block>();
            // берем все цепочки во всех узлов нашей сети и проверяем их
            foreach (var neighbour in neighbours)
            {
                var address = "http://" + neighbour + "/chain";
                GetChain(address, out neighbourLength, out neighbourChain);

                // Проверяем что цепочка имеет максимальную длину и она корректна
                if (neighbourLength > maxLength && IsValidChain(neighbourChain))
                {
                    maxLength = neighbourLength;
                    newChain = neighbourChain;
                }
            }
            //заменяет нашу цепочку, если нашли другую, которая имеет большую длину и является корректной
            if (neighbourLength != -1)
            {
                Chain = newChain;
                return true;
            }
            return false;
        }

        private void GetChain(string address, out int length, out List<Block> chain)
        {
            var chainResponse = new ChainDTO
            {
                Lngth = -1,
                Chain = new List<Block>()
            };
            var request = (HttpWebRequest) WebRequest.Create(address);
            var response = (HttpWebResponse) request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
                using (var stream = response.GetResponseStream())
                {
                    var jsonSerializer = new DataContractJsonSerializer(typeof(ChainDTO));
                    chainResponse = (ChainDTO) jsonSerializer.ReadObject(stream);
                }
            length = chainResponse.Lngth;
            chain = chainResponse.Chain;
            response.Close();
        }

        /// <summary>
        ///     Create a new Block in the Blockchain
        /// </summary>
        /// <param name="proof">The proof given by the Proof of Work algorithm</param>
        /// <param name="previousHash">(Optional) Hash of previous Block</param>
        /// <returns>New Block</returns>
        public Block CreateNewBlock(int proof, byte[] previousHash = null)
        {
            var block = new Block(Chain.Count + 1, DateTime.Now.ToUniversalTime(), proof,
                previousHash ?? Hash(Chain[Chain.Count - 1]), _currentTransactions);

            // удаляет текущий список транзацкий 
            _currentTransactions = new List<Transaction>();
            Chain.Add(block);

            return block;
        }


        /// <summary>
        ///     Creates a new transaction to go into the next mined Block
        /// </summary>
        /// <param name="sender">Address of the Sender</param>
        /// <param name="recipient">ddress of the Recipient</param>
        /// <param name="amount">Amount</param>
        /// <returns>The index of the Block that will hold this transaction</returns>
        public int CreateNewTransaction(byte[] sender, byte[] recipient, int amount)
        {
            _currentTransactions.Add(new Transaction(sender, recipient, amount));
            return LastBlock.Index + 1;
        }

        /// <summary>
        ///     Find new proof where 4 leading zeroes
        /// </summary>
        /// <param name="lastProof">Previous proof</param>
        /// <returns></returns>
        public int CalculateProofOfWork(int lastProof)
        {
            var proof = 0;
            while (!IsValidProof(lastProof, proof))
                proof++;
            return proof;
        }

        /// <summary>
        ///     Validates the Proof: Does Hash of the LastProf and Proof contain 3 leadind zeroes
        /// </summary>
        /// <param name="lastProof">Previous proof</param>
        /// <param name="proof">Current proof</param>
        /// <returns>True - Valid; False - Not Valid</returns>
        public static bool IsValidProof(int lastProof, int proof)
        {
            byte[] ConvertIntToByteArray(int intValue)
            {
                var intBytes = BitConverter.GetBytes(intValue);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(intBytes);
                var res = intBytes;
                return res;
            }

            bool IsValid(byte[] hash)
            {
                var len = hash.Length;
                var zero = Convert.ToByte(0);
                return hash[len - 1] == zero && hash[len - 2] == zero && hash[len - 3] == zero;
            }


            var lastProofBytes = ConvertIntToByteArray(lastProof);
            var proofBytes = ConvertIntToByteArray(proof);

            var crypt = new SHA256Managed();

            var bytes = lastProofBytes.Concat(proofBytes).ToArray();

            var cryptHash = crypt.ComputeHash(bytes);
            var result = IsValid(cryptHash);
            return result;
        }


        /// <summary>
        ///     Creates a SHA-256 hash of a Block
        /// </summary>
        /// <param name="block">Block</param>
        /// <returns>Hass</returns>
        public static byte[] Hash(Block block)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(Block));
            var memoryStream = new MemoryStream();
            jsonSerializer.WriteObject(memoryStream, block);
            var bytesOfStream = memoryStream.ToArray();
            var crypt = new SHA256Managed();
            var result = crypt.ComputeHash(bytesOfStream);
            return result;
        }
    }
}