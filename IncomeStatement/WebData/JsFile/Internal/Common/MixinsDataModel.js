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
  },
};

export default CommonDataModel;
