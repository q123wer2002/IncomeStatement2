using Newtonsoft.Json.Linq;
using IncomeStatement.WebData.Server_Code.CommonModule;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IncomeStatement.WebData.Server_Code
{
	public partial class S_LoginChecker : System.Web.UI.Page
	{
		RequestHandler m_requestHandler;
		const int nDefaultUserId = 0;
		async protected void Page_Load( object sender, EventArgs e )
		{
			m_requestHandler = new RequestHandler( ConnectionInfo.isAWS, ConfigReader.GetConnection() );

			//set default response
			m_requestHandler.StatusCode = (int)ErrorCode.Error;
			m_requestHandler.ReturnData = string.Empty;

			//get user typing
			string szUserName = Request.Form[ Param.Username ].ToString();
			string szUserPassword = Request.Form[ Param.Password ].ToString();
			SyntecJWTModel jwtTokenObject = new SyntecJWTModel();
			DateTime ExpireTime = DateTime.Now.AddDays( 1d );
			bool isUserInfoValid = false;

			//create token 
			string szJWTToken = "";
			szJWTToken = await JWTChecker.CreateNewJWTObjectString( szUserName );
			Response.Cookies[ CookieKey.SyntecJWT ].Value = szJWTToken;
			Response.Cookies[ CookieKey.SyntecJWT ].Expires = ExpireTime;
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
		string EncryptedString( string szText )
		{
			SHA256 hashFunction = new SHA256CryptoServiceProvider();
			byte[] bHashFunc = hashFunction.ComputeHash( Encoding.Default.GetBytes( szText ) );
			string szEncrytedPassword = Convert.ToBase64String( bHashFunc );
			return szEncrytedPassword;
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
	}
}
