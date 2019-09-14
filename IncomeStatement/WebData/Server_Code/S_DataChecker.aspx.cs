using IncomeStatement.WebData.Server_Code.CommonModule;
using IncomeStatement.WebData.Server_Code.CommonModule.mssql;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IncomeStatement.WebData.Server_Code
{
	public partial class S_DataChecker : System.Web.UI.Page
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

				if( szAction.ToUpper() == "CHECK" ) {
					return ApiAction.CHECK;
				}

				return ApiAction.UNKNOW;
			}
			catch {
				return ApiAction.UNKNOW;
			}
		}
		bool isParamValid( ApiAction action )
		{
			return true;
		}
		dynamic DoAction( ApiAction action )
		{
			return null;
		}

		enum ApiAction
		{
			READ,
			CHECK,
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
			public static string DayRemark
			{
				get
				{
					return "Remark";
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
		List<string> m_paramList = new List<string>();
	}
}
