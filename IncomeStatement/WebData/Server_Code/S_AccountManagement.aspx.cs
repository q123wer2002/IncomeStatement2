﻿using IncomeStatement.WebData.Server_Code.CommonModule;
using IncomeStatement.WebData.Server_Code.CommonModule.mssql;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace IncomeStatement.WebData.Server_Code
{
	public partial class S_AccountManagement : System.Web.UI.Page
	{
		RequestHandler m_requestHandler;
		string szUserCode;
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

			// get user id
			szUserCode = Request.Cookies[ CookieKey.UserID ].Value;

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
				switch( action ) {
					case ApiAction.READ:
						if( Request.Form[ Param.Account ] != null ) {
							string szAccount = Request.Form[ Param.Account ].ToString();
							m_param.Add($"{TableName.CoSysAuth}.login_id LIKE '%{szAccount}%'");
						}

						if( Request.Form[ Param.Name ] != null ) {
							string szName = Request.Form[ Param.Name ].ToString();
							m_param.Add($"{TableName.CoSysAuth}.user_name LIKE '%{szName}%'");
						}

						if( Request.Form[ Param.Status ] != null ) {
							string szState = Request.Form[ Param.Status ].ToString();
							if( szState != "-1" ) {
								m_param.Add($"{TableName.CoSysAuth}.state = '{szState}'");
							}
						}

						return true;
					case ApiAction.WRITE:
					case ApiAction.DELETE:
						JObject jUser = JObject.Parse(Request.Form[ Param.UserObject ].ToString());
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
			switch( action ) {
				case ApiAction.READ:
					return ReadData();
				case ApiAction.WRITE:
					return UpdateOrInsertUser();
				case ApiAction.DELETE:
					return DeleteUser();
				default:
					return null;
			}
		}
		dynamic ReadData()
		{
			string szSelectSQL = $"SELECT {TableName.CoSysAuth}.*, {TableName.CoSysUser}.* FROM {TableName.CoSysAuth} " +
				$"LEFT JOIN {TableName.CoSysUser} ON {TableName.CoSysUser}.user_id={TableName.CoSysAuth}.login_id ";

			if( m_param.Count > 0 ) {
				szSelectSQL += "WHERE ";
				for( int i = 0; i < m_param.Count; i++ ) {
					szSelectSQL += m_param[ i ];
					szSelectSQL += i == m_param.Count - 1 ? " " : " AND ";
				}
			}

			JArray jResult;
			m_mssql.TryQuery(szSelectSQL, out jResult);
			return jResult;
		}
		dynamic UpdateOrInsertUser()
		{
			JObject jUser = JObject.Parse(Request.Form[ Param.UserObject ].ToString());
			string szEmail = jUser[ "email" ].ToString().Length == 0 ? "NULL" : $"'{jUser[ "email" ].ToString()}'";
			string szTelNo = jUser[ "tel_no" ].ToString().Length == 0 ? "NULL" : $"'{jUser[ "email" ].ToString()}'";
			string szTitle = jUser[ "title" ].ToString().Length == 0 ? "NULL" : $"'{jUser[ "email" ].ToString()}'";
			string szDepName = jUser[ "dep_name" ].ToString().Length == 0 ? "NULL" : $"'{jUser[ "email" ].ToString()}'";
			string szRemark = jUser[ "remark" ].ToString().Length == 0 ? "NULL" : $"'{jUser[ "email" ].ToString()}'";
			DateTime dtStart = DateTime.Parse(jUser[ "start_date" ].ToString());
			string szEndDate = jUser[ "end_date" ].ToString().Length == 0 ? dtStart.AddYears(2).ToString("yyyy-MM-dd") : jUser[ "end_date" ].ToString();

			// select once
			string szErrorMsg;
			JArray jTempUser;
			m_mssql.TryQuery($"SELECT * FROM {TableName.CoSysAuth} WHERE login_id='{jUser[ "login_id" ].ToString()}'", out jTempUser);
			if( jTempUser.Count == 1 ) {
				// means update
				bool isSuccess;
				
				// co_sys_auth
				isSuccess = m_mssql.TryQuery($"UPDATE {TableName.CoSysAuth} " +
					$"SET pwd='{jUser[ "pwd" ].ToString()}', " +
					$"role='{jUser[ "role" ].ToString()}', " +
					$"state='{jUser[ "state" ].ToString()}', " +
					$"start_date='{jUser[ "start_date" ].ToString()}', " +
					$"end_date='{szEndDate}', " +
					$"upd_date=CURRENT_TIMESTAMP, " +
					$"upd_user='{szUserCode}'" +
					$"WHERE login_id='{jUser[ "login_id" ].ToString()}'", out szErrorMsg);
				if( isSuccess == false ) {
					return false;
				}

				// co_sys_user
				return m_mssql.TryQuery($"UPDATE {TableName.CoSysUser} " +
					$"SET user_name='{jUser[ "user_name" ].ToString()}', " +
					$"email={szEmail}, " +
					$"tel_no={szTelNo}, " +
					$"title={szTitle}, " +
					$"dep_name={szDepName}, " +
					$"remark={szRemark}, " +
					$"upd_date=CURRENT_TIMESTAMP, " +
					$"upd_user='{szUserCode}'" +
					$"WHERE user_id='{jUser[ "login_id" ].ToString()}'", out szErrorMsg);
			}
			else {
				//means insert
				// means update
				bool isSuccess;

				// co_sys_auth
				isSuccess = m_mssql.TryQuery($"INSERT INTO {TableName.CoSysAuth} VALUES ('{jUser[ "login_id" ].ToString()}', '937e8d5fbb48bd4949536cd65b8d35c426b80d2f830c5c308e2cdec422ae2244', 0, '{jUser[ "role" ].ToString()}', '{jUser[ "login_id" ].ToString()}', '{jUser[ "state" ].ToString()}', '{jUser[ "start_date" ].ToString()}', '{szEndDate}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, CURRENT_TIMESTAMP, '{szUserCode}', NULL, NULL)", out szErrorMsg);
				if( isSuccess == false ) {
					return false;
				}
			
				// co_sys_user
				return m_mssql.TryQuery($"INSERT INTO {TableName.CoSysUser} VALUES (" +
					$"'{jUser[ "login_id" ].ToString()}', '{jUser[ "user_name" ].ToString()}', {szEmail}, {szTelNo}, {szTitle}, {szDepName}, {szRemark}, CURRENT_TIMESTAMP, '{szUserCode}', NULL, NULL)", out szErrorMsg);
			}
		}
		dynamic DeleteUser()
		{
			JObject jUser = JObject.Parse(Request.Form[ Param.UserObject ].ToString());
			string szErrorMsg;
			return m_mssql.TryQuery($"DELETE FROM {TableName.CoSysAuth} WHERE login_id='{jUser[ "login_id" ].ToString()}'", out szErrorMsg);
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
			public static string Account
			{
				get
				{
					return "Account";
				}
			}
			public static string Name
			{
				get
				{
					return "Name";
				}
			}
			public static string Status
			{
				get
				{
					return "Status";
				}
			}

			// for insert
			public static string UserObject
			{
				get
				{
					return "UserObject";
				}
			}
		}
		MSSQL m_mssql = new MSSQL();
		List<string> m_param = new List<string>();
	}
}
