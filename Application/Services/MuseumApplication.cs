using AutoMapper;
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
using System.Collections.Generic;

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

        public async Task<BaseResponse<IEnumerable<MuseumResponseViewModel>>> ListAllMuseums()
        {
            var response = new BaseResponse<IEnumerable<MuseumResponseViewModel>>();
            var museums = await _unitOfWork.Museum.GetAllAsync();
            var articles = await _unitOfWork.Article.GetAllAsync();


            if (museums is not null)
            {
                foreach (var item in articles)
                {
                    var idMuseum = item.IdMuseum;
                    if (item.IdMuseum == idMuseum)
                    {
                        var museum = museums.Where(w => w.Id == idMuseum).FirstOrDefault();
                        item.IdMuseumNavigation = museum!;
                        museum!.Articles.Add(item);
                    }

                }
                response.IsSuccess = true;
                response.Data = _mapper.Map<IEnumerable<MuseumResponseViewModel>>(museums);
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
            var articles = await _unitOfWork.Article.GetAllAsync();

            if (museum is not null)
            {
                foreach (var article in articles)
                {
                    article.IdMuseumNavigation = museum;
                    if (article.IdMuseum == museum.Id)
                    {
                        museum.Articles.Add(article);
                    }
                }
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

        public async Task<BaseResponse<IEnumerable<ArticleResponseViewModel>>> GetArticlesByMuseum(int museumId)
        {
            var response = new BaseResponse<IEnumerable<ArticleResponseViewModel>>();
            var museum = await _unitOfWork.Museum.GetByIdAsync(museumId);

            if (museum is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
                return response;
            }

            var articles = await _unitOfWork.Article.GetAllAsync();
            articles = articles.Where(w => w.IdMuseum == museumId);

            foreach (var item in articles)
            {
                museum.Articles.Add(item);
                item.IdMuseumNavigation = museum;
            }

            response.IsSuccess = true;
            response.Data = _mapper.Map<IEnumerable<ArticleResponseViewModel>>(articles);
            response.Message = ReplyMessages.MESSAGE_QUERY;

            return response;
        }

        public async Task<BaseResponse<IEnumerable<MuseumResponseViewModel>>> GetMuseumsByTheme(int theme)
        {
            var response = new BaseResponse<IEnumerable<MuseumResponseViewModel>>();
            var result = await _unitOfWork.Museum.ListMuseumsByTheme(theme);
            var museums = result.Items ?? Enumerable.Empty<Museum>();

            if (museums.Any())
            {
                var articles = await _unitOfWork.Article.GetAllAsync();

                foreach (var item in articles)
                {
                    var museum = museums.FirstOrDefault(w => w.Id == item.IdMuseum);
                    if (museum is not null)
                    {
                        item.IdMuseumNavigation = museum;
                        museum.Articles.Add(item);
                    }
                }
            }

            response.IsSuccess = true;
            response.Data = _mapper.Map<IEnumerable<MuseumResponseViewModel>>(museums);
            response.Message = ReplyMessages.MESSAGE_QUERY;

            return response;
        }

        public async Task<BaseResponse<int>> RegisterMuseum(MuseumRequestViewModel requestViewModel)
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

            var museum = _mapper.Map<Museum>(requestViewModel);
            var success = await _unitOfWork.Museum.RegisterAsync(museum);

            if (success)
            {
                response.IsSuccess = true;
                response.Data = museum.Id;
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

            if (museumEdit.Data is null)
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

            if (museumDelete.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
                return response;
            }

            var articles = await _unitOfWork.Article.GetAllAsync();
            if (articles.Any(a => a.IdMuseum == museumId))
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_HAS_ARTICLES;
                return response;
            }

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

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveMuseum(int museumId)
        {
            var response = new BaseResponse<bool>();
            var museumDelete = await GetMuseumById(museumId);

            if (museumDelete.Data is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessages.MESSAGE_QUERY_EMPTY;
                return response;
            }

            var articles = await _unitOfWork.Article.GetAllAsync();
            foreach (var article in articles.Where(a => a.IdMuseum == museumId))
                await _unitOfWork.Article.RemoveAsync(article.Id);

            response.Data = await _unitOfWork.Museum.RemoveAsync(museumId);

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

            return response;
        }
    }
}
