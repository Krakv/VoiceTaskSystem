using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.RegularExpressions;

namespace SpeechProcessingService.Infrastructure.Utils;

/// <summary>
/// Утилита для скачивания публично доступных файлов с Google Drive
/// </summary>
public static class GoogleDriveFileDownloader
{
    /// <summary>
    /// Скачивает публично доступный файл с Google Drive по указанному локальному пути.
    /// Обрабатывает как маленькие файлы (прямое скачивание), так и большие,
    /// для которых Google требует токен подтверждения из-за проверки на вирусы.
    /// </summary>
    /// <param name="fileId">
    /// Идентификатор файла на Google Drive, извлечённый из ссылки на файл.
    /// Например: <c>1A2B3C4D5E</c> из <c>https://drive.google.com/file/d/1A2B3C4D5E/view</c>.
    /// </param>
    /// <param name="destination">
    /// Полный локальный путь, по которому будет сохранён скачанный файл.
    /// Например: <c>C:\Downloads\model.bin</c>. Родительские директории создаются автоматически.
    /// </param>
    /// <param name="logger">Экземпляр логгера для записи прогресса и ошибок.</param>
    /// <exception cref="HttpRequestException">
    /// Выбрасывается если HTTP-запрос завершился ошибкой или файл недоступен публично.
    /// </exception>
    /// <exception cref="IOException">
    /// Выбрасывается если файл назначения не удалось создать или записать.
    /// </exception>
    public static async Task DownloadGoogleDriveFileAsync(string fileId, string destination, ILogger logger)
    {
        var cookies = new CookieContainer();
        using var handler = new HttpClientHandler
        {
            CookieContainer = cookies,
            AllowAutoRedirect = true
        };
        using var http = new HttpClient(handler);
        http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

        string url = $"https://drive.google.com/uc?export=download&id={fileId}";

        // Первый запрос — для маленьких файлов сразу вернёт контент,
        // для больших — HTML-страницу с формой подтверждения
        using var firstResponse = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        firstResponse.EnsureSuccessStatusCode();

        string? downloadUrl = null;

        if (IsHtmlResponse(firstResponse))
        {
            string html = await firstResponse.Content.ReadAsStringAsync();
            downloadUrl = ExtractDownloadUrl(html, fileId);

            if (downloadUrl == null)
            {
                var uuidMatch = Regex.Match(html, @"name=""uuid""\s+value=""([^""]+)""");
                if (uuidMatch.Success)
                {
                    string uuid = uuidMatch.Groups[1].Value;
                    downloadUrl = $"https://drive.usercontent.google.com/download?id={fileId}&export=download&authuser=0&confirm=t&uuid={uuid}";
                }
            }
        }

        // Если HTML не встретился — файл уже скачивается напрямую
        Stream contentStream;
        HttpResponseMessage? downloadResponse = null;

        if (downloadUrl != null)
        {
            downloadResponse = await http.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            downloadResponse.EnsureSuccessStatusCode();
            contentStream = await downloadResponse.Content.ReadAsStreamAsync();
        }
        else
        {
            contentStream = await firstResponse.Content.ReadAsStreamAsync();
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

            await using var fs = File.Create(destination);
            await contentStream.CopyToAsync(fs);

            logger.LogInformation("Downloaded {Destination}", destination);
        }
        finally
        {
            await contentStream.DisposeAsync();
            downloadResponse?.Dispose();
        }
    }

    private static bool IsHtmlResponse(HttpResponseMessage response)
    {
        var contentType = response.Content.Headers.ContentType?.MediaType;
        return contentType != null && contentType.Contains("text/html");
    }

    private static string? ExtractDownloadUrl(string html, string fileId)
    {
        // Формат с confirm-токеном
        var confirmMatch = Regex.Match(html, @"confirm=([0-9A-Za-z_-]+)");
        if (confirmMatch.Success)
        {
            string token = confirmMatch.Groups[1].Value;
            return $"https://drive.google.com/uc?export=download&confirm={token}&id={fileId}";
        }

        // Прямая ссылка внутри HTML 
        var hrefMatch = Regex.Match(html,
            @"href=""(https://drive\.usercontent\.google\.com/download[^""]+)""");
        if (hrefMatch.Success)
            return WebUtility.HtmlDecode(hrefMatch.Groups[1].Value);

        return null;
    }
}