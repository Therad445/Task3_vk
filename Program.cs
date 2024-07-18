using System;
using System.Net.Http;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // URL страницы Steam с топ-продажами в РФ
        string url = "https://store.steampowered.com/search/?filter=topsellers&cc=RU";

        // Создаем HttpClient для отправки запроса
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Отправляем GET запрос и получаем HTML страницу
                HttpResponseMessage response = client.GetAsync(url).Result;
                string htmlContent = response.Content.ReadAsStringAsync().Result;

                // Используем регулярное выражение для поиска названий игр и их цен
                string patternTitle = @"<span\s+class=""title"">(.+?)</span>";
                MatchCollection matchesTitle = Regex.Matches(htmlContent, patternTitle);

                string patternPrice = @"<div\s+class=""discount_final_price\s*(?:free)?\"">(.*?)</div>";
                MatchCollection matchesPrice = Regex.Matches(htmlContent, patternPrice);

                // Выводим топ-10 игр и их цен
                int count = Math.Min(10, matchesTitle.Count);
                for (int i = 0; i < count; i++)
                {
                    string title = matchesTitle[i].Groups[1].Value.Trim();
                    string price = matchesPrice[i].Groups[1].Value.Trim();

                    // Удаление HTML тегов из цены (оставляем только текст)
                    price = Regex.Replace(price, "<.*?>", "");

                    Console.WriteLine($"{i + 1}. {title} - Цена: {price}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при выполнении запроса: {ex.Message}");
            }
        }
    }
}
