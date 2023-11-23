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
    public class PedidoController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JardineriaContext _context;

        public PedidoController(IUnitOfWork unitOfWork, IMapper mapper, JardineriaContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> Get()
        {
            var nombreVariable = await _unitOfWork.Pedidos.GetAllAsync();
            return _mapper.Map<List<PedidoDto>>(nombreVariable);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoDto>> Get(int id)
        {
            var nombreVariable = await _unitOfWork.Pedidos.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            return _mapper.Map<PedidoDto>(nombreVariable);
        }

        //      Devuelve un listado con el código de pedido, código de cliente, fecha
        //      esperada y fecha de entrega de los pedidos que no han sido entregados a
        //      tiempo.

        [HttpGet("GetPedidosRetrasados")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PedidoDto>> Post(PedidoDto PedidoDto)
        {
            var nombreVariable = _mapper.Map<Pedido>(PedidoDto);
            this._unitOfWork.Pedidos.Add(nombreVariable);
            await _unitOfWork.SaveAsync();

            if (nombreVariable == null)
            {
                return BadRequest();
            }
            PedidoDto.Id = nombreVariable.Id;
            return CreatedAtAction(nameof(Post), new { id = PedidoDto.Id }, PedidoDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoDto>> Put(int id, [FromBody] PedidoDto PedidoDto)
        {
            if (PedidoDto.Id == 0)
            {
                PedidoDto.Id = id;
            }

            if (PedidoDto.Id != id)
            {
                return BadRequest();
            }

            if (PedidoDto == null)
            {
                return NotFound();
            }

            var nombreVariable = _mapper.Map<Pedido>(PedidoDto);
            _unitOfWork.Pedidos.Update(nombreVariable);
            await _unitOfWork.SaveAsync();
            return PedidoDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var nombreVariable = await _unitOfWork.Pedidos.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            _unitOfWork.Pedidos.Remove(nombreVariable);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}