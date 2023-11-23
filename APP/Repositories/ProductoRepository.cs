using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Persistence.Data;

namespace APP.Repositories
{
    public class ProductoRepository : GenericRepositoryVarchar<Producto>, IProducto
    {
        private readonly JardineriaContext _context;

        public ProductoRepository(JardineriaContext context) : base(context)
        {
            _context = context;
        }
    }
}