using System;
using Xamarin.Forms;

namespace ProyectoMovil.Modelos
{
    public class Carrito
    {
        public Carrito(String id, String nombre_producto, String cantidad, String precio, String tipo_producto, ImageSource imagen, string img64, String total)
        {
            this.id = id;
            this.nombre_producto = nombre_producto;
            this.cantidad = cantidad;
            this.precio = precio;
            this.tipo_producto = tipo_producto;
            this.imagen = imagen;
            this.img64 = img64;
            this.total = total;
        }

        public String id { get; set; }
        public String nombre_producto { get; set; }
        public String cantidad { get; set; }
        public String precio { get; set; }
        public String tipo_producto { get; set; }
        public string img64 { get; set; }
        public ImageSource imagen { get; set; }
        public string total { get; set; }
    }
}
