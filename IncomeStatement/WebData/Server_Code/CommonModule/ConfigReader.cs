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

			// file not exists
			if( szFileContent == m_szDefaultJson ) {
				return null;
			}

			return JObject.Parse( szFileContent );
		}
		public static string GetConnection()
		{
			JObject jConn = GetConfigFile( Config.Connection );
			if( jConn == null ) {
				return string.Empty;
			}

			// for aws
			if( jConn[ "HostName" ] != null && jConn[ "HostName" ].ToString().Length != 0 ) {
				return jConn[ "HostName" ].ToString();
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
			Connection,
			DBAPI,
			CNCAPI,
			AWSAPI
		}
		#endregion

		#region Private Method
		static string GetConfigName( Config config )
		{
			switch( config ) {
				case Config.Connection:
					return "ConnectionConfig.ini";
				case Config.DBAPI:
					return "DBRestfulAPITemplate.ini";
				case Config.CNCAPI:
					return "RestfulAPITemplate.ini";
				case Config.AWSAPI:
					return "AWSAPITemplate.ini";
			}

			return string.Empty;
		}
		static string ReadConfigJsonString( string szConfigFileName )
		{
			// only accept json in Object, not allow Array
			string szConfigFilePath = m_szConfigPath + szConfigFileName;
			string szJsonConfigString = "{";
			string szReadLine = "";
			if( File.Exists( szConfigFilePath ) == false ) {
				return "{}";
			}

			// read config file
			StreamReader file = new StreamReader( szConfigFilePath );
			while( ( szReadLine = file.ReadLine() ) != null ) {
				szJsonConfigString += szReadLine + ",";
			}
			file.Close();

			// package config string
			szJsonConfigString = szJsonConfigString.Remove( szJsonConfigString.Length - 1 );
			szJsonConfigString += "}";

			return szJsonConfigString;
		}
		#endregion

		#region Private Attribute
		const string m_szDefaultJson = "{}";
		static string m_szConfigPath = FileLocation.ConfigPath;
		#endregion
	}
}
