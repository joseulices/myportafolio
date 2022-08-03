using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.cProducto
{
   public class Productos_Stock
   {
      [PrimaryKey, Identity]
      public int ID { set; get; }     //0
      public int productoId { get; set; }  //1
      public string Codigo { set; get; }   //2
      public decimal Precio { set; get; } //3
      public decimal Descuento { set; get; } //4
      public byte[] Imagen { set; get; } //5
      public string Marca { set; get; } //6
      public string Modelo { set; get; } //7
      public string Serie { set; get; } //8
      public string Numero { set; get; } //9
      public string Size { set; get; } //10
      public string Peso { set; get; } //11
      public string Color { set; get; } //12
      public bool Activo { set; get; } //13
      public int UsuarioId { set; get; } //14
   }
}
