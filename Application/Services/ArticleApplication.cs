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

        public async Task<BaseResponse<bool>> RegisterArticle(ArticleRequestViewModel requestViewModel)
        {
            var response = new BaseResponse<bool>();
            var validationResult = await _validationRules.ValidateAsync(requestViewModel);

            if (!validationResult.IsValid) 
            {
                response.IsSuccess = false; 
                response.Message = ReplyMessages.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors;

                return response;
            }
             
            var article = _mapper.Map<Article>(requestViewModel);
            response.Data = await _unitOfWork.Article.RegisterAsync(article);

            if (response.Data)
            {
                response.IsSuccess = true;
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
    }
}
