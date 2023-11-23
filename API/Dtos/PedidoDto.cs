using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public DateOnly FechaPedido { get; set; }
        public DateOnly FechaEsperada { get; set; }
        public DateOnly FechaEntrega { get; set; }
        public string Estado { get; set; }
        public string Comentarios { get; set; }
        public int IdCliente { get; set; }
    }
}