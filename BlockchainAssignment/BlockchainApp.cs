using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockchainAssignment
{
    public partial class BlockchainApp : Form
    {
        Blockchain blockchain;

        public BlockchainApp()
        {
            InitializeComponent();
            blockchain = new Blockchain();                          // start blockchain
            richTextBox1.Text = "[*] New Blockchain Initialized\n";   // opening message


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // print specific block button function
            try
            {
                int number = Int32.Parse(textBox1.Text);
                richTextBox1.Text += this.blockchain.GetBlockInfo(number);        // call blockchain function
            }
            catch (FormatException)         // only if number given doesn't work in parse method
            {
                richTextBox1.Text += "\n[ERR] Unable to parse number given\n";
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Add wallet button function

            String privKey;
            Wallet.Wallet myNewWallet = new Wallet.Wallet(out privKey);     // generate wallet
            String publicKey = myNewWallet.publicID;                        // set local pubkey variable to show

            textBox3.Text = privKey;            // add pubkey and privkey to interface text boxes
            textBox2.Text = publicKey;

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // validate keys button function

            if (Wallet.Wallet.ValidatePrivateKey(textBox3.Text, textBox2.Text))         // check if the keys are valid
                richTextBox1.Text += "\nKeys are valid\n";
            else
                richTextBox1.Text += "\nKeys are not valid\n";                      // output messages to users
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Create transaction button function

            if (String.IsNullOrEmpty(textBox2.Text) ||
                String.IsNullOrEmpty(textBox6.Text) ||                  // Check all input boxes are filled
                String.IsNullOrEmpty(textBox4.Text) ||
                String.IsNullOrEmpty(textBox5.Text))
            {
                richTextBox1.Text += "\n[ERR] Please fill in required boxed\n";         // Reports error
                return;
            }

            if (!(Wallet.Wallet.ValidatePrivateKey(textBox3.Text, textBox2.Text)))              // Checks if keys are valid
            {
                richTextBox1.Text += "\n[ERR] Keys are not valid\n";                        // Reports error
                return;
            }
            blockchain.CheckBlocksMerkle();               // Check the blockchains hashes via merkle roots and deleting blocks with broken hashes


            if (blockchain.GetBalance(textBox2.Text) < (long.Parse(textBox4.Text) + long.Parse(textBox5.Text))){                // Check the sender has enough funds to carry out the transaction
                richTextBox1.Text += "\n[ERR] Not enough funds to make transaction\n";                                  // Reports funds issue
                return;
            }


            Transaction transaction = new Transaction(textBox2.Text, textBox6.Text, ulong.Parse(textBox4.Text), ulong.Parse(textBox5.Text));        // Create transaction from input text blocks
            richTextBox1.Text += transaction.GetInfo();             // Show info about the transaction
            blockchain.AddTransaction(transaction);               // Add transaction to the blockchain, where it'll sit in the pending queue
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // print specific transaction info
            try
            {
                int number = Int32.Parse(textBox7.Text);                                            // gets input and passes it to the blockchain to get info
                richTextBox1.Text += this.blockchain.ShowPendingTransaction(number);
            }
            catch (FormatException)
            {
                richTextBox1.Text += "\n[ERR] Unable to parse number given\n";                     // if a parse error occurs
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox2.Text))
            {
                richTextBox1.Text += "\n[ERR] Missing public key\n";            // Report error
                return;
            } else if (!(radioButton1.Checked ||
                radioButton2.Checked ||
                radioButton3.Checked||
                radioButton4.Checked)){
                richTextBox1.Text += "\n[ERR] Missing mining choice";
            }

            var t = Task.Run(() =>
            {
                if (radioButton1.Checked)
                {
                    // Generate new block button function

                    blockchain.AddBlock(textBox2.Text, 1);         // Add block to blockchain from pending transactions, takes public key for reward

                }
                else if (radioButton2.Checked)
                {
                    blockchain.AddBlock(textBox2.Text, 2);
                }
                else if (radioButton3.Checked)
                {
                    blockchain.AddBlock(textBox2.Text, 3);
                }
                else if (radioButton4.Checked)
                {
                    blockchain.AddBlock(textBox2.Text, 4);
                }
            });
            

            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Print all blocks info
            richTextBox1.Text += blockchain.GetAllBlockInfo();

        }

        private void button8_Click(object sender, EventArgs e)
        {
            // clear the box button function
            richTextBox1.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            // print wallet ballance button function

            if (String.IsNullOrEmpty(textBox2.Text) ||                      // check if the textbox is empty
                String.IsNullOrEmpty(textBox3.Text))
            {
                richTextBox1.Text += "\n[ERR] Missing public key\n";        // output that keys are missing

            }
            else
            {
                String pubKey= textBox2.Text;       // get public key from interface
                richTextBox1.Text += "Public key: " + pubKey + "\nBalance: " + blockchain.GetBalance(pubKey).ToString() + "\n";             // get balance and print
            }
        }



        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            // print all pending transactions info
            richTextBox1.Text += this.blockchain.ShowAllPendingTransactions();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (blockchain.CheckBlocks()) richTextBox1.Text += "\nBlocks are not coherent\n";
            else if (blockchain.CheckBlocksMerkle()) richTextBox1.Text += "\nTransactions have been tampered with\n";
            else if (blockchain.CheckTransactionSignature()) richTextBox1.Text += "\nTransaction signatures have been tampered with\n";
            else richTextBox1.Text += "\nBlocks are coherent\n";
        }
    }
}
