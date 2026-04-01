using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class PaymentQrModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string QrUrl { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string PaymentUrl { get; set; } = string.Empty;

    public void OnGet() { }
}
