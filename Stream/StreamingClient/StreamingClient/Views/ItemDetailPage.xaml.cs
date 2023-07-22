using StreamingClient.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace StreamingClient.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}