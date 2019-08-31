<template>
  <div id="mainPage">
    <h5>收支科目維護</h5>
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
        <b-button variant="info" :disabled="!selected.length > 0">
          匯出資料
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
        <template slot="[stop_fg]" slot-scope="{ item }">
          <span v-if="item.stop_fg === `Y`">是</span>
          <span v-else>否</span>
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
    <b-modal ref="domModal" size="xl" title="科目維護" hide-footer>
      <subject-view :subjectData="subjectData"></subject-view>
    </b-modal>
  </div>
</template>

<script>
import { subjectModel } from '../DataModel/selectorModel.js';
import Selector from './CVue_Selector.vue';
import SubjectView from './CVue_SubjectView.vue';

export default {
  name: 'IncomeDataMaintain',
  components: {
    Selector,
    SubjectView,
  },
  props: {},
  data() {
    return {
      selectorModel: subjectModel,
      subjectData: {},

      fields: [],
      items: [],
      currentPage: 1,
      perPage: 20,
      selected: [],
      isBusy: false,
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
        this.subjectData = dataObj;
      } else {
        this.subjectData = {};
      }

      this.$refs.domModal.show();
    },
  },
  created() {
    this.fields = [
      { key: `selected`, label: `勾選` },
      { key: `code_no`, label: `科目代碼` },
      { key: `code_name`, label: `科目名稱` },
      { key: `upp_lim`, label: `金額上限` },
      { key: `low_lim`, label: `金額下限` },
      { key: `place`, label: `購買地點` },
      { key: `stop_fg`, label: `是否停用` },
      { key: `edit`, label: `` },
    ];
    this.items = [
      {
        code_no: 10101,
        code_name: `支付私人`,
        upp_lim: 700,
        low_lim: 200,
        place: 2,
        stop_fg: `Y`,
      },
      {
        code_no: 10102,
        code_name: `支付金融機關與企業政府`,
        upp_lim: 1700,
        low_lim: 100,
        place: 4,
        stop_fg: `N`,
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
