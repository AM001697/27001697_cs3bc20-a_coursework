using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainAssignment
{
    class Blockchain
    {
        private const int TRANSACTIONS_PER_BLOCK = 5;
        private int difficulty = 4;
        private const double IDEAL_TIME_TO_MINE = 1;
        private List<Block> Blocks;
        private List<Transaction> PendingTransactions;
        

        public Blockchain()
        {
            Blocks = new List<Block>();
            PendingTransactions = new List<Transaction>();
            Blocks.Add(new Block());
        }


        public String GetBlockInfo(int index)
        {
            if (0 <= index && index < Blocks.Count)
                return Blocks[index].GetInfo();
            else
                return "\n[ERR] Block not found\n";
        }

        public String GetAllBlockInfo()
        {
            String temp = "";
            foreach (Block block in Blocks)
            {
                temp += block.GetInfo();
            }
            return temp;
        }

        public Block GetLastBlock()
        {
            return Blocks[Blocks.Count - 1];
        }

        public Block GetWrongLastBlock()
        {
            if (Blocks.Count >= 2) return Blocks[Blocks.Count - 2];
            else return Blocks[0];
        }

        private int CalculateNewDifficulty()
        {
            if (Blocks.Count > 4 && (Blocks.Count % 5 == 0))
            {
                double totalTime = 0;
                for (int i = Blocks.Count-1; i > Blocks.Count-6; i--){
                    totalTime += Blocks[i].timeToMine.TotalSeconds;
                    totalTime += Blocks[i].timeToMine.TotalMilliseconds / 1000;
                }


                totalTime = Convert.ToDouble(totalTime.ToString().Substring(0,4));


                double ratio = (5 * IDEAL_TIME_TO_MINE) / totalTime;

                int new_difficulty = difficulty;
                if (ratio >= 1.5) new_difficulty = difficulty + 1;
                else if (ratio <= 0.5 && difficulty > 2) new_difficulty = difficulty - 1;

                //int new_difficulty = (int)(difficulty * ratio);

                Console.WriteLine("New difficulty: " + new_difficulty);

                return new_difficulty;
            }

            

            return difficulty;
        }

        public void AddBlock(String public_hash, int option)
        {

            List<Transaction> temp_trans = new List<Transaction>(new Transaction[(TRANSACTIONS_PER_BLOCK + 1)]);
            if (PendingTransactions.Count() > 5)
            {
                if (option == 1)          PendingTransactions = PendingTransactions.OrderBy(o => o.Timestamp).ToList();

                else if (option == 2)     PendingTransactions = PendingTransactions.OrderBy(o => o.Fee).ToList();

                else if (option == 3)     PendingTransactions = PendingTransactions.OrderBy(o => { return (o.SenderAddress == public_hash); }).ToList();

                else if (option == 4)     PendingTransactions = PendingTransactions.OrderBy(o => Guid.NewGuid()).ToList();

                else   return;
                
                for (int i = 0; i < TRANSACTIONS_PER_BLOCK; i++)
                {
                    Transaction trans = PendingTransactions[0];

                    temp_trans[i] = trans;
                    PendingTransactions.RemoveAt(0);
                }

            }
            else
            {
                temp_trans = PendingTransactions.ToList<Transaction>();
                PendingTransactions.Clear();
            }

            difficulty = CalculateNewDifficulty();

            Block new_block = new Block(GetLastBlock(), temp_trans, public_hash, difficulty);

            if (new_block.GetMerkle() == new_block.merkleroot)
            {
                Blocks.Add(new_block);
            }


        }

        public void AddBrokenBlock(String public_hash, int option)
        {

            List<Transaction> temp_trans = new List<Transaction>(new Transaction[(TRANSACTIONS_PER_BLOCK + 1)]);
            if (PendingTransactions.Count() > 5)
            {
                if (option == 1) PendingTransactions = PendingTransactions.OrderBy(o => o.Timestamp).ToList();

                else if (option == 2) PendingTransactions = PendingTransactions.OrderBy(o => o.Fee).ToList();

                else if (option == 3) PendingTransactions = PendingTransactions.OrderBy(o => { return (o.SenderAddress == public_hash); }).ToList();

                else if (option == 4) PendingTransactions = PendingTransactions.OrderBy(o => Guid.NewGuid()).ToList();

                else return;

                for (int i = 0; i < TRANSACTIONS_PER_BLOCK; i++)
                {
                    Transaction trans = PendingTransactions[0];

                    temp_trans[i] = trans;
                    PendingTransactions.RemoveAt(0);
                }

            }
            else
            {
                temp_trans = PendingTransactions.ToList<Transaction>();
                PendingTransactions.Clear();
            }

            difficulty = CalculateNewDifficulty();

            Block new_block = new Block(GetWrongLastBlock(), temp_trans, public_hash, difficulty);

            if (new_block.GetMerkle() == new_block.merkleroot)
            {
                Blocks.Add(new_block);
            }


        }

        public void AddTransaction(Transaction transaction)
        {
            PendingTransactions.Add(transaction);
        }

        public String ShowPendingTransaction(int index)
        {
            if (0 <= index && index < PendingTransactions.Count)
                return PendingTransactions.ToList<Transaction>()[index].GetInfo();
            else
                return "\n[ERR] Transaction not found\n";
        }

        public String ShowAllPendingTransactions()
        {
            String temp = "";
            foreach (Transaction trans in PendingTransactions.ToList<Transaction>())
            {
                temp += trans.GetInfo();
            }
            return temp;
        }

        public long GetBalance(String pubKey)
        {
            long Balance = 0;
            foreach (Transaction trans in PendingTransactions)
            {
                Balance += trans.GetBalance(pubKey);
            }
            foreach (Block block in Blocks)
            {
                Balance += block.GetBalance(pubKey);
            }


            return Balance;
        }

        public bool CheckBlocks()
        {
            bool flag = false;

            for (int i = 1; i < (Blocks.Count - 1); i++)
            {
                if (Blocks[i].previousHash != Blocks[i - 1].hash)
                {
                    flag = true;
                }
            }

            return flag;
        }

        public bool CheckBlocksMerkle()
        {
            bool flag = false;
            for (int i = Blocks.Count - 1; i >= 0; i--)
            {
                if (Blocks[i].GetMerkle() != Blocks[i].merkleroot)
                {
                    //Blocks.RemoveAt(i);
                    flag = true;
                }

            }
            return flag;
        }

        public bool CheckTransactionSignature()
        {
            bool flag = false;

            for (int i = Blocks.Count - 1; i >= 0; i--)
            {
                if (Blocks[i].CheckTransactionSignature())
                {
                    //Blocks.RemoveAt(i);
                    flag = true;
                }
            }

            return flag;
        }
    }
}
