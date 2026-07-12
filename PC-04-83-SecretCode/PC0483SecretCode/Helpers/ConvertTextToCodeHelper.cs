using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PC0483SecretCode.Helpers
{
    public static class ConvertTextToCodeHelper
    {
        public static string SeparateVietnameseWord(string word)
        {
            var temp = new StringBuilder();
            var tone = "";

            foreach (var c in word)
            {
                var normalized = c.ToString().Normalize(NormalizationForm.FormD);
                var baseChar = '\0';
                var combining = new StringBuilder();

                foreach (var ch in normalized)
                {
                    var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                    if (uc == UnicodeCategory.NonSpacingMark)
                        combining.Append(ch);
                    else
                        baseChar = ch;
                }

                var combinedBase = baseChar.ToString();

                foreach (var mark in combining.ToString())
                    switch (mark)
                    {
                        // Dấu thanh
                        case '\u0301': tone = "/"; break; // sắc
                        case '\u0300': tone = "\\"; break; // huyền
                        case '\u0309': tone = "?"; break; // hỏi
                        case '\u0303': tone = "~"; break; // ngã
                        case '\u0323': tone = "."; break; // nặng

                        // Dấu nguyên âm
                        case '\u0302': combinedBase = ComposeWithCircumflex(baseChar); break; // mũ
                        case '\u0306': combinedBase = ComposeWithBreve(baseChar); break; // ă
                        case '\u031B': combinedBase = ComposeWithHorn(baseChar); break; // ơ, ư
                    }

                temp.Append(combinedBase);
            }

            var (consonant, vowel) = SeperateVowelAndConsonant(temp.ToString());

            var consonants = TextHelper.ConsonantCodes
                           .Where(pair => pair.Value.Any(x => x.Equals(consonant, StringComparison.OrdinalIgnoreCase)))
                           .Select(pair => pair.Key)
                           .ToList();

            var vowels = TextHelper.VowelCodes
                            .Where(pair => pair.Value.Any(x => x.Equals(vowel, StringComparison.OrdinalIgnoreCase)))
                            .Select(pair => pair.Key)
                            .ToList();

            var tones = TextHelper.ToneMarksAndPunctuations
                            .Where(pair => pair.Value.Equals(tone, StringComparison.OrdinalIgnoreCase))
                            .Select(pair => pair.Key)
                            .ToList();

            List<string> resuls = [];
            foreach (var _consonant in consonants)
            {
                foreach (var _vowel in vowels)
                {
                    foreach (var _tone in tones)
                    {
                        resuls.Add($"{_consonant}{_vowel}{_tone}");
                    }
                }
            }
            
            if (resuls.Count > 1)
            {
                var t = string.Join(" ", resuls);
                return $"({t})";
            }
            return resuls[0];
        }

        private static string ComposeWithCircumflex(char c)
        {
            return c switch
            {
                'o' => "ô",
                'O' => "Ô",
                'a' => "â",
                'A' => "Â",
                'e' => "ê",
                'E' => "Ê",
                _ => c.ToString()
            };
        }

        private static string ComposeWithBreve(char c)
        {
            return c switch
            {
                'a' => "ă",
                'A' => "Ă",
                _ => c.ToString()
            };
        }

        private static string ComposeWithHorn(char c)
        {
            return c switch
            {
                'o' => "ơ",
                'O' => "Ơ",
                'u' => "ư",
                'U' => "Ư",
                _ => c.ToString()
            };
        }
        
        static (string, string) SeperateVowelAndConsonant(string tu)
        {
            string vowels = "aeiouyăâêôơưáàạảãấầậẩẫắằặẳẵéèẹẻẽếềệểễíìịỉĩóòọỏõốồộổỗớờợởỡúùụủũứừựửữýỳỵỷỹ"
                            + "AEIOUYĂÂÊÔƠƯÁÀẠẢÃẤẦẬẨẪẮẰẶẲẴÉÈẸẺẼẾỀỆỂỄÍÌỊỈĨÓÒỌỎÕỐỒỘỔỖỚỜỢỞỠÚÙỤỦŨỨỪỰỬỮÝỲỴỶỸ";

            int index = -1;

            // tìm vị trí đầu tiên của nguyên âm trong từ
            for (int i = 0; i < tu.Length; i++)
            {
                if (vowels.Contains(tu[i]))
                {
                    index = i;
                    break;
                }
            }
            
            if (index == -1) // không có nguyên âm
            {
                return (tu, "");
            }
            else
            {
                return (tu.Substring(0, index), tu.Substring(index));
            }
        }
    }
}