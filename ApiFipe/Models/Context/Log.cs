using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Log
    {
        public Log()
        {
        }
        public int      IdLog              { get; set; }
        public DateTime DtLog              { get; set; }
        public int      IdUsuario          { get; set; }
        public string   DsMensagem         { get; set; }
    }
}
