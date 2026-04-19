using Utilities.Statics;

namespace Test.Museums;

/// <summary>
/// Integration tests for <see cref="MuseumApplication"/> using an EF Core InMemory database.
/// Covers: listing, querying, CRUD operations, theme filtering, and article-by-museum retrieval.
/// </summary>
public class MuseumApplicationTests : TestBase
{
    // ─────────────────────────────────────────────
    // ListAllMuseums
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that all non-deleted museums are returned and mapped correctly.
    /// </summary>
    [Fact]
    public async Task ListAllMuseums_WhenMuseumsExist_ReturnsAllNonDeleted()
    {
        SeedMuseum("Museo Arte", 1);
        SeedMuseum("Museo Historia", 3);

        var result = await MuseumApp.ListAllMuseums();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count());
    }

    /// <summary>
    /// Verifies that an empty table still returns a successful response with no items.
    /// </summary>
    [Fact]
    public async Task ListAllMuseums_WhenEmpty_ReturnsSuccessWithEmptyData()
    {
        var result = await MuseumApp.ListAllMuseums();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!);
    }

    /// <summary>
    /// Verifies that soft-deleted museums are excluded from the results.
    /// </summary>
    [Fact]
    public async Task ListAllMuseums_ExcludesSoftDeletedMuseums()
    {
        var museum = SeedMuseum("Museo Eliminado");
        museum.DeletedAt = DateTime.Now;
        Context.Museums.Update(museum);
        Context.SaveChanges();

        var result = await MuseumApp.ListAllMuseums();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!);
    }

    // ─────────────────────────────────────────────
    // ListSelectMuseums
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that <c>ListSelectMuseums</c> returns the correct MuseumId and Name for each museum.
    /// </summary>
    [Fact]
    public async Task ListSelectMuseums_ReturnsSelectViewModels()
    {
        var museum = SeedMuseum("Museo A");

        var result = await MuseumApp.ListSelectMuseums();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!);
        Assert.Equal(museum.Id, result.Data!.First().MuseumId);
        Assert.Equal("Museo A", result.Data!.First().Name);
    }

    // ─────────────────────────────────────────────
    // GetMuseumById
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that a museum is correctly retrieved and mapped by its primary key.
    /// </summary>
    [Fact]
    public async Task GetMuseumById_WhenExists_ReturnsMappedMuseum()
    {
        var museum = SeedMuseum("Museo Arte", 1);

        var result = await MuseumApp.GetMuseumById(museum.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Museo Arte", result.Data!.Name);
        Assert.Equal(1, result.Data.ThemeId);
    }

    /// <summary>
    /// Verifies that querying a non-existent ID returns a failure response with null data.
    /// </summary>
    [Fact]
    public async Task GetMuseumById_WhenNotExists_ReturnsFailure()
    {
        var result = await MuseumApp.GetMuseumById(999);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ReplyMessages.MESSAGE_QUERY_EMPTY, result.Message);
    }

    // ─────────────────────────────────────────────
    // RegisterMuseum
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that a museum with valid data is persisted and returns success.
    /// </summary>
    [Fact]
    public async Task RegisterMuseum_WithValidData_PersistsAndReturnsSuccess()
    {
        var request = new MuseumRequestViewModel { Name = "Museo Nuevo", Theme = 2 };

        var result = await MuseumApp.RegisterMuseum(request);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data > 0);
        Assert.Equal(ReplyMessages.MESSAGE_SAVE, result.Message);
        Assert.Equal(1, Context.Museums.Count());
    }

    /// <summary>
    /// Verifies that a museum with a null name fails FluentValidation and is not persisted.
    /// </summary>
    [Fact]
    public async Task RegisterMuseum_WithNullName_ReturnsValidationErrorAndDoesNotPersist()
    {
        var request = new MuseumRequestViewModel { Name = null, Theme = 1 };

        var result = await MuseumApp.RegisterMuseum(request);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors!, e => e.PropertyName == "Name");
        Assert.Equal(0, Context.Museums.Count());
    }

    /// <summary>
    /// Verifies that a museum with an empty name fails FluentValidation and is not persisted.
    /// </summary>
    [Fact]
    public async Task RegisterMuseum_WithEmptyName_ReturnsValidationError()
    {
        var request = new MuseumRequestViewModel { Name = "", Theme = 1 };

        var result = await MuseumApp.RegisterMuseum(request);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
    }

    // ─────────────────────────────────────────────
    // EditMuseum
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that editing a museum with valid data updates its name and theme in the database.
    /// </summary>
    [Fact]
    public async Task EditMuseum_WithValidData_UpdatesNameAndTheme()
    {
        var museum = SeedMuseum("Nombre Original", 1);
        var request = new MuseumRequestViewModel { Name = "Nombre Editado", Theme = 3 };

        var result = await MuseumApp.EditMuseum(museum.Id, request);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_UPDATE, result.Message);
        var updated = Context.Museums.AsNoTracking().First(m => m.Id == museum.Id);
        Assert.Equal("Nombre Editado", updated.Name);
        Assert.Equal(3, updated.Theme);
    }

    /// <summary>
    /// Verifies that editing a museum with a null name returns a validation error
    /// and does not modify the database.
    /// </summary>
    [Fact]
    public async Task EditMuseum_WithNullName_ReturnsValidationError()
    {
        var museum = SeedMuseum("Original");
        var request = new MuseumRequestViewModel { Name = null, Theme = 1 };

        var result = await MuseumApp.EditMuseum(museum.Id, request);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        var unchanged = Context.Museums.AsNoTracking().First(m => m.Id == museum.Id);
        Assert.Equal("Original", unchanged.Name);
    }

    // ─────────────────────────────────────────────
    // DeleteMuseum
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that deleting a museum removes it permanently from the database.
    /// </summary>
    [Fact]
    public async Task DeleteMuseum_WhenExists_RemovesRowFromDatabase()
    {
        var museum = SeedMuseum();

        var result = await MuseumApp.DeleteMuseum(museum.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_DELETE, result.Message);
        Assert.Null(Context.Museums.Find(museum.Id));
    }

    /// <summary>
    /// Verifies that deleting a museum with active articles is rejected to prevent FK violations.
    /// </summary>
    [Fact]
    public async Task DeleteMuseum_WhenHasActiveArticles_ReturnsFailureAndDoesNotDelete()
    {
        var museum = SeedMuseum();
        SeedArticle(museum.Id, "Vasija");

        var result = await MuseumApp.DeleteMuseum(museum.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_HAS_ARTICLES, result.Message);
        Assert.NotNull(Context.Museums.Find(museum.Id));
    }

    // ─────────────────────────────────────────────
    // RemoveMuseum (soft delete)
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that removing a museum sets <c>DeletedAt</c> without deleting the row.
    /// </summary>
    [Fact]
    public async Task RemoveMuseum_WhenExists_SetsDeletedAtAndKeepsRow()
    {
        var museum = SeedMuseum();

        var result = await MuseumApp.RemoveMuseum(museum.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_REMOVED, result.Message);
        var softDeleted = Context.Museums.Find(museum.Id);
        Assert.NotNull(softDeleted);
        Assert.NotNull(softDeleted!.DeletedAt);
    }

    /// <summary>
    /// Verifies that removing a museum also soft-deletes all its active articles.
    /// </summary>
    [Fact]
    public async Task RemoveMuseum_CascadeSoftDeletesActiveArticles()
    {
        var museum = SeedMuseum();
        SeedArticle(museum.Id, "Vasija");
        SeedArticle(museum.Id, "Cuadro");

        await MuseumApp.RemoveMuseum(museum.Id);

        var remaining = Context.Articles.AsNoTracking()
            .Where(a => a.IdMuseum == museum.Id && a.DeletedAt == null)
            .ToList();
        Assert.Empty(remaining);
    }

    /// <summary>
    /// Verifies that a soft-deleted museum is no longer returned by <c>ListAllMuseums</c>.
    /// </summary>
    [Fact]
    public async Task RemoveMuseum_ThenListAll_DoesNotReturnRemovedMuseum()
    {
        var museum = SeedMuseum("Museo Temporal");
        await MuseumApp.RemoveMuseum(museum.Id);

        var result = await MuseumApp.ListAllMuseums();

        Assert.Empty(result.Data!);
    }

    // ─────────────────────────────────────────────
    // GetMuseumsByTheme
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that only museums matching the requested theme are returned.
    /// </summary>
    [Fact]
    public async Task GetMuseumsByTheme_ReturnsOnlyMuseumsWithMatchingTheme()
    {
        SeedMuseum("Arte", theme: 1);
        SeedMuseum("Ciencias", theme: 2);
        SeedMuseum("Historia", theme: 3);

        var result = await MuseumApp.GetMuseumsByTheme(1);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!);
        Assert.Equal("Arte", result.Data!.First().Name);
    }

    /// <summary>
    /// Verifies that querying a theme with no matching museums still returns a successful,
    /// empty response (not an error).
    /// </summary>
    [Fact]
    public async Task GetMuseumsByTheme_WhenNoMatch_ReturnsSuccessWithEmptyData()
    {
        SeedMuseum("Arte", theme: 1);

        var result = await MuseumApp.GetMuseumsByTheme(2);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!);
    }

    /// <summary>
    /// Verifies that multiple museums sharing the same theme are all returned.
    /// </summary>
    [Fact]
    public async Task GetMuseumsByTheme_WithMultipleMatches_ReturnsAll()
    {
        SeedMuseum("Arte Moderno", theme: 1);
        SeedMuseum("Arte Clásico", theme: 1);
        SeedMuseum("Historia", theme: 3);

        var result = await MuseumApp.GetMuseumsByTheme(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count());
    }

    // ─────────────────────────────────────────────
    // GetArticlesByMuseum
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that only articles belonging to the specified museum are returned.
    /// </summary>
    [Fact]
    public async Task GetArticlesByMuseum_ReturnsOnlyArticlesForThatMuseum()
    {
        var museum1 = SeedMuseum("Museo 1");
        var museum2 = SeedMuseum("Museo 2");
        SeedArticle(museum1.Id, "Vasija");
        SeedArticle(museum1.Id, "Cuadro");
        SeedArticle(museum2.Id, "Escultura");

        var result = await MuseumApp.GetArticlesByMuseum(museum1.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count());
        Assert.All(result.Data!, a => Assert.Equal(museum1.Id, a.IdMuseum));
    }

    /// <summary>
    /// Verifies that the museum name is propagated to each article's <c>NameMuseum</c> field.
    /// </summary>
    [Fact]
    public async Task GetArticlesByMuseum_SetsNameMuseumOnEachArticle()
    {
        var museum = SeedMuseum("Museo Arte");
        SeedArticle(museum.Id, "Cuadro");

        var result = await MuseumApp.GetArticlesByMuseum(museum.Id);

        Assert.Equal("Museo Arte", result.Data!.First().NameMuseum);
    }
}
