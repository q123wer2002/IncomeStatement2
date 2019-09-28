<template>
  <div id="mainPage">
    <h5>{{ title }}</h5>
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
            <span v-if="item.type === `span`">{{ tempFam[item.key] }}</span>
            <b-form-select
              v-if="item.type === `select`"
              v-model="tempFam[item.key]"
              :options="item.options"
              size="sm"
            ></b-form-select>
            <b-form-radio-group
              v-if="item.type === `radio`"
              v-model="tempFam[item.key]"
              :options="item.options"
              size="sm"
            ></b-form-radio-group>
            <template v-if="item.type === `text`">
              <b-form-input
                v-model="tempFam[item.key]"
                v-if="userKey.includes(item.key)"
                @update="onChangeUser($event, item.key)"
                size="sm"
                value="null"
              ></b-form-input>
              <b-form-input
                v-model="tempFam[item.key]"
                v-else
                size="sm"
                value="null"
              ></b-form-input>
            </template>
          </template>
        </b-col>
      </b-row>
    </b-container>

    <hr />

    <ul class="famList my-3">
      <li v-for="(famObj, index) in familyAry" :key="index">
        <a href="javascript:;" @click="deleteFam(index)">
          <img width="24px" :src="$options.imgSrc.trash" />
        </a>
        <b-form-input
          v-model="famObj.fam_no"
          @update="onChangeFamNo($event, index)"
          class="d-inline w-50"
        ></b-form-input>
        <b-form-select
          v-model="famObj.state"
          :options="stateOpts"
          class="d-inline w-25"
        ></b-form-select>
      </li>
    </ul>
    <b-button variant="info" @click="save">儲存</b-button>
  </div>
</template>

<script>
import { portState } from '../DataModel/dataModel.js';

export default {
  /* eslint-disable no-undef, no-param-reassign, camelcase */
  imgSrc: {
    trash: `/${webpackDashboardName}/WebData/Picture/icon/material-io/baseline_delete_forever_black_48dp.png`,
  },

  name: 'AccountMaintain',
  components: {},
  props: {
    famObj: {
      type: Object,
      required: false,
    },
  },
  data() {
    return {
      isEdit: false,
      tempFam: {},
      sysUserList: [],
      familyAry: [
        {
          fam_no: ``,
          state: `1`,
        },
      ],
    };
  },
  methods: {
    async getSysUserList() {
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.mySysUser
      );

      if (resObject.status !== this.mixinBackendErrorCode.success) {
        // do nothing
        return;
      }

      this.sysUserList = resObject.data;
    },
    async queryAccount() {
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.famInfo,
        {
          Action: `READ`,
          ChechInNo: this.famObj.rec_user,
          ReviewNo: this.famObj.adi_user,
        }
      );

      if (resObject.status !== this.mixinBackendErrorCode.success) {
        // do nothing
        return;
      }

      this.familyAry = [...resObject.data, ...this.familyAry];
    },
    onChangeUser(value, key) {
      if (key === `adi_user`) {
        this.tempFam.adi_name = this.userIdToName(value);
      } else {
        this.tempFam.rec_name = this.userIdToName(value);
      }
    },
    onChangeFamNo(value, index) {
      if (index === this.familyAry.length - 1) {
        if (value.length > 0) {
          this.familyAry.push({
            fam_no: ``,
            state: 1,
          });
        }
      }
    },
    deleteFam(index) {
      this.familyAry.splice(index, 1);

      if (this.familyAry.length === 0) {
        this.familyAry.push({
          fam_no: ``,
          state: `1`,
        });
      }
    },
    userIdToName(userId) {
      const userObj = this.sysUserList.find(obj => obj.user_id === userId);
      if (userObj) {
        return userObj.user_name;
      }

      return ``;
    },
    save() {
      const result = this.familyAry
        .filter(obj => obj.fam_no.length > 0)
        .map(obj => {
          return {
            ...this.tempFam,
            fam_no: obj.fam_no,
            state: obj.state,
          };
        });

      this.$emit(`save-fam`, result);
    },
  },
  created() {},
  async mounted() {
    if (Object.keys(this.famObj).length > 0) {
      this.tempFam = JSON.parse(JSON.stringify(this.famObj));
      this.isEdit = true;

      await this.queryAccount();
    } else {
      this.tempFam = {
        fam_no: ``,
        rec_user: ``,
        rec_name: ``,
        adi_user: ``,
        adi_name: ``,
        state: ``,
      };
    }

    await this.getSysUserList();
  },
  computed: {
    itemKeys() {
      return [
        {
          isShow: true,
          text: '登錄人員編號',
          key: 'rec_user',
          type: 'text',
        },
        {
          isShow: true,
          text: '登錄人員',
          key: 'rec_name',
          type: 'span',
        },
        {
          isShow: true,
          text: '審核員編號',
          key: 'adi_user',
          type: 'text',
        },
        {
          isShow: true,
          text: '審核人員',
          key: 'adi_name',
          type: 'span',
        },
      ];
    },
    stateOpts() {
      return Object.keys(portState).map(key => {
        return {
          text: portState[key],
          value: key,
        };
      });
    },
    userKey() {
      return [`adi_user`, `rec_user`];
    },
    title() {
      if (this.isEdit) {
        return `登錄戶號編輯`;
      }
      return `登錄戶號新增`;
    },
  },
  watch: {},
  /* eslint-disable no-undef, no-param-reassign, camelcase */
};
</script>

<style scoped>
.famList li {
  list-style: none;
}
</style>
