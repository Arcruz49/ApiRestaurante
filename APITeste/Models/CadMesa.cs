using System;
using System.Collections.Generic;

namespace APITeste.Models;

public partial class CadMesa
{
    public int CdMesa { get; set; }

    public DateTime? DtCriacao { get; set; }

    public virtual ICollection<CadReserva> CadReservas { get; set; } = new List<CadReserva>();
}
