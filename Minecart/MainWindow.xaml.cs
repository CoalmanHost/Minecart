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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Minecart
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {
        MinecartProfile profile;

        public MainWindow()
        {
            InitializeComponent();
            if (!MinecartProfile.Exists())
            {
                profile = new MinecartProfile();
            }
            profile = MinecartProfile.LoadProfile();
            PathTextBox.Text = profile.gamedir;
            PathTextBox.IsReadOnly = true;
            profile.SaveSettings();
            foreach (var pack in profile.modpacks)
            {
                ModpacksListBox.Items.Add(pack.Name);
            }
            EditModpackButton.IsEnabled = false;
            DeployButton.IsEnabled = false;
        }

        private void ModsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CheckPathButton_Click(object sender, RoutedEventArgs e)
        {
            profile.gamedir = PathTextBox.Text;
            profile.SaveSettings();
        }

        private void AddModPackButton_Click(object sender, RoutedEventArgs e)
        {
            Modpack modpack = new Modpack("New modpack");
            profile.AddModpack(modpack);
            modpack.Pack();
            ModpacksListBox.Items.Add(modpack.Name);
            profile.SaveSettings();
        }

        private void ModpacksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModsListBox.Items.Clear();
            if (ModpacksListBox.SelectedItems.Count == 1)
            {
                foreach (var item in profile.GetModpack(ModpacksListBox.SelectedItem.ToString()).GetAllMods())
                {
                    ModsListBox.Items.Add(item.Name);
                }
            }
            EditModpackButton.IsEnabled = ModpacksListBox.SelectedItem != null;
            DeployButton.IsEnabled = ModpacksListBox.SelectedItem != null;
        }

        private void SetPathButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = MinecartProfile.modsDirName;
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            dialog.ShowDialog();
            foreach (var item in dialog.FileNames)
            {
                profile.gamedir = item;
            }
            PathTextBox.Text = profile.gamedir;
            profile.SaveSettings();
        }

        private void EditModpackButton_Click(object sender, RoutedEventArgs e)
        {
            ModpackWindow modpackWindow = new ModpackWindow(profile, profile.GetModpack(ModpacksListBox.SelectedItem.ToString()));
            modpackWindow.ShowDialog();
            ModpacksListBox.Items.Clear();
            foreach (var pack in profile.modpacks)
            {
                ModpacksListBox.Items.Add(pack.Name);
            }
            ModsListBox.Items.Clear();
            profile.SaveSettings();
        }

        private void DeployButton_Click(object sender, RoutedEventArgs e)
        {
            profile.DetachActiveModpack();
            //profile.DeployModpack(profile.GetModpack(ModpacksListBox.SelectedItem.ToString()));
            Modpack pack = profile.GetModpack(ModpacksListBox.SelectedItem.ToString());
            IEnumerable<Mod> mods = pack.GetAllMods();
            Progress progress = new Progress(mods.Count());
            ProgressBar progressWindow = new ProgressBar($"Deployment of \"{pack.Name}\" modpack...", progress, (Progress p) => { profile.DeployModpackProgressive(pack, p); });
            progressWindow.ShowDialog();
            ModsListBox.SelectedItem = null;
        }
    }
}
