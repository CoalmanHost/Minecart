using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Minecart
{
    public class Mod : IMinecartObject , IEquatable<Mod>
    {
        public string sourceFile;
        public string Name => $"{sourceFile.Split('\\').Last()}";

        public Mod()
        {

        }
        public Mod(string path)
        {
            sourceFile = MinecartProfile.modsDirName + $"\\{path.Split('\\').Last()}";
            File.WriteAllBytes(sourceFile, File.ReadAllBytes(path));
        }
        public void DeployTo(string path)
        {
            File.WriteAllBytes(path + $"\\{sourceFile.Split('\\').Last()}", File.ReadAllBytes(sourceFile));
        }

        public bool Equals(Mod other)
        {
            return sourceFile == other.sourceFile;
        }
    }
}
