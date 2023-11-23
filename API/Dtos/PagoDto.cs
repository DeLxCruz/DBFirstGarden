using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class PagoDto
    {
        public int Id { get; set; }
        public string FormaPago { get; set; }
        public string IdTransaccion { get; set; }
        public DateOnly FechaPago { get; set; }
        public decimal Total { get; set; }
    }
}