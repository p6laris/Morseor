using Howler.Blazor.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Morseor;
using MorseSharp.Converter;
using MudBlazor.Services;
using ClipLazor.Extention;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddSingleton<MorseAudioConverter>();
builder.Services.AddSingleton<MorseTextConverter>();
builder.Services.AddScoped<IHowl, Howl>();
builder.Services.AddScoped<IHowlGlobal, HowlGlobal>();
builder.Services.AddClipboard();
await builder.Build().RunAsync();
