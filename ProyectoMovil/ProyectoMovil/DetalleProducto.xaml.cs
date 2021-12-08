using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoMovil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetalleProducto : ContentPage
    {
        int cantidadProducto = 1;
        String usuario;
        String id;

        public DetalleProducto()
        {
            InitializeComponent();
            ObtenerCredenciales();
        }

        private async void ObtenerCredenciales()
        {
            usuario = await SecureStorage.GetAsync("usuario");
            id = await SecureStorage.GetAsync("ID");
        }

        private async void btnAumentarCant_Clicked(System.Object sender, System.EventArgs e)
        {
            if (cantidadProducto == Convert.ToInt32(txtCantInventario.Text))
            {
                await DisplayAlert("Alerta", "Ha alcanzado la cantidad máxima disponible", "OK");
            }
            else
            {
                cantidadProducto++;
                txtCantidad.Text = cantidadProducto.ToString();
            }
        }

        void btnDisminuirCant_Clicked(System.Object sender, System.EventArgs e)
        {
            if (cantidadProducto == 1)
                return;
            else
                cantidadProducto--;

            txtCantidad.Text = cantidadProducto.ToString();
        }

       private async void btnAgregarCarrito_Clicked(System.Object sender, System.EventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                object producto = new
                {
                    producto = Convert.ToInt32(txtIdProducto.Text),
                    cantidad = Convert.ToDecimal(txtCantidad.Text),
                    usuario = usuario
            };

                String jsonContent = JsonConvert.SerializeObject(producto);
                StringContent contenido = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                using (HttpClient cliente = new HttpClient())
                {
                    HttpResponseMessage response = await cliente.PostAsync(Controller.Configuraciones.EndPointCarrito, contenido);

                    if (response.IsSuccessStatusCode)
                    {
                        String respuesta = response.Content.ReadAsStringAsync().Result;

                        JObject respJson = JObject.Parse(respuesta);

                        String msj = respJson["Mensaje"].ToString();

                        if (msj.Length >= 56)
                        {
                            String msj2 = respJson["Mensaje"]["data"].ToString();
                            await DisplayAlert("Alerta", $"{msj2}", "OK");
                        }
                        else
                        {
                            bool r = await DisplayAlert("Acción", "Producto agregado correctamente. \n¿Desea ver el carrito?", "Yes", "No");
                            if (r)
                            {
                                await Navigation.PushAsync(new ListaCarritoPage());
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "Ha ocurrido un problema al registar el producto", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Error", "Verifique su conexión de Internet", "OK");
            }
        }
    }
}