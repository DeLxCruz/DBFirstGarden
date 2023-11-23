using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Persistence.Data;

namespace APP.Repositories
{
    public class DetallePedidoRepository : GenericRepositoryInt<DetallePedido>, IDetallePedido
    {
        private readonly JardineriaContext _context;

        public DetallePedidoRepository(JardineriaContext context) : base(context)
        {
            _context = context;
        }
    }
}