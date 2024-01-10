using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbIninitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings
            .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Year, KeyType.Text)
            .CreateAsync();



        var count = await DB.CountAsync<Item>();

       using var scope = app.Services.CreateScope();

       var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

       var items = await httpClient.GetItemsForSearchDb();

       Console.WriteLine($"Found {items.Count} items from AuctionService");

       if (items.Count > 0) await DB.SaveAsync(items);
       
    }
}