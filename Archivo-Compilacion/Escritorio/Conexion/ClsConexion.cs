using LinqToDB;
using LinqToDB.Data;
using ModelsDB.Cliente;
using ModelsDB.Comprass;
using ModelsDB.Ordenador;
using ModelsDB.Proveedor;
using ModelsDB.Usuarios;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using ModelsDB.cProducto;
using ModelsDB.cCaja;
using ModelsDB.cVentas;
using ModelsDB.cBodega;
using ModelsDB.cStock;
using ModelsDB.cConfiguracion;
using System.Globalization;

namespace ModelsDB.Conexion
{
    public class ClsConexion : DataConnection
    {

        public ClsConexion() : base("PDHN1") { }
        public ITable<Clientes> TClientes { get { return GetTable<Clientes>(); } }
        public ITable<RptClientes> TReportesClientes { get { return GetTable<RptClientes>(); } }
        public ITable<Configuracion> TConfiguration { get { return GetTable<Configuracion>(); } }
        public ITable<Configuracion_Facturacion> TConfigurationFacturacion { get { return GetTable<Configuracion_Facturacion>(); } }
        public ITable<RptIntereses> TRptIntereses { get { return GetTable<RptIntereses>(); } }
        public ITable<ClientesIntereses> TClientesIntereses => GetTable<ClientesIntereses>();
        public ITable<PagosClientes> TPagosClientes => GetTable<PagosClientes>();
        public ITable<PagosIntereses> TPagosIntereses => GetTable<PagosIntereses>();
        public ITable<Usuarios.Usuarios> TUsuarios => GetTable<Usuarios.Usuarios>();
        public ITable<Ordenadores> TOrdenadores => GetTable<Ordenadores>();
        public ITable<Funciones> TFunciones => GetTable<Funciones>();
        public ITable<Roles> TRoles => GetTable<Roles>();
        public ITable<Funciones_Roles> TFuncionesRoles => GetTable<Funciones_Roles>();
        public ITable<Proveedores> TProveedor => GetTable<Proveedores>();
        public ITable<ProveedoresReportes> TReportes_proveedor => GetTable<ProveedoresReportes>();
        public ITable<ProveedoresPagos> TPagos_proveedor => GetTable<ProveedoresPagos>();
        public ITable<ComprasTemporal> TCompras_temporal => GetTable<ComprasTemporal>();
        public ITable<Compras> TCompras => GetTable<Compras>();
        public ITable<ComprasReportes> TReportes_compras => GetTable<ComprasReportes>();
        public ITable<Compras_Stock> TCompras_Stock => GetTable<Compras_Stock>();
        public ITable<Productoss> TProductos => GetTable<Productoss>();
        public ITable<Productos_Stock> TProductosStock => GetTable<Productos_Stock>();
        public ITable<Pedidos> TPedidos => GetTable<Pedidos>();
        public ITable<PedidosReportes> TPedidosReportes => GetTable<PedidosReportes>();
        public ITable<ProductosReportes> TProductosReportes => GetTable<ProductosReportes>();
        public ITable<BodegaTemp> TBodegaTemp => GetTable<BodegaTemp>();
        public ITable<Cajas> TCajas => GetTable<Cajas>();
        public ITable<CajasIngresos> TCajas_ingresos => GetTable<CajasIngresos>();
        public ITable<CajasReportes> TCajas_reportes => GetTable<CajasReportes>();
        public ITable<CajasRegistros> TCajas_registros => GetTable<CajasRegistros>();
        //public ITable<VentasTemporal> TVentas_temporal => GetTable<VentasTemporal>();
        //public ITable<Ventas> TVentas => GetTable<Ventas>();
        public ITable<OrdenTemporal> TOrdenTemporal => GetTable<OrdenTemporal>();
        public ITable<OrdenTemporalDetalle> TOrdenTemporalDetalle => GetTable<OrdenTemporalDetalle>();

        public ITable<Bodegas> TTBodegas => GetTable<Bodegas>();
        public ITable<Ubicaciones> TUbicaciones => GetTable<Ubicaciones>();
        public ITable<BodegasReportes> TBodegasReportes => GetTable<BodegasReportes>();
        public ITable<Categoria> TCategoria => GetTable<Categoria>();
        public ITable<Stock> TStock => GetTable<Stock>();
        public ITable<StockTemporal> TStockTemp => GetTable<StockTemporal>();
        public ITable<StockReporte> TStockReporte => GetTable<StockReporte>();
    }
}
