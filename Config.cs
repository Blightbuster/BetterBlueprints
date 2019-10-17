using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BetterBlueprints
{
    class Config
    {
        public string Path;
        public string Section;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        public Config(string configPath, string section)
        {
            Path = configPath;
            Section = section;
        }

        public void WriteString(string key, string value, string section = "")
        {
            if (section != "") Section = section;
            WritePrivateProfileString(Section, key, value, Path);
        }

        public void WriteInt(string key, int value, string section = "")
        {
            WriteString(key, value.ToString(), section);
        }

        public void WriteFloat(string key, float value, string section = "")
        {
            WriteString(key, value.ToString(), section);
        }

        public void WriteBool(string key, bool value, string section = "")
        {
            WriteString(key, value.ToString(), section);
        }

        public string ReadString(string key, string defaultValue = "", string section = "")
        {
            try
            {
                StringBuilder temp = new StringBuilder(255);
                if (section != "") Section = section;
                GetPrivateProfileString(Section, key, "", temp, 255, Path);
                return temp.ToString();
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }

        public int ReadInt(string key, int defaultValue = 0, string section = "")
        {
            try
            {
                return Int32.Parse(ReadString(key, "0", section));
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }

        public float ReadFloat(string key, float defaultValue = 0, string section = "")
        {
            try
            {
                return float.Parse(ReadString(key, "0", section));
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }

        public bool ReadBool(string key, bool defaultValue = false, string section = "")
        {
            try
            {
                return bool.Parse(ReadString(key, "false", section));
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }
    }
}