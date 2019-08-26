using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IncomeStatement.WebData.Server_Code.CommonModule
{
	public class RequestHandler
	{
		#region Constructor
		public RequestHandler( bool isAWS, string szConnection )
		{
			m_isAWS = isAWS;
			m_szConnection = szConnection;
		}
		public ErrorCode RequestValid( HttpRequest httpRequest )
		{
			ErrorCode eCode = ErrorCode.AuthenticationError;
			if( httpRequest.Cookies[ "SyntecJWT" ] == null ) {
				return eCode;
			}

			//get JWT Object 
			HttpCookie objCookie = httpRequest.Cookies[ "SyntecJWT" ];
			string szTokenString = objCookie.Value;

			//check token is valid or not
			ErrorCode jwtCode = JWTChecker.isJWTValid( szTokenString, out m_JWTToken );
			if( jwtCode != ErrorCode.Success ) {
				return jwtCode;
			}

			//if it's AWS , check header token exist or not  & set token
			if( m_isAWS ) {
				if( isAWSTokenExist( httpRequest ) == false ) {
					return eCode;
				}

				SetAWSHeaders( httpRequest );
			}

			eCode = ErrorCode.Success;
			return eCode;
		}
		#endregion

		#region Public Method
		public dynamic GetReturnResult()
		{
			return m_objReturnReturn;
		}
		#endregion

		#region Public Attribute
		public int StatusCode
		{
			get
			{
				return m_objReturnReturn.status;
			}
			set
			{
				m_objReturnReturn.status = value;
			}
		}
		public dynamic ReturnData
		{
			get
			{
				return m_objReturnReturn.data;
			}
			set
			{
				m_objReturnReturn.data = value;
			}
		}
		#endregion

		#region Private Method
		void SetAWSHeaders( HttpRequest httpRequest )
		{
			// create new object
			if( m_dicAdditionalHeaders == null ) {
				m_dicAdditionalHeaders = new Dictionary<string, string>();
			}

			// re-assign value
			for( int i = 0; i < m_lstAWSHeaders.Count; i++ ) {
				NameValueCollection ReuestHeaders = httpRequest.Headers;
				string szTokenString = ReuestHeaders.GetValues( m_lstAWSHeaders[ i ] )[ 0 ];
				m_dicAdditionalHeaders.Add( m_lstAWSHeaders[ i ], szTokenString );
			}
		}
		bool isAWSTokenExist( HttpRequest httpRequest )
		{
			NameValueCollection ReuestHeaders = httpRequest.Headers;

			foreach( string szAWSTokenKey in m_lstAWSHeaders ) {
				if( ReuestHeaders.GetValues( szAWSTokenKey ) == null ) {
					return false;
				}
			}
			return true;
		}
		#endregion

		#region Private Arrtibute
		bool m_isAWS;
		string m_szConnection;
		SyntecJWTModel m_JWTToken;
		dynamic m_objReturnReturn = new JObject();
		List<string> m_lstAWSHeaders = new List<string>() {
			"IdToken",
			"AccessToken",
			"RefreshToken"
		};
		Dictionary<string, string> m_dicAdditionalHeaders = null;
		#endregion
	}
}
