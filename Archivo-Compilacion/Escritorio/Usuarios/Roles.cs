using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsDB.Usuarios
{
    public class Roles
    {
        [PrimaryKey, Identity]

        public int ID { get; set; }

        public string Role { get; set; }
    }

    public class RolesMode
    {
        public int ID { get; set; }

        public string Role { get; set; }
    }
}
