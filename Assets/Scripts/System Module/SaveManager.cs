using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public void SaveData<T>(string path, T data) where T : class
    {
        string json = JsonUtility.ToJson(data, true);

        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        File.WriteAllText(path, json);
    }

    public T LoadData<T>(string path) where T : class
    {
        if (!File.Exists(path)) return default;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }
}
