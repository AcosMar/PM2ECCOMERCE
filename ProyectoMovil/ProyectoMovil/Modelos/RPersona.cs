using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoMovil.Modelos
{
    public class RPersona
    {
        public int ID { get; set; }

        public string username { get; set; }

        public string direccion { get; set; }

        public string correo { get; set; }

        public string telefono { get; set; }

        public string avatar { get; set; }


        [Xamarin.Forms.TypeConverter(typeof(Xamarin.Forms.ImageSourceConverter))]
        public Xamarin.Forms.ImageSource Source { get; set; }
    }
}
