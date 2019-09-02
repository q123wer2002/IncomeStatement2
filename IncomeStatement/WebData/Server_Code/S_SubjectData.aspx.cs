using IncomeStatement.WebData.Server_Code.CommonModule;
using IncomeStatement.WebData.Server_Code.CommonModule.mssql;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IncomeStatement.WebData.Server_Code
{
	public partial class S_SubjectData : System.Web.UI.Page
	{
		RequestHandler m_requestHandler;
		protected void Page_Load( object sender, EventArgs e )
		{
			// check status, set default
			m_requestHandler = new RequestHandler();
			m_requestHandler.StatusCode = (int)ErrorCode.Error;
			m_requestHandler.ReturnData = "Param Error";

			ErrorCode ec = m_requestHandler.RequestValid(Request);
			if( ec != ErrorCode.Success ) {
				m_requestHandler.StatusCode = (int)ec;
				m_requestHandler.ReturnData = ec.ToString();
				Response.Write(m_requestHandler.GetReturnResult());
				return;
			}

			// check action
			ApiAction action = GetAction();
			if( action == ApiAction.UNKNOW ) {
				Response.Write(m_requestHandler.GetReturnResult());
				return;
			}

			// check param valid
			if( isParamValid(action) == false ) {
				Response.Write(m_requestHandler.GetReturnResult());
				return;
			}

			// do action
			dynamic result = DoAction(action);
			m_requestHandler.StatusCode = (int)ErrorCode.Success;
			m_requestHandler.ReturnData = result;
			Response.Write(m_requestHandler.GetReturnResult());
		}
		protected void Page_Error( object sender, EventArgs e )
		{
			// get error
			Exception ex = Server.GetLastError();

			// return
			m_requestHandler.StatusCode = (int)ErrorCode.Error;
			m_requestHandler.ReturnData = (ConnectionInfo.isDebugMode) ? ex.ToString() : string.Empty;
			Response.Write(m_requestHandler.GetReturnResult());
			Server.ClearError();
		}

		#region Private Method
		ApiAction GetAction()
		{
			try {
				string szAction = Request.Form[ Param.Action ].ToString();
				if( szAction.ToUpper() == "READ" ) {
					return ApiAction.READ;
				}

				if( szAction.ToUpper() == "WRITE" ) {
					return ApiAction.WRITE;
				}

				if( szAction.ToUpper() == "DELETE" ) {
					return ApiAction.DELETE;
				}

				return ApiAction.UNKNOW;
			}
			catch {
				return ApiAction.UNKNOW;
			}
		}
		bool isParamValid( ApiAction action )
		{
			try {
				if( action == ApiAction.READ ) {
					if( Request.Form[ Param.CodeNo ] != null ) {
						string szCodeNo = Request.Form[ Param.CodeNo ].ToString();
						int nCode = int.Parse(szCodeNo);
						if( nCode != -1 ) {
							m_paramList.Add($"code_no LIKE '{nCode}%'");
						}
						else {
							m_paramList.Add($"code_no LIKE '%'");
						}
						
					}

					if( Request.Form[ Param.CodeName ] != null ) {
						string szCodeName = Request.Form[ Param.CodeName ].ToString();
						m_paramList.Add($"code_name={szCodeName}");
					}

					if( m_paramList.Count == 0 ) {
						return false;
					}
					return true;
				}
				if( action == ApiAction.WRITE ) {
					string szSubject = Request.Form[ Param.Subject ].ToString();
					JObject.Parse(szSubject);
					return true;
				}
				if( action == ApiAction.DELETE ) {
					string szSubjectAry = Request.Form[ Param.SubjectArray ].ToString();
					JArray.Parse(szSubjectAry);
					return true;
				}
				return false;
			}
			catch {
				return false;
			}
		}
		dynamic DoAction( ApiAction action )
		{
			if( action == ApiAction.READ ) {
				return ReadData();
			}

			if( action == ApiAction.WRITE ) {
				return Write();
			}

			if( action == ApiAction.DELETE ) {
				return Delete();
			}

			return null;
		}
		JArray ReadData()
		{
			// get param
			string szSQL = $"SELECT * FROM {TableName.CoExpCode} WHERE";
			for( int i = 0; i < m_paramList.Count; i++ ) {
				szSQL += $" {m_paramList[ i ]}";
				szSQL += i == m_paramList.Count - 1 ? " " : " AND";
			}
			JArray result;
			bool isSuccess = m_mssql.TryQuery(szSQL, out result);
			return result;
		}
		bool Write()
		{
			// get subject object
			JObject jSubject = JObject.Parse(Request.Form[ Param.Subject ].ToString());

			int nUpLim, nLowLim;
			if( int.TryParse(jSubject[ "upp_lim" ].ToString(), out nUpLim) == false ) {
				nUpLim = -1;
			}
			if( int.TryParse(jSubject[ "low_lim" ].ToString(), out nLowLim) == false ) {
				nLowLim = -1;
			}

			// insert or update
			string szInsertOrUpdate = $"BEGIN TRAN " +
				$"IF EXISTS( " +
					$"SELECT * FROM {TableName.CoExpCode} WHERE code_no={jSubject[ "code_no" ]} " +
				$")" +
					$"BEGIN UPDATE {TableName.CoExpCode} SET code_rem=N'{jSubject[ "code_rem" ]}', upp_lim={(nUpLim >= 0 ? nUpLim.ToString() : "NULL")}, low_lim={(nLowLim >= 0 ? nLowLim.ToString() : "NULL")}, place='{jSubject[ "place" ]}', param1=N'{jSubject[ "param1" ]}', param2=N'{jSubject[ "param2" ]}', stop_fg='{jSubject[ "stop_fg" ]}', upd_date=CURRENT_TIMESTAMP WHERE code_no={jSubject[ "code_no" ]} " +
					$"END " +
				$"ELSE " +
					$"BEGIN INSERT INTO {TableName.CoExpCode} " +
						$"VALUES ({jSubject[ "code_no" ]}, N'{jSubject[ "code_name" ]}', N'{jSubject[ "code_desc" ]}', {jSubject[ "code1" ]}, {jSubject[ "code2" ]}, N'{jSubject[ "code_rem" ]}', {(nUpLim >= 0 ? nUpLim.ToString() : "NULL")}, {(nLowLim >= 0 ? nLowLim.ToString() : "NULL")}, '{jSubject[ "place" ]}', N'{jSubject[ "param1" ]}', N'{jSubject[ "param2" ]}', '{jSubject[ "stop_fg" ]}', CURRENT_TIMESTAMP, 'SYS')" +
					$"END " +
				$"COMMIT TRAN";
			string szErrorMsg;
			return m_mssql.TryQuery(szInsertOrUpdate, out szErrorMsg);
		}
		bool Delete()
		{
			List<JObject> jSubjectList = JsonConvert.DeserializeObject<List<JObject>>(Request.Form[ Param.SubjectArray ].ToString());

			string szDelete = $"DELETE FROM {TableName.CoExpCode} WHERE code_no IN ({string.Join(", ", jSubjectList.Select(obj => $"'{obj[ "code_no" ]}'"))})";
			string szErrorMsg;
			return m_mssql.TryQuery(szDelete, out szErrorMsg);
		}
		#endregion

		#region Private Attribute
		enum ApiAction
		{
			READ,
			WRITE,
			DELETE,
			UNKNOW
		}
		class Param
		{
			public static string Action
			{
				get
				{
					return "Action";
				}
			}

			// for read
			public static string CodeNo
			{
				get
				{
					return "CodeNo";
				}
			}
			public static string CodeName
			{
				get
				{
					return "CodeName";
				}
			}

			// for write
			public static string Subject
			{
				get
				{
					return "Subject";
				}
			}

			// for delete
			public static string SubjectArray
			{
				get
				{
					return "SubjectArray";
				}
			}
		}
		MSSQL m_mssql = new MSSQL();
		List<string> m_paramList = new List<string>();
		#endregion
	}
}
