using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Security;

namespace Datos
{
    public class Datos
    {

        private SqlConnection dbdbconexion = new SqlConnection(ConfigurationManager.ConnectionStrings["LLL2"].ConnectionString);
        public string msj = "";

        public bool Conectar()
        {

            try
            {
                if (dbdbconexion.State == ConnectionState.Closed)
                {
                    dbdbconexion.Open();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                msj = "Un error durante el proceso de conexión. Contácte con soporte técnico.";
                dbdbconexion.Close();
                return false;
            }


        }

        public DataTable SelectCategoria()
        {
            //string queryString = "select CeId, CeNombre From dbo.categoria_examen;";
            
            byte[] data = new byte[4 * 14];

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            var salHashReturn = PasswordHash.Encrypt.SHA512("1234");

            using (var connection = (dbdbconexion))
            {
                var command = new SqlCommand("CategoriasGET", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                Conectar();
                da = new SqlDataAdapter(command);
                da.Fill(dt);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}, {1}, {2}", reader["CeId"], reader["CeNombre"], salHashReturn));
                    }
                }
            }

            return dt;
        }

        public DataTable LlenarGV()
        {
            DataTable GV = new DataTable();
            SqlDataAdapter da;
            GV.Columns.AddRange(new DataColumn[] {
                new DataColumn("CeId",typeof(string)),
                new DataColumn("CeNombre",typeof(string)),
            });

            using (var connection = (dbdbconexion))
            {
                var command = new SqlCommand("CategoriasGET", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                Conectar();
                da = new SqlDataAdapter(command);
                da.Fill(GV);

            }

            return GV;
        }

    }


}
