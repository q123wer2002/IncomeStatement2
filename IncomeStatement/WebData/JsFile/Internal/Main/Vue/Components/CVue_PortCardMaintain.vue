<template>
  <div id="mainPage">
    <h5>戶口組成資料</h5>
    <selector :filterModel="selectorModel" @search="searchEvent"></selector>
  </div>
</template>

<script>
import { dataCheckerModel } from '../DataModel/selectorModel.js';
import Selector from './CVue_Selector.vue';

export default {
  /* eslint-disable no-undef, no-param-reassign */
  name: 'PortCardMaintain',
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
