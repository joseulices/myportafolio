using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.cProducto
{
   public class Categoria
    {
        [PrimaryKey, Identity]
        public int CategoriaID { set; get; }
        public string Nombre { set; get; }
        public bool Modelo { get; set; }
        public bool Serie { get; set; }
        public bool Numero { get; set; }
        public bool Size { get; set; }
        public bool Peso { get; set; }
        public bool Color { get; set; }
    }

    public class CategoriaModel
    {
        public int CategoriaID { set; get; }
        public string Nombre { set; get; }
        public bool Modelo { get; set; }
        public bool Serie { get; set; }
        public bool Numero { get; set; }
        public bool Size { get; set; }
        public bool Peso { get; set; }
        public bool Color { get; set; }
        public bool Eliminar { get; set; }
    }
}
