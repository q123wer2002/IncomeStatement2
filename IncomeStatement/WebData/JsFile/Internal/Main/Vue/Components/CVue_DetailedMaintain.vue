<template>
  <div id="mainPage">
    <h5>收支明細維護</h5>
    <selector :filterModel="selectorModel" @search="searchEvent"></selector>

    <!-- filtered data -->
    <div id="tableData">
      <b-button-group class="my-3 float-sm-left" size="sm">
        <b-button @click="selectAllRows">全選</b-button>
        <b-button @click="clearSelected">取消全選</b-button>
      </b-button-group>
      <div class="w-25 d-inline-block justify-content-center">
        <label class="d-inline-block">每頁顯示筆數</label>
        <b-form-input
          v-model="perPage"
          type="number"
          size="sm"
          label="1234"
          class="d-inline-block"
        ></b-form-input>
      </div>
      <b-button-group class="my-3 float-sm-right" size="sm">
        <b-button
          variant="info"
          :disabled="selected.length > 0"
          @click="openDetailedView()"
        >
          新增
        </b-button>
        <b-button variant="info" :disabled="!selected.length > 0">
          刪除
        </b-button>
      </b-button-group>

      <b-table
        ref="domDatatable"
        :busy="isBusy"
        :items="items"
        :fields="fields"
        :per-page="perPage"
        :current-page="currentPage"
        selectable
        select-mode="multi"
        selected-variant="info"
        @row-selected="onRowSelected"
        hover
        small
        head-variant="dark"
      >
        <template slot="[selected]" slot-scope="{ rowSelected }">
          <b-form-checkbox
            v-model="rowSelected"
            button-variant="info"
          ></b-form-checkbox>
        </template>
        <template slot="[edit]" slot-scope="{ item }">
          <a href="javascript:;" @click="openDetailedView(item)">編輯</a>
        </template>
      </b-table>

      <b-pagination
        v-model="currentPage"
        :total-rows="items.length"
        :per-page="perPage"
        size="sm"
        class="justify-content-center"
      ></b-pagination>
    </div>

    <!-- add data -->
    <b-modal ref="domModal" size="xl" title="明細維護" hide-footer>
      <detailed-view :data="detailedData"></detailed-view>
    </b-modal>
  </div>
</template>

<script>
import { detailedModel } from '../DataModel/selectorModel.js';
import Selector from './CVue_Selector.vue';
import DetailedView from './CVue_DetailedView.vue';

export default {
  name: 'DetailedMaintain',
  components: {
    Selector,
    DetailedView,
  },
  props: {},
  data() {
    return {
      // for selector
      selectorModel: detailedModel,

      // for table
      fields: [],
      items: [],
      currentPage: 1,
      perPage: 20,
      selected: [],
      isBusy: false,

      // for edit, and new one
      detailedData: [],
    };
  },
  methods: {
    searchEvent(message) {
      console.log(message);
    },
    onRowSelected(items) {
      this.selected = items;
    },
    selectAllRows() {
      this.$refs.domDatatable.selectAllRows();
    },
    clearSelected() {
      this.$refs.domDatatable.clearSelected();
    },
    openDetailedView(dataObj) {
      if (dataObj) {
        this.detailedData = this.items.filter(
          item => item.ie_day === dataObj.ie_day
        );
      } else {
        this.detailedData = [
          {
            ie_year: 2018,
            ie_month: 1,
            fam_no: 123456,
          },
        ];
      }

      this.$refs.domModal.show();
    },
    onDetailedDataChanged(detailedObj) {
      console.log(detailedObj);
    },
  },
  created() {
    this.fields = [
      { key: `selected`, label: `勾選` },
      { key: `ie_day`, label: `日期` },
      { key: `exp_amt`, label: `支出合計` },
      { key: `place`, label: `購買地點` },
      { key: `code_amt`, label: `金額` },
      { key: `code_no`, label: `科目代碼` },
      { key: `code_name`, label: `科目名稱` },
      { key: `edit`, label: `` },
    ];
    this.items = [
      {
        ie_year: 2018,
        ie_month: 1,
        ie_day: 1,
        exp_amt: 300,
        place: 5,
        code_amt: 300,
        code_no: 32101,
        code_name: `水費`,
        fam_no: 123456,
      },
      {
        ie_year: 2018,
        ie_month: 1,
        ie_day: 1,
        exp_amt: 500,
        place: 8,
        code_amt: 200,
        code_no: 92101,
        code_name: `中式米食`,
        fam_no: 123456,
      },
    ];
  },
  mounted() {},
  computed: {},
};
</script>

<style scoped>
#mainPage {
  text-align: center;
}
#tableData {
  text-align: center;
  width: 85%;
  margin: auto;
}
</style>
