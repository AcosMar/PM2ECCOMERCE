using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoMovil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListaPersonaPage : ContentPage
    {
        public ListaPersonaPage()
        {
            InitializeComponent();
        }

        private void LstDatos_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
    }
}