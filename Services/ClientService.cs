using System.Collections.Generic;
using System.Linq;
using TechVendas.Models;
using TechVendas.Data;

namespace TechVendas.Services
{
    public class ClientService
    {
        private readonly JsonDataService<Client> _dataService;

        public ClientService()
        {
            _dataService = new JsonDataService<Client>("clientes");
        }

        public List<Client> ObterTodos()
        {
            return _dataService.LoadData() ?? new List<Client>();
        }

        public List<Client> FiltrarClientes(string nome, string cpf)
        {
            var query = ObterTodos().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(c => c.Nome != null && c.Nome.ToLower().Contains(nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(cpf))
                query = query.Where(c => c.CPF != null && c.CPF.ToString().Contains(cpf));

            return query.ToList();
        }

        public (bool Sucesso, string Mensagem) Salvar(Client client)
        {
            if (!IsCpfValido(client.CPF?.ToString()))
                return (false, "O CPF digitado é inválido! Por favor, verifique.");

            var lista = ObterTodos();
            var existente = lista.FirstOrDefault(c => c.Id == client.Id);
            var cpfExistente = lista.FirstOrDefault(c => c.CPF == client.CPF && c.Id != client.Id);

            if (cpfExistente != null)
                return (false, "Já existe um cliente cadastrado com este CPF! Por favor, verifique.");

            if (existente != null)
            {
                existente.Nome = client.Nome;
                existente.CPF = client.CPF;
            }
            else
            {
                client.Id = lista.Any() ? lista.Max(c => c.Id) + 1 : 1;
                lista.Add(client);
            }

            _dataService.SaveData(lista);
            return (true, "Cliente salvo com sucesso!");
        }

        public void Excluir(int id)
        {
            var lista = ObterTodos();
            var existente = lista.FirstOrDefault(c => c.Id == id);

            if (existente != null)
            {
                lista.Remove(existente);
                _dataService.SaveData(lista);
            }
        }

        public bool IsCpfValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());
            if (cpf.Length != 11) return false;
            if (new string(cpf[0], 11) == cpf) return false;

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;
            for (int i = 0; i < 9; i++) soma += int.Parse(cpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            if (int.Parse(cpf[9].ToString()) != resto) return false;

            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;
            for (int i = 0; i < 10; i++) soma += int.Parse(cpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            return int.Parse(cpf[10].ToString()) == resto;
        }
    }
}