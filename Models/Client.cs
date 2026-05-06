using System;

namespace TechVendas.Models
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; }
        public string CPF { get; set; }
    }
}