using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Statics;

namespace Application.ViewModels.Response
{
    public class MuseumResponseViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ThemeId { get; set; }
        public Themes Theme { get; set; }
        public List<ArticleResponseViewModel>? listArticles { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
