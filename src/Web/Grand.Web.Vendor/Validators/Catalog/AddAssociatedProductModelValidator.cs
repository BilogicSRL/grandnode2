using FluentValidation;
using Grand.Infrastructure;
using Grand.Infrastructure.Validators;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Web.Vendor.Models.Catalog;
using Grand.Business.Core.Interfaces.Catalog.Products;

namespace Grand.Web.Vendor.Validators.Catalog
{
    public class AddAssociatedProductModelValidator : BaseGrandValidator<ProductModel.AddAssociatedProductModel>
    {
        public AddAssociatedProductModelValidator(
            IEnumerable<IValidatorConsumer<ProductModel.AddAssociatedProductModel>> validators,
            ITranslationService translationService, IProductService productService, IWorkContext workContext)
            : base(validators)
        {
            RuleFor(x => x).MustAsync(async (x, _, _) =>
            {
                var product = await productService.GetProductById(x.ProductId);
                if (product == null) return true;
                return product.VendorId == workContext.CurrentVendor.Id;
            }).WithMessage(translationService.GetResource("Vendor.Catalog.Products.Permisions"));
        }
    }
}