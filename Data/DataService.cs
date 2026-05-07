using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TechVendas.Data
{
    public class JsonDataService<T>
    {
        private readonly string _filePath;

        // O construtor recebe apenas o nome do arquivo (ex: "clientes")
        public JsonDataService(string fileName)
        {
            // Cria um caminho padronizado: "Data\clientes.json"
            _filePath = Path.Combine("Data", $"{fileName}.json");
        }

        public List<T> LoadData()
        {
            // Se o arquivo não existir ainda, retorna uma lista vazia e não quebra o sistema
            if (!File.Exists(_filePath))
                return new List<T>();

            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public void SaveData(List<T> data)
        {
            // Pega o diretório do arquivo ("Data") e garante que a pasta exista no Windows
            string directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Converte a lista para texto JSON bonitinho (Indented) e salva
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}