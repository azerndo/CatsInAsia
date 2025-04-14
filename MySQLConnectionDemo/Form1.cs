using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing.Drawing2D;

namespace MySQLConnectionDemo;

public class RoundedTextBox : TextBox
{
    private int borderRadius = 10;
    private Color borderColor = Color.FromArgb(200, 200, 200);
    private int borderWidth = 1;

    public RoundedTextBox()
    {
        this.BorderStyle = BorderStyle.None;
        this.Padding = new Padding(10, 5, 10, 5);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using (GraphicsPath path = new GraphicsPath())
        {
            path.AddArc(0, 0, borderRadius * 2, borderRadius * 2, 180, 90);
            path.AddArc(this.Width - borderRadius * 2, 0, borderRadius * 2, borderRadius * 2, 270, 90);
            path.AddArc(this.Width - borderRadius * 2, this.Height - borderRadius * 2, borderRadius * 2, borderRadius * 2, 0, 90);
            path.AddArc(0, this.Height - borderRadius * 2, borderRadius * 2, borderRadius * 2, 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        this.Invalidate();
    }
}

public partial class Form1 : Form
{
    private string connectionString = "server=127.0.0.1;port=3306;user=root;password=blackprincess21;database=cats_in_asia;";
    private MySqlConnection connection;
    private Panel sidePanel;
    private Panel contentPanel;
    private DataGridView dataGridView;
    private TextBox searchBox;
    private Label statusLabel;
    private Color primaryColor = Color.FromArgb(255, 140, 0); // Dark Orange
    private Color secondaryColor = Color.FromArgb(255, 165, 0); // Orange
    private Color accentColor = Color.FromArgb(255, 69, 0); // Red-Orange
    private Button toggleSidebarButton;
    private bool isSidebarVisible = true;
    private List<Button> menuButtons = new List<Button>();

    public Form1()
    {
        InitializeComponent();
        InitializeCustomComponents();
        SetupDatabase();
    }

    private void InitializeCustomComponents()
    {
        // Form settings
        this.Text = "Cats in Asia Management System";
        this.Size = new Size(1200, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;

        // Create side panel
        sidePanel = new Panel
        {
            BackColor = primaryColor,
            Dock = DockStyle.Left,
            Width = 250
        };

        // Add logo/title panel
        Panel logoPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100,
            BackColor = accentColor,
            Padding = new Padding(10)
        };

        // Create toggle button in the logo panel
        toggleSidebarButton = new Button
        {
            Text = "◀",
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            BackColor = accentColor,
            Size = new Size(40, 40),
            Dock = DockStyle.Left,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 12, FontStyle.Bold)
        };
        toggleSidebarButton.FlatAppearance.BorderSize = 0;
        toggleSidebarButton.Click += ToggleSidebarButton_Click;

        Label titleLabel = new Label
        {
            Text = "CATS",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Padding = new Padding(40, 0, 0, 0) // Add left padding to avoid overlap with toggle button
        };

        logoPanel.Controls.Add(toggleSidebarButton);
        logoPanel.Controls.Add(titleLabel);
        sidePanel.Controls.Add(logoPanel);

        // Create buttons for side panel with emojis as icons
        CreateMenuButton("🐱 Cats", "View and manage all cats", 0);
        CreateMenuButton("👥 Owners", "View pet owners", 1);
        CreateMenuButton("📋 Medical Records", "Access medical records", 2);
        CreateMenuButton("❤️ Adoption Requests", "Manage adoption requests", 3);

        // Create content panel
        contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(240, 240, 240),
            Padding = new Padding(20)
        };

        // Create search panel
        Panel searchPanel = new Panel
        {
            Height = 60,
            Dock = DockStyle.Top,
            BackColor = Color.White,
            Padding = new Padding(20),
            Margin = new Padding(0, 0, 20, 0) // Add right margin
        };

        // Create search box with icon
        searchBox = new RoundedTextBox
        {
            PlaceholderText = "🔍 Search...",
            Width = 300,
            Height = 50,
            Font = new Font("Segoe UI", 12),
            Location = new Point(searchPanel.Width - 400, 12), // Adjusted position for better alignment
            BackColor = Color.FromArgb(240, 240, 240),
            Anchor = AnchorStyles.Top | AnchorStyles.Right // Anchor to right
        };

        // Create search button
        Button searchButton = new Button
        {
            Text = "Search",
            FlatStyle = FlatStyle.Flat,
            BackColor = primaryColor,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Height = 35,
            Width = 80,
            Cursor = Cursors.Hand,
            Location = new Point(searchBox.Location.X + searchBox.Width + 1, 12),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        searchButton.FlatAppearance.BorderSize = 0;
        searchButton.Click += SearchButton_Click;

        searchPanel.Controls.Add(searchBox);
        searchPanel.Controls.Add(searchButton);
        contentPanel.Controls.Add(searchPanel);

        // Create DataGridView
        dataGridView = new DataGridView
        {
            Location = new Point(20, 80),
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            GridColor = Color.FromArgb(240, 240, 240),
            DefaultCellStyle = new DataGridViewCellStyle
            {
                SelectionBackColor = Color.FromArgb(240, 240, 240),
                SelectionForeColor = Color.Black,
                Font = new Font("Segoe UI", 10),
                Padding = new Padding(5),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64)
            },
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(5)
            },
            RowTemplate = new DataGridViewRow
            {
                Height = 40
            },
            EnableHeadersVisualStyles = false
        };

        // Add alternating row colors
        dataGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(250, 250, 250)
        };
        contentPanel.Controls.Add(dataGridView);

        // Create status label
        statusLabel = new Label
        {
            Text = "Ready",
            Dock = DockStyle.Bottom,
            Height = 30,
            BackColor = primaryColor,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0)
        };

        // Add controls to form
        this.Controls.Add(sidePanel);
        this.Controls.Add(contentPanel);
        this.Controls.Add(statusLabel);

        // Add event handlers
        searchBox.TextChanged += SearchBox_TextChanged;
        this.Resize += (s, e) => UpdateLayout();

        // Initial layout update
        UpdateLayout();
    }

    private void CreateMenuButton(string text, string tooltip, int index)
    {
        Panel buttonPanel = new Panel
        {
            Height = 60,
            Dock = DockStyle.Top,
            Padding = new Padding(10),
            BackColor = primaryColor
        };

        Button button = new Button
        {
            Text = text,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            Cursor = Cursors.Hand,
            TabStop = false,
            Tag = text // Store original text
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = secondaryColor;
        button.FlatAppearance.MouseDownBackColor = accentColor;
        button.Click += MenuButton_Click;

        // Add tooltip
        ToolTip tip = new ToolTip();
        tip.SetToolTip(button, tooltip);

        buttonPanel.Controls.Add(button);
        sidePanel.Controls.Add(buttonPanel);
        menuButtons.Add(button);
    }

    private void MenuButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            string buttonText = button.Text.Substring(3); // Remove emoji
            switch (buttonText.Trim())
            {
                case "Cats":
                    LoadCatsData();
                    break;
                case "Owners":
                    LoadOwnersData();
                    break;
                case "Medical Records":
                    LoadMedicalRecordsData();
                    break;
                case "Adoption Requests":
                    LoadAdoptionRequestsData();
                    break;
            }
        }
    }

    private void SetupDatabase()
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            statusLabel.Text = "Connected to database";
            LoadCatsData(); // Load initial data
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Database connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusLabel.Text = "Database connection failed";
        }
    }

    private void LoadCatsData()
    {
        try
        {
            string query = @"
                SELECT 
                    c.cat_id as 'ID',
                    c.name as 'Name',
                    cb.breed_name as 'Breed',
                    c.age as 'Age',
                    CASE 
                        WHEN c.gender = 'M' THEN 'Male'
                        WHEN c.gender = 'F' THEN 'Female'
                        ELSE c.gender
                    END as 'Gender',
                    r.region_name as 'Region'
                FROM cats c
                LEFT JOIN cat_breeds cb ON c.breed_id = cb.breed_id
                LEFT JOIN regions r ON c.region_id = r.region_id";
                
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView.DataSource = dataTable;
            }
            statusLabel.Text = "Data loaded successfully";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusLabel.Text = "Error loading data";
        }
    }

    private void LoadOwnersData()
    {
        try
        {
            string query = "SELECT * FROM cat_ownership";
            LoadTableData(query, "Owners");
        }
        catch (Exception ex)
        {
            HandleError("loading owners data", ex);
        }
    }

    private void LoadMedicalRecordsData()
    {
        try
        {
            string query = "SELECT * FROM medical_records";
            LoadTableData(query, "Medical Records");
        }
        catch (Exception ex)
        {
            HandleError("loading medical records", ex);
        }
    }

    private void LoadAdoptionRequestsData()
    {
        try
        {
            string query = "SELECT * FROM adoption_request";
            LoadTableData(query, "Adoption Requests");
        }
        catch (Exception ex)
        {
            HandleError("loading adoption requests", ex);
        }
    }

    private void LoadTableData(string query, string tableName)
    {
        try
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView.DataSource = dataTable;
            }
            statusLabel.Text = $"{tableName} data loaded successfully";
        }
        catch (Exception ex)
        {
            HandleError($"loading {tableName.ToLower()}", ex);
        }
    }

    private void HandleError(string operation, Exception ex)
    {
        MessageBox.Show($"Error {operation}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        statusLabel.Text = $"Error {operation}";
    }

    private void SearchBox_TextChanged(object? sender, EventArgs e)
    {
        // Only update status label when typing
        if (!string.IsNullOrWhiteSpace(searchBox.Text))
        {
            statusLabel.Text = "Press Search to find matching records";
        }
        else
        {
            statusLabel.Text = "Ready";
        }
    }

    private void SearchButton_Click(object? sender, EventArgs e)
    {
        PerformSearch();
    }

    private void PerformSearch()
    {
        try
        {
            if (dataGridView.DataSource is DataTable dataTable)
            {
                string searchText = searchBox.Text.ToLower();
                DataView dv = dataTable.DefaultView;

                // Determine which table we're working with and set appropriate filter
                if (dataTable.Columns.Contains("Name")) // Cats table
                {
                    dv.RowFilter = $"Convert([Name], 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert([ID], 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert([Breed], 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert([Age], 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert([Gender], 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert([Region], 'System.String') LIKE '%{searchText}%'";
                }
                else if (dataTable.Columns.Contains("owner_name")) // Cat ownership table
                {
                    dv.RowFilter = $"Convert(owner_name, 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert(cat_id, 'System.String') LIKE '%{searchText}%'";
                }
                else if (dataTable.Columns.Contains("diagnosis")) // Medical records table
                {
                    dv.RowFilter = $"Convert(diagnosis, 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert(cat_id, 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert(treatment, 'System.String') LIKE '%{searchText}%'";
                }
                else if (dataTable.Columns.Contains("requester_name")) // Adoption request table
                {
                    dv.RowFilter = $"Convert(requester_name, 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert(cat_id, 'System.String') LIKE '%{searchText}%' OR " +
                                 $"Convert(status, 'System.String') LIKE '%{searchText}%'";
                }

                dataGridView.DataSource = dv;
                statusLabel.Text = $"Found {dv.Count} matching records";
            }
        }
        catch (Exception ex)
        {
            statusLabel.Text = $"Search error: {ex.Message}";
        }
    }

    private void UpdateLayout()
    {
        if (dataGridView != null)
        {
            int leftMargin = isSidebarVisible ? sidePanel.Width + 20 : 80;
            dataGridView.Location = new Point(leftMargin, 80);
            dataGridView.Width = contentPanel.Width - leftMargin - 20;
            dataGridView.Height = contentPanel.Height - 100;
        }
    }

    private void ToggleSidebarButton_Click(object? sender, EventArgs e)
    {
        isSidebarVisible = !isSidebarVisible;
        
        if (isSidebarVisible)
        {
            sidePanel.Width = 250;
            toggleSidebarButton.Text = "◀";
            // Show full text for all buttons
            foreach (var button in menuButtons)
            {
                button.Text = (string)button.Tag;
            }
        }
        else
        {
            sidePanel.Width = 60; // Just enough for icons
            toggleSidebarButton.Text = "▶";
            // Show only icons for all buttons
            foreach (var button in menuButtons)
            {
                string fullText = (string)button.Tag;
                button.Text = fullText.Substring(0, 2); // Keep only the emoji
            }
        }

        UpdateLayout();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        connection?.Close();
    }
}
