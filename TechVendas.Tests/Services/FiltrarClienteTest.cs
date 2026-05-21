using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using TechVendas.Models;
using TechVendas.Services;

namespace TechVendas.Tests.Services
{
    [TestClass]
    public class FiltrarClienteTest
    {
        private ClientService _service;

        [TestInitialize]
        public void PrepararCenario()
        {   
            //O caminho da pasta onde os testes estão rodando (ex: "bin\Debug")
            string caminhoPastaTestes = AppDomain.CurrentDomain.BaseDirectory;
            //O caminho do arquivo JSON que a service vai ler (ex: "bin\Debug\Data\clientes.json")
            string caminhoArquivoJsonDeTeste = Path.Combine(caminhoPastaTestes, "Data", "clientes.json");

            //Se o arquivo existir na pasta de testes, nós forçamos ele a ficar vazio "[]"
            if (File.Exists(caminhoArquivoJsonDeTeste))
            {
                File.WriteAllText(caminhoArquivoJsonDeTeste, "[]");
            }

            //Agora a service vai ler o arquivo que acabamos de zerar ali em cima
            _service = new ClientService();

            //Cria dados de teste para o cenário
            _service.Salvar(new Client { Nome = "João Silva", CPF = "12345678909" });
            _service.Salvar(new Client { Nome = "teste filtro", CPF = "32317215045" });
        }

        [TestMethod]
        public void FiltrarClientes_PorNomeExistente_DeveRetornarListaComResultados()
        {
            var service = new ClientService();
            string nomeBuscado = "teste";
            var filtro = service.FiltrarClientes(nomeBuscado, null);
            var resultado = filtro.ToList();

            //Garante que a lista não veio nula (evita erros que quebram o sistema)
            Assert.IsNotNull(resultado, "A lista de resultados não deveria ser nula.");

            //Garante que encontrou pelo menos 1 cliente
            Assert.IsTrue(resultado.Count > 0, "Deveria ter encontrado pelo menos um cliente com o nome buscado.");

            //A PROVA REAL: Garante que o filtro não trouxe pessoas erradas (ex: Maria)
            //O comando "All" verifica se TODOS os itens da lista passam na condição.
            Assert.IsTrue(resultado.All(c => c.Nome.ToLower().Contains(nomeBuscado.ToLower())), "O filtro trouxe clientes que não possuem 'teste' no nome.");
        }

        [TestMethod]
        public void FiltrarClientes_PorCpfExistente_DeveRetornarListaComResultados()
        {
            string cpfBuscado = "32317215045";
            var service = new ClientService();
            var filtro = service.FiltrarClientes(null, cpfBuscado);
            var resultado = filtro.ToList();

            //Garante que a lista não veio nula (evita erros que quebram o sistema)
            Assert.IsNotNull(resultado, "A lista de resultados não deveria ser nula.");
            //Garante que encontrou pelo menos 1 cliente
            Assert.IsTrue(resultado.Count > 0, "Deveria ter encontrado pelo menos um cliente com o nome buscado.");
            //Garante que o filtro não trouxe pessoas erradas (ex: Maria)
            Assert.IsTrue(resultado.All(c => c.CPF == cpfBuscado), "O filtro deve trazer o cliente exato");
        }

        [TestMethod]
        public void FiltrarClientes_PorCpfENomeNulo_DeveRetornarListaComResultados()
        {
            var service = new ClientService();
            var filtro = service.FiltrarClientes(null, null);
            var resultado = filtro.ToList();

            //Garante que a lista não veio nula (evita erros que quebram o sistema)
            Assert.IsNotNull(resultado, "A lista de resultados não deveria ser nula.");
            //Garante que encontrou pelo menos 1 cliente
            Assert.IsTrue(resultado.Count > 0, "Deveria ter encontrado pelo menos um cliente com o nome buscado.");
        }

        [TestMethod]
        public void FiltrarClientes_NomeInexistente_DeveRetornarListaVazia()
        {
            var service = new ClientService();
            string nomeBuscado = "XptoZikZiraDaSilva";
            var resultado = service.FiltrarClientes(nomeBuscado, null).ToList();

            //Garante que a lista não veio nula (evita erros que quebram o sistema)
            Assert.IsNotNull(resultado, "A lista não deve ser nula, mesmo sem resultados.");
            //Garante que a lista está vazia, pois o nome não existe
            Assert.AreEqual(0, resultado.Count, "A lista deveria estar vazia, pois o cliente não existe.");
        }

        [TestMethod]
        public void FiltrarClientes_PorNomeECpfCorretos_DeveRetornarCliente()
        {
            var service = new ClientService();
            string nome = "teste filtro";
            string cpf = "32317215045";
            var resultado = service.FiltrarClientes(nome, cpf).ToList();

            //Garante que a lista não veio nula (evita erros que quebram o sistema)
            Assert.IsTrue(resultado.Count == 1, "Deveria encontrar exatamente 1 cliente com essa combinação.");
            //Garante que o cliente encontrado tem o nome e CPF corretos
            Assert.AreEqual(nome, resultado[0].Nome);
            Assert.AreEqual(cpf, resultado[0].CPF);
        }
    }
}
