using IncomeStatement.WebData.Server_Code.CommonModule;
using IncomeStatement.WebData.Server_Code.CommonModule.mssql;
using Newtonsoft.Json.Linq;
using System;

namespace IncomeStatement.WebData.Server_Code
{
	public partial class S_LoginChecker : System.Web.UI.Page
	{
		RequestHandler m_requestHandler;
		string szUserId = "";
		string szUserName = "";
		string szRole = "";
		AccountErrorCode aErrorCode = AccountErrorCode.success;
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

			JObject jUserInfo;
			if( isLoginSuccess( szUserName, szUserPassword, out jUserInfo ) ) {
				// check account status
				string szState = jUserInfo[ "account" ][ "state" ].ToString();
				if( szState == "0" ) {
					aErrorCode = AccountErrorCode.accountDisabled;
				}
				else if(szState == "2") {
					aErrorCode = AccountErrorCode.accountLock;
				}

				// check account expired
				string szStartDate = jUserInfo[ "account" ][ "start_date" ].ToString();
				string szEndDate = jUserInfo[ "account" ][ "end_date" ].ToString();

				// change password
				if( jUserInfo[ "account" ][ "chg_pwd" ].ToString() == "Y" ) {
					aErrorCode = AccountErrorCode.changePassword;
				}
			}

			// write login log
			WriteLoginLog( aErrorCode );

			if( (int)aErrorCode < 0 ) {
				// fail
				m_requestHandler.StatusCode = (int)ErrorCode.Error;
				m_requestHandler.ReturnData = aErrorCode;
				Response.Write( m_requestHandler.GetReturnResult() );
				return;
			}

			//create token 
			string szJWTToken = JWTChecker.CreateNewJWTObjectString( szUserName );
			Response.Cookies[ CookieKey.JWTName ].Value = szJWTToken;
			Response.Cookies[ CookieKey.JWTName ].Expires = ExpireTime;
			Response.Cookies[ CookieKey.UserID ].Value = szUserId;
			Response.Cookies[ CookieKey.UserID ].Expires = ExpireTime;
			Response.Cookies[ CookieKey.Username ].Value = szUserName;
			Response.Cookies[ CookieKey.Username ].Expires = ExpireTime;

			//success
			m_requestHandler.StatusCode = (int)ErrorCode.Success;
			m_requestHandler.ReturnData = aErrorCode;
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
				aErrorCode = AccountErrorCode.noAccount;
				return false;
			}

			// check password
			jUserInfo["account"] = (JObject)jResult[ 0 ];
			string szDBPassword = jUserInfo[ "account" ][ "pwd" ].ToString();
			if( szDBPassword != szUserPassword ) {
				aErrorCode = AccountErrorCode.passwordError;
				return false;
			}

			// get user info
			string szUserInfo = $"SELECT * FROM {TableName.CoSysUser} WHERE user_id='{szUserName}'";
			m_mssql.TryQuery(szAccountInfo, out jResult);
			jUserInfo[ "user" ] = (jResult == null) ? null : (JObject)jResult[ 0 ];

			// assign local var
			szUserId = jUserInfo[ "user" ][ "user_id" ].ToString();
			szUserName = jUserInfo[ "user" ][ "user_name" ] == null ? szUserId : jUserInfo[ "user" ][ "user_name" ].ToString();
			szRole = jUserInfo[ "account" ][ "role" ].ToString();
			return true;
		}
		void WriteLoginLog( AccountErrorCode errorcode )
		{
			string szUserId = Request.Form[ Param.Username ].ToString();
			string szIP = Request.Form[ Param.IP ].ToString();
			string szErrorMsg;

			if( (int)errorcode > 0 ) {
				// write success login
				string szLogAuth = $"UPDATE {TableName.CoSysAuth} SET log_date=CURRENT_TIMESTAMP WHERE login_id='{szUserId}'";
				m_mssql.TryQuery( szLogAuth, out szErrorMsg );
			}

			// write log into co_sys_log
			string szLogSys = $"INSERT INTO {TableName.CoSysLog} VALUES (CURRENT_TIMESTAMP, '{szUserId}', '{szIP}', '{parseErrorLog( errorcode )}', N'' )";
			m_mssql.TryQuery( szLogSys, out szErrorMsg );
		}
		string parseErrorLog( AccountErrorCode errorcode )
		{
			switch( errorcode ) {
				case AccountErrorCode.success:
					return "00";
				case AccountErrorCode.noAccount:
					return "01";
				case AccountErrorCode.passwordError:
					return "02";
				case AccountErrorCode.passwordErrorManyTimes:
					return "03";
				case AccountErrorCode.accountDisabled:
					return "04";
				case AccountErrorCode.accountInActive:
					return "05";
				case AccountErrorCode.accountExpired:
					return "06";
				case AccountErrorCode.accountLock:
					return "07";
				case AccountErrorCode.passwordExpired:
					return "08";
				case AccountErrorCode.dbConnectionError:
					return "09";
				default:
					return "10";
			}
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
			public static string IP
			{
				get
				{
					return "IP";
				}
			}
		}
		enum AccountErrorCode
		{
			changePassword = 1,
			success = 0,
			noAccount = -1,
			passwordError = -2,
			passwordErrorManyTimes = -3,
			accountLock = -4,
			accountDisabled = -5,
			accountInActive = -6,
			accountExpired = -7,
			passwordExpired = -8,
			dbConnectionError = -9,
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
