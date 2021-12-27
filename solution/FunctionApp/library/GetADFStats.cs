/*-----------------------------------------------------------------------

 Copyright (c) Microsoft Corporation.
 Licensed under the MIT license.

-----------------------------------------------------------------------*/

using System;
using System.Collections.Generic;


namespace AdsGoFast
{


    public static class GetADFStats
    {

        public static Dictionary<string, Type> KustoDataTypeMapper
        {
            get
            {
                // Add the rest of your CLR Types to SQL Types mapping here
                Dictionary<string, Type> dataMapper = new Dictionary<string, Type>
                    {
                        { "int", typeof(int) },
                        { "string", typeof(string) },
                        { "real", typeof(double) },
                        { "long", typeof(long) },
                        { "datetime", typeof(System.DateTime) },
                        { "guid", typeof(Guid) }

                    };

                return dataMapper;
            }
        }



    }

}







