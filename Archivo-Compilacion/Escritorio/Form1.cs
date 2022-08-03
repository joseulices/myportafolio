using ModelsDB;
using ModelsDB.cCaja;
using ModelsDB.Cliente;
using ModelsDB.Usuarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewModels;
using ViewModels.Libreria;

namespace SistemaVentas
{
    public partial class Form1 : Form
    {
        private VMLogin login;
        private List<RadioButton> _radioConfig;
        private List<Roles> _roles;
        private VMUsuarios usuario;
        private static Usuarios _dataUsuario;
        private static Cajas gCajas;
        private String _tabpageInicio = "";

        public Form1(Usuarios dataUsuario, Cajas cajaData)
        {
            InitializeComponent();

            _dataUsuario = dataUsuario;

            //Codigo de clientes
            clientes = new VMClientes(dataUsuario);

            //Codigo de usuario
            login = new VMLogin();

            object[] perfil =
            {
                lbNombrePerfil,                     //0
                pbPerfil,                           //1
                Properties.Resources.defaultImg1,   //2
                lbCajaNCaja,                        //3
                _roles                              //4
            };

            gCajas = cajaData;
            usuario = new VMUsuarios(dataUsuario, perfil, cajaData);

            ventanasDeshabilitar();

            //CODIGO DE CONFIGURACION
            _radioConfig = new List<RadioButton>
            {
                rbConfLempira,
                rbConfDolar,
                rbConfigIntereses
            };

            config = new VMConfiguracion(_radioConfig);

            //Codigo de proveedor
            proveedor = new VMProveedor(dataUsuario);

            //Codigio de compras
            compras = new VMCompras(dataUsuario);

            //Codigo de productos
            productos = new VMProductos(dataUsuario);

            //CODIGO Inventario
            formInventario = new FromInventario(dataUsuario);

            //Codigo de Cajas
            caja = new VMCajas(dataUsuario);

            //CODIGO UBICACION
            formUbicaciones = new FormUbicaciones(dataUsuario);

            //CODIGO DE VENTAS 
            venta = new VMVentas(dataUsuario, cajaData);

            timer1.Start();

            rolesArreglo();

            switch (_tabpageInicio)
            {
                case "Ventas":
                    SeccionVentas();
                    break;
                case "Productos":
                    SeccionProductos();
                    break;
            }
            //usuario._roles.ForEach(item =>
            //{
            //    tvUsuariosRoles.BeginUpdate();
            //    tvUsuariosRoles.Nodes.Add(item.Role);
            //    tvUsuariosRoles.EndUpdate();
            //});

            //tvUsuariosRoles.Nodes.RemoveAt(0);
            //tvUsuariosRoles.Nodes[0].Nodes.Add("Hijo1");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TabPage tab = tabControlPrincipal.SelectedTab;

            if (tab.Equals(tabPageVentas))
            {
                venta.ingresosCajas();

                clientes.GetReportesDeuda("");
            }

        }

        private bool value = true;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (value)
            {
                if (MessageBox.Show("Estas seguro de salir del sistema ?", "Cerrar sesión",
               MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    value = false;
                    login.Close();
                }
                else
                {
                    value = false;
                    Application.Exit();
                }
            }
        }

        private void EnabledButton(Button button)
        {
            btnVentas.Enabled = true;
            btnCliente.Enabled = true;
            btnProveedor.Enabled = true;
            btnCompras.Enabled = true;
            btnProductos.Enabled = true;
            btnInventario.Enabled = true;
            btnCaja.Enabled = true;
            btnUsuario.Enabled = true;
            btnConfiguracion.Enabled = true;
            btnBodega.Enabled = true;
            button.Enabled = false;
        }

        private void tabControlPrincipal_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlPrincipal.SelectedTab.Name)
            {
                case "tabPageVentas":
                    if (gCajas.IdCaja.Equals(0))
                    {
                        MessageBox.Show("No tiene asignada caja");
                        tabControlPrincipal.TabIndex = 1;
                    }
                    else
                    {
                        SeccionVentas();
                    }
                    break;
                case "tabPageClientes":
                    SeccionCliente();
                    break;
                case "tabPageProveedores":
                    SeccionProveedor();
                    break;
                case "tabPageProductos":
                    SeccionProductos();
                    break;
                case "tabPageCompras":
                    SeccionCompras();
                    break;

                case "tabPageInventario":
                    SeccionInventario();
                    break;
                case "tabPageCajas":
                    SeccionCajas();
                    break;
                case "tabPageUsuarios":
                    SeccionUsuarios();
                    break;
                case "tabPageBodega":
                    SeccionBodegas();
                    break;
                case "tabPageConfiguracion":
                    SeccionConfiguracion();
                    break;
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            switch (tabControlPrincipal.SelectedTab.Name)
            {
                case "tabPageProductos":
                    ClsVMObjetos.uploadimage.printDocument(e, groupBoxProductoCodio, productos.codigos.Codigo);
                    break;
            }
        }

        private void rolesArreglo()
        {
            List<tabPageModel> listTabPages = new List<tabPageModel>();
            var list = listTabPages;

            list.Add(new tabPageModel
            {
                tabPageVenta = false,
                tabPageCliente = false,
                tabPageProveedor = false,
                tabPageCompras = false,
                tabPageProductos = false,
                tabPageInventarios = false,
                tabPageCajas = false,
                tabPageUsuarios = false,
                tabPageBodegas = false,
                tabPageConfiguracion = false
            });

            var usuarios = new VMUsuarios(_dataUsuario);

            var rol = usuarios.getRole();

            rol.ForEach(item =>
            {
                switch (item.Role)
                {
                    case "Admin":
                        list[0].tabPageVenta = true;
                        list[0].tabPageCliente = true;
                        list[0].tabPageProveedor = true;
                        list[0].tabPageCompras = true;
                        list[0].tabPageProductos = true;
                        list[0].tabPageInventarios = true;
                        list[0].tabPageCajas = true;
                        list[0].tabPageUsuarios = true;
                        list[0].tabPageBodegas = true;
                        list[0].tabPageConfiguracion = true;
                        break;
                    case "User":
                        break;
                    case "Bodega":
                        list[0].tabPageProductos = true;
                        list[0].tabPageInventarios = true;
                        list[0].tabPageUsuarios = true;
                        list[0].tabPageBodegas = true;
                        break;
                    case "Caja":
                        list[0].tabPageVenta = true;
                        break;
                }
            });

            list.ForEach(item1 =>
            {
                int i = 0;

                if (item1.tabPageVenta)
                {
                    ventanasEstado("Ventas");
                    ArregloLocation(i++, btnVentas);
                    if (i == 1)
                    {
                        _tabpageInicio = "Ventas";
                    }
                }
                if (item1.tabPageCliente)
                {
                    ventanasEstado("Clientes");
                    ArregloLocation(i++, btnCliente);
                    if (i == 1)
                    {
                        _tabpageInicio = "Clientes";
                    }
                }
                if (item1.tabPageProveedor)
                {
                    ventanasEstado("Proveedores");
                    ArregloLocation(i++, btnProveedor);
                    if (i == 1)
                    {
                        _tabpageInicio = "Proveedores";
                    }
                }
                if (item1.tabPageCompras)
                {
                    ventanasEstado("Compras");
                    ArregloLocation(i++, btnCompras);
                    if (i == 1)
                    {
                        _tabpageInicio = "Compras";
                    }
                }
                if (item1.tabPageProductos)
                {
                    ventanasEstado("Productos");
                    ArregloLocation(i++, btnProductos);
                    if (i == 1)
                    {
                        _tabpageInicio = "Productos";
                    }
                }
                if (item1.tabPageInventarios)
                {
                    ventanasEstado("Inventario");
                    ArregloLocation(i++, btnInventario);
                    if (i == 1)
                    {
                        _tabpageInicio = "Inventario";
                    }
                }
                if (item1.tabPageCajas)
                {
                    ventanasEstado("Cajas");
                    ArregloLocation(i++, btnCaja);
                    if (i == 1)
                    {
                        _tabpageInicio = "Cajas";
                    }
                }
                if (item1.tabPageUsuarios)
                {
                    ventanasEstado("Usuarios");
                    ArregloLocation(i++, btnUsuario);
                    if (i == 1)
                    {
                        _tabpageInicio = "Usuarios";
                    }
                }
                if (item1.tabPageBodegas)
                {
                    ventanasEstado("Bodega");
                    ArregloLocation(i++, btnBodega);
                    if (i == 1)
                    {
                        _tabpageInicio = "Bodega";
                    }
                }
                if (item1.tabPageConfiguracion)
                {
                    ventanasEstado("Configuracion");
                    ArregloLocation(i++, btnConfiguracion);
                    if (i == 1)
                    {
                        _tabpageInicio = "Configuracion";
                    }
                }
            });

            void ArregloLocation(int opcion, Button control)
            {

                switch (opcion)
                {
                    case 0: //Ventas
                        control.Location = new Point(4, 21);
                        break;
                    case 1: //Cliente
                        control.Location = new Point(69, 21);
                        break;
                    case 2: //Proveedor
                        control.Location = new Point(134, 21);
                        break;
                    case 3://Compras
                        control.Location = new Point(199, 21);
                        break;
                    case 4: //Productos
                        control.Location = new Point(264, 21);
                        break;
                    case 5: //Inventario
                        control.Location = new Point(329, 21);
                        break;
                    case 6: //Caja
                        control.Location = new Point(394, 21);
                        break;
                    case 7: //Usuario
                        control.Location = new Point(460, 21);
                        break;
                    case 8: //Bodega
                        control.Location = new Point(525, 21);
                        break;
                    case 9: //Configuracion
                        control.Location = new Point(590, 21);
                        break;
                }
            }

        }

        private void ventanasEstado(String ctr)
        {
            //Deshabilitar y quitar
            switch (ctr)
            {
                case "Ventas":
                    if (!tabControlPrincipal.TabPages.Contains(tabPageVentas))
                    {
                        btnVentas.Enabled = true;
                        btnVentas.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageVentas);
                    }
                    break;
                case "Clientes":

                    if (!tabControlPrincipal.TabPages.Contains(tabPageClientes))
                    {
                        btnCliente.Enabled = true;
                        btnCliente.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageClientes);
                    }
                    break;
                case "Proveedores":
                    if (!tabControlPrincipal.TabPages.Contains(tabPageProveedores))
                    {
                        btnProveedor.Enabled = true;
                        btnProveedor.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageProveedores);
                    }
                    break;
                case "Compras":
                    if (!tabControlPrincipal.TabPages.Contains(tabPageCompras))
                    {
                        btnCompras.Enabled = true;
                        btnCompras.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageCompras);
                    }
                    break;
                case "Productos":
                    if (!tabControlPrincipal.TabPages.Contains(tabPageProductos))
                    {
                        btnProductos.Enabled = true;
                        btnProductos.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageProductos);
                    }
                    break;
                case "Inventario":
                    if (!tabControlPrincipal.TabPages.Contains(tabPageInventario))
                    {
                        btnInventario.Enabled = true;
                        btnInventario.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageInventario);
                    }
                    break;
                case "Cajas":
                    if (!tabControlPrincipal.TabPages.Contains(tabPageCajas))
                    {
                        btnCaja.Enabled = true;
                        btnCaja.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageCajas);
                    }
                    break;
                case "Usuarios":
                    if (!tabControlPrincipal.TabPages.Contains(tabPageUsuarios))
                    {
                        btnUsuario.Enabled = true;
                        btnUsuario.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageUsuarios);
                    }
                    break;
                case "Bodega":
                    if (!tabControlPrincipal.TabPages.Contains(tabPageBodega))
                    {
                        btnBodega.Enabled = true;
                        btnBodega.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageBodega);
                    }
                    break;
                case "Configuracion":
                    if (!tabControlPrincipal.TabPages.Contains(tabPageConfiguracion))
                    {
                        btnConfiguracion.Enabled = true;
                        btnConfiguracion.Visible = true;
                        tabControlPrincipal.TabPages.Add(tabPageConfiguracion);
                    }
                    break;
            }
        }

        private void ventanasDeshabilitar()
        {
            btnVentas.Enabled = false;
            btnCliente.Enabled = false;
            btnProveedor.Enabled = false;
            btnCompras.Enabled = false;
            btnProductos.Enabled = false;
            btnInventario.Enabled = false;
            btnCaja.Enabled = false;
            btnUsuario.Enabled = false;
            btnConfiguracion.Enabled = false;
            btnBodega.Enabled = false;

            btnVentas.Visible = false;
            btnCliente.Visible = false;
            btnProveedor.Visible = false;
            btnCompras.Visible = false;
            btnProductos.Visible = false;
            btnInventario.Visible = false;
            btnCaja.Visible = false;
            btnUsuario.Visible = false;
            btnConfiguracion.Visible = false;
            btnBodega.Visible = false;

            tabControlPrincipal.Controls.Remove(tabPageVentas);
            tabControlPrincipal.Controls.Remove(tabPageClientes);
            tabControlPrincipal.Controls.Remove(tabPageProveedores);
            tabControlPrincipal.Controls.Remove(tabPageCompras);
            tabControlPrincipal.Controls.Remove(tabPageProductos);
            tabControlPrincipal.Controls.Remove(tabPageInventario);
            tabControlPrincipal.Controls.Remove(tabPageCajas);
            tabControlPrincipal.Controls.Remove(tabPageUsuarios);
            tabControlPrincipal.Controls.Remove(tabPageBodega);
            tabControlPrincipal.Controls.Remove(tabPageConfiguracion);
        }
        /**************************************
        *                                    *
        *       CODIGO DEL CLIENTE           * 
        *                                    *
        **************************************/
        #region Cliente

        private VMClientes clientes;

        private void SeccionCliente()
        {
            var textBoxCliente = new List<TextBox> {
            tbClienteNID, //0
            tbClienteRTN, //1
            tbClienteNombre, //2
            tbClienteApellido, //3
            tbClienteEmail, //4
            tbClienteTelMovil, //5
            tbClienteTelFijo, //6
            tbClienteDireccion, //7

            tbClientePagoIngPago, //8
            tbClientePagoCuotas //9
            };


            var labelCliente = new List<Label> {
            lbClienteNID, //0
            lbClienteRTN, //1
            lbClienteNombre, //2
            lbClienteApellido, //3
            lbClienteSexo, //4
            lbClienteEmail, //5
            lbClienteTelMovil, //6
            lbClienteTelFijo, //7
            lbClienteDireccion, //8
            lbClientePaginas //9
        };

            var labelReport = new List<Label> {
            lbClientePagoNombre, //0
            lbClientePagoDeuda, //1 
            lbClientePago, //2
            lbClientePagoTicket, //3
            lbClientePagoFecha, //4
            lbClientePagoCuotasMes, //5

            lbClientePagoIntereses, //6
            lbClientePagoCuotas, //7
            lbClientePagoTicketIntereses, //8
            lbClientePagoFechaInterese, //9
            lbClientePagoIngPago, //10
            lbClientePagoTotalPagar, //11
            lbClienteReportesNombre, //12
            lbClienteReportDias, //13

            lbClientesPagos2Deuda, //14
            lbClientesPagos2Saldo, //15
            lbClientesPagos2FechaDeuda, //16
            lbClientesPagos2Ticket, //17
            lbClientesPagos2DeudaPago,  //18
            lbClientesPagos2CuotasXmes, //19
            lbClientesPagos2PagosFecha, //20
            lbClientesPagos2FechaLimite, //21
            lbClientesPagos2Usuario, //22

            lbClientesReportes2Intereses, //23
            lbClientesReportes2PagoIntereses, //24
            lbClientesReportes2TicketIntereses, //25
            lbClientesReportes2CambioIntereses, //26
            lbClientesReportes2CuotasIntereses, //27
            lbClientesReportes2FechaIntereses, //28
            lbClientesReportes2Usuario //29
            };


            object[] objetos = {
                pbCliente, //0
                chbClienteCredito, //1
                Properties.Resources.defaultImg1, //2
                dgClientes, //3
                nudClienteNoRegistros, //4
                dgClientePagos, //5
                cbClienteSexo, //6
                rbClientePagoCuotas, //7
                rbClientePagoIntereses, //8
                chbClienteReporteExtenderDia, //9
                dtpClienteReporte, //10
                dgClienteReporte, //11
                dgClientesPagosCuotas, //12
                dtpClientesPagos2FI, //13
                dtpClientesPagos2FF, //14
                dgClientesPagos2PagIntereses //15
            };

            clientes = new VMClientes(objetos, textBoxCliente, labelCliente, labelReport);
            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageClientes);
            rbClientePagoCuotas.Checked = true;
            tbClientePagoCuotas.Enabled = false;

            EnabledButton(btnCliente);

        }

        private void btnCliente_Click(object sender, EventArgs e)
        {
            SeccionCliente();
        }

        private void pbCliente_Click(object sender, EventArgs e)
        {
            ClsVMObjetos.uploadimage.CargarImagen(pbCliente);
        }

        private void tbClienteNID_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbClienteNID, "NID", lbClienteNID);
        }

        private void tbClienteNID_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void tbClienteRTN_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbClienteRTN, "RTN", lbClienteRTN);
        }

        private void tbClienteRTN_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void tbClienteNombre_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbClienteNombre, "Nombre", lbClienteNombre);
        }

        private void tbClienteNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.textKeyPress(e);
        }

        private void tbClienteApellido_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbClienteApellido, "Apellido", lbClienteApellido);
        }

        private void tbClienteApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.textKeyPress(e);
        }

        private void cbClienteSexo_TextChanged(object sender, EventArgs e)
        {
            if (cbClienteSexo.SelectedIndex.Equals(-1))
            {
                lbClienteSexo.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbClienteSexo.Text = "Sexo";
                lbClienteSexo.ForeColor = Color.Black;
            }
        }

        private void cbClienteSexo_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.textKeyPress(e);
        }

        private void tbClienteEmail_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbClienteEmail, "e-mail", lbClienteEmail);
        }

        private void tbClienteTelMovil_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbClienteTelMovil, "Teléfono móvil", lbClienteTelMovil);
        }

        private void tbClienteTelMovil_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void tbClienteTelFijo_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbClienteTelFijo, "Teléfono fijo", lbClienteTelFijo);
        }

        private void tbClienteTelFijo_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void tbClienteDireccion_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbClienteDireccion, "Direccion", lbClienteDireccion);
        }

        private void btnClienteSave_Click(object sender, EventArgs e)
        {
            switch (tabControlClienteInformacion.SelectedIndex)
            {
                case 0:
                    clientes.guardarCliente();
                    break;
                case 1:
                    clientes.EjecutarPago();
                    break;
                case 2:
                    var rol = usuario.getRole();
                    var autorizacion = rol.Where(u => u.Role.Equals("Admin")).ToList();

                    if (autorizacion.Count > 0)
                    {
                        clientes.ExtenderDias();
                    }
                    else
                    {
                        MessageBox.Show("No cuenta con el permiso requerido para ejecutar esta acción");
                    }

                    break;
            }
        }

        private void btnClienteReset_Click(object sender, EventArgs e)
        {
            clientes.restablecer();
        }

        private void dgClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgClientes.Rows.Count != 0)
                clientes.GetCliente();
        }

        private void dgClientes_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgClientes.Rows.Count != 0)
                clientes.GetCliente();
        }

        private void btnClientePagPrimero_Click(object sender, EventArgs e)
        {
            clientes.Paginador("Primero");
        }

        private void btnClientePagAnterior_Click(object sender, EventArgs e)
        {
            clientes.Paginador("Anterior");

        }

        private void btnClientePagSiguiente_Click(object sender, EventArgs e)
        {
            clientes.Paginador("Siguiente");

        }

        private void btnClientePagUltima_Click(object sender, EventArgs e)
        {
            clientes.Paginador("Ultimo");

        }

        private void nudClienteNoRegistros_ValueChanged(object sender, EventArgs e)
        {
            clientes.Registro_Paginas();
        }

        private void tbClienteBuscar_TextChanged(object sender, EventArgs e)
        {

            switch (tabControlClienteInformacion.SelectedIndex)
            {
                case 0:
                    clientes.SearchClientes(tbClienteBuscar.Text);
                    break;
                case 1:
                    clientes.GetReportes(tbClienteBuscar.Text);
                    break;
                case 2:
                    clientes.GetReportesDeuda(tbClienteBuscar.Text);
                    break;
            }

        }

        private void tabControlClienteInformacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlClienteInformacion.SelectedIndex)
            {
                case 0:
                    tabControlClienteReportes.SelectedIndex = 0;
                    clientes._seccion = 1;
                    break;
                case 1:
                    tabControlClienteReportes.SelectedIndex = 1;
                    clientes._seccion = 2;
                    break;
                case 2:
                    tabControlClienteReportes.SelectedIndex = 2;
                    clientes._seccion = 3;
                    break;
            }
            clientes.Registro_Paginas();
        }

        private void tabControlClienteReportes_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlClienteReportes.SelectedIndex)
            {
                case 0:
                    tabControlClienteInformacion.SelectedIndex = 0;
                    clientes._seccion = 1;
                    break;
                case 1:
                    tabControlClienteInformacion.SelectedIndex = 1;
                    clientes._seccion = 2;
                    break;
                case 2:
                    tabControlClienteInformacion.SelectedIndex = 2;
                    clientes._seccion = 3;
                    break;
            }
            clientes.Registro_Paginas();
        }

        private void dgClientePagos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgClientePagos.Rows.Count != 0)
            {
                clientes.GetReportCliente();
                clientes.CuotasIntereses();
            }
        }

        private void dgClientePagos_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgClientePagos.Rows.Count != 0)
            {
                clientes.GetReportCliente();
                clientes.CuotasIntereses();
            }
        }

        private void tbClientePagoIngPago_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberDecimalKeyPress(tbClientePagoIngPago, e);
        }

        private void tbClientePagoIngPago_TextChanged(object sender, EventArgs e)
        {
            clientes.CuotasIntereses();
        }

        private void tbClientePagoCuotas_TextChanged(object sender, EventArgs e)
        {
            clientes.CuotasIntereses();
        }

        private void tbClientePagoCuotas_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void rbClientePagoCuotas_CheckedChanged(object sender, EventArgs e)
        {
            tbClientePagoCuotas.Enabled = false;
            if (clientes != null)
            {
                clientes.CuotasIntereses();
            }
        }

        private void rbClientePagoIntereses_CheckedChanged(object sender, EventArgs e)
        {
            tbClientePagoCuotas.Enabled = true;
            if (clientes != null)
            {
                clientes.CuotasIntereses();
            }
        }

        private void dgClienteReporte_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgClienteReporte.Rows.Count != 0)
            {
                var selected = (ClienteModel)dgClienteReporte.CurrentRow.DataBoundItem;
                clientes.GetReporteDeuda(selected);
            }
        }

        private void dgClienteReporte_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgClienteReporte.Rows.Count != 0)
            {
                var selected = (ClienteModel)dgClienteReporte.CurrentRow.DataBoundItem;
                clientes.GetReporteDeuda(selected);
            }
        }

        private void tabControlClientesPagos_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlClientesPagos.SelectedIndex)
            {
                case 0:
                    rbClientePagoCuotas.Checked = true;
                    rbClientePagoIntereses.Checked = false;
                    tabControlClientesPagos2.SelectedIndex = 1;
                    break;
                case 1:
                    rbClientePagoCuotas.Checked = false;
                    rbClientePagoIntereses.Checked = true;
                    tabControlClientesPagos2.SelectedIndex = 2;
                    break;
            }
        }

        private void tabControlClientesPagos2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlClientesPagos2.SelectedIndex)
            {
                case 1:
                    tabControlClientesPagos.SelectedIndex = 0;
                    break;
                case 2:
                    tabControlClientesPagos.SelectedIndex = 1;
                    break;
            }

            clientes._seccion1 = tabControlClientesPagos2.SelectedIndex;
            clientes.Registro_Paginas();
        }

        private void btnClientesPagos2Buscar_Click(object sender, EventArgs e)
        {
            switch (tabControlClientesPagos2.SelectedIndex)
            {
                case 1:
                    clientes.historialPagos(true);
                    break;
                case 2:
                    clientes.historialIntereses(true);
                    break;
            }
        }

        private void dgClientesPagosCuotas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgClientesPagosCuotas.Rows.Count != 0)
                clientes.GetHistorialPago();
        }

        private void dgClientesPagosCuotas_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgClientesPagosCuotas.Rows.Count != 0)
                clientes.GetHistorialPago();
        }

        private void btnClientesPagos2TicketDeuda_Click(object sender, EventArgs e)
        {
            clientes.TicketDeuda();
        }

        private void dgClientesPagos2PagIntereses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgClientesPagos2PagIntereses.Rows.Count != 0)
                clientes.GethistorialIntereses();
        }

        private void dgClientesPagos2PagIntereses_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgClientesPagos2PagIntereses.Rows.Count != 0)
                clientes.GethistorialIntereses();
        }

        private void btnClientesPagos2TicketIntereses_Click(object sender, EventArgs e)
            => clientes.TicketDeuda();

        private void btnClientesReportes2CancelarFiltro_Click(object sender, EventArgs e)
            => clientes.cancelarFiltro();
        #endregion Cliente


        /******************************************
        *                                         *
        *       CODIGO DE USUARIOS                * 
        *                                         *
        ******************************************/
        #region Usuarios

        private void SeccionUsuarios()
        {
            var textBoxUsuario = new List<TextBox>
            {
                tbUsuarioNID, //0
                tbUsuarioNombre, //1
                tbUsuarioApellido, //2
                tbUsuarioEmail, //3
                tbUsuarioTelefono, //4
                tbUsuarioDireccion, //5
                tbUsuario, //6
                tbUsuarioPass //7
            };

            var labelUsuario = new List<Label>
            {
                lbUsuarioNID, //0
                lbUsuarioNombre, //1
                lbUsuarioApellido, //2
                lbUsuarioEmail, //3
                lbUsuarioTelefono, //4
                lbUsuarioDireccion, //5
                lbUsuario, //6
                lbUsuarioPass, //7
                lbUsuarioPaginas //8
            };

            object[] objectos = {
                pbUsuario,               //0
                chbUsuarioEstado,        //1
                cbUsuarioFunciones,      //2
                dgUsuarioDatos,          //3
                nudUsuarioPaginas,       //4
                btnUsuariosCambiarPass,  //5
                btnUsuariosRegResetPass, //6
                dgUsuariosFunciones,     //7
                tvUsuariosRoles,         //8
                cbUsuariosRoles          //9
            };

            usuario = new VMUsuarios(objectos, textBoxUsuario, labelUsuario);
            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageUsuarios);
            EnabledButton(btnUsuario);
        }

        private void btnUsuario_Click(object sender, EventArgs e)
        {
            SeccionUsuarios();
        }

        private void pbUsuario_Click(object sender, EventArgs e)
        {
            ClsVMObjetos.uploadimage.CargarImagen(pbUsuario);
        }

        private void tbUsuarioNID_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbUsuarioNID, "NID", lbUsuarioNID);
        }

        private void tbUsuarioNID_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void tbUsuarioNombre_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbUsuarioNombre, "Nombre", lbUsuarioNombre);
        }

        private void tbUsuarioNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.textKeyPress(e);
        }

        private void tbUsuarioApellido_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbUsuarioApellido, "Apellido", lbUsuarioApellido);
        }

        private void tbUsuarioApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.textKeyPress(e);
        }

        private void tbUsuarioEmail_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbUsuarioEmail, "e-mail", lbUsuarioEmail);
        }

        private void tbUsuarioTelefono_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbUsuarioEmail, "e-mail", lbUsuarioEmail);

            if (tbUsuarioTelefono.Text.Equals(""))
            {
                lbUsuarioTelefono.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbUsuarioTelefono.Text = "Teléfono";
                lbUsuarioTelefono.ForeColor = Color.Green;
            }
        }

        private void tbUsuarioTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberKeyPress(e);

        }

        private void btnUsuariosCambiarPass_Click(object sender, EventArgs e)
        {
            //var autorizacion_changepass = usuario._cambiarContra;

            if (usuario._cambiarContra.Equals(1))
            {
                FormCambiarPass _formCambioPass = new FormCambiarPass();
                _formCambioPass.ShowDialog();
                var pass = _formCambioPass._pass;
                if (pass.Equals(""))
                {
                    MessageBox.Show("No se puede cambiar contraseña.", "Cambio de Contraseña");
                }
                else
                {
                    usuario.cambiarPass(pass);
                }
            }
            else
            {
                MessageBox.Show("No se puede cambiar contraseña.", "Cambio de Contraseña");
            }

        }

        private void btnUsuarioPrimero_Click(object sender, EventArgs e)
        {
            usuario.Paginador("Primero");
        }

        private void btnUsuarioAnterior_Click(object sender, EventArgs e)
        {
            usuario.Paginador("Anterior");

        }

        private void btnUsuarioSiguiente_Click(object sender, EventArgs e)
        {
            usuario.Paginador("Siguiente");

        }

        private void btnUsuarioUltimo_Click(object sender, EventArgs e)
        {
            usuario.Paginador("Ultimo");

        }

        private void nudUsuarioPaginas_ValueChanged(object sender, EventArgs e)
        {
            usuario.Registro_Paginas();
        }

        private void dgUsuarioDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgUsuarioDatos.Rows.Count != 0)
                usuario.GetUsuario();
        }

        private void dgUsuarioDatos_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgUsuarioDatos.Rows.Count != 0)
                usuario.GetUsuario();
        }

        private void tbUsuarioBuscar_TextChanged(object sender, EventArgs e)
        {
            switch (usuario._seccion)
            {
                case 0:
                    usuario.SearchUsuarios(tbUsuarioBuscar.Text);
                    break;
                case 1:
                    usuario.SearchUsuarioFunciones(tbUsuarioBuscar.Text);
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            usuario.guardarUsuario();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            usuario.restablecer();

        }

        private void btnUsuariosRegResetPass_Click(object sender, EventArgs e)
        {
            usuario.restablecerPass();
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl2.SelectedIndex)
            {
                case 0:
                    tabControl1.SelectedIndex = 0;
                    usuario._seccion = 0;
                    break;
                case 1:
                    tabControl1.SelectedIndex = 1;
                    usuario._seccion = 1;
                    break;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    tabControl2.SelectedIndex = 0;
                    usuario._seccion = 0;
                    break;
                case 1:
                    tabControl2.SelectedIndex = 1;
                    usuario._seccion = 1;
                    break;
            }
        }

        private void btnUsuariosSave_Click(object sender, EventArgs e)
        {
            usuario.AgregarRole();
        }

        private void btnUsuariosEliminar_Click(object sender, EventArgs e)
        {
            usuario.EliminarRole();
        }

        private void dgUsuariosFunciones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgUsuarioDatos.Rows.Count != 0)
                usuario.GetFuncion();
        }

        private void dgUsuariosFunciones_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgUsuarioDatos.Rows.Count != 0)
                usuario.GetFuncion();
        }
        #endregion


        /******************************************
        *                                         *
        *       CODIGO DE CONFIGURACION           * 
        *                                         *
        ******************************************/
        #region Configuracion

        VMConfiguracion config;
        VMCategoria categoria;
        private void SeccionConfiguracion()
        {
            var textBoxConfig = new List<TextBox>
            {
                tbConfigIntereses,              //0
                tbConfiguracionParam1,          //1
                tbConfiguracionParam2,          //2
                tbConfiguracionParam3,          //3
                tbConfiguracionCorrelativo1,    //4
                tbConfiguracionCorrelativo2     //5
            };
            var labelConfig = new List<Label>
            {
                lbConfigMsjIntereses,       //0
                lbConfigIntereses,          //1
                lbConfiguracionParam1,      //2
                lbConfiguracionParam2,      //3
                lbConfiguracionParam3,      //4
                lbConfiguracionCorrelativo  //5
            };


            /* ***************
            *                *
            *  Categoria     *
            *                *
            *****************/
            var textBoxCategoria = new List<TextBox>
            {
                tbCategoriasNombre,
                tbCategoriasBuscar
            };
            var labelCategoria = new List<Label>
            {
                lbCategorias
            };
            var chbCategoria = new List<CheckBox>
            {
                chbCategoriaModelo,   //0
                chbCategoriasSerie,   //1
                chbCategoriasNumero,  //2
                chbCategoriasSize,    //3
                chbCategoriasPeso,    //4
                                      
                chbCategoriasEliminar, //5
                chbCategoriaColor      //6
            };
            object[] objetos = {
                dgCategorias,
            };
            categoria = new VMCategoria(objetos, textBoxCategoria, labelCategoria, chbCategoria);

            config = new VMConfiguracion(_radioConfig, textBoxConfig, labelConfig);
            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageConfiguracion);
            EnabledButton(btnConfiguracion);
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            SeccionConfiguracion();
        }

        private void tbConfigIntereses_TextChanged(object sender, EventArgs e)
        {
            lbConfigMsjIntereses.Text = "";
        }

        private void tbConfigIntereses_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberDecimalKeyPress(tbConfigIntereses, e);
        }

        private void btnConfigAddInteres_Click(object sender, EventArgs e)
        {
            config.RegistrarIntereses();
        }

        private void tbCategoriasNombre_TextChanged(object sender, EventArgs e)
        {
            if (tbCategoriasNombre.Text.Equals(""))
            {
                lbCategorias.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbCategorias.Text = "Categoria";
                lbCategorias.ForeColor = Color.Green;
            }
        }

        private void tbCategoriasNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbCategoriasBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void dgCategorias_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgCategorias.RowCount > 0)
                categoria.GetCategoria();
        }

        private void dgCategorias_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgCategorias.RowCount > 0)
                categoria.GetCategoria();
        }

        private void tbCategoriasBuscar_TextChanged(object sender, EventArgs e)
        {
            categoria.SearchCategoria(tbCategoriasBuscar.Text, chbCategoriasEliminar.Checked);
        }

        private void btnCategoriasEliminar_Click(object sender, EventArgs e)
        {
            categoria.eliminar();
        }

        private void chbCategoriasEliminar_CheckedChanged(object sender, EventArgs e)
        {
            chbCategoriasEliminar.ForeColor = chbCategoriasEliminar.Checked ? Color.Red
              : Color.LightSlateGray;
            categoria.SearchCategoria(tbCategoriasBuscar.Text, chbCategoriasEliminar.Checked);
        }

        private void btnCategoriasSave_Click(object sender, EventArgs e)
        {
            categoria.guardarCompras();
        }

        private void btnCategoriasReset_Click(object sender, EventArgs e)
        {
            categoria.restablecer();
        }

        private void tbConfiguracionParam1_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbConfiguracionParam1, "Punto de Emisión", lbConfiguracionParam1);
        }

        private void tbConfiguracionParam2_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbConfiguracionParam2, "Establecimiento", lbConfiguracionParam2);
        }

        private void tbConfiguracionParam3_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbConfiguracionParam3, "Tipo de Documento", lbConfiguracionParam3);
        }

        private void tbConfiguracionCorrelativo1_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbConfiguracionCorrelativo1, "Correlativo", lbConfiguracionCorrelativo);
        }

        private void tbConfiguracionCorrelativo2_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbConfiguracionCorrelativo2, "Correlativo", lbConfiguracionCorrelativo);
        }

        private void tbConfiguracionParam1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                e.Handled = true;
            }
        }

        private void tbConfiguracionParam2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                e.Handled = true;
            }
        }

        private void tbConfiguracionParam3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                e.Handled = true;
            }
        }

        private void tbConfiguracionCorrelativo1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                e.Handled = true;
            }
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void tbConfiguracionCorrelativo2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                e.Handled = true;
            }
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void btnConfiguracionSave_Click(object sender, EventArgs e)
        {
            config.SaveData();
        }
        #endregion


        /******************************************
         *                                         *
         *       CODIGO DE PROVEEDORES             * 
         *                                         *
         ******************************************/
        #region proveedores
        private VMProveedor proveedor;

        private void SeccionProveedor()
        {
            EnabledButton(btnProveedor);

            var textBoxProveedor = new List<TextBox>
            {
                tbProveedorNombre,//0
                tbProveedorEmail,//1
                tbProveedorTelefono,//2
                tbProveedorDireccion,//3

                tbProveedorPagos,//4
                tbProveedoresFPCuotasQM
            };

            var labelProveedor = new List<Label>
            {
                lbProveedorNombre,//0
                lbProveedorEmail,//1
                lbProveedorTelefono,//2
                lbProveedorDireccion,//3
                label_PaginasProveedor, //4

                lbPagosNombreProveedor,//5
                lbProveedorDeuda,//6
                lbProveedorPago,//7
                lbProveedorFechaPago,//8
                lbProveedorCuota,//9
                lbProveedorTicket,//10

               lbProveedorPagos,//11
               lbProveedorDeudas,//12
               lbProveedorSaldo,//13
               lbProveedorFechaDeuda,//14
               lbProveedorTicketDeuda,//15
               lbProveedorDeudaPago,//16
               lbProveedorMesCuotas,//17
               lbProveedorPagosFecha,//18
               lbProveedorUsuario,//19

               lbProveedoresFPDeudaTotal, //20
               lbProveedoresFPCuotasXmes, //21
               lbProveedoresFPQM, //22
               lbProveedoresFPCuotas, //23
               lbProveedoresFormaPago, //24
               lbProveedorForma //25
            };

            object[] objectos = {
                pbProveedor,
                Properties.Resources.defaultImg1,
                dgProveedor,
                nudPaginasProveedor,
                dgProveedorReport,
                dtpProveedorDeuda1,
                dtpProveedorDeuda2,
                dgProveedorPagosCuotas,
                rbProveedoresFPQuincenal,
                rbProveedoresFPMensual,
                nudProveedoresCuotas

            };
            proveedor = new VMProveedor(objectos, textBoxProveedor, labelProveedor);
            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageProveedores);
        }
        private void btnProveedor_Click(object sender, EventArgs e)
        {
            SeccionProveedor();
        }

        private void tbProveedorNombre_TextChanged(object sender, EventArgs e)
        {
            if (tbProveedorNombre.Text.Equals(""))
            {
                lbProveedorNombre.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbProveedorNombre.Text = "Proveedor";
                lbProveedorNombre.ForeColor = Color.Green;
            }
        }

        private void tbProveedorEmail_TextChanged(object sender, EventArgs e)
        {
            if (tbProveedorEmail.Text.Equals(""))
            {
                lbProveedorEmail.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbProveedorEmail.Text = "Email";
                lbProveedorEmail.ForeColor = Color.Green;
            }
        }

        private void tbProveedorTelefono_TextChanged(object sender, EventArgs e)
        {
            if (tbProveedorTelefono.Text.Equals(""))
            {
                lbProveedorTelefono.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbProveedorTelefono.Text = "Teléfono";
                lbProveedorTelefono.ForeColor = Color.Green;
            }
        }

        private void tbProveedorTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void tbProveedorDireccion_TextChanged(object sender, EventArgs e)
        {
            if (tbProveedorDireccion.Text.Equals(""))
            {
                lbProveedorDireccion.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbProveedorDireccion.Text = "Dirección";
                lbProveedorDireccion.ForeColor = Color.Green;
            }
        }

        private void pbProveedor_Click(object sender, EventArgs e)
        {
            ClsVMObjetos.uploadimage.CargarImagen(pbProveedor);
        }


        private void btnProveedorAgregar_Click(object sender, EventArgs e)
        {
            switch (tabControlProveedores.SelectedIndex)
            {
                case 0:
                    proveedor.guardarProveedor();
                    break;
                case 1:
                    proveedor.EjecutarPago();
                    break;
                case 2:
                    proveedor.setCuotas();
                    break;

            }
        }

        private void dgProveedor_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgProveedor.Rows.Count != 0)
                proveedor.GetProveedor();
        }

        private void dgProveedor_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgProveedor.Rows.Count != 0)
                proveedor.GetProveedor();
        }

        private void nudPaginasProveedor_ValueChanged(object sender, EventArgs e)
        {
            proveedor.Registro_Paginas();
        }

        private void btnProveedorPagina1_Click(object sender, EventArgs e)
        {
            proveedor.Paginador("Primero");
        }

        private void btnProveedorPaginaAnterior_Click(object sender, EventArgs e)
        {
            proveedor.Paginador("Anterior");
        }

        private void btnProveedorPaginaSiguiente_Click(object sender, EventArgs e)
        {
            proveedor.Paginador("Siguiente");
        }

        private void btnProveedorUltimaPagina_Click(object sender, EventArgs e)
        {
            proveedor.Paginador("Ultimo");
        }

        //tabControlProveedor1
        private void tabControlProveedores_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlProveedores.SelectedIndex)
            {
                case 0:
                    tabControlProveedorHistorial.SelectedIndex = 0;
                    proveedor._seccion = 0;
                    break;
                case 1:
                    tabControlProveedorHistorial.SelectedIndex = 1;
                    proveedor._seccion = 1;
                    break;
                case 2:
                    tabControlProveedorHistorial.SelectedIndex = 1;
                    proveedor._seccion = 2;
                    break;
            }
            proveedor.Registro_Paginas();
        }

        //TabControlProveedor2
        private void tabControlProveedorHistorial_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlProveedorHistorial.SelectedIndex)
            {
                case 0:
                    tabControlProveedores.SelectedIndex = 0;
                    proveedor._seccion = 0;
                    break;
                case 1:
                    //tabControlProveedores.SelectedIndex = 1;
                    proveedor._seccion = 1;
                    break;

            }
            proveedor.Registro_Paginas();
        }

        private void btnProveedorCancelar_Click(object sender, EventArgs e)
        {
            switch (tabControlProveedores.SelectedIndex)
            {
                case 0:
                    proveedor.restablecer();
                    break;
                case 1:
                    proveedor.restablecerReport();
                    break;
            }
        }

        private void tbProveedorBuscar_TextChanged(object sender, EventArgs e)
        {
            switch (tabControlProveedores.SelectedIndex)
            {
                case 0:
                    proveedor.SearchProveedor(tbProveedorBuscar.Text);
                    break;
                case 1:
                    proveedor.GetReportes(tbProveedorBuscar.Text);
                    break;
            }
        }

        private void dgProveedorReport_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgProveedorReport.Rows.Count != 0)
                proveedor.GetReportProveedor();
        }

        private void dgProveedorReport_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgProveedorReport.Rows.Count != 0)
                proveedor.GetReportProveedor();
        }

        private void tbProveedorPagos_TextChanged(object sender, EventArgs e)
        {
            if (tbProveedorPagos.Text.Equals(""))
            {
                lbProveedorPagos.ForeColor = Color.LightSlateGray;

            }
            else
            {
                lbProveedorPagos.Text = "Ingrese el pago";
                lbProveedorPagos.ForeColor = Color.Green;
            }
            proveedor.Pagos(false);
        }

        private void tbProveedorPagos_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberDecimalKeyPress(tbProveedorPagos, e);
        }

        //tabControlProveedores3
        private void tabControlProveedorHistorialPagos_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlProveedorHistorialPagos.SelectedIndex)
            {
                case 0:
                    proveedor._seccion1 = 0;
                    break;
                case 1:
                    proveedor._seccion1 = 1;
                    break;

            }
            proveedor.Registro_Paginas();
        }

        private void dgProveedorPagosCuotas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgProveedorPagosCuotas.Rows.Count != 0)
                proveedor.GetHistorialPago();
        }

        private void dgProveedorPagosCuotas_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgProveedorPagosCuotas.Rows.Count != 0)
                proveedor.GetHistorialPago();
        }

        private void btnProveedorTicketDeuda_Click(object sender, EventArgs e)
        {
            proveedor.TicketDeuda();
        }

        private void btnProveedorCancelarFiltro_Click(object sender, EventArgs e)
        {
            proveedor.cancelarFiltro();
        }

        private void btnProveedorBuscarPagos_Click(object sender, EventArgs e)
        {
            proveedor.historialPagos(true);
        }

        private void tbProveedorBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                e.Handled = true;
            }
        }

        private void tbProveedoresFPCuotasQM_TextChanged(object sender, EventArgs e)
        {
            if (tbProveedoresFPCuotasQM.Text.Equals(""))
            {
                lbProveedoresFPCuotas.ForeColor = Color.LightSlateGray;
            }
            else
            {
                var value = rbProveedoresFPQuincenal.Checked ? "Cuota quincenal" : "Cuotas mensual";
                lbProveedoresFPCuotas.Text = value;
                lbProveedoresFPCuotas.ForeColor = Color.Green;
            }
            proveedor.getCuotas();
        }

        private void tbProveedoresFPCuotasQM_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberDecimalKeyPress(tbProveedoresFPCuotasQM, e);

        }

        private void rbProveedoresFPQuincenal_Click(object sender, EventArgs e)
        {
            proveedor.getCuotas();

        }

        private void rbProveedoresFPMensual_Click(object sender, EventArgs e)
        {
            proveedor.getCuotas();

        }

        private void nudProveedoresCuotas_ValueChanged(object sender, EventArgs e)
        {
            proveedor.Pagos(true);
        }

        private void nudProveedoresCuotas_KeyUp(object sender, KeyEventArgs e)
        {
            proveedor.Pagos(false);
        }
        #endregion Proveedores


        /*****************************************
        *                                        *
        *       CODIGO DE COMPRAS                * 
        *                                        *
        *****************************************/
        #region compras

        private VMCompras compras;

        private void SeccionCompras()
        {
            var textBoxCompras = new List<TextBox>
            {
                tbComprasDescripcion,//0
                tbComprasCantidad,   //1
                tbComprasPrecio,     //2
                tbComprasProveedor,  //3
                tbComprasBuscar,     //4
                tbComprasCodigo      //5
            };

            var labelCompras = new List<Label>
            {
                lbComprasDescripcion,  //0
                lbComprasCantidad, //1
                lbComprasPrecio, //2
                lbComprasProveedor, //3
                lbComprasImporteValor, //4
                lbComprasPaginas, //5
                lbComprasImporteTotal, //6
                lbComprasCodigo, //7

                //lbComprasMontoPagar, //8
                //lbComprasDeuda, //9
                //lbComprasDeudas, //10
                //lbComprasPagosProveedor, //11
                //lbComprasPagos //12
            };

            object[] objectos =
            {
                cbComprasProveedor, //0
                chbComprasCredito, //1
                dgComprasTemporal, //2
                chbComprasCreditos, //3
                chbComprasTodos, //4
                nudComprasPaginas, //5
                chbComprasEliminar, //6
                //chbComprasPagoCredito //7
            };

            compras = new VMCompras(objectos, textBoxCompras, labelCompras);
            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageCompras);
            EnabledButton(btnCompras);
        }

        private void btnCompras_Click(object sender, EventArgs e)
        {
            SeccionCompras();
        }
        private void tbComprasProveedor_TextChanged(object sender, EventArgs e)
        {
            if (tbComprasProveedor.Text.Equals(""))
            {
                lbComprasProveedor.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbComprasProveedor.Text = "Proveedor";
                lbComprasProveedor.ForeColor = Color.Green;
            }
            compras.SearchProveedor(tbComprasProveedor.Text);
        }
        private void tbComprasProveedor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbComprasDescripcion_TextChanged(object sender, EventArgs e)
        {
            if (tbComprasDescripcion.Text.Equals(""))
            {
                lbComprasDescripcion.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbComprasDescripcion.Text = "Descripción";
                lbComprasDescripcion.ForeColor = Color.Green;
            }
        }

        private void tbComprasDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbComprasCantidad_TextChanged(object sender, EventArgs e)
        {
            if (tbComprasCantidad.Text.Equals(""))
            {
                lbComprasCantidad.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbComprasCantidad.Text = "Cantidad";
                lbComprasCantidad.ForeColor = Color.Green;
            }
            compras.importes();
        }

        private void tbComprasCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberKeyPress(e);
        }

        private void tbComprasPrecio_TextChanged(object sender, EventArgs e)
        {
            if (tbComprasPrecio.Text.Equals(""))
            {
                lbComprasPrecio.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbComprasPrecio.Text = "Precio";
                lbComprasPrecio.ForeColor = Color.Green;
            }
            compras.importes();
        }

        private void tbComprasPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberDecimalKeyPress(tbComprasPrecio, e);
        }

        private void btnComprasCancelar_Click(object sender, EventArgs e)
        {
            compras.restablecer();
        }

        private void btnComprasSave_Click(object sender, EventArgs e)
        {
            switch (tabControlCompras1.SelectedIndex)
            {
                case 0:
                    compras.guardarCompras();
                    //compras.efectuarCompras();
                    break;
                    //case 1:
                    //    compras.efectuarCompras();
                    //    break;
            }
        }


        private void chbComprasCreditos_Click(object sender, EventArgs e)
        {
            chbComprasTodos.Checked = false;
            chbComprasTodos.ForeColor = Color.Black;
            if (chbComprasCreditos.Checked)
            {
                chbComprasCreditos.ForeColor = Color.Green;
            }
            else
            {
                chbComprasCreditos.ForeColor = Color.Black;
            }
            compras.Registro_Paginas();
            compras.GetImporteTotal();
            compras.importes();
        }

        private void chbComprasCreditos_CheckedChanged(object sender, EventArgs e)
        {
            compras.SearchCompras(tbComprasBuscar.Text, chbComprasEliminar.Checked);
        }

        private void chbComprasTodos_Click(object sender, EventArgs e)
        {
            chbComprasCredito.Checked = false;
            chbComprasCredito.ForeColor = Color.Black;
            if (chbComprasTodos.Checked)
            {
                chbComprasTodos.ForeColor = Color.Green;
            }
            else
            {
                chbComprasTodos.ForeColor = Color.Black;
            }
            compras.Registro_Paginas();
            compras.GetImporteTotal();
            compras.importes();
        }

        private void chbComprasTodos_CheckedChanged(object sender, EventArgs e)
        {
            compras.SearchCompras(tbComprasBuscar.Text, chbComprasEliminar.Checked);
        }
        private void dgComprasTemporal_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgComprasTemporal.Rows.Count != 0)
                compras.GetCompra();
        }

        private void dgComprasTemporal_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgComprasTemporal.Rows.Count != 0)
                compras.GetCompra();
        }

        private void chbComprasCredito_Click(object sender, EventArgs e)
        {
            if (chbComprasCredito.Checked)
            {
                chbComprasCredito.ForeColor = Color.Green;
            }
            else
            {
                chbComprasCredito.ForeColor = Color.Black;
            }
        }

        private void btnCompras1_Click(object sender, EventArgs e)
        {
            compras.Paginador("Primero");
        }

        private void btnComprasAnterior_Click(object sender, EventArgs e)
        {
            compras.Paginador("Anterior");
        }

        private void btnComprasSeguiente_Click(object sender, EventArgs e)
        {
            compras.Paginador("Siguiente");
        }

        private void btnComprasUltimo_Click(object sender, EventArgs e)
        {
            compras.Paginador("Ultimo");
        }

        private void tabControlCompras1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlCompras1.SelectedIndex)
            {
                case 0:
                    //tabControlCompras2.SelectedIndex = 0;
                    compras._seccion1 = 0;
                    break;
                case 1:
                    //tabControlCompras2.SelectedIndex = 1;
                    compras._seccion1 = 1;
                    compras.restablecerPagos();
                    break;
            }
            compras.Registro_Paginas();
        }

        private void tabControlCompras2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlCompras2.SelectedIndex)
            {
                case 0:
                    //tabControlCompras1.SelectedIndex = 0;
                    compras._seccion1 = 0;
                    break;
                case 1:
                    //tabControlCompras1.SelectedIndex = 1;
                    compras._seccion1 = 1;
                    break;
            }
            compras.Registro_Paginas();
        }

        private void nudComprasPaginas_ValueChanged(object sender, EventArgs e)
        {
            compras.Registro_Paginas();
        }

        private void chbComprasEliminar_Click(object sender, EventArgs e)
        {
            if (chbComprasEliminar.Checked)
            {
                chbComprasEliminar.ForeColor = Color.Red;
            }
            else
            {
                chbComprasEliminar.ForeColor = Color.Black;
            }
            compras.SearchCompras(tbComprasBuscar.Text, chbComprasEliminar.Checked);
        }

        private void btnComprasEliminar_Click(object sender, EventArgs e)
        {
            compras.eliminar();
        }


        //private void tbComprasPagos_TextChanged(object sender, EventArgs e)
        //{
        //    if (tbComprasPagos.Text.Equals(""))
        //    {
        //        lbComprasPagos.ForeColor = Color.LightSlateGray;
        //    }
        //    else
        //    {
        //        lbComprasPagos.Text = "Ingrese el pagos";
        //        lbComprasPagos.ForeColor = Color.Green;
        //    }
        //    compras.importes();
        //}

        //private void tbComprasPagos_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    ClsVMObjetos.validacion.numberDecimalKeyPress(tbComprasPagos, e);

        //}


        #endregion

        /*****************************************
        *                                        *
        *       CODIGO DE PRODUCTOS              * 
        *                                        *
        *****************************************/

        #region productos

        private VMProductos productos;

        private void SeccionProductos()
        {

            var textBoxProducto = new List<TextBox>
            {
                tbProductosCodigo,  //0
                tbProductosDescripcion, //1
                tbProductosPrecioVenta, //2
                tbProductosDescuento, //3
                tbProductosMarca, //4
                tbProductosModelo, //5
                tbProductosSerie, //6
                tbProductosNumero, //7
                tbProductosSize, //8
                tbProductosPeso, //9
                tbProductosColor,    //10

                tbProductosCategoria, //11
                tbProductosCantidad,    //12

                tbProductosPedidoCodigo,    //13
                tbProductosPedidoCantidad,    //14
                tbProductosPedidoRazon,    //15
                tbProductosPedidoValor,    //16
                tbProductosPedidoDesc, //17
                tbProductosBuscar,      //18
                tbProductosPedidoCosto  //19  
            };

            var labelProducto = new List<Label>
            {
                lbProductosCodigo,  //0
                lbProductosDescripcion, //1
                lbProductosPrecioVenta, //2
                lbProductosDescuento, //3
                lbProductosMarca, //4
                lbProductosModelo, //5
                lbProductosSerie, //6
                lbProductosNumero, //7
                lbProductosSize, //8
                lbProductosPeso, //9
                lbProductosColor, //10

                lbProductosCategoria, //11
                lbProductosProducto, //12
                lbProductosPrecio, //13
                lbProductosPaginasProductos, //14
                lbProductosCantidad, //15

                lbProductosPedidoCodigo, //16
                lbProductosPedidoCantidad,    //17
                lbProductosPedidoRazon,    //18
                lbProductosPedidoValor,    //19
                lbProductosAccion,         //20
                lbProductosPedidoCosto      //21

            };

            object[] objectos = {
               panelProductosCodigo, //0
               chbProductosCodigo, //1
               dgProductosCompras, //2
               printDocument1, //3
               nudProducto, //4
               dgProductosProductos, //5
               pbProducto, //6
               Properties.Resources._003_caja, //7
               chbProductosImprimir, //8
               cbProductosCategoria, //9
               chbProductosIngresarManual, //10
               dgProductosPedidos,   //11
               pbProductosPedidoImagen, //12
               chbProductoPedido, //13
               pbProductosIdCompra, //14
               Properties.Resources.rojo3, //15
               Properties.Resources.verde3,  //16
               chbProductosPedidoEntregados,  //17
               chbProductosPedidoTodos,  //18
               Properties.Resources.baseline_save_white_24dp,   //19
               Properties.Resources.Agregar1,        //20
            };

            productos = new VMProductos(objectos, textBoxProducto, labelProducto);
            productos.restablecer();
            tabControlProducto.SelectedIndex = 0;
            tabControlProducto.SelectedIndex = 1;
            productos._seccion = 1;
            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageProductos);
            EnabledButton(btnProductos);
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            SeccionProductos();
        }


        private void dgProductosCompras_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgProductosCompras != null)
            {
                if (dgProductosCompras.Rows.Count != 0)
                    productos.dataGridViewCompra();

                if (!_vlrCodigo.Equals(""))
                {
                    tbProductosCodigo.Text = _vlrCodigo;
                }
            }
        }

        private void dgProductosCompras_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgProductosCompras != null)
            {
                if (dgProductosCompras.Rows.Count != 0)
                    productos.dataGridViewCompra();
            }
        }



        private void tbProductosCodigo_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosCodigo, "Código", lbProductosCodigo);

            if (tbProductosCodigo.Enabled)
            {
                var value = productos.codigos.codigoBarra(panelProductosCodigo, tbProductosCodigo.Text, tbProductosDescripcion.Text, tbProductosPrecioVenta.Text);

                if (value == null)
                {
                    lbProductosCodigo.Text = "El codigo ya esta registrado";
                    lbProductosCodigo.ForeColor = Color.Red;
                }
            }
        }

        private void chbProductosCodigo_Click(object sender, EventArgs e)
        {
            if (!tbProductosCodigo.Enabled)
            {
                tbProductosCodigo.Enabled = true;
                chbProductosCodigo.ForeColor = Color.Green;
            }
            else
            {
                tbProductosCodigo.Enabled = false;
                tbProductosCodigo.Text = "";
                chbProductosCodigo.ForeColor = Color.LightSlateGray;
            }
            productos.codigos.codigoBarra(panelProductosCodigo, tbProductosCodigo.Text,
                tbProductosDescripcion.Text, tbProductosPrecioVenta.Text);

        }

        private void tbProductosCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbProductosDescripcion_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosDescripcion, "Descripción", lbProductosDescripcion);

            productos.codigos.codigoBarra(panelProductosCodigo, tbProductosCodigo.Text, tbProductosDescripcion.Text, tbProductosPrecioVenta.Text);
        }

        private void tbProductosDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbProductosPrecioVenta_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosPrecioVenta, "Precio venta", lbProductosPrecioVenta);

            productos.codigos.codigoBarra(panelProductosCodigo, tbProductosCodigo.Text, lbProductosDescripcion.Text, tbProductosPrecioVenta.Text);
            //productos.verificarPrecioVenta();
        }

        private void tbProductosPrecioVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberDecimalKeyPress(tbProductosPrecioVenta, e);
        }

        private void tbProductosCategoria_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosCategoria, "Categoria", lbProductosCategoria);
            productos.cbSearchCategoria(tbProductosCategoria.Text);
        }

        private void tbProductosCategoria_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void btnProductosAgregar_Click(object sender, EventArgs e)
        {
            productos.RegistrarProducto();
        }

        private void tabControlProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlProducto.SelectedIndex)
            {
                case 2:
                    tabControlProductos1.SelectedIndex = 1;
                    break;
                default:
                    tabControlProductos1.SelectedIndex = 0;
                    break;
            }

            productos._seccion = tabControlProducto.SelectedIndex;
            productos.Registro_Paginas();
        }

        private void tabControlProductos1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlProductos1.SelectedIndex)
            {
                case 0:
                    tabControlProducto.SelectedIndex = 1;
                    break;
                case 1:
                    tabControlProducto.SelectedIndex = 2;
                    break;
            }

        }

        private void btnProductosPagina1_Click(object sender, EventArgs e)
        {
            productos.Paginador("Primero");
        }

        private void btnProductosAnterior_Click(object sender, EventArgs e)
        {
            productos.Paginador("Anterior");
        }

        private void btnProductosSiguiente_Click(object sender, EventArgs e)
        {
            productos.Paginador("Siguiente");
        }

        private void btnProductosUltimo_Click(object sender, EventArgs e)
        {
            productos.Paginador("Ultimo");
        }

        private void nudProducto_ValueChanged(object sender, EventArgs e)
        {
            productos.Registro_Paginas();
        }

        private void dgProductosProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgProductosProductos != null)
            {
                if (dgProductosProductos.Rows.Count != 0)
                {
                    productos.dataGridViewCompra();
                }
            }


        }

        private void dgProductosProductos_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgProductosProductos != null)
            {
                if (dgProductosProductos.Rows.Count != 0)
                {

                    if (e.KeyCode == Keys.Right)
                    {
                        productos.Paginador("Siguiente");
                    }
                    else if (e.KeyCode == Keys.Left)
                    {
                        productos.Paginador("Anterior");
                    }
                    else
                    {
                        //if (dgProductosProductos != null)
                        //{
                        //    if (dgProductosProductos.Rows.Count != 0)
                        //    {
                        productos.dataGridViewCompra();
                        //    }
                        //}
                    }
                }
            }

        }

        private void dgProductosPedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgProductosPedidos != null)
            {
                if (dgProductosPedidos.Rows.Count != 0)
                {
                    productos.dataGridViewCompra();
                }
            }
        }

        private void dgProductosProductos_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (dgProductosPedidos != null)
                {
                    if (dgProductosPedidos.Rows.Count != 0)
                    {
                        productos.dataGridViewCompra();
                    }
                }
            }
        }

        private void dgProductosPedidos_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgProductosPedidos != null)
            {
                if (dgProductosPedidos.Rows.Count != 0)
                {
                    productos.dataGridViewCompra();
                }
            }
        }

        private void tbProductosDescuento_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosDescuento, "Descuento", lbProductosDescuento);
        }

        private void tbProductosModelo_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosModelo, "Modelo", lbProductosModelo);
        }

        private void tbProductosModelo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbProductosSerie_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosSerie, "Serie", lbProductosSerie);
        }

        private void tbProductosSerie_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbProductosNumero_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosNumero, "Número", lbProductosNumero);
        }

        private void tbProductosNumero_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbProductosSize_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosSize, "Tamaño", lbProductosSize);
        }

        private void tbProductosSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbProductosPeso_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosPeso, "Peso", lbProductosPeso);
        }

        private void tbProductosPeso_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbProductosDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberDecimalKeyPress(tbProductosDescuento, e);
        }

        private void btnProductoCargarImagen_Click(object sender, EventArgs e)
        {
            ClsVMObjetos.uploadimage.CargarImagen(pbProducto);
        }

        private void pbProducto_DoubleClick(object sender, EventArgs e)
        {
            Image nuevaImagen = pbProducto.Image;
            var bmpNuevo = new Bitmap(nuevaImagen, 1024, 1024);
            String dir = Path.GetTempPath() + pbProducto.Name + ".png";
            bmpNuevo.Save(dir);
            System.Diagnostics.Process.Start(dir);

            bmpNuevo.Dispose();
        }

        private void chbProductosImprimir_Click(object sender, EventArgs e)
        {
            chbProductosImprimir.ForeColor = chbProductosImprimir.Checked ? Color.Green
              : Color.LightSlateGray;
        }

        private void btnProductosCancelar_Click(object sender, EventArgs e)
        {
            productos.restablecer();
            _vlrCodigo = "";
        }

        private void tbProductosMarca_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosMarca, "Marca", lbProductosMarca);
        }

        private void tbProductosCategoria_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbProductosColor_TextChanged(object sender, EventArgs e)
        {
            ClsVMObjetos.validacion.textBoxEfectoTC(tbProductosColor, "Color", lbProductosColor);
        }

        private void tbProductosColor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void chbProductosIngresarManual_CheckedChanged(object sender, EventArgs e)
        {
            productos.restablecer();
        }

        int contador_segundos = 0; 
        private void timer2_Tick(object sender, EventArgs e)
        {
            contador_segundos = contador_segundos + 1;
            Console.Write(timer2.ToString());
            if (contador_segundos == 2)
            {
                timer2.Stop();
                contador_segundos = 0;

                switch (productos._seccion)
                {
                    case 0:
                        productos.SearchCompras(tbProductosBuscar.Text);
                        productos.restablecer_num_paginas();
                        productos.Registro_Paginas();
                        break;

                    case 1:
                        productos.SearchProducto(tbProductosBuscar.Text);
                        productos.restablecer_num_paginas();
                        productos.Registro_Paginas();
                        break;
                    case 2:
                        productos.SearchPedidos(tbProductosBuscar.Text, chbProductosPedidoEliminar.Checked, chbProductosPedidoEntregados.Checked, !chbProductosPedidoTodos.Checked);
                        productos.restablecer_num_paginas();
                        productos.Registro_Paginas();
                        break;

                }

            }
        }

        private void tbProductosBuscar_TextChanged(object sender, EventArgs e)
        {

            timer2.Start();

        }

        private String _vlrCodigo = "";
        private void lbProductosCodigo_Click(object sender, EventArgs e)
        {
            _vlrCodigo = tbProductosCodigo.Text;
            Clipboard.SetText(_vlrCodigo);
        }

        private void chbProductosPedidoEliminar_CheckedChanged(object sender, EventArgs e)
        {
            productos.SearchPedidos(tbProductosBuscar.Text, chbProductosPedidoEliminar.Checked, chbProductosPedidoEntregados.Checked, false);
        }

        private void btnProductosPedidoEliminar_Click(object sender, EventArgs e)
        {
            productos.eliminarPedidos();
        }

        private void chbProductosPedidoTodos_CheckedChanged(object sender, EventArgs e)
        {
            productos.Registro_Paginas();
            //productos.SearchPedidos(tbProductosBuscar.Text, chbProductosPedidoEliminar.Checked, chbProductosPedidoEntregados.Checked, chbProductosPedidoTodos.Checked);
        }

        private void chbProductosPedidoEntregados_CheckedChanged(object sender, EventArgs e)
        {
            productos.Registro_Paginas();
            //productos.SearchPedidos(tbProductosBuscar.Text, chbProductosPedidoEliminar.Checked, chbProductosPedidoEntregados.Checked, chbProductosPedidoTodos.Checked);
        }
        #endregion productos


        /* ***************
        *                *
        *  Inventario    *
        *                *
        *****************/

        #region Inventario

        private static FromInventario formInventario;
        private void SeccionInventario()
        {
            formInventario.TopLevel = false;
            formInventario.Dock = DockStyle.Fill;
            formInventario.WindowState = FormWindowState.Maximized;
            formInventario.FormBorderStyle = FormBorderStyle.None;

            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageInventario);
            tabPageInventario.Controls.Add(formInventario);
            formInventario.restablecer();
            formInventario.Show();
            EnabledButton(btnInventario);
        }

        private void btnInventario_Click(object sender, EventArgs e)
        {
            SeccionInventario();
        }
        #endregion Inventario

        /* *******************
        *                    *
        *  CODIGO DE CAJAS   *
        *                    *
        *********************/

        #region Cajas

        private VMCajas caja;
        private void SeccionCajas()
        {
            var textBoxCaja = new List<TextBox>
            {
                tbCajaBilletes,
                tbCajaMoneda,
            };
            var labeCaja = new List<Label>
            {
                lbNumCajas,
                lbCajaNumero,
                lbCajaBillete,
                lbCajaMoneda,
                lbCajaAsignacion
            };
            object[] objectos =
            {
               nudCajas,
               chbCajaEstado,
               dgCajaLista,
               chbCajasIngresos,
               cbAsignacionCaja
            };
            caja = new VMCajas(objectos, textBoxCaja, labeCaja);
            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageCajas);
            EnabledButton(btnCaja);
        }
        private void btnCaja_Click(object sender, EventArgs e)
        {
            SeccionCajas();
        }


        private void btnCajaAgregar_Click(object sender, EventArgs e)
        {
            switch (tabControlCaja1.SelectedIndex)
            {
                case 1:
                    if (chbCajasIngresos.Checked)
                    {
                        caja.asignarIngresos();
                    }
                    else
                    {
                        caja.registrarCajas();
                    }
                    break;
            }
        }

        private void tabControlCaja1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlCaja1.SelectedIndex)
            {
                case 0:
                    tabControlCaja2.SelectedIndex = 0;
                    caja._seccion = 0;
                    break;
                case 1:
                    tabControlCaja2.SelectedIndex = 1;
                    caja._seccion = 1;
                    break;
            }
        }

        private void tabControlCaja2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlCaja2.SelectedIndex)
            {
                case 0:
                    tabControlCaja1.SelectedIndex = 0;
                    caja._seccion = 0;
                    break;
                case 1:
                    tabControlCaja1.SelectedIndex = 1;
                    caja._seccion = 1;
                    break;
            }
        }

        private void dgCajaLista_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgCajaLista.Rows.Count != 0)
                caja.dataCaja();

        }

        private void dgCajaLista_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgCajaLista.Rows.Count != 0)
                caja.dataCaja();
        }

        private void chbCajaEstado_CheckedChanged(object sender, EventArgs e)
        {
            chbCajaEstado.ForeColor = chbCajaEstado.Checked ? Color.Green
               : Color.LightSlateGray;
        }

        private void chbCajasIngresos_CheckedChanged(object sender, EventArgs e)
        {
            if (chbCajasIngresos.Checked)
            {
                chbCajasIngresos.ForeColor = Color.Green;
                chbCajaEstado.Enabled = false;
                nudCajas.Enabled = false;
                tbCajaBilletes.Enabled = true;
                tbCajaMoneda.Enabled = true;
                cbAsignacionCaja.Enabled = true;
            }
            else
            {
                chbCajasIngresos.ForeColor = Color.LightSlateGray;
                chbCajaEstado.Enabled = true;
                nudCajas.Enabled = true;
                tbCajaBilletes.Enabled = false;
                tbCajaMoneda.Enabled = false;
                cbAsignacionCaja.Enabled = false;
            }
        }


        #endregion Cajas


        /* ***************
        *                *
        *  VENTAS        *
        *                *
        *****************/


        #region Ventas

        private VMVentas venta;
        private Boolean valueVenta = false;
        private void SeccionVentas()
        {
            var textBoxVenta = new List<TextBox>
            {
                tbVentasBuscarCodigo, //0
                tbVentaPagos,         //1
                tbVentasDescuento,     //2
                tbVentasCliente,         //3
                tbVentasPrecio          //4
            };
            var labeVenta = new List<Label>
            {
                lbVentasValorImporte,       //0
                lbVentasValorMontoPagar,    //1
                lbVentaPaginas,             //2
                lbVentaIngresoIni,          //3
                lbVentaIngresosVt,          //4
                lbVentaIngresoTotal,        //5
                lbVentaPago,                //6
                lbVentasCambio,             //7
                lbVentasValorCambio,        //8
                lbVentasValorSubTotal,      //9
                lbVentasValorISV,           //10
                lbVentasValorTotal,         //11
                lbVentasValorDeuda,         //12
                lbVentasDescuento           //13
            };
            object[] objectos =
            {
                nudVentasCantidad,  //0
                dgVentasProductos,  //1
                chbVentasEliminar,  //2
                nudVentaPage,       //3
                chbVentaCredito,     //4
                chbVentasEsCliente,  //5
                dgVentasListaClientes  //6
            };

            venta = new VMVentas(objectos, textBoxVenta, labeVenta);
            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageVentas);
            EnabledButton(btnVentas);
        }

        private void btnVentas_Click(object sender, EventArgs e)
        {
            if (gCajas.IdCaja.Equals(0))
            {
                MessageBox.Show("No tiene asignada caja");
                tabControlPrincipal.TabIndex = 1;
            }
            else
            {
                SeccionVentas();
            }
        }

        private void btnVentasBuscar_Click(object sender, EventArgs e)
        {
            //venta._accion = "insert";
            venta.saveVentasTempo();
        }

        private void tbVentasBuscarCodigo_TextChanged(object sender, EventArgs e)
        {
            if (valueVenta)
            {
                //venta.searchVentasTempo(tbVentasBuscarCodigo.Text, chbVentasEliminar.Checked);
            }
        }

        private void tbVentasBuscarCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            valueVenta = true;
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }

        }

        private void dgVentasProductos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex.Equals(4))
            {
                venta.saveVentasTempo();
                dgVentasProductos.ClearSelection();
                dgVentasProductos.CurrentCell = null;
            }
        }

        private void dgVentasProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgVentasProductos.Rows.Count != 0)
            {
                valueVenta = false;
                venta.dataGridViewVentas();
            }
        }

        private void dgVentasProductos_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgVentasProductos.Rows.Count != 0)
            {
                valueVenta = false;
                venta.dataGridViewVentas();

            }
        }

        private void dgVentasListaClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgVentasListaClientes.Rows.Count != 0)
            {
                valueVenta = false;
                venta.dataGridViewVentas();
                var nombrecompleto = Convert.ToString(dgVentasListaClientes.CurrentRow.Cells["Nombre"].Value) + " " + Convert.ToString(dgVentasListaClientes.CurrentRow.Cells["Apellido"].Value);
                tbVentasCliente.Text = nombrecompleto;
            }
        }

        private void dgVentasListaClientes_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgVentasListaClientes.Rows.Count != 0)
            {
                valueVenta = false;
                venta.dataGridViewVentas();

            }
        }

        private void btnVentasEliminar_Click(object sender, EventArgs e)
        {
            venta.eliminar();
        }

        private void chbVentasEliminar_CheckedChanged(object sender, EventArgs e)
        {
            chbVentasEliminar.ForeColor = chbVentasEliminar.Checked ? Color.Red
               : Color.LightSlateGray;
            venta.searchVentasTempo(tbVentasBuscarCodigo.Text, chbVentasEliminar.Checked);
        }

        private void dgVentasProductos_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex.Equals(4))
            {
                if (Regex.IsMatch(e.FormattedValue.ToString(), "^[0-9]"))
                {
                    e.Cancel = false;
                }
                else
                {
                    MessageBox.Show("Ingrese un valor valido", "Ventas",
                                                             MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void btnVentaPage1_Click(object sender, EventArgs e)
        {
            venta.Paginador("Primero");
        }

        private void btnVentaPage2_Click(object sender, EventArgs e)
        {
            venta.Paginador("Anterior");
        }

        private void btnVentaPage3_Click(object sender, EventArgs e)
        {
            venta.Paginador("Siguiente");
        }

        private void btnVentaPage4_Click(object sender, EventArgs e)
        {
            venta.Paginador("Ultimo");
        }

        private void nudVentaPage_ValueChanged(object sender, EventArgs e)
        {
            venta.Registro_Paginas();
        }

        private void chbVentaCredito_CheckedChanged(object sender, EventArgs e)
        {
            chbVentaCredito.ForeColor = chbVentaCredito.Checked ? Color.Green
               : Color.LightSlateGray;
            venta.restablecer();
        }

        private void tbVentaPagos_TextChanged(object sender, EventArgs e)
        {
            if (tbVentaPagos.Text.Equals(""))
            {
                lbVentaPago.ForeColor = Color.LightSlateGray;
            }
            else
            {
                lbVentaPago.Text = "Pago";
                lbVentaPago.ForeColor = Color.Green;
            }
            venta.pagosCliente();
        }

        private void tbVentaPagos_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberDecimalKeyPress(tbVentaPagos, e);
        }

        private void btnVentasGuardar_Click(object sender, EventArgs e)
        {
            //venta.cobrar();
        }

        private void btnVentasCancelar_Click(object sender, EventArgs e)
        {
            venta.restablecer();
        }
        private void btnVentasFacturaGuardar_Click(object sender, EventArgs e)
        {

        }

        private void tbVentasDescuento_TextChanged(object sender, EventArgs e)
        {
            venta.restablecer();
        }

        private void tabControl5_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl5.SelectedIndex)
            {
                case 0:
                    venta._seccion = 0;
                    break;
                case 1:
                    venta._seccion = 1;
                    break;
            }
        }

        private void btnVentasQuitarCliente_Click(object sender, EventArgs e)
        {
            venta.quitarCliente();
        }

        private void tbVentasPrecio_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbVentasPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClsVMObjetos.validacion.numberDecimalKeyPress(tbVentasPrecio, e);
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) { e.Handled = true; }
        }

        private void tbVentasBuscarCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.B)
            {
                this.Hide();
                FormInventarioExistencia form = new FormInventarioExistencia();
                form.TopLevel = true;
                form.Dock = DockStyle.Fill;
                form.WindowState = FormWindowState.Maximized;
                form.ShowDialog();
                this.Show();
            }
        }
        #endregion Ventas


        /* ***************
        *                *
        *  Bodegas       *
        *                *
        *****************/


        #region bodegas

        private static FormUbicaciones formUbicaciones;
        private void SeccionBodegas()
        {
            formUbicaciones.TopLevel = false;
            formUbicaciones.Dock = DockStyle.Fill;
            formUbicaciones.WindowState = FormWindowState.Maximized;
            formUbicaciones.FormBorderStyle = FormBorderStyle.None;

            tabControlPrincipal.SelectedIndex = tabControlPrincipal.TabPages.IndexOf(tabPageBodega);
            tabPageBodega.Controls.Add(formUbicaciones);
            formUbicaciones.Show();
            EnabledButton(btnBodega);
        }

        private void btnBodega_Click(object sender, EventArgs e)
        {
            SeccionBodegas();
        }














        #endregion bodegas

        private void pbProductosPedidoImagen_DoubleClick(object sender, EventArgs e)
        {
            Image nuevaImagen = pbProductosPedidoImagen.Image;
            var bmpNuevo = new Bitmap(nuevaImagen, 1024, 1024);
            String dir = Path.GetTempPath() + pbProducto.Name + ".png";
            bmpNuevo.Save(dir);
            System.Diagnostics.Process.Start(dir);

            bmpNuevo.Dispose();
        }

        private void pbProducto_Click(object sender, EventArgs e)
        {

        }


    }
}
