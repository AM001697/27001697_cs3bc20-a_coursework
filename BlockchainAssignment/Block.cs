using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    /// <summary>
    /// Object representing each block in the blockchain
    /// </summary>
    class Block
    {
        private const int TRANSACTIONS_PER_BLOCK = 5;             // Max number of pre assigned transactions allowed
        private List<Transaction> transactions;       // List of transaction objects
        private DateTime timestamp;                 // Timestamp of when this block was created
        public int index;                       // Index, representing the numbered block this is
        public String hash;                         // Hash representing the blocks info
        public String previousHash;                // Hash of the previous block
        private String miner;
        private uint nonce;                   
        private int DIFFICULTY;            // How many 0's a hash must contain
        private int reward;                      // Number of funds awarded to the miner
        public String merkleroot;                   // Hash representing all hashes from transactions 
        public TimeSpan timeToMine;


        public Block()
        {
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[sizeof(uint)];
            rngCsp.GetBytes(buffer);
            nonce =  BitConverter.ToUInt32(buffer, 0);



            DIFFICULTY = 4;
            Mine(out hash);
            timestamp = DateTime.Now;           // Constructor for the gensis block
            index = 0;
            previousHash = "";
            
            merkleroot = GetMerkle();
        }

        public Block(Block lastBlock, List<Transaction> input_transactions, String miner_hash, int difficulty)          // Constructor for other blocks in the chain 
        {
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[sizeof(uint)];
            rngCsp.GetBytes(buffer);
            nonce = BitConverter.ToUInt32(buffer, 0);

            DIFFICULTY = difficulty;
            reward = 5;
            timestamp = DateTime.Now;
            index = lastBlock.index + 1;
            previousHash = lastBlock.hash;                  // Set attributes
            miner = miner_hash;
 
            this.Mine(out hash);

            if (input_transactions.Count > TRANSACTIONS_PER_BLOCK)          // Check the list of input transactions
            {
                input_transactions.RemoveRange(TRANSACTIONS_PER_BLOCK, input_transactions.Count - TRANSACTIONS_PER_BLOCK);      // Remove excess transactions
            }


            Transaction transaction = new Transaction("Mine Rewards", miner_hash, (ulong)(reward), 0);          // Create reward transaction for the miner
            input_transactions.Add(transaction);

            transactions = input_transactions;

            merkleroot = GetMerkle();           // Calculate Merkle root
        }

        /// <summary>
        /// Generates a hash from all attributes provided in the object
        /// </summary>
        /// <returns>String of the hash</returns>
        private String CreateHash()
        {
            SHA256 hasher;                      // Create hasher object
            hasher = SHA256Managed.Create();
            String input = index.ToString() + timestamp.ToString() + previousHash + nonce + reward;         // combine all relevant info into one string for hashing
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes((input)));                       // create the hash

            String hash = string.Empty;

            foreach (byte x in hashByte)
            {
                hash += String.Format("{0:x2}", x);             // convert the hash to a string
            }
            return hash;

        }

        /// <summary>
        /// Function for the miner to run to generate a suitable hash for the object
        /// </summary>
        /// <returns>String for the hash</returns>
        private void Mine(out String temp_hash)
        {
            String thread_hash = null;
            String temp = "";
            object thread_nonce = nonce; 

            IEnumerable<bool> Infinite()
            {
                while (true) yield return true;
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            
            Parallel.ForEach(Infinite(), (i, state) =>
            {



                thread_hash = CreateHash();
                if (thread_hash.Substring(0, DIFFICULTY) == (String.Concat(Enumerable.Repeat("0", DIFFICULTY))))
                {
                    temp = thread_hash;
                    state.Break();
                }

                lock (thread_nonce)
                    nonce++;

            });
            /*
            while (true) {
                thread_hash = CreateHash();
                if (thread_hash.Substring(0, DIFFICULTY) == (String.Concat(Enumerable.Repeat("0", DIFFICULTY))))
                {
                    temp = thread_hash;
                    break;
                }

                nonce++;
            }*/

            stopWatch.Stop();

            temp_hash = temp;

            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = ts.Seconds.ToString() +"."+  (ts.Milliseconds / 10).ToString();
            Console.WriteLine("RunTime: " + elapsedTime);

            timeToMine = ts;
        }

        /// <summary>
        /// Shows all the info (pre formatted) about the object
        /// </summary>
        /// <returns>Formatted string of the attributes of the block</returns>
        public String GetInfo()
        {
            String temp = "";
            if (transactions != null)           // Add all the transactions info to the output
            {
                foreach (Transaction transaction in transactions)
                    temp += transaction.GetInfo();
            }
            
           
            
            return "\nBlockIndex: " + this.index  + 
                "\tTimestamp: " + this.timestamp.ToString() + 
                "\nHash: " + this.hash + "\nPrevious Hash: " + 
                this.previousHash + "\n" + temp + "\nNonce: " + 
                nonce.ToString() + "\nDifficulty: " + DIFFICULTY + 
                "\nMerkle Root: " + merkleroot + "\n"; 
        }

        /// <summary>
        /// Finds the funds gained or lost to a specific wallet in the bloacks transactions
        /// </summary>
        /// <param name="pubKey"></param>
        /// <returns>long of the amount gained or lost</returns>
        public long GetBalance(String pubKey)
        {
            if (transactions == null) {
                return 0;
            }
            
            long Balance = 0;
            foreach (Transaction trans in transactions)  // loop through transactions and find the funds gained/lost
            {
                Balance += trans.GetBalance(pubKey);
            }

            if (miner == pubKey)
            {
                foreach (Transaction trans in transactions)  // loop through transactions and find the funds gained/lost
                {
                    Balance += trans.GetFees();
                }
            }

            return Balance;
        }


        /// <summary>
        /// Callable function which takes the relevant info and feeds it into the other merkle function
        /// to get a required result
        /// </summary>
        /// <returns></returns>
        public String GetMerkle()
        {
            if (transactions == null)
            {
                return "";
            }
            List<String> hashes = new List<String>();               // Add blocks transactions to the list
            foreach (Transaction transaction in transactions)
            {
                hashes.Add(transaction.Hash);
            }
            List<String> merkle = CalculateMerkle(hashes);          // Calculate merkle from list

            return merkle[0]; // Calculate merkale should return a list with one element when recursion is over
        }

        /// <summary>
        /// Recursive function to calculate merkle root
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns>String List of the Merkle root</returns>
        private static List<String> CalculateMerkle(List<String> hashes)
        {
            if (hashes.Count == 1)          // If one element given, end recursion and return hash to above layer
            {
                List<String> hash = new List<string>();
                hash.Add(hashes[0]);
                return hash;
            }
            List<String> parentHashes = new List<string>();
            for(int i = 0; i < hashes.Count()-1; i += 2){
                String hashedString = HashCode.HashTools.combineHash(hashes[i], hashes[i+1]);           // Create a list of hashes which is halved and combined
                parentHashes.Add(hashedString);
            }
            if (hashes.Count() % 2 == 1)
            {
                String lastHash = hashes[hashes.Count() - 1];               // Add last hash if it is there
                parentHashes.Add(lastHash);
            }
            return CalculateMerkle(parentHashes);           // Call function with parent hash
        }

        /// <summary>
        /// Checks if any transactions have faults in the hashes
        /// </summary>
        /// <returns></returns>
        public bool CheckTransactionSignature()
        {

            bool flag = false;

            if (transactions == null) return flag;

            foreach (Transaction trans in transactions)
            {
                if (trans.CheckTransactionSignature())
                {
                    flag = true;
                }
            }

            return flag;
        }

    }
}
