using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProyectoMovil.Modelos;
using System.Net.Http;
using System.Net;
using System.Dynamic;
using Xamarin.Essentials;

namespace ProyectoMovil
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //txtUser.Text = "aguilera18";
            //txtPass.Text = "Alejandro189";
            txtUser.Text = "andrea";
            txtPass.Text = "admin123";
        }

        private async void btnIngresar_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUser.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese su usuario ", "Ok");
            }
            else if (String.IsNullOrEmpty(txtPass.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese una contraseña ", "Ok");
            }
            else
            {
                Login log = new Login
                {
                    user = txtUser.Text,
                    pass = txtPass.Text
                };

                Uri RequestUri = new Uri("https://appmovil2.herokuapp.com/Api/Login");

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(log);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(RequestUri, contentJson);



                if (response.StatusCode == HttpStatusCode.OK)
                {
                    String jsonx = response.Content.ReadAsStringAsync().Result;

                    JObject jsons = JObject.Parse(jsonx);

                    Boolean Data = Convert.ToBoolean(jsons["Data"].ToString());
                    String Mensaje = jsons["Mensaje"].ToString();
                    Boolean empleado = Convert.ToBoolean(jsons["PermisoE"].ToString());

                    Console.WriteLine(Data.ToString());
                    Console.WriteLine(Mensaje);

                    if (Data == true)
                    {

                        if (empleado == false)
                        {
                            try
                            {
                                SecureStorage.RemoveAll();
                                await SecureStorage.SetAsync("usuario", txtUser.Text);
                                await SecureStorage.SetAsync("ID", txtPass.Text);

                            }
                            catch (Exception ex)
                            {
                                // Si entra aqui es porque el dispositivo no acepta el almacenamiento seguro
                            }

                            await DisplayAlert("Empleado", " " + empleado, "Ok");

                        }
                        else
                        {
                            try
                            {
                                SecureStorage.RemoveAll();
                                await SecureStorage.SetAsync("usuario", txtUser.Text);
                                await SecureStorage.SetAsync("ID", txtPass.Text);

                            }
                            catch (Exception ex)
                            {
                                // Si entra aqui es porque el dispositivo no acepta el almacenamiento seguro
                            }



                            await Navigation.PushAsync(new MyFlyoutPage());
                        }
                    }
                    else
                    {
                        await DisplayAlert("Advertencia", "" + Mensaje, "Ok");
                        txtPass.Text = "";
                        txtUser.Text = "";
                    }

                }
                else
                {
                    await DisplayAlert("Error", "Estamos en mantenimiento", "Ok");
                }
            }
            
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var a = new RegistrarUsuario();
            await Navigation.PushAsync(a);
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            var a = new RecuperarContra();
            await Navigation.PushAsync(a);
        }
    }
}
