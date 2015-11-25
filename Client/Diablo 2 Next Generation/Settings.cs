using Microsoft.Win32;
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
using System.IO.Compression;
using System.Diagnostics;
namespace Diablo_2_Next_Generation
{
    public partial class Settings : Form
    {
        
        string glide;
        public Settings()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
            {
                reg.SetValue("Diablo 2 Path", textBox1.Text);
                reg.SetValue("Settings", sAcc.Checked.ToString() + "," + sPW.Checked.ToString() + "," + sFS.Checked.ToString());
                this.Close();
            }

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "Game.exe";
            openFileDialog1.Title = "Select Diablo 2 Game.exe";
            openFileDialog1.Filter = ("Diablo 2 (Game.exe)|Game.exe");
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;
            openFileDialog1.ShowDialog();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            button1.FlatAppearance.BorderSize = 1;
            button1.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 1;
            button2.FlatStyle = FlatStyle.Flat;
            button3.FlatAppearance.BorderSize = 1;
            button3.FlatStyle = FlatStyle.Flat;
            button4.FlatAppearance.BorderSize = 1;
            button4.FlatStyle = FlatStyle.Flat;
           button1.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
           button2.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
           button3.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
           button4.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
           sAcc.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
           sPW.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
           sFS.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
           label1.Font = new Font(HandlerClass.Instance.ff, 10f, FontStyle.Regular);
           label2.Font = new Font(HandlerClass.Instance.ff, 8f, FontStyle.Regular);
            using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
            {

                textBox1.Text = reg.GetValue("Diablo 2 Path").ToString();
                string set = reg.GetValue("Settings").ToString();
                try
                {
                    string[] Set = set.Split(new[] {","}, StringSplitOptions.None);
                    if (Set[0].ToString() == "True")
                        sAcc.Checked = true;
                    if (Set[1].ToString() == "True")
                        sPW.Checked = true;
                    if (set[2].ToString() == "True")
                        sFS.Checked = true;
                }
                catch{}
                try
                {
                    glide = reg.GetValue("Diablo 2 Path").ToString();
                    glide = glide.Replace("Game.exe", "glide3x.dll");
                    glide = glide.Replace("game.exe", "glide3x.dll");
                }
                catch
                {
                    glide = string.Empty;
                }
                if (reg.GetValue("Diablo 2 Path").ToString() != "" && !File.Exists(glide))
                    button4.Enabled = true;

                else
                {
                    sFS.Checked = false;
                    sFS.Enabled = true;
                }

            }
            
            if(!button4.Enabled)
            {
                button4.ForeColor = Color.Red;
            }
        }
        WebClient downloader = new WebClient();
        private void button4_Click(object sender, EventArgs e)
        {
            using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
            {
                if(reg.GetValue("Diablo 2 Path").ToString() != "")
                {
                    MessageBox.Show("Will now download Glide!");
                    button4.Enabled = false;
                    
                    downloader.DownloadDataCompleted += downloader_DownloadDataCompleted;
                    downloader.DownloadDataAsync(new Uri("http://www.svenswrapper.de/gl32ogl14e.zip"), Environment.CurrentDirectory + "/d2bs/Glide.zip");
                }
                else
                {
                    MessageBox.Show("Select Game.exe First! before starting download!");
                }
            }
            
        }

        void downloader_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
            {
                downloader.Dispose();
                string exTo = reg.OpenSubKey("Diablo 2 Path").ToString();
                exTo = exTo.Replace("Game.exe", "");
                exTo = exTo.Replace("game.exe", "");
                System.IO.Compression.ZipFile.ExtractToDirectory(Environment.CurrentDirectory + "/d2bs/Glide.zip", exTo);
            }
            Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Software\GLIDE3toOpenGL", true);
            using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Software\GLIDE3toOpenGL", true))
            {
                reg.SetValue("corner", 0x0, RegistryValueKind.DWord);
                reg.SetValue("showfps", 0x0, RegistryValueKind.DWord);
                reg.SetValue("showclock", 0x0, RegistryValueKind.DWord);
                reg.SetValue("showtexturemass", 0x0, RegistryValueKind.DWord);
                reg.SetValue("sequence", 0x1, RegistryValueKind.DWord);
                reg.SetValue("GL_EXT_vertex_array", 0x1, RegistryValueKind.DWord);
                reg.SetValue("GL_ATI_fragment_shader", 0x0, RegistryValueKind.DWord);
                reg.SetValue("GL_ARB_fragment_program", 0x0, RegistryValueKind.DWord);
                reg.SetValue("GL_EXT_paletted_texture", 0x1, RegistryValueKind.DWord);
                reg.SetValue("GL_EXT_shared_texture_palette", 0x1, RegistryValueKind.DWord);
                reg.SetValue("GL_EXT_packed_pixels", 0x1, RegistryValueKind.DWord);
                reg.SetValue("GL_EXT_texture_env_combine", 0x1, RegistryValueKind.DWord);
                reg.SetValue("WGL_EXT_swap_control", 0x1, RegistryValueKind.DWord);
                reg.SetValue("windowed", 0x1, RegistryValueKind.DWord);
                reg.SetValue("capturedmouse", 0x0, RegistryValueKind.DWord);
                reg.SetValue("windowextras", 0x1, RegistryValueKind.DWord);
                reg.SetValue("aspectratio", 0x0, RegistryValueKind.DWord);
                reg.SetValue("refreshrate", 0x3c, RegistryValueKind.DWord);
                reg.SetValue("vsync", 0x0, RegistryValueKind.DWord);
                reg.SetValue("staticview", 0x0, RegistryValueKind.DWord);
                reg.SetValue("fpslimit", 0x0, RegistryValueKind.DWord);
                reg.SetValue("rememberpos", 0x0, RegistryValueKind.DWord);
                reg.SetValue("keepcomposition", 0x0, RegistryValueKind.DWord);
                reg.SetValue("texturemem", 0x6c, RegistryValueKind.DWord);
                reg.SetValue("texturesize", 0x0c, RegistryValueKind.DWord);
                reg.SetValue("32bitrenderwindow", 0x0, RegistryValueKind.DWord);
                reg.SetValue("texturevideos", 0x1, RegistryValueKind.DWord);
                reg.SetValue("rendertotexture", 0x1, RegistryValueKind.DWord);
                reg.SetValue("bilinear", 0x1, RegistryValueKind.DWord);
                reg.SetValue("supersampling", 0x1, RegistryValueKind.DWord);
                reg.SetValue("shadergamma", 0x1, RegistryValueKind.DWord);
                reg.SetValue("nogamma", 0x0, RegistryValueKind.DWord);
                reg.SetValue("d2_active", 0x0, RegistryValueKind.DWord);
            }
            DialogResult dialogResult = MessageBox.Show("Glide Installed need to run D2VidTest Then select Glide" + Environment.NewLine + Environment.NewLine + "Want to run D2VidTest now?", "D2VidTest", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
                {
                    string path = reg.OpenSubKey("Diablo 2 Path").ToString();
                    path = path.Replace("Game.exe", "D2VidTst.exe");
                    path = path.Replace("game.exe", "D2VidTst.exe");
                    var p = new Process();
                    p.StartInfo.FileName = path;  // just for example, you can use yours.
                    p.Start();
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\D2NG", true))
            {
                string path = reg.OpenSubKey("Diablo 2 Path").ToString();
                path = path.Replace("Game.exe", "glide-init.exe");
                path = path.Replace("game.exe", "glide-init.exe");
                var p = new Process();
                p.StartInfo.FileName = path;  // just for example, you can use yours.
                p.Start();
            }
        }
    }
}
