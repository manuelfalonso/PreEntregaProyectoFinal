using Pre_Entrega_Proyecto_Final.Models;
using System.Data.SqlClient;

Console.WriteLine("PRE ENTREGA PROYECTO FINAL");
Console.WriteLine("==========================");

// Connection string
SqlConnectionStringBuilder connectionBuilder = new();
connectionBuilder.DataSource = "DESKTOP-5L9GC2G\\SQLEXPRESS";
connectionBuilder.InitialCatalog = "SistemaGestion";
connectionBuilder.IntegratedSecurity = true;
var cs = connectionBuilder.ConnectionString;

#region Testing
Console.WriteLine($"{TraerUsuario("NombreUsuario").Id}"); // OK
Console.WriteLine($"{TraerProductos(1).Count}"); // OK
Console.WriteLine($"{TraerProductosVendidos(1).Count}"); // OK
Console.WriteLine($"{TraerVentas(3).Count}"); // OK
Console.WriteLine($"{InicioDeSesion("NombreUsuario", "Contraseña").Id}"); // OK
Console.WriteLine($"{InicioDeSesion("NombreUsuario", "mal").Id}"); // OK
#endregion

/// <summary>
/// Recibe como parámetro un nombre del usuario, buscarlo en la base de datos y 
/// devolver el objeto con todos sus datos (Esto se hará para la página en la que 
/// se mostrara los datos del usuario y en la página para modificar sus datos).
/// </summary>
Usuario TraerUsuario(string nombreUsuario)
{
    Console.WriteLine("\nTraerUsuario");

    Usuario usuario = new();

    using (SqlConnection connection = new(cs))
    {
        connection.Open();

        SqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * " +
            "FROM Usuario" +
            " WHERE NombreUsuario = @nombreUsuario";

        // Crear parametro
        var param =
            new SqlParameter("nombreUsuario", System.Data.SqlDbType.VarChar);
        param.Value = nombreUsuario;

        // Agregar el parametro
        cmd.Parameters.Add(param);

        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            usuario.Id = Convert.ToInt32(reader.GetValue(0));
            usuario.Nombre = reader.GetValue(1).ToString();
            usuario.Apellido = reader.GetValue(2).ToString();
            usuario.NombreUsuario = reader.GetValue(3).ToString();
            usuario.Contraseña = reader.GetValue(4).ToString();
            usuario.Mail = reader.GetValue(5).ToString();
        }

        reader.Close();
        connection.Close();
    }

    return usuario;
}

/// <summary>
/// Recibe un número de IdUsuario como parámetro, debe traer todos los productos 
/// cargados en la base de este usuario en particular.
/// </summary>
List<Producto> TraerProductos(int idUsuario)
{
    Console.WriteLine("\nTraerProducto");

    List<Producto> productos = new();

    using (SqlConnection connection = new(cs))
    {
        connection.Open();

        SqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * " +
            "FROM Producto" +
            " WHERE IdUsuario = @idUsuario";

        // Crear parametro
        var param =
            new SqlParameter("idUsuario", System.Data.SqlDbType.BigInt);
        param.Value = idUsuario;

        // Agregar el parametro
        cmd.Parameters.Add(param);

        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var producto = new Producto();

            producto.Id = Convert.ToInt32(reader.GetValue(0));
            producto.Descripciones = reader.GetValue(1).ToString();
            producto.Costo = Convert.ToDouble(reader.GetValue(2));
            producto.PrecioVenta = Convert.ToDouble(reader.GetValue(3));
            producto.Stock = Convert.ToInt32(reader.GetValue(4));
            producto.IdUsuario = Convert.ToInt32(reader.GetValue(5));

            productos.Add(producto);
        }

        reader.Close();
        connection.Close();
    }

    return productos;
}

/// <summary>
/// Traer Todos los productos vendidos de un Usuario, cuya información está en 
/// su producto (Utilizar dentro de esta función el "Traer Productos"
/// anteriormente hecho para saber que productosVendidos ir a buscar).
/// </summary>
List<ProductoVendido> TraerProductosVendidos(int idUsuario)
{
    Console.WriteLine("\nTraerProductosVendidos");

    // Traigo listado completo de productos de un usuario
    List<Producto> productos = TraerProductos(idUsuario);

    // Si el producto existe en la tabla producto vendido lo guardo en la lista
    List<ProductoVendido> productosVendidos = new();

    foreach (Producto producto in productos)
    {
        using (SqlConnection connection = new(cs))
        {
            connection.Open();

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * " +
                "FROM ProductoVendido " +
                "WHERE IdProducto = @idProducto";

            // Crear parametro
            var param =
                new SqlParameter("idProducto", System.Data.SqlDbType.BigInt);
            param.Value = producto.Id;

            // Agregar el parametro
            cmd.Parameters.Add(param);

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var productoVendido = new ProductoVendido();

                productoVendido.Id = Convert.ToInt32(reader.GetValue(0));
                productoVendido.Stock = Convert.ToInt32(reader.GetValue(1));
                productoVendido.IdProducto = Convert.ToInt32(reader.GetValue(2));
                productoVendido.IdVenta = Convert.ToInt32(reader.GetValue(3));

                productosVendidos.Add(productoVendido);
            }

            reader.Close();
            connection.Close();
        }
    }

    return productosVendidos;
}

/// <summary>
/// Recibe como parámetro un IdUsuario, debe traer todas las ventas de la
/// base asignados al usuario particular.
/// </summary>
List<Venta> TraerVentas(int idUsuario)
{
    Console.WriteLine("\nTraerVentas");

    List<Venta> ventas = new();

    using (SqlConnection connection = new(cs))
    {
        connection.Open();

        SqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * " +
            "FROM Venta " +
            "WHERE IdUsuario = @idUsuario";

        // Crear parametro
        var param =
            new SqlParameter("idUsuario", System.Data.SqlDbType.BigInt);
        param.Value = idUsuario;

        // Agregar el parametro
        cmd.Parameters.Add(param);

        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var venta = new Venta();

            venta.Id = Convert.ToInt32(reader.GetValue(0));
            venta.Comentarios = reader.GetValue(1).ToString();
            venta.IdUsuario = Convert.ToInt32(reader.GetValue(2));

            ventas.Add(venta);
        }

        reader.Close();
        connection.Close();
    }

    return ventas;
}

/// <summary>
/// Se le pase como parámetro el nombre del usuario y la contraseña, buscar 
/// en la base de datos si el usuario existe y si coincide con la contraseña 
/// lo devuelve (el objeto Usuario), caso contrario devuelve uno vacío (Con 
/// sus datos vacíos y el id en 0).
/// </summary>
Usuario InicioDeSesion(string nombreUsuario, string contraseña)
{
    Console.WriteLine("\nInicioDeSesion");

    Usuario usuario = TraerUsuario(nombreUsuario);

    if (usuario.Contraseña == contraseña)
        return usuario;
    else
        return new Usuario();
}