using Domain.Entities;

namespace Domain.Entities;

public partial class Article : BaseEntity
{

    public bool? IsDamaged { get; set; }

    public int IdMuseum { get; set; }

    public virtual Museum IdMuseumNavigation { get; set; } = null!;
}
