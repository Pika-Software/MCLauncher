using Newtonsoft.Json;
using System;
using System.IO;

namespace Console_Launcher
{
    class UserConfig
    {

        public string Nickname;
        public string JavaPath;
        public string Version;
        public int[] Memory;

        private string file_path;
        public UserConfig( string path )
        {
            file_path = path;
            if (File.Exists(path)) {
                using (StreamReader file = new StreamReader(path))
                {
                    UserConfig config = JsonConvert.DeserializeObject<UserConfig>(file.ReadToEnd());
                    this.Nickname = config.Nickname;
                    this.JavaPath = config.JavaPath;
                    this.Version = config.Version;
                    this.Memory = config.Memory;
                }
            }
        }

        public bool Save()
        {
            if (file_path != null)
            {
                File.WriteAllLinesAsync(file_path, JsonConvert.SerializeObject(this).Split("\n"));
                return true;
            }

            return false;
        }

    }
}
