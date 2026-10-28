namespace FocusFlow.Shared.Tasks;

// Counts shown on the dashboard
public record TaskSummary(int Pending, int Completed, int Overdue);
