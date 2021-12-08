using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoMovil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
            ObtenerCredenciales();

        }


        private async void ObtenerCredenciales()
        {
            var usuario = await SecureStorage.GetAsync("usuario");
            var id = await SecureStorage.GetAsync("ID");
            BtnSingin.Text = usuario;

        }

        private async void BtnSingin_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Que deseas hacer?", "Cancel", null, "Modificar tus datos", "Cerrar Sesion");

            if (action == "Modificar tus datos")
            {
                string mod = await DisplayActionSheet("Usted quiere modificar sus datos?", "Yes", "No", null, "", "");

                if (mod == "Yes")
                {
                    await Navigation.PushAsync(new ModificarRPersonaPage());
                }

            }

            else if (action == "Cerrar Sesion")
            {
                string cierre = await DisplayActionSheet("Usted quiere Cerrar Sesion?", "Yes", "No", null, "", "");

                if (cierre == "Yes")
                {

                    await Navigation.PushAsync(new MainPage());
                }
            }

        }
    }
}