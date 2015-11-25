using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Diablo_2_Next_Generation
{
    public class ConfigReader
    {
        #region ConfigTemplate - template
        private string template = 
@"function LoadConfig() {
Scripts.clickHelper = true;
Scripts.UserAddon = true;
Config.HealHP = 50;
Config.HealMP = 0;
Config.HealStatus = false;
Config.UseMerc = true;
Config.MercWatch = false;
Config.UseHP = 75;
Config.UseRejuvHP = 40;
Config.UseMP = 30;
Config.UseRejuvMP = 0;
Config.UseMercHP = 75;
Config.UseMercRejuv = 0;
Config.HPBuffer = 0;
Config.MPBuffer = 0;
Config.RejuvBuffer = 0;
Config.PublicMode = 0;
Config.AutoMap = true;
Config.LastMessage = """";
Config.MFSwitchPercent = 0;
Config.MFSwitch = 0;
Config.FCR = 0;
Config.FHR = 0;
Config.FBR = 0;
Config.IAS = 0;
Config.PacketCasting = 0;
Config.WaypointMenu = false;
Config.AntiHostile = false;
Config.HostileAction = 0;
Config.TownOnHostile = false;
Config.RandomPrecast = false;
Config.ViperCheck = false;
Config.StopOnDClone = false;
Config.SoJWaitTime = 5;
Config.KillDclone = false;
Config.DCloneQuit = false;
Config.SkipImmune = [];
Config.SkipEnchant = [];
Config.SkipAura = [];
Config.LifeChicken = 30;
Config.ManaChicken = 0;
Config.MercChicken = 0;
Config.TownHP = 0;
Config.TownMP = 0;
Config.AttackSkill[0] = -1;
Config.AttackSkill[1] = -1;
Config.AttackSkill[2] = -1;
Config.AttackSkill[3] = -1;
Config.AttackSkill[4] = -1;
Config.AttackSkill[5] = -1;
Config.AttackSkill[6] = -1;
}";
        #endregion
        string[] configBuilder;
        string configToSave;
        public bool Config = true;
        public bool cheatsEnable = true;
        IniFile MyIni = new IniFile("gameConfig.ini");
        public ConfigReader()
        {
            configBuilder = template.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (!File.Exists(Directory.GetCurrentDirectory() + "/gameConfig.ini"))
                Config = false;
        }

        public string LoadConf(string box)
        {
            return MyIni.Read(box);
        }

        public bool Save(string setting, string Value)
        {
            try
            {
                MyIni.Write(setting, Value);
                return true; 
            }
            catch { return false; }
        }

        public void SaveD2Conf()
        {

        }

        public bool Reset(string value)
        {
            try
            {
                MyIni.Read(value);
                return true;
            }
            catch { return false; }
        }

    }

    class IniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}
