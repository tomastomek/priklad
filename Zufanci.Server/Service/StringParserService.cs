using System.Text.RegularExpressions;

namespace Zufanci.Server.Service
{
    /// <summary>
    /// Provides functionality to parse and modify strings based on a specified pattern.
    /// </summary>
    public class StringParserService : IStringParserService
    {
        /// <summary>
        /// Parses the provided string based on the specified pattern.
        /// </summary>
        /// <param name="textToParse">The string to parse.</param>
        /// <param name="pattern">The pattern used for parsing.</param>
        /// <returns>The parsed string.</returns>
        /// <exception cref="ArgumentException">Thrown when the pattern is invalid or when removing all characters from the text.</exception>
        public string ParseString(string textToParse, string pattern)
        {
            Match match = Regex.Match(pattern, @"(.*)\[(\d+)\]TEXT\[(\d+)\](.*)");
            if (match.Success)
            {
                // In match.Groups collection, index 0 represents the entire match
                int charsToRemoveFromStart = int.Parse(match.Groups[2].Value);
                int charsToRemoveFromEnd = int.Parse(match.Groups[3].Value);

                // Check if removing characters from the start would leave at least one character
                if (charsToRemoveFromStart >= textToParse.Length)
                {
                    throw new ArgumentException("Cannot remove all characters from the text.");
                }

                // Check if removing characters from the end would leave at least one character
                if (charsToRemoveFromEnd >= textToParse.Length)
                {
                    throw new ArgumentException("Cannot remove all characters from the text.");
                }

                if (charsToRemoveFromStart > 0)
                {
                    textToParse = textToParse.Substring(charsToRemoveFromStart);
                }
                if (charsToRemoveFromEnd > 0)
                {
                    textToParse = textToParse.Substring(0, textToParse.Length - charsToRemoveFromEnd);
                }

                string textBefore = match.Groups[1].Value;
                string textAfter = match.Groups[4].Value;
                return textBefore + textToParse + textAfter;
            }
            else
            {
                throw new ArgumentException("Invalid pattern format.");
            }
        }
    }
}
