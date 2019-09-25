<template>
  <div id="mainPage">
    <h5>變更密碼</h5>
    <div class="w-50 mx-auto my-3">
      <label for="input-original">原密碼</label>
      <b-form-input
        id="input-original"
        v-model="originalPwd"
        :state="isValidOrigialPwd"
        @update="validPwd($event, false)"
        type="password"
      ></b-form-input>
      <label for="input-newone">新密碼</label>
      <b-form-input
        id="input-newone"
        v-model="newPwd"
        :state="isValidNewPwd"
        @update="validPwd($event, true)"
        type="password"
      ></b-form-input>
    </div>
    <b-button
      variant="info"
      @click="changePassword"
      :disabled="!isValidOrigialPwd || !isValidNewPwd"
    >
      變更
    </b-button>
  </div>
</template>

<script>
const sha256 = require('js-sha256');

export default {
  /* eslint-disable no-undef, no-param-reassign */
  name: 'ChangePassword',
  components: {},
  props: {},
  data() {
    return {
      originalPwd: ``,
      isValidOrigialPwd: false,
      newPwd: ``,
      isValidNewPwd: false,
    };
  },
  methods: {
    validPwd(value, isNewPassword) {
      const isValid = value.length > 0;
      if (isNewPassword) {
        this.isValidNewPwd = isValid;
      } else {
        this.isValidOrigialPwd = isValid;
      }
    },
    async changePassword() {
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.changePwd,
        {
          OPwd: sha256(this.originalPwd),
          NPwd: sha256(this.newPwd),
        }
      );

      if (resObject.status !== this.mixinBackendErrorCode.success) {
        alert(`變更錯誤`);
        return;
      }

      alert(`變更成功`);
    },
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
  margin: auto;
}
</style>
