using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechVendas.Models;
using TechVendas.Services;

namespace TechVendas.Tests.ClientServices
{
    [TestClass]
    public class SalvarClienteTest
    {
        [TestMethod]
        public void SalvarCliente_DeveRetornarFalse_QuandoOValidacaoReceberUmCpfExistente()
        {
            string cpfExistente = "91411874900";//Arrange (Cenário com um CPF repetido)
            var service = new ClientService();
            var cliente = new Client { Nome = "Teste", CPF = cpfExistente };
            // Salva o cliente para garantir que o CPF exista
            service.Salvar(cliente);
            // Tenta salvar outro cliente com o mesmo CPF
            var resultado = service.Salvar(new Client { Nome = "Teste 2", CPF = cpfExistente });
            Assert.IsFalse(resultado.Sucesso, "Deveria ter falhado ao tentar salvar um cliente com CPF já existente.");
        }
        [TestMethod]
        public void SalvarCliente_DeveRetornarTrue_QuandoOValidacaoReceberUmCpfNovo()
        {
            string cpfNovo = "50674872088"; // Arrange (Cenário com um CPF novo)
            var service = new ClientService();
            var resultado = service.Salvar(new Client { Nome = "Teste Novo", CPF = cpfNovo });
            Assert.IsTrue(resultado.Sucesso, "Deveria ter conseguido salvar um cliente com CPF novo.");
        }
        [TestMethod]
        public void SalvarCliente_DeveRetornarFalse_QuandoOValidacaoReceberUmCpfInvalido()
        {
            string cpfInvalido = "11111111111"; // Arrange (Cenário com um CPF inválido)
            var service = new ClientService();
            var resultado = service.Salvar(new Client { Nome = "Teste CPF Inválido", CPF = cpfInvalido });
            Assert.IsFalse(resultado.Sucesso, "Deveria ter falhado ao tentar salvar um cliente com CPF inválido.");
        }
    }
}
