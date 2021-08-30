using SimpleTcp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SimpleTcpClient client;
        private static ArrayList allWords = new ArrayList();
        private string ourWord = "";
        private static int counter = 15;
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (client.IsConnected)
            { 
                if(!string.IsNullOrEmpty(txtWord.Text))
                {
                    ourWord = txtWord.Text.ToLower();
                    if (allWords.Count > 0)
                    {
                        if (allWords[allWords.Count - 1].ToString().Substring(allWords[allWords.Count - 1].ToString().Length - 2) == ourWord.Substring(0, 2) && !(allWords.Contains(ourWord)))
                        {
                            lblWarning.Text = "";
                            allWords.Add(ourWord);
                            client.Send(ourWord);
                            txtInfo.Text += $"PLAYER 2: {ourWord}{Environment.NewLine}";
                            txtWord.Text = string.Empty;
                            timer1.Stop();
                            counter = 15;
                            lblTimer.Text = counter.ToString();
                        }
                        else lblWarning.Text = "This word is not valid! Please, enter an another word.";
                    } // COUNT >0
                    else
                    {
                        allWords.Add(ourWord);
                        client.Send(ourWord);
                        txtInfo.Text += $"PLAYER 2: {ourWord}{Environment.NewLine}";
                        txtWord.Text = string.Empty;
                        timer1.Stop();
                        counter = 15;
                        lblTimer.Text = counter.ToString();
                    }
                }
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {

                // if (client == null)
                // {
                //    client = new SimpleTcpClient(txtIp.Text);
                //    client.Events.Connected += Events_Connected;
                //    client.Events.DataReceived += Events_DataReceived;
                //    client.Events.Disconnected += Events_Disconnected;
                //}
               
                client.Connect();
                btnSend.Enabled = true;
                btnConnect.Enabled = false;
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Your connection proccess is wrong!{Environment.NewLine}Server ip and port format must be '127.0.0.1:9000'");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new(txtIp.Text);
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Events_DataReceived;
            client.Events.Disconnected += Events_Disconnected;
            btnSend.Enabled = false;


        }

        private void Events_Disconnected(object sender, ClientDisconnectedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server disconnected.{Environment.NewLine}";
            });
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (Encoding.UTF8.GetString(e.Data) == "------| TIME IS UP! PLAYER 2 WINS |------")
                {
                    btnSend.Enabled = false;
                    timer1.Stop();
                    lblTimer.Text = "15";
                    txtInfo.Text += $"{Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                }
                else
                {
                    txtInfo.Text += $"PLAYER 1: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                    allWords.Add(Encoding.UTF8.GetString(e.Data));
                    counter = 15;
                    timer1.Start();
                }
            });
        }

        private void Events_Connected(object sender, ClientConnectedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server connected.{Environment.NewLine}";
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = (--counter).ToString();
            if (counter == 0)
            {
                timer1.Stop();
                btnSend.Enabled = false;
                txtInfo.Text += $"------| TIME IS UP! PLAYER 1 WINS |------{Environment.NewLine}";
                client.Send($"------| TIME IS UP! PLAYER 1 WINS |------");
            }
        }
    }
}
