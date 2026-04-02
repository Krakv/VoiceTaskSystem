namespace SpeechProcessingService.API.DTOs;

public record ProcessAudioDto(IFormFile FormFile, string UserTimeZone) : FormFileDto(FormFile);
