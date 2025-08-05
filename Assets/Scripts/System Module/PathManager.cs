using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathCategory
{
    Custom = 0,
    DataPath,
    StreamingAssetPath,
    PersistentDataPath,
    TemporaryCachePath,
    ConsoleLogPath
}

public class PathManager : Singleton<PathManager>
{
    public string GetPathFromCategory(PathCategory pathCategory)
    {
        string path = "";
        switch (pathCategory)
        {
            case PathCategory.Custom:
                break;
            case PathCategory.DataPath:
                path = Application.dataPath;
                break;
            case PathCategory.StreamingAssetPath:
                path = Application.streamingAssetsPath;
                break;
            case PathCategory.PersistentDataPath:
                path = Application.persistentDataPath;
                break;
            case PathCategory.TemporaryCachePath:
                path = Application.temporaryCachePath;
                break;
            case PathCategory.ConsoleLogPath:
                path = Application.consoleLogPath;
                break;
        }
        return path;
    }
}
