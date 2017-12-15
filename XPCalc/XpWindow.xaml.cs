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

using GUIx;

namespace XPCalc {
    /// <summary>
    /// Interaction logic for XpWindow.xaml
    /// </summary>
    public partial class XpWindow : Window {
        SpinBox xpAdjustBox, totalXpBox;

        private bool valid = false;
        private int encounterXp;

        public XpWindow(String character, int encounterXp, int xpAdjust) {
            InitializeComponent();
            this.nameBox.Text = character;
            this.encounterXp = encounterXp;
            this.encounterXpBox.Text = "" + encounterXp;
            this.xpAdjustBox = new SpinBox();
            this.xpAdjustBox.Value = xpAdjust;
            this.xpAdjustBox.ValueChanged += this.xpAdjustChanged;
            Grid.SetRow(this.xpAdjustBox, 2);
            Grid.SetColumn(this.xpAdjustBox, 1);
            Grid.SetColumnSpan(this.xpAdjustBox, 3);
            xpGrid.Children.Add(this.xpAdjustBox);
            this.totalXpBox = new SpinBox();
            this.totalXpBox.Value = encounterXp + xpAdjust;
            this.totalXpBox.ValueChanged += this.totalXpChanged;
            Grid.SetRow(this.totalXpBox, 3);
            Grid.SetColumn(this.totalXpBox, 1);
            Grid.SetColumnSpan(this.totalXpBox, 3);
            xpGrid.Children.Add(this.totalXpBox);
        }

        public void xpAdjustChanged(object sender, RoutedEventArgs e) {
            this.totalXpBox.Value = this.encounterXp + (int)this.xpAdjustBox.Value;
        }

        public void totalXpChanged(object sender, RoutedEventArgs e) {
            this.xpAdjustBox.Value = (int)this.totalXpBox.Value - this.encounterXp;
        }

        public void doOk(object sender, RoutedEventArgs e) {
            this.valid = true;
            this.Close();
        }

        public void doCancel(object sender, RoutedEventArgs e) {
            this.Close();
        }

        public bool isValid() {
            return this.valid;
        }

        public int xpAdjust {
            get { return (int)this.xpAdjustBox.Value; }
        }
    }
}
