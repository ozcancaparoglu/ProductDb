using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Data.OnnetDb
{
    public class OnnetStatics
    {
        public static string sqlText = @"SELECT PP.Id,P.Name,PR.UnrealStock,PR.LogoCode FROM ProjectProducts PP 
		                                  INNER JOIN Projects P ON PP.ProjectId = P.Id
	                                      INNER JOIN Products PR ON PR.Id = PP.ProductId
		                                  WHERE  P.Name = '{0}' AND PR.LogoCode IN ({1}) AND PP.IsActive = 1";
    }
}
