using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

public static class SaveLoadUtil
{
    private const string savePath = "user://godotMarble.save";
    private const string configPath = "user://godotMarble.config";

    // Save data struct for working with the data (instead of json)
    public struct SaveData
    {
        private List<float> levelTimes;
        public int Length => levelTimes.Count;

        public SaveData()
        {
            levelTimes = new List<float>();
        }

        public SaveData(float[] levelTimes)
        {
            this.levelTimes = new List<float>(levelTimes);
        }

        /// <summary></summary>
        /// <returns>Completion time in milliseconds.</returns>
        public float GetLevelTime(int index)
        {
            if (index > levelTimes.Count - 1 || index < 0)
                return -1f;
            else
                return levelTimes[index];
        }

        public void SetLevelTime(int index, float time)
        {
            if (index >= this.Length)
                ExtendToLength(index + 1);

            // Store completion in list at the correct index
            GD.Print($"Length & Index: {Length} {index}");
            levelTimes[index] = time;
        }

        private void ExtendToLength(int length)
        {
            int amountToAdd = length - this.Length;
            for (int i = 0; i < amountToAdd; i++)
            {
                levelTimes.Add(-1f);
            }
        }
    }

    public static void SaveGame()
    {
        // DATA TO SAVE:
        //  - Level completion times ( <0 if not completed)

        // Create save file, if it doesn't exist
        if (!FileAccess.FileExists(savePath))
            FileAccess.Open(savePath, FileAccess.ModeFlags.Write);

        // Open existing save data
        SaveData save = LoadGame();

        // Retrieve level time from level manager
        // ...and compare to save file
        int levelIndex = LevelManager.loadedLevelIndex;
        float newTime = LevelManager.CompletionTime;
        float oldTime = save.GetLevelTime(levelIndex);
        float time = (oldTime == -1f || newTime < oldTime) ? newTime : oldTime;

        save.SetLevelTime(levelIndex, time);

        using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);

        GD.Print($"Levels to save: {save.Length}");

        // Store data in a dictionary
        for (int i = 0; i < save.Length; i++)
        {
            Godot.Collections.Dictionary<string, Variant> dataObject = new Godot.Collections.Dictionary<string, Variant>()
            {
                { "Level", i },
                { "Time", save.GetLevelTime(i)}
            };

            // Compile and save data via json
            string jsonString = Json.Stringify(dataObject);
            file.StoreLine(jsonString);

            GD.Print($"Storing line {i}.");
        }
        file.Close();
    }

    // TODO: Change return type
    public static SaveData LoadGame()
    {
        // Return empty save data
        if (!FileAccess.FileExists(savePath))
            return new SaveData();

        using var data = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);

        List<float> completionTimes = new List<float>();
        while (data.GetPosition() < data.GetLength())
        {
            // Get line at the "cursor" in the json
            var jsonString = data.GetLine();

            // Parse string in the json into a json object
            var json = new Json();
            var output = json.Parse(jsonString);
            if (output != Error.Ok)
            {
                GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
                continue;
            }

            // Data for one level
            var levelData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);

            completionTimes.Add((float)levelData["Time"]);
        }
        data.Close();

        // Return save data struct
        return new SaveData(completionTimes.ToArray());
    }

    public static void SaveConfig()
    {
        // Create config file, if it doesn't exist
        if (!FileAccess.FileExists(configPath)) { }
        FileAccess.Open(configPath, FileAccess.ModeFlags.Write);

        using var file = FileAccess.Open(configPath, FileAccess.ModeFlags.Write);

        Godot.Collections.Dictionary<string, Variant> configs = new Godot.Collections.Dictionary<string, Variant>();

        // Save the value for each volume slider in the dictionary
        for (int i = 0; i < VolumeSlider.All.Count; i++)
        {
            string sliderName = VolumeSlider.All[i].Name;
            float sliderValue = VolumeSlider.All[i].NormalizedValue;
            configs.Add(sliderName, sliderValue);
            
            GD.Print($"Saving {sliderName} value: {sliderValue}");
        }

        // Save the dictionary object in the file using Json
        string jsonString = Json.Stringify(configs);
        file.StoreLine(jsonString);

        // Close the file
        file.Close();
    }

    public static Godot.Collections.Dictionary<string, Variant> LoadConfig()
    {
        if (!FileAccess.FileExists(configPath))
            return new Godot.Collections.Dictionary<string, Variant>();

        using var file = FileAccess.Open(configPath, FileAccess.ModeFlags.Read);

        string jsonString = file.GetLine();

        // Parse string in the json into a json object
        var json = new Json();
        var output = json.Parse(jsonString);
        if (output != Error.Ok)
        {
            GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
        }
        
        // Convert json data to dictionary
        var configData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
        
        return configData;
    }
}