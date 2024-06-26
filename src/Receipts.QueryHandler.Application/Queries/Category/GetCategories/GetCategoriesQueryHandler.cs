﻿using Contracts.Web.Models;
using Contracts.Web.Services.Auth;
using MediatR;
using Receipts.QueryHandler.Application.Mappers;
using Receipts.QueryHandler.Domain.Interfaces;

namespace Receipts.QueryHandler.Application.Queries.Category.GetCategories
{
    public class GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IAuthService authService)
                : IRequestHandler<GetCategoriesQuery, GetCategoriesResponse>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IAuthService _authService = authService;
        public async Task<GetCategoriesResponse> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            int tenantId = _authService.GetTenant();
            Guid userId = _authService.GetUser();

            var authModel = new AuthModel(tenantId, userId);

            var domainFilters = request.GetReceiptsRequest.ToDomainFilters(authModel);
            var categoriesQueryResult = await _categoryRepository.GetCategoriesAsync(domainFilters);
            return categoriesQueryResult.ToResponse(request.GetReceiptsRequest.PageFilter);
        }
    }
}
