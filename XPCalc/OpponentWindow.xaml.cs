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
using Storage;

namespace XPCalc {
    /// <summary>
    /// Interaction logic for OpponentWindow.xaml
    /// </summary>
    public partial class OpponentWindow : Window {
        private SpinBox crBox, countBox;

        private DictionaryStore<string, int> opponents;
        private bool valid = false;

        public OpponentWindow(DictionaryStore<string, int> opponents) {
            InitializeComponent();
            this.opponents = opponents;
            foreach (String name in opponents.Keys.OrderBy(k => k)) {
                nameBox.Items.Add(name);
            }
            this.crBox = new SpinBox();
            this.crBox.Value = 1;
            this.crBox.Minimum = 1;
            Grid.SetRow(this.crBox, 1);
            Grid.SetColumn(this.crBox, 1);
            opponentGrid.Children.Add(this.crBox);
            this.countBox = new SpinBox();
            this.countBox.Value = 1;
            this.countBox.Minimum = 1;
            Grid.SetRow(this.countBox, 2);
            Grid.SetColumn(this.countBox, 1);
            opponentGrid.Children.Add(this.countBox);
        }

        public void nameChanged(object sender, SelectionChangedEventArgs e) {
            if (!this.opponents.ContainsKey((String)nameBox.SelectedItem)) { return; }
            this.crBox.Value = this.opponents[(String)nameBox.SelectedItem];
        }

        public void doRemove(object sender, RoutedEventArgs e) {
            if (!this.opponents.ContainsKey(nameBox.Text)) { return; }
            this.opponents.Remove(nameBox.Text);
            this.opponents.save();
            nameBox.Items.Remove(nameBox.Text);
        }

        public void doOk(object sender, RoutedEventArgs e) {
            if (nameBox.Text.Length <= 0) {
                nameBox.Text = "Opponent";
            }
            if (saveBox.IsChecked.Value) {
                if ((!this.opponents.ContainsKey(nameBox.Text)) || (this.opponents[nameBox.Text] != (int)this.crBox.Value)) {
                    this.opponents[nameBox.Text] = (int)this.crBox.Value;
                    this.opponents.save();
                }
            }
            this.valid = true;
            this.Close();
        }

        public void doCancel(object sender, RoutedEventArgs e) {
            this.Close();
        }

        public void setExisting(String name, int cr, int count) {
            nameBox.Text = name;
            this.crBox.Value = cr;
            this.countBox.Value = count;
            if (!this.opponents.ContainsKey(name)) {
                saveBox.IsChecked = false;
            }
        }

        public bool isValid() {
            return this.valid;
        }

        public String name {
            get { return nameBox.Text; }
        }

        public int cr {
            get { return (int)this.crBox.Value; }
        }

        public int count {
            get { return (int)this.countBox.Value; }
        }
    }
}
