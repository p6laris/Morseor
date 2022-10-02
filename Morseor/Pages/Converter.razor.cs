using Howler.Blazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Morseor.Pages;
using Morseor.Tools.Animation;
using MorseSharp.MorseConverter;
using MudBlazor;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;

namespace MorseLazor.Base
{
    public class ConverterBase : ComponentBase
    {
        [Inject]
        public MorseTextConverter morseTextConverter { get; set; } = default!;

        [Inject]
        ISnackbar snackbar { get; set; }

        [Inject]
        MorseAudioConverter morseAudioConverter { get; set; } = default!;


        [Inject]
        public IHowl? SoundManager { get; set; }
        [Inject]
        IHowlGlobal? HowlGlobal { get; set; }

        [Inject]
        IJSRuntime? JSRuntime { get; set; }

        List<int> Sound = default!;
        public string? Message { get; set; } = string.Empty;
        public string? Morse { get; set; } = string.Empty;

        public bool CanLight { get; set; } = false;
        public bool CanPlay { get; set; } = false;
        public bool CanStop { get; set; } = false;
        public bool CanPause { get; set; } = false;

        Memory<byte> morseAudio = Memory<byte>.Empty;

        public List<int> LightSignals = default!;

        public MudPaper? LightPaper;
        protected override void OnInitialized()
        {
            Sound = new List<int>();
            LightSignals = new List<int>();
            SoundManager!.OnPlay += e =>
            {
                CanPause = true;
                CanPlay = false;
                CanStop = true;
                CanLight = true;
                StateHasChanged();
            };
            SoundManager!.OnPause += e =>
            {
                CanPlay = false;
                CanStop = true;
                StateHasChanged();
            };
            SoundManager!.OnStop += e =>
            {
                CanPlay = true;
                CanPause = false;
                CanStop = false;

                StateHasChanged();
            };
            SoundManager!.OnEnd += e =>
            {
                CanPlay = true;
                CanPause = false;
                CanStop = false;
                StateHasChanged();
            };


        }
        public async Task Convert()
        {
            Morse = string.Empty;


            if (Message!.Length > 0)
            {
                try
                {
                    Morse = await morseTextConverter.ConvertToMorseEnglish(Message!);
                    morseAudio = await morseAudioConverter.ConvertMorseToAudio(Message!);
                    CanPlay = true;
                }
                catch (Exception ex)
                {
                    snackbar.Add(ex.Message, Severity.Error);
                }

            }
            else
                CanPlay = false;

        }
        public async Task PlayMorse()
        {
            Sound.Add(await SoundManager!.Play(morseAudio.ToArray(), "audio/wav"));
            CanPause = true;
        }
        public async Task PauseMorse()
        {
            foreach (var id in Sound)
            {
                await SoundManager!.Pause(id);
            }
        }

        public async Task StopMorse()
        {
            foreach (var id in Sound)
            {
                await SoundManager!.Stop(id);
            }
        }

        public async Task DownloadMorse()
        {
            await JSRuntime!.InvokeVoidAsync("downloadFile", "audio/wav", System.Convert.ToBase64String(morseAudio.ToArray()), $"{Message}");
        }

       
        public async Task AnimateMorse()
        {
            await Task.Delay(500);

            foreach (var morse in Morse!)
            {
                if (morse == '.')
                    LightPaper.Style = "background-color: #ffffff;";
                else if (morse == '_')
                    LightPaper.Style = "background-color: #ffffff;";
                else
                    LightPaper.Style = "background-color: #ffffff;";

                StateHasChanged();
                await Task.Delay(500);
                LightPaper.Style = "background-color: black;";
                StateHasChanged();
            }
        }
    }
}
