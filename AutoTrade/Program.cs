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
            "eyJhbGciOiJSUzI1NiIsImtpZCI6InB1YmxpYzowMTYwZjIyNC0zYWVjLTQ1MTQtYjgzMi1kZjZhMDE1MTZhMDUiLCJ0eXAiOiJKV1QifQ.eyJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdCJdLCJjbGllbnRfaWQiOiJwaW5nX2xvY2FsIiwiZXhwIjoxNzA1MjQ0NzIxLCJleHQiOnt9LCJpYXQiOjE3MDI2NTI3MjEsImlzcyI6Imh0dHBzOi8vYXV0aC1kZXZlbG9wLnRlYXJzb2Z0cmFkZXJzLmNvbS8iLCJqdGkiOiIxZTRmNmYzMS1hYjUxLTQzM2UtYmFkYy02MjU3NzJiYjVjNTUiLCJuYmYiOjE3MDI2NTI3MjEsInNjcCI6WyJvcGVuaWQiLCJvZmZsaW5lX2FjY2VzcyJdLCJzdWIiOiJjb3JlaWQ6YWIzOGM5MmVmMTMyZjg5ZmU2YmMyNmMzNjRjYTk2ZWIwNzRjNGRhNDkxZGYifQ.s6GnmzvsLKJXR4AD4L0lmubJhdzt5wUN5yFl1ZWuvB8rKCw-cgIZye8DOoH1kD0Nuu8mtIBGSgeXrqjGR51l6byJWmsy5qAxUSYaj9JXg9YwWMpr7AzoryWwU5h5SQOjYzJue8Ap2ymdGz2pV5dThE-Jv2ukfw4YP284Q-FL6qiN_jQqTnMVk2LkrIIs2iRUw77koCX_ZAjpSApfg_wXI1LqN9DIPy7dtMDz2h5FakSAs03e8_0gYfJuIkSsXEXDvR0eg-v6dCMuVt8N54B397iumf26eNKBRP6ybiLVm2Wpj4RnPMG2QMUeK3_YvywqBWvJ8eTZ87ZTH3_csBxZZMIzyKd7y8ZVnxpcZl3kFOnGiTTeqo19q3mGqLuyyJA2vStqO02gW6ZZk6eQql8PHAWEr9nALFuxixgI34KYCi6lvVXs3WeKLPIpEAET1cs2Muecuy8jQLVVnlckINJa4aam1SYvx__-waxNyaCna8xwIGhMSyWgnRwuMrEtprraJSH3XFvVJFoA35DaTMrj16k4TpZNFrkNYTY4sFLSRMHK5xe_H9BF1bMDCQ4NsuR1kK-BLDi1v-5oQPAMImUwICyG3s23bomEWvxDNUjCjpBbnyv-oLGBE6Fk6PNIXVHxbrq55gqfJ-A1kfHNXFVT3doGQdU5ruthOj7zRVsXBZM";

        private const string AuthTokenForSeller =
            "eyJhbGciOiJSUzI1NiIsImtpZCI6InB1YmxpYzowMTYwZjIyNC0zYWVjLTQ1MTQtYjgzMi1kZjZhMDE1MTZhMDUiLCJ0eXAiOiJKV1QifQ.eyJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdCJdLCJjbGllbnRfaWQiOiJwaW5nX2xvY2FsIiwiZXhwIjoxNzA1MjQ0NzcyLCJleHQiOnt9LCJpYXQiOjE3MDI2NTI3NzEsImlzcyI6Imh0dHBzOi8vYXV0aC1kZXZlbG9wLnRlYXJzb2Z0cmFkZXJzLmNvbS8iLCJqdGkiOiIwZGMwMDg2MC01MDIwLTRmYzUtOGQyMC1jOTFjNGY0ZTc4MDUiLCJuYmYiOjE3MDI2NTI3NzEsInNjcCI6WyJvcGVuaWQiLCJvZmZsaW5lX2FjY2VzcyJdLCJzdWIiOiJjb3JlaWQ6YWI5MjFkYzYxNjRhYTczNjgyYzZhMmQ4ZjQ5MTc2OGE5ZDg0NmQ2OTc5NGIifQ.V56Kp0rqgukA4xQDpWlo7NLWbWbx4r1zKWR2zrDfK-SwahAQSdJTZ7kHuyiUnX9Cyp3ASJ3s867LPA-KIK_qzU_H-Wa3XH6zhjwvfPUGwIuM8KVaQyYqIzjKBwggyhxF0bI3FB_QONRF7C77JKkIGkIo1jYYeRcCaaTjB2aHWmnrmC4BkaEm0uGkIu36ALNDgp94WiBEj-EqjiqtwdvHlwiZ-bkB_os2nBE17Ur5WkE4AXIXD2VTstF5V1P1_pZMIvS9bIV0yZQ43232VZXltzUsJKndNpkIowC8oQC2jPnUSpbj_E4_48v8FPkFoptgRpTP8ZDJgv0KWEymwQvhyhHUwFV26Ti2NsUR4UIVspgMmzzykcF8DMUVHlAA3jgXiB5hqfPmdY__Lh7AUfF-nFmP_Q85FIueicDB8NsKZ-nVI7_DOVPsoekFdsWfnXl9ch4cl1OFDsI0weYBb6YRnXWtodzgkf-OnD8kvFxT2ktFIobvOSnV5cXxzyYoiBZafsRi_Ncx7lZc9RhxwF3TJmIQq4epCFYJqdTS4RJQ9pSaYV1P0Qgx3EYBfnxTmjVl_hShKyYZa0dPNuvT35MW_BpmdEBLXUeHy1R3LikYjjXgM7DpHjvbAMUgvqMS_abT6R2BgvMMFQNR1VQUvzjJ_J_nv4EBALDL06HctF1i_jA";

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
                        //var response = await httpClient.PostAsync(OrderEndpoint, content);
                        httpClient.PostAsync(OrderEndpoint, content);
                        //response.EnsureSuccessStatusCode();
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