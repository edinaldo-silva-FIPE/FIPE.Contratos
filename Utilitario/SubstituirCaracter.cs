using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ApiFipe.Utilitario.Extensoes
{
    public static class SubstituirCaracter
    {


        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Set/2020 
        *  A função C# abaixo substitui caracteres com acento pelo seu equivalente sem acento, 
		*  por exemplo, troca “á” por “a” e “ç” por “c” sem perder o maiúsculo e minúsculo do texto.
        *  Exemplos de entrada e saída:   "teste!@#&#" -> "teste"
        ===========================================================================================*/
        public static string _RemoveCaracterSpecial(string pTexto)
        {
            if (string.IsNullOrEmpty(pTexto))
            {
                return "";
            } else { 
                pTexto = pTexto.Replace("  "    , " ");
                pTexto = pTexto.Replace("=\r\n" , "");
                pTexto = pTexto.Replace(";\r\n" , "");
                pTexto = pTexto.Replace("\r\n"  , "");
                pTexto = pTexto.Replace("\t"    , " ");
                pTexto = pTexto.Replace("\u0009", "");
                pTexto = RemoveSpecialCharacters(pTexto, true);
                return pTexto;
            }
        }


        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Função utiliza Regular Expression para substituir caracteres especiais de uma string 
        *  Exemplos de entrada e saída:   "teste!@#&#" -> "teste"
        ===========================================================================================*/
        public static string RemoveSpecialCharacters(string text, bool allowSpace = false)
        {
            string ret;

            if (allowSpace)
                ret = System.Text.RegularExpressions.Regex.Replace(text, @"[^0-9a-zA-ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\s]+?()#  ", string.Empty);
            else
                ret = System.Text.RegularExpressions.Regex.Replace(text, @"[^0-9a-zA-ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ]+?", string.Empty);

            return ret;
        }


        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  A função C# abaixo substitui caracteres com acento pelo seu equivalente sem acento, 
		*  por exemplo, troca “á” por “a” e “ç” por “c” sem perder o maiúsculo e minúsculo do texto.
        *  Exemplos de entrada e saída:   "teste!@#&#" -> "teste"
        ===========================================================================================*/
        public static string ObterStringSemAcentosECaracteresEspeciais(string str)
        {
            /** Troca os caracteres acentuados por não acentuados **/
            string[] acentos   = new string[] { "ç", "Ç", "á", "é", "í", "ó", "ú", "ý", "Á", "É", "Í", "Ó", "Ú", "Ý", "à", "è", "ì", "ò", "ù", "À", "È", "Ì", "Ò", "Ù", "ã", "õ", "ñ", "ä", "ë", "ï", "ö", "ü", "ÿ", "Ä", "Ë", "Ï", "Ö", "Ü", "Ã", "Õ", "Ñ", "â", "ê", "î", "ô", "û", "Â", "Ê", "Î", "Ô", "Û" };
            string[] semAcento = new string[] { "c", "C", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "Y", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U", "a", "o", "n", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "A", "O", "N", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U" };

            for (int i = 0; i < acentos.Length; i++)
            {
                str = str.Replace(acentos[i], semAcento[i]);
            }
            /** Troca os caracteres especiais da string por "" **/
            string[] caracteresEspeciais = { "¹", "²", "³", "£", "¢", "¬", "º", "¨", "\"", "'", ".", ",", "-", ":", "(", ")", "ª", "|", "\\\\", "°", "_", "@", "#", "!", "$", "%", "&", "*", ";", "/", "<", ">", "?", "[", "]", "{", "}", "=", "+", "§", "´", "`", "^", "~" };

            for (int i = 0; i < caracteresEspeciais.Length; i++)
            {
                str = str.Replace(caracteresEspeciais[i], "");
            }

            /** Troca os caracteres especiais da string por " " **/
            str = Regex.Replace(str, @"[^\w\.@-]", " ", RegexOptions.None, TimeSpan.FromSeconds(1.5));

            return str.Trim();
        }

    }
}
