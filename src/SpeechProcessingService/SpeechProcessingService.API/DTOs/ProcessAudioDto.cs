namespace SpeechProcessingService.API.DTOs;

public record ProcessAudioDto(Guid CommandRequestId, IFormFile FormFile, string UserTimeZone) : FormFileDto(FormFile);
