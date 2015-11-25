using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

using D2NGBotHelper;
using System.Security.Cryptography;
using System.Net;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public string ID;
        public string User;
        private string hash = string.Empty;
        private static string key = "wGNuqvkYNrRxKNfGN2cS50oVfTIW1FRAlW0otF3KLxo=";
        private static string iv = "bT31M32Ik5TKkCS/dvasSTJHwcHdKyCZ1ix7LHwVWIA=";
        private string realm;
        int lad = 0;
        private string msg;
        private string[] MSG;
        int region = -1;
        private string realmd
        {
            get
            {
                if (string.IsNullOrEmpty(realm))
                {
                    try
                    {   // Open the text file using a stream reader.
                        using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "/realm.d2"))
                        {
                            // Read the stream to a string, and write the string to the console.
                            String line = sr.ReadToEnd();
                            realm = "http://" + line + "/d2ng/";
                            return realm;
                        }
                    }
                    catch
                    {
                        return realm;
                    }
                }
                else
                    return realm;
            }
        }
        private string Hash
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(hash))
                    {
                        string tmp = EncryptString("1|" + GetHWID() + "|" + "BoT" + "|" + 0);
                        string dec = sendPost(tmp);
                        hash = AES_decrypt(dec);
                        return hash;
                    }
                    else if (hash != string.Empty)
                    {
                        return hash;
                    }
                    return string.Empty;
                }
                catch { return "Unable to Get Hash, Wrong Realm or Server Down!"; }
            }
            set { return; }

        }
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {

            if (m.Msg == 74)
            {
                Type type = new MessageHelper.COPYDATASTRUCT().GetType();
                MessageHelper.COPYDATASTRUCT copydatastruct = (MessageHelper.COPYDATASTRUCT)m.GetLParam(type);

                try
                {
                    IntPtr num = new IntPtr(m.WParam.ToInt32());
                    string str = string.Copy(copydatastruct.lpData);

                    HandleMessage(num, str);

                }
                catch
                {
                }
                m.Result = (IntPtr)1;
            }
            base.WndProc(ref m);
        }

        private void HandleMessage(IntPtr hWnd, string message)
        {
            msg = message;
            msg = msg.Replace("\"", "");
            MSG = msg.Split(';');
            if(MSG[0] == "1")
            {
                //EncryptString("2|" + game + "|" + pass + "|" + diff.ToString() + "|" + Char.Name + " [" + Char.ClassLevel + "]" + "|" + lad + "|" + desc + "|" + region + "|" + Hash);
                sendPost(EncryptString("7|" + MSG[1] + "|" + MSG[2] + "|" + MSG[3] + "|" + MSG[4] + "|" + lad + "|" + "" + "|" + region + "|" + Hash));
            }
            if (MSG[0] == "2")
            {
               sendPost(EncryptString("3|" + Hash));
            }
        }

        private bool Ping()
        {
            try
            {
                sendPost(EncryptString("6|" + Hash));
                return true;
            }
            catch { return false; }
        }

        #region sendPostWeb
        /*
         * Senting Get responce to server to get and send games to server
         */
        private string sendPost(string postData)
        {
            WebRequest request = WebRequest.Create(realmd + "index.php");
            request.Method = "POST";
            request.Timeout = 999999;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "text/plain";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }
        #endregion

        
        private string GetHWID()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == "")
                {
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            return cpuInfo;
        }   

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Hash;
        }


        #region Crypts
        private static string EncryptString(string Input)
        {
            var aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 256;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Convert.ToBase64String(xBuff);
            return Output;
        }

        private String AES_decrypt(string Input)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);
                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }
                    xBuff = ms.ToArray();
                }
                String Output = Encoding.UTF8.GetString(xBuff);
                return Output;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            Ping();
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                lad = 1;
            else
                lad = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "Europe":
                    realm = "3";
                    break;
                case "U.S. West":
                    realm = "0";
                    break;
                case "U.S. East":
                    realm = "1";
                    break;
                case "Asia":
                    realm = "2";
                    break;
            }
        }
    }

   
}
