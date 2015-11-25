using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diablo_2_Next_Generation
{
    public class Games
    {
        public string ID;
        public string Name;
        public string Password;
        public string Diff;
        public string Description;
        public string Ladder;
        public string Realm;
        public string Time;
        public bool selected = false;
        public List<string> Characters = new List<string>();
    }
}
