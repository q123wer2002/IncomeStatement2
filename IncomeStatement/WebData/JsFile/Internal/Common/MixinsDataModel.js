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
  mixinBackendService: {},
};

export default CommonDataModel;
