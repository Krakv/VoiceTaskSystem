using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace SpeechProcessingService.Infrastructure.Utils;

/// <summary>
/// Универсальная утилита для скачивания файлов по URL.
/// Автоматически определяет тип ссылки и выбирает подходящий способ загрузки:
/// для Google Drive используется специальный обработчик, для остальных — стандартный HTTP.
/// </summary>
public static class FileDownloader
{
    /// <summary>
    /// Проверяет наличие файла по указанному локальному пути и скачивает его если он отсутствует.
    /// Родительские директории создаются автоматически.
    /// </summary>
    /// <param name="url">
    /// Прямая HTTP(S)-ссылка или публичная ссылка Google Drive на файл.
    /// </param>
    /// <param name="localPath">
    /// Полный локальный путь для сохранения файла, например <c>data/models/model.bin</c>.
    /// </param>
    /// <param name="logger">Экземпляр логгера для записи прогресса и ошибок.</param>
    /// <exception cref="ArgumentException">
    /// Выбрасывается если передана некорректная ссылка Google Drive.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Выбрасывается если HTTP-запрос завершился ошибкой.
    /// </exception>
    public static async Task EnsureFileAsync(string url, string localPath, ILogger logger)
    {
        if (File.Exists(localPath))
        {
            logger.LogInformation("{LocalPath} already exists", localPath);
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);

        if (url.Contains("drive.google.com"))
        {
            await GoogleDriveFileDownloader.DownloadGoogleDriveFileAsync(ExtractDriveFileId(url), localPath, logger);
        }
        else
        {
            await DownloadHttpAsync(url, localPath, logger);
        }
    }

    /// <summary>
    /// Скачивает файл по стандартной HTTP(S)-ссылке с поддержкой стриминга,
    /// чтобы избежать загрузки всего содержимого в память.
    /// </summary>
    /// <param name="url">Прямая HTTP(S)-ссылка на файл.</param>
    /// <param name="localPath">Полный локальный путь для сохранения файла.</param>
    /// <param name="logger">Экземпляр логгера для записи прогресса и ошибок.</param>
    /// <exception cref="HttpRequestException">
    /// Выбрасывается если сервер вернул код ошибки.
    /// </exception>
    private static async Task DownloadHttpAsync(string url, string localPath, ILogger logger)
    {
        logger.LogInformation("Downloading {Url} via HTTP...", url);

        using var http = new HttpClient();
        http.Timeout = TimeSpan.FromMinutes(10);

        using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
        await response.Content.CopyToAsync(fs);

        logger.LogInformation("Saved to {LocalPath}", localPath);
    }

    /// <summary>
    /// Извлекает идентификатор файла из публичной ссылки Google Drive.
    /// </summary>
    /// <param name="url">
    /// Публичная ссылка вида <c>https://drive.google.com/file/d/FILE_ID/view?usp=sharing</c>.
    /// </param>
    /// <returns>Идентификатор файла Google Drive.</returns>
    /// <exception cref="ArgumentException">
    /// Выбрасывается если ссылка не содержит корректного идентификатора файла.
    /// </exception>
    private static string ExtractDriveFileId(string url)
    {
        var match = Regex.Match(url, @"\/d\/([a-zA-Z0-9_-]+)");
        if (!match.Success)
            throw new ArgumentException("Некорректная ссылка Google Drive", nameof(url));

        return match.Groups[1].Value;
    }
}