using System;
using System.Collections.Generic;

namespace APITeste.Models;

public partial class CadReserva
{
    public int CdReserva { get; set; }

    public int? CdMesa { get; set; }

    public int? CdCliente { get; set; }

    public string? NmCliente { get; set; }

    public DateTime? DtCriacao { get; set; }

    public virtual CadCliente? CdClienteNavigation { get; set; }

    public virtual CadMesa? CdMesaNavigation { get; set; }
}
