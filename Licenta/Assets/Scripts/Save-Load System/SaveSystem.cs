using UnityEngine;
using System.IO;

public static class SaveSystem {

    private static readonly string saveFolder = Application.dataPath + "/Saves/";

    public static void InitSaveSystem() {
        if(!Directory.Exists(saveFolder)) {
            Directory.CreateDirectory(saveFolder);
            // Debug.Log("SaveSystem: Save directory created");
        }
    }
    
    public static void SaveGame() {
        SavedData data = new SavedData(GameManager.instance.getCurrentLevel());

        string jsonFileContents = JsonUtility.ToJson(data, true);

        File.WriteAllText(saveFolder + "save.json", jsonFileContents);
        // Debug.Log("SaveSystem: Game saved.");
    }

    public static SavedData LoadGame() {
        if (File.Exists(saveFolder + "save.json")) {
            string jsonFileContents = File.ReadAllText(saveFolder + "save.json");
            SavedData data = JsonUtility.FromJson<SavedData>(jsonFileContents);

            // Debug.Log("SaveSystem: Game loaded.");
            return data;
        } else {
            return null;
        }
    }
}
