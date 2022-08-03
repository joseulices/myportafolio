using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.cProducto
{
    public class Pedidos
    {
        [PrimaryKey, Identity]
        public int ProductosPedidosID { get; set; }
        public String Codigo { get; set; }
        public decimal Costo { get; set; }
        public decimal Valor { get; set; }
        public String Razon { get; set; }
        public int ProductoId { get; set; }
        public int ProductoStockId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Recibido { get; set; }
        public int Cantidad { get; set; }
    }

    public class PedidosMode
    {
        public int PedidosID { get; set; }
        public String Codigo { get; set; }
        public decimal Valor { get; set; }
        public decimal Costo { get; set; }
        public String Razon { get; set; }
        public int ProductoId { get; set; }
        public int ProductoStockId { get; set; }
        public int UsuarioId { get; set; }
        public String Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public bool Recibido { get; set; }
        public int Cantidad { get; set; }
        public bool Eliminar { get; set; }
    }

    public class PedidosReportes
    {
        public int ID { get; set; }
        public String Accion { get; set; }
        public String Usuario { get; set; }
        public String Descripcion { get; set; }
    }
}
