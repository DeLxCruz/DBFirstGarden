using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Pago : BaseEntityInt
{
    public string FormaPago { get; set; } = null!;

    public string IdTransaccion { get; set; } = null!;

    public DateOnly FechaPago { get; set; }

    public decimal Total { get; set; }

    public virtual Cliente IdNavigation { get; set; } = null!;
}
