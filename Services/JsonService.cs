using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace TechVendas.Services
{
    public class JsonDataService<T>
    {
        private readonly string _filePath;

        public JsonDataService(string fileName)
        {
            if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");
            _filePath = $"Data/{fileName}.json";
        }

        public List<T> LoadData()
        {
            if (!File.Exists(_filePath)) return new List<T>();
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public void SaveData(List<T> data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}