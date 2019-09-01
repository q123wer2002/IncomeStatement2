<template>
  <div id="mainPage">
    <h5>收支資料維護</h5>
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
          :disabled="!selected.length > 0"
          @click="confirm"
        >
          登入完成確認
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
        <div slot="table-busy" class="text-center text-danger my-2">
          <b-spinner class="align-middle"></b-spinner>
          <strong>Loading...</strong>
        </div>
        <template slot="[selected]" slot-scope="{ rowSelected }">
          <b-form-checkbox
            v-model="rowSelected"
            button-variant="info"
          ></b-form-checkbox>
        </template>
        <template slot="[state]" slot-scope="{ item }">
          <span>{{ statusToString(item.state) }}</span>
        </template>
        <template slot="[btnIncomeDetail]" slot-scope="{ item }">
          <a href="javascript:;">明細</a>
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
  </div>
</template>

<script>
import { incomeDataModel } from '../DataModel/selectorModel.js';
import Selector from './CVue_Selector.vue';
import { statusMapToString } from '../DataModel/dataModel.js';

export default {
  /* eslint-disable no-undef, no-param-reassign, camelcase */
  name: 'IncomeDataMaintain',
  components: {
    Selector,
  },
  props: {},
  data() {
    return {
      selectorModel: incomeDataModel,

      fields: [],
      items: [],
      currentPage: 1,
      perPage: 20,
      selected: [],
      isBusy: false,
    };
  },
  methods: {
    async searchEvent(filterObject) {
      const { date, loginman, port } = filterObject;
      const queryObject = {};

      // add date
      if (date.year !== 0 && date.month !== 0) {
        queryObject.Year = date.year;
        queryObject.Month = date.month;
      }

      // add loginman
      if (loginman.id > 0) {
        queryObject.RecNo = loginman.id;
      }

      // add port
      if (port.end > port.start) {
        queryObject.FamNoStart = port.start;
        queryObject.FamNoEnd = port.end;
      }

      await this.queryIncomeStateData(queryObject);
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
    async queryIncomeStateData(queryObject) {
      this.isBusy = true;
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.incomeData,
        {
          Action: `READ`,
          ...queryObject,
        }
      );

      if (resObject.status === this.mixinBackendErrorCode.success) {
        this.items = resObject.data;
      }
      this.isBusy = false;
      return resObject;
    },
    async confirm() {
      const paramList = this.selected.map(famObj => {
        const { ie_year, ie_mon, fam_no } = famObj;
        return {
          ie_year,
          ie_mon,
          fam_no,
        };
      });

      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.incomeData,
        {
          Action: `CONFIRM`,
          ConfirmList: JSON.stringify(paramList),
        }
      );
      if (resObject.status === this.mixinBackendErrorCode.success) {
        this.selected.forEach(obj => {
          obj.state = 2;
        });

        this.clearSelected();
      }
    },
  },
  created() {
    this.fields = [
      { key: `selected`, label: `勾選` },
      { key: `ie_year`, label: `年` },
      { key: `ie_mon`, label: `月` },
      { key: `rec_name`, label: `登入人員`, sortable: true },
      { key: `fam_no`, label: `戶號`, sortable: true },
      { key: `state`, label: `資料狀態`, sortable: true },
      { key: `btnIncomeDetail`, label: `收支資料` },
    ];
  },
  async mounted() {
    // default load last month data
    const dtcurrent = new Date();
    dtcurrent.setMonth(dtcurrent.getMonth() - 1);
    await this.queryIncomeStateData({
      Year: dtcurrent.getFullYear() - 1911,
      Month: dtcurrent.getMonth() + 1,
    });
  },
  computed: {
    statusToString() {
      return statusCode => {
        return statusMapToString[statusCode];
      };
    },
  },
  /* eslint-disable no-undef, no-param-reassign, camelcase */
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
