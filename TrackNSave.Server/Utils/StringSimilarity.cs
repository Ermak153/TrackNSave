using System.Text.RegularExpressions;

namespace TrackNSave.Server.Utils
{
    public static class StringSimilarity
    {
        private static readonly string[] CommonWords = new[]
        {
            "напиток", "газированный", "питьевой", "бутилированный",
            "пастеризованное", "ультрапастеризованное", "стерилизованное",
            "светлое", "темное", "нефильтрованное", "фильтрованное",
            "пэт", "стекло", "жестяная", "банка", "бутылка",
            "кусок", "порция", "шт", "штука", "упаковка", "пачка",
            "целая", "половина", "четверть"
        };

        private static readonly string[] VolumeUnits = new[] { "л", "мл", "г", "кг" };

        public static double CalculateSimilarity(string searchQuery, string productName)
        {
            if (string.IsNullOrEmpty(searchQuery) || string.IsNullOrEmpty(productName))
                return 0;

            searchQuery = searchQuery.ToLower().Trim();
            productName = productName.ToLower().Trim();

            var (searchClean, searchVolume, searchCharacteristics) = ExtractProductInfo(searchQuery);

            var (productClean, productVolume, productCharacteristics) = ExtractProductInfo(productName);

            double wordSimilarity = CompareWords(searchClean, productClean);

            if (wordSimilarity >= 0.7)
            {
                double volumeSimilarity = CompareVolumes(searchVolume, productVolume);
                double characteristicsSimilarity = CompareCharacteristics(searchCharacteristics, productCharacteristics);

                if (string.IsNullOrEmpty(searchVolume) || volumeSimilarity >= 0.7)
                {
                    if (searchCharacteristics.Length == 0 || characteristicsSimilarity >= 0.7)
                    {
                        double totalSimilarity = wordSimilarity * 0.6 +
                            (string.IsNullOrEmpty(searchVolume) ? 0.2 : volumeSimilarity * 0.2) +
                            (searchCharacteristics.Length == 0 ? 0.2 : characteristicsSimilarity * 0.2);

                        return totalSimilarity;
                    }
                }
            }

            if (productClean.Contains(searchClean) || searchClean.Contains(productClean))
            {
                double volumeSimilarity = CompareVolumes(searchVolume, productVolume);
                double characteristicsSimilarity = CompareCharacteristics(searchCharacteristics, productCharacteristics);

                if (string.IsNullOrEmpty(searchVolume) || volumeSimilarity >= 0.7)
                {
                    if (searchCharacteristics.Length == 0 || characteristicsSimilarity >= 0.7)
                    {
                        return 0.85;
                    }
                }
            }

            var searchWords = searchClean.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var productWords = productClean.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (searchWords.Length > 0 && productWords.Length > 0)
            {
                int matches = searchWords.Count(sw => productWords.Any(pw => pw.Contains(sw) || sw.Contains(pw)));
                if (matches > 0)
                {
                    double wordMatchRatio = (double)matches / searchWords.Length;
                    if (wordMatchRatio >= 0.5)
                    {
                        return 0.65 * wordMatchRatio;
                    }
                }
            }

            return 0.1;
        }

        private static double CompareWords(string s1, string s2)
        {
            var words1 = s1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var words2 = s2.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words1.Length == 0 || words2.Length == 0)
                return 0;

            int matches = 0;
            foreach (var word1 in words1)
            {
                if (word1.Length <= 2) continue;

                foreach (var word2 in words2)
                {
                    if (word2.Length <= 2) continue;

                    if (word1 == word2 || word1.Contains(word2) || word2.Contains(word1))
                    {
                        matches++;
                        break;
                    }
                    else
                    {
                        double similarity = 1.0 - (double)LevenshteinDistance(word1, word2) / Math.Max(word1.Length, word2.Length);
                        if (similarity >= 0.8)
                        {
                            matches++;
                            break;
                        }
                    }
                }
            }

            return (double)matches / words1.Length;
        }

        private static (string Clean, string Volume, string[] Characteristics) ExtractProductInfo(string s)
        {
            string volume = "";
            var volumeMatches = Regex.Matches(s, @"(\d+[,.]?\d*)\s*([мллкг]+)");
            if (volumeMatches.Count > 0)
            {
                foreach (Match match in volumeMatches)
                {
                    string number = match.Groups[1].Value.Replace(',', '.');
                    volume = $"{number}{match.Groups[2].Value}";
                }
            }

            var characteristics = new List<string>();
            var percentageMatches = Regex.Matches(s, @"(\d+[,.]?\d*)\s*%");
            foreach (Match match in percentageMatches)
            {
                string number = match.Groups[1].Value.Replace(',', '.');
                characteristics.Add($"{number}%");
            }

            string clean = s;

            clean = Regex.Replace(clean, @"(\d+[,.]?\d*)\s*([мллкг]+)", " ");
            clean = Regex.Replace(clean, @"(\d+[,.]?\d*)\s*%", " ");

            foreach (var word in CommonWords)
            {
                clean = Regex.Replace(clean, $"\\b{word}\\b", " ", RegexOptions.IgnoreCase);
            }

            clean = Regex.Replace(clean, @"\s+", " ").Trim();

            return (clean, volume, characteristics.ToArray());
        }

        private static double CompareVolumes(string v1, string v2)
        {
            if (string.IsNullOrEmpty(v1) || string.IsNullOrEmpty(v2))
                return 1.0;

            try
            {
                double value1 = NormalizeVolumeToNumber(v1);
                double value2 = NormalizeVolumeToNumber(v2);

                if (value1 <= 0 || value2 <= 0)
                    return 0.5;

                double difference = Math.Abs(value1 - value2);
                double maxVolume = Math.Max(value1, value2);

                double tolerance = maxVolume * 0.1;

                if (difference <= tolerance)
                    return 1.0 - (difference / (tolerance * 2));
                else
                    return 0.5 * (1.0 - Math.Min(1.0, difference / maxVolume));
            }
            catch
            {
                return 0.5;
            }
        }

        private static double NormalizeVolumeToNumber(string volume)
        {
            string numStr = Regex.Replace(volume, @"[^\d.,]", "").Replace(',', '.');

            if (!double.TryParse(numStr, out double num))
                return 0;

            if (volume.Contains("кг"))
                return num * 1000;
            else if (volume.Contains("г") && !volume.Contains("мг"))
                return num;
            else if (volume.Contains("л") && !volume.Contains("мл"))
                return num * 1000;
            else if (volume.Contains("мл"))
                return num;

            if (num > 1000)
                return num;
            else if (num < 10)
                return num * 1000;

            return num;
        }

        private static double CompareCharacteristics(string[] c1, string[] c2)
        {
            if (c1.Length == 0 || c2.Length == 0)
                return 1.0;

            int matches = 0;
            foreach (var char1 in c1)
            {
                if (string.IsNullOrEmpty(char1)) continue;

                foreach (var char2 in c2)
                {
                    if (string.IsNullOrEmpty(char2)) continue;

                    if (char1 == char2)
                    {
                        matches++;
                        break;
                    }
                    else if (char1.EndsWith("%") && char2.EndsWith("%"))
                    {
                        if (double.TryParse(char1.TrimEnd('%'), out double val1) &&
                            double.TryParse(char2.TrimEnd('%'), out double val2))
                        {
                            if (Math.Abs(val1 - val2) <= 0.5)
                            {
                                matches++;
                                break;
                            }
                        }
                    }
                }
            }

            return (double)matches / Math.Max(c1.Length, c2.Length);
        }

        private static int LevenshteinDistance(string s1, string s2)
        {
            int[,] matrix = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[s1.Length, s2.Length];
        }
    }
}