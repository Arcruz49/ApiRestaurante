using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITeste.Data;
using APITeste.Models;

namespace APITeste.Controllers
{
    /// <summary>
    /// Controlador para gerenciar produtos.
    /// </summary>
    [ApiController]
    [Route("api/cardapio")]
    public class CardapioController : ControllerBase
    {
        private readonly DbRestauranteContext db;

        /// <summary>
        /// Construtor do controlador de produtos.
        /// </summary>
        public CardapioController(DbRestauranteContext context)
        {
            db = context;
        }

        /// <summary>
        /// Retorna todos os produtos.
        /// </summary>
        [HttpGet("GetProdutos")]
        public async Task<ActionResult<IEnumerable<CadCardapio>>> GetProdutos()
        {
            return await db.CadCardapios.ToListAsync();
        }

        /// <summary>
        /// Retorna um produto específico pelo ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        [HttpGet("GetProdutoPorId")]
        public async Task<ActionResult<CadCardapio>> GetProdutoPorId(int id)
        {
            var produto = await db.CadCardapios.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        /// <param name="produto">Objeto produto.</param>
        [HttpPost("CreateProduto")]
        public async Task<ActionResult<CadCardapio>> CreateProduto(CadCardapio produto)
        {
            produto.DtCriacao = DateTime.Now;
            db.CadCardapios.Add(produto);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProdutoPorId), new { id = produto.CdPrato});
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <param name="produto">Dados atualizados do produto.</param>
        [HttpPut("AtualizaProduto")]
        public async Task<IActionResult> AtualizaProduto(int id, CadCardapio produto)
        {
            var oldProduto = db.CadCardapios.Find(id);
            
            if (oldProduto == null) return NotFound(new { Sucesso = false, Mensagem = "Cliente não existe." });
            
            oldProduto.NmPrato = produto.NmPrato;
            oldProduto.Preco = produto.Preco;
            oldProduto.DsPrato = produto.DsPrato;

            db.Entry(oldProduto).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProdutoExists(id))
                {
                    return NotFound(new { Sucesso = false, Mensagem = "Produto não existe." });
                }
                else
                {
                    return NotFound(new { Sucesso = false, Mensagem = "Erro.", Detalhes = ex.Message });
                }
            }

            return Ok(new { Sucesso = true, Mensagem = "Produto Atualizado." });
        }

        /// <summary>
        /// Exclui um produto.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        [HttpDelete("DeleteProduto")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await db.CadCardapios.FindAsync(id);
            if (produto == null)
            {
                return NotFound(new { Sucesso = false, Mensagem = "Produto não encontrado." });
            }

            try
            {
                db.CadCardapios.Remove(produto);
                await db.SaveChangesAsync();
                return Ok(new { Sucesso = true, Mensagem = "Produto excluído com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Sucesso = false, Mensagem = "Erro ao excluir o Produto.", Detalhes = ex.Message });
            }
        }


        /// <summary>
        /// Verifica se o produto existe pelo ID.
        /// </summary>
        private bool ProdutoExists(int id)
        {
            return db.CadCardapios.Any(e => e.CdPrato == id);
        }
    }
}
