using simple_artifacterp_back.DTOs;

namespace simple_artifacterp_back.Services
{
    public class QuotationPricingService
    {
        public QuotationPricingResult Calculate(QuotationVersionInput input, decimal suppliesTotal, decimal assetsTotal)
        {
            var extraCostsTotal = input.ExtraCosts?.Sum(c => c.Cost) ?? 0m;
            var baseCost = suppliesTotal + assetsTotal + input.LaborCost + extraCostsTotal;
            var profit = baseCost * (input.ProfitMargin / 100m);
            var subTotal = baseCost + profit;
            var productTax = input.ProductTax ?? 0m;
            var totalCost = subTotal - input.Discount + productTax;

            return new QuotationPricingResult
            {
                ExtraCostsTotal = extraCostsTotal,
                Profit = profit,
                SubTotal = subTotal,
                TotalCost = totalCost
            };
        }
    }

    public class QuotationPricingResult
    {
        public decimal ExtraCostsTotal { get; set; }
        public decimal Profit { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalCost { get; set; }
    }
}
