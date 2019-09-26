<template>
  <div id="mainPage">
    <h5>帳號維護</h5>
    <selector
      :filterModel="selectorModel"
      :isDetailed="true"
      @additem="addItem"
      @search="searchEvent"
    ></selector>

    <div id="tableData">
      <div class="w-25 d-inline-block justify-content-center">
        <label class="d-inline-block">每頁顯示筆數</label>
        <b-form-input
          v-model="perPage"
          type="number"
          size="sm"
          class="d-inline-block"
        ></b-form-input>
      </div>
      <b-table
        ref="domDatatable"
        :items="items"
        :fields="fields"
        :per-page="perPage"
        :current-page="currentPage"
        hover
        small
        head-variant="dark"
        class="my-2"
      >
        <template slot="[action]" slot-scope="data">
          <a href="javascript:;">刪除</a>
          <a href="javascript:;">編輯</a>
        </template>
        <span slot="[state]" slot-scope="data">
          {{ stateToString(data.item.state) }}
        </span>
      </b-table>
      <b-pagination
        v-model="currentPage"
        :total-rows="items.length"
        :per-page="perPage"
        size="sm"
        class="justify-content-center"
      ></b-pagination>
    </div>

    <b-modal ref="domModal" size="xl" hide-footer></b-modal>
  </div>
</template>

<script>
import { portState } from '../DataModel/dataModel.js';
import { checkinPortModel } from '../DataModel/selectorModel.js';
import Selector from './CVue_Selector.vue';

export default {
  /* eslint-disable no-undef, no-param-reassign, camelcase */
  name: 'CheckInManagement',
  components: {
    Selector,
  },
  props: {},
  data() {
    return {
      // for selector
      selectorModel: checkinPortModel,
      queryObject: {},

      // for table
      items: [],
      currentPage: 1,
      perPage: 20,
    };
  },
  methods: {
    // filter
    async searchEvent(filterObject) {
      this.queryObject = {};
      console.log(filterObject);
      const { loginman, port, reviewman } = filterObject;

      if (loginman.id.length > 0) {
        this.queryObject.ChechInNo = loginman.id;
      }

      if (reviewman.id.length > 0) {
        this.queryObject.ReviewNo = reviewman.id;
      }

      if (
        port.end.length !== 0 &&
        port.start.length !== 0 &&
        port.end >= port.start
      ) {
        this.queryObject.FamNoStart = port.start;
        this.queryObject.FamNoEnd = port.end;
      }

      await this.queryAccount(this.queryObject);
    },
    addItem() {},

    // ui show
    stateToString(state) {
      return portState[state];
    },

    // backend api
    async queryAccount(queryObject) {
      this.items = [];
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.famInfo,
        {
          Action: `READ`,
          ...queryObject,
        }
      );

      if (resObject.status !== this.mixinBackendErrorCode.success) {
        // do nothing
        return;
      }

      this.items = resObject.data;
    },
  },
  created() {},
  mounted() {},
  computed: {
    fields() {
      return [
        { key: `action`, label: `` },
        { key: `fam_no`, label: `戶號` },
        { key: `rec_user`, label: `登錄人員編號` },
        { key: `rec_name`, label: `登錄人員` },
        { key: `adi_user`, label: `審核人員編號` },
        { key: `adi_name`, label: `審核人員` },
        { key: `state`, label: `戶號狀態` },
      ];
    },
  },
  watch: {},
  /* eslint-disable no-undef, no-param-reassign, camelcase */
};
</script>

<style scoped>
#mainPage {
  text-align: center;
  margin: auto;
}
#tableData {
  width: 85%;
  margin: auto;
}
</style>
