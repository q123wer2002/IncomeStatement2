<template>
  <div id="mainPage">
    <b-container fluid>
      <b-row class="my-1" v-for="item in itemKeys" :key="item.key">
        <b-col>
          <label>{{ item.text }}</label>
        </b-col>
        <b-col>
          <template>
            <b-form-select
              v-if="item.type === `select`"
              v-model="subjectData[item.key]"
              :options="placeOpts"
              size="sm"
            ></b-form-select>
            <b-form-checkbox
              v-if="item.type === `checkbox`"
              v-model="isDisabled"
              size="sm"
            ></b-form-checkbox>
            <b-form-input
              v-if="inputType.includes(item.type)"
              v-model="subjectData[item.key]"
              :type="item.type"
              size="sm"
            ></b-form-input>
          </template>
        </b-col>
      </b-row>
    </b-container>

    <hr />

    <b-button variant="danger">結束</b-button>
    <b-button variant="info">儲存</b-button>
  </div>
</template>

<script>
export default {
  name: 'SubjectView',
  components: {},
  props: {
    subjectData: {
      type: Object,
      required: false,
    },
  },
  data() {
    return {
      itemKeys: [
        {
          key: `code_no`,
          text: `科目代碼`,
          type: `text`,
        },
        {
          key: `code_name`,
          text: `科目名稱`,
          type: `text`,
        },
        {
          key: `upp_lim`,
          text: `金額上限`,
          type: `number`,
        },
        {
          key: `low_lim`,
          text: `金額下限`,
          type: `number`,
        },
        {
          key: `place`,
          text: `購買地點`,
          type: `select`,
        },
        {
          key: `param1`,
          text: `科目設定1`,
          type: `text`,
        },
        {
          key: `param2`,
          text: `科目設定2`,
          type: `text`,
        },
        {
          key: `stop_fg`,
          text: `是否停用`,
          type: `checkbox`,
        },
        {
          key: `remark`,
          text: `備註`,
          type: `text`,
        },
      ],
    };
  },
  methods: {},
  created() {},
  mounted() {},
  computed: {
    isDisabled: {
      get() {
        return this.subjectData.stop_fg === `Y`;
      },
      set(value) {
        this.subjectData.stop_fg = value ? `Y` : `N`;
      },
    },
    placeOpts() {
      return [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    },
    inputType() {
      return [`text`, `number`];
    },
  },
};
</script>

<style scoped></style>
