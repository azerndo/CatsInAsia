using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MySQLConnectionDemo
{
    public partial class AddCatForm : Form
    {
        private TextBox textBoxName;
        private ComboBox comboBoxBreed;
        private TextBox textBoxAge;
        private ComboBox comboBoxGender;
        private ComboBox comboBoxRegion;

        // Constructor expects lists of breeds and regions from the database
        public AddCatForm(List<(int id, string name)> breeds, List<(int id, string name)> regions)
        {
            this.Text = "Add New Cat";

            // Name
            Label labelName = new Label() { Text = "Name:", Left = 10, Top = 20, Width = 80 };
            textBoxName = new TextBox() { Left = 100, Top = 20, Width = 150 };

            // Breed
            Label labelBreed = new Label() { Text = "Breed:", Left = 10, Top = 60, Width = 80 };
            comboBoxBreed = new ComboBox() { Left = 100, Top = 60, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            comboBoxBreed.DataSource = breeds;
            comboBoxBreed.DisplayMember = "name";
            comboBoxBreed.ValueMember = "id";

            // Age
            Label labelAge = new Label() { Text = "Age:", Left = 10, Top = 100, Width = 80 };
            textBoxAge = new TextBox() { Left = 100, Top = 100, Width = 150 };

            // Gender
            Label labelGender = new Label() { Text = "Gender:", Left = 10, Top = 140, Width = 80 };
            comboBoxGender = new ComboBox() { Left = 100, Top = 140, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            comboBoxGender.Items.AddRange(new object[] { "M", "F" });

            // Region
            Label labelRegion = new Label() { Text = "Region:", Left = 10, Top = 180, Width = 80 };
            comboBoxRegion = new ComboBox() { Left = 100, Top = 180, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            comboBoxRegion.DataSource = regions;
            comboBoxRegion.DisplayMember = "name";
            comboBoxRegion.ValueMember = "id";

            // Buttons
            Button buttonSave = new Button() { Text = "Save", Left = 40, Width = 80, Top = 230, DialogResult = DialogResult.OK };
            Button buttonCancel = new Button() { Text = "Cancel", Left = 140, Width = 80, Top = 230, DialogResult = DialogResult.Cancel };

            buttonSave.Click += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                    comboBoxBreed.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(textBoxAge.Text) ||
                    comboBoxGender.SelectedItem == null ||
                    comboBoxRegion.SelectedItem == null)
                {
                    MessageBox.Show("Please fill all fields.");
                    this.DialogResult = DialogResult.None;
                    return;
                }
                if (!int.TryParse(textBoxAge.Text, out int age))
                {
                    MessageBox.Show("Age must be a number.");
                    this.DialogResult = DialogResult.None;
                    return;
                }
                this.Close();
            };

            buttonCancel.Click += (sender, e) => { this.Close(); };

            this.Controls.Add(labelName);
            this.Controls.Add(textBoxName);
            this.Controls.Add(labelBreed);
            this.Controls.Add(comboBoxBreed);
            this.Controls.Add(labelAge);
            this.Controls.Add(textBoxAge);
            this.Controls.Add(labelGender);
            this.Controls.Add(comboBoxGender);
            this.Controls.Add(labelRegion);
            this.Controls.Add(comboBoxRegion);
            this.Controls.Add(buttonSave);
            this.Controls.Add(buttonCancel);

            this.AcceptButton = buttonSave;
            this.CancelButton = buttonCancel;
            this.ClientSize = new System.Drawing.Size(280, 280);
        }

        // Properties to retrieve entered/selected data
        public string CatName => textBoxName.Text;
        public int BreedId => comboBoxBreed.SelectedItem != null ? ((ValueTuple<int, string>)comboBoxBreed.SelectedItem).Item1 : -1;
public int RegionId => comboBoxRegion.SelectedItem != null ? ((ValueTuple<int, string>)comboBoxRegion.SelectedItem).Item1 : -1;
        public int CatAge => int.TryParse(textBoxAge.Text, out int age) ? age : 0;
        public string Gender => comboBoxGender.SelectedItem?.ToString();
    }
}