using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private MuseumsDbContext _context;
        public IArticleRepository Article { get; private set; }

        public IMuseumRepository Museum { get; private set; }

        public UnitOfWork(MuseumsDbContext context)
        {
            _context = context;
            Article = new ArticleRepository(_context);
            Museum = new MuseumRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
