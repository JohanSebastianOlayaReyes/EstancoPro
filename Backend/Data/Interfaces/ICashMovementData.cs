namespace Data.Interfaces;

using Entity.Dto;
using Entity.Model;

public interface ICashMovementData
{
    Task<CashMovementDto> GetByIdAsync(int cashSessionId, DateTime at);
    Task<IEnumerable<CashMovementDto>> GetAllAsync();
    Task<CashMovementDto> CreateAsync(CashMovementDto dto);
    Task UpdateAsync(int cashSessionId, DateTime at, CashMovementDto dto);
    Task DeleteAsync(int cashSessionId, DateTime at);
}
