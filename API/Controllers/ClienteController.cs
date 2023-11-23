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
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace API.Controllers
{
    public class ClienteController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JardineriaContext _context;

        public ClienteController(IUnitOfWork unitOfWork, IMapper mapper, JardineriaContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> Get()
        {
            var nombreVariable = await _unitOfWork.Clientes.GetAllAsync();
            return _mapper.Map<List<ClienteDto>>(nombreVariable);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteDto>> Get(int id)
        {
            var nombreVariable = await _unitOfWork.Clientes.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            return _mapper.Map<ClienteDto>(nombreVariable);
        }

        //       Devuelve el listado de clientes indicando el nombre del cliente y cuántos
        //       pedidos ha realizado. Tenga en cuenta que pueden existir clientes que no
        //       han realizado ningún pedido.

        // [HttpGet("GetClientesPedidos")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientesPedidos()
        // {
        //     var nombreVariable = await _unitOfWork.Clientes.GetAllAsync();
        //     var clientes = _mapper.Map<List<ClienteDto>>(nombreVariable);
        //     var clientesPedidos = new List<ClienteDto>();
        //     foreach (var cliente in clientes)
        //     {
        //         var pedidos = await _unitOfWork.Pedidos.GetAllAsync();
        //         var pedidosCliente = pedidos.Where(p => p.Id == cliente.Id).ToList();
        //         cliente.Pedidos = pedidosCliente.Count;
        //         clientesPedidos.Add(cliente);
        //     }
        //     return clientesPedidos;
        // }

        // Devuelve eI nombre de los clientes a los que no se les ha entregado a tiempo un pedido

        // [HttpGet("GetClientesPedidosRetrasados")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientesPedidosRetrasados()
        // {
        //     var nombreVariable = await _unitOfWork.Clientes.GetAllAsync();
        //     var clientes = _mapper.Map<List<ClienteDto>>(nombreVariable);
        //     var clientesPedidos = new List<ClienteDto>();
        //     foreach (var cliente in clientes)
        //     {
        //         var pedidos = await _unitOfWork.Pedidos.GetAllAsync();
        //         var pedidosCliente = pedidos.Where(p => p.Id == cliente.Id).ToList();
        //         cliente.Pedidos = pedidos.Count();
        //         clientesPedidos.Add(cliente);
        //     }
        //     return clientesPedidos;
        // }

        // Devuelve un listado de las diferentes gamas de producto que ha comprado
        // cada cliente.

        [HttpGet("GetClientesGamasProductos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> GetClientesGamasProductos()
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


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClienteDto>> Post(ClienteDto clienteDto)
        {
            var nombreVariable = _mapper.Map<Cliente>(clienteDto);
            this._unitOfWork.Clientes.Add(nombreVariable);
            await _unitOfWork.SaveAsync();

            if (nombreVariable == null)
            {
                return BadRequest();
            }
            clienteDto.Id = nombreVariable.Id;
            return CreatedAtAction(nameof(Post), new { id = clienteDto.Id }, clienteDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteDto>> Put(int id, [FromBody] ClienteDto clienteDto)
        {
            if (clienteDto.Id == 0)
            {
                clienteDto.Id = id;
            }

            if (clienteDto.Id != id)
            {
                return BadRequest();
            }

            if (clienteDto == null)
            {
                return NotFound();
            }

            var nombreVariable = _mapper.Map<Cliente>(clienteDto);
            _unitOfWork.Clientes.Update(nombreVariable);
            await _unitOfWork.SaveAsync();
            return clienteDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var nombreVariable = await _unitOfWork.Clientes.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            _unitOfWork.Clientes.Remove(nombreVariable);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}