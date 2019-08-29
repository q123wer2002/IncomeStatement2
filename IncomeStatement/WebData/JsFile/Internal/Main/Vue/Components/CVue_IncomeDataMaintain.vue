<template>
  <div id="mainPage">
    <h5>收支資料維護</h5>
    <selector @search="searchEvent"></selector>

    <!-- filtered data -->
    <div></div>
  </div>
</template>

<script>
import Selector from './CVue_Selector.vue';

export default {
  name: 'IncomeDataMaintain',
  components: {
    Selector,
  },
  data() {
    return {};
  },
  props: {},
  methods: {
    searchEvent(message) {
      console.log(message);
    },
  },
  created() {},
  mounted() {},
  computed: {},
};
</script>

<style scoped="">
#mainPage {
  text-align: center;
}
</style>
