using Application.Mappers;
using Application.Validators;
using AutoMapper;
using Infrastructure.Persistences.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Common;

/// <summary>
/// Base class for all application-layer tests. Creates an isolated InMemory database per test
/// instance and wires up real AutoMapper, FluentValidation, and both application services.
/// Each xUnit test class receives a fresh constructor call, so databases never share state.
/// </summary>
public abstract class TestBase : IDisposable
{
    protected readonly MuseumsDbContext Context;
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IMapper Mapper;
    protected readonly MuseumApplication MuseumApp;
    protected readonly ArticleApplication ArticleApp;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<MuseumsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new MuseumsDbContext(options);
        UnitOfWork = new UnitOfWork(Context);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ArticleMappingsProfile).Assembly));
        Mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();

        MuseumApp = new MuseumApplication(UnitOfWork, Mapper, new MuseumValidator());
        ArticleApp = new ArticleApplication(UnitOfWork, Mapper, new ArticleValidator());
    }

    /// <summary>
    /// Inserts a museum directly into the InMemory database and returns it with the auto-generated Id.
    /// </summary>
    protected Museum SeedMuseum(string name = "Test Museum", int theme = 1)
    {
        var museum = new Museum { Name = name, Theme = theme, CreatedAt = DateTime.Now };
        Context.Museums.Add(museum);
        Context.SaveChanges();
        Context.Entry(museum).State = EntityState.Detached;
        return museum;
    }

    /// <summary>
    /// Inserts an article directly into the InMemory database and returns it with the auto-generated Id.
    /// </summary>
    protected Article SeedArticle(int museumId, string name = "Test Article", bool? isDamaged = null)
    {
        var article = new Article
        {
            Name = name,
            IdMuseum = museumId,
            IsDamaged = isDamaged,
            CreatedAt = DateTime.Now
        };
        Context.Articles.Add(article);
        Context.SaveChanges();
        Context.Entry(article).State = EntityState.Detached;
        return article;
    }

    public void Dispose() => Context.Dispose();
}
