using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Minecart
{
    public class MinecartProfile
    {
        public class ModpacksList : List<Modpack>
        {
            public new bool Remove(Modpack item)
            {
                item.Detach();
                return base.Remove(item);
            }
        }

        public string gamedir;
        [JsonIgnore]
        public string GameModsDir { get { return $"{gamedir}\\mods"; } }
        public static string settingsFileName = Directory.GetCurrentDirectory() + "\\minecart-settings.json";
        public static string modsDirName = Directory.GetCurrentDirectory() + "\\mods";
        public static string modpacksDirName = Directory.GetCurrentDirectory() + "\\modpacks";
        [JsonIgnore]
        public List<Mod> mods;
        [JsonIgnore]
        public ModpacksList modpacks;
        public MinecartProfile()
        {
            mods = new List<Mod>();
            modpacks = new ModpacksList();
            Directory.CreateDirectory(modsDirName);
            Directory.CreateDirectory(modpacksDirName);
            File.WriteAllText(settingsFileName, JsonConvert.SerializeObject(this));
        }
        public MinecartProfile(string gamedir) : this()
        {
            this.gamedir = gamedir;
        }
        public void AddModpack(Modpack pack)
        {
            modpacks.Add(pack);
        }
        public Modpack GetModpack(string name)
        {
            return modpacks.ToList().Find(m => m.Name == name);
        }
        public Mod GetMod(string name)
        {
            return mods.Find(m => m.Name == name);
        }
        public void UpdateLists()
        {
            modpacks.Clear();
        }
        public void SaveSettings()
        {
            File.WriteAllText(settingsFileName, JsonConvert.SerializeObject(this));
        }
        public static MinecartProfile LoadProfile()
        {
            MinecartProfile profile = JsonConvert.DeserializeObject<MinecartProfile>(File.ReadAllText(settingsFileName));
            if (profile.mods == null)
            {
                profile.mods = new List<Mod>();
            }
            foreach (var mod in Directory.GetFiles(modsDirName))
            {
                profile.mods.Add(new Mod(mod));
            }
            if (profile.modpacks == null)
            {
                profile.modpacks = new ModpacksList();
            }
            foreach (var modpack in Directory.GetFiles(modpacksDirName))
            {
                Modpack pack = Modpack.Unpack(File.ReadAllText(modpack));
                profile.modpacks.Add(pack);
            }
            foreach (var pack in profile.modpacks)
            {
                pack.UnpackRelatedModpacks(profile.modpacks);
            }

            Directory.CreateDirectory(profile.GameModsDir);
            return profile;
        }

        public void DeployModpack(Modpack modpack)
        {
            Directory.CreateDirectory(GameModsDir);
            foreach (var mod in modpack.GetAllMods())
            {
                mod.DeployTo(GameModsDir);
            }
        }

        public void DeployModpackProgressive(Modpack modpack, Progress progress)
        {
            Directory.CreateDirectory(GameModsDir);
            foreach (var mod in modpack.GetAllMods())
            {
                mod.DeployTo(GameModsDir);
                progress.Step();
            }
        }

        public void DetachActiveModpack()
        {
            if (Directory.Exists(GameModsDir))
            {
                Directory.Delete(GameModsDir, true);
            }
        }
        public static bool Exists()
        {
            return File.Exists(settingsFileName);
        }
    }
}
