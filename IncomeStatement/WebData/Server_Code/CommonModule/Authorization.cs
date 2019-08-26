using Newtonsoft.Json;
using System.Collections.Generic;

namespace IncomeStatement.WebData.Server_Code.CommonModule
{
	public class Authorization
	{
		#region Public Method
		public static bool isAccessAutho( string szAuth, string szFunctionName = ProductDB + "_Allow_View" )
		{
			// check params
			if( szAuth == string.Empty ) {
				return false;
			}

			// check authorization
			try {
				List<AuthObject> AuthoList = PraseAuthoString( szAuth );
				if( AuthoList == null ) {
					//no auth list
					return false;
				}

				if( AuthoList.Count == 0 ) {
					//means no auth
					return false;
				}

				//check admin
				if( AuthoList[ 0 ].app_name.Equals( ADMIN ) ) {
					return true;
				}

				//find correct auth
				AuthObject DashboardAutho = AuthoList.Find( obj => obj.app_name.Equals( WinformName ) );
				if( DashboardAutho == null ) {
					//means not found
					return false;
				}

				//parse utility auth
				List<string> szAuthList = DashboardAutho.flag_name.FindAll( obj => obj.StartsWith( ProductDB ) );
				if( szAuthList.IndexOf( szFunctionName.ToUpper() ) == -1 ) {
					return false;
				}

				return true;
			}
			catch {
				return false;
			}
		}
		public static string GetAllAppFlag()
		{
			return JsonConvert.SerializeObject( AllFunctionList );
		}
		#endregion

		#region Private Method
		static List<AuthObject> PraseAuthoString( string szAuthoList )
		{
			List<AuthObject> AuthList = null;

			try {
				AuthList = JsonConvert.DeserializeObject<List<AuthObject>>( szAuthoList );
			}
			catch {
				//do nothing
			}

			return AuthList;
		}
		#endregion

		#region Private Attribute
		const string ADMIN = "Administrator";
		const string WinformName = "str_WinformBrowser";
		const string ProductDB = "PRODUCEDB";
		static List<string> AllFunctionList = new List<string>()
		{
			ProductDB + "_ALLOW_VIEW"
		};
		#endregion
	}

	public class AuthObject
	{
		public int flag_count
		{
			get; set;
		}
		public string app_name
		{
			get; set;
		}
		public List<string> flag_name
		{
			get; set;
		}
	}
}
