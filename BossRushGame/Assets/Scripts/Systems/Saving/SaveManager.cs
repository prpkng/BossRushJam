using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BRJ.Systems.Saving
{
    public static class SaveManager
    {
        private static SaveData currentSaveData;

        public static readonly string SavePath = Application.persistentDataPath + "/save.dat";

        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static void DeleteSave()
        {
            if (HasSave())
                File.Delete(SavePath);
        }

        public static void LoadData()
        {
            if (File.Exists(SavePath))
            {
                BinaryFormatter binaryFormatter = new();
                FileStream file = File.Open(SavePath, FileMode.Open);

                currentSaveData = (SaveData)binaryFormatter.Deserialize(file);

                file.Close();
                Debug.Log("SAVE: Successfully loaded save file");
            }
            else
            {
                Debug.Log("SAVE: Save file not found, creating new save file");
                SaveData();
            }

        }

        public static void SaveData()
        {
            BinaryFormatter binaryFormatter = new();
            FileStream file = File.Open(SavePath, FileMode.OpenOrCreate);

            binaryFormatter.Serialize(file, currentSaveData);

            file.Close();
            Debug.Log("SAVE: Successfully saved save file");
        }



        public static string GetLastEnteredBoss()
        {
            LoadData();
            return currentSaveData.LastEnteredBoss;
        }

        public static void SetLastEnteredBoss(string boss)
        {
            currentSaveData.LastEnteredBoss = boss;
            SaveData();
        }
        public static Type GetCurrentModifierType()
        {
            LoadData();
            if (currentSaveData.CurrentModifierType == null)
                return null;
            return Type.GetType(currentSaveData.CurrentModifierType);
        }

        public static void SetCurrentModifierType(Type type)
        {
            Debug.Log($"SAVE: Setting current modifier type to: {type.FullName}");
            currentSaveData.CurrentModifierType = type.FullName;
            SaveData();
        }

        public static SaveData GetSaveData()
        {
            LoadData();
            return currentSaveData;
        }
    }
}