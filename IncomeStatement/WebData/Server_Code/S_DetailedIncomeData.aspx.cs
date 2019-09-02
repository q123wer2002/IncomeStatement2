﻿using IncomeStatement.WebData.Server_Code.CommonModule;
using IncomeStatement.WebData.Server_Code.CommonModule.mssql;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IncomeStatement.WebData.Server_Code
{
	public partial class S_DetailedIncomeData : System.Web.UI.Page
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
				int nTempValue;

				// check fam no
				if( Request.Form[ Param.FamNo ] == null ) {
					return false;
				}

				string szFamNo = Request.Form[ Param.FamNo ].ToString();
				int.Parse(szFamNo);

				m_paramExpDList.Add($"{TableName.CoExpD}.fam_no={szFamNo}");
				m_paramExpMList.Add($"{TableName.CoExpM}.fam_no={szFamNo}");

				if( action == ApiAction.READ ) {
					// check date
					if( Request.Form[ Param.Year ] != null && Request.Form[ Param.Month ] != null ) {
						string szYear = Request.Form[ Param.Year ].ToString();
						string szMonth = Request.Form[ Param.Month ].ToString();

						if(
							int.TryParse(szYear, out nTempValue) == false ||
							int.TryParse(szMonth, out nTempValue) == false
						) {
							return false;
						}

						m_paramExpDList.Add($"{TableName.CoExpD}.ie_year={szYear} AND {TableName.CoExpD}.ie_mon={szMonth}");
						m_paramExpMList.Add($"{TableName.CoExpM}.ie_year={szYear} AND {TableName.CoExpM}.ie_mon={szMonth}");
					}

					if( Request.Form[ Param.Day ] != null ) {
						string szDay = Request.Form[ Param.Day ].ToString();
						if( int.TryParse(szDay, out nTempValue) == false ) {
							return false;
						}

						m_paramExpDList.Add($"{TableName.CoExpD}.ie_day={szDay}");
						m_paramExpMList.Add($"{TableName.CoExpM}.ie_day={szDay}");
					}

					// check duration
					if( Request.Form[ Param.DurationStart ] != null && Request.Form[ Param.DurationEnd ] != null ) {
						string szDurationStart = Request.Form[ Param.DurationStart ].ToString();
						string szDurationEnd = Request.Form[ Param.DurationEnd ].ToString();

						if(
							int.TryParse(szDurationStart, out nTempValue) == false ||
							int.TryParse(szDurationEnd, out nTempValue) == false
						) {
							return false;
						}

						m_paramExpDList.Add($"{TableName.CoExpD}.ie_day BETWEEN {szDurationStart} AND {szDurationEnd}");
						m_paramExpMList.Add($"{TableName.CoExpM}.ie_day BETWEEN {szDurationStart} AND {szDurationEnd}");
					}

					// check no
					if( Request.Form[ Param.CodeNo ] != null ) {
						string szCodeNo = Request.Form[ Param.CodeNo ].ToString();
						int nCode = int.Parse(szCodeNo);
						m_paramExpDList.Add($"{TableName.CoExpD}.code_no={nCode}");
					}

					if( m_paramExpDList.Count == 0 ) {
						return false;
					}
					return true;
				}
				if( action == ApiAction.WRITE ) {
					// get item array
					string szUpdateItems = Request.Form[ Param.UpdateItems ].ToString();
					JArray.Parse(szUpdateItems);
					string szInsertItems = Request.Form[ Param.InsertItems ].ToString();
					JArray.Parse(szInsertItems);

					// get date
					string szYear = Request.Form[ Param.Year ].ToString();
					int.Parse(szYear);
					string szMonth = Request.Form[ Param.Month ].ToString();
					int.Parse(szMonth);
					string szDay = Request.Form[ Param.Day ].ToString();
					int.Parse(szDay);

					m_paramExpDList.Add($"{TableName.CoExpD}.ie_year={szYear} AND {TableName.CoExpD}.ie_mon={szMonth} AND {TableName.CoExpD}.ie_day={szDay}");
					m_paramExpMList.Add($"{TableName.CoExpM}.ie_year={szYear} AND {TableName.CoExpM}.ie_mon={szMonth} AND {TableName.CoExpM}.ie_day={szDay}");

					// check total cosst
					string szTotalCost = Request.Form[ Param.TotalCost ].ToString();
					int.Parse(szTotalCost);
					return true;
				}
				if( action == ApiAction.DELETE ) {
					string szItemArray = Request.Form[ Param.ItemArray ].ToString();
					JArray.Parse(szItemArray);
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
		JObject ReadData()
		{
			JObject jResult = new JObject();

			// get co_exp_d
			string szSQL = $"SELECT {TableName.CoExpD}.*, {TableName.CoExpM}.exp_amt FROM {TableName.CoExpD} " +
				$"LEFT JOIN {TableName.CoExpM} ON {TableName.CoExpD}.ie_year={TableName.CoExpM}.ie_year AND {TableName.CoExpD}.ie_mon={TableName.CoExpM}.ie_mon AND {TableName.CoExpD}.ie_day={TableName.CoExpM}.ie_day AND {TableName.CoExpD}.fam_no={TableName.CoExpM}.fam_no " +
				$"WHERE ";
			for( int i = 0; i < m_paramExpDList.Count; i++ ) {
				szSQL += $" {m_paramExpDList[ i ]}";
				szSQL += i == m_paramExpDList.Count - 1 ? " " : " AND";
			}
			JArray result;
			bool isSuccess = m_mssql.TryQuery(szSQL, out result);
			if( isSuccess ) {
				jResult[ "CoExpD" ] = result;
			}

			// get co_exp_m
			szSQL = $"SELECT * FROM {TableName.CoExpM} WHERE ";
			for( int i = 0; i < m_paramExpMList.Count; i++ ) {
				szSQL += $" {m_paramExpMList[ i ]}";
				szSQL += i == m_paramExpMList.Count - 1 ? " " : " AND";
			}
			isSuccess = m_mssql.TryQuery(szSQL, out result);
			if( isSuccess ) {
				jResult[ "CoExpM" ] = result;
			}

			return jResult;
		}
		bool Write()
		{
			// for update
			List<JObject> updateItems = JsonConvert.DeserializeObject<List<JObject>>(Request.Form[ Param.UpdateItems ].ToString());
			DeleteItems("M", updateItems.Select(obj => int.Parse(obj[ "item_no" ].ToString())).ToList());
			InsertIntems(true, updateItems);

			// for insert
			List<JObject> insertItems = JsonConvert.DeserializeObject<List<JObject>>(Request.Form[ Param.InsertItems ].ToString());
			if( insertItems.Count > 0 ) {
				InsertIntems(false, insertItems);
			}

			// update cost
			string szErrorMsg;
			int nTotalCosst = int.Parse(Request.Form[ Param.TotalCost ].ToString());
			string szWhere = "";
			for( int i = 0; i < m_paramExpMList.Count; i++ ) {
				szWhere += $" {m_paramExpMList[ i ]}";
				szWhere += i == m_paramExpMList.Count - 1 ? " " : " AND";
			}
			string szInsertSql = $"INSERT INTO {TableName.CoExpMLog} SELECT 'M', CURRENT_TIMESTAMP, 'SYS', * FROM {TableName.CoExpM} WHERE {szWhere}";
			string szUpdate = $"UPDATE {TableName.CoExpM} SET exp_amt='{nTotalCosst}' WHERE {szWhere}";
			m_mssql.TryQuery(szInsertSql, out szErrorMsg);
			m_mssql.TryQuery(szUpdate, out szErrorMsg);

			return true;
		}
		bool Delete()
		{
			List<int> itemNoList = JsonConvert.DeserializeObject<List<int>>(Request.Form[ Param.ItemArray ].ToString());
			return DeleteItems("D", itemNoList);
		}
		bool DeleteItems( string szReason, List<int> itemNos )
		{
			string szWhere = $"item_no IN({string.Join(", ", itemNos.Select(itemNo => $"'{itemNo}'"))}) AND ";
			string szErrorMsg;
			for( int i = 0; i < m_paramExpDList.Count; i++ ) {
				szWhere += $" {m_paramExpDList[ i ]}";
				szWhere += i == m_paramExpDList.Count - 1 ? " " : " AND";
			}

			// copy currnt data into log file
			string szInsertLog = $"INSERT INTO {TableName.CoExpDLog} SELECT '{szReason}', CURRENT_TIMESTAMP, 'SYS', * FROM {TableName.CoExpD} WHERE {szWhere}";
			bool isSuccess = m_mssql.TryQuery(szInsertLog, out szErrorMsg);
			if( isSuccess == false ) {
				return false;
			}

			// delete items
			string szDelete = $"DELETE FROM {TableName.CoExpD} WHERE {szWhere}";
			isSuccess = m_mssql.TryQuery(szDelete, out szErrorMsg);
			return isSuccess;
		}
		bool InsertIntems( bool isOwnItemNo, List<JObject> items )
		{
			int nNextItemNo = 0;
			if( isOwnItemNo == false ) {
				// get the newest item_no
				string szGetItemNo = $"SELECT TOP(1) item_no FROM {TableName.CoExpD} WHERE fam_no='{Request.Form[ Param.FamNo ].ToString()}' ORDER BY item_no DESC";
				JArray result;
				m_mssql.TryQuery(szGetItemNo, out result);
				nNextItemNo = int.Parse(((JObject)result[ 0 ])[ "item_no" ].ToString());
			}

			string szFamNo = Request.Form[ Param.FamNo ].ToString();
			string szErrorMsg;
			string szInsert = $"INSERT INTO {TableName.CoExpD} VALUES ";
			for( int i = 0; i < items.Count; i++ ) {
				JObject jItem = items[ i ];
				int nItemNo = isOwnItemNo == false ? nNextItemNo + 1 : int.Parse(jItem[ "item_no" ].ToString());
				szInsert += $"('{jItem[ "ie_year" ].ToString()}', '{jItem[ "ie_mon" ].ToString()}', '{jItem[ "ie_day" ].ToString()}', '{szFamNo}', '{nItemNo}', '{jItem[ "place" ].ToString()}', '{jItem[ "code_amt" ].ToString()}', '{jItem[ "code_no" ].ToString()}', '{jItem[ "code_name" ].ToString()}', NULL, NULL, CURRENT_TIMESTAMP, NULL )";
				szInsert += i == items.Count - 1 ? " " : ", ";
			}
			bool isSuccess = m_mssql.TryQuery(szInsert, out szErrorMsg);
			return isSuccess;
		}

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
			public static string FamNo
			{
				get
				{
					return "FamNo";
				}
			}
			public static string DurationStart
			{
				get
				{
					return "DurationStart";
				}
			}
			public static string DurationEnd
			{
				get
				{
					return "DurationEnd";
				}
			}
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
			public static string Day
			{
				get
				{
					return "Day";
				}
			}
			public static string UpdateItems
			{
				get
				{
					return "UpdateItems";
				}
			}
			public static string InsertItems
			{
				get
				{
					return "InsertItems";
				}
			}
			public static string TotalCost
			{
				get
				{
					return "TotalCost";
				}
			}

			// for delete
			public static string ItemArray
			{
				get
				{
					return "ItemArray";
				}
			}
		}
		MSSQL m_mssql = new MSSQL();
		List<string> m_paramExpDList = new List<string>();
		List<string> m_paramExpMList = new List<string>();
	}
}
