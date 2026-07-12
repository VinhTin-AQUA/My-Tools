using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PC0483SecretCode.Helpers
{
    public class VietNameseNormalizeHelper
    {
        private static readonly Dictionary<string, string[]> vowelMap = new()
        {
            { "a", ["á", "à", "ả", "ã", "ạ"] },
            { "ă", ["ắ", "ằ", "ẳ", "ẵ", "ặ"] },
            { "â", ["ấ", "ầ", "ẩ", "ẫ", "ậ"] },
            { "e", ["é", "è", "ẻ", "ẽ", "ẹ"] },
            { "ê", ["ế", "ề", "ể", "ễ", "ệ"] },
            { "i", ["í", "ì", "ỉ", "ĩ", "ị"] },
            { "o", ["ó", "ò", "ỏ", "õ", "ọ"] },
            { "ô", ["ố", "ồ", "ổ", "ỗ", "ộ"] },
            { "ơ", ["ớ", "ờ", "ở", "ỡ", "ợ"] },
            { "u", ["ú", "ù", "ủ", "ũ", "ụ"] },
            { "ư", ["ứ", "ừ", "ử", "ữ", "ự"] },
            { "y", ["ý", "ỳ", "ỷ", "ỹ", "ỵ"] },
            { "A", ["À", "Á", "Ả", "Ã", "Ạ"] },
            { "Ă", ["Ằ", "Ắ", "Ẳ", "Ẵ", "Ặ"] },
            { "Â", ["Ầ", "Ấ", "Ẩ", "Ẫ", "Ậ"] },
            { "E", ["È", "É", "Ẻ", "Ẽ", "Ẹ"] },
            { "Ê", ["Ề", "Ế", "Ể", "Ễ", "Ệ"] },
            { "I", ["Ì", "Í", "Ỉ", "Ĩ", "Ị"] },
            { "O", ["Ò", "Ó", "Ỏ", "Õ", "Ọ"] },
            { "Ô", ["Ồ", "Ố", "Ổ", "Ỗ", "Ộ"] },
            { "Ơ", ["Ờ", "Ớ", "Ở", "Ỡ", "Ợ"] },
            { "U", ["Ù", "Ú", "Ủ", "Ũ", "Ụ"] },
            { "Ư", ["Ừ", "Ứ", "Ử", "Ữ", "Ự"] },
            { "Y", ["Ỳ", "Ý", "Ỷ", "Ỹ", "Ỵ"] }
        };

        // Bảng quy chiếu ký hiệu sang index
        private static readonly Dictionary<char, int> toneMap = new ()
        {
            { '/', 0 }, // sắc
            { '\\', 1 }, // huyền
            { '?', 2 }, // hỏi
            { '~', 3 }, // ngã
            { '.', 4 }  // nặng
        };

        private static (string baseChar, int accent) ExtractAccent(char c)
        {
            foreach (var kvp in vowelMap)
            {
                int idx = Array.IndexOf(kvp.Value, c.ToString());
                if (idx != -1)
                    return (kvp.Key, idx);
            }
            return (c.ToString(), -1);
        }

        // Áp dụng dấu lên nguyên âm
        private static string ApplyAccent(string baseChar, int accent)
        {
            if (accent == -1) return baseChar;
            if (vowelMap.ContainsKey(baseChar))
                return vowelMap[baseChar][accent];
            return baseChar;
        }

        // Kiểm tra phụ âm cuối
        private static bool HasFinalConsonant(string word)
        {
            if (string.IsNullOrEmpty(word)) return false;
            var lastChar = word[word.Length - 1];
            var (baseChar, _) = ExtractAccent(lastChar);
            return !vowelMap.ContainsKey(baseChar) && !vowelMap.ContainsKey(baseChar.ToLower());
        }

        private static string NormalizeWord(string word)
        {
            var correctChars = new List<string>();
            var vowels = new List<(string charBase, int index, int accent)>();
            var specialConsonants = new[] { "gi", "qu" };
            int accentIndex = -1;

            for (int i = 0; i < word.Length; i++)
            {
                var c = word[i];
                var (baseChar, mark) = ExtractAccent(c);
                bool isSpecialConsonant = false;

                if (i == 0 && word.Length > 2 && i + 1 < word.Length)
                {
                    var next = word[i + 1];
                    var (nextBase, nextMark) = ExtractAccent(next);
                    var pair = baseChar + nextBase;
                    foreach (var special in specialConsonants)
                    {
                        if (pair.ToLower() == special)
                        {
                            isSpecialConsonant = true;
                            correctChars.Add(pair);
                            i++;
                            if (nextMark != -1) accentIndex = nextMark;
                            break;
                        }
                    }
                }

                if (!isSpecialConsonant)
                {
                    if (vowelMap.ContainsKey(baseChar) || vowelMap.ContainsKey(baseChar.ToLower()))
                    {
                        vowels.Add((baseChar.ToLower(), i, mark));
                        correctChars.Add(ApplyAccent(baseChar, accentIndex != -1 ? accentIndex : mark));
                        accentIndex = -1;
                    }
                    else
                    {
                        correctChars.Add(baseChar);
                    }
                }
            }

            var correctedWord = string.Join("", correctChars);
            if (vowels.Count == 0) return correctedWord;

            // Tìm dấu cần giữ lại
            int accentToPreserve = -1, currentAccentIndex = -1;
            for (int i = 0; i < vowels.Count; i++)
            {
                if (vowels[i].accent != -1)
                {
                    accentToPreserve = vowels[i].accent;
                    currentAccentIndex = i;
                    break;
                }
            }
            if (accentToPreserve == -1) return correctedWord;

            // Xác định vị trí nguyên âm nhận dấu
            int targetIndex = 0;
            var priorityVowels = new[] { "ê", "ơ" };
            for (int i = 0; i < vowels.Count; i++)
            {
                if (Array.IndexOf(priorityVowels, vowels[i].charBase) != -1)
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex == 0 && Array.IndexOf(priorityVowels, vowels[0].charBase) == -1)
            {
                if (vowels.Count == 1)
                    targetIndex = 0;
                else if (vowels.Count == 2)
                    targetIndex = HasFinalConsonant(correctedWord) ? 1 : 0;
                else if (vowels.Count >= 3)
                    targetIndex = 1;
            }

            if (currentAccentIndex == targetIndex) return correctedWord;

            // Đặt dấu lên nguyên âm đúng
            var result = correctedWord.ToCharArray();
            var targetVowel = vowels[targetIndex];
            var (_baseChar, _) = ExtractAccent(result[targetVowel.index]);
            result[targetVowel.index] = ApplyAccent(_baseChar, accentToPreserve)[0];

            // Loại bỏ dấu ở các nguyên âm khác
            for (int i = 0; i < vowels.Count; i++)
            {
                if (i != targetIndex)
                {
                    var vowel = vowels[i];
                    var (baseChar2, _) = ExtractAccent(result[vowel.index]);
                    result[vowel.index] = baseChar2[0];
                }
            }

            return new string(result);
        }

        public static string AddTone(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            char lastChar = input[input.Length - 1];

            if (!toneMap.ContainsKey(lastChar))
                return input; // Không có ký hiệu thì giữ nguyên

            int toneIndex = toneMap[lastChar];
            string word = input.Substring(0, input.Length - 1);

            // Tìm nguyên âm để đặt dấu
            for (int i = word.Length - 1; i >= 0; i--)
            {
                char c = word[i];
                if (vowelMap.ContainsKey($"{c}"))
                {
                    string vowelWithTone = vowelMap[$"{c}"][toneIndex];
                    return word.Substring(0, i) + vowelWithTone + word.Substring(i + 1);
                }
            }

            return word; // Nếu không tìm thấy nguyên âm
        }
        
        // Hàm chính chuẩn hóa dấu cho cả chuỗi
        public static string NormalizeVietnameseAccent(string text)
        {
            if (text == null!) return "";
            var parts = Regex.Split(text.Normalize(NormalizationForm.FormC), @"(\s+|\p{P}+)");
            for (int i = 0; i < parts.Length; i++)
            {
                if (!Regex.IsMatch(parts[i], @"^\s+$|^\p{P}+$"))
                    parts[i] = NormalizeWord(parts[i]);
            }
            return string.Join("", parts);
        }
    }
}