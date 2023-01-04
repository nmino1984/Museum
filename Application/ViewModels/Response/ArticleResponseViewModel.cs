namespace Application.ViewModels.Response
{
    public class ArticleResponseViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsDamaged { get; set; }
        public int IdMuseum { get; set; }
        public string? NameMuseum { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
