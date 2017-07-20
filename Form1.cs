using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;           //匯入網路通訊協定相關函數
using System.Net.Sockets;   //匯入網路插座功能函數
using System.Threading;     //匯入多執行緒功能函數

namespace ClientAP
{
    public partial class Form1 : Form
    {
        //公用變數
        Socket T;           //通訊物件
        string User;        //使用者
        Thread DATA_receive;

        public Form1()
        {
            InitializeComponent();
            Form1.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //傳送訊息給 Server (Send Message to the Server)
        
        private void Button1_Click(object sender, EventArgs e)
        {

            string IP = TextBox1.Text;                                  //伺服器IP
            int Port = int.Parse(TextBox2.Text);                        //伺服器Port
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port);  //伺服器的連線端點資訊
            //建立可以雙向通訊的TCP連線
            T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            User = TextBox3.Text;  

            try
            {
                T.Connect(EP);                  //連上伺服器的端點EP(類似撥號給電話總機)
                DATA_receive = new Thread(Listen);
                DATA_receive.IsBackground = true;
                DATA_receive.Start();
                byte[] B = Encoding.Default.GetBytes("i"+User);
                T.Send(B);               //連線後隨即傳送自己的名稱給伺服器
            }
            catch (Exception)
            {
                MessageBox.Show("無法連上伺服器！"); //連線失敗時顯示訊息
                return;
            }

            Button1.Enabled = false; //讓連線按鍵失效，避免重複連線
            Button3.Enabled = true;
        }

        private void Listen()
        {
            while (true)
            {
                    try
                    {
                        byte[] B = new byte[1023];
                        int inLen = T.Receive(B);
                        string Msg = Encoding.Default.GetString(B, 0, inLen);
                        listBox1.Items.Add(Msg);
                    }
                    catch (Exception)
                    {

                    } 
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Button1.Enabled == false)
            {
                byte[] B = Encoding.Default.GetBytes("o" + User);
                T.Send(B);  //傳送自己的離線訊息給伺服器
                T.Close(); //關閉網路通訊器T
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (Button1.Enabled == false)
            {
                byte[] B = Encoding.Default.GetBytes("o" + User);
                T.Send(B); //傳送自己的離線訊息給伺服器
                T.Close();        //關閉網路通訊器T
                Button1.Enabled = true;
                Button3.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Button1.Enabled == false)
            {
                byte[] B = Encoding.Default.GetBytes(User.Length + User + textBox4.Text);
                T.Send(B);
                textBox4.Text = "";
            }
        }
    }
}
