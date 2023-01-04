﻿using AutoMapper;
using FluentValidation;
using Application.Commons.Bases;
using Application.Interfaces;
using Application.Validators;
using Application.ViewModels.Request;
using Application.ViewModels.Response;
using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Utilities.Statics;

namespace Application.Services
{
    public class MuseumApplication : IMuseumApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MuseumValidator _validationRules;

        public MuseumApplication(IUnitOfWork unitOfWork, IMapper mapper, MuseumValidator validationRules)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validationRules = validationRules;
        }

        public async Task<BaseResponse<BaseEntityResponse<MuseumResponseViewModel>>> ListMuseums(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<MuseumResponseViewModel>>();
            var museums = await _unitOfWork.Museum.ListMuseums(filters);

            if (museums is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<MuseumResponseViewModel>>(museums);
                response.Message = ReplyMessages.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<MuseumSelectResponseViewModel>>> ListSelectMuseums()
        {
            var response = new BaseResponse<IEnumerable<MuseumSelectResponseViewModel>>();
            var museums = await _unitOfWork.Museum.GetAllAsync();

            if (museums is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<IEnumerable<MuseumSelectResponseViewModel>>(museums);
                response.Message = ReplyMessages.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<MuseumResponseViewModel>> GetMuseumById(int museumId)
        {
            var response = new BaseResponse<MuseumResponseViewModel>();
            var museum = await _unitOfWork.Museum.GetByIdAsync(museumId);

            if (museum is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<MuseumResponseViewModel>(museum);
                response.Message = ReplyMessages.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<ArticleRequestViewModel>>> GetArticlesByMuseum(int museumId)
        {
            var response = new BaseResponse<IEnumerable<ArticleRequestViewModel>>();
            var museum = await _unitOfWork.Museum.GetByIdAsync(museumId);
            var articles = _unitOfWork.Article.;

            if (museum is not null)
            {
                if (articles is not null)
                {
                    response.IsSuccess = true;
                    response.Data = (IEnumerable<ArticleRequestViewModel>?)_mapper.Map<ArticleRequestViewModel>(articles);
                    response.Message = ReplyMessages.MESSAGE_QUERY;

                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterMuseum(MuseumRequestViewModel requestViewModel)
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

            var museum = _mapper.Map<Museum>(requestViewModel);
            response.Data = await _unitOfWork.Museum.RegisterAsync(museum);

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

        public async Task<BaseResponse<bool>> EditMuseum(int museumId, MuseumRequestViewModel requestViewModel)
        {
            var response = new BaseResponse<bool>();
            var museumEdit = await GetMuseumById(museumId);

            if (museumEdit is null)
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

                var museum = _mapper.Map<Museum>(requestViewModel);
                museum.Id = museumId;
                response.Data = await _unitOfWork.Museum.EditAsync(museum);

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

        public async Task<BaseResponse<bool>> DeleteMuseum(int museumId)
        {
            var response = new BaseResponse<bool>();
            var museumDelete = await GetMuseumById(museumId);

            if (museumDelete is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
            }
            else
            {
                response.Data = await _unitOfWork.Museum.DeleteAsync(museumId);

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

        /*public async Task<BaseResponse<BaseEntityResponse<ArticleResponseViewModel>>> ListArticles(BaseFiltersRequest filters)
        {
            
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
        }*/
    }
}