using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySQLConnectionDemo
{
    public class AdoptionRequestForm : Form
    {
        private ComboBox comboBoxCat;
        private TextBox textBoxOwnerId;
        private DateTimePicker dateTimePickerRequestDate;
        private Button buttonSubmit;

        public AdoptionRequestForm()
        {
            this.Text = "Apply to Adopt a Cat";
            this.Width = 350;
            this.Height = 250;

            // Cat ComboBox
            Label labelCat = new Label() { Text = "Select Cat:", Left = 10, Top = 20, Width = 100 };
            comboBoxCat = new ComboBox() { Left = 120, Top = 20, Width = 180 };

            // Owner ID TextBox
            Label labelOwnerId = new Label() { Text = "Owner ID:", Left = 10, Top = 60, Width = 100 };
            textBoxOwnerId = new TextBox() { Left = 120, Top = 60, Width = 180 };

            // Request Date (today, readonly)
            Label labelDate = new Label() { Text = "Request Date:", Left = 10, Top = 100, Width = 100 };
            dateTimePickerRequestDate = new DateTimePicker() { Left = 120, Top = 100, Width = 180 };
            dateTimePickerRequestDate.Value = DateTime.Today;
            dateTimePickerRequestDate.Enabled = false;

            // Submit Button
            buttonSubmit = new Button() { Text = "Submit", Left = 120, Top = 140, Width = 100 };
            buttonSubmit.Click += buttonSubmit_Click;

            // Add controls to form
            this.Controls.Add(labelCat);
            this.Controls.Add(comboBoxCat);
            this.Controls.Add(labelOwnerId);
            this.Controls.Add(textBoxOwnerId);
            this.Controls.Add(labelDate);
            this.Controls.Add(dateTimePickerRequestDate);
            this.Controls.Add(buttonSubmit);

            LoadCats();
        }

        private void LoadCats()
        {
            using (MySqlConnection conn = new MySqlConnection(Form1.connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT cat_id, name FROM cats";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
        
                    comboBoxCat.DisplayMember = "name";
                    comboBoxCat.ValueMember = "cat_id";
                    comboBoxCat.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading cats: " + ex.Message);
                }
            }
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            if (comboBoxCat.SelectedValue == null || string.IsNullOrWhiteSpace(textBoxOwnerId.Text))
            {
                MessageBox.Show("Please select a cat and enter your Owner ID.");
                return;
            }

            int catId = Convert.ToInt32(comboBoxCat.SelectedValue);
            string ownerId = textBoxOwnerId.Text.Trim();
            DateTime requestDate = DateTime.Today;

            string connStr = Form1.connectionString;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "INSERT INTO adoption_request (cat_id, owner_id, request_date, status) VALUES (@cat_id, @owner_id, @request_date, 'Pending')";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@cat_id", catId);
                    cmd.Parameters.AddWithValue("@owner_id", ownerId);
                    cmd.Parameters.AddWithValue("@request_date", requestDate);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Adoption request submitted!");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to submit request.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}