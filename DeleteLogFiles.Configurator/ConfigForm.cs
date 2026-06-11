using System.Diagnostics;
using System.Drawing;

namespace DeleteLogFiles.Configurator;

internal sealed class ConfigForm : Form
{
    private const string WebsiteUrl = "https://laun-it.de/log-dateien-automatisiert-loschen-mit-deletelogfiles-dienst/";

    private readonly TextBox configPathTextBox = new();
    private readonly NumericUpDown intervalInput = new();
    private readonly NumericUpDown retentionInput = new();
    private readonly CheckBox includeSubdirectoriesCheckBox = new();
    private readonly CheckBox dryRunCheckBox = new();
    private readonly CheckBox fileLoggingEnabledCheckBox = new();
    private readonly TextBox logPathTextBox = new();
    private readonly ComboBox minimumLogLevelComboBox = new();
    private readonly ListBox directoriesListBox = new();
    private readonly ListBox extensionsListBox = new();
    private readonly TextBox newExtensionTextBox = new();
    private readonly Label statusLabel = new();

    public ConfigForm(string? initialConfigPath)
    {
        Text = "DeleteLogFiles Konfiguration";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(820, 640);
        Size = new Size(900, 700);
        Font = new Font("Segoe UI", 10F);
        Icon = TryLoadIcon();

        BuildLayout();

        configPathTextBox.Text = ResolveInitialConfigPath(initialConfigPath);
        LoadConfiguration();
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            Padding = new Padding(18),
            BackColor = Color.White
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 118));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Controls.Add(root);

        root.Controls.Add(CreateHeader(), 0, 0);
        root.Controls.Add(CreateConfigPathPanel(), 0, 1);
        root.Controls.Add(CreateSettingsPanel(), 0, 2);
        root.Controls.Add(CreateLoggingPanel(), 0, 3);
        root.Controls.Add(CreateListsPanel(), 0, 4);
        root.Controls.Add(CreateFooter(), 0, 5);
    }

    private Control CreateHeader()
    {
        var panel = new TableLayoutPanel
        {
            ColumnCount = 3,
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 16)
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var logo = new PictureBox
        {
            Size = new Size(56, 56),
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = TryLoadLogo()
        };
        panel.Controls.Add(logo, 0, 0);

        var titlePanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            WrapContents = false,
            Dock = DockStyle.Fill
        };
        titlePanel.Controls.Add(new Label
        {
            Text = "DeleteLogFiles",
            Font = new Font(Font.FontFamily, 18F, FontStyle.Bold),
            AutoSize = true
        });
        titlePanel.Controls.Add(new Label
        {
            Text = "Konfiguration für automatische Logdatei-Bereinigung",
            AutoSize = true,
            ForeColor = Color.FromArgb(80, 80, 80)
        });
        panel.Controls.Add(titlePanel, 1, 0);

        var link = new LinkLabel
        {
            Text = "Laun IT Webseite",
            AutoSize = true,
            LinkColor = Color.FromArgb(0, 92, 170),
            Margin = new Padding(0, 18, 0, 0)
        };
        link.LinkClicked += (_, _) => OpenWebsite();
        panel.Controls.Add(link, 2, 0);

        return panel;
    }

    private Control CreateConfigPathPanel()
    {
        var group = new GroupBox
        {
            Text = "Konfigurationsdatei",
            Dock = DockStyle.Top,
            Height = 82,
            Padding = new Padding(12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        group.Controls.Add(layout);

        configPathTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(configPathTextBox, 0, 0);

        var browseButton = new Button
        {
            Text = "Auswählen",
            AutoSize = true,
            Margin = new Padding(8, 0, 0, 0)
        };
        browseButton.Click += (_, _) => BrowseConfigPath();
        layout.Controls.Add(browseButton, 1, 0);

        var reloadButton = new Button
        {
            Text = "Neu laden",
            AutoSize = true,
            Margin = new Padding(8, 0, 0, 0)
        };
        reloadButton.Click += (_, _) => LoadConfiguration();
        layout.Controls.Add(reloadButton, 2, 0);

        return group;
    }

    private Control CreateSettingsPanel()
    {
        var group = new GroupBox
        {
            Text = "Bereinigungsregeln",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 2
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        group.Controls.Add(layout);

        intervalInput.Minimum = 1;
        intervalInput.Maximum = 1440;
        intervalInput.Dock = DockStyle.Left;
        intervalInput.Width = 120;

        retentionInput.Minimum = 1;
        retentionInput.Maximum = 3650;
        retentionInput.Dock = DockStyle.Left;
        retentionInput.Width = 120;

        includeSubdirectoriesCheckBox.Text = "Unterordner einbeziehen";
        includeSubdirectoriesCheckBox.AutoSize = true;

        dryRunCheckBox.Text = "Dry-Run aktivieren";
        dryRunCheckBox.AutoSize = true;

        layout.Controls.Add(new Label { Text = "Intervall (Minuten)", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        layout.Controls.Add(intervalInput, 1, 0);
        layout.Controls.Add(new Label { Text = "Löschen nach (Tagen)", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);
        layout.Controls.Add(retentionInput, 3, 0);
        layout.Controls.Add(includeSubdirectoriesCheckBox, 1, 1);
        layout.Controls.Add(dryRunCheckBox, 3, 1);

        return group;
    }

    private Control CreateListsPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(0, 12, 0, 12)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        layout.Controls.Add(CreateDirectoriesPanel(), 0, 0);
        layout.Controls.Add(CreateExtensionsPanel(), 1, 0);
        return layout;
    }

    private Control CreateLoggingPanel()
    {
        var group = new GroupBox
        {
            Text = "Dienst-Log",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        group.Controls.Add(layout);

        fileLoggingEnabledCheckBox.Text = "aktiv";
        fileLoggingEnabledCheckBox.AutoSize = true;
        fileLoggingEnabledCheckBox.Anchor = AnchorStyles.Left;
        layout.Controls.Add(fileLoggingEnabledCheckBox, 0, 0);

        logPathTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(logPathTextBox, 1, 0);

        layout.Controls.Add(new Label { Text = "Level", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(12, 4, 4, 4) }, 2, 0);

        minimumLogLevelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        minimumLogLevelComboBox.Items.AddRange(["Trace", "Debug", "Information", "Warning", "Error", "Critical"]);
        minimumLogLevelComboBox.Dock = DockStyle.Fill;
        layout.Controls.Add(minimumLogLevelComboBox, 3, 0);

        var browseButton = new Button
        {
            Text = "Auswählen",
            AutoSize = true,
            Margin = new Padding(8, 0, 0, 0)
        };
        browseButton.Click += (_, _) => BrowseLogPath();
        layout.Controls.Add(browseButton, 4, 0);

        return group;
    }

    private Control CreateDirectoriesPanel()
    {
        var group = new GroupBox
        {
            Text = "Verzeichnisse",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        group.Controls.Add(layout);

        directoriesListBox.Dock = DockStyle.Fill;
        layout.Controls.Add(directoriesListBox, 0, 0);

        var buttons = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Dock = DockStyle.Fill
        };
        buttons.Controls.Add(CreateButton("Hinzufügen", AddDirectory));
        buttons.Controls.Add(CreateButton("Entfernen", () => RemoveSelected(directoriesListBox)));
        layout.Controls.Add(buttons, 0, 1);

        return group;
    }

    private Control CreateExtensionsPanel()
    {
        var group = new GroupBox
        {
            Text = "Dateiendungen",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        group.Controls.Add(layout);

        extensionsListBox.Dock = DockStyle.Fill;
        layout.Controls.Add(extensionsListBox, 0, 0);

        newExtensionTextBox.PlaceholderText = ".log";
        newExtensionTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(newExtensionTextBox, 0, 1);

        var buttons = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Dock = DockStyle.Fill
        };
        buttons.Controls.Add(CreateButton("Hinzufügen", AddExtension));
        buttons.Controls.Add(CreateButton("Entfernen", () => RemoveSelected(extensionsListBox)));
        layout.Controls.Add(buttons, 0, 2);

        return group;
    }

    private Control CreateFooter()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            ColumnCount = 2,
            AutoSize = true
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        statusLabel.AutoSize = true;
        statusLabel.ForeColor = Color.FromArgb(80, 80, 80);
        statusLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(statusLabel, 0, 0);

        var buttons = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true
        };
        buttons.Controls.Add(CreateButton("Speichern", SaveConfiguration));
        buttons.Controls.Add(CreateButton("Schließen", Close));
        layout.Controls.Add(buttons, 1, 0);

        return layout;
    }

    private static Button CreateButton(string text, Action action)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            Margin = new Padding(4)
        };
        button.Click += (_, _) => action();
        return button;
    }

    private void BrowseConfigPath()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "appsettings.json auswählen",
            Filter = "JSON-Dateien (*.json)|*.json|Alle Dateien (*.*)|*.*",
            FileName = Path.GetFileName(configPathTextBox.Text),
            InitialDirectory = Directory.Exists(Path.GetDirectoryName(configPathTextBox.Text))
                ? Path.GetDirectoryName(configPathTextBox.Text)
                : Environment.CurrentDirectory
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            configPathTextBox.Text = dialog.FileName;
            LoadConfiguration();
        }
    }

    private void BrowseLogPath()
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Logdatei auswählen",
            Filter = "Logdateien (*.log)|*.log|Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
            FileName = Path.GetFileName(logPathTextBox.Text),
            InitialDirectory = Directory.Exists(Path.GetDirectoryName(logPathTextBox.Text))
                ? Path.GetDirectoryName(logPathTextBox.Text)
                : Environment.CurrentDirectory
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            logPathTextBox.Text = dialog.FileName;
        }
    }

    private void AddDirectory()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Verzeichnis für die Bereinigung auswählen",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            AddUnique(directoriesListBox, dialog.SelectedPath);
        }
    }

    private void AddExtension()
    {
        var extension = NormalizeExtension(newExtensionTextBox.Text);
        if (string.IsNullOrWhiteSpace(extension))
        {
            MessageBox.Show(this, "Bitte eine Dateiendung eingeben.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        AddUnique(extensionsListBox, extension);
        newExtensionTextBox.Clear();
    }

    private static void AddUnique(ListBox listBox, string value)
    {
        if (!listBox.Items.Cast<string>().Any(item => string.Equals(item, value, StringComparison.OrdinalIgnoreCase)))
        {
            listBox.Items.Add(value);
        }
    }

    private static void RemoveSelected(ListBox listBox)
    {
        while (listBox.SelectedItems.Count > 0)
        {
            var selectedItem = listBox.SelectedItems[0];
            if (selectedItem is not null)
            {
                listBox.Items.Remove(selectedItem);
            }
        }
    }

    private void LoadConfiguration()
    {
        try
        {
            var configuration = ConfigurationFile.Load(configPathTextBox.Text);
            var cleanup = configuration.Cleanup;

            intervalInput.Value = Clamp(cleanup.IntervalMinutes, intervalInput.Minimum, intervalInput.Maximum);
            retentionInput.Value = Clamp(cleanup.DeleteAfterDays, retentionInput.Minimum, retentionInput.Maximum);
            includeSubdirectoriesCheckBox.Checked = cleanup.IncludeSubdirectories;
            dryRunCheckBox.Checked = cleanup.DryRun;
            fileLoggingEnabledCheckBox.Checked = configuration.FileLogging.Enabled;
            logPathTextBox.Text = configuration.FileLogging.Path;
            minimumLogLevelComboBox.SelectedItem = string.IsNullOrWhiteSpace(configuration.FileLogging.MinimumLevel)
                ? "Information"
                : configuration.FileLogging.MinimumLevel;
            if (minimumLogLevelComboBox.SelectedIndex < 0)
            {
                minimumLogLevelComboBox.SelectedItem = "Information";
            }

            directoriesListBox.Items.Clear();
            foreach (var directory in cleanup.Directories.Where(item => !string.IsNullOrWhiteSpace(item)))
            {
                directoriesListBox.Items.Add(directory);
            }

            extensionsListBox.Items.Clear();
            foreach (var extension in cleanup.Extensions.Select(NormalizeExtension).Where(item => !string.IsNullOrWhiteSpace(item)))
            {
                extensionsListBox.Items.Add(extension);
            }

            SetStatus("Konfiguration geladen.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Konfiguration konnte nicht geladen werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveConfiguration()
    {
        try
        {
            if (directoriesListBox.Items.Count == 0 || extensionsListBox.Items.Count == 0)
            {
                MessageBox.Show(this, "Bitte mindestens ein Verzeichnis und eine Dateiendung konfigurieren.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var configuration = new AppConfiguration
            {
                Cleanup = new CleanupSettings
                {
                    IntervalMinutes = (int)intervalInput.Value,
                    DeleteAfterDays = (int)retentionInput.Value,
                    IncludeSubdirectories = includeSubdirectoriesCheckBox.Checked,
                    DryRun = dryRunCheckBox.Checked,
                    Directories = directoriesListBox.Items.Cast<string>().ToList(),
                    Extensions = extensionsListBox.Items.Cast<string>().Select(NormalizeExtension).Where(item => !string.IsNullOrWhiteSpace(item)).ToList()
                },
                FileLogging = new FileLoggingSettings
                {
                    Enabled = fileLoggingEnabledCheckBox.Checked,
                    Path = string.IsNullOrWhiteSpace(logPathTextBox.Text) ? "Logs\\DeleteLogFiles.log" : logPathTextBox.Text.Trim(),
                    MinimumLevel = minimumLogLevelComboBox.SelectedItem?.ToString() ?? "Information"
                }
            };

            ConfigurationFile.Save(configPathTextBox.Text, configuration);
            SetStatus("Konfiguration gespeichert.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Konfiguration konnte nicht gespeichert werden", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static decimal Clamp(int value, decimal minimum, decimal maximum)
    {
        return Math.Min(Math.Max(value, (int)minimum), (int)maximum);
    }

    private static string NormalizeExtension(string value)
    {
        var extension = value.Trim();
        if (string.IsNullOrWhiteSpace(extension))
        {
            return string.Empty;
        }

        return extension.StartsWith('.') ? extension : "." + extension;
    }

    private void SetStatus(string message)
    {
        statusLabel.Text = message;
    }

    private static string ResolveInitialConfigPath(string? initialConfigPath)
    {
        if (!string.IsNullOrWhiteSpace(initialConfigPath))
        {
            return Path.GetFullPath(initialConfigPath);
        }

        var baseDirectory = AppContext.BaseDirectory;
        var current = new DirectoryInfo(baseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "DeleteLogFiles", "appsettings.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        return Path.Combine(baseDirectory, "appsettings.json");
    }

    private static Icon? TryLoadIcon()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "favicon.ico");
        return File.Exists(path) ? new Icon(path) : null;
    }

    private static Image? TryLoadLogo()
    {
        var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "logo_180x180.png");
        if (File.Exists(logoPath))
        {
            return Image.FromFile(logoPath);
        }

        using var icon = TryLoadIcon();
        return icon?.ToBitmap();
    }

    private static void OpenWebsite()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = WebsiteUrl,
            UseShellExecute = true
        });
    }
}
