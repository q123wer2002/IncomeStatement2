<template>
  <div id="mainPage">
    <h5>{{ title }}</h5>
    <p v-if="isEdit === false">預設密碼：test1234</p>
    <b-container fluid>
      <b-row
        class="my-1"
        v-for="item in itemKeys"
        :key="item.key"
        v-if="item.isShow"
      >
        <b-col style="text-align: right;" col lg="2">
          <label>{{ item.text }}</label>
        </b-col>
        <b-col style="text-align: left;" cols="8">
          <template>
            <b-form-select
              v-if="item.type === `select`"
              v-model="tempUser[item.key]"
              :options="item.options"
              size="sm"
            ></b-form-select>
            <b-form-radio-group
              v-if="item.type === `radio`"
              v-model="tempUser[item.key]"
              :options="item.options"
              size="sm"
            ></b-form-radio-group>
            <b-form-input
              v-if="inputType.includes(item.type)"
              v-model="tempUser[item.key]"
              size="sm"
              value="null"
            ></b-form-input>
          </template>
        </b-col>
      </b-row>
    </b-container>

    <hr />

    <b-button variant="info" @click="save">儲存</b-button>
    <b-button variant="info" v-if="isEdit" @click="deleteUser">刪除</b-button>
  </div>
</template>

<script>
import { mapState } from 'vuex';
import { accountRole, accountStateToString } from '../DataModel/dataModel.js';

export default {
  name: 'AccountMaintain',
  components: {},
  props: {
    userObj: {
      type: Object,
      required: false,
    },
  },
  data() {
    return {
      isEdit: false,
      tempUser: {},
    };
  },
  methods: {
    save() {
      this.$emit(`save-user`, this.tempUser);
    },
    deleteUser() {
      this.$emit(`delete-user`, this.tempUser);
    },
  },
  created() {},
  mounted() {
    if (Object.keys(this.userObj).length > 0) {
      this.tempUser = JSON.parse(JSON.stringify(this.userObj));
      this.isEdit = true;
    } else {
      this.tempUser = {
        email: ``,
        tel_no: ``,
        title: ``,
        remark: ``,
        dep_name: ``,
        role: ``,
        state: ``,
        login_id: ``,
        pwd: ``,
        start_date: ``,
        end_date: ``,
      };
    }
  },
  computed: {
    ...mapState(['paramArray']),
    itemKeys() {
      return [
        {
          isShow: true,
          text: '登入帳號',
          key: 'login_id',
          type: 'text',
        },
        {
          isShow: true,
          text: '姓名',
          key: 'user_name',
          type: 'text',
        },
        {
          isShow: this.isEdit,
          text: '密碼',
          key: 'pwd',
          type: 'text',
        },
        {
          isShow: true,
          text: '使用者角色',
          key: 'role',
          type: 'radio',
          options: this.roleOpts,
        },
        {
          isShow: true,
          text: '使用者信箱',
          key: 'email',
          type: 'text',
        },
        {
          isShow: true,
          text: '使用者電話',
          key: 'tel_no',
          type: 'text',
        },
        {
          isShow: true,
          text: '職稱',
          key: 'title',
          type: 'select',
          options: this.titleOpts,
        },
        {
          isShow: true,
          text: '機關單位',
          key: 'dep_name',
          type: 'select',
          options: this.depOpts,
        },
        {
          isShow: true,
          text: '備註',
          key: 'remark',
          type: 'text',
        },
        {
          isShow: true,
          text: '狀態',
          key: 'state',
          type: 'select',
          options: this.stateOpts,
        },
        {
          isShow: true,
          text: '帳號開始日期',
          key: 'start_date',
          type: 'text',
        },
        {
          isShow: true,
          text: '帳號結束日期',
          key: 'end_date',
          type: 'text',
        },
      ];
    },
    roleOpts() {
      return Object.keys(accountRole).map(key => {
        return {
          text: accountRole[key],
          value: key,
        };
      });
    },
    stateOpts() {
      return Object.keys(accountStateToString).map(key => {
        return {
          text: accountStateToString[key],
          value: key,
        };
      });
    },
    titleOpts() {
      return this.paramArray
        .filter(obj => obj.par_typ === `B`)
        .map(obj => {
          return {
            text: obj.par_name,
            value: obj.par_name,
          };
        });
    },
    depOpts() {
      return this.paramArray
        .filter(obj => obj.par_typ === `C`)
        .map(obj => {
          return {
            text: obj.par_name,
            value: obj.par_name,
          };
        });
    },
    inputType() {
      return [`text`];
    },
    title() {
      if (this.isEdit) {
        return `帳號編輯`;
      }
      return `帳號新增`;
    },
  },
  watch: {},
};
</script>

<style scoped></style>
