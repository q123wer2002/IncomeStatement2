<template>
  <div id="mainPage">
    <h5>收支資料檢誤</h5>
    <selector
      :isDataCheckBtn="true"
      :filterModel="selectorModel"
      @search="searchEvent"
      @checkData="startCheck"
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
          <a href="javascript:;" @click="openDeleteDialog(data.item)">
            <img width="24px" :src="$options.imgSrc.trash" />
          </a>
          <a href="javascript:;" @click="editItem(data.item)">
            <img width="24px" :src="$options.imgSrc.edit" />
          </a>
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
  </div>
</template>

<script>
import { checkNo } from '../DataModel/dataModel.js';
import { dataCheckerModel } from '../DataModel/selectorModel.js';
import Selector from './CVue_Selector.vue';

export default {
  /* eslint-disable no-undef, no-param-reassign */
  name: 'DataChecker',
  components: {
    Selector,
  },
  props: {},
  data() {
    return {
      selectorModel: dataCheckerModel,
      queryObject: {},

      // for table
      items: [],
      currentPage: 1,
      perPage: 20,
    };
  },
  methods: {
    async searchEvent(filterObject) {
      const { checkType, checker, port, date } = filterObject;
      this.queryObject = {};

      // add date
      if (date.year !== 0 && date.month !== 0) {
        this.queryObject.Year = date.year;
        this.queryObject.Month = date.month;
      }

      // add port
      if (
        port.end.length !== 0 &&
        port.start.length !== 0 &&
        port.end >= port.start
      ) {
        this.queryObject.FamNoStart = port.start;
        this.queryObject.FamNoEnd = port.end;
      }

      if (checker && checker.id.length !== 0) {
        this.queryObject.CheckNo = checker.id;
      }

      if (checkType.code > 0) {
        this.queryObject.CheckType = checkType.code;
      }

      await this.queryCheckData(this.queryObject);
    },
    async queryCheckData(queryObject) {
      this.items = [];
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.dataChecker,
        {
          Action: `READ`,
          ...queryObject,
        }
      );

      if (resObject.status !== this.mixinBackendErrorCode.success) {
        return;
      }

      this.items = resObject.data || [];
    },
    async startCheck() {
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.dataChecker,
        {
          Action: `CHECK`,
          ...this.queryObject,
        }
      );

      if (
        resObject.status !== this.mixinBackendErrorCode.success ||
        resObject.data === false
      ) {
        alert(`資料檢誤失敗`);
        return;
      }

      alert(`資料檢誤成功`);
    },
  },
  created() {},
  mounted() {},
  computed: {
    fields() {
      return [
        { key: `ie_year`, label: `年` },
        { key: `ie_mon`, label: `月` },
        { key: `ie_day`, label: `日` },
        { key: `fam_no`, label: `戶號` },
        { key: `code_no`, label: `科目代碼` },
        { key: `code_name`, label: `科目名稱` },
        { key: `code_amt`, label: `金額` },
        { key: `upp_lim`, label: `金額上限` },
        { key: `low_lim`, label: `金額下限` },
        { key: `chk_no`, label: `檢誤代碼` },
        { key: `chk_desc`, label: `說明` },
      ];
    },
  },
  watch: {},
  /* eslint-disable no-undef, no-param-reassign */
};
</script>

<style scoped>
#mainPage {
  text-align: center;
}
#tableData {
  width: 85%;
  margin: auto;
}
</style>
