using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ProyectoMovil.Modelos
{
    class Producto
    {
        public Producto(string IdProducto, string nombre_producto, string descripcion,
                         string fecha_expira, string precio, string cantidad, string tipo_producto,
                         string fecha_registro, ImageSource imagen, string img64)
        {
            this.IdProducto = IdProducto;
            this.nombre_producto = nombre_producto;
            this.descripcion = descripcion;
            this.fecha_expira = fecha_expira;
            this.precio = precio;
            this.cantidad = cantidad;
            this.tipo_producto = tipo_producto;
            this.fecha_registro = fecha_registro;
            this.imagen = imagen;
            this.img64 = img64;
        }

        public string IdProducto { get; set; }
        public string nombre_producto { get; set; }
        public string descripcion { get; set; }
        public string fecha_expira { get; set; }
        public string precio { get; set; }
        public string cantidad { get; set; }
        public string tipo_producto { get; set; }
        public string fecha_registro { get; set; }
        public string img64 { get; set; }
        public ImageSource imagen { get; set; }
    }
}
