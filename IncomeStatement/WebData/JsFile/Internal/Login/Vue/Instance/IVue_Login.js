import Vue from 'vue';
import BootstrapVue from 'bootstrap-vue';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';
import vueStore from '../Vuex/Vuex_GlobalStore';
import '../Mixins/Vue_GlobalMixins.js';

Vue.use(BootstrapVue);

function IVueInitalLogin() {
  const _this = this;
  let _Instance = null;
  this.Intital = () => {
    _Instance = new Vue({
      /* eslint-disable no-undef */
      store: vueStore,
      el: '#vue_instance',
      data: {
        userInput: {
          account: ``,
          password: ``,
        },
        loginErrorMsg: ``,
      },
      watch: {},
      computed: {},
      methods: {
        async login() {
          // check is empty
          const { account, password } = this.userInput;
          if (account.length === 0 || password.length === 0) {
            this.loginErrorMsg = `帳號密碼不能為空`;
            return;
          }

          const resObject = await this.mixinCallBackService(
            this.mixinBackendService.login,
            { Username: account, Password: password },
            false
          );

          if (resObject.status === this.mixinBackendErrorCode.success) {
            // re-direct to home page
            this.mixinToHomePage();
          }
        },
        async checkAccountStatus() {
          const { isSuccess } = await this.mixinAccountStatus();

          // direct to login page
          if (isSuccess) {
            this.mixinToHomePage();
          }
        },
      },
      created() {},
      async mounted() {
        // check account status
        await this.checkAccountStatus();
      },
      /* eslint-disable no-undef */
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
  window.IncomeStatement.js_Vue_Instance.IVue_Login = new IVueInitalLogin();
  /* eslint-disable no-param-reassign */
})(window);
