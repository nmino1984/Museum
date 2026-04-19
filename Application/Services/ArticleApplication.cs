using Application.Commons.Bases;
using Application.Interfaces;
using Application.Validators;
using Application.ViewModels.Request;
using Application.ViewModels.Response;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Utilities.Statics;

namespace Application.Services
{
    public class ArticleApplication : IArticleApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ArticleValidator _validationRules;

        public ArticleApplication(IUnitOfWork unitOfWork, IMapper mapper, ArticleValidator validationRules)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationRules = validationRules;
        }

        public async Task<BaseResponse<IEnumerable<ArticleResponseViewModel>>> ListAllArticles()
        {
            var response = new BaseResponse<IEnumerable<ArticleResponseViewModel>>();
            var articles = await _unitOfWork.Article.GetAllAsync();
            var museums = await _unitOfWork.Museum.GetAllAsync();


            if (articles is not null)
            {
                foreach (var item in articles)
                {
                    var idMuseum = item.IdMuseum;
                    var museum = museums.Where(w => w.Id == idMuseum).FirstOrDefault();
                    item.IdMuseumNavigation = museum!;

                }
                response.IsSuccess = true;
                response.Data = _mapper.Map<IEnumerable<ArticleResponseViewModel>>(articles);
                response.Message = ReplyMessages.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<ArticleResponseViewModel>> GetArticleById(int articleId)
        {
            var response = new BaseResponse<ArticleResponseViewModel>();
            var article = await _unitOfWork.Article.GetByIdAsync(articleId);

            if (article is not null)
            {
                int idMuseum = article.IdMuseum;
                var museum = await _unitOfWork.Museum.GetByIdAsync(idMuseum);

                article.IdMuseumNavigation = museum;

                response.IsSuccess = true;
                response.Data = _mapper.Map<ArticleResponseViewModel>(article);
                response.Message = ReplyMessages.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }


            return response;

        }

        public async Task<BaseResponse<int>> RegisterArticle(ArticleRequestViewModel requestViewModel)
        {
            var response = new BaseResponse<int>();
            var validationResult = await _validationRules.ValidateAsync(requestViewModel);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors;

                return response;
            }

            var article = _mapper.Map<Article>(requestViewModel);
            var success = await _unitOfWork.Article.RegisterAsync(article);

            if (success)
            {
                response.IsSuccess = true;
                response.Data = article.Id;
                response.Message = ReplyMessages.MESSAGE_SAVE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditArticle(int articleId, ArticleRequestViewModel requestViewModel)
        {
            var response = new BaseResponse<bool>();
            var articleEdit = await GetArticleById(articleId);

            if (articleEdit.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }
            else 
            {
                var validationResult = await _validationRules.ValidateAsync(requestViewModel);

                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessages.MESSAGE_VALIDATE;
                    response.Errors = validationResult.Errors;

                    return response;
                }

                var article = _mapper.Map<Article>(requestViewModel);
                article.Id = articleId;
                response.Data = await _unitOfWork.Article.EditAsync(article);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessages.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessages.MESSAGE_FAILED;
                }
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteArticle(int articleId)
        {
            var response = new BaseResponse<bool>();
            var articleDelete = await GetArticleById(articleId);

            if (articleDelete is null || articleDelete.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }
            else 
            {
                response.Data = await _unitOfWork.Article.DeleteAsync(articleId);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessages.MESSAGE_DELETE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessages.MESSAGE_FAILED;
                }
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveArticle(int articleId)
        {
            var response = new BaseResponse<bool>();
            var articleDelete = await GetArticleById(articleId);

            if (articleDelete is null || articleDelete.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }
            else
            {
                response.Data = await _unitOfWork.Article.RemoveAsync(articleId);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessages.MESSAGE_REMOVED;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessages.MESSAGE_FAILED;
                }
            }

            return response;
        }

        /// <summary>
        /// Validates every item in the collection using the configured <see cref="ArticleValidator"/>.
        /// If all items pass, inserts them in a single database round-trip via <c>BulkRegisterAsync</c>.
        /// If any item fails validation, the method returns immediately with the first set of errors found
        /// and no rows are written to the database.
        /// </summary>
        /// <param name="requestViewModels">Collection of articles to create.</param>
        public async Task<BaseResponse<int>> BulkRegisterArticles(IEnumerable<ArticleRequestViewModel> requestViewModels)
        {
            var response = new BaseResponse<int>();
            var items = requestViewModels.ToList();

            if (items.Count == 0)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
                return response;
            }

            for (int i = 0; i < items.Count; i++)
            {
                var validationResult = await _validationRules.ValidateAsync(items[i]);

                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = $"{ReplyMessages.MESSAGE_VALIDATE} (item index {i})";
                    response.Errors = validationResult.Errors;
                    return response;
                }
            }

            var articles = _mapper.Map<IEnumerable<Article>>(items);
            var success = await _unitOfWork.Article.BulkRegisterAsync(articles);

            if (success)
            {
                response.IsSuccess = true;
                response.Data = items.Count;
                response.Message = ReplyMessages.MESSAGE_SAVE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_FAILED;
            }

            return response;
        }

        /// <summary>
        /// Marks the specified article as damaged. The operation is rejected if the article
        /// does not exist or has already been soft-deleted.
        /// Only the <c>IsDamaged</c> and <c>UpdatedAt</c> columns are modified in the database.
        /// </summary>
        /// <param name="articleId">Primary key of the article to mark as damaged.</param>
        public async Task<BaseResponse<bool>> MarkArticleAsDamaged(int articleId)
        {
            var response = new BaseResponse<bool>();
            var existing = await GetArticleById(articleId);

            if (existing.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
                return response;
            }

            response.Data = await _unitOfWork.Article.MarkAsDamagedAsync(articleId);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessages.MESSAGE_DAMAGED;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_FAILED;
            }

            return response;
        }

        /// <summary>
        /// Moves an article to a different museum by replacing its <c>IdMuseum</c> foreign key.
        /// The method verifies that both the article and the target museum exist and are active
        /// (not soft-deleted) before persisting the change.
        /// Only <c>IdMuseum</c> and <c>UpdatedAt</c> are modified; all other fields remain unchanged.
        /// </summary>
        /// <param name="articleId">Primary key of the article to relocate.</param>
        /// <param name="requestViewModel">DTO containing the destination museum identifier.</param>
        public async Task<BaseResponse<bool>> RelocateArticle(int articleId, RelocateArticleRequestViewModel requestViewModel)
        {
            var response = new BaseResponse<bool>();

            var existingArticleResponse = await GetArticleById(articleId);
            if (existingArticleResponse.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
                return response;
            }

            var targetMuseum = await _unitOfWork.Museum.GetByIdAsync(requestViewModel.NewMuseumId);
            if (targetMuseum is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
                return response;
            }

            if (existingArticleResponse.Data.IdMuseum == requestViewModel.NewMuseumId)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessages.MESSAGE_UPDATE;
                response.Data = true;
                return response;
            }

            var article = await _unitOfWork.Article.GetByIdAsync(articleId);
            article.IdMuseum = requestViewModel.NewMuseumId;

            response.Data = await _unitOfWork.Article.EditAsync(article);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessages.MESSAGE_UPDATE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_FAILED;
            }

            return response;
        }
    }
}
