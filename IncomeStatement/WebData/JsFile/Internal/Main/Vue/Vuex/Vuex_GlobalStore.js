import Vue from 'vue';
import Vuex from 'vuex';
import UtilFn from '../../../Common/MixinsFunctions.js';
import UtilData from '../../../Common/MixinsDataModel.js';

Vue.use(Vuex); // regist Vuex to Vue
/* eslint-disable no-undef, no-param-reassign, camelcase */
const vueStore = new Vuex.Store({
  modules: {},
  state: {
    subjectArray: [],
    paramArray: [],
  },
  mutations: {
    fnUpdateSubject(state, tempArry) {
      for (let i = 0; i < tempArry.length; i++) {
        const subIdx = state.subjectArray.findIndex(
          obj => obj.code_no === tempArry[i].code_no
        );

        if (subIdx === -1) {
          Vue.set(state.subjectArray, state.subjectArray.length, tempArry[i]);
          continue;
        }

        Vue.set(state.subjectArray, subIdx, tempArry[i]);
      }
    },
    fnDeleteSubject(state, deleteAry) {
      for (let i = 0; i < deleteAry.length; i++) {
        const subIdx = state.subjectArray.findIndex(
          obj => obj.code_no === deleteAry[i].code_no
        );

        if (subIdx === -1) {
          continue;
        }

        Vue.delete(state.subjectArray, subIdx);
      }
    },
    fnUpdateParam(state, placeArray) {
      state.paramArray = placeArray;
    },
  },
  actions: {
    // for subject
    async initialSubject(context, queryObject) {
      const resObject = await UtilFn.mixinCallBackService(
        UtilData.mixinBackendService.subjectData,
        {
          Action: `READ`,
          ...queryObject,
        }
      );

      if (resObject.status === UtilData.mixinBackendErrorCode.success) {
        context.commit(`fnUpdateSubject`, resObject.data || []);
      }
    },
    async saveSubject(context, subjectObject) {
      const resObject = await UtilFn.mixinCallBackService(
        UtilData.mixinBackendService.subjectData,
        {
          Action: `WRITE`,
          Subject: JSON.stringify(subjectObject),
        }
      );

      if (resObject.status === UtilData.mixinBackendErrorCode.success) {
        context.commit(`fnUpdateSubject`, [subjectObject]);
      }
    },
    async deleteSubjects(context, subjectArray) {
      const resObject = await UtilFn.mixinCallBackService(
        UtilData.mixinBackendService.subjectData,
        {
          Action: `DELETE`,
          SubjectArray: JSON.stringify(subjectArray),
        }
      );

      if (resObject.status === UtilData.mixinBackendErrorCode.success) {
        context.commit(`fnDeleteSubject`, subjectArray);
      }
    },

    // for param
    async initialParam(context) {
      const resObject = await UtilFn.mixinCallBackService(
        UtilData.mixinBackendService.paramInfo
      );

      if (resObject.status === UtilData.mixinBackendErrorCode.success) {
        context.commit(`fnUpdateParam`, resObject.data || []);
      }
    },
  },
  getters: {},
});
/* eslint-disable no-undef, no-param-reassign, camelcase */
export default vueStore;
