using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GameSaveLoader
{
    public static void Save<T>(T saveObject, string fileName) where T : class
    {
        if (saveObject == null || string.IsNullOrEmpty(fileName))
            return;

        string filePath = GetSaveDirectory() + "/" + fileName + ".save";

        BinaryFormatter bf = new BinaryFormatter();
        using FileStream stream = File.Create(filePath);
        bf.Serialize(stream, saveObject);
        Debug.Log($"{fileName} file is saved");
    }

    public static bool TryLoad<T>(string fileName, out T loadedFile) where T : class
    {
        loadedFile = null;
        string filePath = GetSaveDirectory() + "/" + fileName + ".save";

        if (string.IsNullOrEmpty(filePath))
            return false;

        if (!File.Exists(filePath))
            return false;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream stream = File.Open(filePath, FileMode.Open))
            {
                loadedFile = (T)bf.Deserialize(stream);
                stream.Close();
            }

            Debug.Log($"{filePath} is loaded.");
            return true;
        }
        catch (SerializationException ex)
        {
            Debug.Log($"Failed to load {fileName} file {ex}");
            return false;
        }
    }

    public static void DeleteSaves(string fileName)
    {
        string filePath = GetSaveDirectory() + "/" + fileName + ".save";

        if (!File.Exists(filePath))
            return;
        Debug.Log($"{fileName} file is deleted. Path: {filePath}");
        File.Delete(filePath);
    }

    public static string GetSaveDirectory()
    {
        var saveDirectory = "Saves";
        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + saveDirectory);

        return Application.persistentDataPath + "/" + saveDirectory;
    }
}