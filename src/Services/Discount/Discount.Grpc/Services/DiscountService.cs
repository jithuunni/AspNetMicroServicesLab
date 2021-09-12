using System;
using Grpc.Core;
using AutoMapper;
using System.Threading.Tasks;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Microsoft.Extensions.Logging;
using Discount.Grpc.Entities;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository discountRepository;
        private readonly ILogger<DiscountService> logger;
        private readonly IMapper mapper;

        public DiscountService(IDiscountRepository discountRepository, ILogger<DiscountService> logger, IMapper mapper)
        {
            this.discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await discountRepository.GetDiscount(request.ProductName);

            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discoutn with ProductName={request.ProductName} is not found."));
            }
            logger.LogInformation("Discount is retrieved for ProductName:{productName}, Amount:{amount}", coupon.ProductName, coupon.Amount);

            return mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = mapper.Map<Coupon>(request.Coupon);

            await discountRepository.CreateDiscount(coupon);
            logger.LogInformation("Discount is successfully created. ProductName: {productName}", coupon.ProductName);

            return mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = mapper.Map<Coupon>(request.Coupon);

            await discountRepository.CreateDiscount(coupon);
            logger.LogInformation("Discount is successfully updated. ProductName: {productName}", coupon.ProductName);

            return mapper.Map<CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            bool deleted = await discountRepository.DeleteDiscount(request.ProductName);
            logger.LogInformation("Discount is successfully deleted. ProductName: {productName}", request.ProductName);

            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };
            return response;
        }
    }
}
