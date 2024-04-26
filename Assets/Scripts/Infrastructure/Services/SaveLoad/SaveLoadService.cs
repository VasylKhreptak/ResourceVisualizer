using System;
using System.IO;
using Infrastructure.Services.SaveLoad.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace Infrastructure.Services.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        public void Save<T>(string key, T data)
        {
            string jsonData = JsonConvert.SerializeObject(data);

            string path = Path.Combine(Application.persistentDataPath, key);

            File.WriteAllText(path, jsonData);
        }

        public T Load<T>(string key, T defaultValue = default)
        {
            string path = Path.Combine(Application.persistentDataPath, key);

            if (File.Exists(path))
            {
                string jsonData = File.ReadAllText(path);

                try
                {
                    T instance = JsonConvert.DeserializeObject<T>(jsonData);

                    if (instance == null)
                        return defaultValue;

                    return instance;
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        public bool HasKey(string key)
        {
            string path = Path.Combine(Application.persistentDataPath, key);

            return File.Exists(path);
        }

        public void Delete(string key)
        {
            string path = Path.Combine(Application.persistentDataPath, key);

            if (File.Exists(path))
                File.Delete(path);
        }
    }
}