using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class TaskItem
{
	public int Id { get; set; }
	public required string Title { get; set; }
	[MaxLength(60)]
	public string? Description { get; set; }
	public bool CompletionStatus { get; set; } = false;
	public DateTime? DueDate { get; set; }
}
