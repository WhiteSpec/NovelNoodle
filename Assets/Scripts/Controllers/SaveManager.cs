using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static string SAVED_GAME_PREFIX = "savedGame_";

    public static void SaveGame(SaveData data, string dataHolderID)
    {
        PlayerPrefs.SetString(SAVED_GAME_PREFIX + dataHolderID, JsonUtility.ToJson(data));
    }

    public static SaveData LoadGame(string dataHolderID)
    {
        return JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SAVED_GAME_PREFIX + dataHolderID));
    }

    public static bool IsGameSaved(string dataHolderID)
    {
        return PlayerPrefs.HasKey(SAVED_GAME_PREFIX + dataHolderID);
    }

    public static void ClearSavedGame(string dataHolderID)
    {
        PlayerPrefs.DeleteKey(SAVED_GAME_PREFIX + dataHolderID);
    }
}
