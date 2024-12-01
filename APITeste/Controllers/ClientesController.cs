using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITeste.Data;
using APITeste.Models;

namespace APITeste.Controllers
{
    /// <summary>
    /// Controlador para gerenciar clientes.
    /// </summary>
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly DbRestauranteContext db;

        /// <summary>
        /// Construtor do controlador de clientes.
        /// </summary>
        public ClientesController(DbRestauranteContext context)
        {
            db = context;
        }

        /// <summary>
        /// Retorna todos os clientes.
        /// </summary>
        [HttpGet("GetClientes")]
        public async Task<ActionResult<IEnumerable<CadCliente>>> GetClientes()
        {
            return await db.CadClientes.ToListAsync();
        }

        /// <summary>
        /// Retorna um cliente específico pelo ID.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        [HttpGet("GetClientePorId")]
        public async Task<ActionResult<CadCliente>> GetClientePorId(int id)
        {
            var cliente = await db.CadClientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="cliente">Objeto cliente.</param>
        [HttpPost("CreateCliente")]
        public async Task<ActionResult<CadCliente>> CreateCliente(CadCliente cliente)
        {
            cliente.DtCriacao = DateTime.Now;
            db.CadClientes.Add(cliente);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClientePorId), new { id = cliente.CdCliente });
        }

        /// <summary>
        /// Atualiza um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <param name="cliente">Dados atualizados do cliente.</param>
        [HttpPut("AtualizaCliente")]
        public async Task<IActionResult> AtualizaCliente(int id, CadCliente cliente)
        {
            
            var oldCliente = db.CadClientes.Find(id);
            if (oldCliente == null) return NotFound(new { Sucesso = false, Mensagem = "Cliente não existe." });
            oldCliente.NmCliente = cliente.NmCliente;

            db.Entry(oldCliente).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ClienteExists(id))
                {
                    return NotFound(new { Sucesso = false, Mensagem = "Cliente não existe." });
                }
                else
                {
                    return NotFound(new { Sucesso = false, Mensagem = "Erro.", Detalhes = ex.Message });
                }
            }

            return Ok(new { Sucesso = true, Mensagem = "Cliente Atualizado." });
        }

        /// <summary>
        /// Exclui um cliente.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        [HttpDelete("DeleteCliente")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await db.CadClientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound(new { Sucesso = false, Mensagem = "Cliente não encontrado." });
            }

            try
            {
                db.CadClientes.Remove(cliente);
                await db.SaveChangesAsync();
                return Ok(new { Sucesso = true, Mensagem = "Cliente excluído com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Sucesso = false, Mensagem = "Erro ao excluir o cliente.", Detalhes = ex.Message });
            }
        }


        /// <summary>
        /// Verifica se o cliente existe pelo ID.
        /// </summary>
        private bool ClienteExists(int id)
        {
            return db.CadClientes.Any(e => e.CdCliente == id);
        }
    }
}
