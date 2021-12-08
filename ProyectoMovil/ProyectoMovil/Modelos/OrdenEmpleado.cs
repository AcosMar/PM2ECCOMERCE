using System;
namespace ProyectoMovil.Modelos
{
    public class OrdenEmpleado
    {
        public OrdenEmpleado(String id, String cliente, String telefono, String productos, String empleado_orden, String subtotal, String isv, String total, String t_pago, String fecha_registro, String estado, String longitud, String latitud)
        {
            this.id = id;
            this.cliente = cliente;
            this.telefono = telefono;
            this.productos = productos;
            this.empleado_orden = empleado_orden;
            this.subtotal = subtotal;
            this.isv = isv;
            this.total = total;
            this.t_pago = t_pago;
            this.fecha_registro = fecha_registro;
            this.estado = estado;
            this.longitud = longitud;
            this.latitud = latitud;
        }

        public String id { get; set; }
        public String cliente { get; set; }
        public String telefono { get; set; }
        public String productos { get; set; }
        public String empleado_orden { get; set; }
        public String subtotal { get; set; }
        public String isv { get; set; }
        public String total { get; set; }
        public String t_pago { get; set; }
        public String fecha_registro { get; set; }
        public String estado { get; set; }
        public String longitud { get; set; }
        public String latitud { get; set; }
    }
}
