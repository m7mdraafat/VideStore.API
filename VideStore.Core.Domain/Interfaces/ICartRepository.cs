namespace VideStore.Domain.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(string userId, bool includeItems = true);
        Task<Cart?> GetByGuestIdAsync(Guid guestId, bool includeItems = true);
    }
}
