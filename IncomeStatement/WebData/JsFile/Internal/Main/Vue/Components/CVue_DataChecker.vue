<template>
  <div id="mainPage">
    <h5>收支資料檢誤</h5>
    <selector :filterModel="selectorModel" @search="searchEvent"></selector>
  </div>
</template>

<script>
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
    };
  },
  methods: {
    searchEvent() {},
  },
  created() {},
  mounted() {},
  computed: {},
  watch: {},
  /* eslint-disable no-undef, no-param-reassign */
};
</script>

<style scoped>
#mainPage {
  text-align: center;
}
</style>
