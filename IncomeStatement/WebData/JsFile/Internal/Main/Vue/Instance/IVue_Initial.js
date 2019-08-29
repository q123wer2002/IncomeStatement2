import Vue from 'vue';
import BootstrapVue from 'bootstrap-vue';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';
import vueStore from '../Vuex/Vuex_GlobalStore';
import '../Mixins/Vue_GlobalMixins';

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
      components: {},
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
              key: `dataMaintain`,
              text: `收支資料維護`,
            },
            {
              key: `detailMaintain`,
              text: `收支明細維護`,
            },
            {
              key: `subjectMaintain`,
              text: `收支科目維護`,
            },
            {
              key: `cardMaintain`,
              text: `戶口組成卡維護`,
            },
            {
              key: `dataChecker`,
              text: `收支資料檢誤`,
            },
          ],
          reportPage: [
            {
              key: `familyIncomeResult`,
              text: `家庭收支記帳調查結果`,
            },
            {
              key: `familyIncomeReport`,
              text: `家庭收支記帳調查統計月報`,
            },
          ],
          systemPage: [
            {
              key: `accountManagement`,
              text: `帳號管理`,
            },
            {
              key: `changePassword`,
              text: `密碼變更`,
            },
            {
              key: `incomePortManagement`,
              text: `收支戶號管理`,
            },
          ],
        },
      },
      methods: {
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
      computed: {},
      created() {},
      async mounted() {
        // check account status
        await this.checkAccountStatus();
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
