<template>
  <div id="mainPage">
    <b-container fluid>
      <b-row class="my-1">
        <b-col>
          <label>戶號：</label>
        </b-col>
        <b-col>
          <span>{{ familyNo }}</span>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col>
          <label>日期：</label>
        </b-col>
        <b-col>
          <span>{{ dataDate }}</span>
          <div class="d-block-inline" v-if="isNeedSelectDay">
            <input
              v-model="queryObject.Day"
              class="w-25"
              type="number"
              min="0"
              max="31"
            />
            <span>日</span>
          </div>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col>
          <label>輸入本日總和：</label>
        </b-col>
        <b-col>
          <b-form-input v-model="todayTotalCost" type="number"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col>
          <label>本日實際總和：</label>
        </b-col>
        <b-col>
          <span :style="costStyle">{{ totalCost }}</span>
        </b-col>
      </b-row>
    </b-container>

    <hr />

    <b-table :items="items" :fields="fields" small hover head-variant="dark">
      <template slot="[delete]" slot-scope="data">
        <a href="javascript:;" @click="deleteItem(data.index)" title="刪除">
          <img :src="$options.imgSrc.trash" width="32px" />
        </a>
      </template>
      <template slot="[no]" slot-scope="data">
        {{ data.index + 1 }}
      </template>
      <template slot="[place]" slot-scope="{ item }">
        <b-form-select
          v-model="item.place"
          :options="placeOpts"
        ></b-form-select>
      </template>
      <template slot="[code_amt]" slot-scope="{ item }">
        <b-form-input v-model="item.code_amt" type="number"></b-form-input>
      </template>
      <template slot="[code_no]" slot-scope="data">
        <b-form-input
          v-model="data.item.code_no"
          @update="onCodeChanged(data.index)"
          type="number"
        ></b-form-input>
      </template>
      <template slot="[code_name]" slot-scope="{ item }">
        <b-form-input v-model="item.code_name"></b-form-input>
      </template>
      <template slot="bottom-row">
        <b-td colspan="6">
          <a href="javascript:;" @click="addItem">
            <img :src="$options.imgSrc.add" width="30px" />
            <span>新增一筆資料</span>
          </a>
        </b-td>
      </template>
    </b-table>

    <hr />
    <b-button variant="info" :disabled="!isEnabledSave" @click="saveItems">
      儲存
    </b-button>
  </div>
</template>

<script>
import { mapState } from 'vuex';

export default {
  /* eslint-disable no-undef, no-param-reassign, camelcase */
  imgSrc: {
    trash: `/${webpackDashboardName}/WebData/Picture/icon/material-io/baseline_delete_forever_black_48dp.png`,
    add: `/${webpackDashboardName}/WebData/Picture/icon/material-io/baseline_add_circle_black_48dp.png`,
  },

  name: 'DetailedView',
  components: {},
  props: {
    queryObject: {
      type: Object,
      required: false,
    },
    data: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      tempCost: 0,

      // table
      fields: [
        { key: `delete`, label: `` },
        { key: `no`, label: `項次` },
        { key: `place`, label: `購買地點` },
        { key: `code_amt`, label: `金額` },
        { key: `code_no`, label: `科目代碼` },
        { key: `code_name`, label: `科目名稱` },
      ],
      items: this.data,
    };
  },
  methods: {
    deleteItem(index) {
      this.items.splice(index, 1);
    },
    addItem() {
      this.items.push({
        fam_no: 123456,
        ie_year: 2018,
        ie_mon: 1,
        ie_day: 1,
        place: 0,
        code_amt: 0,
        code_no: 0,
        code_name: ``,
      });
    },
    onCodeChanged(index) {
      const tempItem = this.items[index];
      const codeObj = this.subjectArray.find(
        obj => obj.code_no === tempItem.code_no
      );
      let newName = ``;
      if (codeObj) {
        newName = codeObj.code_name;
      }

      this.$set(this.items[index], `code_name`, newName);
    },
    saveItems() {
      const { ie_year, ie_mon, ie_day, fam_no } = this.data[0];
      this.items.forEach(obj => {
        obj.ie_year = ie_year;
        obj.ie_mon = ie_mon;
        obj.ie_day = ie_day;
        obj.fam_no = fam_no;
      });

      this.$emit(`save`, this.items);
    },
    initialSubName() {
      this.items.forEach(obj => {
        if (obj.code_no !== undefined) {
          const subObject = this.subjectArray.find(
            subObj => subObj.code_no === obj.code_no
          );
          obj.code_name = subObject.code_name || ``;
        }
      });
    },
    async queryDetailedData(queryObject) {
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.detatiledData,
        {
          Action: `READ`,
          ...queryObject,
        }
      );

      this.items = resObject.data.CoExpD;
      this.initialSubName();
    },
  },
  created() {},
  mounted() {
    if (this.data.length > 0) {
      this.initialSubName();
    }
  },
  computed: {
    ...mapState([`paramArray`, `subjectArray`]),
    todayTotalCost: {
      get() {
        if (this.items.length > 0) {
          return parseInt(this.items[0].exp_amt, 10);
        }

        return this.tempCost;
      },
      set(value) {
        if (this.items.length === 0) {
          this.tempCost = value;
          return;
        }

        this.items.forEach(obj => {
          obj.exp_amt = value;
        });
      },
    },
    placeOpts() {
      return this.paramArray
        .filter(obj => obj.par_typ === `A`)
        .map(obj => {
          return {
            text: obj.par_name,
            value: obj.par_no,
          };
        });
    },
    subjectOpts() {
      return this.subjectArray.map(obj => {
        return {
          text: obj.code_name,
          value: obj.code_no,
        };
      });
    },
    familyNo() {
      if (this.data.length > 0) {
        return this.data[0].fam_no;
      }

      const { FamNo } = this.queryObject;
      if (FamNo) {
        return FamNo;
      }

      return ``;
    },
    dataDate() {
      if (this.data.length > 0) {
        const { ie_year, ie_mon, ie_day } = this.data[0];
        return `${ie_year}年${ie_mon}月${ie_day}日`;
      }

      const { Year, Month } = this.queryObject;
      if (Year && Month) {
        return `${Year}年${Month}月`;
      }

      return ``;
    },
    totalCost() {
      if (this.items.length > 0) {
        return this.items.reduce((totalCost, itemObj) => {
          totalCost += parseInt(itemObj.code_amt, 10);
          return totalCost;
        }, 0);
      }

      return 0;
    },
    costStyle() {
      if (this.totalCost !== this.todayTotalCost) {
        return {
          color: `red`,
        };
      }

      return {
        color: `green`,
      };
    },
    isEnabledSave() {
      // check cost
      if (this.totalCost !== this.todayTotalCost) {
        return false;
      }

      // check empty
      return this.items.every(obj => obj.code_name.length !== 0);
    },
    isNeedSelectDay() {
      if (this.data.length > 0) {
        return false;
      }

      const { Year, Month, FamNo } = this.queryObject;
      if (Year && Month && FamNo) {
        return true;
      }

      return false;
    },
  },
  watch: {
    queryObject: {
      async handler(value) {
        await this.queryDetailedData(value);
      },
      deep: true,
    },
  },
  /* eslint-disable no-undef, no-param-reassign, camelcase */
};
</script>

<style scoped>
#mainPage {
  text-align: left;
}
</style>
