using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.cProducto
{
    public class BodegaTemp
    {
        [PrimaryKey, Identity]
        public int Id { set; get; }
        public int IdProducto { set; get; }
        public int Existencia { set; get; }
        public DateTime Fecha { set; get; }
    }
}
