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


        public async Task<BaseResponse<BaseEntityResponse<ArticleResponseViewModel>>> ListArticles(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<ArticleResponseViewModel>>();
            var articles = await _unitOfWork.Article.ListArticles(filters);

            if (articles is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<ArticleResponseViewModel>>(articles);
                response.Message = ReplyMessages.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public Task<BaseResponse<BaseEntityResponse<ArticleResponseViewModel>>> ListAllArticles()
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<ArticleResponseViewModel>> GetArticleById(int id)
        {
            var response = new BaseResponse<ArticleResponseViewModel>();
            var article = await _unitOfWork.Article.GetByIdAsync(id);

            if (article is not null)
            {
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

        public async Task<BaseResponse<BaseEntityResponse<ArticleResponseViewModel>>> ListArticlesByMuseum(BaseFiltersRequest filters, int museumId)
        {
            var response = new BaseResponse<BaseEntityResponse<ArticleResponseViewModel>>();
            var articles = await _unitOfWork.Article.GetArticlesByMuseum(filters, museumId);

            if (articles is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<ArticleResponseViewModel>>(articles);
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

        public async Task<BaseResponse<bool>> EditArticle(int categoryId, ArticleRequestViewModel requestViewModel)
        {
            var response = new BaseResponse<bool>();
            var articleEdit = await GetArticleById(categoryId);

            if (articleEdit is null)
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
                article.Id = categoryId;
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

            if (articleDelete is null)
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
    }
}
