using Microsoft.EntityFrameworkCore;
using Sponsorship.Application.Interfaces.Repositories;
using Sponsorship.Domain.Entities;
using Sponsorship.Infrastructure.Data;

namespace Sponsorship.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id) =>
        _db.Users.FindAsync(id).AsTask();

    public Task<User?> GetByEmailAsync(string email) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email);
}
