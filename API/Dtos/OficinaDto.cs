using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class OficinaDto
    {
        public string Id { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public string Region { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }

    }
}