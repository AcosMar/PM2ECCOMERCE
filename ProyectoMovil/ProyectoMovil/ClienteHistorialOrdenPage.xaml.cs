using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using ProyectoMovil.Controller;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ProyectoMovil
{
    public partial class ClienteHistorialOrdenPage : ContentPage
    {
        String ordenID = null;
        String ordenProd = null;
        String ordenFecha = null;
        String ordenEmp = null;
        String usuario;
        String id;
        public ClienteHistorialOrdenPage()
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
                                if (item.estado.ToString() == "Entregado")
                                {
                                    lista.Add(new Modelos.Orden(item.pk.ToString(), item.cliente.ToString(), item.productos.ToString(),
                                    item.empleado_orden.ToString(), item.subtotal.ToString(), item.isv.ToString(), item.Total.ToString(),
                                    item.t_pago.ToString(), item.fecha_registro.ToString(), item.estado.ToString()));
                                }
                            }
                            listaHistorialOrdenes.ItemsSource = lista;
                        }
                        else
                        {
                            listaHistorialOrdenes.ItemsSource = null;
                            await DisplayAlert("Notificación", "No ha realizado ninguna compra", "OK");
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

        private async void btnCalificar_Clicked(System.Object sender, System.EventArgs e)
        {
            if (System.String.IsNullOrWhiteSpace(ordenID))
            {
                await this.DisplayAlert("Alerta", "Seleccione una orden.", "OK");
                return;
            }
            else
            {
                object orden = new { id = ordenID, fecha_registro = ordenFecha, productos = ordenProd, empleado_orden = ordenEmp };

                var detalleOrden = new ClienteCalificacionPage();
                detalleOrden.BindingContext = orden;
                await Navigation.PushAsync(detalleOrden);
            }
        }

        void listaHistorialOrdenes_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            UpdateSelectionData(e.PreviousSelection, e.CurrentSelection);
        }

        private void UpdateSelectionData(IEnumerable<object> previousSelectedContact, IEnumerable<object> currentSelectedContact)
        {
            var item = currentSelectedContact.FirstOrDefault() as Modelos.Orden;
            ordenID = item.id;
            ordenProd = item.productos;
            ordenFecha = item.fecha_registro;
            ordenEmp = item.empleado_orden;
        }
    }
}
