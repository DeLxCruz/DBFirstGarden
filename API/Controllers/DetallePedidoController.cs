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

namespace API.Controllers
{
    public class DetallePedidoController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DetallePedidoController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DetallePedidoDto>>> Get()
        {
            var nombreVariable = await _unitOfWork.DetallePedidos.GetAllAsync();
            return _mapper.Map<List<DetallePedidoDto>>(nombreVariable);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetallePedidoDto>> Get(int id)
        {
            var nombreVariable = await _unitOfWork.DetallePedidos.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            return _mapper.Map<DetallePedidoDto>(nombreVariable);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DetallePedidoDto>> Post(DetallePedidoDto detallePedidoDto)
        {
            var nombreVariable = _mapper.Map<DetallePedido>(detallePedidoDto);
            this._unitOfWork.DetallePedidos.Add(nombreVariable);
            await _unitOfWork.SaveAsync();

            if (nombreVariable == null)
            {
                return BadRequest();
            }
            detallePedidoDto.Id = nombreVariable.Id;
            return CreatedAtAction(nameof(Post), new { id = detallePedidoDto.Id }, detallePedidoDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetallePedidoDto>> Put(int id, [FromBody] DetallePedidoDto detallePedidoDto)
        {
            if (detallePedidoDto.Id == 0)
            {
                detallePedidoDto.Id = id;
            }

            if(detallePedidoDto.Id != id)
            {
                return BadRequest();
            }

            if(detallePedidoDto == null)
            {
                return NotFound();
            }

            var nombreVariable = _mapper.Map<DetallePedido>(detallePedidoDto);
            _unitOfWork.DetallePedidos.Update(nombreVariable);
            await _unitOfWork.SaveAsync();
            return detallePedidoDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var nombreVariable = await _unitOfWork.DetallePedidos.GetByIdAsync(id);

            if (nombreVariable == null)
            {
                return NotFound();
            }

            _unitOfWork.DetallePedidos.Remove(nombreVariable);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}