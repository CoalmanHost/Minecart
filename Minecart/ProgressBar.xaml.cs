using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window
    {
        public delegate void ProgressTask(Progress progress);
        public int value;
        Progress progress;
        public ProgressBar(string processName, Progress progress, ProgressTask task)
        {
            InitializeComponent();
            Title = processName;
            this.progress = progress;
            ProcessProgressBar.Minimum = this.progress.minValue;
            ProcessProgressBar.Maximum = this.progress.maxValue;
            Task progressTask = new Task(() => { task(progress); });
            this.progress.OnStep += () => { Dispatcher.Invoke(() => { ProcessProgressBar.Value = progress.value; }); };
            progressTask.Start();
        }

        private void ProcessProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ProcessProgressBar.Value >= ProcessProgressBar.Maximum)
            {
                Close();
            }
        }
    }
}
