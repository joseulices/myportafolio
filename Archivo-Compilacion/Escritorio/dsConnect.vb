Imports MySql.Data.MySqlClient
Imports System.Collections.Specialized
Imports System.Configuration
Imports MiCodigo
Imports System.Windows.Forms

Public Class dsConnect
    Private conn As New MySqlConnection(ConfigurationManager.ConnectionStrings("biomedConnection").ConnectionString)
    Private cmd As New MySqlCommand
    Private oencriptar As New MiCodigo.Encriptar
    Private da As New MySqlDataAdapter
    Private dr As MySqlDataReader

    Public Function EstadoConexion() As Boolean
        If conn.State.Equals(ConnectionState.Open) Then
            Return True
        Else
            Return False
        End If
    End Function

#Region "Configuracion"

    Public Function CAIS() As List(Of CAIS)

        Dim ocais As New List(Of CAIS)()
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim query As String = ""

        query = "select *
               From configuracion_facturacion 
               ;"

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }

            'With cmd.Parameters
            '    .AddWithValue("_idusuario", idusuario)
            'End With

            da = New MySqlDataAdapter(cmd)
            da.Fill(dt)
            conn.Close()

            For Each row As DataRow In dt.Rows

                ocais.Add(New CAIS With {
                    .ID = row.Item(0),
                    .CAI1 = row.Item(1),
                    .CAI2 = row.Item(2),
                    .CAI3 = row.Item(3),
                    .CAI4 = row.Item(4),
                    .CAILimiteSuperior = row.Item(5),
                    .CAILimiteInferior = row.Item(6),
                    .VDefault = CType(row.Item(7), Boolean),
                    .ValorActual = row.Item(8),
                    .FechaIngreso = row.Item("FechaIngreso").ToString,
                    .FechaModificacion = row.Item("FechaModificacion").ToString
                })

            Next

            Console.Write(ocais.First.FechaIngreso)
            'Console.Write(ocais.First.fechaModificacion)

        Catch ex As Exception

            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")

        End Try

        Return ocais

    End Function

    Public Sub CAIS_Update(cais As CAIS)

        Try
            conn.Open()
            cmd = New MySqlCommand("cai_update", conn) With {
            .CommandType = CommandType.StoredProcedure
            }

            cmd.Parameters.Clear()

            With cmd.Parameters
                .AddWithValue("@_id", cais.ID)
                .AddWithValue("@_cai1", cais.CAI1)
                .AddWithValue("@_cai2", cais.CAI2)
                .AddWithValue("@_cai3", cais.CAI3)
                .AddWithValue("@_cai4", cais.CAI4)
                .AddWithValue("@_cails", cais.CAILimiteSuperior)
                .AddWithValue("@_caili", cais.CAILimiteInferior)
                .AddWithValue("@_cai_ultimo_valor", cais.ValorActual)
                .AddWithValue("@_caivdefault", cais.VDefault)
            End With

            cmd.ExecuteNonQuery()

            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")

            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub

    Public Sub CAIS_Insert(cais As CAIS)

        Try
            conn.Open()
            cmd = New MySqlCommand("cai_insert", conn) With {
            .CommandType = CommandType.StoredProcedure
            }

            cmd.Parameters.Clear()

            With cmd.Parameters
                .AddWithValue("@_cai1", cais.CAI1)
                .AddWithValue("@_cai2", cais.CAI2)
                .AddWithValue("@_cai3", cais.CAI3)
                .AddWithValue("@_cai4", cais.CAI4)
                .AddWithValue("@_cails", cais.CAILimiteSuperior)
                .AddWithValue("@_caili", cais.CAILimiteInferior)
                .AddWithValue("@_cai_ultimo_valor", cais.ValorActual)
                .AddWithValue("@_caivdefault", cais.VDefault)
            End With

            cmd.ExecuteNonQuery()

            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")

            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub

#End Region

#Region "Usuario"
    Public Function UsuarioRoles(id As Integer) As List(Of String)
        Dim roles As New List(Of String)
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = $"select R.`Role` Role from `usuarios` U 
                            join `seg_funcionrole` FR On FR.`FuncionId` = U.`Funcion` 
                            join `seg_role` R On R.`ID` = FR.`RoleId` 
                            where U.Id = @_id 
                            ;"

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(sql, conn) With {
            .CommandType = CommandType.Text
            }

            With cmd.Parameters
                .AddWithValue("_id", id)
            End With

            da = New MySqlDataAdapter(cmd)
            da.Fill(dt)
            conn.Close()

            For Each row As DataRow In dt.Rows

                roles.Add(row.Item(0))

            Next

        Catch ex As Exception

            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")

        End Try

        Return roles

    End Function

    Public Function Usuarios(nombre As String) As List(Of Usuarios)
        Dim ousuarios As New List(Of Usuarios)()
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand("usuarios_get", conn) With {
            .CommandType = CommandType.StoredProcedure
            }

            With cmd.Parameters
                .AddWithValue("_consulta", nombre)
            End With

            da = New MySqlDataAdapter(cmd)
            da.Fill(dt)
            conn.Close()

            Dim i As Integer = 1
            For Each row As DataRow In dt.Rows

                ousuarios.Add(New Usuarios With {
                    .No = i,
                    .Id = row.Item(0).ToString,
                    .Funcion = row.Item(1).ToString,
                    .Usuario = row.Item(2).ToString,
                    .Nombre = row.Item(3).ToString,
                    .Apellido = row.Item(4).ToString,
                    .Telefono = row.Item(5).ToString,
                    .Correo = row.Item(6).ToString,
                    .NombreCompleto = $"{row.Item(3)} {row.Item(4)}",
                    .Eliminar = False
                })
                i = i + 1

            Next

        Catch ex As Exception

            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")

        End Try

        Return ousuarios

    End Function

    Public Function Usuarios(idusuario As Integer) As List(Of Usuarios)
        Dim ousuarios As New List(Of Usuarios)()
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim query As String = ""

        query = "select Id, Funcion, `User`, Nombre, Apellido, Telefono, Correo 
               From usuarios 
                where Id = @_idusuario;"


        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }

            With cmd.Parameters
                .AddWithValue("_idusuario", idusuario)
            End With

            da = New MySqlDataAdapter(cmd)
            da.Fill(dt)
            conn.Close()

            Dim i As Integer = 1
            For Each row As DataRow In dt.Rows

                ousuarios.Add(New Usuarios With {
                    .No = i,
                    .Id = row.Item(0).ToString,
                    .Funcion = row.Item(1).ToString,
                    .Usuario = row.Item(2).ToString,
                    .Nombre = row.Item(3).ToString,
                    .Apellido = row.Item(4).ToString,
                    .Telefono = row.Item(5).ToString,
                    .Correo = row.Item(6).ToString,
                    .NombreCompleto = $"{row.Item(3)} {row.Item(4)}",
                    .Eliminar = False
                })
                i = i + 1

            Next

        Catch ex As Exception

            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")

        End Try

        Return ousuarios

    End Function

    Public Sub UsuariosInsert(usuario As String, nombre As String, apellido As String, telefono As String, correo As String, funcion As Integer)
        Dim id As Integer

        Try
            conn.Open()
            cmd = New MySqlCommand("usuarios_insert", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_funcion", funcion)
                .AddWithValue("@_usuario", usuario)
                .AddWithValue("@_nombre", nombre)
                .AddWithValue("@_apellido", apellido)
                .AddWithValue("@_telefono", telefono)
                .AddWithValue("@_correo", correo)
            End With
            cmd.Parameters.Add(New MySqlParameter("@idusuario_last", MySqlDbType.Int32))
            cmd.Parameters("@idusuario_last").Direction = ParameterDirection.Output

            cmd.ExecuteNonQuery()
            conn.Close()

            If Not IsDBNull(cmd.Parameters("@idusuario_last").Value) Then
                id = cmd.Parameters("@idusuario_last").Value
            Else
                id = 0
            End If

            SetPasswordDefault(id)

        Catch ex As Exception

            id = 0
            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub

    Public Sub UsuariosUpdate(idusuario As Integer, usuario As String, nombre As String, apellido As String, telefono As String, correo As String, funcion As Integer)

        Try
            conn.Open()
            cmd = New MySqlCommand("usuarios_update", conn) With {
            .CommandType = CommandType.StoredProcedure
            }

            cmd.Parameters.Clear()

            With cmd.Parameters
                .AddWithValue("@_idusuario", idusuario)
                .AddWithValue("@_usuario", usuario)
                .AddWithValue("@_nombre", nombre)
                .AddWithValue("@_apellido", apellido)
                .AddWithValue("@_telefono", telefono)
                .AddWithValue("@_correo", correo)
                .AddWithValue("@_idfuncion", funcion)
            End With

            cmd.ExecuteNonQuery()

            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")

            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub

    Public Function UsuarioVerificacion(usuario As String) As Integer
        Dim sql As String = ""
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim leerdatos As MySqlDataReader
        Dim veri As Integer = 0

        sql = "select Id as Veri From usuarios 
               where `User` = @_usuario ;"

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(sql, conn) With {
            .CommandType = CommandType.Text
            }

            cmd.Parameters.Clear()

            With cmd.Parameters
                .AddWithValue("_usuario", usuario)
            End With

            leerdatos = cmd.ExecuteReader

            If leerdatos.Read Then
                veri = leerdatos("Veri")
            End If

            conn.Close()

        Catch ex As Exception

            veri = 0
            MsgBox("Contácte a soporte técnico.1", MsgBoxStyle.Information, "Error")

        End Try

        Return veri

    End Function

    Public Function UsuarioVerificacion_Update(idUsuario As Integer, usuario As String) As Integer

        Dim sql As String = ""
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim leerdatos As MySqlDataReader
        Dim veri As Integer = 0

        sql = "SELECT ID as Veri FROM usuarios 
               where upper(`User`) = @_usuario 
               and ID <> @_idUsuario
                ;"

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(sql, conn) With {
            .CommandType = CommandType.Text
            }

            cmd.Parameters.Clear()

            With cmd.Parameters
                .AddWithValue("_idUsuario", idUsuario)
                .AddWithValue("_usuario", usuario)
            End With

            leerdatos = cmd.ExecuteReader

            If leerdatos.Read Then
                veri = leerdatos("Veri")
            End If

            conn.Close()

        Catch ex As Exception

            veri = 0
            MsgBox("Contácte a soporte técnico.1", MsgBoxStyle.Information, "Error")

        End Try

        Return veri

    End Function

    Public Sub UsuarioDelete(id As Integer)

        Try
            conn.Open()
            cmd = New MySqlCommand("usuarios_delete", conn) With {
            .CommandType = CommandType.StoredProcedure
            }

            cmd.Parameters.Clear()
            With cmd.Parameters
                .AddWithValue("@_id", id)
            End With
            'cmd.Parameters.Add(New MySqlParameter("@_id", MySqlDbType.Int32))
            'cmd.Parameters("@_id").Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception

            id = 0
            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub

#End Region

#Region "Perfil"

    Public Function UsuarioActivo(idusuario As Integer) As Perfil
        Dim ousuario As New Perfil
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim query As String = ""

        query = "select u.Id UsuarioId, u.Funcion FuncionId, (select f.Funcion from seg_funcion f where f.ID = u.Funcion) Funcion, 
                `User` Usuario, Nombre, Apellido, Telefono, Correo                
                From usuarios u
                where u.Id = @_idusuario;"

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }

            With cmd.Parameters
                .AddWithValue("_idusuario", idusuario)
            End With

            da = New MySqlDataAdapter(cmd)
            da.Fill(dt)
            conn.Close()

            For Each row As DataRow In dt.Rows

                With ousuario

                    .Id = row.Item(0).ToString
                    .Funcion = row.Item(1).ToString
                    .FuncionNombre = row.Item(2).ToString
                    .Usuario = row.Item(3).ToString
                    .Nombre = row.Item(4).ToString
                    .Apellido = row.Item(5).ToString
                    .Telefono = row.Item(6).ToString
                    .Correo = row.Item(7).ToString
                    .NombreCompleto = $"{row.Item(4)} {row.Item(5)}"

                End With

            Next

        Catch ex As Exception

            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")

        End Try

        Return ousuario

    End Function

    Public Sub PerfilUpdate(idusuario As Integer, nombre As String, apellido As String, telefono As String, correo As String)

        Try
            conn.Open()
            cmd = New MySqlCommand("perfil_update", conn) With {
            .CommandType = CommandType.StoredProcedure
            }

            cmd.Parameters.Clear()

            With cmd.Parameters

                .AddWithValue("@_idusuario", idusuario)
                .AddWithValue("@_nombre", nombre)
                .AddWithValue("@_apellido", apellido)
                .AddWithValue("@_telefono", telefono)
                .AddWithValue("@_correo", correo)

            End With

            cmd.ExecuteNonQuery()

            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")

            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub

#End Region

#Region "Funcionalidad"
    Public Function Funciones(consulta As String) As DataTable

        Dim sql As String = ""
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter

        sql = "select ID, Funcion From seg_funcion
                where Funcion like concat('%',@_consulta,'%') ;"

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(sql, conn) With {
            .CommandType = CommandType.Text
            }

            With cmd.Parameters
                .AddWithValue("_consulta", consulta)
            End With

            da = New MySqlDataAdapter(cmd)
            da.Fill(dt)
            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")

        End Try

        Return dt

    End Function

    Public Function Roles_Funcion(idFuncion As Integer) As DataTable

        Dim sql As String = ""
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter

        sql = "SELECT r.ID as ID, r.`Role` as Nombre FROM seg_funcionrole f
                join seg_role r on r.ID = f.RoleId
                where f.FuncionId = @_idfuncion
                ;"

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(sql, conn) With {
            .CommandType = CommandType.Text
            }

            With cmd.Parameters
                .AddWithValue("_idfuncion", idFuncion)
            End With

            da = New MySqlDataAdapter(cmd)
            da.Fill(dt)
            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")

        End Try

        Return dt

    End Function

    Public Function FuncionVerificacion(funcion As String) As Integer

        Dim sql As String = ""
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim leerdatos As MySqlDataReader
        Dim veri As Integer = 0

        sql = "SELECT ID as Veri FROM seg_funcion
               where upper(`Funcion`) = @_funcion;"

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(sql, conn) With {
            .CommandType = CommandType.Text
            }

            cmd.Parameters.Clear()

            With cmd.Parameters
                .AddWithValue("_funcion", funcion)
            End With

            leerdatos = cmd.ExecuteReader

            If leerdatos.Read Then
                veri = leerdatos("Veri")
            End If

            conn.Close()

        Catch ex As Exception

            veri = 0
            MsgBox("Contácte a soporte técnico.1", MsgBoxStyle.Information, "Error")

        End Try

        Return veri

    End Function

    Public Function FuncionVerificacion_Update(idFuncion As Integer, funcion As String) As Integer

        Dim sql As String = ""
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim leerdatos As MySqlDataReader
        Dim veri As Integer = 0

        sql = "SELECT ID as Veri FROM seg_funcion 
               where upper(`Funcion`) = @_funcion 
               and ID <> @_idfuncion;"

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(sql, conn) With {
            .CommandType = CommandType.Text
            }

            cmd.Parameters.Clear()

            With cmd.Parameters
                .AddWithValue("_funcion", funcion)
                .AddWithValue("_idfuncion", idFuncion)
            End With

            leerdatos = cmd.ExecuteReader

            If leerdatos.Read Then
                veri = leerdatos("Veri")
            End If

            conn.Close()

        Catch ex As Exception

            veri = 0
            MsgBox("Contácte a soporte técnico.1", MsgBoxStyle.Information, "Error")

        End Try

        Return veri

    End Function

    Public Sub FuncionInsert(funcion As String, roles As List(Of Integer))
        Dim id_funcion As Integer = 0

        Try
            conn.Open()
            cmd = New MySqlCommand("seg_funcion_insert", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_funcion", funcion)
            End With

            cmd.Parameters.Add(New MySqlParameter("@_id", MySqlDbType.Int32))
            cmd.Parameters("@_id").Direction = ParameterDirection.Output

            cmd.ExecuteNonQuery()

            conn.Close()

            If Not IsDBNull(cmd.Parameters("@_id").Value) Then
                id_funcion = Convert.ToInt32(cmd.Parameters("@_id").Value)
            End If

            For Each ids In roles.ToList
                FuncionRoleInsert(id_funcion, ids)
            Next

        Catch ex As Exception

            id_funcion = 0

            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")

            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub

    Public Sub FuncionUpdate(idfuncion As Integer, nombre As String, roles As List(Of Integer))

        Try
            conn.Open()
            cmd = New MySqlCommand("seg_funcion_update", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_idfuncion", idfuncion)
                .AddWithValue("@_nombre", nombre)
            End With

            cmd.ExecuteNonQuery()

            conn.Close()

            For Each ids In roles.ToList
                FuncionRoleInsert(idfuncion, ids)
            Next

        Catch ex As Exception

            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")

            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub

    Private Sub FuncionRoleInsert(funcion_id As Integer, role_id As Integer)

        Try
            conn.Open()
            cmd = New MySqlCommand("seg_funcionrole_insert", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            cmd.Parameters.Clear()
            With cmd.Parameters
                .AddWithValue("@_idfuncion", funcion_id)
                .AddWithValue("@_idrole", role_id)
            End With

            cmd.ExecuteNonQuery()

            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try


    End Sub

    Public Sub FuncionEliminar(funcion_id As Integer)

        Try
            conn.Open()
            cmd = New MySqlCommand("seg_funcion_eliminar", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            cmd.Parameters.Clear()
            With cmd.Parameters
                .AddWithValue("@_idfuncion", funcion_id)
            End With

            cmd.ExecuteNonQuery()

            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try


    End Sub


#End Region

#Region "Role"
    Public Function Roles() As DataTable

        Dim sql As String = ""
        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter

        sql = "select ID, Role From seg_role; "

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(sql, conn) With {
            .CommandType = CommandType.Text
            }

            'With cmd.Parameters
            '    .AddWithValue("_consulta", consulta)
            'End With

            da = New MySqlDataAdapter(cmd)
            da.Fill(dt)
            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")

        End Try

        Return dt

    End Function
#End Region

#Region "Ordenador"
    Public Function OrdenadorActivo(hdd As String) As Object()

        Dim usuarioId = 0, estado As Integer = 0
        Dim isActive As Boolean = False

        Dim objetos As Object()

        Try

            conn.Open()
            cmd = New MySqlCommand("seg_getOrdenador", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd
                .Parameters.Clear()
                .Parameters.AddWithValue("@_hdd", hdd)
                .Parameters.Add(New MySqlParameter("@_estado", MySqlDbType.Int32))
                .Parameters.Add(New MySqlParameter("@_usuarioId", MySqlDbType.Int32))
                .Parameters("@_estado").Direction = ParameterDirection.Output
                .Parameters("@_usuarioId").Direction = ParameterDirection.Output
            End With

            cmd.ExecuteNonQuery()
            conn.Close()

            If Not IsDBNull(cmd.Parameters("@_estado").Value) Then
                estado = Convert.ToBoolean(cmd.Parameters("@_estado").Value)
                usuarioId = cmd.Parameters("@_usuarioId").Value
            End If


            If estado = 1 Then
                isActive = True
            End If


        Catch ex As Exception
            MsgBox("Problemas de conexión." + ex.ToString, MsgBoxStyle.Critical, "Error")
            If EstadoConexion() Then
                conn.Close()
            End If

            estado = 0
            usuarioId = 0
            isActive = False
        End Try

        objetos = {usuarioId, isActive}

        Return objetos

    End Function

    Public Sub OrdenadorCerrarSesion(id As Int32)
        Try
            conn.Open()
            cmd = New MySqlCommand("seg_cerrarsesion", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_usuarioId", id)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Problemas de conexión en Update Parametros.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub

    Public Sub OrdenadorGrabarSesion(id As Int32, ordenador As String)
        Try
            conn.Open()
            cmd = New MySqlCommand("seg_grabarsesion", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_usuarioId", id)
                .AddWithValue("@_ordenador", ordenador)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Problemas de conexión en Update Parametros.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub

#End Region

#Region "Estructura"
    Public Function ExamenListarQuimica(nombre As String) As List(Of EstructuraExamenQuimica)

        Dim datos As New List(Of EstructuraExamenQuimica)
        Dim ds As New DataSet
        Dim da As New MySqlDataAdapter

        conn.Open()

        cmd = New MySqlCommand("estructura_examen_quimica_listar", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_nombre", nombre)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(ds, "Quimica")
        conn.Close()

        If ds.Tables("Quimica").Rows.Count > 0 Then
            For Each row As DataRow In ds.Tables("Quimica").Rows

                datos.Add(New EstructuraExamenQuimica With {
                         .ID = row("ID"),
                         .Nombre = row("Nombre"),
                         .Codigo = row("Codigo"),
                         .Referencia = Convert.ToString(row("Referencia")),
                         .Unidad = Convert.ToString(row("Unidad")),
                         .Precio = row("Precio")
                          })

            Next
        Else
            datos.Add(New EstructuraExamenQuimica With {
                         .ID = 0,
                         .Nombre = "",
                         .Codigo = "",
                         .Referencia = "",
                         .Unidad = "",
                         .Precio = 0
                          })
        End If

        Return datos

    End Function

    Public Function ExamenListarHematologia(nombre As String) As List(Of EstructuraExamenHematologia)

        Dim datos As New List(Of EstructuraExamenHematologia)
        Dim ds As New DataSet
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"SELECT ID, Nombre, Codigo, Precio
	            FROM estructura_examen_hematologia
	            where Nombre like concat('%',@_nombre,'%')
	            order by ID;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_nombre", nombre)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(ds, "Hematologia")
        conn.Close()

        If ds.Tables("Hematologia").Rows.Count > 0 Then
            For Each row As DataRow In ds.Tables("Hematologia").Rows

                datos.Add(New EstructuraExamenHematologia With {
                         .ID = row("ID"),
                         .Nombre = row("Nombre"),
                         .Codigo = row("Codigo"),
                         .Precio = row("Precio")
                          })

            Next
        Else
            datos.Add(New EstructuraExamenHematologia With {
                         .ID = 0,
                         .Nombre = "",
                         .Codigo = "",
                         .Precio = 0
                          })
        End If

        Return datos

    End Function

    Public Function ExamenListarHematozoario(nombre As String) As List(Of EstructuraExamenHematozoario)

        Dim datos As New List(Of EstructuraExamenHematozoario)
        Dim ds As New DataSet
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"SELECT ID, Nombre, Codigo, Precio
	            FROM estructura_examen_hematozoario
	            where Nombre like concat('%',@_nombre,'%')
	            order by ID;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_nombre", nombre)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(ds, "Hematozoario")
        conn.Close()

        If ds.Tables("Hematozoario").Rows.Count > 0 Then
            For Each row As DataRow In ds.Tables("Hematozoario").Rows

                datos.Add(New EstructuraExamenHematozoario With {
                         .ID = row("ID"),
                         .Nombre = row("Nombre"),
                         .Codigo = row("Codigo"),
                         .Precio = row("Precio")
                          })

            Next
        Else
            datos.Add(New EstructuraExamenHematozoario With {
                         .ID = 0,
                         .Nombre = "",
                         .Codigo = "",
                         .Precio = 0
                          })
        End If

        Return datos

    End Function

    Public Function EsctructuraExamenHematozoario_Insert(nombre As String) As Integer

        Dim id As Integer

        Try
            conn.Open()
            cmd = New MySqlCommand("estructura_examen_hematozoario_insert", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_nombre", nombre)
            End With
            cmd.Parameters.Add(New MySqlParameter("@_id", MySqlDbType.Int32))
            cmd.Parameters("@_id").Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            conn.Close()
            id = cmd.Parameters("@_id").Value

        Catch ex As Exception

            id = 0
            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

        Return id

    End Function

    Public Function ExamenListarInmunoserologia(nombre As String) As List(Of EstructuraExamenInmunoserologia)

        Dim datos As New List(Of EstructuraExamenInmunoserologia)
        Dim ds As New DataSet
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"SELECT ID, Nombre, Codigo, Precio
	            FROM estructura_examen_inmunoserologia
	            where Nombre like concat('%',@_nombre,'%')
	            order by ID;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_nombre", nombre)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(ds, "Inmunoserologia")
        conn.Close()

        If ds.Tables("Inmunoserologia").Rows.Count > 0 Then
            For Each row As DataRow In ds.Tables("Inmunoserologia").Rows

                datos.Add(New EstructuraExamenInmunoserologia With {
                         .ID = row("ID"),
                         .Nombre = row("Nombre"),
                         .Codigo = row("Codigo"),
                         .Precio = row("Precio")
                          })

            Next
        Else
            datos.Add(New EstructuraExamenInmunoserologia With {
                         .ID = 0,
                         .Nombre = "",
                         .Codigo = "",
                         .Precio = 0
                          })
        End If

        Return datos

    End Function

    Public Function ExamenListarPruebaEspecial(nombre As String) As List(Of EstructuraExamenPruebasEspeciales)

        Dim datos As New List(Of EstructuraExamenPruebasEspeciales)
        Dim ds As New DataSet
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"SELECT ID, Nombre, Codigo, Precio
	            FROM estructura_examen_pruebasespeciales
	            where Nombre like concat('%',@_nombre,'%')
	            order by ID;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_nombre", nombre)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(ds, "Inmunoserologia")
        conn.Close()

        If ds.Tables("Inmunoserologia").Rows.Count > 0 Then
            For Each row As DataRow In ds.Tables("Inmunoserologia").Rows

                datos.Add(New EstructuraExamenPruebasEspeciales With {
                         .ID = row("ID"),
                         .Nombre = row("Nombre"),
                         .Codigo = row("Codigo"),
                         .Precio = row("Precio")
                          })

            Next
        Else
            datos.Add(New EstructuraExamenPruebasEspeciales With {
                         .ID = 0,
                         .Nombre = "",
                         .Codigo = "",
                         .Precio = 0
                          })
        End If

        Return datos

    End Function
#End Region

#Region "Examenes"
    Public Function ExamenListar(nombre As String) As List(Of Examenes)

        Dim datos As New List(Of Examenes)
        Dim ds As New DataSet
        Dim da As New MySqlDataAdapter

        conn.Open()

        cmd = New MySqlCommand("examen_listar", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_nombre", nombre)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(ds, "Examenes")
        conn.Close()

        If ds.Tables("Examenes").Rows.Count > 0 Then
            For Each row As DataRow In ds.Tables("Examenes").Rows

                If row("ID") Is DBNull.Value Then
                    row("ID") = "0"
                End If

                If row("Examen") Is DBNull.Value Then
                    row("Examen") = ""
                End If

                If row("Categoria") Is DBNull.Value Then
                    row("Categoria") = "0"
                End If

                If row("NecesitaParametros") Is DBNull.Value Then
                    row("NecesitaParametros") = False
                End If

                If row("Precio") Is DBNull.Value Then
                    row("Precio") = "0.00"
                End If

                datos.Add(New Examenes With {
                         .ID = row("ID"),
                         .Examen = row("Examen"),
                         .Categoria = row("Categoria"),
                         .ProcesoFinalizado = row("NecesitaParametros"),
                         .Precio = row("Precio")
                          })

            Next
        Else
            datos.Add(New Examenes With {
                         .ID = 0,
                         .Examen = "No hay Datos",
                         .Categoria = 0,
                         .ProcesoFinalizado = False,
                         .Precio = 0
                          })
        End If

        Return datos

    End Function

    Public Function GestionarExamenes_NumRegistros(fecha_inicial As DateTime, fecha_final As DateTime,
                                      valores As Integer(), idUsuario As Integer) As Integer

        Dim datos As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""
        Dim etapa As Boolean = valores(2)
        Dim num_registros As Integer = 0

        sql = $"SELECT count(*) Registros
	            FROM factura_detalle fd
	            join factura f on f.ID = fd.FacturaID
                join tipoexamen te on te.TiExaId = fd.TipoExamenID
                where te.CeId in (select (case RoleId 
                when 4 then 1 
                when 5 then 2 
                when 6 then 3 
                when 7 then 4 
                when 8 then 5 
                when 9 then 6 
                when 10 then 7 
                when 11 then 8 
                when 12 then 9 
                end) from seg_funcionrole where FuncionId = (select Funcion from usuarios where Id = @_idUsuario)) 
                and date(Fecha) between date(@_fechainicial) and (@_fechafinal)
                and f.EsNula = false and fd.Etapa = @_etapa
                order by fd.ID
                ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_fechainicial", fecha_inicial.ToString("yyyy-MM-dd"))
            .AddWithValue("@_fechafinal", fecha_final.ToString("yyyy-MM-dd"))
            .AddWithValue("@_etapa", etapa)
            .AddWithValue("@_idUsuario", idUsuario)
        End With

        dr = cmd.ExecuteReader()

        If dr.Read Then
            num_registros = dr("Registros")
        End If

        conn.Close()

        Return num_registros

    End Function

    Public Function GestionarExamenes(fecha_inicial As DateTime, fecha_final As DateTime,
                                      valores As Integer(), idUsuario As Integer) As DataTable

        Dim datos As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""
        Dim pagina_inicial = valores(0)
        Dim pagina_siguiente = valores(1)
        Dim etapa As Boolean = valores(2)

        Dim i As Integer = (pagina_inicial - 1) * pagina_siguiente + 1
        pagina_inicial = (pagina_siguiente) * (pagina_inicial - 1)

        sql = $"SELECT 1 `No`, f.ID FacturaID, fd.ID FacturaDetalleID, fd.TipoExamenId idTipoExamen, if(f.EsPermanente = True, (Select ClieNombre From cliente c where c.ClieId = f.ClienteId), (Select 
	            Nombre From cliente_temporal ct where ct.ID = f.ClienteId)) Nombre, te.TiExaNombre Examen, (SELECT CeId FROM categoria_examen where CeId = te.CeId) Categoria, (SELECT CeNombre FROM categoria_examen where CeId = te.CeId) TipoExamen, if(fd.Etapa=TRUE, 'Finalizada',
	            'Procesando') 'Etapa', date(f.Fecha) FechaRecepcion, date(fd.FechaEmision) FechaEmision
	            FROM factura_detalle fd
	            join factura f on f.ID = fd.FacturaID
                join tipoexamen te on te.TiExaId = fd.TipoExamenID
                where te.CeId in (select (case RoleId 
                when 4 then 1 
                when 5 then 2 
                when 6 then 3 
                when 7 then 4 
                when 8 then 5 
                when 9 then 6 
                when 10 then 7 
                when 11 then 8 
                when 12 then 9 
                end) from seg_funcionrole where FuncionId = (select Funcion from usuarios where Id = @_idUsuario)) 
                and date(Fecha) between date(@_fechainicial) and (@_fechafinal)
                and f.EsNula = false and fd.Etapa = @_etapa
                order by fd.ID
                LIMIT @_limite OFFSET @_pagina;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_fechainicial", fecha_inicial.ToString("yyyy-MM-dd"))
            .AddWithValue("@_fechafinal", fecha_final.ToString("yyyy-MM-dd"))
            .AddWithValue("@_pagina", pagina_inicial)
            .AddWithValue("@_limite", pagina_siguiente)
            .AddWithValue("@_etapa", etapa)
            .AddWithValue("@_idUsuario", idUsuario)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(datos)
        conn.Close()


        For Each row As DataRow In datos.Rows
            row("No") = i
            i = i + 1
        Next

        Return datos

    End Function

    Public Function GestionarExamenes_ParametrosHematologia(facturaDetalleId) As DataTable

        Dim datos As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"Select EXH.Codigo Codigo From factura_detalle_parametros FDP
	            Join factura_detalle FD on FD.ID = FDP.FacturaDetalleId
                Join estructura_examen_hematologia EXH on EXH.ID = FDP.ParametroId
                Where FDP.FacturaDetalleId = @_idFacturaDetalle
                ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }

        cmd.Parameters.Clear()

        With cmd.Parameters
            .AddWithValue("@_idFacturaDetalle", facturaDetalleId)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(datos)
        conn.Close()

        Return datos

    End Function

    Public Function GestionarExamenes_ParametrosHematozoario(facturaDetalleId) As String

        Dim nombre As String = ""
        Dim da As New MySqlDataAdapter
        Dim dr As MySqlDataReader
        Dim sql As String = ""

        sql = $"Select EXH.Nombre Nombre From factura_detalle_parametros FDP
	            Join factura_detalle FD on FD.ID = FDP.FacturaDetalleId
                Join estructura_examen_hematozoario EXH on EXH.ID = FDP.ParametroId
                Where FDP.FacturaDetalleId = @_idFacturaDetalle
                ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }

        cmd.Parameters.Clear()

        With cmd.Parameters
            .AddWithValue("@_idFacturaDetalle", facturaDetalleId)
        End With

        dr = cmd.ExecuteReader()

        If dr.Read Then
            nombre = dr("Nombre")
        End If

        conn.Close()

        Return nombre

    End Function

    Public Function GestionarExamenes_ParametrosInmunoserologia(facturaDetalleId) As DataTable

        Dim datos As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"Select EXI.Codigo Codigo From factura_detalle_parametros FDP
	            Join factura_detalle FD on FD.ID = FDP.FacturaDetalleId
                Join estructura_examen_inmunoserologia EXI on EXI.ID = FDP.ParametroId
                Where FDP.FacturaDetalleId = @_idFacturaDetalle
                ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }

        cmd.Parameters.Clear()

        With cmd.Parameters
            .AddWithValue("@_idFacturaDetalle", facturaDetalleId)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(datos)
        conn.Close()

        Return datos

    End Function

    Public Function GestionarExamenes_ParametrosQuimica(facturaDetalleId) As DataTable

        Dim datos As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"Select EXQ.Codigo Codigo From factura_detalle_parametros FDP
	            Join factura_detalle FD on FD.ID = FDP.FacturaDetalleId
                Join estructura_examen_quimica EXQ on EXQ.ID = FDP.ParametroId
                Where FDP.FacturaDetalleId = @_idFacturaDetalle
                ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }

        cmd.Parameters.Clear()

        With cmd.Parameters
            .AddWithValue("@_idFacturaDetalle", facturaDetalleId)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(datos)
        conn.Close()

        Return datos

    End Function

    Public Function GestionarExamenes_ParametrosPruebasEspeciales(facturaDetalleId) As DataTable

        Dim datos As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"Select EXP.Codigo Codigo From factura_detalle_parametros FDP
	            Join factura_detalle FD on FD.ID = FDP.FacturaDetalleId
                Join estructura_examen_pruebasespeciales EXP on EXP.ID = FDP.ParametroId
                Where FDP.FacturaDetalleId = @_idFacturaDetalle
                ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }

        cmd.Parameters.Clear()

        With cmd.Parameters
            .AddWithValue("@_idFacturaDetalle", facturaDetalleId)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(datos)
        conn.Close()

        Return datos

    End Function

    Public Function GetIdFormatoExamen(idTipoExamen As Integer) As Integer

        Dim _idTipoExamen As Integer = 0
        Dim da As New MySqlDataAdapter
        Dim dr As MySqlDataReader
        Dim sql As String = ""

        sql = $"select tef.IdFormato IdFormato from formato_examen fe
	            join tipoexamen_formato tef on tef.IdFormato = fe.Id
                where tef.IdTipoExamen = @_idTipoExamen
                ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }

        cmd.Parameters.Clear()

        With cmd.Parameters
            .AddWithValue("@_idTipoExamen", idTipoExamen)
        End With

        dr = cmd.ExecuteReader()

        If dr.Read Then
            _idTipoExamen = dr("IdFormato")
        End If

        conn.Close()

        Return _idTipoExamen

    End Function

    Public Function GetIdNombreExamenXTipoExamen(idTipoExamen As Integer) As String

        Dim nombre As String = ""
        Dim da As New MySqlDataAdapter
        Dim dr As MySqlDataReader
        Dim sql As String = ""

        sql = $"select TiExaNombre Nombre from tipoexamen
                where TiExaId = @_idTipoExamen
                ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }

        cmd.Parameters.Clear()

        With cmd.Parameters
            .AddWithValue("@_idTipoExamen", idTipoExamen)
        End With

        dr = cmd.ExecuteReader()

        If dr.Read Then
            nombre = dr("Nombre")
        End If

        conn.Close()

        Return nombre

    End Function

#End Region

#Region "Factura Temporal"
    Public Function FacturaOrdenTemporal_Insertar(ClienteId As Integer, EsPermanente As Integer, Usuario As Integer,
                             Mes As Integer, Dia As Integer, MetodoPago As Integer, Descuento As Decimal, Pago As Decimal,
                             Cambio As Decimal, Deuda As Decimal) As Integer

        Dim ordenTemporalId As Integer

        Try
            conn.Open()
            cmd = New MySqlCommand("factura_ordentemporal_insert", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_clienteId", ClienteId)
                .AddWithValue("@_isPermanente", EsPermanente)
                .AddWithValue("@_usuario", Usuario)
                .AddWithValue("@_mes", Mes)
                .AddWithValue("@_dia", Dia)
                .AddWithValue("@_metodopago", MetodoPago)
                .AddWithValue("@_descuento", Descuento)
                .AddWithValue("@_pago", Pago)
                .AddWithValue("@_cambio", Cambio)
                .AddWithValue("@_deuda", Deuda)
                .AddWithValue("@_getordenTemporaLid", 0)
            End With
            cmd.Parameters.Add(New MySqlParameter("@_setidtemporal", MySqlDbType.Int32))
            cmd.Parameters("@_setidtemporal").Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            conn.Close()
            ordenTemporalId = cmd.Parameters("@_setidtemporal").Value
        Catch ex As Exception
            ordenTemporalId = 0
            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

        Return ordenTemporalId

    End Function

    Public Sub FacturaOrdenTemporal_Update(ID As Integer, ClienteId As Integer, EsPermanente As Boolean, Mes As Integer, Dia As Integer,
                                                MetodoPago As Integer, SubTotal As Decimal, Total As Decimal, Descuento As Decimal,
                                                Pago As Decimal, Cambio As Decimal, Deuda As Decimal)

        Try
            conn.Open()
            cmd = New MySqlCommand("factura_ordentemporal_update", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_ID", ID)
                .AddWithValue("@_clienteId", ClienteId)
                .AddWithValue("@_isPermanente", EsPermanente)
                .AddWithValue("@_mes", Mes)
                .AddWithValue("@_dia", Dia)
                .AddWithValue("@_metodopago", MetodoPago)
                .AddWithValue("@_subtotal", SubTotal)
                .AddWithValue("@_descuento", Descuento)
                .AddWithValue("@_total", Total)
                .AddWithValue("@_pago", Pago)
                .AddWithValue("@_cambio", Cambio)
                .AddWithValue("@_deuda", Deuda)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error FacturaOrdenTemporal_Update")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try


    End Sub

    Public Sub FacturaOrdenTemporal_Cliente_Update(idOrdenTemporal As Integer, cliente As Integer, espermanente As Boolean)
        Try
            conn.Open()
            cmd = New MySqlCommand("factura_ordentemporal_cliente_update", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_ID", idOrdenTemporal)
                .AddWithValue("@_cliente", cliente)
                .AddWithValue("@_isPermanente", espermanente)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub


    Public Function FacturaOrdenTemporal_Verificacion(usuario As Integer) As Object()

        Dim objetos As New Object()
        Dim verificacion_siexiste As Boolean
        Dim da As New MySqlDataAdapter
        Dim dt As New DataTable

        Try
            conn.Open()
            cmd = New MySqlCommand("factura_ordentemporal_verificacion", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_usuario", usuario)
            End With
            cmd.Parameters.Add(New MySqlParameter("@verificacion_facturaActiva", MySqlDbType.Int32))
            cmd.Parameters("@verificacion_facturaActiva").Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()


            verificacion_siexiste = Convert.ToBoolean(cmd.Parameters("@verificacion_facturaActiva").Value)

            If verificacion_siexiste Then
                da = New MySqlDataAdapter(cmd)
                da.Fill(dt)
            End If
            conn.Close()

        Catch ex As Exception
            verificacion_siexiste = False
            MsgBox("Contácte soporte técnico.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

        objetos = {verificacion_siexiste, dt}

        Return objetos

    End Function

    Public Function FacturaOrdenTemporal_Detalle(idFacturaOrdenTempo As Integer) As List(Of DetalleFacturaTemp)

        Dim datosDetalleFacturaTemp As New List(Of DetalleFacturaTemp)
        Dim datosExamenes As New List(Of Examenes)
        Dim ds As New DataSet
        Dim da As New MySqlDataAdapter
        Dim dt As New DataTable

        datosExamenes = ExamenListar("")

        conn.Open()

        cmd = New MySqlCommand("factura_ordentemporal_detalle", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_idFacturaOrdenTemp", idFacturaOrdenTempo)
        End With

        da = New MySqlDataAdapter(cmd)
        da.Fill(ds, "Examenes")
        conn.Close()

        If ds.Tables("Examenes").Rows.Count > 0 Then
            For Each row As DataRow In ds.Tables("Examenes").Rows

                datosDetalleFacturaTemp.Add(New DetalleFacturaTemp With {
                         .No = datosDetalleFacturaTemp.Count + 1,
                         .ID = row("ID"),
                         .FacturaOrdenTemporalId = row("FacturaOrdenTemporal_id"),
                         .TipoExamenId = row("TipoExamenId"),
                         .TipoExamenNombre = datosExamenes.Where(Function(x) x.ID = row("TipoExamenId")).First().Examen,
                         .FechaEmision = Convert.ToDateTime(row("FechaEmision")),
                         .Valor = row("Valor"),
                         .NecesitaParametros = Not Convert.ToBoolean(row("HasPendientes")),
                         .TieneParametros = Convert.ToBoolean(row("TieneParametros"))
                          })

            Next
        Else
            datosDetalleFacturaTemp.Add(New DetalleFacturaTemp With {
                        .ID = 0,
                         .TipoExamenNombre = "",
                         .FechaEmision = Now,
                         .Valor = 0,
                         .NecesitaParametros = False,
                         .TieneParametros = False
                          })

        End If



        Return datosDetalleFacturaTemp

    End Function

    Public Function FacturaOrdenTemporal_Detalle_VerificacionExamen(ID As Integer, idExamenTemporal As Integer) As Boolean

        Dim verificacion As Boolean = False
        Dim leerdatos As MySqlDataReader

        Try
            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand("factura_ordentemporal_detalle_verificarexamen", conn) With {
            .CommandType = CommandType.StoredProcedure
            }

            With cmd.Parameters
                .AddWithValue("_ID", ID)
                .AddWithValue("_idTipoExamen", idExamenTemporal)
            End With

            leerdatos = cmd.ExecuteReader

            If leerdatos.Read Then
                verificacion = leerdatos("Verificacion")
            End If

            conn.Close()

        Catch ex As Exception

            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")

        End Try

        Return verificacion

    End Function

    Public Sub FacturaOrdenTemporal_Detalle_Insert(idFactura As Integer, idtipoExamen As Integer,
                                    fechaEmision As DateTime, valor As Decimal, procesofinalizado As Boolean)

        Try
            conn.Open()

            cmd = New MySqlCommand("factura_ordentemporal_detalle_insert", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("_idfactura", idFactura)
                .AddWithValue("_idtipoexamen", idtipoExamen)
                .AddWithValue("_fechaemision", fechaEmision)
                .AddWithValue("_valor", valor)
                .AddWithValue("_haspendiente", Not procesofinalizado)
            End With

            cmd.ExecuteNonQuery()

            conn.Close()


        Catch ex As Exception
            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")
        End Try

    End Sub

    Public Sub FacturaOrdenTemporal_Detalle_Delete(idOrdenTemporal As Integer)
        Try
            conn.Open()
            cmd = New MySqlCommand("factura_ordentemporal_detalle_delete", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_id", idOrdenTemporal)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub

    Public Sub FacturaOrdenTemporal_Detalle_ActualizarFechaEmisionyValor(idOrdenTemporal As Integer, idFacturaTemporalDetalle As Integer,
                                                                   fechaEmision As DateTime, valor As Decimal)
        Try
            conn.Open()
            cmd = New MySqlCommand("factura_ordentemporal_detalle_actualizarfechaemisionyvalor", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_idFacturaOrdenTemporal", idOrdenTemporal)
                .AddWithValue("@_idFacturaDetalleTemporal", idFacturaTemporalDetalle)
                .AddWithValue("@_fechaemision", fechaEmision.ToString("yyyy-MM-dd"))
                .AddWithValue("@_valor", valor)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub

    Public Sub FacturaOrdenTemporal_Detalle_ActualizarValor(idOrdenTemporal As Integer, idFacturaTemporalDetalle As Integer, valor As Decimal)

        Try

            conn.Open()

            cmd = New MySqlCommand("factura_ordentemporal_detalle_actualizarvalor", conn) With {
                .CommandType = CommandType.StoredProcedure
            }

            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_idFacturaOrdenTemporal", idOrdenTemporal)
                .AddWithValue("@_idFacturaDetalleTemporal", idFacturaTemporalDetalle)
                .AddWithValue("@_valor", valor)
            End With

            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")
        End Try

    End Sub

    Public Sub FacturaOrdenTemporal_Detalle_InsertParametros(idFacturaTemporalDetalle As Integer, parametro As Integer)

        Dim sql As String = "INSERT INTO `factura_orden_detalle_temporal_parametros`
                            (`FacturaDetalleTemporalId`,
                            `ParametroId`
                            )
                            VALUES
                            (
                            @_idFacturaDetalleTemporal,
                            @_parametro);"

        Try

            conn.Open()

            cmd = New MySqlCommand(sql, conn) With {
                .CommandType = CommandType.Text
            }

            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_idFacturaDetalleTemporal", idFacturaTemporalDetalle)
                .AddWithValue("@_parametro", parametro)
            End With

            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")
        End Try

    End Sub

    Public Sub FacturaOrdenTemporal_Detalle_FinalizarProceso(idFacturaTemporalDetalle As Integer)

        Dim sql As String = "UPDATE `factura_orden_detalle_temporal`
                            SET
                            `HasPendientes` = False
                            WHERE `ID` = @_idFacturaDetalleTemporal;"

        Try

            conn.Open()

            cmd = New MySqlCommand(sql, conn) With {
                .CommandType = CommandType.Text
            }

            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_idFacturaDetalleTemporal", idFacturaTemporalDetalle)
            End With

            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Contácte a soporte técnico.", MsgBoxStyle.Information, "Error")
        End Try

    End Sub

    Public Function ExamenParametrosSeleccionadosQuimica(id As Integer) As DataTable

        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = "SELECT Codigo FROM `estructura_examen_quimica`
              where ID in (select ParametroId from factura_orden_detalle_temporal_parametros 
              where FacturaDetalleTemporalId = @_id)
              ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_id", id)
        End With

        da = New MySqlDataAdapter(cmd)
        da.Fill(dt)
        conn.Close()


        Return dt

    End Function

    Public Function ExamenParametrosSeleccionadosHematologia(id As Integer) As DataTable

        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = "SELECT Codigo FROM `estructura_examen_hematologia`
              where ID in (select ParametroId from factura_orden_detalle_temporal_parametros 
              where FacturaDetalleTemporalId = @_id)
              ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_id", id)
        End With

        da = New MySqlDataAdapter(cmd)
        da.Fill(dt)
        conn.Close()


        Return dt

    End Function

    Public Function ExamenParametrosSeleccionadosHematozoario(id As Integer) As DataTable

        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = "SELECT ID FROM `estructura_examen_hematozoario`
              where ID in (select ParametroId from factura_orden_detalle_temporal_parametros 
              where FacturaDetalleTemporalId = @_id)
              ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_id", id)
        End With

        da = New MySqlDataAdapter(cmd)
        da.Fill(dt)
        conn.Close()


        Return dt

    End Function

    Public Function ExamenParametrosSeleccionadosInmunoserologia(id As Integer) As DataTable

        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = "SELECT Codigo FROM `estructura_examen_inmunoserologia`
              where ID in (select ParametroId from factura_orden_detalle_temporal_parametros 
              where FacturaDetalleTemporalId = @_id)
              ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_id", id)
        End With

        da = New MySqlDataAdapter(cmd)
        da.Fill(dt)
        conn.Close()


        Return dt

    End Function

    Public Function ExamenParametrosSeleccionadosPruebasEspeciales(id As Integer) As DataTable

        Dim dt As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = "SELECT Codigo FROM `estructura_examen_pruebasespeciales`
              where ID in (select ParametroId from factura_orden_detalle_temporal_parametros 
              where FacturaDetalleTemporalId = @_id)
              ;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_id", id)
        End With

        da = New MySqlDataAdapter(cmd)
        da.Fill(dt)
        conn.Close()


        Return dt

    End Function

#End Region

#Region "Factura"

    Public Function Facturas(fecha_inicial As DateTime, fecha_final As DateTime) As DataTable

        Dim datos As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"SELECT '' As `No`, F.ID, CAI, (if(EsPermanente = True, (Select C.ClieNombre from cliente C where C.ClieId = F.ClienteId), (Select CT.Nombre from cliente_temporal CT where CT.ID = F.ClienteId)) ) NombrePaciente, Fecha, if(EsNula = True, 'Anulada', 'Activa') as Estado, Concat(U.Nombre, ' ', U.Apellido) Usuario, Total
	            FROM factura F
                left join usuarios U on U.Id = F.Usuario 
	            where date(Fecha) between @_fechainicial and @_fechafinal
	            order by ID;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_fechainicial", fecha_inicial.ToString("yyyy-MM-dd"))
            .AddWithValue("@_fechafinal", fecha_final.ToString("yyyy-MM-dd"))
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(datos)
        conn.Close()

        Dim i As Integer = 1

        For Each row As DataRow In datos.Rows
            row("No") = i
            i = i + 1
        Next

        Return datos

    End Function

    Public Function Facturas_Por_Cajero(fecha_inicial As DateTime, fecha_final As DateTime, id_usuario As Integer) As DataTable

        Dim datos As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"SELECT '' As `No`, F.ID, CAI, (if(EsPermanente = True, (Select C.ClieNombre from cliente C where C.ClieId = F.ClienteId), (Select CT.Nombre from cliente_temporal CT where CT.ID = F.ClienteId)) ) NombrePaciente, Fecha, if(EsNula = True, 'Anulada', 'Activa') as Estado, Concat(U.Nombre, ' ', U.Apellido) Usuario, Total
	            FROM factura F
                left join usuarios U on U.Id = F.Usuario 
	            where date(Fecha) between @_fechainicial and @_fechafinal
                and F.Usuario = @_idUsuario 
	            order by ID;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_fechainicial", fecha_inicial.ToString("yyyy-MM-dd"))
            .AddWithValue("@_fechafinal", fecha_final.ToString("yyyy-MM-dd"))
            .AddWithValue("@_idUsuario", id_usuario)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(datos)
        conn.Close()

        Dim i As Integer = 1

        For Each row As DataRow In datos.Rows
            row("No") = i
            i = i + 1
        Next

        Return datos

    End Function

    Public Function Facturar(idFacturaTemporal As Integer) As Integer

        Dim idFactura As Integer = 0

        Try

            conn.Open()

            cmd = New MySqlCommand("factura_facturar", conn) With {
                .CommandType = CommandType.StoredProcedure
            }

            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("_idFacturaTemporal", idFacturaTemporal)
            End With

            cmd.Parameters.Add(New MySqlParameter("@ultima_factura", MySqlDbType.Int32))
            cmd.Parameters("@ultima_factura").Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()


            idFactura = Convert.ToInt32(cmd.Parameters("@ultima_factura").Value)

            conn.Close()

        Catch ex As Exception

            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error Facturar")
            idFactura = 0

            If EstadoConexion() Then
                conn.Close()
            End If

        End Try

        Return idFactura

    End Function

    Public Sub AnularFactura(noFactura As Long, motivo As String, usuario As Integer)

        'Dim idFactura As Long = 0

        Try

            conn.Open()

            cmd = New MySqlCommand("factura_anular", conn) With {
                .CommandType = CommandType.StoredProcedure
            }

            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_cai", noFactura)
                .AddWithValue("@_motivo", motivo)
                .AddWithValue("@_usuario", usuario)
            End With

            'cmd.Parameters.Add(New MySqlParameter("@ultima_factura", MySqlDbType.Int32))
            'cmd.Parameters("@ultima_factura").Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()

            'idFactura = Convert.ToInt32(cmd.Parameters("@ultima_factura").Value)

            conn.Close()

        Catch ex As Exception

            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error Facturar")
            noFactura = 0

            If EstadoConexion() Then
                conn.Close()
            End If

        End Try

    End Sub

    Public Function GetEdad_DesdeFactura(idFactura As String) As DataTable

        Dim odatos As New DataTable
        Dim da As New MySqlDataAdapter
        Dim sql As String = ""

        sql = $"SELECT Mes, Dia FROM factura
                where ID = @_idFactura;"

        conn.Open()

        cmd = New MySqlCommand(sql, conn) With {
        .CommandType = CommandType.Text
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_idFactura", idFactura)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(odatos)
        conn.Close()

        Return odatos

    End Function

    '***************************************************************
    '*****                                                     *****
    '***** Este es tanto para cliente temporal como registrado *****
    '*****                                                     *****
    '***************************************************************
    Public Function ClienteGeneralGetXFacturaId(idFactura As String) As DataTable

        Dim odatos As New DataTable
        Dim da As New MySqlDataAdapter

        conn.Open()

        cmd = New MySqlCommand("clientegeneral_getInfo_desdeFactura", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_idFactura", idFactura)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(odatos)
        conn.Close()

        Return odatos

    End Function

#End Region

#Region "Factura Detalle"
    Public Sub FacturaDetalle_ActualizarEtapa(id As Int64)

        Try

            conn.Open()

            cmd = New MySqlCommand("factura_detalle_actualizar_etapa", conn) With {
                .CommandType = CommandType.StoredProcedure
            }

            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_id", id)
            End With

            cmd.ExecuteNonQuery()

            conn.Close()

        Catch ex As Exception

            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error Facturar")

            If EstadoConexion() Then
                conn.Close()
            End If

        End Try

    End Sub
#End Region

#Region "Cliente"

    Public Function ClienteRegistradoGetXNombre(nombre As String) As DataTable

        Dim odatos As New DataTable
        Dim da As New MySqlDataAdapter

        conn.Open()

        cmd = New MySqlCommand("clienteRegistrado_getXNombre", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_nombre", nombre)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(odatos)
        conn.Close()

        Return odatos

    End Function

    Public Function ClienteRegistradoGetDatosId(id As Integer) As DataTable

        Dim odatos As New DataTable
        Dim da As New MySqlDataAdapter

        conn.Open()

        cmd = New MySqlCommand("clienteRegistrado_getDatosXid", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("_id", id)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(odatos)
        conn.Close()

        Return odatos

    End Function

    Public Function ClienteRegistradoVerificarIdentidad(clienteid As Integer, identidad As String) As Boolean
        Dim verificacion As Boolean = False

        Dim leerdatos As MySqlDataReader

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("clienteRegistrado_ver_identidad", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("_clienteId", clienteid)
            .AddWithValue("_identidad", identidad)
        End With

        leerdatos = cmd.ExecuteReader

        If leerdatos.Read Then
            verificacion = leerdatos("verificacion")
        Else
            verificacion = False
        End If

        conn.Close()


        Return verificacion
    End Function

    Public Function ClienteRegistradoVerificarRTN(clienteid As Integer, rtn As String) As Boolean
        Dim verificacion As Boolean = False

        Dim leerdatos As MySqlDataReader

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("clienteRegistrado_ver_rtn", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("_clienteId", clienteid)
            .AddWithValue("_rtn", rtn)
        End With

        leerdatos = cmd.ExecuteReader

        If leerdatos.Read Then
            verificacion = leerdatos("verificacion")
        Else
            verificacion = False
        End If

        conn.Close()


        Return verificacion
    End Function

    Public Function ClienteRegistradoVerificarPasaporte(clienteid As Integer, pasaporte As String) As Boolean
        Dim verificacion As Boolean = False

        Dim leerdatos As MySqlDataReader

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("clienteRegistrado_ver_pasaporte", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("_clienteId", clienteid)
            .AddWithValue("_pasaporte", pasaporte)
        End With

        leerdatos = cmd.ExecuteReader

        If leerdatos.Read Then
            verificacion = leerdatos("verificacion")
        Else
            verificacion = False
        End If

        conn.Close()


        Return verificacion
    End Function

    Public Sub ClienteRegistrado_insert(nombre As String, sexo As String, edad As Integer,
                                             email As String, telefono As String, identidad As String, rtn As String,
                                             pasaporte As String, extranjero As Boolean, credito As Boolean)

        Try

            conn.Open()

            cmd = New MySqlCommand("clienteRegistrado_insertYsave", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("_accion", "insert")
                .AddWithValue("_clienteId", 0)
                .AddWithValue("_nombre", nombre)
                .AddWithValue("_sexo", sexo)
                .AddWithValue("_edad", edad)
                .AddWithValue("_correo", email)
                .AddWithValue("_telefono", telefono)
                .AddWithValue("_rtn", rtn)
                .AddWithValue("_identidad", identidad)
                .AddWithValue("_pasaporte", pasaporte)
                .AddWithValue("_extranjero", extranjero)
                .AddWithValue("_credito", credito)
            End With

            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            If EstadoConexion() Then
                conn.Close()
            End If

        End Try

    End Sub

    Public Sub ClienteRegistrado_update(clienteid As Long, nombre As String, sexo As String, edad As Integer,
                                             email As String, telefono As String, identidad As String, rtn As String,
                                             pasaporte As String, extranjero As Boolean, credito As Boolean)

        Try

            conn.Open()

            cmd = New MySqlCommand("clienteRegistrado_insertYsave", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("_accion", "save")
                .AddWithValue("_clienteId", clienteid)
                .AddWithValue("_nombre", nombre)
                .AddWithValue("_sexo", sexo)
                .AddWithValue("_edad", edad)
                .AddWithValue("_correo", email)
                .AddWithValue("_telefono", telefono)
                .AddWithValue("_identidad", identidad)
                .AddWithValue("_rtn", rtn)
                .AddWithValue("_pasaporte", pasaporte)
                .AddWithValue("_extranjero", extranjero)
                .AddWithValue("_credito", credito)
            End With

            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            If EstadoConexion() Then
                conn.Close()
            End If

        End Try

    End Sub
#End Region

#Region "Cliente Temporal"
    Public Function ClienteTemporal_insert(nombre As String, rtn As String, genero As String, edad As Integer, email As String,
                                           telefono As String, extranjero As Integer) As Int32

        Dim idClienteTemporal As Int32 = 0

        Try

            conn.Open()

            cmd = New MySqlCommand("cliente_temporal_insertar", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("_nombre", nombre)
                .AddWithValue("_rtn", rtn)
                .AddWithValue("_genero", genero)
                .AddWithValue("_edad", edad)
                .AddWithValue("_email", email)
                .AddWithValue("_telefono", telefono)
                .AddWithValue("_extranjero", extranjero)
                cmd.Parameters.Add(New MySqlParameter("@_idClienteTemporal", MySqlDbType.Int32))
                cmd.Parameters("@_idClienteTemporal").Direction = ParameterDirection.Output
            End With

            cmd.ExecuteNonQuery()
            conn.Close()

            idClienteTemporal = cmd.Parameters("@_idClienteTemporal").Value


        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            If EstadoConexion() Then
                conn.Close()
            End If

            idClienteTemporal = 0
        End Try

        Return idClienteTemporal

    End Function
#End Region

#Region "Seguridad"
    Public Sub ResetPassword(passActual As String, passNueva As String)

        Dim msj As String = ""

        Dim opass As String = oencriptar.StringEncryptarECryptography(passActual, 0)
        Dim query As String = "SELECT Pass as Contra FROM usuarios"

        Dim verificacion As String = ""
        Dim leerdatos As MySqlDataReader

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }

            'With cmd.Parameters
            '    .AddWithValue("", nombre)
            'End With

            leerdatos = cmd.ExecuteReader

            If leerdatos.Read Then
                verificacion = leerdatos("Contra")
            End If

            conn.Close()

            If verificacion.Equals(opass) Then
                conn.Open()
                'MsgBox(opass)

                query = "UPDATE usuarios SET Pass = @passNew WHERE (Id = 1)"
                Dim opass1 As String = oencriptar.StringEncryptarECryptography(passNueva, 0)

                cmd = New MySqlCommand(query, conn) With {
                    .CommandType = CommandType.Text
                    }
                With cmd.Parameters
                    cmd.Parameters.Clear()
                    .AddWithValue("@passNew", opass1)
                End With

                cmd.ExecuteNonQuery()

                conn.Close()

                msj = "Contraseña Actualizada."

            Else
                msj = "Contraseña Incorrecta."
            End If


        Catch ex As Exception
            conn.Close()
            msj = ""
        End Try

    End Sub

    Public Function SetPasswordDefault(idUsuario As Integer) As String

        Dim oencriptar As New Encriptar
        Dim msj = "", query As String = ""

        Dim pass As String = oencriptar.StringEncryptarECryptography("Temporal+10000", 0)

        Try
            conn.Open()

            query = "UPDATE usuarios SET Pass = @_pass WHERE (Id = @_idusuario);"

            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }

            cmd.Parameters.Clear()
            With cmd.Parameters
                .AddWithValue("@_idusuario", idUsuario)
                .AddWithValue("@_pass", pass)
            End With

            cmd.ExecuteNonQuery()

            conn.Close()

            msj = "Contraseña Actualizada."


        Catch ex As Exception
            msj = "Contraseña Incorrecta."
            conn.Close()
        End Try

        Return msj

    End Function

    Public Function PassVerificacion(user As String, pass As String) As Object()

        Dim objetos As Object() = {}
        Dim opass As String = oencriptar.StringEncryptarECryptography(pass, 0)
        Dim query As String = "SELECT Id, Pass as Contra FROM usuarios where User = @user and Pass = @pass"

        Dim id As Integer = 0
        Dim verificacion As Boolean = False
        Dim getPass As String = ""
        Dim leerdatos As MySqlDataReader

        Try

            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }

            With cmd.Parameters
                .AddWithValue("@user", user)
                .AddWithValue("@pass", opass)
            End With

            leerdatos = cmd.ExecuteReader

            If leerdatos.Read Then
                id = leerdatos("Id")
                getPass = leerdatos("Contra")
            End If

            conn.Close()
            If id > 0 Then

                If getPass.Equals(opass) Then

                    verificacion = True

                Else

                    verificacion = False

                End If

            Else
                verificacion = False
            End If


            objetos = {id, verificacion}

        Catch ex As Exception
            conn.Close()
            id = 0
            verificacion = False
            objetos = {id, verificacion}
            MessageBox.Show("Poblema Seguridad" + ex.Message())
        End Try

        Return objetos

    End Function
#End Region

#Region "Hoja Requisicion"

    'Insertar'
    Public Function Insertar(fecha As Date, clienteId As Integer, identidad As String, pasaporte As String, rtn As String, nombre As String, sexo As String, años As Integer, meses As Integer, dias As Integer, tel As String, correo As String, extranj As Integer, factura_id As Int32)

        Dim hojarequisicionid As Integer 'guarda el id recien ingresado'

        Try
            conn.Open()

            cmd = New MySqlCommand("hoja_requisicion_insertar", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@fecha", Format(fecha, "yyyy-MM-dd"))
                .AddWithValue("@id_cliente", clienteId)
                .AddWithValue("@identidad", identidad)
                .AddWithValue("@pasaporte", pasaporte)
                .AddWithValue("@rtn", rtn)
                .AddWithValue("@nombre", nombre)
                .AddWithValue("@sexo", sexo)
                .AddWithValue("@edad", años)
                .AddWithValue("@mes", meses)
                .AddWithValue("@dia", dias)
                .AddWithValue("@tel", tel)
                .AddWithValue("@extranjero", extranj)
                .AddWithValue("@correo", correo)
                .AddWithValue("@factura_id", factura_id)
            End With

            cmd.Parameters.Add(New MySqlParameter("@id", MySqlDbType.Int32))
            cmd.Parameters("@id").Direction = ParameterDirection.Output

            cmd.ExecuteNonQuery()
            conn.Close()

            hojarequisicionid = cmd.Parameters("@id").Value
        Catch ex As Exception
            MsgBox("Problemas de conexión." + ex.ToString(), MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

        Return hojarequisicionid

    End Function


    Public Function ObtDatosHoja(id As Integer, fecha As Date)
        Dim otabla As New DataTable

        conn.Open()

        cmd = New MySqlCommand("datoshoja_requisicion", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@hojaid", id)
            .AddWithValue("@fecha", Format(fecha, "yyyy-MM-dd"))
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(otabla)
        conn.Close()

        Return otabla

    End Function

    Public Function HojaRequisicion_Factura_Verificar(fecha As Date, idFactura As Int32) As DataTable

        Dim otabla As New DataTable
        Dim verificacion As Boolean = False
        Dim verificacion_hojarequisicion As Integer = 0
        conn.Open()

        cmd = New MySqlCommand("hoja_requisicion_verificar", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@fecha", Format(fecha, "yyyy-MM-dd"))
            .AddWithValue("@factura", idFactura)
        End With

        da = New MySqlDataAdapter(cmd)
        da.Fill(otabla)

        conn.Close()

        Return otabla

    End Function

#End Region

#Region "Programation Old"
    Public Function ExamenGetObservacion(id As Integer)

        Dim oObservacion As String = ""

        Try

            conn.Open()
            cmd = New MySqlCommand("examen_getObservacion", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd
                .Parameters.Clear()
                .Parameters.AddWithValue("@_id", id)
                .Parameters.Add(New MySqlParameter("@_observacion", MySqlDbType.String))
                .Parameters("@_observacion").Direction = ParameterDirection.Output
            End With

            cmd.ExecuteNonQuery()

            conn.Close()
            oObservacion = cmd.Parameters("@_observacion").Value.ToString

        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

        Return oObservacion

    End Function

    Public Sub UpdateExamenParametros(opcion As Integer, id As Long, etiq As String, result As String, refer As String)
        Try
            conn.Open()
            cmd = New MySqlCommand("update_parametros", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@opcion", opcion)
                .AddWithValue("@id", id)
                .AddWithValue("@etiq", etiq)
                .AddWithValue("@result", result)
                .AddWithValue("@refer", refer)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Problemas de conexión en Update Parametros.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub

    Public Sub UpdateExamenNombre(nombre As String, id As Long)
        Try
            conn.Open()
            cmd = New MySqlCommand("update_examen_nombre", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@nombre", nombre)
                .AddWithValue("@examen", id)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión en Update Nombre.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub

    Public Sub UpdateExamenObservacion(examId As Long, observacion As String)
        Try
            conn.Open()
            cmd = New MySqlCommand("update_examen_observacion", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_examId", examId)
                .AddWithValue("@_observacion", observacion)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión en Update Nombre.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub

    Public Sub UpdateExamenMembretado(examId As Long, membretado As Integer)
        Try
            conn.Open()
            cmd = New MySqlCommand("update_examen_membretada", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_examId", examId)
                .AddWithValue("@_membretada", membretado)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de Actualización.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub

    Public Sub UpdateExamenNombreHematologia(nombre As String, id As Long)
        Try
            conn.Open()
            cmd = New MySqlCommand("update_nombre_hemograma", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@nombre", nombre)
                .AddWithValue("@examen", id)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión en Update Nombre.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub

    Public Sub UpdateExamenNombreMicrobiologia(nombre As String, id As Long)
        Try
            conn.Open()
            cmd = New MySqlCommand("update_nombre_microbiologia", conn) With {
                .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@nombre", nombre)
                .AddWithValue("@examen", id)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión en Update Nombre.", MsgBoxStyle.Information, "Error")
        End Try
    End Sub





    Public Sub Insertar_temp()

        Try
            conn.Open()
            Dim query As String = "select insertar();"
            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }
            cmd.ExecuteNonQuery()
            conn.Close()

        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub

    'Fin Insertar'

    Public Function tipoexamen(opcion As String, nombre As String, categoria As String)
        Try
            Dim otable As New DataTable
            conn.Open()

            Select Case opcion
                Case "verificar"
                    cmd = New MySqlCommand("tipoexamen_proces", conn) With {
                .CommandType = CommandType.StoredProcedure
                }
                    With cmd.Parameters
                        cmd.Parameters.Clear()
                        .AddWithValue("@opcion", opcion)
                        .AddWithValue("@nombre", nombre)
                        .AddWithValue("@categoria", categoria)
                    End With
                    da = New MySqlDataAdapter(cmd)
                    da.Fill(otable)
                    conn.Close()
                    If otable.Rows.Count > 0 Then
                        Return otable.Rows(0).Item(0)
                    Else
                        Return False
                    End If

                Case "insertar"
                    cmd = New MySqlCommand("tipoexamen_proces", conn) With {
                .CommandType = CommandType.StoredProcedure
                }
                    With cmd.Parameters
                        cmd.Parameters.Clear()
                        .AddWithValue("@opcion", opcion)
                        .AddWithValue("@nombre", nombre)
                        .AddWithValue("@categoria", categoria)
                    End With
                    cmd.ExecuteNonQuery()
                    conn.Close()

            End Select

        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return 0
    End Function

    Public Function CrearExamen(tipo As Integer, observacion As String, membretado As Integer)
        Dim ExamenId As Integer
        Try
            conn.Open()
            cmd = New MySqlCommand("examen_insertar", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_tipo", tipo)
                .AddWithValue("@_observacion", observacion)
                .AddWithValue("@_membretado", membretado)
            End With
            cmd.Parameters.Add(New MySqlParameter("@examenId", MySqlDbType.Int32))
            cmd.Parameters("@examenId").Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            conn.Close()
            ExamenId = cmd.Parameters("@examenId").Value
        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return ExamenId
    End Function

    Public Function ConectarExamen_Hoja(hojaId As Integer, fecha As Date, examenId As Long)
        Try
            conn.Open()
            cmd = New MySqlCommand("hoja_examen", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@id", hojaId)
                .AddWithValue("@fecha", fecha)
                .AddWithValue("@examen", examenId)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return 0
    End Function

    Public Function Examen_Parametros_Vista(id As Long)
        Dim otabla As New DataTable

        Try
            conn.Open()
            cmd = New MySqlCommand("vista_examen_parametros", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@id", id)
            End With
            da = New MySqlDataAdapter(cmd)
            da.Fill(otabla)
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return otabla
    End Function

    Public Function Examen_Parametros_EdicionVista(id As Long)
        Dim otabla As New DataTable

        Try
            conn.Open()
            cmd = New MySqlCommand("vista_examenes_parametros_edicion", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@id", id)
            End With
            da = New MySqlDataAdapter(cmd)
            da.Fill(otabla)
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return otabla
    End Function

    Public Function Examen_Parametros_VistaDS(id As Long)
        Dim otabla As New DataSet

        Try
            conn.Open()
            cmd = New MySqlCommand("vista_examen_parametros", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@id", id)
            End With
            da = New MySqlDataAdapter(cmd)
            da.Fill(otabla, "Parametros")
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return otabla
    End Function

    Public Function ExamenParametros_insertar(id As Long, etiq As String, result As String, refere As String)
        Try
            conn.Open()
            cmd = New MySqlCommand("examen_parametros_insertar", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@id", id)
                .AddWithValue("@etiqueta", etiq)
                .AddWithValue("@resultado", result)
                .AddWithValue("@referencia", refere)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return 0
    End Function

    Public Function VistaExamRealizados(fecha As String)
        Dim otabla As New DataTable

        Try

            conn.Open()

            cmd = New MySqlCommand("vista_examenes", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@fecha", fecha)
            End With
            da = New MySqlDataAdapter(cmd)
            da.Fill(otabla)
            conn.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        Return otabla
    End Function

    Public Function VistaExamRealizadosDS(fecha As String)
        Dim otabla As New DataSet

        conn.Open()

        cmd = New MySqlCommand("vista_examenes", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@fecha", fecha)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(otabla, "Examenes")
        conn.Close()

        Return otabla
    End Function

    Public Function VistaExamRealizadosXnombre(nombre As String)
        Dim otabla As New DataTable

        conn.Open()

        cmd = New MySqlCommand("vista_examenes_xnombre", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_nombre", nombre)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(otabla)
        conn.Close()

        Return otabla
    End Function

    Public Function VistaExamRealizadosXnombreDS(nombre As String)
        Dim otabla As New DataSet

        conn.Open()

        cmd = New MySqlCommand("vista_examenes_xnombre", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_nombre", nombre)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(otabla, "Examenes")
        conn.Close()

        Return otabla
    End Function

    Public Function VistaExamRealizadosXdetalle(no As Integer, fecha As Date)
        Dim otabla As New DataTable

        conn.Open()

        cmd = New MySqlCommand("vista_examenes_detalle", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_no", no)
            .AddWithValue("@_fecha", Format(fecha, "yyyy-MM-dd"))
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(otabla)
        conn.Close()

        Return otabla
    End Function

    Public Function VistaExamRealizadosXdetalleDS(no As Integer, fecha As Date)
        Dim otabla As New DataSet

        conn.Open()

        cmd = New MySqlCommand("vista_examenes_detalle", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_no", no)
            .AddWithValue("@_fecha", Format(fecha, "yyyy-MM-dd"))
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(otabla, "Examenes")
        conn.Close()

        Return otabla
    End Function


    Public Function ExamenesXHoja(hoja As Long, fecha As Date)
        Dim otabla As New DataTable

        conn.Open()

        cmd = New MySqlCommand("examenes_x_hoja", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@hoja", hoja)
            .AddWithValue("@fecha", fecha)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(otabla)
        conn.Close()

        Return otabla
    End Function


    Public Function datos_temp(id As Long, etiqueta As String)
        Try
            conn.Open()
            cmd = New MySqlCommand("lista_a_columnas", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@examenId", id)
                .AddWithValue("@etiqueta", etiqueta)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return 0
    End Function

    Public Function limpiar_tb_temp()
        Try
            conn.Open()
            cmd = New MySqlCommand("limpiar_tb_temp", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return 0
    End Function

    Public Function Actualizar_Cliente(hojaid As Integer, fecha As Date, nombre As String, sexo As String,
                                       edad As Integer, correo As String, tel As String, fechaemision As Date)
        Try
            conn.Open()
            cmd = New MySqlCommand("actualizacion_cliente", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@hojaid", hojaid)
                .AddWithValue("@hojafecha", Format(fecha, "yyyy-MM-dd"))
                .AddWithValue("@fechaemision", Format(fechaemision, "yyyy-MM-dd"))
                .AddWithValue("@nombre", nombre)
                .AddWithValue("@sexo", sexo)
                .AddWithValue("@edad", edad)
                .AddWithValue("@correo", correo)
                .AddWithValue("@tel", tel)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            MsgBox(ex.ToString)
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return 0
    End Function

    Public Sub ClienteActualizar(id As Integer, identidad As String, nombre As String,
                                 sexo As String, edad As Integer, correo As String,
                                 tel As String, pasaporte As String, extranjero As Integer)
        Try
            conn.Open()
            cmd = New MySqlCommand("cliente_actualizar", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@id_cliente", id)
                .AddWithValue("@identidad", identidad)
                .AddWithValue("@nombre", nombre)
                .AddWithValue("@sexo", sexo)
                .AddWithValue("@edad", edad)
                .AddWithValue("@correo", correo)
                .AddWithValue("@tel", tel)
                .AddWithValue("@pasaporte", pasaporte)
                .AddWithValue("@extranjero", extranjero)
            End With
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            MsgBox("Problemas de conexión con cliente.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    'Configuracion de Correo'
    Public Function ObtenerDatosCorreo()
        Dim otabla As New DataTable

        conn.Open()

        cmd = New MySqlCommand("correo_obtenerdatos", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(otabla)
        conn.Close()

        Return otabla
    End Function

    Public Sub ModificarDatosCorreo(correo As String, pass As String, host As String, puerto As String, asunto As String, body As String)
        Dim otabla As New DataTable
        Dim opass As String = oencriptar.StringEncryptarECryptography(pass, 0)

        Try
            conn.Open()
            'MsgBox(opass)

            cmd = New MySqlCommand("correo_modificarcontrasena", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@correo", correo)
                .AddWithValue("@pass", opass)
                .AddWithValue("@ohost", host)
                .AddWithValue("@puerto", puerto)
                .AddWithValue("@asunto", asunto)
                .AddWithValue("@body", body)
            End With
            cmd.ExecuteNonQuery()

            conn.Close()
        Catch ex As Exception
            conn.Close()
            MsgBox(ex.ToString)
        End Try

        'Return otabla
    End Sub

#Region "Formato Examenes"

    'verificacion de existencia
    Public Function FormatoExamenVerificacion(nombre As String) As Integer

        Dim verificacion As Integer = 0
        Dim leerdatos As MySqlDataReader

        nombre = Trim(nombre)

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_verificacion", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("_nombre", nombre)
        End With

        leerdatos = cmd.ExecuteReader

        If leerdatos.Read Then
            verificacion = leerdatos("verificacion")
        End If

        conn.Close()

        Return verificacion

    End Function

    Public Function FormatoExamenVerificacionUpdate(id As Integer, nombre As String) As Integer

        Dim verificacion As Integer = 0
        Dim leerdatos As MySqlDataReader

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_cambiarnombre_update", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("_id", id)
            .AddWithValue("_nombre", nombre)
        End With

        leerdatos = cmd.ExecuteReader

        If leerdatos.Read Then
            verificacion = leerdatos("verificacion")
        End If

        conn.Close()

        Return verificacion

    End Function

    Public Function ClienteVerificarIdentidad(identidad As String) As Integer
        Dim verificacion As Integer = 0

        Dim leerdatos As MySqlDataReader

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("cliente_ver_identidad", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("identidad", identidad)
        End With

        leerdatos = cmd.ExecuteReader

        If leerdatos.Read Then
            verificacion = leerdatos("verificacion")
        End If

        conn.Close()


        Return verificacion
    End Function

    Public Function ClienteVerificarPasaporte(pasaporte As String) As Integer
        Dim verificacion As Integer = 0

        Dim leerdatos As MySqlDataReader

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("cliente_ver_pasaporte", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("@pasaporte", pasaporte)
        End With

        leerdatos = cmd.ExecuteReader

        If leerdatos.Read Then
            verificacion = leerdatos("verificacion")
        End If

        conn.Close()


        Return verificacion
    End Function

    Public Function ClienteGetDatosIdentidad(identidad As String) As DataTable

        Dim odatos As New DataTable

        conn.Open()

        cmd = New MySqlCommand("cliente_getDatos", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("identidad", identidad)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(odatos)
        conn.Close()

        Return odatos

    End Function

    Public Function ClienteGetDatosXpasaporte(pasaporte As String) As DataTable

        Dim odatos As New DataTable

        conn.Open()

        cmd = New MySqlCommand("cliente_getDatosXpasaporte", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("pasaporte", pasaporte)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(odatos)
        conn.Close()

        Return odatos

    End Function



    Public Function ClienteGetDatosTodos(consulta As String, extranjero As Integer) As DataTable

        Dim odatos As New DataTable

        conn.Open()

        cmd = New MySqlCommand("cliente_getDatosTodos", conn) With {
        .CommandType = CommandType.StoredProcedure
        }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("consulta", consulta)
            .AddWithValue("extranjero", extranjero)
        End With
        da = New MySqlDataAdapter(cmd)
        da.Fill(odatos)
        conn.Close()

        Return odatos

    End Function

    'Ingresar formato
    Public Function FormatoExamenInsert(nombre As String, nombre_corto As String, observacion As Integer,
                                   ceid As Integer, obsDefault As String, membretado As Integer) As Integer
        Dim idFormatoLast As Integer = 0

        Try



            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand("formato_examen_insert", conn) With {
            .CommandType = CommandType.StoredProcedure
            }

            With cmd.Parameters
                .AddWithValue("@_nombre", nombre)
                .AddWithValue("@_nombre_corto", nombre_corto)
                .AddWithValue("@_observacion", observacion)
                .AddWithValue("@_ceid", ceid)
                .AddWithValue("@_obsDefault", obsDefault)
                .AddWithValue("@_membretado", membretado)
            End With

            cmd.Parameters.Add(New MySqlParameter("@_idFormatoLast", MySqlDbType.Int32))
            cmd.Parameters("@_idFormatoLast").Direction = ParameterDirection.Output

            cmd.ExecuteNonQuery()
            conn.Close()

            If Not IsDBNull(cmd.Parameters("@_idFormatoLast").Value) Then
                idFormatoLast = cmd.Parameters("@_idFormatoLast").Value
            Else
                idFormatoLast = 0
            End If

        Catch ex As Exception
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
            idFormatoLast = 0
            MsgBox("Error al ingresar el nuevo formato.")
        End Try

        Return idFormatoLast

    End Function

    Public Sub FormatoExamenParamInsert(id As Integer, label As String, vdefault As String,
                                        valor As Integer, referencia As Integer, refDefault As String)

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_param_insert", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("@_id", id)
            .AddWithValue("@_label", label)
            .AddWithValue("@_vdefault", vdefault)
            .AddWithValue("@_valor", valor)
            .AddWithValue("@_referencia", referencia)
            .AddWithValue("@_refDefault", refDefault)
        End With

        cmd.ExecuteNonQuery()
        conn.Close()

    End Sub

    'Actualizar formato
    Public Sub FormatoExamenUpdate(id As Integer, nombre As String, nombreCorto As String, activo As Integer, observacion As Integer,
                                   ceid As Integer, obsDefault As String, membretado As Integer)

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_update", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("@_id", id)
            .AddWithValue("@_nombre", nombre)
            .AddWithValue("@_nombre_corto", nombreCorto)
            .AddWithValue("@_activo", activo)
            .AddWithValue("@_observacion", observacion)
            .AddWithValue("@_ceid", ceid)
            .AddWithValue("@_obsDefault", obsDefault)
            .AddWithValue("@_membretado", membretado)
        End With

        cmd.ExecuteNonQuery()
        conn.Close()

    End Sub

    Public Sub FormatoExamenParamUpdate(id As Integer, label As String, valor As Integer, referencia As Integer,
                                        valDefault As String, refDeault As String)

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_param_update", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("@_id", id)
            .AddWithValue("@_label", label)
            .AddWithValue("@_valor", valor)
            .AddWithValue("@_referencia", referencia)
            .AddWithValue("@_valDefault", valDefault)
            .AddWithValue("@_refDefault", refDeault)
        End With

        cmd.ExecuteNonQuery()
        conn.Close()

    End Sub

    'Buscar formato
    Public Function FormatoExamenBuscar(consulta As String) As DataTable
        Dim dt = New DataTable
        Dim Adaptador As MySqlDataAdapter
        Dim datos As New DataTable

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_buscar", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_consulta", consulta)
        End With
        'cmd.ExecuteNonQuery()
        datos.Clear()
        Adaptador = New MySqlDataAdapter(cmd)
        Adaptador.Fill(datos)

        conn.Close()

        Return datos
    End Function

    'Buscar formato parecidos
    Public Function FormatoExamenParecidos(consulta As String) As DataTable
        Dim dt = New DataTable
        Dim Adaptador As MySqlDataAdapter
        Dim datos As New DataTable

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_parecidos", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_consultar", consulta)
        End With
        'cmd.ExecuteNonQuery()
        datos.Clear()
        Adaptador = New MySqlDataAdapter(cmd)
        Adaptador.Fill(datos)

        conn.Close()

        Return datos
    End Function

    'Verificar si es membretado'
    Public Function EsMembretado(consulta As String) As DataTable
        Dim dt = New DataTable
        Dim Adaptador As MySqlDataAdapter
        Dim datos As New DataTable


        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_esmembretado", conn) With {
                .CommandType = CommandType.StoredProcedure
                }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_consulta", consulta)
        End With
        'cmd.ExecuteNonQuery()
        datos.Clear()
        Adaptador = New MySqlDataAdapter(cmd)
        Adaptador.Fill(datos)

        conn.Close()




        Return datos
    End Function

    'Verificar si es membretado'
    Public Function EsMembretadoExamen(examen As Integer) As Integer
        Dim dr As MySqlDataReader
        Dim membretado As Integer = 0

        Try
            conn.Open()

            cmd.Parameters.Clear()

            cmd = New MySqlCommand("examen_esmembretado", conn) With {
                    .CommandType = CommandType.StoredProcedure
                    }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@_examen", examen)
            End With
            'cmd.ExecuteNonQuery()
            dr = cmd.ExecuteReader

            If dr.Read Then
                membretado = dr("EsM")
            End If

            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If

            MessageBox.Show("Error")
        End Try


        Return membretado
    End Function

    'Get Id'
    Public Function FormatoExamenGetId(consulta As String) As DataTable
        Dim dt = New DataTable
        Dim Adaptador As MySqlDataAdapter
        Dim datos As New DataTable

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_getid", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_consulta", consulta)
        End With
        'cmd.ExecuteNonQuery()
        datos.Clear()
        Adaptador = New MySqlDataAdapter(cmd)
        Adaptador.Fill(datos)

        conn.Close()

        Return datos
    End Function

    'Get datos'
    Public Function FormatoExamenGetDatos(id As Integer, opcion As Integer) As DataTable
        Dim dt = New DataTable
        Dim Adaptador As MySqlDataAdapter
        Dim datos As New DataTable

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_datos", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_id", id)
            .AddWithValue("@_opcion", opcion)
        End With
        'cmd.ExecuteNonQuery()
        datos.Clear()
        Adaptador = New MySqlDataAdapter(cmd)
        Adaptador.Fill(datos)

        conn.Close()

        Return datos
    End Function

    Public Sub FormatoExamenParamDel(id As Integer)

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_examen_param_del", conn) With {
        .CommandType = CommandType.StoredProcedure
        }

        With cmd.Parameters
            .AddWithValue("@_id", id)
        End With

        cmd.ExecuteNonQuery()
        conn.Close()

    End Sub

    Public Function FormatoExamenGetFormatosExamenesActivos(ceid As Integer) As DataTable
        Dim dt = New DataTable
        Dim Adaptador As MySqlDataAdapter
        Dim datos As New DataTable

        conn.Open()

        cmd.Parameters.Clear()

        cmd = New MySqlCommand("formato_ls_examenesxcoviduotros", conn) With {
            .CommandType = CommandType.StoredProcedure
            }
        With cmd.Parameters
            cmd.Parameters.Clear()
            .AddWithValue("@_ceId", ceid)
        End With
        'cmd.ExecuteNonQuery()
        datos.Clear()
        Adaptador = New MySqlDataAdapter(cmd)
        Adaptador.Fill(datos)

        conn.Close()

        Return datos
    End Function
#End Region

#Region "TipoExamen"

    Public Function TipoExamenGetDatos(consulta As String)

        Try
            Dim otabla As New DataTable

            conn.Open()

            Dim query As String

            If consulta = String.Empty Then

                query = "SELECT TiExaNombre, Precio, TiExaId FROM tipoexamen where Estado = 1 limit 30;"

            Else

                query = "SELECT TiExaNombre, Precio, TiExaId FROM tipoexamen where TiExaNombre regexp @consulta and Estado = 1 limit 30;"

            End If


            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }
            With cmd
                .Parameters.Clear()
                .Parameters.AddWithValue("@consulta", consulta)
            End With
            da = New MySqlDataAdapter(cmd)
            da.Fill(otabla)
            conn.Close()

            Return otabla

            'cmd.ExecuteNonQuery()

            conn.Close()

        Catch ex As Exception
            MsgBox("Problemas de conexión.", MsgBoxStyle.Critical, "Error")
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

        Return 0

    End Function

    Public Sub TipoExamenUpdatePrecio(id As Integer, precio As Double)

        Try
            conn.Open()
            Dim query As String = "UPDATE `tipoexamen` SET " +
                "`Precio` = @precio " +
                "WHERE `TiExaId` = @id;"
            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@id", id)
                .AddWithValue("@precio", precio)
            End With
            cmd.ExecuteNonQuery()

            conn.Close()
        Catch ex As Exception
            conn.Close()
        End Try

    End Sub

    Public Sub TipoExamenDesactivar(id As Integer)

        Try
            conn.Open()
            Dim query As String = "UPDATE `tipoexamen` SET " +
                "`Estado` = 0 " +
                "WHERE `TiExaId` = @id;"
            cmd = New MySqlCommand(query, conn) With {
            .CommandType = CommandType.Text
            }
            With cmd.Parameters
                cmd.Parameters.Clear()
                .AddWithValue("@id", id)
            End With
            cmd.ExecuteNonQuery()

            conn.Close()
        Catch ex As Exception
            conn.Close()
        End Try

    End Sub

#End Region

#End Region

End Class
