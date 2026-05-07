using System.Windows.Controls;
using TechVendas.Models;

namespace TechVendas.Views
{
    public partial class PedidoView : UserControl
    {
        public PedidoView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.PedidoViewModel();
        }

        public PedidoView(Client clientePreSelecionado)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.PedidoViewModel(clientePreSelecionado);
        }
    }
}