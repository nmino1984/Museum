using Utilities.Statics;

namespace Test.Articles;

/// <summary>
/// Integration tests for <see cref="ArticleApplication"/> using an EF Core InMemory database.
/// Covers: listing, querying, CRUD operations, mark-as-damaged, relocate, and bulk register.
/// </summary>
public class ArticleApplicationTests : TestBase
{
    // ─────────────────────────────────────────────
    // ListAllArticles
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that all non-deleted articles are returned and mapped correctly.
    /// </summary>
    [Fact]
    public async Task ListAllArticles_ReturnsAllNonDeletedIncludingDamaged()
    {
        var museum = SeedMuseum();
        SeedArticle(museum.Id, "Vasija");
        SeedArticle(museum.Id, "Cuadro Dañado", isDamaged: true);

        var result = await ArticleApp.ListAllArticles();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count());
    }

    /// <summary>
    /// Verifies that soft-deleted articles are excluded from the results.
    /// </summary>
    [Fact]
    public async Task ListAllArticles_ExcludesSoftDeletedArticles()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id, "Vasija");
        article.DeletedAt = DateTime.Now;
        Context.Articles.Update(article);
        Context.SaveChanges();

        var result = await ArticleApp.ListAllArticles();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!);
    }

    // ─────────────────────────────────────────────
    // GetArticleById
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that an article is correctly retrieved and mapped by its primary key.
    /// </summary>
    [Fact]
    public async Task GetArticleById_WhenExists_ReturnsMappedArticle()
    {
        var museum = SeedMuseum("Museo Arte");
        var article = SeedArticle(museum.Id, "Escultura");

        var result = await ArticleApp.GetArticleById(article.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Escultura", result.Data!.Name);
        Assert.Equal(museum.Id, result.Data.IdMuseum);
    }

    /// <summary>
    /// Verifies that querying a non-existent ID returns a failure response with null data.
    /// </summary>
    [Fact]
    public async Task GetArticleById_WhenNotExists_ReturnsFailure()
    {
        var result = await ArticleApp.GetArticleById(999);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ReplyMessages.MESSAGE_QUERY_EMPTY, result.Message);
    }

    // ─────────────────────────────────────────────
    // RegisterArticle
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that an article with valid data is persisted and returns success.
    /// </summary>
    [Fact]
    public async Task RegisterArticle_WithValidData_PersistsAndReturnsSuccess()
    {
        var museum = SeedMuseum();
        var request = new ArticleRequestViewModel { Name = "Vasija Griega", IsDamaged = false, IdMuseum = museum.Id };

        var result = await ArticleApp.RegisterArticle(request);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data > 0);
        Assert.Equal(ReplyMessages.MESSAGE_SAVE, result.Message);
        Assert.Equal(1, Context.Articles.Count());
    }

    /// <summary>
    /// Verifies that an article with a null name fails FluentValidation and is not persisted.
    /// </summary>
    [Fact]
    public async Task RegisterArticle_WithNullName_ReturnsValidationErrorAndDoesNotPersist()
    {
        var museum = SeedMuseum();
        var request = new ArticleRequestViewModel { Name = null, IdMuseum = museum.Id };

        var result = await ArticleApp.RegisterArticle(request);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors!, e => e.PropertyName == "Name");
        Assert.Equal(0, Context.Articles.Count());
    }

    /// <summary>
    /// Verifies that an article with an empty name fails FluentValidation and is not persisted.
    /// </summary>
    [Fact]
    public async Task RegisterArticle_WithEmptyName_ReturnsValidationError()
    {
        var museum = SeedMuseum();
        var request = new ArticleRequestViewModel { Name = "", IdMuseum = museum.Id };

        var result = await ArticleApp.RegisterArticle(request);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Equal(0, Context.Articles.Count());
    }

    // ─────────────────────────────────────────────
    // EditArticle
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that editing an article with valid data updates its name in the database.
    /// </summary>
    [Fact]
    public async Task EditArticle_WithValidData_UpdatesNameInDatabase()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id, "Nombre Original");
        var request = new ArticleRequestViewModel { Name = "Nombre Editado", IsDamaged = false, IdMuseum = museum.Id };

        var result = await ArticleApp.EditArticle(article.Id, request);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_UPDATE, result.Message);
        var updated = Context.Articles.AsNoTracking().First(a => a.Id == article.Id);
        Assert.Equal("Nombre Editado", updated.Name);
    }

    /// <summary>
    /// Verifies that editing an article with a null name returns a validation error
    /// and does not modify the database.
    /// </summary>
    [Fact]
    public async Task EditArticle_WithNullName_ReturnsValidationError()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id, "Original");
        var request = new ArticleRequestViewModel { Name = null, IdMuseum = museum.Id };

        var result = await ArticleApp.EditArticle(article.Id, request);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        var unchanged = Context.Articles.AsNoTracking().First(a => a.Id == article.Id);
        Assert.Equal("Original", unchanged.Name);
    }

    // ─────────────────────────────────────────────
    // DeleteArticle
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that deleting an article removes it permanently from the database.
    /// </summary>
    [Fact]
    public async Task DeleteArticle_WhenExists_RemovesRowFromDatabase()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id);

        var result = await ArticleApp.DeleteArticle(article.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_DELETE, result.Message);
        Assert.Null(Context.Articles.Find(article.Id));
    }

    // ─────────────────────────────────────────────
    // RemoveArticle (soft delete)
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that removing an article sets <c>DeletedAt</c> without deleting the row.
    /// </summary>
    [Fact]
    public async Task RemoveArticle_WhenExists_SetsDeletedAtAndKeepsRow()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id, "Vasija");

        var result = await ArticleApp.RemoveArticle(article.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_REMOVED, result.Message);
        var softDeleted = Context.Articles.Find(article.Id);
        Assert.NotNull(softDeleted);
        Assert.NotNull(softDeleted!.DeletedAt);
    }

    /// <summary>
    /// Verifies that a soft-deleted article no longer appears in <c>ListAllArticles</c>.
    /// </summary>
    [Fact]
    public async Task RemoveArticle_ThenListAll_DoesNotReturnRemovedArticle()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id, "Vasija Temporal");
        await ArticleApp.RemoveArticle(article.Id);

        var result = await ArticleApp.ListAllArticles();

        Assert.Empty(result.Data!);
    }

    // ─────────────────────────────────────────────
    // MarkArticleAsDamaged
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that marking an article as damaged sets <c>IsDamaged = true</c> in the database.
    /// </summary>
    [Fact]
    public async Task MarkArticleAsDamaged_WhenExists_SetsIsDamagedTrue()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id, "Vasija Intacta", isDamaged: false);

        var result = await ArticleApp.MarkArticleAsDamaged(article.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_DAMAGED, result.Message);
        var damaged = Context.Articles.AsNoTracking().First(a => a.Id == article.Id);
        Assert.True(damaged.IsDamaged);
    }

    /// <summary>
    /// Verifies that marking an already-damaged article as damaged again is idempotent.
    /// </summary>
    [Fact]
    public async Task MarkArticleAsDamaged_WhenAlreadyDamaged_ReturnsSuccess()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id, "Vasija Rota", isDamaged: true);

        var result = await ArticleApp.MarkArticleAsDamaged(article.Id);

        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Verifies that attempting to mark a non-existent article as damaged returns a failure response.
    /// </summary>
    [Fact]
    public async Task MarkArticleAsDamaged_WhenNotExists_ReturnsFailure()
    {
        var result = await ArticleApp.MarkArticleAsDamaged(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_QUERY_EMPTY, result.Message);
    }

    // ─────────────────────────────────────────────
    // RelocateArticle
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that relocating an article updates its <c>IdMuseum</c> foreign key in the database.
    /// </summary>
    [Fact]
    public async Task RelocateArticle_WhenBothExist_UpdatesIdMuseumInDatabase()
    {
        var origin = SeedMuseum("Museo Origen");
        var destination = SeedMuseum("Museo Destino");
        var article = SeedArticle(origin.Id, "Vasija");
        var request = new RelocateArticleRequestViewModel { NewMuseumId = destination.Id };

        var result = await ArticleApp.RelocateArticle(article.Id, request);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_UPDATE, result.Message);
        var relocated = Context.Articles.AsNoTracking().First(a => a.Id == article.Id);
        Assert.Equal(destination.Id, relocated.IdMuseum);
    }

    /// <summary>
    /// Verifies that attempting to relocate a non-existent article returns a failure response.
    /// </summary>
    [Fact]
    public async Task RelocateArticle_WhenArticleNotExists_ReturnsFailure()
    {
        var museum = SeedMuseum();
        var request = new RelocateArticleRequestViewModel { NewMuseumId = museum.Id };

        var result = await ArticleApp.RelocateArticle(999, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_QUERY_EMPTY, result.Message);
    }

    /// <summary>
    /// Verifies that relocating to a non-existent museum returns a failure response
    /// and does not modify the article.
    /// </summary>
    [Fact]
    public async Task RelocateArticle_WhenDestinationMuseumNotExists_ReturnsFailureAndDoesNotModify()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id);
        var request = new RelocateArticleRequestViewModel { NewMuseumId = 999 };

        var result = await ArticleApp.RelocateArticle(article.Id, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_QUERY_EMPTY, result.Message);
        var unchanged = Context.Articles.AsNoTracking().First(a => a.Id == article.Id);
        Assert.Equal(museum.Id, unchanged.IdMuseum);
    }

    /// <summary>
    /// Verifies that relocating an article to its current museum returns success immediately
    /// without issuing an unnecessary database write.
    /// </summary>
    [Fact]
    public async Task RelocateArticle_WhenSameMuseum_ReturnsSuccessWithoutModifying()
    {
        var museum = SeedMuseum();
        var article = SeedArticle(museum.Id);
        var request = new RelocateArticleRequestViewModel { NewMuseumId = museum.Id };

        var result = await ArticleApp.RelocateArticle(article.Id, request);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_UPDATE, result.Message);
        var unchanged = Context.Articles.AsNoTracking().First(a => a.Id == article.Id);
        Assert.Equal(museum.Id, unchanged.IdMuseum);
    }

    // ─────────────────────────────────────────────
    // BulkRegisterArticles
    // ─────────────────────────────────────────────

    /// <summary>
    /// Verifies that all articles in a valid batch are inserted in a single operation.
    /// </summary>
    [Fact]
    public async Task BulkRegisterArticles_WithValidItems_InsertsAllRows()
    {
        var museum = SeedMuseum();
        var items = new List<ArticleRequestViewModel>
        {
            new() { Name = "Vasija",    IdMuseum = museum.Id, IsDamaged = false },
            new() { Name = "Cuadro",    IdMuseum = museum.Id, IsDamaged = false },
            new() { Name = "Escultura", IdMuseum = museum.Id, IsDamaged = false }
        };

        var result = await ArticleApp.BulkRegisterArticles(items);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data > 0);
        Assert.Equal(ReplyMessages.MESSAGE_SAVE, result.Message);
        Assert.Equal(3, Context.Articles.Count());
    }

    /// <summary>
    /// Verifies that sending an empty list returns a failure response and inserts nothing.
    /// </summary>
    [Fact]
    public async Task BulkRegisterArticles_WhenEmptyList_ReturnsFailureAndInsertsNothing()
    {
        var result = await ArticleApp.BulkRegisterArticles(new List<ArticleRequestViewModel>());

        Assert.False(result.IsSuccess);
        Assert.Equal(ReplyMessages.MESSAGE_QUERY_EMPTY, result.Message);
        Assert.Equal(0, Context.Articles.Count());
    }

    /// <summary>
    /// Verifies that if one item in the batch fails validation, the entire batch is rejected
    /// and no rows are inserted.
    /// </summary>
    [Fact]
    public async Task BulkRegisterArticles_WhenOneItemIsInvalid_AbortsEntireBatchAndInsertsNothing()
    {
        var museum = SeedMuseum();
        var items = new List<ArticleRequestViewModel>
        {
            new() { Name = "Vasija Válida",   IdMuseum = museum.Id },
            new() { Name = null,              IdMuseum = museum.Id },
            new() { Name = "Cuadro Válido",   IdMuseum = museum.Id }
        };

        var result = await ArticleApp.BulkRegisterArticles(items);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains("item index 1", result.Message!);
        Assert.Equal(0, Context.Articles.Count());
    }

    /// <summary>
    /// Verifies that <c>CreatedAt</c> is automatically stamped on every inserted article.
    /// </summary>
    [Fact]
    public async Task BulkRegisterArticles_StampsCreatedAtOnAllItems()
    {
        var museum = SeedMuseum();
        var before = DateTime.Now.AddSeconds(-1);
        var items = new List<ArticleRequestViewModel>
        {
            new() { Name = "A", IdMuseum = museum.Id },
            new() { Name = "B", IdMuseum = museum.Id }
        };

        await ArticleApp.BulkRegisterArticles(items);

        var inserted = Context.Articles.AsNoTracking().ToList();
        Assert.All(inserted, a => Assert.True(a.CreatedAt >= before));
    }
}
