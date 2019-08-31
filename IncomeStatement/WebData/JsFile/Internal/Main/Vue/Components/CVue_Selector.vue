<template>
  <div id="mainPage">
    <div class="filterItem" v-for="item in items" :key="item.key">
      <span>{{ item.text }}</span>
      <span v-for="(typeValue, typeKey) in item.type" :key="typeKey">
        <b-form-select
          v-if="typeValue === `select`"
          v-model="item.value[typeKey]"
          :options="item.source[typeKey]"
          class="d-inline w-25 h-10"
          size="sm"
          :state="item.valid[typeKey](item.value[typeKey])"
        ></b-form-select>

        <b-form-input
          v-else
          v-model="item.value[typeKey]"
          :type="typeValue"
          class="d-inline w-25 h-25"
          size="sm"
          :state="item.valid[typeKey](item.value[typeKey])"
        ></b-form-input>

        <span v-if="typeKey === `start`">~</span>
      </span>
    </div>

    <b-button
      variant="info"
      id="btnSearch"
      size="sm"
      @click="search"
      :disabled="!isBtnSearchDisabled"
    >
      查詢
    </b-button>
  </div>
</template>

<script>
export default {
  /* eslint-disable no-undef, no-param-reassign */
  name: 'Selector',
  components: {},
  props: {
    filterModel: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      items: [],
    };
  },
  methods: {
    initialFilterItem() {
      // key, text, type, value, source
      this.items = this.filterModel;
    },
    search() {
      const itemValueObj = this.items.reduce((tempObj, itemObj) => {
        tempObj[itemObj.key] = itemObj.value;
        return tempObj;
      }, {});

      // emit
      this.$emit(`search`, itemValueObj);
    },
  },
  created() {},
  mounted() {
    this.initialFilterItem();
  },
  computed: {
    isBtnSearchDisabled() {
      if (this.items.length === 0) {
        return false;
      }

      return this.items.every(itemObj => {
        return Object.keys(itemObj.value).every(key => {
          const value = itemObj.value[key];
          return itemObj.valid[key](value);
        });
      });
    },
  },
  /* eslint-disable no-undef, no-param-reassign */
};
</script>

<style scoped>
#mainPage {
  position: relative;
  width: 85%;
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
