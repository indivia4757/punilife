using System;
using System.IO;
using UnityEngine;

public sealed class SaveManager
{
    private const string FileName = "puni_save.json";
    private const string BackupFileName = "puni_save.backup.json";
    private const string TempFileName = "puni_save.tmp";

    public string SavePath => Path.Combine(Application.persistentDataPath, FileName);
    public string BackupPath => Path.Combine(Application.persistentDataPath, BackupFileName);
    private string TempPath => Path.Combine(Application.persistentDataPath, TempFileName);

    public SaveData Load()
    {
        if (TryLoadFromPath(SavePath, out SaveData data) || TryLoadFromPath(BackupPath, out data))
        {
            return data;
        }

        return SaveData.CreateNew();
    }

    public void Save(SaveData data)
    {
        data.EnsureDefaults();
        data.lastSavedAt = DateTime.UtcNow.ToString("O");
        string json = JsonUtility.ToJson(data, true);

        Directory.CreateDirectory(Application.persistentDataPath);
        File.WriteAllText(TempPath, json);

        if (File.Exists(SavePath))
        {
            File.Copy(SavePath, BackupPath, true);
        }

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        File.Move(TempPath, SavePath);
    }

    public void ResetSave()
    {
        DeleteIfExists(SavePath);
        DeleteIfExists(BackupPath);
        DeleteIfExists(TempPath);
    }

    private static bool TryLoadFromPath(string path, out SaveData data)
    {
        data = null;
        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(json);
            if (data == null)
            {
                return false;
            }

            data.EnsureDefaults();
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to load save file at {path}: {exception.Message}");
            data = null;
            return false;
        }
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
