namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para CashSession con flujo de apertura/cierre de caja
/// </summary>
public class CashSessionBusiness : BaseBusiness<CashSession, CashSessionDto>, ICashSessionBusiness
{
    private readonly ICashSessionData _cashSessionData;
    private readonly ApplicationDbContext _context;

    public CashSessionBusiness(
        ICashSessionData cashSessionData,
        ApplicationDbContext context,
        ILogger<BaseBusiness<CashSession, CashSessionDto>> logger)
        : base(cashSessionData, logger)
    {
        _cashSessionData = cashSessionData;
        _context = context;
    }

    /// <summary>
    /// Abre una nueva sesión de caja
    /// Flujo 3.3: Inicio del turno
    /// Validación crítica punto 4: Solo una sesión abierta a la vez
    /// Regla punto 2.4: Registrar monto de apertura
    /// </summary>
    public async Task<CashSessionDto> OpenSessionAsync(decimal openingAmount)
    {
        // Validar monto de apertura
        if (openingAmount < 0)
        {
            throw new ArgumentException("El monto de apertura no puede ser negativo");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Validar que no haya otra sesión abierta
            var hasOpenSession = await _context.cashSessions
                .AnyAsync(cs => cs.ClosedAt == null);

            if (hasOpenSession)
            {
                throw new InvalidOperationException(
                    "Ya existe una sesión de caja abierta. Debe cerrar la sesión actual antes de abrir una nueva.");
            }

            // Crear nueva sesión
            var session = new CashSession
            {
                OpenedAt = DateTime.UtcNow,
                ClosedAt = null,
                OpeningAmount = openingAmount,
                ClosingAmount = 0,
                Active = true,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            await _context.cashSessions.AddAsync(session);
            await _context.SaveChangesAsync();

            // Opcional: Crear movimiento de apertura (punto 3.3)
            var openingMovement = new CashMovement
            {
                CashSessionId = session.Id,
                Type = "Opening",
                Amount = openingAmount,
                Reason = "Apertura de caja",
                At = DateTime.UtcNow,
                RelatedId = null,
                RelatedEntity = null
            };

            await _context.cashMovements.AddAsync(openingMovement);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Retornar DTO
            return new CashSessionDto
            {
                Id = session.Id,
                OpenedAt = session.OpenedAt,
                ClosedAt = session.ClosedAt,
                OpeningAmount = session.OpeningAmount,
                ClosingAmount = session.ClosingAmount,
                Active = session.Active,
                CreateAt = session.CreateAt,
                UpdateAt = session.UpdateAt,
                DeleteAt = session.DeleteAt
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Cierra una sesión de caja y calcula la diferencia
    /// Flujo 3.3: Cierre del turno con cuadre
    /// Regla punto 2.4: Esperado = OpeningAmount + ΣEntradas - ΣSalidas
    /// Diferencia = ClosingAmount (físico) - Esperado
    /// </summary>
    public async Task<decimal> CloseSessionAsync(int sessionId, decimal closingAmount)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Obtener sesión con sus movimientos
            var session = await _context.cashSessions
                .Include(cs => cs.cashmovement)
                .FirstOrDefaultAsync(cs => cs.Id == sessionId);

            if (session == null)
            {
                throw new KeyNotFoundException($"No se encontró la sesión de caja con Id {sessionId}");
            }

            // Validar que esté abierta
            if (session.ClosedAt != null)
            {
                throw new InvalidOperationException("La sesión de caja ya está cerrada");
            }

            // Calcular esperado (Regla punto 2.4)
            var entries = session.cashmovement
                .Where(cm => cm.Type == "Sale" || cm.Type == "Deposit" || cm.Type == "Opening")
                .Sum(cm => cm.Amount);

            var exits = session.cashmovement
                .Where(cm => cm.Type == "PurchasePayment" || cm.Type == "Expense" || cm.Type == "Withdrawal")
                .Sum(cm => cm.Amount);

            var expectedAmount = session.OpeningAmount + entries - exits;

            // Calcular diferencia
            var difference = closingAmount - expectedAmount;

            // Guardar monto de cierre
            session.ClosingAmount = closingAmount;
            session.ClosedAt = DateTime.UtcNow;
            session.UpdateAt = DateTime.UtcNow;

            // Opcional: Crear movimiento de cierre
            var closingMovement = new CashMovement
            {
                CashSessionId = session.Id,
                Type = "Closing",
                Amount = closingAmount,
                Reason = difference >= 0
                    ? $"Cierre de caja - Sobrante: {difference:C}"
                    : $"Cierre de caja - Faltante: {Math.Abs(difference):C}",
                At = DateTime.UtcNow,
                RelatedId = null,
                RelatedEntity = null
            };

            await _context.cashMovements.AddAsync(closingMovement);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Retornar diferencia (positivo = sobrante, negativo = faltante)
            return difference;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Obtiene la sesión de caja actualmente abierta
    /// Validación crítica: Necesario antes de finalizar ventas (punto 4)
    /// </summary>
    public async Task<CashSessionDto?> GetOpenSessionAsync()
    {
        var session = await _context.cashSessions
            .Where(cs => cs.ClosedAt == null)
            .Select(cs => new CashSessionDto
            {
                Id = cs.Id,
                OpenedAt = cs.OpenedAt,
                ClosedAt = cs.ClosedAt,
                OpeningAmount = cs.OpeningAmount,
                ClosingAmount = cs.ClosingAmount,
                Active = cs.Active,
                CreateAt = cs.CreateAt,
                UpdateAt = cs.UpdateAt,
                DeleteAt = cs.DeleteAt
            })
            .FirstOrDefaultAsync();

        return session;
    }

    /// <summary>
    /// Obtiene sesiones de caja en un rango de fechas
    /// Punto 6: Reporte de sesiones por periodo
    /// </summary>
    public async Task<IEnumerable<CashSessionDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var sessions = await _context.cashSessions
            .Where(cs => cs.OpenedAt >= from && cs.OpenedAt <= to)
            .Select(cs => new CashSessionDto
            {
                Id = cs.Id,
                OpenedAt = cs.OpenedAt,
                ClosedAt = cs.ClosedAt,
                OpeningAmount = cs.OpeningAmount,
                ClosingAmount = cs.ClosingAmount,
                Active = cs.Active,
                CreateAt = cs.CreateAt,
                UpdateAt = cs.UpdateAt,
                DeleteAt = cs.DeleteAt
            })
            .OrderByDescending(cs => cs.OpenedAt)
            .ToListAsync();

        return sessions;
    }

    /// <summary>
    /// Calcula el balance de una sesión (esperado, real, diferencia)
    /// Punto 6: Reporte de cuadre de caja
    /// Regla punto 2.4: Cálculo de esperado vs real
    /// </summary>
    public async Task<CashSessionBalanceDto> GetSessionBalanceAsync(int sessionId)
    {
        // Obtener sesión con movimientos
        var session = await _context.cashSessions
            .Include(cs => cs.cashmovement)
            .FirstOrDefaultAsync(cs => cs.Id == sessionId);

        if (session == null)
        {
            throw new KeyNotFoundException($"No se encontró la sesión de caja con Id {sessionId}");
        }

        // Calcular entradas (ventas, depósitos, apertura)
        var entries = session.cashmovement
            .Where(cm => cm.Type == "Sale" || cm.Type == "Deposit" || cm.Type == "Opening")
            .Sum(cm => cm.Amount);

        // Calcular salidas (pagos compras, gastos, retiros)
        var exits = session.cashmovement
            .Where(cm => cm.Type == "PurchasePayment" || cm.Type == "Expense" || cm.Type == "Withdrawal")
            .Sum(cm => cm.Amount);

        // Monto esperado
        var expectedAmount = session.OpeningAmount + entries - exits;

        // Monto real (si está cerrada, el ClosingAmount; si está abierta, el esperado)
        var actualAmount = session.ClosedAt != null
            ? session.ClosingAmount
            : expectedAmount;

        // Diferencia
        var difference = actualAmount - expectedAmount;

        // Mapear movimientos a DTOs
        var movementDtos = session.cashmovement
            .OrderBy(cm => cm.At)
            .Select(cm => new CashMovementDto
            {
                CashSessionId = cm.CashSessionId,
                Type = cm.Type,
                Amount = cm.Amount,
                Reason = cm.Reason,
                At = cm.At,
                RelatedId = cm.RelatedId,
                RelatedEntity = cm.RelatedEntity
            })
            .ToList();

        return new CashSessionBalanceDto
        {
            SessionId = session.Id,
            OpeningAmount = session.OpeningAmount,
            ExpectedAmount = expectedAmount,
            ActualAmount = actualAmount,
            Difference = difference,
            Movements = movementDtos
        };
    }
}
