using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.cProducto
{
   public class ProductoModel
   {
      public int IdCompra { set; get; }
      public string Codigo { set; get; }
      public string Descripcion { set; get; }
      public int Cantidad { set; get; }
      public int IdUsuario { set; get; }
      public string Precio { set; get; }
      public string Importe { set; get; }
      public String Fecha { set; get; }
      public bool Credito { set; get; }
      public string Ticket { set; get; }
      public bool Inventariado { get; set; }
   }

   public class Productos
   {
      public int IdProducto { set; get; } //0
      public string Producto { set; get; } //1
      public int Categoria { set; get; } //2
      public String Fecha { set; get; } //3
      public int productosStockID { get; set; } //4
      public string Codigo { set; get; } //5
      public string Precio { set; get; } //6
      public string Descuento { set; get; } //7
        public byte[] Imagen { set; get; } //8
        public string Marca { set; get; } //9
      public string Modelo { set; get; } //10
      public string Serie { set; get; } //11
      public string Numero { set; get; } //12
      public string Size { set; get; } //13
      public string Peso { set; get; } //14
      public string Color { set; get; } //15
      public int UsuarioId { set; get; } //16
      public string Usuario { set; get; } //17
      public bool Pedido { get; set; } //18
      public bool Eliminar { get; set; } //19

   }
}
