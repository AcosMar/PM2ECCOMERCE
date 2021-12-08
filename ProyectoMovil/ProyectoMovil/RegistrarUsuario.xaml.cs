using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Plugin.Media;
using System.IO;
using ProyectoMovil.Modelos;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ProyectoMovil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]


    public partial class RegistrarUsuario : ContentPage
    {
        int genero = 0;
        public RegistrarUsuario()
        {
            InitializeComponent();

        }

        private async void btnTomarFoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Alerta", "Cámara no disponible", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                //Directory = "Sample",
                //Name = "test.jpg"
                SaveToAlbum = true
            });

            if (file == null)
                return;

            pathFoto.Text = file.AlbumPath;

            //convertir a arreglo de bytes
            byte[] fileByte = System.IO.File.ReadAllBytes(file.AlbumPath);

            //convertir a base64
            string pathBase64 = Convert.ToBase64String(fileByte);


            fotografia.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });
        }

        private async void btnBuscarFoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Alerta", "No se puede elegir una foto", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null)
                return;
            pathFoto.Text = file.Path;


            fotografia.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });
        }

        public void Borrar()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtDNI.Text = "";
            txtCelular.Text = "";
            txtCorreo.Text = "";
            txtPass.Text = "";
            txtDireccion.Text = "";
            //falta el de limpiar imagen


        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(pathFoto.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese una Foto ", "Ok");
            }
            else if (String.IsNullOrEmpty(txtNombre.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese un nombre ", "Ok");
            }
            else if (String.IsNullOrEmpty(txtApellido.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese un apellido ", "Ok");
            }
            else if (String.IsNullOrEmpty(txtDNI.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese un DNI ", "Ok");
            }
            else if (String.IsNullOrEmpty(txtCelular.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese un numero telefonico ", "Ok");
            }
            else if (String.IsNullOrEmpty(txtCorreo.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese un correo ", "Ok");
            }
            else if (String.IsNullOrEmpty(txtPass.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese una contraseña ", "Ok");
            }
            else if (String.IsNullOrEmpty(txtDireccion.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, Ingrese una direccion", "Ok");
            }
            else if (string.IsNullOrEmpty(label.Text))
            {
                await DisplayAlert("Campo Vacio", "Por favor, seleccione su genero", "Ok");
            }
            else
            {
                //Nos Preparamos para Guardar

                string imagen = pathFoto.Text;

                //convertir a arreglo de bytes
                byte[] fileByte = System.IO.File.ReadAllBytes(imagen);

                //convertir a base64
                string pathBase64 = Convert.ToBase64String(fileByte);

                RUsuario save = new RUsuario
                {
                    nombres = txtNombre.Text,
                    apellidos = txtApellido.Text,
                    correo = txtCorreo.Text,
                    direccion = txtDireccion.Text,
                    n_identidad = txtDNI.Text,
                    genero = Convert.ToInt32(label.Text),
                    password = txtPass.Text,
                    telefono = txtCelular.Text,
                    avatar = pathBase64,
                };

                Uri RequestUri = new Uri("https://appmovil2.herokuapp.com/Api/Cliente");

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(save);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(RequestUri, contentJson);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    String jsonx = response.Content.ReadAsStringAsync().Result;

                    JObject jsons = JObject.Parse(jsonx);


                    String Mensaje = jsons["Mensaje"].ToString();

                    await DisplayAlert("Success", "Datos guardados correctamente", "Ok");

                    Borrar();

                }
                else
                {
                    await DisplayAlert("Error", "Estamos en mantenimiento", "Ok");
                }
            }//cierre del else de guardar 


        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            label.Text = $"{radioButton.Value}";
        }
    }
}