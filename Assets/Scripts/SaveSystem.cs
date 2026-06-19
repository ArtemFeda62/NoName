using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/progress.json";

    public static void SaveProgress(int levelIndex)
    {
        ProgressData data = new ProgressData
        {
            lastLevel = levelIndex
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Прогресс сохранён: уровень {levelIndex}");
    }

    public static int LoadProgress()
    {
        if (!HasSaveData())
        {
            Debug.Log("Сохранений нет, начинаем с уровня 1");
            return 1;
        }

        string json = File.ReadAllText(SavePath);
        ProgressData data = JsonUtility.FromJson<ProgressData>(json);
        Debug.Log($"Загружен уровень: {data.lastLevel}");
        return data.lastLevel;
    }

    public static bool HasSaveData()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteProgress()
    {
        if (HasSaveData())
        {
            File.Delete(SavePath);
            Debug.Log("Прогресс удалён");
        }
    }
}

[System.Serializable]
public class ProgressData
{
    public int lastLevel;
}