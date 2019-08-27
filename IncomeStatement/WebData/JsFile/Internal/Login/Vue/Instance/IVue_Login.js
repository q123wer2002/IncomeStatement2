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
      },
      watch: {},
      computed: {},
      methods: {},
      created() {},
      mounted() {},
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
