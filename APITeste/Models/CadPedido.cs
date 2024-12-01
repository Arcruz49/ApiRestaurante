using System;
using System.Collections.Generic;

namespace APITeste.Models;

public partial class CadPedido
{
    public int CdPedido { get; set; }

    public DateTime? DtCriacao { get; set; }

    public int CdCliente { get; set; }

    public virtual ICollection<CadPedidoPrato> CadPedidoPratos { get; set; } = new List<CadPedidoPrato>();

    public virtual CadCliente CdClienteNavigation { get; set; } = null!;
}
