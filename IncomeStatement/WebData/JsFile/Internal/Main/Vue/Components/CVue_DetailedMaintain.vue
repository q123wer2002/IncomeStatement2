<template>
  <div id="mainPage">
    <h5>收支明細維護</h5>
    <selector
      v-if="isShowFilter"
      :filterModel="selectorModel"
      @search="searchEvent"
    ></selector>

    <!-- filtered data -->
    <div id="tableData" v-if="items.length > 0">
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
        <b-button
          variant="info"
          :disabled="!selected.length > 0"
          @click="deleteItems"
        >
          刪除
        </b-button>
      </b-button-group>

      <div>
        <div class="m-50 d-inline-block mx-2 my-2">
          <span>資料檢查</span>
          <b-form-select :options="errorDaysOpts"></b-form-select>
        </div>
        <div class="m-50 d-inline-block mx-2 my-2">
          <span>無支出日期</span>
          <b-form-input v-model="noPaymentDays" disabled></b-form-input>
        </div>
      </div>

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
        <div slot="empty" class="text-center text-danger my-2">
          <strong>資料不存在</strong>
        </div>
        <template slot="[selected]" slot-scope="{ rowSelected }">
          <b-form-checkbox
            v-model="rowSelected"
            button-variant="info"
          ></b-form-checkbox>
        </template>
        <template slot="[place]" slot-scope="{ item }">
          <span>{{ placeName(item.place) }}</span>
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

    <span v-else>{{ hintText }}</span>

    <!-- add data -->
    <b-modal ref="domModal" size="xl" title="明細維護" hide-footer>
      <detailed-view
        :queryObject="addQueryObject"
        :data="detailedData"
        @save="onSaveEvent"
      ></detailed-view>
    </b-modal>
  </div>
</template>

<script>
import { mapState } from 'vuex';
import { detailedModel } from '../DataModel/selectorModel.js';
import Selector from './CVue_Selector.vue';
import DetailedView from './CVue_DetailedView.vue';

export default {
  /* eslint-disable no-undef, no-param-reassign, camelcase */
  name: 'DetailedMaintain',
  components: {
    Selector,
    DetailedView,
  },
  props: {
    isShowFilter: {
      type: Boolean,
      default: true,
    },
    inputedQueryObj: {
      type: Object,
    },
  },
  data() {
    return {
      // for selector
      selectorModel: detailedModel,
      hintText: `請先查詢資料`,
      queryObject: {},
      addQueryObject: {},

      // for table
      fields: [],
      items: [],
      coExpMitems: [],
      currentPage: 1,
      perPage: 20,
      selected: [],
      isBusy: false,

      // for edit, and new one
      detailedData: [],
    };
  },
  methods: {
    async searchEvent(filterObj) {
      const { date, duration, port, subjectCode } = filterObj;
      // add date
      if (date.year !== 0 && date.month !== 0) {
        this.queryObject.Year = date.year;
        this.queryObject.Month = date.month;
      }

      // add duration
      if (duration.end > duration.start) {
        this.queryObject.DurationStart = duration.start;
        this.queryObject.DurationEnd = duration.end;
      }

      // add port
      if (port.num > 0) {
        this.queryObject.FamNo = port.num;
      }

      // add subjectCode
      if (subjectCode.code_no > 0) {
        this.queryObject.CodeNo = subjectCode.code_no;
      }

      await this.queryDetailedData(this.queryObject);
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
        this.detailedData = JSON.parse(
          JSON.stringify(
            this.items.filter(item => item.ie_day === dataObj.ie_day)
          )
        );
      } else {
        const { Year, Month, FamNo } = this.queryObject;
        this.detailedData = [];
        this.addQueryObject = {
          Year,
          Month,
          FamNo,
        };
      }

      this.$refs.domModal.show();
    },
    onDetailedDataChanged(detailedObj) {
      console.log(detailedObj);
    },
    async queryDetailedData(queryObject) {
      this.isBusy = true;
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.detatiledData,
        {
          Action: `READ`,
          ...queryObject,
        }
      );

      if (resObject.status === this.mixinBackendErrorCode.success) {
        if (resObject.data.CoExpD.length === 0) {
          this.hintText = `無資料`;
        }

        this.items = resObject.data.CoExpD || [];

        // set code_name
        this.items.forEach(obj => {
          if (obj.code_no !== undefined) {
            const subObject = this.subjectArray.find(
              subObj => subObj.code_no === obj.code_no
            );
            obj.code_name = subObject.code_name || ``;
          }
        });

        this.coExpMitems = resObject.data.CoExpM || [];
      } else {
        this.hintText = `查詢錯誤發生，請確認有輸入必要參數`;
      }

      this.isBusy = false;
      return resObject;
    },
    async onSaveEvent(items) {
      const { ie_year, ie_mon, ie_day, fam_no } = items[0];
      const filteredItems = this.items.filter(
        obj =>
          obj.ie_year === ie_year &&
          obj.ie_mon === ie_mon &&
          obj.ie_day === ie_day &&
          obj.fam_no === fam_no
      );

      const updateItems = items.filter(obj => obj.item_no !== undefined);
      const insertItems = items.filter(obj => !obj.item_no);
      const deleteItems = filteredItems.filter(
        obj =>
          updateItems.map(obj2 => obj2.item_no).includes(obj.item_no) === false
      );

      await this.deleteItems(deleteItems);
      await this.saveItems(updateItems, insertItems);

      this.$refs.domModal.hide();
    },
    async saveItems(updateItems, insertItems) {
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.detatiledData,
        {
          Action: `WRITE`,
          UpdateItems: JSON.stringify(updateItems),
          InsertItems: JSON.stringify(insertItems),
          FamNo: updateItems[0].fam_no,
          Year: updateItems[0].ie_year,
          Month: updateItems[0].ie_mon,
          Day: updateItems[0].ie_day,
          TotalCost: updateItems[0].exp_amt,
        }
      );

      if (resObject.status === this.mixinBackendErrorCode.success) {
        for (let i = 0; i < updateItems.length; i++) {
          const itemIdx = this.items.findIndex(
            obj => obj.item_no === updateItems[i].item_no
          );
          this.$set(this.items, itemIdx, updateItems[i]);
        }
        this.items = [...insertItems, ...this.items];
      }
    },
    async deleteItems(items) {
      const tempItems = items.length > 0 ? items : this.selected;
      if (!tempItems || tempItems.length === 0) {
        return;
      }

      const deleteItemNoAry = tempItems.map(obj => obj.item_no);

      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.detatiledData,
        {
          Action: `DELETE`,
          ItemArray: JSON.stringify(deleteItemNoAry),
          FamNo: tempItems[0].fam_no,
        }
      );

      if (resObject.status === this.mixinBackendErrorCode.success) {
        // delete item in local var
        for (let i = 0; i < tempItems.length; i++) {
          const itemIdx = this.items.findIndex(
            obj => obj.item_no === tempItems[i].item_no
          );
          this.$delete(this.items, itemIdx);
        }
      }
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
  },
  async mounted() {
    if (this.inputedQueryObj) {
      await this.queryDetailedData(this.inputedQueryObj);
    }
  },
  computed: {
    ...mapState([`paramArray`, `subjectArray`]),
    placeName() {
      return placeNo => {
        if (!placeNo || placeNo === 0) {
          return ``;
        }

        if (this.paramArray.length === 0) {
          return ``;
        }

        const paramObj = this.paramArray.find(
          obj => obj.par_typ === `A` && obj.par_no === placeNo
        );
        if (!paramObj) {
          return ``;
        }

        return paramObj.par_name;
      };
    },
    noPaymentDays() {
      if (this.coExpMitems.length === 0) {
        return ``;
      }

      return this.coExpMitems
        .filter(obj => obj.exp_amt.length === 0)
        .map(obj => obj.ie_day)
        .join(', ');
    },
    errorDaysOpts() {
      if (this.items.length === 0 || this.coExpMitems.length === 0) {
        return [];
      }

      return this.coExpMitems
        .reduce((tempAry, mObj) => {
          const ieDay = mObj.ie_day;
          const totalCost = parseInt(mObj.exp_amt, 10);
          const realCost = this.items
            .filter(dObj => dObj.ie_day === ieDay)
            .reduce((total, dObj) => {
              if (dObj.code_amt) {
                total += parseInt(dObj.code_amt, 10);
              }
              return total;
            }, 0);
          if (totalCost !== realCost) {
            tempAry.push(ieDay);
          }

          return tempAry;
        }, [])
        .map(day => {
          return {
            value: day,
            text: `錯誤日期:${day}`,
          };
        });
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
