namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetProjects;

public record GetProjectsResponse(List<string?>? Projects, int TotalCount);
