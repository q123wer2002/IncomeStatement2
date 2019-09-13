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
          class="d-inline-block"
        ></b-form-input>
      </div>
      <b-button
        variant="info"
        :disabled="!enabledConfirm"
        @click="confirm(`2`)"
        class="my-3 mx-1 float-sm-right"
        size="sm"
      >
        登錄確認
      </b-button>
      <b-button-group class="my-3 float-sm-right" size="sm">
        <b-button
          variant="info"
          :disabled="!enabledReview"
          @click="confirm(`3`)"
        >
          審核確認
        </b-button>
        <b-button
          variant="info"
          :disabled="!isSelectedItems"
          @click="confirm(`1`)"
        >
          狀態回復
        </b-button>
        <b-button variant="info" disabled>
          收支匯入
        </b-button>
        <b-button
          variant="info"
          :disabled="!isSelectedItems"
          @click="exportCSV"
        >
          收支匯出
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
        <template slot="[selected]" slot-scope="{ rowSelected, index }">
          <b-form-checkbox
            v-model="rowSelected"
            @change="selectOneItem(index)"
          ></b-form-checkbox>
        </template>
        <template slot="[state]" slot-scope="{ item }">
          <span>{{ statusToString(item.state) }}</span>
        </template>
        <template slot="[btnIncomeDetail]" slot-scope="{ item }">
          <a href="javascript:;" @click="showDetails(item.fam_no)">明細</a>
        </template>
      </b-table>

      <b-pagination
        v-model="currentPage"
        :total-rows="items.length"
        :per-page="perPage"
        size="sm"
        class="justify-content-center"
      ></b-pagination>

      <b-modal ref="domModal" size="xl" hide-footer>
        <detailed-maintain
          :isShowFilter="false"
          :inputedQueryObj="detailedQueryObj"
        ></detailed-maintain>
      </b-modal>
    </div>
  </div>
</template>

<script>
import { incomeDataModel } from '../DataModel/selectorModel.js';
import Selector from './CVue_Selector.vue';
import { statusMapToString } from '../DataModel/dataModel.js';
import DetailedMaintain from './CVue_DetailedMaintain.vue';

export default {
  /* eslint-disable no-undef, no-param-reassign, camelcase */
  name: 'IncomeDataMaintain',
  components: {
    Selector,
    DetailedMaintain,
  },
  props: {},
  data() {
    return {
      selectorModel: incomeDataModel,
      detailedQueryObj: {},
      queryObject: {},
      isSelectAll: false,

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
      const { date, loginman, port, status, reviewman } = filterObject;
      this.queryObject = {};

      // add date
      if (date.year !== 0 && date.month !== 0) {
        this.queryObject.Year = date.year;
        this.queryObject.Month = date.month;
      }

      // add loginman
      if (loginman.id > 0) {
        this.queryObject.RecNo = loginman.id;
      }

      // add port
      if (port.end > port.start) {
        this.queryObject.FamNoStart = port.start;
        this.queryObject.FamNoEnd = port.end;
      }

      if (reviewman && reviewman.id.length !== 0) {
        this.queryObject.AdiUser = reviewman.id;
      }

      if (status.code > 0) {
        this.queryObject.State = status.code;
      }

      await this.queryIncomeStateData(this.queryObject);
    },
    onRowSelected(items) {
      this.selected = items;
    },
    selectAllRows() {
      this.$refs.domDatatable.selectAllRows();
      this.isSelectAll = true;
    },
    clearSelected() {
      this.$refs.domDatatable.clearSelected();
      this.isSelectAll = false;
    },
    selectOneItem(index) {
      const isSelected = this.$refs.domDatatable.isRowSelected(index);
      if (isSelected) {
        this.$refs.domDatatable.unselectRow(index);
        return;
      }

      this.$refs.domDatatable.selectRow(index);
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
        this.items = resObject.data || [];
      }
      this.isBusy = false;
      return resObject;
    },
    async confirm(confirmType) {
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
          ConfirmType: confirmType,
        }
      );
      if (resObject.status === this.mixinBackendErrorCode.success) {
        this.selected.forEach(obj => {
          obj.state = confirmType;
        });

        this.clearSelected();
      }
    },
    exportCSV() {
      // create csv data
      const filedName = [
        `類別`,
        `登打日期`,
        `記帳帳戶號`,
        `科目代碼`,
        `設算別`,
        `未使用欄位`,
        `未使用欄位`,
        `戶內人數`,
        `就業人數`,
        `本業：行業編號`,
        `本業：職業編號`,
        `金額`,
        `流水號`,
        `購買地`,
      ];

      let exportData = this.selected;
      if (this.isSelectAll) {
        exportData = this.items;
      }
      console.log(exportData);
      const csvData = [
        filedName,
        ...exportData.map(obj => {
          const {
            ie_year,
            ie_mon,
            fam_no,
            fam_cnt,
            job_cnt,
            job_type_no,
            job_no,
          } = obj;
          return [
            `1`,
            `${ie_year}${ie_mon}`,
            fam_no,
            `code_no`,
            `0`,
            ` `,
            `0`,
            fam_cnt,
            job_cnt,
            job_type_no,
            job_no,
            `code_amt`,
            `${fam_no}${ie_year}${ie_mon}`,
            `place`,
          ];
        }),
      ];
      const csvDataString = csvData.map(col => col.join(`,`)).join('\n');
      const encodedUri = URL.createObjectURL(
        new Blob([`\uFEFF${csvDataString}`], {
          type: `text/csv;charset=utf-8;`,
        })
      );

      // create link
      const link = document.createElement(`a`);
      link.setAttribute(`href`, encodedUri);
      link.setAttribute(`download`, `incomestatement.csv`);
      document.body.appendChild(link);
      link.click();

      // clear
      this.clearSelected();
    },
    showDetails(famNo) {
      const { Year, Month } = this.queryObject;
      this.detailedQueryObj = {
        Year,
        Month,
        FamNo: famNo,
      };

      this.$refs.domModal.show();
    },
  },
  created() {
    this.fields = [
      { key: `selected`, label: `勾選` },
      { key: `ie_year`, label: `年` },
      { key: `ie_mon`, label: `月` },
      { key: `rec_name`, label: `登錄人員`, sortable: true },
      { key: `adi_name`, label: `審核人員`, sortable: true },
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
    enabledConfirm() {
      if (this.selected.length === 0) {
        return false;
      }

      return this.selected.every(obj => obj.state === `1`);
    },
    enabledReview() {
      if (this.selected.length === 0) {
        return false;
      }

      return this.selected.every(obj => obj.state === `2`);
    },
    isSelectedItems() {
      if (this.selected.length === 0) {
        return false;
      }

      return this.selected.length > 0;
    },
  },
  watch: {
    currentPage: {
      handler() {
        this.$nextTick(() => {
          if (this.isSelectAll) {
            this.selectAllRows();
          }
        });
      },
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
