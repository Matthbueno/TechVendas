using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechVendas.Services;
using TechVendas.Models; 

namespace TechVendas.Tests
{
    [TestClass]
    public class ValidacaoCpfTests
    {
        [TestMethod]
        public void ValidarCpf_DeveRetornarTrue_QuandoOValidacaoReceberUmCpfValido()
        {
            // 1. Arrange (Preparar o cenário com um CPF válido conhecido)
            // Você pode usar o nome real da sua classe aqui, ex: Validadores.ValidarCpf
            string cpfValido = "12345678909"; // Substitua por um CPF válido para testes

            // 2. Act (Executar o seu método de validação)
            var service = new ClientService();
            bool resultado = service.IsCpfValido(cpfValido);

            // 3. Assert (Verificar se o resultado foi True)
            Assert.IsTrue(resultado, "O CPF deveria ter sido considerado válido.");
        }
        [TestMethod]
        public void ValidarCpf_DeveRetornarTrue_QuandoOValidacaoReceberUmCpfValidoComCaracteres()
        {
            // 1. Arrange (Preparar o cenário com um CPF válido conhecido)
            string cpfValido = "123.456.789-09";

            // 2. Act (Executar o seu método de validação)
            var service = new ClientService();
            bool resultado = service.IsCpfValido(cpfValido);

            // 3. Assert (Verificar se o resultado foi True)
            Assert.IsTrue(resultado, "O CPF deveria ter sido considerado válido.");
        }

        [TestMethod]
        public void ValidarCpf_DeveRetornarFalse_QuandoOValidacaoReceberUmCpfComTodosDigitosIguais()
        {
            // 1. Arrange (Cenário com um CPF sabidamente errado)
            string cpfInvalido = "11111111111";

            // 2. Act
            var service = new ClientService();
            bool resultado = service.IsCpfValido(cpfInvalido);

            // 3. Assert
            Assert.IsFalse(resultado, "O CPF deveria ter sido considerado inválido.");
        }
        [TestMethod]
        public void ValidarCpf_DeveRetornarFalse_QuandoOValidacaoReceberUmCpfComLetras()
        {
            // 1. Arrange (Cenário com um CPF contendo letras)
            string cpfComLetras = "1234567890A";
            // 2. Act
            var service = new ClientService();
            bool resultado = service.IsCpfValido(cpfComLetras);
            // 3. Assert
            Assert.IsFalse(resultado, "O CPF deveria ter sido considerado inválido por conter letras.");
        }
        [TestMethod]
        public void ValidarCpf_DeveRetornarFalse_QuandoOValidacaoReceberUmCpfComMenosDe11Digitos()
        {
            // 1. Arrange (Cenário com um CPF com menos de 11 dígitos)
            string cpfCurto = "1234567890"; // Apenas 10 dígitos
            // 2. Act
            var service = new ClientService();
            bool resultado = service.IsCpfValido(cpfCurto);
            // 3. Assert
            Assert.IsFalse(resultado, "O CPF deveria ter sido considerado inválido por conter menos de 11 dígitos.");
        }
        [TestMethod]
        public void ValidarCpf_DeveRetornarFalse_QuandoOValidacaoReceberUmCpfComMaisDe11Digitos()
        {
            // 1. Arrange (Cenário com um CPF com mais de 11 dígitos)
            string cpfLongo = "123456789012"; // 12 dígitos
            // 2. Act
            var service = new ClientService();
            bool resultado = service.IsCpfValido(cpfLongo);
            // 3. Assert
            Assert.IsFalse(resultado, "O CPF deveria ter sido considerado inválido por conter mais de 11 dígitos.");
        }
        [TestMethod]
        public void ValidarCpf_DeveRetornarFalse_QuandoOValidacaoReceberUmCpfExistente()
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
    }
}   