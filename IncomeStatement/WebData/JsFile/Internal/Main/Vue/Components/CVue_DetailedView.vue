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
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col>
          <label>本日支出總和：</label>
        </b-col>
        <b-col>
          <span>{{ totalCost }}</span>
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
        <b-form-input v-model="item.place"></b-form-input>
      </template>
      <template slot="[code_amt]" slot-scope="{ item }">
        <b-form-input v-model="item.code_amt"></b-form-input>
      </template>
      <template slot="[code_no]" slot-scope="{ item }">
        <b-form-input v-model="item.code_no"></b-form-input>
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
    <b-button variant="danger">結束</b-button>
    <b-button variant="info">儲存</b-button>
  </div>
</template>

<script>
export default {
  /* eslint-disable no-undef, no-param-reassign, camelcase */
  imgSrc: {
    trash: `/${webpackDashboardName}/WebData/Picture/icon/material-io/baseline_delete_forever_black_48dp.png`,
    add: `/${webpackDashboardName}/WebData/Picture/icon/material-io/baseline_add_circle_black_48dp.png`,
  },

  name: 'DetailedView',
  components: {},
  props: {
    data: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      // local var

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
        ie_month: 1,
        ie_day: 1,
        place: 0,
        code_amt: 0,
        code_no: 0,
        code_name: ``,
      });
    },
  },
  created() {},
  mounted() {},
  computed: {
    familyNo() {
      if (this.data.length > 0) {
        return this.data[0].fam_no;
      }

      return ``;
    },
    dataDate() {
      if (this.data.length > 0) {
        const { ie_year, ie_month, ie_day } = this.data[0];
        return `${ie_year}年${ie_month}月${ie_day}日`;
      }

      return ``;
    },
    totalCost() {
      if (this.data.length > 0) {
        return this.data.reduce((totalCost, itemObj) => {
          totalCost += itemObj.code_amt;
          return totalCost;
        }, 0);
      }

      return 0;
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
