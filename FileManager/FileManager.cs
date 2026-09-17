using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Xml.Linq;


namespace xInvaders;


internal class FileManager
{

    // Here we are processing these files:
    // 1. x-invaders_config.json -  file containing configuration data (like music volume, ATH score, etc.)
    // 2. game-strings.json      -  file containing default game strings (like "Press ENTER to start the game", "Game Over", etc.)
    // 3. game-credits.txt       -  file containing game credits (like links to game resources, music, sounds, etc.


    /// <summary>
    /// This method is used to get the configuration data from the x-invaders_config.json file.
    /// If file does not exist, it will create a new file with default values and return the default values.
    /// </summary>
    /// <returns></returns>
    public FileConfigData GetConfigData()
    {

        // We are storing config.json file in the Local AppData folder.
        // This is a good place for storing user-specific data.
        
        FileConfigData cData = null;

        string appDataPath = Path.Combine(
                             Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                             "vt-soft\\x-Invaders");

        // Check if the directory (for the config file) exists, if not, create it.
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        string configFileName = Path.Combine(appDataPath, "x-invaders_config.json");


        if (File.Exists(configFileName))  // File exist: read it
        {

            using (var reader = new StreamReader(configFileName))
            {
                // Read the json file and convert it to a string.
                string jsonContent = reader.ReadToEnd();

                // Deserialize the json string to a DataConfig object.
                try
                {
                    cData = JsonSerializer.Deserialize<FileConfigData>(jsonContent);
                    if (cData == null)
                    {
                        // Deserialized to null -> treat as corrupted
                        MessageBox.Show($"Configuration file is corrupted: {configFileName}\n\n" +
                                         "Fix or delete (Application will then create a new file).", 
                                         "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }
                catch (JsonException ex)
                {
                        // Corrupted JSON: show message box and exit so user can read the error
                        MessageBox.Show($"Configuration file is corrupted: {configFileName}\n\n" +
                                         "Fix or delete (Application will then create a new file) the file.\n\n" + $"Error: {ex.Message}",
                                         "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                }
            }
        }
        else  // File does not exist: create it with default values
        {
            // Create a new DataConfig object with default values.
            cData = new FileConfigData();

            // Serialize the DataConfig object to a json string.
            string jsonContent = JsonSerializer.Serialize(cData, new JsonSerializerOptions { WriteIndented = true });

            // Write the json string to the config file.
            using (var writer = new StreamWriter(configFileName))
            {
                writer.Write(jsonContent);
            }
        }

        return cData;

    }



    /// <summary>
    /// This method is used to get the game strings from the game-strings.json file.
    /// If file does not exist, it will create a new file with default values and return the default values.
    /// </summary>
    /// <returns></returns>
    public FileGameStrings GetGameStrings()
    {

        string fileName = "game-strings.json";
        string folderName = AppContext.BaseDirectory;
        fileName = Path.Combine(folderName, fileName);

        FileGameStrings gsData = null;
        
        if (File.Exists(fileName))  // File exist: read it
        {
            using (var reader = new StreamReader(fileName))
            {
                // Read the json file and convert it to a string.
                string jsonContent = reader.ReadToEnd();

                // Deserialize the json string to a DataConfig object.
                try
                {
                    gsData = JsonSerializer.Deserialize<FileGameStrings>(jsonContent);
                    if (gsData == null)
                    {
                        // Deserialized to null -> treat as corrupted
                        MessageBox.Show($"File with game strings is corrupted: {fileName}\n\n" +
                                        "Fix or delete (Application will then create a new file).", 
                                        "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }
                catch (JsonException ex)
                {
                        // Corrupted JSON: show message box and exit so user can read the error
                        MessageBox.Show($"File with game strings is corrupted: {fileName}\n\n" +
                                         "Fix or delete (Application will then create a new file) the file.\n\n" +
                                        $"Error: {ex.Message}", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                }
            }
        }
        else  // File does not exist: create it with default values
        {
            // Create a new DataConfig object with default values.
            gsData = new FileGameStrings();

            // Serialize the DataConfig object to a json string.
            string jsonContent = JsonSerializer.Serialize(gsData, new JsonSerializerOptions { WriteIndented = true });

            // Write the json string to the config file.
            using (var writer = new StreamWriter(fileName))
            {
                writer.Write(jsonContent);
            }
        }

        return gsData;
    }



    /// <summary>
    /// This method is used to get the game credits from the game-credits.json file.
    /// </summary>
    /// <returns>A list of strings containing the game credits.</returns>
    public List<string> GetGameCredits()
    {
        string fileName = "game-credits.txt";
        string folderName = AppContext.BaseDirectory;
        string filePath = Path.Combine(folderName, fileName);

        if (!File.Exists(filePath))
        {
            MessageBox.Show($"Missing file: {filePath}", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }

        List<string> creditsList = new List<string>();
     
        foreach (var line in File.ReadAllLines(filePath))
        {
            creditsList.Add(line);
        }

        // User is not allowed to delete links from the game-credits.txt file.
        // So at least we are checking this one link :)
        bool found = creditsList.Exists(l => string.Equals(l?.Trim(), "https://www.vt-soft.com/x-invaders", StringComparison.OrdinalIgnoreCase));

        if (!found)
        {
            MessageBox.Show("File game-credits.txt is corrupted. You are not allowed to modify this file. Please reinstall the application.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }

        return creditsList;
    }

    /// <summary>
    /// This method is used to save the configuration data to the x-invaders_config.json file.
    /// </summary>
    /// <param name="data">The configuration data to save.</param>
    public void SaveConfigData(FileConfigData data)
    {
        string appDataPath = Path.Combine(
                             Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                             "vt-soft\\x-invaders");

        // Check if the directory (for the config file) exists, if not, create it.
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        string configFileName = Path.Combine(appDataPath, "x-invaders_config.json");

        // Serialize the DataConfig object to a json string.
        string jsonContent = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

        // Write the json string to the config file.
        using (var writer = new StreamWriter(configFileName))
        {
            writer.Write(jsonContent);
        }
    }


}
