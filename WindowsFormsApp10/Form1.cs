using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp10
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string[] list;
        string ftpUsername = "yourftpusername";
        string ftpPassword = "yourftppassword";
        string ftpserver = @"ftp://yourdomain.com/yourfile/";
        private void Form1_Load(object sender, EventArgs e)
        {
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form_DragEnter);
            this.DragDrop += new DragEventHandler(Form_DragDrop);
        }
        void Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;

        }
        
        void Form_DragDrop(object sender, DragEventArgs e)
        {
           
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            
            foreach (string File in FileList)
                AddListbox(File);
            GetFile();
            
        }

        private void GetFile()
        {
            timer1.Start();
            list = listBox1.Items.OfType<string>().ToArray();
            bool isFileExists = false;
            bool isFileDirectory = false;
            foreach (string items in list)
            {
                if (File.Exists(items))
                {
                    //MessageBox.Show("Dosya mevcut");
                    isFileExists = true;
                }
                else
                {
                    if (Directory.Exists(items))
                    {
                        //MessageBox.Show("Dosya mevcut");
                        isFileExists = true;
                        isFileDirectory = true;
                        GetFileListOnDirectory(items);
                    }
                    else
                    {
                        MessageBox.Show("Dosya mevcut değil. Lütfen mevcut dosya yükleyiniz.");
                        isFileExists = true;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            RunSqlQuery(text);
        }

        private void RunSqlQuery(string text)
        {
            string siteUrl = "http://littlep.xyz/pc/dataverabime/"+text;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(siteUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null )
            {
                UploadFile(listBox1.SelectedItem.ToString());
            }
            else if(listBox1.Items.Count != 0) {
                foreach (string file in list)
                    UploadFile(file);
            }
            
        }


        private void UploadFile(string file)
        {
            string[] name = file.Split('\\');
            MessageBox.Show(name.Last().ToString());
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                client.UploadFile(@ftpserver+name.Last().ToString(), WebRequestMethods.Ftp.UploadFile, @file);
            }
        }

        private void GetFileListOnDirectory(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            foreach (string item in directories)
            {
                if (Directory.Exists(item)) { GetFileListOnDirectory(item); AddListbox(item); }
                else { AddListbox(item); }
            }
            foreach (string file in files)
            {
                if (Directory.Exists(file)) { GetFileListOnDirectory(file); AddListbox(file); }
                else { AddListbox(file); }
            }
            foreach (string items in list)
            {
                AddListbox(items);
            }
        }

        private void AddListbox(string item)
        {
            if (!listBox1.Items.Contains(item))
            {
                listBox1.Items.Add(item);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = listBox1.Items.Count.ToString();
        }
    }
}
