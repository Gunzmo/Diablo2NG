using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2Bot;
using Microsoft.Win32;

using System.Runtime.InteropServices;
using System.Threading;
using System.Collections;
using Newtonsoft.Json;
using System.Drawing.Text;
using System.Drawing;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Diablo_2_Next_Generation
{
    public class HandlerClass
    {
        #region Shit Load of vars
        public enum dif
        {
            Normal = 0,
            Nightmare = 1,
            Hell = 2
        }
        public bool Creator = false;
        public string D2Title;
        public int HeartBeat = 0;
        public bool WasIngame = false;
        public string hash;
        public dif Diffeculty;
        public bool MakeGame = true;
        public bool SetUiCharacters = false;
        public bool ReadOnece = false;
        public int checkLad = 0;
        public bool SendPing = false;
        public int SelectedPlayer = 0;
        public string Account;
        public string Password;
        public string Realm;
        public bool consoleCheck;
        public bool SetUILogin = true;
        public bool SetUiLobby = true;
        public int GameList = 0;
        public int gameRefreshWait = 0;
        public bool SendfromIngameOnce = false;
        public List<Chars> Characters = new List<Chars>();
        public List<Games> Games = new List<Games>();
        public List<BnetGames> Bnetgame = new List<BnetGames>();
        public Console console = new Console();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public List<ProfileBase> bpro = new List<ProfileBase>();
        public string MSG;
        public bool AddToList = false;
        public System.Drawing.FontFamily ff;
        public System.Drawing.Font font;
        public string FilterDiff = string.Empty;
        D2Profile d2profile = new D2Profile();

        public enum ScreenState
        {
            MainMenu,
            RealmMenu,
            PleaseWait,
            Login,
            CharScreen,
            Lobby,
            inGame,
            GameExists,
            CDKeyInUse,
            RealmDown
        }
        public ScreenState screenState;
        public ScreenState OldscreenState;
        public enum Running
        {
            Off,
            On
        }
        public Running Runs;
        //int state;
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
  
        enum ShowWindowCommands
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window 
            /// for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>       
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value 
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except 
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position. 
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level 
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is 
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position. 
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
            /// that owns the window is not responding. This flag should only be 
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }
         private static HandlerClass handler;
        public bool SetProfile
         {
             get
             {
                 try
                 {
                     Random rng = new Random();
                     D2Title = rng.Next(10000000, 99999999).ToString();
                     d2profile.Account = Account;
                     d2profile.Password = Password;
                     d2profile.Realm = Realm;
                     d2profile.BlockRD = true;
                     d2profile.Name = D2Title;
                    // D2Bot.Program.GM = new Main();
                     using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
                         d2profile.D2Path = reg.GetValue("Diablo 2 Path").ToString();
                     d2profile.Difficulty = "hell";
                     if (Settings[2] == "True")
                         d2profile.Parameters = "-w";
                     bpro.Add(d2profile); 
                     return true;
                 }
                 catch { return false; }
             }
         }



        /// <summary>
        /// 1 Hide Screen;
        /// 2 Show Screen;
        /// 3 Set Window Mode;
        /// 4 Set FullScreen Mode;
        /// 5 Focus Screen;
        /// </summary>
        /// <param name="Setting"></param>
        public void setD2Screen(int Setting)
        {
            
             if (Setting == 1)
             { 
                 foreach (D2Profile pro in bpro)
                 {
                     ShowWindow(pro.D2Process.MainWindowHandle, ShowWindowCommands.Hide);
                     break;
                 }
             }
                 //
             if (Setting == 2)
             {
                 foreach (D2Profile pro in bpro)
                 {
                     ShowWindow(pro.D2Process.MainWindowHandle, ShowWindowCommands.Show);
                     //ShowWindow(pro.D2Process.MainWindowHandle, ShowWindowCommands.f);
                     break;
                 }
             }
             

            if (Setting == 3)
            {
                using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\GLIDE3toOpenGL", true))
                {
                    if (Convert.ToInt32(reg.GetValue("windowed")) == 0)
                        reg.SetValue("windowed", 1);
                }
            }
            if (Setting == 4)
            {
                using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\GLIDE3toOpenGL", true))
                {
                    if (Convert.ToInt32(reg.GetValue("windowed")) == 1)
                        reg.SetValue("windowed", 0);
                }
            }
        }
        public string status;
        bool Toggle;
        void ToggleCall(bool toggle)
        {
            Toggle = toggle;
            //status = Toggle.ToString();
        }
        string[] Message;
        public void HandleMessage(IntPtr hWnd, string message)
        {
            ProfileMessage profileMessage;
            try
            {
                profileMessage = (ProfileMessage)JsonConvert.DeserializeObject<ProfileMessage>(message);
            }
            catch (Exception ex)
            {
                MSG = ("Message: " + message + "\n" + ex.ToString());
                return;
            }
            ReadOnece = true;
            MSG = profileMessage.args[0];
            MSG = MSG.Replace("\"", "");
            Message = MSG.Split(';');
            ReadMSG(Message);

        }
        #endregion

        #region Structer and HandlerTimer
        /* 
         * Adds timer to make show/hide global can thing of another solution aswell :O
         */
        private HandlerClass() 
         {
             timer.Interval = 1000;
             timer.Tick += timer_tick;
             timer.Enabled = true;
             timer.Start();
             LoadPrivateFontCollection();
         }

        private void timer_tick(object sender, EventArgs e)
        {
            if (Runs == Running.On)
                HeartBeat++;
            else
                HeartBeat = 0;
            if(HeartBeat >= 5)
            {
                try
                {
                    Runs = Running.Off;
                    foreach (D2Profile pro in bpro)
                    {
                        Process.GetProcessById(pro.D2Process.Id).Kill();
                    }
                }
                catch {}
            }
            ShowHideDebug();
        }
        public void ShowHideDebug()
        {
            if (consoleCheck && !console.Visible)
                console.Show();
            else if (!consoleCheck && console.Visible)
                console.Hide();
        }

        #endregion

        #region StartDiablo, Inject D2BS
        public bool StartDiablo()
         {
            using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
            {
                if (reg.GetValue("Diablo 2 Path").ToString() != "")
                {
                    if (Settings[2] == "True")
                        setD2Screen(3);
                    else
                        setD2Screen(4);
                    if (Settings[0] == "True")
                        reg.SetValue("Diablo 2 Account", Account);
                    if (Settings[1] == "True")
                        reg.SetValue("Diablo 2 Password", Password);
                    if (SetProfile)
                        if (SetProfile)
                        {
                            bpro.Clear();
                            bpro.Add(d2profile);
                            foreach (D2Profile pro in bpro)
                            {
                                D2Bot.Program.LaunchClient(pro);
                                Runs = Running.On;
                            }
                            return true;
                        }
                        else
                            return false;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Select Game.exe in settings!");
                }
            }
             
             return false;
         }
        #endregion

        #region Singelton
        /*
         * Making this class a singelton
         */
        public static HandlerClass Instance
           {
              get 
              {
                 if (handler == null)
                 {
                     handler = new HandlerClass();
                 }
                 return handler;
              }
           }
        #endregion

        #region Settings
        public string[] Settings
        {
            get
            {
                using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", false))
                {
                    string set = reg.GetValue("Settings").ToString();
                    try
                    {
                        string[] Set = set.Split(new[] { "," }, StringSplitOptions.None);
                        return Set;
                    }
                    catch
                    {
                        string[] failsafe = { "false", "false", "false" };
                        return failsafe;
                    }
                }
            }
        }
        #endregion

        #region MSG Handler
        public void ReadMSG(string[] MSG)
        {
            #region OOG
            if (MSG[0] == "1")
            {
                switch (MSG[1])
                {
                    case "1":
                        //FINIALY IN THE FUCKINGLOBBY!!!§
                        screenState = ScreenState.Lobby;
                        setD2Screen(1);
                        break;
                    case "8": //Main Menu
                        screenState = ScreenState.MainMenu;
                        Characters.Clear();
                        SendMSG("1");
                        setD2Screen(1);
                        break;
                    case "27": //Select Realm
                        screenState = ScreenState.RealmMenu;
                        setD2Screen(1);
                        break;
                    case "9": //Login
                        screenState = ScreenState.Login;
                        setD2Screen(1);
                        SendMSG("4;" + Account + ";" + Password);
                        console.SendToConsole("Sent: 4;" + Account + ";*******");
                        break;
                    case "12": // Character screen!
                        screenState = ScreenState.CharScreen;
                        setD2Screen(1);
                        break;
                    case "26": // Game Exists OWND!
                        screenState = ScreenState.GameExists;
                        setD2Screen(1);
                        break;
                    case "9999"://InGame WE SUCCSEEDED!
                        screenState = ScreenState.inGame;
                        setD2Screen(2);
                        setD2Screen(5);
                        SendfromIngameOnce = true;
                        break;
                    case "19":
                        screenState = ScreenState.CDKeyInUse;
                        //setD2Screen(1);
                        break;
                    case "42":
                      //  screenState = ScreenState.RealmDown;
                        break;
                    case "109234": //Game Was Created
                        Creator = true;
                        break;

                }
                if(screenState != OldscreenState)
                {
                    console.SendToConsole("ScreenState: " + screenState.ToString());
                    OldscreenState = screenState;
                }
            
            }
            if (MSG[0] == "2")
            {
                if (MSG[1].ToLower() != Realm.ToLower())
                {
                    SendMSG("2;" + Realm);
                    console.SendToConsole("Sent: 2; " + Realm);
                }
                else
                {
                    // Thread.Sleep(1000);
                    SendMSG("3;");
                    console.SendToConsole("Sent:Request For Bnet");
                }
                //SendMSG("3:" + Account + ";" + Password);
            }
            if(MSG[0] == "3")
                SetCharacters(MSG[1]);
            if(MSG[0] == "4")
                WasIngame = true;
            if(MSG[0] == "100")
            {
                Characters.Clear();
                console.SendToConsole("Character List Cleared! Waiting for refil..");
            }
            if(MSG[0] == "200")
                SetUiCharacters = true;
            if(MSG[0] == "44")
            {
                string fullGameInfo = MSG[1];
                Bnetgame.Clear();
                try
                {
                    string[] gameInfo = fullGameInfo.Split(':');
                    string[] bNetGame;
                    for (int i = 0; i < gameInfo.Length; i++ )
                    {
                        bNetGame = gameInfo[i].Split(',');
                        BnetGames bnet = new BnetGames();
                        bnet.Name = bNetGame[0];
                        bnet.Players = bNetGame[1];
                        Bnetgame.Add(bnet);
                        
                    }
                }
                catch {}
                AddToList = true;
            }
            #endregion
            #region HeartBeat
            if(MSG[0] == "9999")
            {
                HeartBeat = 0;
            }

            #endregion

        }
        #endregion

        #region Build CharList
        private void SetCharacters(string CharInfo)
        {
            CharInfo = CharInfo.Replace(@"\", "");
            console.SendToConsole("Got Char: " + CharInfo);
            CharInfo = CharInfo.Replace(@"[", "");
            CharInfo = CharInfo.Replace(@"]", "");
            Chars chars = new Chars();
            string[] tmpChar = CharInfo.Split(',');
            chars.rank = tmpChar[0];
            chars.Name = tmpChar[1];
            chars.ClassLevel = tmpChar[2];
            chars.Exp = tmpChar[3];
            try { chars.Lad = tmpChar[4]; }
            catch { chars.Lad = ""; }
            try { chars.Expire = tmpChar[5]; }
            catch { chars.Expire = ""; }
            Characters.Add(chars);
        }
        #endregion

        #region SendMsgtoD2
        public bool SendMSG(string MSG)
        {
            try
            {
                D2Bot.MessageHelper.SendStringMessageToHandle(d2profile.D2Process.MainWindowHandle, 1, MSG);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region AddCostumFont
        private void LoadPrivateFontCollection()
        {
            byte[] fontArray = Properties.Resources.Avqest;
            int dataLength = Properties.Resources.Avqest.Length;
            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontArray, 0, ptrData, dataLength);
            uint cFonts = 0;
            AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddMemoryFont(ptrData, dataLength);
            Marshal.FreeCoTaskMem(ptrData);
            ff = pfc.Families[0];
            font = new Font(ff, 15f, FontStyle.Bold);
        }
        #endregion

    }
}
