const CommonDataModel = {
  mixinBackendErrorCode: {
    success: 1001,
    error: 1000,
    authenticationError: 1002,
    noAuthorization: 1003,
  },
  mixinApiErrorMsg: {
    success: `OK`,
  },
  mixinBackendService: {
    login: `S_LoginChecker.aspx`,
    checkStatus: `S_CheckAccountStatus.aspx`,
    logout: `S_Logout.aspx`,
    incomeData: `S_IncomeStateData.aspx`,
    subjectData: `S_SubjectData.aspx`,
    paramInfo: `S_ParamInfo.aspx`,
    detatiledData: `S_DetailedIncomeData.aspx`,
    myFamNo: `S_MyFamInfo.aspx`,
    familyData: `S_FamilyData.aspx`,
  },
};

export default CommonDataModel;
