using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

namespace Minecart
{
    public class Modpack : IMinecartObject , ICloneable , IEquatable<Modpack>
    {
        public class RelatedModpacksCollection : List<Modpack>
        {
            Modpack parent;

            public RelatedModpacksCollection()
            { }
            public RelatedModpacksCollection(Modpack baseModpack)
            {
                parent = baseModpack;
            }

            public new void Add(Modpack item)
            {
                if (HasInRelates(parent))
                {
                    return;
                }
                base.Add(item);
            }

            public new bool Remove(Modpack item)
            {
                if (parent.ChildrenModpacks.Contains(item))
                {
                    return base.Remove(item) && ((List<Modpack>)(item.ParentsModpacks)).Remove(parent);
                }
                else if (parent.ParentsModpacks.Contains(item))
                {
                    return base.Remove(item) && ((List<Modpack>)(item.ChildrenModpacks)).Remove(parent);
                }
                return false;
            }

            /*
             * TODO Две различные функции для обхода вверх и вниз по иерархии паков
             */
            bool HasInRelates(Modpack pack)
            {
                return HasInChildren(pack) || HasInParents(pack);
            }
            bool HasInParents(Modpack pack)
            {
                foreach (var parent in pack.ParentsModpacks)
                {
                    if (parent == pack)
                    {
                        return true;
                    }
                    else
                    {
                        return parent.ParentsModpacks.HasInParents(parent);
                    }
                }
                return false;
            }
            bool HasInChildren(Modpack pack)
            {
                foreach (var child in pack.ChildrenModpacks)
                {
                    if (child == pack)
                    {
                        return true;
                    }
                    else
                    {
                        return child.ChildrenModpacks.HasInChildren(child);
                    }
                }
                return false;
            }
        }
        [JsonProperty(PropertyName = "ID")]
        public readonly int id;
        string name;
        [JsonIgnore]
        string infoFile;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (infoFile != MinecartProfile.modpacksDirName + $"\\{value}.json")
                {
                    if (infoFile != null)
                    {
                        File.Delete(infoFile);
                    }
                    infoFile = MinecartProfile.modpacksDirName + $"\\{value}.json";
                }
                name = value;
            }
        }
        public List<int> children;
        public List<int> parents;
        [JsonIgnore]
        public RelatedModpacksCollection ChildrenModpacks;
        [JsonIgnore]
        public RelatedModpacksCollection ParentsModpacks;

        public List<Mod> ModsList;

        public Modpack(string name)
        {
            this.name = name;
            infoFile = MinecartProfile.modpacksDirName + $"\\{Name}.json";
            ModsList = new List<Mod>();
            children = new List<int>();
            parents = new List<int>();
            ChildrenModpacks = new RelatedModpacksCollection(this);
            ParentsModpacks = new RelatedModpacksCollection(this);
            id = GetHashCode();
        }

        Modpack(Modpack source)
        {
            id = source.id;
        }

        public void AddChild(Modpack pack)
        {
            ChildrenModpacks.Add(pack);
            pack.ParentsModpacks.Add(this);
            children.Add(pack.id);
            pack.parents.Add(id);
        }

        public void Detach()
        {
            foreach (var parent in ParentsModpacks)
            {
                parent.ChildrenModpacks.Remove(this);
                parent.children.Remove(id);
            }
        }

        public IEnumerable<Mod> GetAllMods()
        {
            List<Mod> mods = new List<Mod>();
            mods.AddRange(ModsList);
            foreach (var child in ChildrenModpacks)
            {
                mods.AddRange(child.GetAllMods());
            }
            return mods;
        }

        public void Pack()
        {
            File.WriteAllText(MinecartProfile.modpacksDirName + $"\\{Name}.json", JsonConvert.SerializeObject(this));
        }
        public static Modpack Unpack(string modpackInfo)
        {
            Modpack pack = JsonConvert.DeserializeObject<Modpack>(modpackInfo);
            pack.infoFile = MinecartProfile.modpacksDirName + $"\\{pack.Name}.json";
            if (pack.children == null)
            {
                pack.children = new List<int>();
            }
            if (pack.parents == null)
            {
                pack.parents = new List<int>();
            }
            return pack;
        }
        public void UnpackRelatedModpacks(List<Modpack> environment)
        {
            foreach (var childID in children)
            {
                ChildrenModpacks.Add(environment.Find(c => c.id == childID));
            }
            foreach (var parentID in parents)
            {
                ParentsModpacks.Add(environment.Find(p => p.id == parentID));
            }
        }

        public object Clone()
        {
            Modpack pack = new Modpack(this);
            pack.name = name;
            pack.infoFile = $"{infoFile}";
            pack.ModsList = new List<Mod>();
            pack.ModsList.AddRange(ModsList);
            pack.children = new List<int>();
            pack.children.AddRange(children);
            pack.parents = new List<int>();
            pack.parents.AddRange(parents);
            pack.ParentsModpacks = new RelatedModpacksCollection(pack);
            pack.ParentsModpacks.AddRange(ParentsModpacks);
            pack.ChildrenModpacks = new RelatedModpacksCollection(pack);
            pack.ChildrenModpacks.AddRange(ChildrenModpacks);
            return pack;
        }
        public void CopyTo(Modpack target)
        {
            target.name = name;
            target.infoFile = $"{infoFile}";
            target.ModsList = new List<Mod>();
            target.ModsList.AddRange(ModsList);
            target.children = new List<int>();
            target.children.AddRange(children);
            target.parents = new List<int>();
            target.parents.AddRange(parents);
            target.ParentsModpacks = new RelatedModpacksCollection(target);
            target.ParentsModpacks.AddRange(ParentsModpacks);
            target.ChildrenModpacks = new RelatedModpacksCollection(target);
            target.ChildrenModpacks.AddRange(ChildrenModpacks);
        }

        public bool Equals(Modpack other)
        {
            return id == other.id;
        }
    }
}
