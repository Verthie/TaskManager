using System;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options) // <= Passing options to AppDbContext to create instance of DbContext with these options and inherit from it
{
	// public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } <= Another way of passing options to DbContext - through constructor
	public DbSet<TaskItem> Tasks { get; set; }
}
