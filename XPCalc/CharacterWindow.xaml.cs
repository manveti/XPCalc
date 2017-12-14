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
    /// Interaction logic for CharacterWindow.xaml
    /// </summary>
    public partial class CharacterWindow : Window {
        private SpinBox levelBox, totalXpBox, unspentXpBox;

        private bool valid = false;

        public CharacterWindow() {
            InitializeComponent();
            this.levelBox = new SpinBox();
            this.levelBox.Value = 1;
            this.levelBox.Minimum = 1;
            this.levelBox.ValueChanged += this.levelChanged;
            Grid.SetRow(this.levelBox, 1);
            Grid.SetColumn(this.levelBox, 1);
            Grid.SetColumnSpan(this.levelBox, 3);
            characterGrid.Children.Add(this.levelBox);
            this.totalXpBox = new SpinBox();
            this.totalXpBox.Value = 0;
            this.totalXpBox.Minimum = 0;
            this.totalXpBox.ValueChanged += this.totalXpChanged;
            Grid.SetRow(this.totalXpBox, 2);
            Grid.SetColumn(this.totalXpBox, 1);
            Grid.SetColumnSpan(this.totalXpBox, 3);
            characterGrid.Children.Add(this.totalXpBox);
            this.unspentXpBox = new SpinBox();
            this.unspentXpBox.Value = 0;
            this.unspentXpBox.Minimum = 0;
            this.unspentXpBox.ValueChanged += this.levelChanged;
            Grid.SetRow(this.unspentXpBox, 3);
            Grid.SetColumn(this.unspentXpBox, 1);
            Grid.SetColumnSpan(this.unspentXpBox, 3);
            characterGrid.Children.Add(this.unspentXpBox);
        }

        public void levelChanged(object sender, RoutedEventArgs e) {
            this.totalXpBox.Value = this.levelToXp((int)this.levelBox.Value) + (int)this.unspentXpBox.Value;
        }

        public void totalXpChanged(object sender, RoutedEventArgs e) {
            int level = 1, xp = (int)this.totalXpBox.Value;
            while (xp >= level * 1000) {
                xp -= level * 1000;
                level += 1;
            }
            this.levelBox.Value = level;
            this.unspentXpBox.Value = xp;
        }

        public void doOk(object sender, RoutedEventArgs e) {
            if (nameBox.Text.Length <= 0) {
                nameBox.Text = "Character";
            }
            this.valid = true;
            this.Close();
        }

        public void doCancel(object sender, RoutedEventArgs e) {
            this.Close();
        }

        public void setExisting(String name, int level, int xp) {
            nameBox.Text = name;
            this.levelBox.Value = level;
            this.totalXpBox.Value = this.levelToXp(level) + xp;
            this.unspentXpBox.Value = xp;
        }

        public int levelToXp(int level) {
            return level * (level - 1) * 500;
        }
        public bool isValid() {
            return this.valid;
        }

        public String name {
            get { return nameBox.Text; }
        }

        public int level {
            get { return (int)this.levelBox.Value; }
        }

        public int totalXp {
            get { return (int)this.totalXpBox.Value; }
        }

        public int unspentXp {
            get { return (int)this.unspentXpBox.Value; }
        }
    }
}
