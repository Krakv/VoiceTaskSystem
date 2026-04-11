using TaskManager.Shared.Domain.Entities;

namespace TaskManager.Calendar.Application.Interfaces;

public interface ICalendarIcsGenerator
{
    string Generate(CalendarEvent e);
}
