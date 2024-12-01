
namespace APITeste.Models;

public partial class CadCliente
{
    public int CdCliente { get; set; }

    public DateTime? DtCriacao { get; set; }

    public string? NmCliente { get; set; }

    public virtual ICollection<CadPedido> CadPedidos { get; set; } = new List<CadPedido>();
}
