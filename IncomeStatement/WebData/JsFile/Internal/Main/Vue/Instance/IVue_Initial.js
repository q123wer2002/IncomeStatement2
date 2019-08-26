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

      // component
      store: vueStore,
      el: '#vue-instance',
      components: {},
      data: {},
      methods: {},
      updated() {},
      computed: {},
      created() {},
      mounted() {},
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
