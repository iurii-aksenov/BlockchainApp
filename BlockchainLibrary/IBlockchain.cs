using System.Collections.Generic;

namespace BlockchainLibrary
{
    public interface IBlockchain
    {
        HashSet<string> Nodes { get; }
        List<Block> Chain { get; }
        Block LastBlock { get; }

        /// <summary>
        ///     Add a new node to the list of nodes
        /// </summary>
        /// <param name="address">Address of node. Eg. http://192.168.0.1:5001</param>
        void RegisterNode(string address);

        /// <summary>
        ///     Determine if a given blockchain is valid
        /// </summary>
        /// <param name="chain">A blockchain</param>
        /// <returns>True if valid, False if not</returns>
        bool IsValidChain(List<Block> chain);

        /// <summary>
        ///     This is our Consensus Algorithm, it resolves conflicts
        ///     by replacing our chain with the longest one in the network
        /// </summary>
        /// <returns>True if our chain was replaced, False if not</returns>
        bool ResolveConflicts();

        /// <summary>
        ///     Create a new Block in the Blockchain
        /// </summary>
        /// <param name="proof">The proof given by the Proof of Work algorithm</param>
        /// <param name="previousHash">(Optional) Hash of previous Block</param>
        /// <returns>New Block</returns>
        Block CreateNewBlock(int proof, byte[] previousHash = null);

        /// <summary>
        ///     Creates a new transaction to go into the next mined Block
        /// </summary>
        /// <param name="sender">Address of the Sender</param>
        /// <param name="recipient">ddress of the Recipient</param>
        /// <param name="amount">Amount</param>
        /// <returns>The index of the Block that will hold this transaction</returns>
        int CreateNewTransaction(byte[] sender, byte[] recipient, int amount);

        /// <summary>
        ///     Find new proof where 4 leading zeroes
        /// </summary>
        /// <param name="lastProof">Previous proof</param>
        /// <returns></returns>
        int CalculateProofOfWork(int lastProof);
    }
}