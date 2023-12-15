using System.Text.Json;
using System.Text;

namespace CryptoConsoleApp
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private const string CoinGeckoBtcEthPriceUrl =
            "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=eth";

        private const string CoinGeckoBtcUsdPriceUrl = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd,usd-coin,eth";
        private static readonly Random random = new Random();
        
        private const string OrderEndpoint = "http://localhost:8085/api/order";
        private const string AuthTokenForBuyer = "AuthTokenForBuyer";
        private const string AuthTokenForSeller = "AuthTokenForSeller";

        static async Task Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            // Start Buy thread
            Thread buyThread = new Thread(async () => await PlaceOrder("buy", cancellationTokenSource.Token));
            buyThread.Start();

            // Start Sell thread
            Thread sellThread = new Thread(async () => await PlaceOrder("sell", cancellationTokenSource.Token));
            sellThread.Start();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            cancellationTokenSource.Cancel();
            buyThread.Join();
            sellThread.Join();
        }

        private static async Task PlaceOrder(string type, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //decimal btcUsdPrice = await GetLatestPrice(CoinGeckoBtcUsdPriceUrl, "usd");
                    decimal btcUsdcPrice = await GetLatestPrice(CoinGeckoBtcUsdPriceUrl, "usd-coin");
                    //decimal btcEthPrice = await GetLatestPrice(CoinGeckoBtcEthPriceUrl, "eth");
                    //decimal usdAmount = 10; 

                    // Get the equivalent amount in ETH for 10 USD
                    //decimal ethEquivalentForUsd = usdAmount / btcUsdPrice * btcEthPrice;
        
                    // Get a random value between 0 and the previous ETH equivalent value 
                    //decimal randomAmountInEth = (decimal)random.NextDouble() * ethEquivalentForUsd;

                    // Modify the order price based on randomAmountInEth
                    //decimal orderPrice = type == "buy" ? btcEthPrice - randomAmountInEth : btcEthPrice + randomAmountInEth;

                    decimal orderPrice = btcUsdcPrice + RandomDecimalBetween(0, 10);
                    
                    decimal orderAmount = OrderVolumeBuilder(0);
                    
                    var orderData = new
                    {
                        order = new
                        {
                            instrument = "btc_eth",
                            type,
                            amount = orderAmount,
                            price = orderPrice,
                            activationPrice = 0m,
                            isLimit = true,
                            isStop = false,
                            selfMatchStrategy = "cancelAggressor",
                            selfMatchToken = Guid.NewGuid() 
                        }
                    };

                    var content = new StringContent(JsonSerializer.Serialize(orderData), Encoding.UTF8,
                        "application/json");

                    httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",  type == "sell" ? AuthTokenForSeller : AuthTokenForBuyer);

                    // Submit the order
                    var response = await httpClient.PostAsync(OrderEndpoint, content);
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($"Successfully placed a {type} order.");

                    await Task.Delay(10000); // Delay to prevent rate limiting
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error placing {type} order: {ex.Message}");
            }
        }
        
        private static async Task<decimal> GetLatestPrice(string url, string currency)
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);
            return jsonDoc.RootElement.GetProperty("bitcoin").GetProperty(currency).GetDecimal();
        }
        
        private static async Task<decimal> GetLastTradeAmountAsync()
        {
            // Placeholder for API call to get the last traded amount. Replace with actual API call and parsing.
            var response = await httpClient.GetAsync("https://api.coingecko.com/api/v3/coins/bitcoin/market_chart?vs_currency=eth&days=1&interval=minute");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
    
            using var jsonDoc = JsonDocument.Parse(responseBody);
            decimal lastTradeAmount = jsonDoc.RootElement
                .GetProperty("volumes") // Assuming the API returns a volume array
                .EnumerateArray()
                .Last() // Get the last entry which should be the most recent
                .GetDecimal();

            return lastTradeAmount; // return the actual last trade amount in BTC
        }
        
        public static decimal OrderVolumeBuilder(int baseVolume)
        {
            string orderVolumePart0 = baseVolume.ToString();
            string orderVolumePart1 = random.Next(0, 99).ToString("D2"); // Generates a random number between 0 and 99 and formats it with 2 digits.

            // If you need to print out the result, uncomment the following line:
            // Console.WriteLine("[+] Order Volume: " + $"{orderVolumePart0}.{orderVolumePart1}");

            string orderVolumeString = $"{orderVolumePart0}.{orderVolumePart1}";

            decimal orderVolume = decimal.Parse(orderVolumeString);

            return orderVolume;
        }
        
        public static decimal RandomDecimalBetween(decimal min, decimal max)
        {
            decimal range = max - min;
            double randomDouble = random.NextDouble();
            decimal randomInRange = Convert.ToDecimal(randomDouble) * range + min;
        
            return Math.Round(randomInRange, 2);
        }
    }
}