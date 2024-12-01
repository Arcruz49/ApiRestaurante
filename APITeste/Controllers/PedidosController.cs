using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITeste.Data;
using APITeste.Models;
using APITeste.Models.Resources;

namespace APITeste.Controllers
{
    /// <summary>
    /// Controlador para gerenciar clientes.
    /// </summary>
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly DbRestauranteContext db;

        /// <summary>
        /// Construtor do controlador de pedidos.
        /// </summary>
        public PedidosController(DbRestauranteContext context)
        {
            db = context;
        }

        /// <summary>
        /// Retorna todos os pedidos.
        /// </summary>
        [HttpGet("GetPedidos")]
        public async Task<ActionResult<IEnumerable<ResourcePedido>>> GetClientes()
        {
            var pedidos = await (from cliente in db.CadClientes
                                 join pedido in db.CadPedidos on cliente.CdCliente equals pedido.CdCliente
                                 join pedidoPrato in db.CadPedidoPratos on pedido.CdPedido equals pedidoPrato.CdPedido
                                 join cardapio in db.CadCardapios on pedidoPrato.CdPrato equals cardapio.CdPrato
                                 select new
                                 {
                                     pedido.CdPedido,
                                     cliente.CdCliente,
                                     cliente.NmCliente,
                                     pedido.DtCriacao,
                                     pedidoPrato.CdPrato,
                                     cardapio.NmPrato,
                                     cardapio.DsPrato,
                                     cardapio.Preco,
                                     pedidoPrato.Quantidade
                                 })
                                 .GroupBy(p => p.CdPedido) 
                                 .Select(g => new ResourcePedido
                                 {
                                     CdPedido = g.Key,
                                     CdCliente = g.FirstOrDefault().CdCliente,
                                     NmCliente = g.FirstOrDefault().NmCliente,
                                     DtCriacao = g.FirstOrDefault().DtCriacao,
                                     Produtos = g.Select(p => new ProdutoPedido
                                     {
                                         CdPrato = p.CdPrato,
                                         NmPrato = p.NmPrato,
                                         DsPrato = p.DsPrato,
                                         Preco = p.Preco,
                                         Quantidade = p.Quantidade
                                     }).ToList()
                                 }).ToListAsync();

            return Ok(pedidos);
        }




        /// <summary>
        /// Retorna um pedido específico pelo ID.
        /// </summary>
        /// <param name="id">ID do Pedido.</param>
        [HttpGet("GetPedidoPorId")]
        public async Task<ActionResult<ResourcePedido>> GetPedidoPorId(int id)
        {
            var pedidos = await (from cliente in db.CadClientes
                                 join pedido in db.CadPedidos on cliente.CdCliente equals pedido.CdCliente
                                 join pedidoPrato in db.CadPedidoPratos on pedido.CdPedido equals pedidoPrato.CdPedido
                                 join cardapio in db.CadCardapios on pedidoPrato.CdPrato equals cardapio.CdPrato
                                 where pedido.CdPedido == id
                                 select new ResourcePedido
                                 {
                                     CdPedido = pedido.CdPedido,
                                     CdCliente = cliente.CdCliente,
                                     NmCliente = cliente.NmCliente,
                                     DtCriacao = pedido.DtCriacao,
                                     Produtos = db.CadPedidoPratos
                                                 .Where(pp => pp.CdPedido == pedido.CdPedido)
                                                 .Join(db.CadCardapios, pp => pp.CdPrato, prato => prato.CdPrato,
                                                       (pp, prato) => new ProdutoPedido
                                                       {
                                                           CdPrato = prato.CdPrato,
                                                           NmPrato = prato.NmPrato,
                                                           DsPrato = prato.DsPrato,
                                                           Preco = prato.Preco,
                                                           Quantidade = pp.Quantidade
                                                       }).ToList()
                                 }).FirstOrDefaultAsync();





            if (pedidos == null)
            {
                return NotFound();
            }

            return pedidos;
        }


        /// <summary>
        /// Cria um novo pedido.
        /// </summary>
        /// <param name="cdCliente">id Cliente.</param>
        /// <param name="cdProdutos">lista id Produtos.</param>
        [HttpPost("CreatePedido")]
        public async Task<ActionResult<ResourcePedido>> CreatePedido(int cdCliente, List<int> cdProdutos)
        {
            try
            {
                var cliente = await db.CadClientes.FindAsync(cdCliente);
                if (cliente == null)
                {
                    return NotFound($"Cliente com ID {cdCliente} não encontrado.");
                }

                var pedido = new CadPedido
                {
                    DtCriacao = DateTime.Now,
                    CdCliente = cdCliente
                };

                db.CadPedidos.Add(pedido);
                await db.SaveChangesAsync();  

                foreach (var cdProduto in cdProdutos)
                {
                    var produto = await db.CadCardapios.FindAsync(cdProduto);
                    if (produto != null)
                    {
                        var pedidoPrato = new CadPedidoPrato
                        {
                            CdPedido = pedido.CdPedido,
                            CdPrato = cdProduto,
                            Quantidade = 1 
                        };

                        db.CadPedidoPratos.Add(pedidoPrato);
                    }
                }

                await db.SaveChangesAsync();

                
                return CreatedAtAction(nameof(GetPedidoPorId), new { id = pedido.CdPedido });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar pedido: {ex.Message}");
            }
        }




        /// <summary>
        /// Exclui um pedido.
        /// </summary>
        /// <param name="id">ID do pedido.</param>
        [HttpDelete("DeletePedido")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            try
            {

            }
            catch(Exception ex)
            {

            }

            var cadPedidoPratos = await (from a in db.CadPedidoPratos
                                   where a.CdPedido == id
                                   select a).ToListAsync();

            var cadPedido = await (from a in db.CadPedidos
                                   where a.CdPedido == id
                                   select a).FirstOrDefaultAsync();

            if (cadPedido == null)
            {
                return NotFound(new { Sucesso = false, Mensagem = "Pedido não encontrado." });
            }

            try
            {
                foreach(var cadPedidoPrato in cadPedidoPratos)
                {
                    db.CadPedidoPratos.Remove(cadPedidoPrato);
                }

                db.CadPedidos.Remove(cadPedido);

                await db.SaveChangesAsync();
                return Ok(new { Sucesso = true, Mensagem = "Pedido excluído com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Sucesso = false, Mensagem = "Erro ao excluir o Pedido.", Detalhes = ex.Message });
            }
        }



    }
}
