<template>
  <div id="mainPage">
    <div class="filterItem">
      <span>年月</span>
      <b-form-select
        v-model="date.year"
        :options="yearOptions"
        class="d-inline w-25 h-10"
        size="sm"
      ></b-form-select>
      <b-form-select
        v-model="date.month"
        :options="monthOptions"
        class="d-inline w-25 h-10"
        size="sm"
      ></b-form-select>
    </div>
    <div class="filterItem">
      <span>戶號</span>
      <b-form-input
        v-model="port.start"
        type="number"
        class="d-inline w-25 h-25"
        size="sm"
      ></b-form-input>
      ~
      <b-form-input
        v-model="port.end"
        type="number"
        class="d-inline w-25 h-25"
        size="sm"
      ></b-form-input>
    </div>
    <div class="filterItem">
      <span>每頁顯示比數</span>
      <b-form-input
        v-model="showNumbers"
        type="number"
        class="d-inline w-25 h-25"
        size="sm"
      ></b-form-input>
    </div>

    <b-button variant="info" id="btnSearch" size="sm" @click="search">
      查詢
    </b-button>
  </div>
</template>

<script>
export default {
  name: 'Selector',
  components: {},
  props: {},
  data() {
    return {
      date: {
        year: 0,
        month: 0,
      },
      port: {
        start: 0,
        end: 0,
      },
      showNumbers: 20,
    };
  },
  methods: {
    search() {
      this.$emit(`search`, {
        date: this.date,
        port: this.port,
        showNumbers: this.showNumbers,
      });
    },
  },
  created() {},
  mounted() {},
  computed: {
    yearOptions() {
      return [2017, 2018, 2019].map(year => {
        return {
          value: year,
          text: `${year}年`,
        };
      });
    },
    monthOptions() {
      return [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12].map(month => {
        return {
          value: month,
          text: `${month}月`,
        };
      });
    },
  },
};
</script>

<style scoped>
#mainPage {
  position: relative;
  width: 70%;
  padding: 8px 16px;
  margin: 8px auto;
  border-bottom: 1px solid #61c7d0;
  font-size: 16px;
}
#hintTitle {
  text-align: left;
  font-weight: 900;
}
.filterItem {
  margin: 8px auto;
  text-align: left;
}
#btnSearch {
  position: absolute;
  bottom: 8px;
  right: 8px;
}
</style>
