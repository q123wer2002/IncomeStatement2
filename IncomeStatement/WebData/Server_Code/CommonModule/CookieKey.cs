using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IncomeStatement.WebData.Server_Code.CommonModule
{
	public class CookieKey
	{
		public static string Username
		{
			get
			{
				return "UserName";
			}
		}
		public static string UserID
		{
			get
			{
				return "UserID";
			}
		}
		public static string SyntecJWT
		{
			get
			{
				return "SyntecJWT";
			}
		}
		public static string AWSSFReToken
		{
			get
			{
				return "AWS_SF_ReToken";
			}
		}
	}
}
