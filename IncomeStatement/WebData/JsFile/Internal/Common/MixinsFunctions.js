import 'babel-polyfill';
import { fnDeepClone } from './Utility';

/* eslint-disable no-undef */
/* eslint-disable no-unused-vars */

const mixinFuncitons = {
  // go pages
  mixinToHomePage(pageLanguage) {
    const tempPageLanguage = pageLanguage || this.mixinGetCurrentLanguageCode();
    const currentURLInfo = new URL(window.location.origin);
    window.location = `${
      currentURLInfo.href
    }${webpackDashboardName}/Language/${tempPageLanguage}`;
  },
  mixinToLoginPage(pageLanguage) {
    const tempPageLanguage = pageLanguage || this.mixinGetCurrentLanguageCode();
    const currentURLInfo = new URL(window.location.origin);
    window.location = `${
      currentURLInfo.href
    }${webpackDashboardName}/Login.html?Language=${tempPageLanguage}&ToPrevious=1`;
  },

  // check status
  mixinIsBackendStatusGood(responseStatus, isAutoDirect = false) {
    let isSuccess = false;
    let fnGoPage;

    switch (responseStatus) {
      case backendErrorCode.success: {
        isSuccess = true;
        break;
      }
      case backendErrorCode.error: {
        isSuccess = false;
        break;
      }
      case backendErrorCode.authenticationError: {
        fnGoPage = this.mixinToLoginPage;
        isSuccess = false;
        break;
      }
      case backendErrorCode.noAuthorization: {
        fnGoPage = this.mixinToNoPermissionPage;
        isSuccess = false;
        break;
      }
      default: {
        isSuccess = false;
        break;
      }
    }

    if (isAutoDirect && fnGoPage) {
      fnGoPage();
    }

    return isSuccess;
  },

  // get cookie , if not exist return null
  mixinGetCookie(key) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${key}=`);
    if (parts.length === 2) {
      return parts
        .pop()
        .split(';')
        .shift();
    }
    return '';
  },
  mixinEraseCookie(key) {
    document.cookie = `${key}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
  },
  mixinGetCurrentLanguageCode() {
    const parsedUrl = new URL(window.location.href);
    let languageCode = parsedUrl.searchParams.get('Language');
    if (!languageCode) {
      languageCode = parsedUrl.pathname;
      if (languageCode) {
        const tempArray = languageCode.split('/');
        if (tempArray[2] === 'Language') {
          languageCode = tempArray[3].replace('-', '_');
        } else {
          // get
          languageCode = navigator.language;
          languageCode = languageCode.replace('-', '_');
        }
      }
    }

    return languageCode;
  },

  // useful api function
  async mixinLogoutProcess() {
    // call api
    const resObj = await this.mixinCallBackService(
      this.mixinBackendService.Logout,
      null
    );

    if (resObj.status === backendErrorCode.success) {
      const languageCode = this.mixinGetCurrentLanguageCode();
      this.mixinToLoginPage(languageCode);
    }
  },
  async mixinAccountStatus() {
    // call api
    const resObj = await this.mixinCallBackService(
      this.mixinBackendService.AccountStatus,
      null
    );

    return {
      isErrorAuth: resObj.status === backendErrorCode.authenticationError,
      isErrorNoAuth: resObj.status === backendErrorCode.noAuthorization,
      isSuccess: resObj.status === backendErrorCode.success,
    };
  },

  // api list
  async mixinCallBackService(
    backendServiceName,
    payloadObject = null,
    isThrowException = false
  ) {
    // call api
    const _this = this;
    const backendUrl = `/${webpackDashboardName}/WebData/Server_Code/${backendServiceName}`;
    const errormessage = `ERROR API, ${backendServiceName}`;
    const fnResultHandler = (isSuccess, message, resultObj) => {
      // fail
      if (isSuccess === false && isThrowException) {
        console.error(message);
        return Promise.reject(new Error(message));
      }

      return resultObj;
    };
    let tempPayload = {};
    if (payloadObject !== null) {
      tempPayload = fnDeepClone(payloadObject);
      tempPayload.UserId = tempPayload.UserId || this.mixinGetCookie(`UserID`);
    }

    const additionalHeader = {};
    if (webpackIsAWS) {
      // get token, append header
      additionalHeader.IdToken = window.localStorage.getItem(`IdToken`);
      additionalHeader.AccessToken = window.localStorage.getItem(`AccessToken`);
      additionalHeader.RefreshToken = window.localStorage.getItem(
        `RefreshToken`
      );
    }

    return $.ajax({
      url: backendUrl,
      method: `POST`,
      headers: additionalHeader,
      dataType: `json`,
      data: tempPayload,
      cache: false,
      success(resObject) {
        const isBNDGood = _this.mixinIsBackendStatusGood(
          resObject.status,
          isThrowException
        );
        return fnResultHandler(
          isBNDGood,
          isBNDGood ? null : errormessage,
          resObject
        );
      },
      error(result) {
        return fnResultHandler(false, errormessage);
      },
    });
  },
};
export default mixinFuncitons;
