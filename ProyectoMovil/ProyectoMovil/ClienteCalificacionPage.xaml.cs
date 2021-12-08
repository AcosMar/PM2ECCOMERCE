using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ProyectoMovil
{
    public partial class ClienteCalificacionPage : ContentPage
    {
        public ClienteCalificacionPage()
        {
            InitializeComponent();
        }

        private async void btnCalificarOrden_Clicked(System.Object sender, System.EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                object orden = new { ID = txtIdOrden.Text, CalificacionPedido = Calificacion.SelectedStarValue };

                String jsonContent = JsonConvert.SerializeObject(orden);
                StringContent contenido = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                using (HttpClient cliente = new HttpClient())
                {
                    HttpResponseMessage response = await cliente.PostAsync(Controller.Configuraciones.EndPointPedido, contenido);

                    if (response.IsSuccessStatusCode)
                    {
                        String respuesta = response.Content.ReadAsStringAsync().Result;

                        JObject respJson = JObject.Parse(respuesta);

                        String msj = respJson["Mensaje"].ToString();
                        Console.WriteLine(respuesta);
                        Console.WriteLine("LENGHT: " + respuesta.Length);

                        if (msj.Length >= 10)
                        {
                            await DisplayAlert("Notificación", $"{msj}", "OK");
                            //await Navigation.PushAsync(new MyFlyoutPage());
                        }
                        else
                        {
                            await DisplayAlert("Alerta", $"{msj}", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "Ha ocurrido un problema", "OK");
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
