using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainLibrary;
using BlockchainServer.DTO;
using Microsoft.AspNetCore.Mvc;
using ChainDTO = BlockchainServer.DTO.ChainDTO;

namespace BlockchainServer.Controllers
{
    [Route("/[controller]")]
    public class AppController : Controller
    {
        private readonly IBlockchain _blockchain;
        private readonly Guid _nodeIdentifier = Guid.NewGuid();

        public AppController(IBlockchain blockchain)
        {
            _blockchain = blockchain;
        }

        // GET 
        [Route("/home")]
        [HttpGet]
        public string Index()
        {
            return "All is ok";
        }

        // GET /mine
        [Route("/mine")]
        [HttpGet]
        public IActionResult Mine()
        {
            // ищем новый prof of work
            var lastBlock = _blockchain.LastBlock;
            var lastProof = lastBlock.Proof;
            var newProof = _blockchain.CalculateProofOfWork(lastProof);

            // получаем награду за найденный proof
            // если sender = 0, то узел заработал биткойн
            _blockchain.CreateNewTransaction(new[] {Convert.ToByte(0)}, _nodeIdentifier.ToByteArray(), 1);

            // формируем новый блок путем добавления его в цепочку
            var block = _blockchain.CreateNewBlock(newProof);
            var response = new MineDTO
            {
                Message = "New Block Forged",
                Index = block.Index,
                Transactions = block.Transactions,
                Proof = block.Proof,
                PreviousHash = block.PreviousHash
            };
            return Json(response);
        }

        // Get /chain
        [Route("/chain")]
        [HttpGet]
        public IActionResult GetFullChain()
        {
            var response = new ChainDTO
            {
                Chain = _blockchain.Chain,
                Lngth = _blockchain.Chain.Count
            };
            return Json(response);
        }

        // Post /transaction/new
        [Route("/transaction/new")]
        [HttpPost]
        public IActionResult CreateNewTransaction([FromBody] TransactionDTO transaction)
        {
            if(transaction == null) return BadRequest("Missing values");
            // проверяем что есть все параметры
            if (transaction.Sender == null || transaction.Recipient == null) return BadRequest("Missing values");

            // создаем новую транзакцию
            var newIndex = _blockchain.CreateNewTransaction(transaction.Sender, transaction.Recipient, transaction.Amount);
            return Ok($"Transaction will be added to Block {newIndex}");
        }

        // Get /nodes/resolve
        [Route("/nodes/resolve")]
        [HttpGet]
        public IActionResult ReachConsensus()
        {
            var isReplaced = _blockchain.ResolveConflicts();
            var response = new ResolvedConflictsDTO();
            if (isReplaced)
            {
                response.Message = "Our chain was replaced";
                response.Chain = _blockchain.Chain;
            }
            else
            {
                response.Message = "Our chain was authoritative";
                response.Chain = _blockchain.Chain;
            }

            return Json(response);
        }

        // POST /nodes/register
        [Route("/nodes/register")]
        [HttpPost]
        public IActionResult RegisterNodes([FromBody] IEnumerable<string> nodes)
        {
            if (nodes == null)
                return BadRequest("Error: Please supply a valid list of nodes");
            foreach (var node in nodes)
                _blockchain.RegisterNode(node);
            var result = new TotalNodesDTO
            {
                Message = "New nodes have been added",
                Nodes = _blockchain.Nodes.ToList()
            };
            return Json(result);
        }
    }
}