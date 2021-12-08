using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;
using ProyectoMovil.Controller;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ProyectoMovil
{
    public partial class ListaCarritoPage : ContentPage
    {
        CancellationTokenSource cts;
        String usuario;
        String id;
        public ListaCarritoPage()
        {
            InitializeComponent();
            ObtenerCredenciales();
            ListaProductosCarrito();
        }

        private async void ObtenerCredenciales()
        {
            usuario = await SecureStorage.GetAsync("usuario");
            id = await SecureStorage.GetAsync("ID");
        }

        async Task GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    txtlatitud.Text = location.Latitude.ToString();
                    txtlongitud.Text = location.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await DisplayAlert("Alerta", "Geolocalización no soportada", "OK");
                return;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await DisplayAlert("Alerta", "Geolocalización inhabilitada", "OK");
                return;
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Alerta", "Se ha denegado el permiso", "OK");
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", "No se ha podido obtener la ubicación", "OK");
                return;
            }
        }

        protected override void OnDisappearing()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }

        private void ClearScreen()
        {
            txtISV.Text = String.Empty;
            txtSubtotal.Text = String.Empty;
            txtTotal.Text = String.Empty;
        }

        private async void ListaProductosCarrito()
        {
            btnComprarTarjeta.IsVisible = false;
            btnComprarEfectivo.IsVisible = false;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                using (HttpClient cliente = new HttpClient())
                {
                    ClearScreen();
                    object carrito = new { usuario = usuario};

                    String jsonContent = JsonConvert.SerializeObject(carrito);
                    StringContent contenido = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    var response = await cliente.PutAsync(Configuraciones.EndPointCarrito, contenido);

                    if (response.IsSuccessStatusCode)
                    {
                        String respuesta = response.Content.ReadAsStringAsync().Result;

                        if (respuesta.Length > 65)
                        {
                            dynamic array = JsonConvert.DeserializeObject(respuesta);

                            List<Modelos.Carrito> lista = new List<Modelos.Carrito>();

                            foreach (var item in array.carrito)
                            {
                                string img64 = item.imagen.ToString();
                                byte[] newBytes = Convert.FromBase64String(img64);
                                var stream = new MemoryStream(newBytes);

                                double subtotalProducto = Convert.ToDouble(item.cantidad) * Convert.ToDouble(item.precio);

                                lista.Add(new Modelos.Carrito(
                                    item.id_carrito.ToString(), item.nombre_producto.ToString(), item.cantidad.ToString(), item.precio.ToString(), item.tipo_producto.ToString(),
                                    ImageSource.FromStream(() => new MemoryStream(newBytes)), img64, subtotalProducto.ToString()));
                            }
                            collectionViewCart.ItemsSource = lista;
                            txtSubtotal.Text = "Subtotal: L " + array.lista_calculos.Subtotal.ToString();
                            txtISV.Text = "ISV: L " + array.lista_calculos.ISV.ToString();
                            txtTotal.Text = "Total: L " + array.lista_calculos.Total.ToString();
                            btnComprarTarjeta.IsVisible = true;
                            btnComprarEfectivo.IsVisible = true;
                        }
                        else
                        {
                            collectionViewCart.ItemsSource = null;
                            await DisplayAlert("Notificación", "Agregue productos a el carrito", "OK");
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

        private async void SwipeEliminar_Invoked(SwipeItem sender, System.EventArgs e)
        {
            var s = sender.CommandParameter as Modelos.Carrito;

            bool r = await DisplayAlert("Acción", "¿Desea eliminar el producto del carrito?", "Yes", "No");

            if (r)
            {
                object sitio = new {ID = s.id, usuario = usuario};

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    Uri RequestUri = new Uri(Configuraciones.EndPointCarrito);
                    var client = new HttpClient();
                    var json = JsonConvert.SerializeObject(sitio);

                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Delete,
                        RequestUri = RequestUri
                    };

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Notificación", "Producto eliminado del carrito", "OK");
                        ListaProductosCarrito();
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

        private async void btnComprarTarjeta_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new TarjetaPagoPage());
        }

        private async void btnComprarEfectivo_Clicked(System.Object sender, System.EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await GetCurrentLocation();
                var posicion = await CrossGeolocator.Current.GetPositionAsync();

                object producto = new
                {
                    usuario = usuario,
                    latitud = posicion.Latitude.ToString(),
                    longitud = posicion.Longitude.ToString(),
                    t_pago = 1
                };

                String jsonContent = JsonConvert.SerializeObject(producto);
                StringContent contenido = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                Console.WriteLine("JsonContent" + jsonContent);

                using (HttpClient cliente = new HttpClient())
                {
                    HttpResponseMessage response = await cliente.PostAsync(Controller.Configuraciones.EndPointOrden, contenido);

                    if (response.IsSuccessStatusCode)
                    {
                        String respuesta = response.Content.ReadAsStringAsync().Result;

                        JObject respJson = JObject.Parse(respuesta);

                        String msj = respJson["Mensaje"].ToString();
                        Console.WriteLine(respuesta);
                        Console.WriteLine("LENGHT: " + respuesta.Length);

                        if (msj.Length <= 30)
                        {
                            await DisplayAlert("Acción", "Compra realizada con éxito", "OK");
                            await Navigation.PushAsync(new MyFlyoutPage());
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
