using Howler.Blazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Morseor.Pages;
using MorseSharp.Audio.Languages;
using MorseSharp.Converter;
using MudBlazor;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using Morseor.Model;
using ClipLazor.Components;
using System.Text;

namespace Morseor.Pages
{
    public partial class Converter
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

        [Inject]
        IDialogService? Dialog { get; set; }

        [Inject]
        ClipLazor.Components.ClipLazor? clipbaord { get; set; }

        List<int> Sound = default!;
        public string? Message { get; set; } = string.Empty;
        public string? Morse { get; set; } = string.Empty;

        public bool CanLight { get; set; } = false;
        public bool CanPlay { get; set; } = false;
        public bool CanStop { get; set; } = false;
        public bool CanPause { get; set; } = false;
        public bool CanShare { get; set; } = false;
        public bool CanDownload { get; set; } = false;


        Memory<byte> morseAudio = Memory<byte>.Empty;

        public List<int> LightSignals = default!;

        public Language Lang { get; set; }

        public SettingsModel Settings;

       
        protected override void OnInitialized()
        {
            Sound = new List<int>();
            LightSignals = new List<int>();

            Settings = new SettingsModel()
            {
                CharSpeed = 25,
                WordSpeed = 25,
                Frequency = 600
            };

            SoundManager!.OnPlay += async e =>
            {
                CanPause = true;
                CanPlay = false;
                CanStop = true;
                CanLight = true;
                await InvokeAsync(StateHasChanged);
            };
            SoundManager!.OnPause += async e =>
            {
                CanPlay = false;
                CanStop = true;
                await InvokeAsync(StateHasChanged);
            };
            SoundManager!.OnStop += async e =>
            {
                CanPlay = true;
                CanPause = false;
                CanStop = false;

                await InvokeAsync(StateHasChanged);
            };
            SoundManager!.OnEnd += async e =>
            {
                CanPlay = true;
                CanPause = false;
                CanStop = false;
                await InvokeAsync(StateHasChanged);
            };


        }
        public async Task Convert()
        {
            Morse = string.Empty;


            if (Message!.Length > 0)
            {
                
                try
                {
                    if(Lang == Language.English)
                    {
                        morseAudioConverter = new MorseAudioConverter(Language.English,
                            Settings.CharSpeed,
                            Settings.WordSpeed,
                            Settings.Frequency);
                        Morse = await morseTextConverter.ConvertToMorseEnglish(Message!);
                        morseAudio = await morseAudioConverter.ConvertMorseToAudio(Message!);
                    }
                    else
                    {
                        morseAudioConverter = new MorseAudioConverter(Language.Kurdish,
                             Settings.CharSpeed,
                             Settings.WordSpeed,
                             Settings.Frequency);
                        Morse = await morseTextConverter.ConvertToMorseKurdish(Message!);
                        morseAudio = await morseAudioConverter.ConvertMorseToAudio(Message!);
                    }
                    CanPlay = true;
                    CanShare = true;
                    CanDownload = true;
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

        public async void Setting()
        {
            var parameters = new DialogParameters();
            parameters.Add("Settings", Settings);
            


            var res = await Dialog!.Show<SettingDialog>("Settings",parameters).Result;

            if (!res.Cancelled)
            {
                Settings = (SettingsModel)res.Data;
                await Convert();
                await InvokeAsync(StateHasChanged);
            }
        }
        public async void CopyToClipboard()
        {
            StringBuilder strBuilder = new();

            if(Morse.Length > 0)
            {
                var response = await clipbaord!.CopyAsync(Morse.AsMemory());
                strBuilder.Append($"\"{response}\"");
                strBuilder.Append(" Copied.");
                snackbar.Add(strBuilder.ToString(), Severity.Info); ;
            }
            
        }
    }
}

