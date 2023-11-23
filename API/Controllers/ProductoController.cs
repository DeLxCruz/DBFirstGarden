using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductoController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JardineriaContext _context;

        public ProductoController(IUnitOfWork unitOfWork, IMapper mapper, JardineriaContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> Get()
        {
            var nombreVariable = await _unitOfWork.Productos.GetAllAsync();
            return _mapper.Map<List<ProductoDto>>(nombreVariable);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDto>> Get(string id)
        {
            var nombreVariable = await _unitOfWork.Productos.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            return _mapper.Map<ProductoDto>(nombreVariable);
        }
        //      Devuelve un listado de los productos que nunca han aparecido en un
        //      pedido. El resultado debe mostrar el nombre, la descripción y la imagen del
        //      producto.

        [HttpGet("GetProductosSinPedidos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Object>>> GetProductosSinPedidos()
        {
            var productosSinPedidos = await _context.Productos
            .Where(p => p.DetallePedidos.Count == 0)
            .Select(p => p.Nombre)
            .ToListAsync();

            return productosSinPedidos;
        }

        //      Lista las ventas totales de los productos que hayan facturado más de 3000
        //      euros. Se mostrará el nombre, unidades vendidas, total facturado y total
        //      facturado con impuestos (21% IVA).

        [HttpGet("GetVentasProductosMasDe3000Euros")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        // Devuelve el nombre del producto del que se han vendido más unidades.
        // (Tenga en cuenta que tendrá que calcular cuál es eI número total de unidades que se han vendido de cada producto a partir de los datos de la tabla detalle_pedido)

        [HttpGet("GetProductoMasVendido")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Object>>> GetProductoMasVendido()
        {
            var productoMasVendido = await _context.Productos
            .Where(p => p.DetallePedidos.Sum(dp => dp.Cantidad) == p.DetallePedidos.Sum(dp => dp.Cantidad))
            .Select(p => new { p.Nombre, V = p.DetallePedidos.Sum(dp => dp.Cantidad) })
            .ToListAsync();

            return productoMasVendido;
        }

        //      Devuelve un listado de los 20 productos más vendidos y el número total de
        //      unidades que se han vendido de cada uno. EI listado deberá estar ordenado
        //      por el número total de unidades vendidas.

        [HttpGet("Get20ProductosMasVendidos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> Post(ProductoDto ProductoDto)
        {
            var nombreVariable = _mapper.Map<Producto>(ProductoDto);
            this._unitOfWork.Productos.Add(nombreVariable);
            await _unitOfWork.SaveAsync();

            if (nombreVariable == null)
            {
                return BadRequest();
            }
            ProductoDto.Id = nombreVariable.Id;
            return CreatedAtAction(nameof(Post), new { id = ProductoDto.Id }, ProductoDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDto>> Put(string id, [FromBody] ProductoDto ProductoDto)
        {
            if (ProductoDto.Id == null)
            {
                ProductoDto.Id = id;
            }

            if (ProductoDto.Id != id)
            {
                return BadRequest();
            }

            if (ProductoDto == null)
            {
                return NotFound();
            }

            var nombreVariable = _mapper.Map<Producto>(ProductoDto);
            _unitOfWork.Productos.Update(nombreVariable);
            await _unitOfWork.SaveAsync();
            return ProductoDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            var nombreVariable = await _unitOfWork.Productos.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            _unitOfWork.Productos.Remove(nombreVariable);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}