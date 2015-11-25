using System;
using System.Drawing;
using System.Windows.Forms;
using D2Bot;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Permissions;
using Gecko;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using Gecko.DOM;
using System.Linq;
namespace Diablo_2_Next_Generation
{
    public partial class Form1 : Form
    {
        #region vars
        private List<Games> games = new List<Games>();
        private Network login = new Network();
        private int refrechcounter = 0;
        private bool sendFailOnce = true;
        private int adtimer = 0;
        private bool setChannel = false;
        ConfigReader configs = new ConfigReader();
        #endregion

        #region FixFlicker2.0
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        #endregion

        #region Init
        public Form1()
        {
            Gecko.Xpcom.Initialize(System.IO.Directory.GetCurrentDirectory() + @"\d2bs\xulrunner");
            InitializeComponent();
            //HandlerClass.Instance.consoleCheck = true;
            D2JSP.Navigate("http://d2jsp.org");
            Arreatsubmit.Navigate("http://classic.battle.net/diablo2exp/");
            Blizzhackers.Navigate("http://www.blizzhackers.cc/");
            IrcChat.Navigate("https://kiwiirc.com/client");
            add.Navigate(login.realmd + "add.php");
            //
            //http://classic.battle.net/diablo2exp/
        }
        #endregion

        #region SendCopyData
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
                    foreach (D2Profile pro in HandlerClass.Instance.bpro)
                    {
                        if (pro.D2Process.MainWindowHandle == pro.D2Process.MainWindowHandle)//Cheking For the real Window so its not wrong one! This fix will allow more clients!
                            HandlerClass.Instance.HandleMessage(num, str);
                    }
                    
                }
                catch
                {
                }
                m.Result = (IntPtr)1;
            }
            base.WndProc(ref m);
        }
        #endregion

        #region Moving Login
        void moveLogin()
        {
            textBox1.Location = new Point(this.Width / 2 - 50, this.Height / 2 - 100);
            textBox2.Location = new Point(this.Width / 2 - 50, this.Height / 2 - 60);
            label1.Location = new Point(this.Width / 2 - 115, this.Height / 2 - 95);
            label2.Location = new Point(this.Width / 2 - 117, this.Height / 2 - 55);
            comboBox1.Location = new Point(this.Width / 2 - 50, this.Height / 2 - 25);
            button1.Location = new Point(this.Width / 2 - 50, this.Height / 2 + 5);
            settingsBtn.Location = new Point(this.Width / 3 - 20, this.Height / 7);
        }
        #endregion

        #region D2NG Loading Stuff
        private void Form1_Load(object sender, EventArgs e)
        {
            D2Bot.StyleHelper.DisableFlicker(this);

            #region Login textbox add Y;
            textBox2.AutoSize = false;
            textBox1.AutoSize = false;
            this.textBox1.Size = new System.Drawing.Size(142, 30);
            this.textBox2.Size = new System.Drawing.Size(142, 30);
            #endregion
            
            #region Set Screen Font
            textBox1.Font = new Font(HandlerClass.Instance.ff, 20f, FontStyle.Regular);
            textBox2.Font = new Font(HandlerClass.Instance.ff, 20f, FontStyle.Regular);
            comboBox1.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
            label1.Font = label2.Font = label3.Font = button1.Font = label9.Font =
            char1_exp.Font = char1_level.Font = char1_rank.Font = char1_lad.Font =
            char2_exp.Font = char2_level.Font = char2_rank.Font = char2_lad.Font =
            char3_exp.Font = char3_level.Font = char3_rank.Font = char3_lad.Font =
            char4_exp.Font = char4_level.Font = char4_rank.Font = char4_lad.Font =
            char5_exp.Font = char5_level.Font = char5_rank.Font = char5_lad.Font =
            char6_exp.Font = char6_level.Font = char6_rank.Font = char6_lad.Font =
            char7_exp.Font = char7_level.Font = char7_rank.Font = char7_lad.Font =
            char8_exp.Font = char8_level.Font = char8_rank.Font = char8_lad.Font =
            label4.Font = label5.Font = textBox3.Font = textBox4.Font = listView1.Font =
            button4.Font = label6.Font = label7.Font = label8.Font = label11.Font = textBox6.Font = 
            checkBox1.Font = radioButton1.Font = radioButton2.Font = radioButton3.Font = label12.Font =
            button2.Font = d2ngList.Font = label10.Font = button5.Font = button3.Font = richTextBox1.Font =
            tabControl1.Font = tabControl2.Font = listBox2.Font = tabControl3.Font =
                    new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);

            char_conv.Font = char_create.Font = char_del.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
            char_title.Font = new Font(HandlerClass.Instance.ff, 20f, FontStyle.Regular);
            #endregion

            #region Set Region!
            switch (comboBox1.Text)
            {
                case "Europe":
                    HandlerClass.Instance.Realm = "3";
                    break;
                case "U.S. West":
                    HandlerClass.Instance.Realm = "0";
                    break;
                case "U.S. East":
                    HandlerClass.Instance.Realm = "1";
                    break;
                case "Asia":
                    HandlerClass.Instance.Realm = "2";
                    break;
            }
            #endregion

            #region GetSettings/AddSettings
            /*
             * AddSettings for first start
             * Getsetings if it has them in registry
             */
            try
                {
                    using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
                    {
                        reg.OpenSubKey("D2NG");
                        reg.GetValue("Diablo 2 Path");
                        textBox1.Text = reg.GetValue("Diablo 2 Account").ToString();
                        textBox2.Text = reg.GetValue("Diablo 2 Password").ToString();
                    }
                }
                catch
                {
                    Registry.CurrentUser.CreateSubKey(@"SOFTWARE\D2NG");
                    using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
                    {
                        reg.OpenSubKey("D2NG");
                        reg.SetValue("Diablo 2 Path", "");
                        reg.SetValue("Diablo 2 Account", "");
                        reg.SetValue("Diablo 2 Password", "");
                        reg.SetValue("Settings", "false,false,false,false");
                    }
                }
            #endregion

            timer1.Start();
        }
        #endregion

        #region center loginForms
        /*
         * Will add proper setting for this
         */
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            moveLogin();
        }
        #endregion

        #region Buttons
        //LoginBtn
            #region Loggin btn
        private void button1_Click(object sender, EventArgs e)
            {
                HandlerClass.Instance.Account = textBox1.Text;
                HandlerClass.Instance.Password = textBox2.Text;
                HandlerClass.Instance.hash = login.Hash;
                if (HandlerClass.Instance.Runs == HandlerClass.Running.Off)
                {
                    if (!HandlerClass.Instance.StartDiablo())
                        System.Windows.Forms.MessageBox.Show("Culd not Start Diablo 2, Dont know what happend" + Environment.NewLine + "Try to run as Admin!"); ;
                }
                else
                {
                HandlerClass.Instance.SendMSG("9;");
                }
            }
        #endregion

            #region send 14
        private void button8_Click(object sender, EventArgs e)
                {
                    HandlerClass.Instance.SendMSG("14;");
                }
        #endregion

            #region JoinGame bnet
        private void button4_Click(object sender, EventArgs e)
            {
                HandlerClass.Instance.SendMSG("6;" + textBox3.Text + ";" + textBox4.Text);
            }
            #endregion

            #region refresh
            private void button5_Click(object sender, EventArgs e)
                {
                    NGgameRefresher(HandlerClass.Instance.GameList);
                    button5.Enabled = false;
                    refrechcounter = 0;
                }
            #endregion

            #region Create Game
            private void button2_Click(object sender, EventArgs e)
                {
                    HandlerClass.Instance.MakeGame = false;
                }
            #endregion

            #region D2NG JoinGame
            private void button3_Click(object sender, EventArgs e)
                {
                    foreach(Games game in HandlerClass.Instance.Games)
                    {
                        if(game.selected == true)
                        {
                            HandlerClass.Instance.SendMSG("6;" + game.Name + ";" + game.Password);
                        }
                    }
                }
                #endregion

            #region DontRemember :O
            private void button7_Click(object sender, EventArgs e)
            {
                HandlerClass.Instance.SendMSG("8;");
            }
            #endregion

            #region CreateChar
            private void char_create_Click(object sender, EventArgs e)
            {

            }
            #endregion

            #region Remove Char
            private void char_del_Click(object sender, EventArgs e)
            {
                DialogResult dialogResult = MessageBox.Show("Sure YOU wanna delete" + HandlerClass.Instance.Characters[HandlerClass.Instance.SelectedPlayer].Name, "Warning!!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    HandlerClass.Instance.SendMSG("11");

                }
            }
            #endregion

            #region ConvertChar
            private void char_conv_Click(object sender, EventArgs e)
            {

            }
            #endregion

            #region LogoutAccount
            private void char_loggout_Click(object sender, EventArgs e)
            {
                Lobby.Visible = false;
                CharacterSelect.Visible = false;
                HandlerClass.Instance.SetUiCharacters = false;
                HandlerClass.Instance.SetUiLobby = false;

            }
            #endregion

            #region Login
            private void char_select_Click(object sender, EventArgs e)
            {
                HandlerClass.Instance.SendMSG("10;");
            }
            #endregion

            #region Small/Big Lobby
            private void button6_Click(object sender, EventArgs e)
            {
                if (tabControl2.Visible)
                {
                    tabControl2.Visible = false;
                    button6.Text = "<";
                    this.MaximumSize = new Size(286, int.MaxValue);
                    this.Width = 286;
                    D2JSP.Navigate("about:blank");
                    Arreatsubmit.Navigate("about:blank");
                    Blizzhackers.Navigate("about:blank");
                    //286, 600
                }
                else
                {
                    tabControl2.Visible = true;
                    button6.Text = ">";
                    this.MinimumSize = new Size(800, 600);
                    this.MaximumSize = new Size(int.MaxValue, int.MaxValue);
                    this.Width = 800;
                    D2JSP.Navigate("http://d2jsp.org");
                    Arreatsubmit.Navigate("http://classic.battle.net/diablo2exp/");
                    Blizzhackers.Navigate("http://www.blizzhackers.cc/");
                }
            }
            #endregion

            #region Settings
            private void settingsBtn_Click(object sender, EventArgs e)
            {
                Settings settings = new Settings();
                settings.StartPosition = FormStartPosition.CenterParent;
                settings.ShowDialog();
            
            }
            #endregion

        #endregion

        #region Combobox HilightFix
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        
        private void comboBox1_MouseMove(object sender, MouseEventArgs e)
        {
            comboBox1.SelectionLength = 0;
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            comboBox1.Select(0, 0);
        }
        #endregion

        #region Closeing Mainform
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (D2Profile pro in HandlerClass.Instance.bpro)
                {
                    Process.GetProcessById(pro.D2Process.Id).Kill();
                }
            }
            catch { MessageBox.Show("Failed to Close Diablo 2. May still be running in background!"); }
        }
        #endregion

        #region SetRealm
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "Europe":
                    HandlerClass.Instance.Realm = "3";
                    break;
                case "U.S. West":
                    HandlerClass.Instance.Realm = "0";
                    break;
                case "U.S. East":
                    HandlerClass.Instance.Realm = "1";
                    break;
                case "Asia":
                    HandlerClass.Instance.Realm = "2";
                    break;
            }
            //HandlerClass.Instance.Realm = comboBox1.Text;
        }
        #endregion

        #region D2NG Handler
        private void timer1_Tick(object sender, EventArgs e)
        {
            #region Refresh Counter
            refrechcounter++;
            if(refrechcounter >= 40)
            {
                button5.Enabled = true;
            }
            #endregion

            #region Set IrcChannel
            if (!setChannel)
                {
                    try
                    { 
                        setChannel = true;
                        GeckoInputElement txtbox = new GeckoInputElement(IrcChat.Document.GetElementById("server_select_channel").DomObject);
                        txtbox.Value = "#D2NG";
                    }
                    catch { }
                }
            #endregion

            #region adRefresher
            if (adtimer >= 10000)
            {
                add.Navigate(login.realmd + "d2ng/add.php");
                adtimer = 0;
            }
            #endregion

            #region Cdkey In Use
            if (HandlerClass.Instance.screenState == HandlerClass.ScreenState.CDKeyInUse)
            {
                HandlerClass.Instance.screenState = HandlerClass.ScreenState.MainMenu;
                MessageBox.Show("CD Key in Use!");
                try
                {
                    foreach (D2Profile pro in HandlerClass.Instance.bpro)
                    {
                        Process.GetProcessById(pro.D2Process.Id).Kill();
                        HandlerClass.Instance.Runs = HandlerClass.Running.Off;
                    }
                }
                catch { MessageBox.Show("Failed to Close Diablo 2. May still be running in background!"); }
            }

            if (HandlerClass.Instance.screenState == HandlerClass.ScreenState.RealmDown)
            {
                HandlerClass.Instance.screenState = HandlerClass.ScreenState.MainMenu;
                MessageBox.Show("Realm Down Wait for 3 minutes...");
                try
                {
                    foreach (D2Profile pro in HandlerClass.Instance.bpro)
                    {
                        Process.GetProcessById(pro.D2Process.Id).Kill();
                        HandlerClass.Instance.Runs = HandlerClass.Running.Off;
                    }
                }
                catch { MessageBox.Show("Failed to Close Diablo 2. May still be running in background!"); }
            }
            #endregion

            #region Check if Running
            if (HandlerClass.Instance.Runs == HandlerClass.Running.Off)
            {
                Lobby.Visible = false;
                CharacterSelect.Visible = false;
            }
            #endregion

            #region InGame
            if (HandlerClass.Instance.SendfromIngameOnce)
            {
                foreach (Games game in HandlerClass.Instance.Games)
                {
                    if (game.selected == true)
                    {
                        login.JoinGame(Convert.ToInt16(game.ID), HandlerClass.Instance.Characters[HandlerClass.Instance.SelectedPlayer]);
                    }
                }
                HandlerClass.Instance.SendfromIngameOnce = false;
            }
            if (HandlerClass.Instance.screenState == HandlerClass.ScreenState.inGame)
            { tabControl1.Enabled = false; button7.Enabled = false; }
            else
            { tabControl1.Enabled = true; button7.Enabled = true; }

            if(HandlerClass.Instance.WasIngame)
            {
                login.RequestToRemove();
                HandlerClass.Instance.console.SendToConsole("Sent Request to remove Game!");
                HandlerClass.Instance.WasIngame = false;
            }
            #endregion

            #region MakeGame
            if (!HandlerClass.Instance.MakeGame)
            {
                if (checkBox1.Checked)
                {
                    HandlerClass.Instance.SendMSG("7;" + textBox5.Text + ";" + "xy" + ";" + (int)HandlerClass.Instance.Diffeculty);
                    //http://217.210.160.125/getsendgames.php?game=test2&pass=woot&desc=Difficulty:Hell&player=Name&lad=1&region=1
                    HandlerClass.Instance.MakeGame = true;
                    sendFailOnce = true;
                    return;
                }
                else
                {
                    HandlerClass.Instance.SendMSG("7;" + textBox5.Text + ";" + createG_password.Text + ";" + HandlerClass.Instance.Diffeculty);
                    HandlerClass.Instance.MakeGame = true;
                    sendFailOnce = true;
                    return;
                }
            }
            if(HandlerClass.Instance.Creator)
            {
                login.CreateGame(textBox5.Text, "xy", HandlerClass.Instance.Diffeculty, HandlerClass.Instance.Characters[HandlerClass.Instance.SelectedPlayer], textBox7.Text, HandlerClass.Instance.Realm);
                HandlerClass.Instance.Creator = false;
            }
            else if (HandlerClass.Instance.screenState == HandlerClass.ScreenState.GameExists && sendFailOnce)
            {
                sendFailOnce = false;
                HandlerClass.Instance.MakeGame = true;
                MessageBox.Show("Game Allready Exists!");
                return;
            }
            #endregion

            #region SetScreen sise
            if (HandlerClass.Instance.screenState != HandlerClass.ScreenState.Lobby && HandlerClass.Instance.screenState != HandlerClass.ScreenState.inGame)
            {
                this.MaximumSize = new Size(int.MaxValue, int.MaxValue);
                this.MinimumSize = new Size(800, 600);
                this.Width = 800;
            }
            #endregion

            #region Show Login Status
            if (HandlerClass.Instance.screenState == HandlerClass.ScreenState.Login)
                label3.Text = HandlerClass.Instance.screenState.ToString();
            else
                label3.Text = "";
            #endregion

            #region MainMenu
            if (HandlerClass.Instance.screenState == HandlerClass.ScreenState.MainMenu)
            {
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                button1.Visible = true;
                textBox1.Visible = true; 
                textBox2.Visible = true; 
                comboBox1.Visible = true;
                textBox2.Visible = true;
                textBox1.Visible = true;
                CharacterSelect.Visible = false;
                Lobby.Visible = false;
                HandlerClass.Instance.SetUiCharacters = true;
                HandlerClass.Instance.SetUiLobby = true;
            }
            #endregion

            #region Lobby
            if (HandlerClass.Instance.screenState == HandlerClass.ScreenState.Lobby && HandlerClass.Instance.SetUiLobby)
            {
                CharacterSelect.Visible = false;
                Lobby.Visible = true;
                HandlerClass.Instance.SetUiLobby = false;
                HandlerClass.Instance.SetUiCharacters = true;
                HandlerClass.Instance.console.SendToConsole("Showing Lobby!");
            }
            #endregion

            #region Character Select
            if (HandlerClass.Instance.screenState == HandlerClass.ScreenState.CharScreen && HandlerClass.Instance.SetUiCharacters)
            {
                CharacterSelect.Visible = true;
                Lobby.Visible = false;
                HandlerClass.Instance.SetUiLobby = true;
                HandlerClass.Instance.SetUiCharacters = false;
                SetCharacters();
                HandlerClass.Instance.console.SendToConsole("Characters Dumped TO UI done!");
            }
            #endregion

            #region d2GameList
            if (HandlerClass.Instance.AddToList)
            {
                HandlerClass.Instance.AddToList = false;
                listView1.Items.Clear();
                foreach (BnetGames games in HandlerClass.Instance.Bnetgame)
                {
                    ListViewItem item = new ListViewItem(games.Name);
                    item.SubItems.Add(games.Players);
                    listView1.Items.Add(item);
                }
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            #endregion

            #region Game Allready Exists!
            if (HandlerClass.Instance.screenState == HandlerClass.ScreenState.GameExists)
                label12.Text = "Game Allready Exists!";
            else
                label12.Text = string.Empty;
            #endregion
        }
        #endregion

        #region CharacterScreen
        private void SetPicure(string S, PictureBox pic)
        {
           string[] hold;
           hold = S.Split(' ');

            switch(hold[2].ToUpper())
            {
                case "DRUID":
                    pic.Image = Properties.Resources.Drood;
                    break;
                case "SORCERESS":
                    pic.Image = Properties.Resources.SoSo;
                    break;
                case "PALADIN":
                    pic.Image = Properties.Resources.Pala;
                    break;
                case "BARBARIAN":
                    pic.Image = Properties.Resources.Barb;
                    break;
                case "NECROMANCER":
                    pic.Image = Properties.Resources.Necro;
                    break;
                case "ASSASSIN":
                    pic.Image = Properties.Resources.AssA;
                    break;
                case "AMAZON":
                    pic.Image = Properties.Resources.AmA;
                    break;
            }
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        private void SetCharacters()
        {
            #region Set Character Display
            try
            { 
                char1_rank.Text = HandlerClass.Instance.Characters[0].rank;
                char1_exp.Text = HandlerClass.Instance.Characters[0].Exp;
                char1_dis.Text = HandlerClass.Instance.Characters[0].Expire;
                char1_level.Text = HandlerClass.Instance.Characters[0].Name + " " + HandlerClass.Instance.Characters[0].ClassLevel.Substring(0, 8);
                char1_lad.Text = HandlerClass.Instance.Characters[0].Lad;
                SetPicure(HandlerClass.Instance.Characters[0].ClassLevel, char1_pic);
                char1_panel.Visible = true;
            }
            catch { char1_panel.Visible = false; }
            try
            { 
                char2_rank.Text = HandlerClass.Instance.Characters[1].rank;
                char2_exp.Text = HandlerClass.Instance.Characters[1].Exp;
                char2_dis.Text = HandlerClass.Instance.Characters[1].Expire;
                char2_level.Text = HandlerClass.Instance.Characters[1].Name + " " + HandlerClass.Instance.Characters[1].ClassLevel.Substring(0, 8);
                char2_lad.Text = HandlerClass.Instance.Characters[1].Lad;
                SetPicure(HandlerClass.Instance.Characters[1].ClassLevel, char2_pic);
                char2_panel.Visible = true;
            }
            catch { char2_panel.Visible = false; }
            try
            { 
                char3_rank.Text = HandlerClass.Instance.Characters[2].rank;
                char3_exp.Text = HandlerClass.Instance.Characters[2].Exp;
                char3_dis.Text = HandlerClass.Instance.Characters[2].Expire;
                char3_level.Text = HandlerClass.Instance.Characters[2].Name + " " + HandlerClass.Instance.Characters[2].ClassLevel.Substring(0, 8);
                char3_lad.Text = HandlerClass.Instance.Characters[2].Lad;
                SetPicure(HandlerClass.Instance.Characters[2].ClassLevel, char3_pic);
                char3_panel.Visible = true;
            }
            catch { char3_panel.Visible = false; }
            try
            { 
                char4_rank.Text = HandlerClass.Instance.Characters[3].rank;
                char4_exp.Text = HandlerClass.Instance.Characters[3].Exp;
                char4_dis.Text = HandlerClass.Instance.Characters[3].Expire;
                char4_level.Text = HandlerClass.Instance.Characters[3].Name + " " + HandlerClass.Instance.Characters[3].ClassLevel.Substring(0, 8);
                char4_lad.Text = HandlerClass.Instance.Characters[3].Lad;
                SetPicure(HandlerClass.Instance.Characters[3].ClassLevel, char4_pic);
                char4_panel.Visible = true;
            }
            catch { char4_panel.Visible = false; }
            try
            { 
                char5_rank.Text = HandlerClass.Instance.Characters[4].rank;
                char5_exp.Text = HandlerClass.Instance.Characters[4].Exp;
                char5_dis.Text = HandlerClass.Instance.Characters[4].Expire;
                char5_level.Text = HandlerClass.Instance.Characters[4].Name + " " + HandlerClass.Instance.Characters[4].ClassLevel.Substring(0, 8);
                char5_lad.Text = HandlerClass.Instance.Characters[4].Lad;
                SetPicure(HandlerClass.Instance.Characters[4].ClassLevel, char5_pic);
                char5_panel.Visible = true;
            }
            catch { char5_panel.Visible = false; }
            try
            { 
                char6_rank.Text = HandlerClass.Instance.Characters[5].rank;
                char6_exp.Text = HandlerClass.Instance.Characters[5].Exp;
                char6_dis.Text = HandlerClass.Instance.Characters[5].Expire;
                char6_level.Text = HandlerClass.Instance.Characters[5].Name + " " + HandlerClass.Instance.Characters[5].ClassLevel.Substring(0, 8);
                char6_lad.Text = HandlerClass.Instance.Characters[5].Lad;
                SetPicure(HandlerClass.Instance.Characters[5].ClassLevel, char6_pic);
                char6_panel.Visible = true;
            }
            catch { char6_panel.Visible = false; }
            try
            { 
                char7_rank.Text = HandlerClass.Instance.Characters[6].rank;
                char7_exp.Text = HandlerClass.Instance.Characters[6].Exp;
                char7_dis.Text = HandlerClass.Instance.Characters[6].Expire;
                char7_level.Text = HandlerClass.Instance.Characters[6].Name + " " + HandlerClass.Instance.Characters[6].ClassLevel.Substring(0, 8);
                char7_lad.Text = HandlerClass.Instance.Characters[6].Lad;
                SetPicure(HandlerClass.Instance.Characters[6].ClassLevel, char7_pic);
                char7_panel.Visible = true;
            }
            catch { char7_panel.Visible = false; }
            try
            { 
                char8_rank.Text = HandlerClass.Instance.Characters[7].rank;
                char8_exp.Text = HandlerClass.Instance.Characters[7].Exp;
                char8_dis.Text = HandlerClass.Instance.Characters[7].Expire;
                char8_level.Text = HandlerClass.Instance.Characters[7].Name + " " + HandlerClass.Instance.Characters[7].ClassLevel.Substring(0, 8);
                char8_lad.Text = HandlerClass.Instance.Characters[7].Lad;
                SetPicure(HandlerClass.Instance.Characters[7].ClassLevel, char8_pic);
                char8_panel.Visible = true;
            }
            catch { char8_panel.Visible = false; }
            #endregion
        }

        private void char1_panel_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.SendMSG("5;"+HandlerClass.Instance.Characters[0].Name);
            HandlerClass.Instance.SelectedPlayer = 0;
            setPanelBorder(char1_panel);
        }

        private void char2_panel_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.SendMSG("5;" + HandlerClass.Instance.Characters[1].Name);
            HandlerClass.Instance.SelectedPlayer = 1;
            setPanelBorder(char2_panel);
        }

        private void char3_panel_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.SendMSG("5;" + HandlerClass.Instance.Characters[2].Name);
            HandlerClass.Instance.SelectedPlayer = 2;
            setPanelBorder(char3_panel);
        }

        private void char4_panel_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.SendMSG("5;" + HandlerClass.Instance.Characters[3].Name);
            HandlerClass.Instance.SelectedPlayer = 3;
            setPanelBorder(char4_panel);
        }

        private void char5_panel_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.SendMSG("5;" + HandlerClass.Instance.Characters[4].Name);
            HandlerClass.Instance.SelectedPlayer = 4;
            setPanelBorder(char5_panel);
        }
        

        private void char6_panel_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.SendMSG("5;" + HandlerClass.Instance.Characters[5].Name);
            HandlerClass.Instance.SelectedPlayer = 5;
            setPanelBorder(char6_panel);
        }

        private void char7_panel_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.SendMSG("5;" + HandlerClass.Instance.Characters[6].Name);
            HandlerClass.Instance.SelectedPlayer = 6;
            setPanelBorder(char7_panel);
        }

        private void char8_panel_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.SendMSG("5;" + HandlerClass.Instance.Characters[7].Name);
            HandlerClass.Instance.SelectedPlayer = 7;
            setPanelBorder(char8_panel);
        }

        void setPanelBorder(Panel pan)
        {
            char1_panel.BackgroundImage = null;
            char2_panel.BackgroundImage = null;
            char3_panel.BackgroundImage = null;
            char4_panel.BackgroundImage = null;
            char5_panel.BackgroundImage = null;
            char6_panel.BackgroundImage = null;
            char7_panel.BackgroundImage = null;
            char8_panel.BackgroundImage = null;
            pan.BackgroundImage = Properties.Resources.Charborder;
        }
        private void ShowDebug_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.consoleCheck = true;
        }
        #endregion

        #region Add To D2NG Server
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                createG_password.Enabled = false;
                checkBox3.Enabled = true;
            }
            else
            {
                createG_password.Enabled = true;
                checkBox3.Enabled = false;
            }
        }
        #endregion

        #region Start AutoRefresh
        private void Lobby_VisibleChanged(object sender, EventArgs e)
        {
            if(Lobby.Visible == true)
            {
                refreshGames.Enabled = true;
                refreshGames.Start();
                NGgameRefresher(0);
            }
            else
                refreshGames.Stop();
        }
        #endregion

        #region D2NG Game Refresher
        private void refreshGames_Tick(object sender, EventArgs e)
        {
            NGgameRefresher(HandlerClass.Instance.GameList);
        }

        
        private void NGgameRefresher(int mode)
        {
            try
            {
                string gameInfo = login.RequestGameList(mode, HandlerClass.Instance.Characters[HandlerClass.Instance.SelectedPlayer], HandlerClass.Instance.Realm);
                HandlerClass.Instance.Games.Clear();
                d2ngList.Items.Clear();
                //68,woot1,10,,Normal,3,-1915,Fake.;
                string[] Games = gameInfo.Split('|');
                for (int i = 0; i < Games.Length; i++)
                {

                    string[] GameInfo = Games[i].Split('\'');
                    string[] Players = GameInfo[7].Split('.');
                    Games game = new Games();
                    game.ID = GameInfo[0];
                    game.Name = GameInfo[1];
                    game.Password = GameInfo[2];
                    game.Description = GameInfo[3];
                    game.Diff = GameInfo[4];
                    game.Time = GameInfo[5];
                    for (int a = 0; a < Players.Length; a++)
                    {
                        game.Characters.Add(Players[a]);
                    }
                    HandlerClass.Instance.Games.Add(game);
                    d2ngList.Items.Add(game.Name);


                }
            }
            catch { }
        }
        #endregion

        #region SetDif
        /*
         * Know bug need to click two times! first time using radiobuttons need some more research
         */
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            if (radioButton1.Checked)
                HandlerClass.Instance.Diffeculty = HandlerClass.dif.Normal;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton3.Checked = false;
            radioButton1.Checked = false;
            if (radioButton2.Checked)
                HandlerClass.Instance.Diffeculty = HandlerClass.dif.Nightmare;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            if(radioButton3.Checked)
                HandlerClass.Instance.Diffeculty = HandlerClass.dif.Hell;
        }
        #endregion

        #region GameSelection
        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandlerClass.Instance.GameList = tabControl3.SelectedIndex;
            
            if (refrechcounter >= 40)
            {
                NGgameRefresher(HandlerClass.Instance.GameList);
            }
        }

        private void d2ngList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //HandlerClass.Instance.Games[d2ngList.SelectedIndex].selected = true;
            foreach(Games game in HandlerClass.Instance.Games)
            {
                if(game.Name == (string)d2ngList.SelectedItem)
                {
                    string players = Environment.NewLine;
                    game.selected = true;
                    foreach (string player in game.Characters)
                    {
                        players += player + Environment.NewLine;
                    }
                    richTextBox1.Text = "Difficulty: " + game.Diff + Environment.NewLine + "Players [" + Environment.NewLine + players + "]";
                    if(!string.IsNullOrEmpty(game.Description))
                    {
                        richTextBox1.Text += "Descriptopn: " + game.Description;
                    }
                }
                if(game.Name != (string)d2ngList.SelectedItem)
                    game.selected = false;
            }
        }

        private void textBox7_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.OemPipe)
                e.Handled = true;
            return;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 1)
            {
                HandlerClass.Instance.SendMSG("15;");
            }
        }


        #endregion

        #region ShowConsole
        private void panel2_Click(object sender, EventArgs e)
        {
            HandlerClass.Instance.consoleCheck = true;
        }
        #endregion

        #region SelectBnetGames
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        foreach(BnetGames game in HandlerClass.Instance.Bnetgame)
            {
                try { 
                if (listView1.SelectedItems[0].SubItems[0].Text == game.Name)
                {
                    textBox3.Text = game.Name;
                    game.selected = true;
                }
                    }
                catch { }
            }
        }
        #endregion

        #region GameFilter
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandlerClass.Instance.FilterDiff = (string)comboBox2.SelectedItem;
            return;
        }
        

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            d2ngList.Items.Clear();
            if (!string.IsNullOrEmpty(textBox10.Text))
            {
                d2ngList.Items.Clear();
                foreach (Games game in games.Where(g => g.Name.ToLower().Contains(textBox10.Text.ToLower())))
                {
                    d2ngList.Items.Add(game.Name);
                }
            }
            else
            {
                foreach (Games game in games)
                {
                    d2ngList.Items.Add(game.Name);
                }
            }
                   
            
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
                if (!string.IsNullOrEmpty(textBox6.Text))
                {
                    foreach (BnetGames game in HandlerClass.Instance.Bnetgame.Where(g => g.Name.ToLower().Contains(textBox6.Text.ToLower())))
                    {
                        ListViewItem item = new ListViewItem(game.Name);
                        item.SubItems.Add(game.Players);
                        listView1.Items.Add(item);   
                    }
                        
                }
                else
                {
                    foreach (BnetGames game in HandlerClass.Instance.Bnetgame)
                    {
                        ListViewItem item = new ListViewItem(game.Name);
                        item.SubItems.Add(game.Players);
                        listView1.Items.Add(item);
                    }
                }
        }
        #endregion

        #region Set IRC Channel
        private void IrcChat_DocumentCompleted(object sender, EventArgs e)
        {
            setChannel = false;
        }
        #endregion

        private void button9_Click(object sender, EventArgs e)
        {
            textBox8.Text = configs.LoadConf("LifeChicken");
            textBox9.Text = configs.LoadConf("ManaChicken");
            textBox15.Text = configs.LoadConf("MercChicken");
            textBox16.Text = configs.LoadConf("TownHP");
            textBox17.Text = configs.LoadConf("TownMP");
            textBox11.Text = configs.LoadConf("FCR");
            textBox12.Text = configs.LoadConf("FHR");
            textBox13.Text = configs.LoadConf("FBR");
            textBox14.Text = configs.LoadConf("IAS");
            textBox18.Text = configs.LoadConf("UseHP");
            textBox22.Text = configs.LoadConf("UseRejuvHP");
            textBox21.Text = configs.LoadConf("UseMP");
            textBox20.Text = configs.LoadConf("UseRejuvMP");
            textBox19.Text = configs.LoadConf("UseMercHP");
            textBox23.Text = configs.LoadConf("UseMercRejuv");
            if (configs.LoadConf("PacketCasting") == "1")
                checkBox2.Checked = true;
            else
                checkBox2.Checked = false;
            if (configs.LoadConf("pickit") == "1")
                checkBox3.Checked = true;
            else
                checkBox3.Checked = false;
            panel3.Visible = true;
        }

        private void Numric(object sender, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
                e.Handled = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            setConf(false);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
                label29.ForeColor = Color.Goldenrod;
            else
                label29.ForeColor = Color.Gray;
        }
       
        private void button10_Click(object sender, EventArgs e)
        {
            setConf(true);
        }
        private void setConf(bool vis)
        {
            textBox8.Text = configs.LoadConf("LifeChicken");
            textBox9.Text = configs.LoadConf("ManaChicken");
            textBox15.Text = configs.LoadConf("MercChicken");
            textBox16.Text = configs.LoadConf("TownHP");
            textBox17.Text = configs.LoadConf("TownMP");
            textBox11.Text = configs.LoadConf("FCR");
            textBox12.Text = configs.LoadConf("FHR");
            textBox13.Text = configs.LoadConf("FBR");
            textBox14.Text = configs.LoadConf("IAS");
            textBox18.Text = configs.LoadConf("UseHP");
            textBox22.Text = configs.LoadConf("UseRejuvHP");
            textBox21.Text = configs.LoadConf("UseMP");
            textBox20.Text = configs.LoadConf("UseRejuvMP");
            textBox19.Text = configs.LoadConf("UseMercHP");
            textBox23.Text = configs.LoadConf("UseMercRejuv");
            if (configs.LoadConf("PacketCasting") == "1")
                checkBox2.Checked = true;
            else
                checkBox2.Checked = false;
            if (configs.LoadConf("pickit") == "1")
                checkBox3.Checked = true;
            else
                checkBox3.Checked = false;
            panel3.Visible = vis;
        }
    }
}
