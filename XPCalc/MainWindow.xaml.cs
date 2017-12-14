using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Storage;

namespace XPCalc {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        enum XpError {
            Success,
            Failure,
            ELTooLow,
            ELTooHigh,
            OverLevel
        }

        class OpponentRow {
            public String name { get; set; }
            public int cr { get; set; }
            public int count { get; set; }
        }

        class CharacterRow {
            public bool present { get; set; }
            public String name { get; set; }
            public int level { get; set; }
            public int totalXp { get; set; }
            public int unspentXp { get; set; }
        }

        class XpRow {
            public String character { get; set; }
            public int xp { get; set; }
            public String notes { get; set; }
            public XpError err { get; set; }
            public String errMsg { get; set; }
        }

        private SpinBox elBox, partyLevelBox, partySizeBox, partyXpBox;
        private XpError simpleErr = XpError.Success;
        private readonly String dataDir;
        private DictionaryStore<string, string> preferences;
        private DictionaryStore<string, int> opponents;
        private bool partyChanged = false;

        public MainWindow() {
            InitializeComponent();
            this.dataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.dataDir = System.IO.Path.Combine(this.dataDir, "XPCalc");
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
            this.opponentList.Items.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));
            this.partyList.Items.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));
            //...
            this.partyXpBox = new SpinBox();
            this.partyXpBox.Value = 0;
            Grid.SetRow(this.partyXpBox, 1);
            Grid.SetColumn(this.partyXpBox, 1);
            Grid.SetColumnSpan(this.partyXpBox, 3);
            xpGrid.Children.Add(this.partyXpBox);
            System.IO.Directory.CreateDirectory(this.dataDir);
            this.preferences = new DictionaryStore<string, string>(System.IO.Path.Combine(this.dataDir, "prefs.cfg"));
            this.opponents = new DictionaryStore<string, int>(System.IO.Path.Combine(this.dataDir, "opponents.db"));
        }

        private void calculateSimpleXp(object sender, RoutedEventArgs e) {
            if (this.elBox.Value < 1) {
                MessageBox.Show("Encounter level must be at least 1", "Error");
                return;
            }
            if (this.partyLevelBox.Value < 1) {
                MessageBox.Show("Party level must be at least 1", "Error");
                return;
            }
            if (this.partySizeBox.Value < 1) {
                MessageBox.Show("Party size must be at least 1", "Error");
                return;
            }
            xpBox.Text = "" + this.calculateXp((int)this.elBox.Value, (int)this.partyLevelBox.Value, (int)this.partySizeBox.Value, out this.simpleErr);
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
            case XpError.OverLevel:
                err = "XP award is more than two levels' worth; truncated to 1XP below second level-up";
                severity = "Notice";
                break;
            }
            MessageBox.Show(err, severity);
        }

        private void addOpponent(object sender, RoutedEventArgs e) {
            OpponentWindow ow = new OpponentWindow(this.opponents);
            ow.ShowDialog();
            if (!ow.isValid()) { return; }
            this.opponentList.Items.Add(new OpponentRow { name = ow.name, cr = ow.cr, count = ow.count });
            this.opponentList.Items.Refresh();
        }

        private void editOpponent(object sender, RoutedEventArgs e) {
            OpponentRow selected = (OpponentRow)this.opponentList.SelectedItem;
            OpponentWindow ow = new OpponentWindow(this.opponents);
            ow.setExisting(selected.name, selected.cr, selected.count);
            ow.ShowDialog();
            if (!ow.isValid()) { return; }
            selected.name = ow.name;
            selected.cr = ow.cr;
            selected.count = ow.count;
            SortDescription sd = this.opponentList.Items.SortDescriptions[0];
            this.opponentList.Items.SortDescriptions.Clear();
            this.opponentList.Items.SortDescriptions.Add(sd);
        }

        private void removeOpponent(object sender, RoutedEventArgs e) {
            OpponentRow selected = (OpponentRow)this.opponentList.SelectedItem;
            if (selected == null) { return; }
            this.opponentList.Items.Remove(selected);
            this.opponentList.Items.Refresh();
        }

        //party load/save/save as (see this.partyChanged)

        private void addCharacter(object sender, RoutedEventArgs e) {
            CharacterWindow cw = new CharacterWindow();
            cw.ShowDialog();
            if (!cw.isValid()) { return; }
            this.partyChanged = true;
            this.partyList.Items.Add(new CharacterRow { present = true, name = cw.name, level = cw.level, totalXp = cw.totalXp, unspentXp = cw.unspentXp });
            this.partyList.Items.Refresh();
        }

        private void editCharacter(object sender, RoutedEventArgs e) {
            CharacterRow selected = (CharacterRow)this.partyList.SelectedItem;
            CharacterWindow cw = new CharacterWindow();
            cw.setExisting(selected.name, selected.level, selected.unspentXp);
            cw.ShowDialog();
            if (!cw.isValid()) { return; }
            this.partyChanged = true;
            selected.name = cw.name;
            selected.level = cw.level;
            selected.totalXp = cw.totalXp;
            selected.unspentXp = cw.unspentXp;
            SortDescription sd = this.partyList.Items.SortDescriptions[0];
            this.partyList.Items.SortDescriptions.Clear();
            this.partyList.Items.SortDescriptions.Add(sd);
        }

        private void removeCharacter(object sender, RoutedEventArgs e) {
            CharacterRow selected = (CharacterRow)this.partyList.SelectedItem;
            if (selected == null) { return; }
            this.partyChanged = true;
            this.partyList.Items.Remove(selected);
            this.partyList.Items.Refresh();
        }

        private void calculateEncounterXp(object sender, RoutedEventArgs e) {
            this.xpList.Items.Clear();
            int partySize = 0;
            foreach (CharacterRow c in this.partyList.Items) {
                if (c.present) { partySize += 1; }
            }
            if (partySize <= 0) {
                MessageBox.Show("Party is empty; add characters to calculate encounter XP", "Error");
                return;
            }
            foreach (CharacterRow c in this.partyList.Items) {
                if (!c.present) { continue; }
                XpError err, worstErr = XpError.Success;
                int xp = 0;
                foreach (OpponentRow o in this.opponentList.Items) {
                    if ((o.cr < 1) || (c.level < 1) || (o.count < 1)) {
                        err = XpError.Failure;
                        continue;
                    }
                    xp += this.calculateXp(o.cr, c.level, partySize, out err) * o.count;
                    switch (worstErr) {
                    case XpError.Failure: break;
                    case XpError.ELTooHigh:
                        if (err == XpError.Failure) { worstErr = err; }
                        break;
                    case XpError.ELTooLow:
                        if ((err == XpError.Failure) || (err == XpError.ELTooHigh)) { worstErr = err; }
                        break;
                    default:
                        if (err != XpError.Success) { worstErr = err; }
                        break;
                    }
                }
                if (xp >= this.overLevelThreshold(c.level) - c.unspentXp) {
                    if (worstErr != XpError.Failure) { worstErr = XpError.OverLevel; }
                    xp = this.overLevelThreshold(c.level) - c.unspentXp - 1;
                }
                String notes="", errMsg = "";
                switch (worstErr) {
                case XpError.Failure:
                    notes = "X";
                    errMsg = "One or more opponents was invalid; unable to calculate XP";
                    break;
                case XpError.ELTooLow:
                    notes = "-";
                    errMsg = "At least one opponent was more than 7 levels below " + c.name;
                    errMsg += "; no XP was awarded for that opponent";
                    break;
                case XpError.ELTooHigh:
                    notes = "+";
                    errMsg = "At least one opponent was more than 7 levels above " + c.name;
                    errMsg += "; consider alternate XP award";
                    break;
                case XpError.OverLevel:
                    notes = "*";
                    errMsg = "XP award for " + c.name + "was more than two levels' worth; truncated to 1XP below second level-up";
                    break;
                }
                this.xpList.Items.Add(new XpRow { character = c.name, xp = xp, notes=notes, err=worstErr, errMsg=errMsg });
            }
            this.xpList.Items.Refresh();
        }

        private int calculateXp(int cr, int level, int count, out XpError err) {
            int origLevel = level;
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
            xp = Math.Round(xp / count, MidpointRounding.AwayFromZero);
            if (xp >= this.overLevelThreshold(origLevel)) {
                err = XpError.OverLevel;
                xp = this.overLevelThreshold(origLevel) - 1;
            }
            return (int)xp;
        }

        private int overLevelThreshold(int level) {
            return (level + level + 1) * 1000;
        }
    }
}
