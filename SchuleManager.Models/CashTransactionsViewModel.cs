namespace SchuleManager.Models
{
    public class CashTransactionsViewModel
    {
        public Transactions CashTransaction { get; set; }
        public GlAccounts CashierAccount { get; set; }
        public Account AccountDetails { get; set; }
    }
}
