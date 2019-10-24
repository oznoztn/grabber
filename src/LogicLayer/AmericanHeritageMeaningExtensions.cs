using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GDomain;

namespace LogicLayer

{
    public static class Ordinals
    {
        public static int LetterToNumeral(string letter)
        {
            letter = letter.Replace(".", "");

            switch (letter)
            {
                case "a":
                    return 1;
                case "b":
                    return 2;
                case "c":
                    return 3;
                case "d":
                    return 4;
                case "e":
                    return 5;
                case "f":
                    return 6;
                case "g":
                    return 7;
                case "h":
                    return 8;
                default:
                    return 0;
            }
        }
    }

    public static class SenseRegisterPolice
    {
        private static readonly List<string> SenseRegisters = new List<string>
        {
            "obsolete",
            "dated",
            "archaid",
            "formal",
            "informal",
            "vulgar",
            "slang"
        };

        public static bool IsSenseRegister(string text)
        {
            text = text ?? "";

            bool isSenseRegister = SenseRegisters.Contains(text.ToLowerInvariant());

            if (isSenseRegister == false)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    // TODO: LOG THE text
                    // Maybe there are some sense registers that I don't know in advance.
                    // So, I have to detect them.

                    return false;
                }
            }

            return isSenseRegister;
        }

        public static bool IsSenseRegionRegister(string text)
        {
            string lower = text.ToLower(CultureInfo.InvariantCulture);

            if (lower.Contains("american"))
                return true;
            if (lower.Contains("british"))
                return true;
            if (lower.Contains("australian"))
                return true;
            if (lower.Contains("chiefly"))
                return true;
            if (lower.Contains("mainly"))
                return true;
            return false;
        }
    }

    public static class AmericanHeritageMeaningExtensions
    {
        private static bool IsNumeralOrdinalIdentifier(string text)
        {
            text = text.Replace(".", "");
            return IsNumber(text);
        }

        private static bool IsOrdinalIdentifierInLetterForm(string text)
        {
            switch (text)
            {
                case "a.":
                    return true;
                case "b.":
                    return true;
                case "c.":
                    return true;
                case "d.":
                    return true;
                case "e.":
                    return true;
                case "f.":
                    return true;
                case "g.":
                    return true;
                case "h.":
                    return true;
                case "i.":
                    return true;
                case "j.":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsNumber(string text)
        {
            int result;
            return int.TryParse(text, out result);
        }

        private static int DetecthNth(string text)
        {
            if (IsNumeralOrdinalIdentifier(text))
            {
                text = text.Replace(".", "");

                return int.Parse(text);
            }

            if (IsOrdinalIdentifierInLetterForm(text))
            {
                switch (text)
                {
                    case "a":
                        return 1;
                    case "b":
                        return 2;
                    case "c":
                        return 3;
                    case "d":
                        return 4;
                    case "e":
                        return 5;
                    case "f":
                        return 6;
                    case "g":
                        return 7;
                    case "h":
                        return 8;
                }
            }

            return 1;
        }
    }
}