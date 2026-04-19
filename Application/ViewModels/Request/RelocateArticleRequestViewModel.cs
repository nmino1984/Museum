namespace Application.ViewModels.Request
{
    /// <summary>
    /// Payload required to move an article from its current museum to a different one.
    /// </summary>
    public class RelocateArticleRequestViewModel
    {
        /// <summary>
        /// Identifier of the destination museum. Must correspond to an existing, non-deleted museum.
        /// </summary>
        public int NewMuseumId { get; set; }
    }
}
