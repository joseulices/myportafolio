using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.Usuarios
{
    public class Funciones
    {
        [PrimaryKey, Identity]

        public int FuncionesID { get; set; }

        public string Nombre { get; set; }
    }

    public class Funciones_Roles
    {
        [PrimaryKey, Identity]

        public int FuncionesRolesID { get; set; }

        public int FuncionesId { get; set; }
        public int RolId { get; set; }

    }
}
