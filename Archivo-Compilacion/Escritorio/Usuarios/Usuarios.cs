using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.Usuarios
{
    public class Usuarios
    {
        [PrimaryKey, Identity]
        public int ID { get; set; }
        public string NID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public int FuncionId { get; set; }
        public byte[] Imagen { get; set; }
        public bool IsActive { get; set; }
        public bool Estado { get; set; }
        public DateTime Fecha { get; set; }
    }
}
