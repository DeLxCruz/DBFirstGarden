
# DBFirstGarden




## API Reference

#### Devuelve un listado con el código de pedido, código de cliente, fecha esperada y fecha de entrega de los pedidos que no han sido entregados a tiempo.

```http
  GET /api/pedido/GetPedidosRetrasados
```

#### Codigo:
        public async Task<ActionResult<Object>> GetPedidosretrasados()
        {
            var results = await _context.Pedidos
            .Where(p => p.FechaEntrega > p.FechaEsperada || p.FechaEntrega == null)
            .OrderBy(p => p.FechaEsperada)
            .Select(p => new { p.Id, p.CodigoCliente, p.FechaEsperada, p.FechaEntrega })
            .ToListAsync();
            if (results == null)
            {
                return NotFound();
            }
            return Ok(results);
        }

#### Devuelve un listado de los productos que nunca han aparecido en un pedido. El resultado debe mostrar el nombre, la descripción y la imagen del producto.

```http
  GET /api/producto/GetProductosSinPedidos
```
#### Codigo: 
        public async Task<ActionResult<IEnumerable<Object>>> GetProductosSinPedidos()
        {
            var productosSinPedidos = await _context.Productos
            .Where(p => p.DetallePedidos.Count == 0)
            .Select(p => p.Nombre)
            .ToListAsync();

            return productosSinPedidos;
        }

#### Devuelve las oficinas donde no trabajan ninguno de los empleados que hayan sido los representantes de ventas de algún cliente que haya realizado la compra de algún producto de la gama Frutales

```http
  GET /api/oficina/OficinasSinEmpleadosProductosDeLaGamaFrutales
```

#### Código:
        public async Task<ActionResult<IEnumerable<Object>>> GetOficinasSinEmpleadosProductosDeLaGamaFrutales()
        {
            var oficinasSinRepresentantesFrutales = await _context.Oficinas
                  .Where(o => !o.Empleados.Any(e => e.Clientes.Any(c => c.Pedidos.Any(p => p.DetallePedidos.Any(dp => dp.CodigoProductoNavigation.GamaNavigation.DescripcionTexto == "Frutales")))))
                 .Select(o => new { o.Ciudad, o.Pais, o.Region })
                 .ToListAsync();

            return oficinasSinRepresentantesFrutales;
        }

#### Devuelve el nombre, apellidos, puesto y teléfono de la oficina de aquellos empleados que no sean representante de ventas de ningún cliente

```http
  GET /api/oficina/EmpleadosSinClientes
```

#### Codigo: 
        public async Task<ActionResult<IEnumerable<Object>>> GetEmpleadosSinClientes()
        {
            var empleadosSinClientesRepresentados = await _context.Empleados
          .Where(e => e.Id == 0 && e.Clientes.Count == 0)
          .Select(e => new { e.Nombre, e.Apellido1, e.Puesto, e.CodigoOficinaNavigation.Telefono })
          .ToListAsync();

            return empleadosSinClientesRepresentados;
        }

#### Devuelve el nombre del producto del que se han vendido más unidades. (Tenga en cuenta que tendrá que calcular cuál es eI número total de unidades que se han vendido de cada producto a partir de los datos de la tabla detalle_pedido).

```http
  GET /api/producto/GetProductoMasVendido
```

#### Codigo:
        public async Task<ActionResult<IEnumerable<Object>>> GetProductoMasVendido()
        {
            var productoMasVendido = await _context.Productos
            .Where(p => p.DetallePedidos.Sum(dp => dp.Cantidad) == p.DetallePedidos.Sum(dp => dp.Cantidad))
            .Select(p => new { p.Nombre, V = p.DetallePedidos.Sum(dp => dp.Cantidad) })
            .ToListAsync();

            return productoMasVendido;
        }

#### Devuelve un listado de los 20 productos más vendidos y el número total de unidades que se han vendido de cada uno. EI listado deberá estar ordenado por el número total de unidades vendidas.
```http
  GET /api/producto/Get20ProductosMasVendidos
```

#### Codigo:
        public async Task<ActionResult<IEnumerable<Object>>> Get20ProductosMasVendidos()
        {
            var results = await _context.Productos
                .OrderByDescending(p => p.DetallePedidos.Sum(dp => dp.Cantidad))
                .Take(20)
                .Select(p => new
                {
                    NombreProducto = p.Nombre,
                    UnidadesVendidas = p.DetallePedidos.Sum(dp => dp.Cantidad)
                })
                .ToListAsync();

            return Ok(results);

        }

#### Devuelve un listado de las diferentes gamas de producto que ha comprado cada cliente.

```http
  GET /api/cliente/GetClientesGamasProductos
```

#### Codigo: 
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientesGamasProductos()
        {
            var results = await _context.Clientes
                .Join(_context.Pedidos,
                    c => c.Id,
                    p => p.CodigoCliente,
                    (c, p) => new { Cliente = c, Pedido = p })
                .Join(_context.DetallePedidos,
                    cp => cp.Pedido.Id,
                    dp => dp.Id,
                    (cp, dp) => new { cp.Cliente, cp.Pedido, DetallePedido = dp })
                .Join(_context.Productos,
                    cpd => cpd.DetallePedido.CodigoProducto,
                    pr => pr.Id,
                    (cpd, pr) => new { cpd.Cliente, cpd.Pedido, cpd.DetallePedido, Producto = pr })
                .GroupBy(cpdp => new { cpdp.Cliente.Id, cpdp.Cliente.NombreCliente })
                .Select(group => new

                {
                    ClienteNombre = group.Key.NombreCliente,
                    GamasCompradas = string.Join(" ", group.Select(cpdp => cpdp.Producto.Gama).Distinct())

                })
                .ToListAsync();

            return Ok(results);
        }

#### Lista las ventas totales de los productos que hayan facturado más de 3000 euros. Se mostrará el nombre, unidades vendidas, total facturado y total facturado con impuestos (21% IVA).

```http
  GET /api/producto/GetVentasProductosMasDe3000Euros
```

#### Codigo: 

        public async Task<ActionResult<IEnumerable<Object>>> GetVentasProductosMasDe3000Euros()
        {
            var ventasProductosMasDe3000Euros = await _context.Productos
            .Where(p => p.DetallePedidos.Sum(dp => dp.Cantidad * dp.PrecioUnidad) > 3000)
            .Select(p => new
            {
                p.Nombre,
                UnidadesVendidas = p.DetallePedidos.Sum(dp => dp.Cantidad),
                TotalFacturado = p.DetallePedidos.Sum(dp => dp.Cantidad * dp.PrecioUnidad),
                TotalFacturadoConIVA = p.DetallePedidos.Sum(dp => dp.Cantidad * dp.PrecioUnidad * 1.21m)
            })
            .ToListAsync();

            return ventasProductosMasDe3000Euros;

        }

