using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using ProyectoMovil.Controller;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ProyectoMovil
{
    public partial class EmpleadoOrdenPage : ContentPage
    {
        String usuario;
        String id;
        public EmpleadoOrdenPage()
        {
            InitializeComponent();
            ObtenerCredenciales();
            EmpleadoListaOrdenes();
        }

        private async void ObtenerCredenciales()
        {
            usuario = await SecureStorage.GetAsync("usuario");
            id = await SecureStorage.GetAsync("ID");
        }

        private async void EmpleadoListaOrdenes()
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

                            List<Modelos.OrdenEmpleado> lista = new List<Modelos.OrdenEmpleado>();

                            foreach (var item in array.orden)
                            {
                                lista.Add(new Modelos.OrdenEmpleado(item.pk.ToString(), item.cliente.ToString(), item.telefono.ToString(), item.productos.ToString(),
                                item.empleado_orden.ToString(), item.subtotal.ToString(), item.isv.ToString(), item.Total.ToString(),
                                item.t_pago.ToString(), item.fecha_registro.ToString(), item.estado.ToString(), item.longitud.ToString(), item.latitud.ToString()));
                            }
                            listaOrdenesEmpleado.ItemsSource = lista;
                        }
                        else
                        {
                            listaOrdenesEmpleado.ItemsSource = null;
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

        private async void SwipeAceptar_Invoked(SwipeItem sender, System.EventArgs e)
        {
            var s = sender.CommandParameter as Modelos.OrdenEmpleado;

            if (s.estado == "Proceso")
            {
                await DisplayAlert("Notificación", "Esta orden ya fue aceptada.", "OK");
            }
            else
            {

                object orden = new { ID = s.id, estado = 2, longitud = s.longitud, latitud = s.latitud };

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    Uri RequestUri = new Uri(Configuraciones.EndPointOrden);
                    var client = new HttpClient();
                    var json = JsonConvert.SerializeObject(orden);

                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Delete,
                        RequestUri = RequestUri
                    };

                    HttpResponseMessage response = await client.SendAsync(request);
                    String respuesta = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Notificación", "Orden Aceptada para entrega", "OK");
                        EmpleadoListaOrdenes();
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "Ha ocurrido un error", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Verifique su conexión de Internet", "OK");
                }
            }
        }

        private async void SwipeVerMapa_Invoked(SwipeItem sender, System.EventArgs e)
        {
            var s = sender.CommandParameter as Modelos.OrdenEmpleado;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (!double.TryParse(s.latitud, out double lat))
                    return;
                if (!double.TryParse(s.longitud, out double lng))
                    return;
                await Map.OpenAsync(lat, lng, new MapLaunchOptions
                {

                    NavigationMode = NavigationMode.Driving

                });
            }
            else
            {
                await DisplayAlert("Error", "Verifique su conexión de Internet", "OK");
            }
        }

        private async void SwipeEntregada_Invoked(SwipeItem sender, System.EventArgs e)
        {
            var s = sender.CommandParameter as Modelos.OrdenEmpleado;

            if (s.estado == "Activo")
            {
                await DisplayAlert("Notificación", "Debe aceptar primero esta orden.", "OK");
            }
            else
            {
                object orden = new { ID = s.id, estado = 3, longitud = s.longitud, latitud = s.latitud };

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    Uri RequestUri = new Uri(Configuraciones.EndPointOrden);
                    var client = new HttpClient();
                    var json = JsonConvert.SerializeObject(orden);

                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Delete,
                        RequestUri = RequestUri
                    };

                    HttpResponseMessage response = await client.SendAsync(request);

                    String respuesta = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Notificación", "Su orden ha sido entregada", "OK");
                        EmpleadoListaOrdenes();
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "Ha ocurrido un error", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Verifique su conexión de Internet", "OK");
                }
            }
        }
    }
}
