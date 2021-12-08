using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using ProyectoMovil.Modelos;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoMovil
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModificarUsuario : ContentPage
    {

        public string imagenglobal = null;

        public ModificarUsuario(string ID, string usuario, string email, string direccion, string telefono)
        {
            //inicializamos las variables que extraemos y volvemos hacer uso de llenar para cargar la imagen
            InitializeComponent();
            idSitio.Text = ID;
            txtusuario.Text = usuario;
            txtcorreo.Text = email;
            txtdireccion.Text = direccion;
            txttelefono.Text = telefono;
            llamar_datos();

        }

        private async void llamar_datos()
        {
            var nombre = await SecureStorage.GetAsync("usuario");
            object Peticion = new
            {
                usuario = txtusuario.Text
            };

            Uri RequestUri = new Uri("https://appmovil2.herokuapp.com/Api/ApiNewCliente");

            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(Peticion);
            var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(RequestUri, contentJson);
            byte[] newBytes = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {

                String jsonx = response.Content.ReadAsStringAsync().Result;
                JObject jsons = JObject.Parse(jsonx);
                string avatar = jsons["clientes"][0]["avatar"].ToString();

                //Convertimos la imagen a base64 y la mostramos en la etiqueta <Image Source="" />
                newBytes = Convert.FromBase64String(avatar);
                var stream = new MemoryStream(newBytes);
                fotografia.Source = ImageSource.FromStream(() => stream);
                imagenglobal = avatar;



            }
            else
            {
                await DisplayAlert("Error", "Estamos en mantenimiento", "Ok");
            }

        }

        //Modificamos,hacemos una validacion de path.text viene vacio que siga almacenando la imagen que muestra
        //de caso contrario pues que almacene la nueva imagen convirtiendola en un texto
        private async void BtnModificar_Clicked(object sender, EventArgs e)
        {
            string pathBase64 = null;
            if (ValidarDatos())
            {
                //Validacion de si path viene vacio
                if (string.IsNullOrEmpty(pathFoto.Text))
                {
                    pathBase64 = imagenglobal;
                }

                else
                {
                    //trae la ruta de la imagen donde esta ubicada en el celular
                    string imagen = pathFoto.Text;
                    //convertir a arreglo de bytes
                    byte[] fileByte = System.IO.File.ReadAllBytes(imagen);
                    //convertir a base64
                    pathBase64 = Convert.ToBase64String(fileByte);

                }




                RPersona Modificar = new RPersona
                {
                    ID = Convert.ToInt32(this.idSitio.Text),
                    username = txtusuario.Text,
                    direccion = txtdireccion.Text,
                    correo = txtcorreo.Text,
                    telefono = txttelefono.Text,
                    avatar = pathBase64
                };


                Uri RequestUri = new Uri("https://appmovil2.herokuapp.com/Api/Cliente");

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(Modificar);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(RequestUri, contentJson);



                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await DisplayAlert("Success", "Datos modificados correctamente", "Ok");
                }
                else
                {
                    await DisplayAlert("Error", "Estamos en mantenimiento", "Ok");
                }

            }
            else
            {
                await DisplayAlert("Advertencia", "Ingresar todos los datos", "ok");
            }
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



        public bool ValidarDatos()
        {
            bool respuesta;

            if (String.IsNullOrEmpty(txtusuario.Text))
            {
                respuesta = false;
            }

            else if (String.IsNullOrEmpty(txttelefono.Text))
            {
                respuesta = false;
            }

            else if (String.IsNullOrEmpty(txtdireccion.Text))
            {
                respuesta = false;
            }
            else if (String.IsNullOrEmpty(txtcorreo.Text))
            {
                respuesta = false;
            }
            else if (String.IsNullOrEmpty(idSitio.Text))
            {
                respuesta = false;
            }
            else
            {
                respuesta = true;
            }
            return respuesta;
        }




    }
}