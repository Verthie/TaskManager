using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Controllers;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Tests;

public class TaskControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TasksController _controller;

    // Setup (since we would have to write this in each function)
    public TaskControllerTests()
    {
        _context = GetDbContext();
        _controller = new TasksController(_context);
    }

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        context.Tasks.AddRange(
            new TaskItem { Id = 1, Title = "Test 1", Description = "Desc 1", CompletionStatus = false },
            new TaskItem { Id = 2, Title = "Test 2", Description = "Desc 2", CompletionStatus = true }
        );
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task Index_ReturnsViewWithTasks()
    {
        var result = await _controller.Index() as ViewResult;
        var model = result?.Model as List<TaskItem>;

        // Without Casting using 'as':
        // var result = await _controller.Index();
        // var view = Assert.IsType<ViewResult>(result);
        // var model = view?.Model;
        // var tasks = Assert.IsType<List<TaskItem>>(model);

        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.IsType<List<TaskItem>>(model);
        Assert.Equal(2, model.Count);
    }

    [Theory]
    [InlineData("New Title Test 1", "New Description Test 1")]
    [InlineData("New Title Test 2", "New Description Test 2", false)]
    [InlineData("New Title Test 3", "New Description Test 3", true)]
    [InlineData("New Title Test 4", null, true)]
    public async Task Crete_Post_ValidTask_RedirectsToIndex(string title, string? description, bool status = false)
    {
        TaskItem task = new TaskItem
        {
            Title = title,
            Description = description,
            CompletionStatus = status
        };

        var result = await _controller.Create(task) as RedirectToActionResult;
        var savedTask = await _context.Tasks.FindAsync(task.Id);

        Assert.NotNull(result);
        Assert.NotNull(savedTask);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal(title, savedTask.Title);
    }

    [Theory]
    [InlineData(1, "Title Edit Test 1", "Description Edit Test 1", true)]
    [InlineData(2, "Title Edit Test 2", "Description Edit Test 2", true)]
    public async Task Edit_GetPost_ValidTask_RedirectsToIndex(int id, string title, string? description, bool status)
    {
        var getResult = await _controller.Edit(id) as ViewResult;
        Assert.NotNull(getResult);

        var task = getResult.Model as TaskItem;
        Assert.NotNull(task);

        task.Title = title;
        task.Description = description;
        task.CompletionStatus = status;

        var postResult = await _controller.Edit(id, task) as RedirectToActionResult;
        var savedTask = await _context.Tasks.FindAsync(task.Id);

        Assert.NotNull(postResult);
        Assert.NotNull(savedTask);
        Assert.Equal("Index", postResult.ActionName);
        Assert.Equal(title, savedTask.Title);
    }

    [Fact]
    public async Task DeleteConfirmed_RedirectsToIndex()
    {
        int taskId = 1;

        var result = await _controller.DeleteConfirmed(taskId) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Null(await _context.Tasks.FindAsync(taskId));
    }

    [Fact]
    public async Task Details_ReturnsPartialView()
    {
        int taskId = 1;

        var result = await _controller.Details(taskId) as PartialViewResult;

        Assert.NotNull(result);
        Assert.Equal("_TaskDetailsPartial", result.ViewName);
        Assert.IsType<TaskItem>(result.Model);
    }

    [Fact]
    public async Task Complete_RedirectsToIndex()
    {
        int taskId = 1;

        var result = await _controller.Complete(taskId) as RedirectToActionResult;
        var savedTask = await _context.Tasks.FindAsync(taskId);

        Assert.NotNull(result);
        Assert.NotNull(savedTask);
        Assert.Equal("Index", result.ActionName);
        Assert.True(savedTask.CompletionStatus);
    }

    // Teardown
    public void Dispose()
    {
        _context?.Dispose();
        _controller?.Dispose();
    }
}
