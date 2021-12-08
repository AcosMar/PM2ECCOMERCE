using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProyectoMovil.Modelos;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoMovil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModificarContra : ContentPage
    {
        public string contra1;
        public string contra2;
        public ModificarContra(string ID, string nombres, string apellidos)
        {
            InitializeComponent();
            idSitio.Text = ID;
            txtusuario.Text = nombres + " " + apellidos;
        }

        private async void BtnModificar_Clicked(object sender, EventArgs e)
        {
            contra1 = txtcontra1.Text;
            contra2 = txtcontra2.Text;
            if (ValidarDatos())
            {
                if (contra1 == contra2)
                {

                    ModContra ModificarContra = new ModContra
                    {
                        ID = Convert.ToInt32(this.idSitio.Text),
                        contraVieja = txtvieja.Text,
                        password1 = txtcontra1.Text,
                        password2 = txtcontra2.Text
                    };


                    Uri RequestUri = new Uri("https://appmovil2.herokuapp.com/Api/Login");

                    var client = new HttpClient();
                    var json = JsonConvert.SerializeObject(ModificarContra);
                    var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(RequestUri, contentJson);



                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        await DisplayAlert("Success", "Contraseñas modificadas correctamente", "Ok");



                    }
                    else
                    {
                        await DisplayAlert("Error", "Estamos en mantenimiento", "Ok");
                    }
                }

                else
                {
                    await DisplayAlert("Advertencia", "Verifique las contraseñas", "ok");
                }


            }
            else
            {
                await DisplayAlert("Advertencia", "Ingresar todos los datos", "ok");
            }

        }

        public bool ValidarDatos()
        {
            bool respuesta;

            if (String.IsNullOrEmpty(txtusuario.Text))
            {
                respuesta = false;
            }

            else if (String.IsNullOrEmpty(txtcontra1.Text))
            {
                respuesta = false;
            }

            else if (String.IsNullOrEmpty(txtcontra2.Text))
            {
                respuesta = false;
            }
            else if (String.IsNullOrEmpty(txtvieja.Text))
            {
                respuesta = false;
            }
            else if (String.IsNullOrEmpty(idSitio.Text))
            {
                respuesta = false;
            }
            else
            {
                respuesta = true;
            }
            return respuesta;
        }
    }
}