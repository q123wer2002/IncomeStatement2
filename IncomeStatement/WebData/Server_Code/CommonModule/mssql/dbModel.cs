namespace IncomeStatement.WebData.Server_Code.CommonModule.mssql
{
	public class ColumnModel
	{
		public string ColName
		{
			get;set;
		}
		public string ColType
		{
			get;set;
		}
		public bool IsPrimaryKey
		{
			get;set;
		}
		public bool IsNull
		{
			get;set;
		}
	}
}