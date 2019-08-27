using Newtonsoft.Json.Linq;
using System.IO;

namespace IncomeStatement.WebData.Server_Code.CommonModule
{
	public class ConfigReader
	{
		#region Public Method
		public static JObject GetConfigFile( Config config )
		{
			string szConfigName = GetConfigName( config );

			// no this config type
			if( szConfigName.Length == 0 ) {
				return null;
			}

			string szFileContent = ReadConfigJsonString( szConfigName );
			return JObject.Parse( szFileContent );
		}
		public static string GetConnection()
		{
			JObject jConn = GetConfigFile( Config.Connection );
			if( jConn == null ) {
				return string.Empty;
			}

			// for private
			string szTempIP = jConn[ "IP" ].ToString();
			string szTempPort = jConn[ "Port" ].ToString();
			if( szTempIP.Length == 0 || szTempPort.Length == 0 ) {
				szTempIP = ConnectionInfo.szConnectIP;
				szTempPort = ConnectionInfo.szConnectPort;
			}

			return szTempIP + ":" + szTempPort;
		}
		#endregion

		#region Public Attribute
		public enum Config
		{
			DBConnection,
			Connection,
		}
		#endregion

		#region Private Method
		static string GetConfigName( Config config )
		{
			switch( config ) {
				case Config.DBConnection:
					return "dbConnection.js";
				case Config.Connection:
					return "connection.js";
			}

			return string.Empty;
		}
		static string ReadConfigJsonString( string szConfigFileName )
		{
			// only accept json in Object, not allow Array
			string szConfigFilePath = m_szConfigPath + szConfigFileName;
			string szTotalString = "";
			string szReadLine = "";
			if( File.Exists( szConfigFilePath ) == false ) {
				return "{}";
			}

			// read config file
			StreamReader file = new StreamReader( szConfigFilePath );
			while( ( szReadLine = file.ReadLine() ) != null ) {
				szTotalString += szReadLine + ",";
			}
			file.Close();

			return szTotalString;
		}
		#endregion

		#region Private Attribute
		static string m_szConfigPath = FileLocation.ConfigPath;
		#endregion
	}
}
