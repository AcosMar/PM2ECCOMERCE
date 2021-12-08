using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class ModificarRPersonaPage : ContentPage
    {
        public string ID;
        public string usuario;
        public string nombres;
        public string apellidos;
        public string email;
        public string direccion;
        public string telefono;
        public string avatares;
        public string user;

        public ModificarRPersonaPage()
        {
            InitializeComponent();
            ObtenerCredenciales();
            llamar_datos();
            
        }

        //llenar datos: mandamos el nombre del usuario y extraemos todos sus datos
        private async void llamar_datos()
        {
            var nombre = await SecureStorage.GetAsync("usuario");

            object Peticion = new
            {
                //se manda a llamar mediante el nombre del usuario 
                usuario = user
            };
            Uri RequestUri = new Uri("https://appmovil2.herokuapp.com/Api/ApiNewCliente");

            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(Peticion);
            var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(RequestUri, contentJson);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                String jsonx = response.Content.ReadAsStringAsync().Result;
                JObject jsons = JObject.Parse(jsonx);
                //Cuando se hace la peticion del usuaio con push despues mandamos a traer
                //los datos necesarios para mostrar en la vista de ver
                string ID = jsons["clientes"][0]["pk"].ToString();
                string nombres = jsons["clientes"][0]["nombres"].ToString();
                string apellidos = jsons["clientes"][0]["apellidos"].ToString();
                string avatar = jsons["clientes"][0]["avatar"].ToString();
                string usuarios = jsons["clientes"][0]["usuario"].ToString();
                string email = jsons["clientes"][0]["correo"].ToString();
                string direccion = jsons["clientes"][0]["direccion"].ToString();
                string telefono = jsons["clientes"][0]["telefono"].ToString();

                
                idSitio.Text = ID;
                txtnomb.Text = nombres;
                txtapellido.Text = apellidos;
                txtcorreo.Text = email;
                txtdireccion.Text = direccion;
                txttelefono.Text = telefono;
                BtnSingin.Text = usuarios;
                avatares = avatar;
            }
            else
            {
                await DisplayAlert("Error", "Estamos en mantenimiento", "Ok");
            }

        }

        private async void ObtenerCredenciales()
        {
            user = await SecureStorage.GetAsync("usuario");
        }


        //boton de ingresar o modificar datos, con un DisplayActionSheet le damos las
        //opciones ya sea salir y modificar dependiendo de la opcion hace su respectiva accion
        private async  void BtnSingin_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Que deseas hacer?", "Cancel", null, "Modificar tus datos", "Cerrar Sesion");

            if (action == "Modificar")
            {
                string mod = await DisplayActionSheet("Usted quiere modificar sus datos?", "Yes", "No", null, "", "");

                if (mod == "Yes")
                {
                    var a = new ModificarRPersonaPage();
                    await Navigation.PushAsync(a);
                }

            }

            else if (action == "Cerrar Sesion")
            {
                string cierre = await DisplayActionSheet("Usted quiere Cerrar Sesion?", "Yes", "No", null, "", "");

                if (cierre == "Yes")
                {
                   
                    await Navigation.PushAsync(new MainPage());
                }
            }
        }

        public bool ValidarDatos()
        {
            bool respuesta;

            if (String.IsNullOrEmpty(txtnomb.Text))
            {
                respuesta = false;
            }

            else if (String.IsNullOrEmpty(txtapellido.Text))
            {
                respuesta = false;
            }

            
            else if (String.IsNullOrEmpty(txtcorreo.Text))
            {
                respuesta = false;
            }
            else if (String.IsNullOrEmpty(txtdireccion.Text))
            {
                respuesta = false;
            }
            else
            {
                respuesta = true;
            }
            return respuesta;
        }

        //Modificar datos: mandamos todos los datos necesarios para modificar filtrados 
        private async void BtnModificarDatos_Clicked(object sender, EventArgs e)
        {
            ID = idSitio.Text;
            usuario = BtnSingin.Text;
            email = txtcorreo.Text;
            direccion = txtdireccion.Text;
            telefono = txttelefono.Text;
            await Navigation.PushAsync(new ModificarUsuario(ID, usuario, email, direccion, telefono));

        }
        
        //Modificar Contraseñas: mandamos todos los datos necesarion para modificar los datos 
        private async void BtnCambiarContra_Clicked(object sender, EventArgs e)
        {
            ID = idSitio.Text;
            nombres = txtnomb.Text;
            apellidos = txtapellido.Text;
           
            await Navigation.PushAsync(new ModificarContra(ID, nombres,apellidos));
        }
    }
}