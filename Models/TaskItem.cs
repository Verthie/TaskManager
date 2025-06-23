using System;

namespace TaskManager.Models;

public class TaskItem
{
	public int Id { get; set; }
	public required string Title { get; set; }
	public string? Description { get; set; }
	public bool CompletionStatus { get; set; }
}
