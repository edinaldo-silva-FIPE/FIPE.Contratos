using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Utilitario.Extensoes
{
    public static class Extensoes
    {
        #region | Métodos

        public static bool ColecaoVazia<T>(this IEnumerable<T> colecao_)
        {
            return colecao_ == null || colecao_.Count() == 0;
        }

        public static bool ColecaoVazia<T>(this T[] colecao_)
        {
            return colecao_ == null || colecao_.Count() == 0;
        }

        public static string ObterValorTexto(this string valor_)
        {
            return string.IsNullOrEmpty(valor_) ? string.Empty : valor_;
        }

        public static string ObterValorTexto(this object valor_)
        {
            return valor_ == null || valor_ == DBNull.Value || string.IsNullOrEmpty(valor_.ToString()) ? string.Empty : valor_.ToString();
        }

        public static int ObterValorInteiro(this object valor_)
        {
            return (valor_ == null || valor_ == DBNull.Value) ? 0 : Convert.ToInt32(valor_);
        }

        public static int ObterValorInteiro(this int? valor_)
        {
            return valor_ == null ? 0 : valor_.Value;
        }

        public static bool DataPadrao(this DateTime? data_)
        {
            return data_.HasValue ? DataPadrao(data_.Value) : true;
        }

        public static bool DataPadrao(this DateTime data_)
        {
            return data_.ToShortDateString().Equals(DateTime.MinValue.ToShortDateString());
        }

        #endregion
    }
}
