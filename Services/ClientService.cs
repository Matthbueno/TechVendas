using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using TechVendas.Models;

namespace TechVendas.Services
{
    public class ClientService 
    {
        private readonly string filePath = @"Data\clients.json";

        public List<Client> ObterTodas()
        {
            if (!File.Exists(filePath)) return new List<Client>();
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Client>>(json);
        }

        public void Salvar(List<Client> clients)
        {
            string json = JsonConvert.SerializeObject(clients, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Filtro de clientes por nome e cpf
        public List<Client> FiltrarPorNome(string nome)
        {
            var clients = ObterTodas();
            return clients.Where(client => client.Nome.ToLower().Contains(nome.ToLower())).ToList();
        }

        public List<Client> FiltrarPorCpf(int cpf)
        {
            var clients = ObterTodas();
            return clients.Where(client => client.CPF.ToString().Contains(cpf.ToString())).ToList();
        }

    }
}