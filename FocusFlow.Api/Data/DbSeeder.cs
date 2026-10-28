using FocusFlow.Api.Data.Entities;
using FocusFlow.Shared.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FocusFlow.Api.Data;

// Creates the database on first run and fills it with a demo user and sample tasks
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Applies any migrations that haven't run yet (creates focusflow.db the first time)
        await db.Database.MigrateAsync();

        var username = config["Seed:DemoUsername"];
        var password = config["Seed:DemoPassword"];
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)
            && !await db.Users.AnyAsync(u => u.Username == username))
        {
            var user = new User { Username = username };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, password);
            db.Users.Add(user);
        }

        if (!await db.Tasks.AnyAsync())
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            db.Tasks.AddRange(
                Sample("Finish API documentation", "Describe every endpoint and the login flow", today.AddDays(2)),
                Sample("Review pull request #42", "Check the paging logic and tests", today.AddDays(1)),
                Sample("Renew passport", "Book an appointment and bring two photos", today.AddDays(-3)),
                Sample("Prepare demo slides", "Five slides: problem, design, demo, code, lessons", today.AddDays(5)),
                Sample("Book dentist appointment", null, today.AddDays(-1), TaskItemStatus.Completed),
                Sample("Plan team lunch", "Find a place that seats eight", today.AddDays(7)),
                Sample("Update CV", "Add the FocusFlow project", today.AddDays(-2), TaskItemStatus.Completed),
                Sample("Water the plants", null, today, TaskItemStatus.Completed),
                Sample("Read chapter 5 of Clean Code", "Functions and naming", today.AddDays(10)),
                Sample("Pay electricity bill", null, today.AddDays(3)),
                Sample("Back up laptop", "Photos and documents to the external drive", today.AddDays(-5)),
                Sample("Call the bank", "Ask about the new card", today.AddDays(4), TaskItemStatus.Completed));
        }

        await db.SaveChangesAsync();
    }

    private static TaskItem Sample(string title, string? description, DateOnly dueDate,
        TaskItemStatus status = TaskItemStatus.Pending) =>
        new() { Title = title, Description = description, DueDate = dueDate, Status = status };
}
