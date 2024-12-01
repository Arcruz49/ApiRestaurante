using System;
using System.Collections.Generic;

namespace APITeste.Models;

public partial class CadCardapio
{
    public int CdPrato { get; set; }

    public DateTime? DtCriacao { get; set; }

    public string? NmPrato { get; set; }

    public string? DsPrato { get; set; }

    public decimal? Preco { get; set; }

    public virtual ICollection<CadPedidoPrato> CadPedidoPratos { get; set; } = new List<CadPedidoPrato>();
}
