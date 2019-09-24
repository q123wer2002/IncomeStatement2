<template>
  <div id="mainPage">
    <h5>戶口組成資料</h5>

    <p class="my-2">
      {{ titleString }}
    </p>

    <b-table :items="items" :fields="fields" hover small head-variant="dark">
      <b-form-input
        slot="[mem_no]"
        slot-scope="data"
        v-model="data.item.mem_no"
      ></b-form-input>
      <b-form-input
        slot="[title]"
        slot-scope="data"
        v-model="data.item.title"
      ></b-form-input>
      <b-form-input
        slot="[fam_head_rel]"
        slot-scope="data"
        v-model="data.item.fam_head_rel"
      ></b-form-input>
      <b-form-input
        slot="[mem_name]"
        slot-scope="data"
        v-model="data.item.mem_name"
      ></b-form-input>
      <b-form-select
        slot="[gender]"
        slot-scope="data"
        v-model="data.item.gender"
        :options="genderOpts"
      ></b-form-select>
      <b-form-input
        slot="[bir_year]"
        slot-scope="data"
        v-model="data.item.bir_year"
      ></b-form-input>
      <b-form-select
        slot="[bir_mon]"
        slot-scope="data"
        v-model="data.item.bir_mon"
        :options="monthOpts"
      ></b-form-select>
      <b-form-select
        slot="[edu_no]"
        slot-scope="data"
        v-model="data.item.edu_no"
        :options="eduNoOpts"
      ></b-form-select>
      <b-form-select
        slot="[job_typ]"
        slot-scope="data"
        v-model="data.item.job_typ"
        :options="jobTypeOpts"
      ></b-form-select>
      <b-form-input
        slot="[mem_remark]"
        slot-scope="data"
        v-model="data.item.mem_remark"
      ></b-form-input>
    </b-table>

    <b-container fluid>
      <b-row class="my-1" v-for="item in fields" :key="item.key">
        <b-col style="text-align: right;" col lg="3">
          <label>{{ item.text }}</label>
        </b-col>
        <b-col style="text-align: left;" cols="8">
          <template>
            <span v-if="item.type === `span`">{{ clonedData[item.key] }}</span>
            <b-form-select
              v-if="item.type === `select`"
              v-model="clonedData[item.key]"
              :options="item.options"
              size="sm"
            ></b-form-select>
            <b-form-checkbox-group
              v-if="item.type === `checkbox`"
              v-model="clonedData[item.key]"
              :options="item.options"
            ></b-form-checkbox-group>
            <b-form-input
              v-if="inputType.includes(item.type)"
              v-model="clonedData[item.key]"
              size="sm"
              value="null"
            ></b-form-input>
          </template>
        </b-col>
      </b-row>
    </b-container>
    <br />
    <b-button @click="saveItem" variant="info">儲存</b-button>
  </div>
</template>

<script>
import { mapState } from 'vuex';

export default {
  /* eslint-disable no-undef, no-param-reassign, camelcase */
  name: 'HouseInfo',
  components: {},
  props: {
    coFamData: {
      type: Array,
      reqiured: true,
    },
  },
  data() {
    return {
      items: {},
    };
  },
  methods: {
    saveItem() {},
  },
  created() {
    this.items = JSON.parse(JSON.stringify(this.coFamData));
    console.log(this.items);
  },
  mounted() {},
  computed: {
    ...mapState(['paramArray']),
    fields() {
      return [
        {
          label: '代號',
          key: 'mem_no',
        },
        {
          label: '稱謂',
          key: 'title',
        },
        {
          label: '與戶長關係代號',
          key: 'fam_head_rel',
        },
        {
          label: '姓名',
          key: 'mem_name',
        },
        {
          label: '性別',
          key: 'gender',
        },
        {
          label: '出生年',
          key: 'bir_year',
        },
        {
          label: '出生月',
          key: 'bir_mon',
        },
        {
          label: '年齡',
          key: 'age',
        },
        {
          label: '最高教育程度',
          key: 'edu_no',
        },
        {
          label: '就業別',
          key: 'job_typ',
        },
        {
          label: '備註',
          key: 'mem_remark',
        },
      ];
    },
    titleString() {
      if (this.items.length <= 0) {
        return '';
      }

      const { ie_year, ie_mon, fam_no } = this.items[0];
      return `年月：${ie_year}年${ie_mon}月、戶號：${fam_no}`;
    },
    genderOpts() {
      return [
        {
          text: '男',
          value: '男',
        },
        {
          text: '女',
          value: '女',
        },
      ];
    },
    monthOpts() {
      return [
        `01`,
        `02`,
        `03`,
        `04`,
        `05`,
        `06`,
        `07`,
        `08`,
        `09`,
        `10`,
        `11`,
        `12`,
      ].map(month => {
        return {
          text: `${month}月`,
          value: parseInt(month, 10),
        };
      });
    },
    eduNoOpts() {
      return this.paramArray
        .filter(obj => obj.par_typ === `E`)
        .map(obj => {
          return {
            text: obj.par_name,
            value: obj.par_no,
          };
        });
    },
    jobTypeOpts() {
      return this.paramArray
        .filter(obj => obj.par_typ === `F`)
        .map(obj => {
          return {
            text: obj.par_name,
            value: obj.par_no,
          };
        });
    },
    inputType() {
      return [`text`, `number`];
    },
  },
  watch: {},
  /* eslint-disable no-undef, no-param-reassign, camelcase */
};
</script>

<style scoped>
#mainPage {
  text-align: center;
}
</style>
