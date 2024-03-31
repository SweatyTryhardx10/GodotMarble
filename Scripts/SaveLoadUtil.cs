using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

public static class SaveLoadUtil
{
    private const string filePath = "user://godotMarble.save";

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
        if (!FileAccess.FileExists(filePath))
            FileAccess.Open(filePath, FileAccess.ModeFlags.Write);

        // Open existing save data
        SaveData save = LoadGame();

        // Retrieve level time from level manager
        // ...and compare to save file
        int levelIndex = LevelManager.loadedLevelIndex;
        float newTime = LevelManager.CompletionTime;
        float oldTime = save.GetLevelTime(levelIndex);
        float time = (oldTime == -1f || newTime < oldTime) ? newTime : oldTime;

        save.SetLevelTime(levelIndex, time);

        using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);

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
        if (!FileAccess.FileExists(filePath))
            return new SaveData();

        using var data = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);

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
}