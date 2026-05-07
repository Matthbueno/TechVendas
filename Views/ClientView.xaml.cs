using System.Windows.Controls;
using TechVendas.ViewModels;

namespace TechVendas.Views
{
    public partial class ClientView : UserControl
    {
        public ClientView()
        {
            InitializeComponent();

            this.DataContext = new ClientViewModel();
        }
    }
}