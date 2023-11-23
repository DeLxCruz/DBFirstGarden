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
    public class OficinaController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JardineriaContext _context;

        public OficinaController(IUnitOfWork unitOfWork, IMapper mapper, JardineriaContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<OficinaDto>>> Get()
        {
            var nombreVariable = await _unitOfWork.Oficinas.GetAllAsync();
            return _mapper.Map<List<OficinaDto>>(nombreVariable);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OficinaDto>> Get(string id)
        {
            var nombreVariable = await _unitOfWork.Oficinas.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            return _mapper.Map<OficinaDto>(nombreVariable);
        }

        // Devuelve las oficinas donde no trabajan ninguno de los empleados que hayan sido los representantes de ventas de algún cliente 
        // que haya realizado la compra de algún producto de la gama Frutales

        [HttpGet("OficinasSinEmpleadosProductosDeLaGamaFrutales")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Object>>> GetOficinasSinEmpleadosProductosDeLaGamaFrutales()
        {
            var oficinasSinRepresentantesFrutales = await _context.Oficinas
                  .Where(o => !o.Empleados.Any(e => e.Clientes.Any(c => c.Pedidos.Any(p => p.DetallePedidos.Any(dp => dp.CodigoProductoNavigation.GamaNavigation.DescripcionTexto == "Frutales")))))
                 .Select(o => new { o.Ciudad, o.Pais, o.Region })
                 .ToListAsync();

            return oficinasSinRepresentantesFrutales;
        }

        //      Devuelve el nombre, apellidos, puesto y teléfono de la oficina de aquellos
        //      empleados que no sean representante de ventas de ningún cliente.

        [HttpGet("EmpleadosSinClientes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Object>>> GetEmpleadosSinClientes()
        {
            var empleadosSinClientesRepresentados = await _context.Empleados
          .Where(e => e.Id == 0 && e.Clientes.Count == 0)
          .Select(e => new { e.Nombre, e.Apellido1, e.Puesto, e.CodigoOficinaNavigation.Telefono })
          .ToListAsync();

            return empleadosSinClientesRepresentados;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OficinaDto>> Post(OficinaDto OficinaDto)
        {
            var nombreVariable = _mapper.Map<Oficina>(OficinaDto);
            this._unitOfWork.Oficinas.Add(nombreVariable);
            await _unitOfWork.SaveAsync();

            if (nombreVariable == null)
            {
                return BadRequest();
            }
            OficinaDto.Id = nombreVariable.Id;
            return CreatedAtAction(nameof(Post), new { id = OficinaDto.Id }, OficinaDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OficinaDto>> Put(string id, [FromBody] OficinaDto OficinaDto)
        {
            if (OficinaDto.Id == null)
            {
                OficinaDto.Id = id;
            }

            if (OficinaDto.Id != id)
            {
                return BadRequest();
            }

            if (OficinaDto == null)
            {
                return NotFound();
            }

            var nombreVariable = _mapper.Map<Oficina>(OficinaDto);
            _unitOfWork.Oficinas.Update(nombreVariable);
            await _unitOfWork.SaveAsync();
            return OficinaDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            var nombreVariable = await _unitOfWork.Oficinas.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            _unitOfWork.Oficinas.Remove(nombreVariable);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}