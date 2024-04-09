using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTech.Domain.Validation;
public sealed class Utils
{
    public static bool HasInvalidCharacters(string stringValue)
    {
        char[] stringValueToArray = stringValue.ToCharArray();
        char[] invalidCharacters = new char[] {
            '\0', '\a', '\b', '\f', '\n', '\r', '\t', '\v' // Caracteres de escape
            , '#', '$', '%', '\'',  // Caracteres especiais
             '@',  '\\', '^', '_', '`', '{', '|', '}' // Outros caracteres especiais
            , '§', '£', '¢', '¬', '¦', // Caracteres especiais diversos
            };
        foreach (char character in stringValueToArray)
        {
            foreach (char invalidCharacter in invalidCharacters)
            {
                if (character.Equals(invalidCharacter))
                {
                    return true;
                }
            }
        }
        return false;
    }
}