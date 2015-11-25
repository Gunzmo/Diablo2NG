using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using D2Bot;
using System.Diagnostics;
namespace Diablo_2_Next_Generation
{
    public class Network
    {
        #region Vars
        public string ID;
        public string User;
        private string hash = string.Empty;
        private static string key = "wGNuqvkYNrRxKNfGN2cS50oVfTIW1FRAlW0otF3KLxo=";
        private static string iv = "bT31M32Ik5TKkCS/dvasSTJHwcHdKyCZ1ix7LHwVWIA=";
        private string realm;
        public string realmd
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
                            realm = "http://"+line+"/d2ng/";
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
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private int ping = 0;
        #endregion

        #region Network() Construct
        public Network()
        {
            timer.Interval = 3000;
            timer.Tick += timer_tick;
            timer.Enabled = true;
            timer.Start();
        }
        #endregion
       
        #region Hash
        public string Hash
        {
            get
            {
               try
               {
                   if (string.IsNullOrEmpty(hash))
                   {
                       string dec = sendPost(EncryptString("1|" + GetHWID() + "|" + HandlerClass.Instance.Account + "|" + HandlerClass.Instance.Realm));
                       hash = AES_decrypt(dec);
                       HandlerClass.Instance.console.SendToConsole("Hash:" + hash);
                       return hash;
                   }
                   else if (hash != string.Empty)
                   {
                       HandlerClass.Instance.console.SendToConsole("Hash Requested send - Hash:" + hash);
                       return hash;
                   }
                   return string.Empty;
               }
               catch (Exception e) { HandlerClass.Instance.console.SendToConsole(e.ToString() + Environment.NewLine + "Hash:" + hash); return hash; }
            }
           private set { return; }
            
        }
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
        #endregion

        #region Ping
        private void timer_tick(object sender, EventArgs e)
        {
            
            if(HandlerClass.Instance.Runs == HandlerClass.Running.Off)
                hash = string.Empty;
            if(HandlerClass.Instance.screenState == HandlerClass.ScreenState.inGame)
            {
                ping++;
                if(ping >= 40)
                {
                    Ping();
                    ping = 0;
                }
            }
            else
                ping = 0;
        }
        private bool Ping()
        {
            try
            {
                HandlerClass.Instance.console.SendToConsole("Ping! " + AES_decrypt(sendPost(EncryptString("6|" + Hash))));
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region Create Game
        public bool CreateGame(string game, string pass, HandlerClass.dif diff, Chars Char, string desc, string region)
        {
            try
            {
                int lad = 0;
                if (Char.Lad == "LADDER CHARACTER")
                    lad = 1;
                string hold;
                string enc = EncryptString("2|" + game + "|" + pass + "|" + diff.ToString() + "|" + Char.Name + " [" + Char.ClassLevel + "]" + "|" + lad + "|" + desc + "|" + region + "|" + Hash);
                hold = "2|" + game + "|" + pass + "|" + diff.ToString() + "|" + Char.Name + "|" + desc + "|" + region + "|" + Hash;
                hold = AES_decrypt(sendPost(enc));
                HandlerClass.Instance.console.SendToConsole(hold);
                return true;
            }
            catch {return false;}
            
        }
        #endregion

        #region JoinGame
        public bool JoinGame(int id, Chars Char)
        {
            try
            {
                sendPost(EncryptString("5|" + id + "|" + Char.Name + " [" + Char.ClassLevel + "]" + "|" + Hash));
                return true;
            }
            catch { return false; }

        }
        #endregion

        #region RequestToRemove
        public bool RequestToRemove()
        {
            try
           {
                sendPost(EncryptString("3|" + Hash));
                return true;
           }
           catch { return false; }

        }
        #endregion

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

        #region RequestGameList
        public string RequestGameList(int mode, Chars player, string region)
        {
            int lad = 0;
            if (player.Lad == "LADDER CHARACTER")
                lad = 1;
            string hold = sendPost(EncryptString("4|" + hash + "|" + lad + "|" + region + "|" + mode + "|" + HandlerClass.Instance.FilterDiff));
            string[] egame = hold.Split('|');
            string dgame = string.Empty;
            for (int i = 0; i < egame.Length; i++ )
            {
                dgame += AES_decrypt(egame[i]) + "|";
            }
                return dgame;
        }
        #endregion

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
    }
}
