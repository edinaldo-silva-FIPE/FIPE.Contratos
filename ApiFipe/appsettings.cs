using System;

namespace ApiFipe
{ 
	public static class AppSettings
    {

        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Agosto/2020 
        *  ID do Usuario logado
        *  
        ===========================================================================================*/
        public static int constGlobalUserID = 0;                                                            //ID do Usuario conectado

        static AppSettings()
        {
            constGlobalUserID = 0;
        }
    }
}
