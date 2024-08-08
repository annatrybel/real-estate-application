using Microsoft.AspNetCore.Mvc;

namespace WebApp.Models.ViewModels
{
    public class MortgagePaymentVM
    {
        public decimal LoanAmount { get; set; }
        public double InterestRate { get; set; }
        public int LoanTerm { get; set; }
        public decimal? MonthlyPayment { get; set; }
        public int ProductId { get; set; }
    }
}

