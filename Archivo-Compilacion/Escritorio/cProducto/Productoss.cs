using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.cProducto
{
    public class Productoss
    {
        [PrimaryKey, Identity]
        public int IdProducto { set; get; }     //0
        //public string Codigo { set; get; }      //1
        public string Producto { set; get; }    //2
        //public Decimal Precio { set; get; }     //3
        //public Decimal Descuento { set; get; }  //4
        public int Categoria { set; get; }   //5
        public DateTime Fecha { set; get; }     //6
        //public byte[] Imagen { set; get; }      //7
        //public string Marca { set; get; }       //8
        //public string Modelo { set; get; }      //9
        //public string Serie { set; get; }       //10
        //public string Numero { set; get; }      //11
        //public string Size { set; get; }        //12
        //public string Peso { set; get; }        //13
        //public int UsuarioId { get; set; }      //14
        //public string Color { set; get; }        //15

    }
}
