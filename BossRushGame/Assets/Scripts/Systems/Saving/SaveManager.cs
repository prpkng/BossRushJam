using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.Systems.Saving {
    public static class SaveManager {
        private static SaveData currentSaveData;

        public static readonly string SavePath = Application.persistentDataPath + "/save.dat";

        public static void LoadData() {
            if (File.Exists(SavePath)) {
                BinaryFormatter binaryFormatter = new();
                FileStream file = File.Open(SavePath, FileMode.Open);

                currentSaveData = (SaveData)binaryFormatter.Deserialize(file);

                file.Close();
                Debug.Log("SAVE: Successfully loaded save file");
            } else {
                Debug.Log("SAVE: Save file not found, creating new save file");
                SaveData();
            }

        }

        public static void SaveData() {
            BinaryFormatter binaryFormatter = new();
            FileStream file = File.Open(SavePath, FileMode.OpenOrCreate);

            binaryFormatter.Serialize(file, currentSaveData);

            file.Close();
            Debug.Log("SAVE: Successfully saved save file");
        }



        public static string GetLastEnteredBoss() {
            LoadData();
            return currentSaveData.LastEnteredBoss;
        }

        public static void SetLastEnteredBoss(string boss) {
            currentSaveData.LastEnteredBoss = boss;
            SaveData();
        }
    }
}