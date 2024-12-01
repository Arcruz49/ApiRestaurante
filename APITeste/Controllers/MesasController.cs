using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITeste.Data;
using APITeste.Models;

namespace APITeste.Controllers
{
    /// <summary>
    /// Controlador para gerenciar mesas.
    /// </summary>
    [ApiController]
    [Route("api/mesas")]
    public class MesasController : ControllerBase
    {
        private readonly DbRestauranteContext db;

        /// <summary>
        /// Construtor do controlador de mesas.
        /// </summary>
        public MesasController(DbRestauranteContext context)
        {
            db = context;
        }

        /// <summary>
        /// Retorna todas as mesas.
        /// </summary>
        [HttpGet("GetMesas")]
        public async Task<ActionResult<IEnumerable<CadMesa>>> GetMesas()
        {
            return await db.CadMesas.ToListAsync();
        }

        /// <summary>
        /// Retorna uma mesa específica pelo ID.
        /// </summary>
        /// <param name="id">ID da mesa.</param>
        [HttpGet("GetMesaPorId")]
        public async Task<ActionResult<CadMesa>> GetMesaPorId(int id)
        {
            var mesa = await db.CadMesas.FindAsync(id);

            if (mesa == null)
            {
                return NotFound();
            }

            return mesa;
        }

        /// <summary>
        /// Cria uma nova mesa.
        /// </summary>
        /// <param name="mesa">Objeto mesa.</param>
        [HttpPost("CreateMesa")]
        public async Task<ActionResult<CadMesa>> Createmesa(CadMesa mesa)
        {
            mesa.DtCriacao = DateTime.Now;
            db.CadMesas.Add(mesa);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMesaPorId), new { id = mesa.CdMesa });
        }

       

        /// <summary>
        /// Exclui uma mesa.
        /// </summary>
        /// <param name="id">ID da mesa.</param>
        [HttpDelete("DeleteMesa")]
        public async Task<IActionResult> DeleteMesa(int id)
        {
            var mesa = await db.CadMesas.FindAsync(id);
            if (mesa == null)
            {
                return NotFound(new { Sucesso = false, Mensagem = "Mesa não encontrada." });
            }

            try
            {
                db.CadMesas.Remove(mesa);
                await db.SaveChangesAsync();
                return Ok(new { Sucesso = true, Mensagem = "Mesa excluída com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Sucesso = false, Mensagem = "Erro ao excluir a mesa.", Detalhes = ex.Message });
            }
        }


        /// <summary>
        /// Verifica se a mesa existe pelo ID.
        /// </summary>
        private bool mesaExists(int id)
        {
            return db.CadMesas.Any(e => e.CdMesa == id);
        }
    }
}
