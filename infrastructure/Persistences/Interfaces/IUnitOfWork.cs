namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //Declaración o Matrícula de nuestras interfaces a nivel de Repositorio
        IMuseumRepository Museum { get; }
        IArticleRepository Article { get; }
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
