
namespace WebApp.Models.ViewModels
{
    public class CreditCalculatorVM
    {
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal LoanAmount { get; set; }
        public double InterestRate { get; set; }
        public int LoanTerm { get; set; }
        public int ProductId { get; set; }
        public decimal MonthlyPaymentLimit { get; set; }
    }
}

