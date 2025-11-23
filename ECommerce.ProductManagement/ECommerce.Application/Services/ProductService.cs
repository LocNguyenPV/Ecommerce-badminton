using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        ProductQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var pagedProducts = await _repository.GetPagedAsync(parameters, cancellationToken);

        return new PagedResult<ProductDto>
        {
            Items = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts.Items),
            TotalCount = pagedProducts.TotalCount,
            Page = pagedProducts.Page,
            PageSize = pagedProducts.PageSize
        };
    }

    public async Task<ProductDetailDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        return product == null ? null : _mapper.Map<ProductDetailDto>(product);
    }

    public async Task<ProductDetailDto> CreateProductAsync(
        CreateProductDto createDto,
        CancellationToken cancellationToken = default)
    {
        // Check if SKU exists
        if (await _repository.ExistsBySKUAsync(createDto.SKU, cancellationToken))
            throw new ValidationException(new[] { "SKU already exists" });

        var product = _mapper.Map<Product>(createDto);
        product.ProductId = Guid.NewGuid();

        var created = await _repository.CreateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDetailDto>(created);
    }

    public async Task<ProductDetailDto> UpdateProductAsync(
        Guid id,
        UpdateProductDto updateDto,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found");

        _mapper.Map(updateDto, product);
        product.UpdatedAt = DateTime.Now.Ticks;

        var updated = await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDetailDto>(updated);
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found");

        product.IsActive = false; // Soft delete
        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
