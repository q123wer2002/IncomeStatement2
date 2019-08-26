using Jose;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IncomeStatement.WebData.Server_Code.CommonModule
{
	public class JWTChecker
	{
		#region Public Method
		public static ErrorCode isJWTValid( string szJWTToken, out SyntecJWTModel jwtObject )
		{
			ErrorCode eCode = ErrorCode.AuthenticationError;

			//parse
			if( TryParseJWTToken( szJWTToken, out jwtObject ) == false ) {
				//parse error, means token error
				return eCode;
			}

			//check server ip
			if( isServerIPVaild( jwtObject ) == false ) {
				return eCode;
			}

			//check UserIP is vaild or not 
			if( isReceiveIPVaild( jwtObject ) == false ) {
				return eCode;
			}

			//if user From SynFactory in private cloud, Token will not expire 
			if( ConnectionInfo.isAWS || isJWTfromSynFactory( jwtObject ) == false ) {
				//check token is expire or not
				if( isTokenExpire( jwtObject ) ) {
					return eCode;
				}
			}

			//valid autho
			if( ConnectionInfo.isAWS == false ) {
				if( isAuthAccess( jwtObject ) == false ) {
					//means no autho
					eCode = ErrorCode.NoAuthorization;
					return eCode;
				}
			}

			//success
			eCode = ErrorCode.Success;
			return eCode;
		}
		public static async Task<string> CreateNewJWTObjectString( string szUserName, bool isSynFactoryLogin = false )
		{
			SyntecJWTModel NewToken = await CreateNewJWTObject( szUserName );

			//synfactory has no expire cookie
			if( isSynFactoryLogin ) {
				NewToken.sub = m_szSFCookieTag;
			}

			return TokenObjecttoString( NewToken );
		}
		public static bool isSynFactoryLoginValid( string szVaildCode )
		{
			if( szVaildCode == m_szSynFactoryIdentityCode ) {
				return true;
			}
			return false;
		}
		#endregion

		#region Private Method
		// parse
		static bool TryParseJWTToken( string szJWTToken, out SyntecJWTModel jwtObject )
		{
			//default
			jwtObject = null;

			//parse
			try {
				jwtObject = Jose.JWT.Decode<SyntecJWTModel>(
											szJWTToken,
											Encoding.UTF8.GetBytes( m_szJWTSecret ),
											JwsAlgorithm.HS256 );
				return true;
			}
			catch {
				return false;
			}
		}
		static string TokenObjecttoString( SyntecJWTModel objJWTToken )
		{
			string szJWT = Jose.JWT.Encode( objJWTToken, Encoding.UTF8.GetBytes( m_szJWTSecret ), JwsAlgorithm.HS256 );
			return szJWT;
		}

		// valid
		static bool isServerIPVaild( SyntecJWTModel obJWTToken )
		{
			if( obJWTToken.iss == System.Web.HttpContext.Current.Request.ServerVariables[ "LOCAL_ADDR" ] ) {
				return true;
			}
			return false;
		}
		static bool isReceiveIPVaild( SyntecJWTModel obJWTToken )
		{
			if( obJWTToken.aud == HttpContext.Current.Request.UserHostAddress ) {
				return true;
			}
			return false;
		}
		static bool isJWTfromSynFactory( SyntecJWTModel obJWTToken )
		{
			if( obJWTToken.sub == m_szSFCookieTag ) {
				return true;
			}
			return false;
		}
		static bool isTokenExpire( SyntecJWTModel obJWTToken )
		{
			double dCurrentTimestamp = ( DateTime.UtcNow.Subtract( new DateTime( 1970, 1, 1 ) ) ).TotalMilliseconds;
			if( obJWTToken.exp > dCurrentTimestamp ) {
				return false;
			}
			return true;
		}
		static bool isAuthAccess( SyntecJWTModel obJWTToken )
		{
			if( Authorization.isAccessAutho( obJWTToken.authorization ) ) {
				return true;
			}

			return false;
		}

		// create
		static async Task<SyntecJWTModel> CreateNewJWTObject( string szUserName )
		{
			SyntecJWTModel objNewJWT = new SyntecJWTModel();
			double dCurrentTime = ( DateTime.UtcNow.Subtract( new DateTime( 1970, 1, 1 ) ) ).TotalMilliseconds;

			//assign
			objNewJWT.iss = System.Web.HttpContext.Current.Request.ServerVariables[ "LOCAL_ADDR" ];
			objNewJWT.sub = "VaildUser";
			objNewJWT.aud = HttpContext.Current.Request.UserHostAddress;
			objNewJWT.exp = dCurrentTime + m_nExpireTime;
			objNewJWT.iat = dCurrentTime;
			objNewJWT.userName = szUserName;

			return objNewJWT;
		}

		#endregion

		#region Private Attribute
		const int m_nExpireTime = 86400000;
		const string m_szJWTSecret = "SyntecCloudFrontEndIsFuckingComplex";//JWT Code
		const string m_szSFCookieTag = "SynFactory";
		const string m_szSynFactoryIdentityCode = "SyntecCloudFrontEndIsFuckingComplexFromSynFactory";
		#endregion
	}
}
