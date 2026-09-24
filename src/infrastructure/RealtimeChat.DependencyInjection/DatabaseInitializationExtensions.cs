using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealtimeChat.Persistence.ContextClasses;

namespace RealtimeChat.DependencyInjection;

public static class DatabaseInitializationExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
            return;

        // Otomatik migration yerel Docker akışı içindir; üretimde kontrollü deployment adımı kullanılmalı.
        if (!app.Environment.IsDevelopment())
            throw new InvalidOperationException(
                "Automatic database migrations can only run in Development.");

        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        RealtimeChatDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<RealtimeChatDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
