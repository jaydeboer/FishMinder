using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FishMinder.Client;
using FishMinder.Client.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Register data services
builder.Services.AddScoped<IFishingTripService, FishingTripService>();
builder.Services.AddScoped<IFishSpeciesService, FishSpeciesService>();
builder.Services.AddScoped<IAnglerSessionService, AnglerSessionService>();
builder.Services.AddScoped<ISpeciesCountService, SpeciesCountService>();

// TODO: Add GPS service and other services in later phases

await builder.Build().RunAsync();
