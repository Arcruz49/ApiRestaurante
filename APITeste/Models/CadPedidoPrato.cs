using System;
using System.Collections.Generic;

namespace APITeste.Models;

public partial class CadPedidoPrato
{
    public int CdPedidoPrato { get; set; }

    public int CdPedido { get; set; }

    public int CdPrato { get; set; }

    public int Quantidade { get; set; }

    public virtual CadPedido CdPedidoNavigation { get; set; } = null!;

    public virtual CadCardapio CdPratoNavigation { get; set; } = null!;
}
