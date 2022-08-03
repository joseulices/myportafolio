using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.cProducto
{
    public class ProductosReportes
    {
        [PrimaryKey, Identity]
        public Int32 ProdutosReportesID { set; get; }
        public Int32 ProductosId { set; get; }
        public Int32 ProductoStockId { set; get; }
      public DateTime Fecha { set; get; }
        public String Usuario { set; get; }
        public String Accion { set; get; }
        public Int32 CompraId { set; get; }
        public Int32 Cantidad { set; get; }
    }
}
