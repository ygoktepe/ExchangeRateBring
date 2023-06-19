using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExchangeRateBring.Controllers
{
    //https://www.tcmb.gov.tr/kurlar/today.xml

    [Route("api/[controller]")]
    [ApiController]

    public class ExchangeRateController:Controller
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet("{currency}")]
        public async Task<ActionResult<CurrencyExchangeRate>> GetExchangeRate(string currency)
        {
            try
            {
                string url = await _httpClient.GetStringAsync("https://www.tcmb.gov.tr/kurlar/today.xml");
                var urlDoc = XDocument.Parse(url);
                var currencyElement = urlDoc.Descendants("Currency").FirstOrDefault(x => x.Attribute("Kod")?.Value == currency);

                if (currencyElement == null)
                {
                    return NotFound();
                }

                var exchangeRate = new CurrencyExchangeRate
                {
                    Unit = currencyElement.Element("Unit").Value,
                    CurrencyCode = currencyElement.Attribute("Kod")?.Value,
                    BuyingRate = currencyElement.Element("ForexBuying").Value,
                    SellingRate = currencyElement.Element("ForexSelling").Value
                };
                return Ok(exchangeRate);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}