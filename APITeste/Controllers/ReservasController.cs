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
    [Route("api/reservas")]
    public class ReservasController : ControllerBase
    {
        private readonly DbRestauranteContext db;

        /// <summary>
        /// Construtor do controlador de clientes.
        /// </summary>
        public ReservasController(DbRestauranteContext context)
        {
            db = context;
        }

        /// <summary>
        /// Retorna todas as reservas.
        /// </summary>
        [HttpGet("GetReservas")]
        public async Task<ActionResult<IEnumerable<CadReserva>>> GetReservas()
        {
            return await db.CadReservas.ToListAsync();
        }

        /// <summary>
        /// Retorna uma reservas específica pelo ID.
        /// </summary>
        /// <param name="id">ID da reserva.</param>
        [HttpGet("GetReservaPorId")]
        public async Task<ActionResult<CadReserva>> GetReservaPorId(int id)
        {
            var reserva = await db.CadReservas.FindAsync(id);

            if (reserva == null)
            {
                return NotFound();
            }

            return reserva;
        }

        /// <summary>
        /// Cria um novo reserva.
        /// </summary>
        /// <param name="reserva">Objeto cliente.</param>
        [HttpPost("CreateReserva")]
        public async Task<ActionResult<CadReserva>> CreateReserva(CadReserva reserva)
        {

            var mesaEmUso = await (from a in db.CadReservas
                             where a.CdMesa == reserva.CdMesa
                             select a).FirstOrDefaultAsync();

            if (mesaEmUso != null) return NotFound(new { Sucesso = false, Mensagem = "A Mesa já está ocupado" });

            reserva.NmCliente = await (from a in db.CadClientes
                                 where a.CdCliente == reserva.CdCliente
                                 select a.NmCliente).FirstOrDefaultAsync();

            reserva.DtCriacao = DateTime.Now;
            db.CadReservas.Add(reserva);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservaPorId), new { id = reserva.CdReserva });
        }

        /// <summary>
        /// Exclui uma reserva.
        /// </summary>
        /// <param name="id">ID do reserva.</param>
        [HttpDelete("DeleteReserva")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await db.CadReservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound(new { Sucesso = false, Mensagem = "Reserva não encontrada." });
            }

            try
            {
                db.CadReservas.Remove(reserva);
                await db.SaveChangesAsync();
                return Ok(new { Sucesso = true, Mensagem = "Reserva excluída com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Sucesso = false, Mensagem = "Erro ao excluir a reserva.", Detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se o cliente existe pelo ID.
        /// </summary>
        private bool ReservaExists(int id)
        {
            return db.CadReservas.Any(e => e.CdReserva == id);
        }
    }
}
