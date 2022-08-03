using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.Ordenador
{
    public class Ordenadores
    {
        [PrimaryKey, Identity]

        public int ID { get; set; }
        public string Ordenador { get; set; }
        public bool IsActive { get; set; }
        public string Usuario { get; set; }
        public DateTime InFecha { get; set; }
        public DateTime OutFecha { get; set; }
        public int UsuarioId { get; set; }
    }
}
