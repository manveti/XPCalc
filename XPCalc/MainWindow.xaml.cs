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

using GUIx;

namespace XPCalc {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        enum XpError {
            Success,
            Failure,
            ELTooLow,
            ELTooHigh
        }

        private SpinBox elBox, partyLevelBox, partySizeBox;
        private XpError simpleErr = XpError.Success;

        public MainWindow() {
            InitializeComponent();
            this.elBox = new SpinBox();
            this.elBox.Value = 1;
            this.elBox.Minimum = 1;
            this.elBox.ValueChanged += this.calculateSimpleXp;
            Grid.SetRow(this.elBox, 0);
            Grid.SetColumn(this.elBox, 1);
            simpleGrid.Children.Add(this.elBox);
            this.partyLevelBox = new SpinBox();
            this.partyLevelBox.Value = 1;
            this.partyLevelBox.Minimum = 1;
            this.partyLevelBox.ValueChanged += this.calculateSimpleXp;
            Grid.SetRow(this.partyLevelBox, 0);
            Grid.SetColumn(this.partyLevelBox, 3);
            simpleGrid.Children.Add(this.partyLevelBox);
            this.partySizeBox = new SpinBox();
            this.partySizeBox.Value = 4;
            this.partySizeBox.Minimum = 1;
            this.partySizeBox.ValueChanged += this.calculateSimpleXp;
            Grid.SetRow(this.partySizeBox, 0);
            Grid.SetColumn(this.partySizeBox, 5);
            simpleGrid.Children.Add(this.partySizeBox);
            this.calculateSimpleXp(null, null);
        }

        public void calculateSimpleXp(object sender, RoutedEventArgs e) {
            int el, level, count;
            if (!int.TryParse(this.elBox.Text, out el)) {
                MessageBox.Show("Unable to parse encounter level '" + this.elBox.Text + "'", "Error");
                return;
            }
            if (el < 1) {
                MessageBox.Show("Encounter level must be at least 1", "Error");
                return;
            }
            if (!int.TryParse(this.partyLevelBox.Text, out level)) {
                MessageBox.Show("Unable to parse party level '" + this.partyLevelBox.Text + "'", "Error");
                return;
            }
            if (level < 1) {
                MessageBox.Show("Party level must be at least 1", "Error");
                return;
            }
            if (!int.TryParse(this.partySizeBox.Text, out count)) {
                MessageBox.Show("Unable to parse party size'" + this.partySizeBox.Text + "'", "Error");
                return;
            }
            if (count < 1) {
                MessageBox.Show("Party size must be at least 1", "Error");
                return;
            }
            xpBox.Text = "" + this.calculateXp(el, level, count, out this.simpleErr);
            this.simpleNotesBut.IsEnabled = (this.simpleErr != XpError.Success);
        }

        private void getSimpleNotes(object sender, RoutedEventArgs e) {
            String err = "An error occurred", severity = "Error";
            switch (this.simpleErr) {
            case XpError.ELTooLow:
                err = "Encounter is more than 7 levels below party; no XP will be awarded";
                severity = "Notice";
                break;
            case XpError.ELTooHigh:
                err = "Encounter is more than 7 levels above party; consider alternate XP award";
                severity = "Warning";
                break;
            }
            MessageBox.Show(err, severity);
        }

        private int calculateXp(int cr, int level, int count, out XpError err) {
            double xp = 0;
            err = XpError.Success;
            if (level <= 3) {
                level = 3;
            }
            if ((cr == 1) && (level < 6)) {
                level = 6;
            }
            int diff = cr - level;
            if (diff < -7) {
                err = XpError.ELTooLow;
                return 0;
            }
            if (diff > 7) {
                err = XpError.ELTooHigh;
            }
            for (xp = 300; diff < -1; diff += 2) {
                xp /= 2;
            }
            for (; diff > 1; diff -= 2) {
                xp *= 2;
            }
            if ((diff < 0) || ((diff > 0) && (level == 4))) {
                xp *= (diff > 0 ? 4 : 2);
                xp /= 3;
            }
            else if (diff > 0) {
                xp *= 1.5;
            }
            xp = Math.Round(xp * level, MidpointRounding.AwayFromZero); // total encounter xp
            return (int)Math.Round(xp / count, MidpointRounding.AwayFromZero);
        }
    }
}
