using System;
namespace ProyectoMovil.Modelos
{
    public class Orden
    {
        public Orden(String id, String cliente, String productos, String empleado_orden, String subtotal, String isv, String total, String t_pago, String fecha_registro, String estado)
        {
            this.id = id;
            this.cliente = cliente;
            this.productos = productos;
            this.empleado_orden = empleado_orden;
            this.subtotal = subtotal;
            this.isv = isv;
            this.total = total;
            this.t_pago = t_pago;
            this.fecha_registro = fecha_registro;
            this.estado = estado;
        }

        public String id { get; set; }
        public String cliente { get; set; }
        public String productos { get; set; }
        public String empleado_orden { get; set; }
        public String subtotal { get; set; }
        public String isv { get; set; }
        public String total { get; set; }
        public String t_pago { get; set; }
        public String fecha_registro { get; set; }
        public String estado { get; set; }
    }
}
