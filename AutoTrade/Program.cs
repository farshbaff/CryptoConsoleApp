using System.Text.Json;
using System.Text;

namespace CryptoConsoleApp
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private const string CoinGeckoBtcEthPriceUrl =
            "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=eth";

        //private const string CoinGeckoBtcUsdPriceUrl = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd-coin";
        private const string CoinGeckoBtcUsdPriceUrl =
            "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd";

        private static readonly Random random = new Random();

        private const string OrderEndpoint = "http://localhost:8085/api/order";

        private const string AuthTokenForBuyer =
            "eyJhbGciOiJSUzI1NiIsImtpZCI6InB1YmxpYzowMTYwZjIyNC0zYWVjLTQ1MTQtYjgzMi1kZjZhMDE1MTZhMDUiLCJ0eXAiOiJKV1QifQ.eyJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdCJdLCJjbGllbnRfaWQiOiJwaW5nX2xvY2FsIiwiZXhwIjoxNzA1MjI5MDA5LCJleHQiOnt9LCJpYXQiOjE3MDI2MzcwMDgsImlzcyI6Imh0dHBzOi8vYXV0aC1kZXZlbG9wLnRlYXJzb2Z0cmFkZXJzLmNvbS8iLCJqdGkiOiJlMGMxNzI3NC0zYWE0LTQ4ZDMtYTFmNS1kNWNlZWIwMjBmYTUiLCJuYmYiOjE3MDI2MzcwMDgsInNjcCI6WyJvcGVuaWQiLCJvZmZsaW5lX2FjY2VzcyJdLCJzdWIiOiJjb3JlaWQ6YWIzOGM5MmVmMTMyZjg5ZmU2YmMyNmMzNjRjYTk2ZWIwNzRjNGRhNDkxZGYifQ.EQhfkuseXfvZeP5fCUDSNHYHXKkJt2ZkV9urQp_HT_o6-D_jFgxUHdiacRkjoNDPKwk0FR6N1LZIbrjiMjN8KNcO153yXM7Wqou0piwWYzMgtlRSO6BKT_CZJhgevx1EjNY3C84B_N2mSOhRFVc8Ne7jtl-UZZkEh9SnQtmfB7Dw4E8rATYpJK70duh82huQPSSR0OZF32OwROLFzFqoWqavTVbZWeBBTma01PHc_9bckRDxLEd9XRspnfuqysjwBzMeCadAEH8QqBJ0KYF0ZXWy7q1sdZCgLmmEsM59mPyeUl0Zoe2PcKXZgmcAMybiIrhVFRQxBSFt5K5xAu_-2UgDjUhVzHCwOoN9CJh4ReZoi8QAEge9IaPySPBs6EKFkFjx8-bPCbden2CXs-CIQarRb3FxWIu1zUGDdzYJZFd4RbDy-OVpjvpXT8WCvazfI8uZguMKmLAKzM6XZVGTpJJjn7v5AKMYD-AoK8zsW56RyBjgQoZilvSqOn9BxBgb-H1ZeLuBjI_oihapDSRI_prdiKef19hDB_OdyfIdeOfLeb5HQvuVsOfnjJ8J45yvxl88294hjFZxljVROeCe1H6FFyeE0X42JMXzpp5RB795gtpHlnCu_Ggj-953Rm7-QwhGmOuXu6uykgZ3PfLYvPBzgWTLdYSJQ4Mw0W-ZkDI";

        private const string AuthTokenForSeller =
            "eyJhbGciOiJSUzI1NiIsImtpZCI6InB1YmxpYzowMTYwZjIyNC0zYWVjLTQ1MTQtYjgzMi1kZjZhMDE1MTZhMDUiLCJ0eXAiOiJKV1QifQ.eyJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdCJdLCJjbGllbnRfaWQiOiJwaW5nX2xvY2FsIiwiZXhwIjoxNzA1MjI5MTYzLCJleHQiOnt9LCJpYXQiOjE3MDI2MzcxNjIsImlzcyI6Imh0dHBzOi8vYXV0aC1kZXZlbG9wLnRlYXJzb2Z0cmFkZXJzLmNvbS8iLCJqdGkiOiJmNTUyYmE1Yi02Yzk3LTRjMzgtOTdiMC03MDkwYjRkNGVkMWEiLCJuYmYiOjE3MDI2MzcxNjIsInNjcCI6WyJvcGVuaWQiLCJvZmZsaW5lX2FjY2VzcyJdLCJzdWIiOiJjb3JlaWQ6YWI5MjFkYzYxNjRhYTczNjgyYzZhMmQ4ZjQ5MTc2OGE5ZDg0NmQ2OTc5NGIifQ.ivqCyEvgegA_fSahP1IjSeXePDv-j7sTkZO83_n5RJAIXOMnXAh9Jxdz5WsosZFH9uPuyr4IV7m8yhkRtgzrJkQ2KyUuJCJ6-d_DNkZ7bmePCFrYjNo25fgM7VlwAWdZv4gpnO8cR4HTI7qoIf18SGwpZzpdyKH-v8X_zBEFcAt-E0lrPN8EUodaTQ-qVqiicUZoERBH1yncTKswCofh396s-Squ9y3HLg9Xh_TueWOycLxLyoXDgivN6tahD6FBg6oPdC5xK38EmXmxIh1_-3jnesQelwUov-rrpCahOXrQW-vCEhIkHNKEq52X2jA4nKHANqTxRnDcUFQio_Xg-M0RKem7NCPA-P_lsrBISBWAjZYJbifoNXCm0Fc_e6sFqTtiBOJ61hw1Eef5h5kCQLpgcSyZkZfifacCJyAiK0WIcbxt7EMkvczkyaaH8YoKgHdkLIJU8VWZKIntsxcXTqs7hLcm6IGo1N8QG-kf12UcsYDX5QpkHBavocW1w9IvE7S-34w_aHZcdNSacetI9M9pofULm3GzGKA0m_dEHklPvRA4_RxaOPM9-wPcXFoUdv6AZcbY2TnPoJzpQrbiZ-HwK_nMAjSBlVln2moCfJNmvENYnlUVZnPwy1PcNe9q6mNCapZDc4Ou2fTKFEIyjwpdzEv0zyi2oCKhnjCFXKc";

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
            var count = 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //decimal btcUsdPrice = await GetLatestPrice(CoinGeckoBtcUsdPriceUrl, "usd");
                    //decimal btcUsdcPrice = await GetLatestPrice(CoinGeckoBtcUsdPriceUrl, "usd-coin");

                    decimal btcUsdcPrice = 40000;

                    try
                    {
                        if (count % 50 == 0)
                            btcUsdcPrice = await GetLatestPrice(CoinGeckoBtcUsdPriceUrl, "usd");
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Exception in fetching from CoinGecko");

                        await Task.Delay(2000);
                    }

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
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                            type == "sell" ? AuthTokenForSeller : AuthTokenForBuyer);

                    // Submit the order
                    try
                    {
                        var response = await httpClient.PostAsync(OrderEndpoint, content);
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(
                            @$"There was an error in submitting order to the Front-office {JsonSerializer.Serialize(orderData, new JsonSerializerOptions { WriteIndented = true })}");
                        
                        Console.WriteLine(e);
                    }

                    Console.WriteLine($"Successfully placed a {type} order.");

                    await Task.Delay(1000); // Delay to prevent rate limiting

                    count++;
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
            var response = await httpClient.GetAsync(
                "https://api.coingecko.com/api/v3/coins/bitcoin/market_chart?vs_currency=eth&days=1&interval=minute");
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
            string
                orderVolumePart1 =
                    random.Next(1, 99)
                        .ToString("D2"); // Generates a random number between 0 and 99 and formats it with 2 digits.

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