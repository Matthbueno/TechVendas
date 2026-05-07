using System.Windows.Controls;
using TechVendas.ViewModels;

namespace TechVendas.Views
{
    public partial class ProdutoView : UserControl
    {
        public ProdutoView()
        {
            InitializeComponent();
            this.DataContext = new ProdutoViewModel();
        }
    }
}