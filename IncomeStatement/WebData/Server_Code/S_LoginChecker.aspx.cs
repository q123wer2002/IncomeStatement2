using IncomeStatement.WebData.Server_Code.CommonModule;
using IncomeStatement.WebData.Server_Code.CommonModule.mssql;
using Newtonsoft.Json.Linq;
using System;

namespace IncomeStatement.WebData.Server_Code
{
	public partial class S_LoginChecker : System.Web.UI.Page
	{
		RequestHandler m_requestHandler;
		const int nDefaultUserId = 0;
		async protected void Page_Load( object sender, EventArgs e )
		{
			m_requestHandler = new RequestHandler();

			//set default response
			m_requestHandler.StatusCode = (int)ErrorCode.Error;
			m_requestHandler.ReturnData = string.Empty;

			//get user typing
			string szUserName = Request.Form[ Param.Username ].ToString();
			string szUserPassword = Request.Form[ Param.Password ].ToString();
			DateTime ExpireTime = DateTime.Now.AddDays( 1d );

			//TODO: check username and password
			JObject jUserInfo;
			if( isLoginSuccess(szUserName, szUserPassword, out jUserInfo) == false ) {
				m_requestHandler.StatusCode = (int)ErrorCode.Error;
				Response.Write(m_requestHandler.GetReturnResult());
				return;
			}

			//create token 
			string szJWTToken = "";
			szJWTToken = await JWTChecker.CreateNewJWTObjectString( szUserName );
			Response.Cookies[ CookieKey.JWTName ].Value = szJWTToken;
			Response.Cookies[ CookieKey.JWTName ].Expires = ExpireTime;
			Response.Cookies[ CookieKey.UserID ].Value = nDefaultUserId.ToString(); //default
			Response.Cookies[ CookieKey.UserID ].Expires = ExpireTime;

			//success
			m_requestHandler.StatusCode = (int)ErrorCode.Success;
			Response.Write( m_requestHandler.GetReturnResult() );
		}

		bool isParamValid()
		{
			try {
				string szUsername = Request.Form[ Param.Username ].ToString();
				string szPassword = Request.Form[ Param.Password ].ToString();
				return true;
			}
			catch {
				return false;
			}
		}
		bool isLoginSuccess( string szUserName, string szUserPassword, out JObject jUserInfo )
		{
			jUserInfo = new JObject();

			// get account info
			string szAccountInfo = $"SELECT * FROM {TableName.CoSysAuth} WHERE login_id='{szUserName}'";
			JArray jResult;
			m_mssql.TryQuery(szAccountInfo, out jResult);
			if( jResult == null || jResult.Count != 1 ) {
				return false;
			}

			// check password
			jUserInfo["account"] = (JObject)jResult[ 0 ];
			string szDBPassword = jUserInfo[ "account" ][ "pwd" ].ToString();
			if( szDBPassword != szUserPassword ) {
				return false;
			}

			// get user info
			string szUserInfo = $"SELECT * FROM {TableName.CoSysUser} WHERE user_id='{szUserName}'";
			m_mssql.TryQuery(szAccountInfo, out jResult);
			jUserInfo[ "user" ] = (jResult == null) ? null : (JObject)jResult[ 0 ];

			return true;
		}
		class Param
		{
			public static string Username
			{
				get
				{
					return "UserName";
				}
			}
			public static string Password
			{
				get
				{
					return "Password";
				}
			}
		}

		protected void Page_Error( object sender, EventArgs e )
		{
			// get error
			Exception ex = Server.GetLastError();

			// return
			m_requestHandler.StatusCode = (int)ErrorCode.Error;
			m_requestHandler.ReturnData = ( ConnectionInfo.isDebugMode ) ? ex.ToString() : string.Empty;
			Response.Write( m_requestHandler.GetReturnResult() );
			Server.ClearError();
		}

		MSSQL m_mssql = new MSSQL();
	}
}
