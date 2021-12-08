using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using ProyectoMovil.Controller;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ProyectoMovil
{
    public partial class ClienteOrdenActivaPage : ContentPage
    {
        String usuario;
        String id;
        public ClienteOrdenActivaPage()
        {
            InitializeComponent();
            ObtenerCredenciales();
            ClienteListaOrdenes();
        }

        private async void ObtenerCredenciales()
        {
            usuario = await SecureStorage.GetAsync("usuario");
            id = await SecureStorage.GetAsync("ID");
        }

        private async void ClienteListaOrdenes()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                using (HttpClient cliente = new HttpClient())
                {
                    object obj = new { usuario = usuario };

                    String jsonContent = JsonConvert.SerializeObject(obj);
                    StringContent contenido = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    var response = await cliente.PutAsync(Configuraciones.EndPointOrden, contenido);

                    if (response.IsSuccessStatusCode)
                    {
                        String respuesta = response.Content.ReadAsStringAsync().Result;

                        if (respuesta.Length > 65)
                        {
                            dynamic array = JsonConvert.DeserializeObject(respuesta);

                            List<Modelos.Orden> lista = new List<Modelos.Orden>();

                            foreach (var item in array.orden)
                            {
                                if (!(item.estado.ToString() == "Entregado"))
                                {
                                    lista.Add(new Modelos.Orden(item.pk.ToString(), item.cliente.ToString(), item.productos.ToString(),
                                    item.empleado_orden.ToString(), item.subtotal.ToString(), item.isv.ToString(), item.Total.ToString(),
                                    item.t_pago.ToString(), item.fecha_registro.ToString(), item.estado.ToString()));
                                }
                            }
                            listaOrdenActiva.ItemsSource = lista;
                        }
                        else
                        {
                            listaOrdenActiva.ItemsSource = null;
                            await DisplayAlert("Notificación", "No hay orden activa. Haga un pedido", "OK");
                            return;
                        }
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
