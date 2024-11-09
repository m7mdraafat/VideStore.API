using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.Interfaces;
using VideStore.Shared.DTOs.Requests.Products;

namespace VideStore.Application.Mapping.Resolvers
{
    public class ProductSizesResolver : IValueResolver<ProductRequest, Product, ICollection<ProductSize>>
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductSizesResolver(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ICollection<ProductSize> Resolve(ProductRequest source, Product destination, ICollection<ProductSize> destMember, ResolutionContext context)
        {
            var sizes = new List<ProductSize>();

            // Fetch sizes based on the IDs
            foreach (var sizeId in source.SizeIds)
            {
                var size = unitOfWork.Repository<Size>().GetEntityAsync(sizeId).Result; // Consider using async properly
                if (size != null)
                {
                    sizes.Add(new ProductSize
                    {
                        SizeId = sizeId,
                        Size = size // Assuming Size is a navigation property in ProductSize
                    });
                }
            }

            return sizes;
        }
    }
}