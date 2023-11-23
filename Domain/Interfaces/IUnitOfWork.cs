using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IOficina Oficinas { get; }
        IPago Pagos { get; }
        IPedido Pedidos { get; }
        IProducto Productos { get; }
        ICliente Clientes { get; }
        IEmpleado Empleados { get; }
        IGamaProducto GamaProductos { get; }
        IDetallePedido DetallePedidos { get; }
        Task<int> SaveAsync(); 
    }
}