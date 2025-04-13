using System;
using System.IO;
using System.Windows;

namespace Calculator.Services
{
    public class SettingsService
    {
        private readonly string _settingsFilePath;

        public SettingsService()
        {
            string appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Calculator");

            if (!Directory.Exists(appDataFolder))
                Directory.CreateDirectory(appDataFolder);

            _settingsFilePath = Path.Combine(appDataFolder, "settings.txt");
        }

        public void SaveSettings(bool isStandardMode, bool digitGrouping, int currentBase)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(_settingsFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (StreamWriter writer = new StreamWriter(_settingsFilePath, false))
                {
                    writer.WriteLine(isStandardMode.ToString());
                    writer.WriteLine(digitGrouping.ToString());
                    writer.WriteLine(currentBase.ToString());
                }

                if (File.Exists(_settingsFilePath))
                {
                    string[] lines = File.ReadAllLines(_settingsFilePath);
                    MessageBox.Show($"Setări salvate:\nStandard: {lines[0]}\nDigit Grouping: {lines[1]}\nBase: {lines[2]}",
                                    "Salvare setări");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea setărilor: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public (bool isStandardMode, bool digitGrouping, int currentBase) LoadSettings()
        {
            bool isStandardMode = true;
            bool digitGrouping = false;
            int currentBase = 10;

            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string[] lines = File.ReadAllLines(_settingsFilePath);
                    if (lines.Length >= 3)
                    {
                        bool.TryParse(lines[0], out isStandardMode);
                        bool.TryParse(lines[1], out digitGrouping);
                        int.TryParse(lines[2], out currentBase);
                    }

                    // Debug
                    MessageBox.Show($"Setări încărcate:\nStandard: {isStandardMode}\nDigit Grouping: {digitGrouping}\nBase: {currentBase}",
                                    "Încărcare setări");
                }
                else
                {
                    MessageBox.Show("Fișierul de setări nu există. Se folosesc valorile implicite.", "Info");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea setărilor: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return (isStandardMode, digitGrouping, currentBase);
        }
    }
}