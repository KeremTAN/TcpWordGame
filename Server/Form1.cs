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

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SimpleTcpServer server;
        private static ArrayList allWords = new ArrayList();
        private string ourWord = "";
        private static int counter = 15;
        private void btnStart_Click(object sender, EventArgs e)
        {
            //if (server==null)
            //{
            //    server = new SimpleTcpServer(txtIp.Text);
            //    server.Events.ClientConnected += Events_ClientConnected;
            //    server.Events.ClientDisconnected += Events_ClientDisconnected;
            //    server.Events.DataReceived += Events_DataReceived;

            //}
            server.Start();
            txtInfo.Text += $"Game is starting...{Environment.NewLine}";
            btnStart.Enabled = false;
            btnSend.Enabled=true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
            server = new SimpleTcpServer(txtIp.Text);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (Encoding.UTF8.GetString(e.Data) == "------| TIME IS UP! PLAYER 1 WINS |------")
                {
                    btnSend.Enabled = false;
                    timer1.Stop();
                    lblTimer.Text = "15";
                    txtInfo.Text += $"{Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                }
                else
                {
                    txtInfo.Text += $"PLAYER 2: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
                    allWords.Add(Encoding.UTF8.GetString(e.Data));
                    counter = 15;
                    timer1.Start();
                }
            });
        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} disconnected.{Environment.NewLine}";
                txtClientIP.Text = "";
            });
        }

        private void Events_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            if (txtClientIP.Text == "")
            {
                this.Invoke((MethodInvoker)delegate
                {
                    txtInfo.Text += $"{e.IpPort} connected.{Environment.NewLine}";
                    txtClientIP.Text = e.IpPort;
                });

            }

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (server.IsListening)
            {
                if (!string.IsNullOrEmpty(txtWord.Text) && txtClientIP.Text != "")
                {
                    ourWord = txtWord.Text.ToLower();
                    if (allWords.Count > 0)
                    {
                        if (allWords[allWords.Count - 1].ToString().Substring(allWords[allWords.Count - 1].ToString().Length - 2) == ourWord.Substring(0, 2) && !(allWords.Contains(ourWord)))
                        {
                            lblWarning.Text = "";
                            allWords.Add(ourWord);
                            server.Send(txtClientIP.Text, ourWord); // SEND THE MESSAGE
                            txtInfo.Text += $"PLAYER 1: {ourWord}{Environment.NewLine}";
                            txtWord.Text = string.Empty;
                            timer1.Stop();
                            counter = 15;
                            lblTimer.Text = counter.ToString();
                        }
                        else lblWarning.Text = "This word is not valid! Please, enter an another word.";
                    } //count > 0
                    else
                    {
                        allWords.Add(ourWord);
                        server.Send(txtClientIP.Text, ourWord); // SEND THE MESSAGE
                        txtInfo.Text += $"PLAYER 1: {ourWord}{Environment.NewLine}";
                        txtWord.Text = string.Empty;
                        timer1.Stop();
                        counter = 15;
                        lblTimer.Text = counter.ToString();
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = (--counter).ToString();
            if (counter == 0)
            {
                timer1.Stop();
                btnSend.Enabled = false;
                txtInfo.Text += $"------| TIME IS UP! PLAYER 2 WINS |------{Environment.NewLine}";
                server.Send(txtClientIP.Text, $"------| TIME IS UP! PLAYER 2 WINS |------");
            }

        }
    }
}
