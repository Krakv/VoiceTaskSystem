namespace SpeechProcessingService.Application.Models;

public class TaskItem
{
    public string? ProjectName { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public TaskItemStatus? Status { get; set; }

    public TaskItemPriority? Priority { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public string? ParentTaskName { get; set; }
}
