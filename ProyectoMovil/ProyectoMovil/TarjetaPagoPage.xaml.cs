using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace ProyectoMovil
{
    public partial class TarjetaPagoPage : ContentPage
    {
        CancellationTokenSource cts;
        String usuario;
        String id;
        public TarjetaPagoPage()
        {
            InitializeComponent();
            ObtenerCredenciales();
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

        private async void btnPagarTarjetaCredito_Clicked(System.Object sender, System.EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (System.String.IsNullOrWhiteSpace(txtTarjeta.Text))
                {
                    await this.DisplayAlert("Alerta", "Ingrese el número de tarjeta.", "OK");
                    return;
                }
                else if (System.String.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    await this.DisplayAlert("Alerta", "Ingrese el nombre de la persona.", "OK");
                    return;
                }
                else if (System.String.IsNullOrWhiteSpace(txtExpiracion.Text))
                {
                    await this.DisplayAlert("Alerta", "Ingrese la fecha de expiración.", "OK");
                    return;
                }
                else if (System.String.IsNullOrWhiteSpace(txtCCV.Text))
                {
                    await this.DisplayAlert("Alerta", "Ingrese los número de CCV.", "OK");
                    return;
                }

                await GetCurrentLocation();

                var posicion = await CrossGeolocator.Current.GetPositionAsync();

                object producto = new {usuario = usuario,
                    latitud = posicion.Latitude.ToString(),
                    longitud = posicion.Longitude.ToString(),
                    t_pago = 2
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
