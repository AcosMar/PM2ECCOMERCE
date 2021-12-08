using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProyectoMovil.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace ProyectoMovil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecuperarContra : ContentPage
    {
        public RecuperarContra()
        {
            InitializeComponent();
        }

        private async void BtnRecuperar_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtCorreo.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese un correo ", "Ok");
            }
            else
            {
                recuContra recu = new recuContra
                {
                    correo = txtCorreo.Text                    
                };

                Uri RequestUri = new Uri("https://appmovil2.herokuapp.com/Api/Login");

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(recu);

                
                

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Delete,
                    RequestUri = RequestUri
                };
                

                HttpResponseMessage response = await client.SendAsync(request);

                Console.WriteLine(response);

                if (response.IsSuccessStatusCode)
                {
                    String jsonx = response.Content.ReadAsStringAsync().Result;
                    JObject jsons = JObject.Parse(jsonx);
                    
                    String  validacion = jsons["a"].ToString();

                    

                    if (validacion == "1")
                    {
                        String Mensaje = jsons["URL"].ToString();
                        url.Text = Mensaje;
                        BtnLINK.IsVisible = true;

                    }
                    else if (validacion == "0")
                    {
                        String Mensaje = jsons["Mensaje"].ToString();
                        await DisplayAlert("ERROR", Mensaje, "Ok");
                    }
                    
                }
                else
                {
                    await DisplayAlert("Notificación", "Ha ocurrido un error", "Ok");
                }

            }
        }


        private void BtnLINK_Clicked(object sender, EventArgs e)
        {
            Browser.OpenAsync(url.Text, BrowserLaunchMode.SystemPreferred);
        }
    }
}