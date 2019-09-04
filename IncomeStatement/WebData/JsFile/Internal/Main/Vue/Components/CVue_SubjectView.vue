<template>
  <div id="mainPage">
    <b-container fluid>
      <b-row class="my-1" v-for="item in itemKeys" :key="item.key">
        <b-col style="text-align: right;" col lg="2">
          <label>{{ item.text }}</label>
        </b-col>
        <b-col style="text-align: left;" cols="8">
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
              size="sm"
              value="null"
            ></b-form-input>
          </template>
        </b-col>
      </b-row>
    </b-container>

    <hr />

    <b-button variant="info" @click="save">儲存</b-button>
  </div>
</template>

<script>
import { mapState } from 'vuex';

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
          key: `code_rem`,
          text: `備註`,
          type: `text`,
        },
      ],
    };
  },
  methods: {
    setDefaultKeys() {
      this.subjectData.code_no = this.subjectData.code_no || ``;
      this.subjectData.code_name = this.subjectData.code_name || ``;
      this.subjectData.upp_lim = this.subjectData.upp_lim || `null`;
      this.subjectData.low_lim = this.subjectData.low_lim || `null`;
      this.subjectData.place = this.subjectData.place || ``;
      this.subjectData.param1 = this.subjectData.param1 || ``;
      this.subjectData.param2 = this.subjectData.param2 || ``;
      this.subjectData.stop_fg = this.subjectData.stop_fg || `N`;
      this.subjectData.code_rem = this.subjectData.code_rem || ``;
    },
    save() {
      this.$emit(`changed`, this.subjectData);
    },
    cancel() {
      this.$el.hide();
    },
  },
  created() {},
  mounted() {
    this.setDefaultKeys();
  },
  computed: {
    ...mapState([`paramArray`]),
    isDisabled: {
      get() {
        return this.subjectData.stop_fg === `Y`;
      },
      set(value) {
        this.subjectData.stop_fg = value ? `Y` : `N`;
      },
    },
    placeOpts() {
      return this.paramArray
        .filter(obj => obj.par_typ === `A`)
        .sort((obj1, obj2) => {
          const n1 = parseInt(obj1.par_no, 10);
          const n2 = parseInt(obj2.par_no, 10);

          if (n1 > n2) {
            return 1;
          }

          if (n1 === n2) {
            return 0;
          }

          return -1;
        })
        .map(obj => {
          return {
            value: obj.par_no,
            text: `${obj.par_no} ${obj.par_name}`,
          };
        });
    },
    inputType() {
      return [`text`, `number`];
    },
  },
};
</script>

<style scoped></style>
