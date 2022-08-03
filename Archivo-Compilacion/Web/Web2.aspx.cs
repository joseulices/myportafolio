using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Web
{
    public partial class Web2 : System.Web.UI.Page
    {
        Datos.Datos datos = new Datos.Datos();
        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.DataSource = datos.LlenarGV();
            GridView1.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            datos.SelectCategoria();
        }
    }
}