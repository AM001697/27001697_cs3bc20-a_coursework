using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    /// <summary>
    /// Class to represent transactinos between different wallets
    /// </summary>
    class Transaction
    {
        public String Hash;             // Represents the main hash generated from info
        public String Signature;            // Signature generated from hash, sender and reciever
        public String SenderAddress;          // Wallet sending funds
        public String RecipientAddress;       // Wallet recieving funds
        public DateTime Timestamp;          // Time transaction was generated
        public ulong Amount;               // Amount of funds transacted
        public ulong Fee;                  // Fee to use blockchain, deducted from sender


        public Transaction(String SenderAddress, String privKey, String RecipientAddress, ulong Amount, ulong Fee)
        {
            this.SenderAddress = SenderAddress;                     // Allocate all the classes variables
            this.RecipientAddress = RecipientAddress;
            this.Amount = Amount;
            this.Fee = Fee;
            this.Timestamp = DateTime.Now;

            this.Hash = this.CreateHash();

            this.Signature = Wallet.Wallet.CreateSignature(this.SenderAddress, privKey, this.Hash);
        }

        public Transaction(String SenderAddress, String RecipientAddress, ulong Amount, ulong Fee)
        {
            this.SenderAddress = SenderAddress;                     // Allocate all the classes variables
            this.RecipientAddress = RecipientAddress;
            this.Amount = Amount;
            this.Fee = Fee;
            this.Timestamp = DateTime.Now;

            this.Hash = this.CreateHash();

            this.Signature = Wallet.Wallet.CreateSignature(this.SenderAddress, RecipientAddress, this.Hash);
        }

        /// <summary>
        /// Function which takes the class variables and returns a hash from them
        /// </summary>
        /// <returns>Hash from the objects attributes</returns>
        public String CreateHash()
        {
            SHA256 hasher;
            hasher = SHA256Managed.Create();  // Declare and set the hasher tool
            String input = this.SenderAddress + this.RecipientAddress + this.Timestamp.ToString() + this.Amount.ToString() + this.Fee.ToString();   // Combine pre set attributes to one string
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes((input)));                                                          // Use hasher object to create a hash

            String hash = string.Empty;

            foreach (byte x in hashByte)                    // Allocate hash bytes to a string
            {
                hash += String.Format("{0:x2}", x);
            }
            return hash;                                // Return the hash as a string
        }

        /// <summary>
        /// Returns all the objects attributes as a single string 
        /// </summary>
        /// <returns> String of all object vaiables pre formatted</returns>
        public String GetInfo()
        {
            return "\nTransaction Hash: " + this.Hash + "\nDigitalSignature: " + 
                this.Signature + "\nTimestamp: " + 
                this.Timestamp.ToString() + "\nTransfered : " + 
                this.Amount + " RobCoins\nFees: "  + this.Fee + "\nSenderAddress: " + 
                this.SenderAddress + "\nRecieverAddress" + this.RecipientAddress + "\n";
        }

        /// <summary>
        /// Checks if the pubkey is in the transaction and returns the funds transacted in this transaction 
        /// </summary>
        /// <param name="pubKey"></param>
        /// <returns>long of the funds gained or lost</returns>
        public long GetBalance(String pubKey)
        {
            if (pubKey == SenderAddress)        // if the sender is the pubkey, return the funds lost
            {
                return (-1 * (long)(Amount + Fee));
            } else if (pubKey == RecipientAddress)      // if the reciever is the pubkey, return the funds gained in the transaction
            {
                return (long)Amount;
            } else
            {
                return 0;               // if neither sender of reciever, return 0
            }
        }

        public long GetFees()
        {
            return (long)(Fee);
        }

        /// <summary>
        /// Check the transactions signature
        /// </summary>
        /// <returns>boolean, true if the transactions hashes do not match </returns>
        public bool CheckTransactionSignature()
        {
            if (Wallet.Wallet.ValidateSignature(SenderAddress, Hash, Signature))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
