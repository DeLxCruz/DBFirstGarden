using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace API.Dtos
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; }
        public string Telefono { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}