using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Minecart
{
    /// <summary>
    /// Логика взаимодействия для ModpackWindow.xaml
    /// </summary>
    public partial class ModpackWindow : Window
    {
        MinecartProfile profile;
        Modpack sourceModpack;
        Modpack currentModpack;
        public ModpackWindow(MinecartProfile profile, Modpack modpack)
        {
            this.profile = profile;
            sourceModpack = modpack;
            currentModpack = (Modpack)modpack.Clone();
            InitializeComponent();
            ModpackNameTextBox.Text = currentModpack.Name;
            ModpackIDLabel.Content = modpack.id;
            foreach (var mod in currentModpack.ModsList)
            {
                ModsListBox.Items.Add(mod.Name);
            }
            ModsListBox.SelectionMode = SelectionMode.Multiple;
            foreach (var pack in currentModpack.ChildrenModpacks)
            {
                ModpacksListBox.Items.Add(pack.Name);
            }
            StringBuilder parentsStringBuilder = new StringBuilder();
            foreach (var pack in currentModpack.ParentsModpacks)
            {
                parentsStringBuilder.Append($"{pack.Name}\n");
            }
            ParentsModpacksTextBlock.Text = parentsStringBuilder.ToString();
            ModpacksListBox.SelectionMode = SelectionMode.Multiple;
            RemoveModButton.IsEnabled = false;
            RemoveModpackButton.IsEnabled = false;
            UpdateLists();
        }

        private void SaveModpackButton_Click(object sender, RoutedEventArgs e)
        {
            currentModpack.Name = ModpackNameTextBox.Text;
            currentModpack.CopyTo(sourceModpack);
            sourceModpack.Pack();
            foreach (var pack in sourceModpack.ChildrenModpacks)
            {
                pack.Pack();
            }
            Close();
        }

        private void AddModButton_Click(object sender, RoutedEventArgs e)
        {
            MinecartObjectsSelectionWindow modsWindow = new MinecartObjectsSelectionWindow(profile, profile.mods, profile.GetMod);
            modsWindow.OnApply += ModsWindow_OnApply;
            modsWindow.ShowDialog();
            UpdateLists();
        }

        private void ModsWindow_OnApply(IEnumerable<IMinecartObject> selected)
        {
            foreach (var item in selected)
            {
                Mod mod = item as Mod;
                if (!currentModpack.ModsList.Contains(mod))
                {
                    currentModpack.ModsList.Add(mod);
                }
            }
            UpdateLists();
        }

        private void ModsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveModButton.IsEnabled = ModsListBox.SelectedItem != null;
        }
        private void RemoveModButton_Click(object sender, RoutedEventArgs e)
        {
            currentModpack.ModsList.Remove(currentModpack.ModsList.Find(m => m.Name == ModsListBox.SelectedItem.ToString()));
            UpdateLists();
        }

        private void ModpackNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
        void UpdateLists()
        {
            ModsListBox.Items.Clear();
            foreach (var mod in currentModpack.ModsList)
            {
                ModsListBox.Items.Add(mod.Name);
            }
            ModpacksListBox.Items.Clear();
            foreach (var pack in currentModpack.ChildrenModpacks)
            {
                ModpacksListBox.Items.Add(pack.Name);
            }
            ParentsModpacksTextBlock.Text = "";
            StringBuilder parentsStringBuilder = new StringBuilder();
            foreach (var pack in currentModpack.ParentsModpacks)
            {
                parentsStringBuilder.Append($"{pack.Name}\n");
            }
            ParentsModpacksTextBlock.Text = parentsStringBuilder.ToString();
        }

        private void ModpacksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void AddModpackButton_Click(object sender, RoutedEventArgs e)
        {
            List<Modpack> modpacks = new List<Modpack>();
            foreach (var pack in profile.modpacks)
            {
                if (sourceModpack != pack)
                {
                    modpacks.Add(pack);
                }
            }
            MinecartObjectsSelectionWindow addWindow = new MinecartObjectsSelectionWindow(profile, modpacks, profile.GetModpack);
            addWindow.AddButton.IsEnabled = false;
            addWindow.OnApply += (IEnumerable<IMinecartObject> selected) =>
            {
                foreach (Modpack pack in selected)
                {
                    currentModpack.AddChild(pack);
                }
            };
            UpdateLists();
            addWindow.ShowDialog();
        }

        private void RemoveModpackButton_Click(object sender, RoutedEventArgs e)
        {
            List<Modpack> toBeRemoved = currentModpack.ChildrenModpacks.FindAll(m => ModpacksListBox.SelectedItems.Contains(m.Name));
            foreach (var pack in toBeRemoved)
            {
                currentModpack.ChildrenModpacks.Remove(pack);
            }
            UpdateLists();
        }

        private void DeleteModpackButton_Click(object sender, RoutedEventArgs e)
        {
            profile.modpacks.Remove(sourceModpack);
            Close();
        }

        private void ModpacksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveModpackButton.IsEnabled = ModpacksListBox.SelectedItem != null;
        }
    }
}
