using IncomeStatement.WebData.Server_Code.CommonModule;
using IncomeStatement.WebData.Server_Code.CommonModule.mssql;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IncomeStatement.WebData.Server_Code
{
	public partial class S_IncomeStateData : System.Web.UI.Page
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

				if( szAction.ToUpper() == "CONFIRM" ) {
					return ApiAction.CONFIRM;
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
					int nTempValue;

					// check data
					if( Request.Form[ Param.Year ] != null && Request.Form[ Param.Month ] != null ) {
						string szYear = Request.Form[ Param.Year ].ToString();
						string szMonth = Request.Form[ Param.Month ].ToString();

						if(
							int.TryParse(szYear, out nTempValue) == false ||
							int.TryParse(szMonth, out nTempValue) == false
						) {
							return false;
						}

						m_paramExpDList.Add($"ie_year={szYear} AND ie_mon={szMonth}");
					}

					// check family no
					if( Request.Form[ Param.FamNoStart ] != null && Request.Form[ Param.FamNoEnd ] != null ) {
						string szFamNoStart = Request.Form[ Param.FamNoStart ].ToString();
						string szFamNoEnd = Request.Form[ Param.FamNoEnd ].ToString();

						if(
							int.TryParse(szFamNoStart, out nTempValue) == false ||
							int.TryParse(szFamNoEnd, out nTempValue) == false
						) {
							return false;
						}

						m_paramExpDList.Add($"fam_no BETWEEN {szFamNoStart} AND {szFamNoEnd}");
					}

					// check record no
					if( Request.Form[ Param.RecNo ] != null ) {
						string szRecNo = Request.Form[ Param.RecNo ].ToString();

						if( int.TryParse(szRecNo, out nTempValue) == false ) {
							return false;
						}

						m_paramExpDList.Add($"rec_user = {szRecNo}");
					}
					
					// no param
					if( m_paramExpDList.Count == 0 ) {
						return false;
					}

					return true;
				}
				if( action == ApiAction.CONFIRM ) {
					string szConfirmList = Request.Form[ Param.ConfirmList ].ToString();
					JArray.Parse(szConfirmList);
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

			if( action == ApiAction.CONFIRM ) {
				return isConfirmData();
			}

			return null;
		}
		dynamic ReadData()
		{
			// get param
			string szSQL = $"SELECT * FROM {TableName.CoFam} WHERE";
			for( int i = 0; i < m_paramExpDList.Count; i++ ) {
				szSQL += $" {m_paramExpDList[ i ]}";
				szSQL += i == m_paramExpDList.Count - 1 ? " " : " AND";
			}
			JArray result;
			bool isSuccess = m_mssql.TryQuery(szSQL, out result);
			return result;
		}
		bool isConfirmData()
		{
			// get all need to confirm data
			List<JObject> jObjectList = JsonConvert.DeserializeObject<List<JObject>>(Request.Form[ Param.ConfirmList ].ToString());
			string szWhere = $"fam_no IN({string.Join(", ", jObjectList.Select(data => $"'{data[ "fam_no" ]}'").ToArray())}) ";
			szWhere += $"AND ie_year={jObjectList[ 0 ][ "ie_year" ]} AND ie_mon={jObjectList[ 0 ][ "ie_mon" ]}";
			string szErrorMsg;

			// insert into log
			string szInertCoFamLog = $"INSERT INTO {TableName.CoFamLog} SELECT 'M', CURRENT_TIMESTAMP, 'SYS', * FROM {TableName.CoFam} WHERE {szWhere}";
			string szInsertExMLog = $"INSERT INTO {TableName.CoExpMLog} SELECT 'M', CURRENT_TIMESTAMP, 'SYS', * FROM {TableName.CoExpM} WHERE {szWhere}";
			m_mssql.TryQuery(szInertCoFamLog, out szErrorMsg);
			m_mssql.TryQuery(szInsertExMLog, out szErrorMsg);

			// update co_fam
			string szUpdateCoFam = $"UPDATE {TableName.CoFam} " +
				$"SET state=2 " +
				$"WHERE {szWhere}";
			bool isSuccess = m_mssql.TryQuery(szUpdateCoFam, out szErrorMsg);
			if( isSuccess == false ) {
				return false;
			}

			// update co_exp_m
			string szUpdateCoExpM = $"UPDATE {TableName.CoExpM} " +
				$"SET state=2 " +
				$"WHERE {szWhere}";
			isSuccess = m_mssql.TryQuery(szUpdateCoFam, out szErrorMsg);
			return isSuccess;
		}
		#endregion

		#region Private Attribute
		enum ApiAction
		{
			READ,
			CONFIRM,
			UNKNOW,
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

			// for query data
			public static string Year
			{
				get
				{
					return "Year";
				}
			}
			public static string Month
			{
				get
				{
					return "Month";
				}
			}
			public static string FamNoStart
			{
				get
				{
					return "FamNoStart";
				}
			}
			public static string FamNoEnd
			{
				get
				{
					return "FamNoEnd";
				}
			}
			public static string RecNo
			{
				get
				{
					return "RecNo";
				}
			}

			// for confirm data
			public static string ConfirmList
			{
				get
				{
					return "ConfirmList";
				}
			}
		}
		MSSQL m_mssql = new MSSQL();
		List<string> m_paramExpDList = new List<string>();
		#endregion
	}
}
