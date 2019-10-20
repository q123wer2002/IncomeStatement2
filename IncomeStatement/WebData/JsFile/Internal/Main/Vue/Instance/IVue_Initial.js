import Vue from 'vue';
import { mapActions } from 'vuex';
import BootstrapVue from 'bootstrap-vue';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';
import vueStore from '../Vuex/Vuex_GlobalStore';
import '../Mixins/Vue_GlobalMixins';

import { accountRole } from '../DataModel/dataModel.js';
import IncomeDataMaintain from '../Components/CVue_IncomeDataMaintain.vue';
import SubjectMaintain from '../Components/CVue_SubjectMaintain.vue';
import DetailedMaintain from '../Components/CVue_DetailedMaintain.vue';
import DataChecker from '../Components/CVue_DataChecker.vue';
import PortCardMaintain from '../Components/CVue_PortCardMaintain.vue';
import ChangePassword from '../Components/CVue_ChangePassword.vue';
import AccountManagement from '../Components/CVue_AccountManagement.vue';
import CheckInManagement from '../Components/CVue_CheckInPortManagement.vue';

Vue.use(BootstrapVue);
function IVueInitialCreator() {
  const _this = this;
  let _Instance = null;
  this.Intital = () => {
    _Instance = new Vue({
      /* eslint-disable no-undef, no-param-reassign */
      imgSrc: {
        dropDown: `/${webpackDashboardName}/WebData/Picture/icon/material-io/sharp_arrow_drop_down_black_48dp.png`,
      },

      // component
      store: vueStore,
      el: '#vue-instance',
      components: {
        IncomeDataMaintain,
        SubjectMaintain,
        DetailedMaintain,
        DataChecker,
        PortCardMaintain,
        ChangePassword,
        AccountManagement,
        CheckInManagement,
      },
      data: {
        // ui
        menu: [
          {
            key: `maintainPage`,
            text: `收支維護`,
            isShowChild: false,
          },
          {
            key: `reportPage`,
            text: `報表`,
            isShowChild: false,
          },
          {
            key: `systemPage`,
            text: `系統維護`,
            isShowChild: false,
          },
        ],
        subMenu: {
          maintainPage: [
            {
              key: `IncomeDataMaintain`,
              text: `收支資料維護`,
              isSupport: true,
              supportRole: [`A`, `B`, `C`],
            },
            {
              key: `DetailedMaintain`,
              text: `收支明細維護`,
              isSupport: true,
              supportRole: [`A`, `B`, `C`],
            },
            {
              key: `SubjectMaintain`,
              text: `收支科目維護`,
              isSupport: true,
              supportRole: [`A`, `C`],
            },
            {
              key: `PortCardMaintain`,
              text: `戶口組成資料`,
              isSupport: true,
              supportRole: [`A`, `B`, `C`],
            },
            {
              key: `DataChecker`,
              text: `收支資料檢誤`,
              isSupport: true,
              supportRole: [`A`, `B`, `C`],
            },
          ],
          reportPage: [
            {
              key: `familyIncomeResult`,
              text: `家庭收支記帳調查結果`,
              isSupport: false,
              supportRole: [`A`, `B`, `C`, `D`],
            },
            {
              key: `familyIncomeReport`,
              text: `家庭收支記帳調查統計月報`,
              isSupport: false,
              supportRole: [`A`, `B`, `C`, `D`],
            },
          ],
          systemPage: [
            {
              key: `AccountManagement`,
              text: `帳號管理`,
              isSupport: true,
              supportRole: [`A`],
            },
            {
              key: `ChangePassword`,
              text: `密碼變更`,
              isSupport: true,
              supportRole: [`A`, `B`, `C`, `D`],
            },
            {
              key: `CheckInManagement`,
              text: `登錄戶號管理`,
              isSupport: true,
              supportRole: [`A`],
            },
          ],
        },
        currentPageKey: ``,

        // user info
        userInfo: {},
      },
      methods: {
        ...mapActions([`initialSubject`, `initialParam`]),
        async checkAccountStatus() {
          const { isErrorAuth } = await this.mixinAccountStatus();

          // direct to login page
          if (isErrorAuth) {
            this.mixinToLoginPage();
          }
        },
        async logout() {
          await this.mixinLogoutProcess();
        },
      },
      updated() {},
      computed: {
        supportSubMenu() {
          return pageKey => {
            const userRole = this.mixinGetCookie(`UserRole`);
            return this.subMenu[pageKey].filter(obj =>
              obj.supportRole.includes(userRole)
            );
          };
        },
        supportedComponent() {
          const userRole = this.mixinGetCookie(`UserRole`);
          const supportedPages = Object.keys(this.subMenu).reduce(
            (tempAry, key) => {
              this.subMenu[key].forEach(obj => {
                if (obj.isSupport && obj.supportRole.includes(userRole)) {
                  tempAry.push(obj.key);
                }
              });

              return tempAry;
            },
            []
          );

          return [...supportedPages, ``];
        },
      },
      created() {},
      async mounted() {
        // check account status
        await this.checkAccountStatus();

        // init param
        await this.initialParam();

        // init subject
        await this.initialSubject({
          CodeNo: -1,
        });

        // get user info
        this.userInfo = {
          id: this.mixinGetCookie('UserID'),
          name: this.mixinGetCookie('UserName'),
          role: accountRole[this.mixinGetCookie('UserRole')],
        };
      },
      watch: {},
      beforeDestroy() {},
    });
  };

  this._getColsure = () => {
    return { _this, _Instance };
  };
}

(function initIVue(window) {
  /* eslint-disable no-param-reassign */
  window.IncomeStatement = window.IncomeStatement || {};
  window.IncomeStatement.js_Vue_Instance =
    window.IncomeStatement.js_Vue_Instance || {};
  window.IncomeStatement.js_Vue_Instance.IVue_Initial = new IVueInitialCreator();
  /* eslint-disable no-param-reassign */
})(window);
