using Microsoft.WindowsAPICodePack.Dialogs;
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
using Microsoft.Win32;

namespace Minecart
{
    /// <summary>
    /// Логика взаимодействия для ModsWindow.xaml
    /// </summary>
    public partial class MinecartObjectsSelectionWindow : Window
    {
        MinecartProfile profile;
        public delegate void ApplyHandler(IEnumerable<IMinecartObject> objects);
        public event ApplyHandler OnApply;
        public delegate IMinecartObject MinecartObjectGetter(string objectName);
        MinecartObjectGetter getter;
        public MinecartObjectsSelectionWindow(MinecartProfile profile, IEnumerable<IMinecartObject> allObjects, MinecartObjectGetter getter)
        {
            this.profile = profile;
            InitializeComponent();
            foreach (var item in allObjects)
            {
                ObjectsListBox.Items.Add(item.Name);
            }
            ObjectsListBox.SelectionMode = SelectionMode.Multiple;
            this.getter = getter;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            List<IMinecartObject> selected = new List<IMinecartObject>();
            foreach (string objectName in ObjectsListBox.SelectedItems)
            {
                selected.Add(getter(objectName));
            }
            OnApply?.Invoke(selected);
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = MinecartProfile.modsDirName;
            dialog.Multiselect = true;
            dialog.ShowDialog();
            foreach (var item in dialog.FileNames)
            {
                Mod mod = new Mod(item);
                if (profile.GetMod(item) == null)
                {
                    profile.mods.Add(mod);
                    ObjectsListBox.Items.Add(mod.Name);
                }
            }
            profile.SaveSettings();
        }

        private void ObjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
