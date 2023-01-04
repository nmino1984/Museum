using Domain.Entities;

namespace Domain.Entities;

public partial class Museum : BaseEntity
{
    public int Theme { get; set; }

    public virtual ICollection<Article> Articles { get; } = new List<Article>();
}
