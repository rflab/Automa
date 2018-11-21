using System.Runtime.InteropServices;
using System.Text;

namespace Automa
{
    class IniFile
    {
        string filePath_;
        const string DEFAULT_SECTION = "Config";

        [DllImport("KERNEL32.DLL")]
        public static extern uint GetPrivateProfileString(
            string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);
        [DllImport("KERNEL32.DLL")]
        public static extern uint GetPrivateProfileInt(
            string lpAppName, string lpKeyName, int nDefault, string lpFileName);
        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(
            string lpAppName, string lpKeyName, string lpstring, string lpFileName);
        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileInt(
            string lpAppName, string lpKeyName, long nDefault, string lpFileName);

        public IniFile(string filePath = "config.ini")
        {
            filePath_ = filePath;
        }

        public string Load(string key, string defaultStr)
        {
            StringBuilder sb = new StringBuilder(256);
            GetPrivateProfileString(DEFAULT_SECTION, key, defaultStr, sb, (uint)sb.Capacity, filePath_);
            return sb.ToString();
        }

        public int Load(string key, int defaultVal)
        {
            return (int)GetPrivateProfileInt(DEFAULT_SECTION, key, defaultVal, filePath_);
        }

        public void Save(string key, string val)
        {
            WritePrivateProfileString(DEFAULT_SECTION, key, val, filePath_);
        }

        public void Save(string key, int val)
        {
            WritePrivateProfileString(DEFAULT_SECTION, key, val.ToString(), filePath_);
        }
    }
}