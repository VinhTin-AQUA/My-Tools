using DynamicData;
using PC0483SecretCode.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using Tmds.DBus.Protocol;

namespace PC0483SecretCode.ViewModels
{
    public class ConverterViewModel : ViewModelBase
    {
        private bool _convertToCode;
        public bool ConvertToCode
        {
            get => _convertToCode;
            set => this.RaiseAndSetIfChanged(ref this._convertToCode, value);
        }

        private string _input1 = "";
        public string Input1
        {
            get => _input1;
            set => this.RaiseAndSetIfChanged(ref _input1, value);
        }

        private string _input2 = "";
        public string Input2
        {
            get => _input2;
            set => this.RaiseAndSetIfChanged(ref _input2, value);
        }
        
        public ReactiveCommand<Unit, Unit> ConvertCommand { get; set; }

        public ConverterViewModel()
        {
            ConvertCommand = ReactiveCommand.Create(() =>
            {
                string[] normalizedContent = NomalizeContent(Input1);

                if (ConvertToCode)
                {
                    Input2 = ConvertTextToCode(normalizedContent);
                }
                else
                {
                    Input2 = ConvertCodeToText(normalizedContent);
                }
            });
        }
        
        public static string ConvertCodeToText(string[] codes)
        {
            List<string> result = [];
            foreach (var code in codes)
            {
                var keys = TextHelper.SpecialChars.Where(pair => pair.Value.Any(x => x.Equals(code, StringComparison.OrdinalIgnoreCase)))
                               .Select(pair => pair.Key)
                               .ToList();

                if(keys != null && keys.Count > 0)
                {
                    result.AddRange(keys);
                    continue;
                }
                     
                var texts = ConvertCodeToTextHelper.ConvertToText(code);

                for (int i = 0; i < texts.Count; i++)
                {
                    var tone = VietNameseNormalizeHelper.AddTone(texts[i]);
                    texts[i]  = VietNameseNormalizeHelper.NormalizeVietnameseAccent(tone);
                }

                if (texts.Count > 1)
                {
                    result.Add($"({string.Join(" ", texts)})");
                }
                else
                {
                    result.Add(texts[0]);
                }
            }
            return string.Join(" ", result);
        }

        public static string ConvertTextToCode(string[] texts)
        {
            List<string> codes = [];
            foreach (var text in texts)
            {
                var specialChar = TextHelper.SpecialChars.TryGetValue(text, out var specialCode);

                if (specialChar && specialCode != null)
                {
                    codes.AddRange(specialCode);
                    continue;
                }

                try
                {
                    var code = ConvertTextToCodeHelper.SeparateVietnameseWord(text);
                    codes.Add(code);
                }
                catch 
                {
                    codes.Add($"ERROR_TEXT({text})");
                }
            }
            return string.Join(" ", codes);
        }
        
        public override void Dispose()
        {
            // các lệnh giải phóng tài nguyên
            base.Dispose();
        }

        #region private method

        private static string[] NomalizeContent(string content)
        {
            char[] separators = { ' ', ',', '.', ';', ':', '!', '?', '\n', '\r', '\t' };
            string[] words = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            string[] result = words.Select(x => x.ToLower()).ToArray();
            return result;
        }

        #endregion
    }
}