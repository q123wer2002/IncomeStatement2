<template>
  <div id="mainPage">
    <b-container fluid>
      <b-row class="my-1">
        <b-col style="text-align: right;" col lg="2">
          <label>戶號：</label>
        </b-col>
        <b-col style="text-align: left" col lg="8">
          <span>{{ familyNo }}</span>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col style="text-align: right;" col lg="2">
          <label>日期：</label>
        </b-col>
        <b-col style="text-align: left" col lg="8">
          <span>{{ dataDate }}</span>
          <div class="d-block-inline" v-if="isNeedSelectDay">
            <input
              v-model="queryObject.Day"
              class="w-25"
              type="number"
              min="0"
              max="31"
            />
            <span>日</span>
          </div>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col style="text-align: right;" col lg="2">
          <label>輸入金額合計：</label>
        </b-col>
        <b-col
          style="text-align: left;font-weight: 900;font-size: 25px;color: red;"
          col
          lg="8"
        >
          <span>{{ totalCost }}</span>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col style="text-align: right;" col lg="2">
          <label>備註：</label>
        </b-col>
        <b-col style="text-align: left" col lg="8">
          <b-form-input v-model="tempRemark" size="sm"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col style="text-align: right;" col lg="2">
          <label>總項次：</label>
        </b-col>
        <b-col style="text-align: left" col lg="8">
          <span>{{ items.length ? items.length : 0 }}</span>
        </b-col>
      </b-row>
    </b-container>

    <hr />

    <b-table :items="items" :fields="fields" small hover head-variant="dark">
      <template slot="[delete]" slot-scope="data">
        <a href="javascript:;" @click="deleteItem(data.index)" title="刪除">
          <img :src="$options.imgSrc.trash" width="32px" />
        </a>
      </template>
      <template slot="[no]" slot-scope="data">
        {{ data.index + 1 }}
      </template>
      <template slot="[place]" slot-scope="data">
        <b-form-input
          v-model="data.item.place"
          @focus="onChangeSubName(data.index)"
          list="placeNoList"
        ></b-form-input>
        <datalist id="placeNoList">
          <option v-for="(placeCode, index) in placeList" :key="index">
            {{ placeCode }}
          </option>
        </datalist>
      </template>
      <template slot="[code_amt]" slot-scope="data">
        <b-form-input
          v-model="data.item.code_amt"
          @focus="onChangeSubName(data.index)"
        ></b-form-input>
      </template>
      <template slot="[code_no]" slot-scope="data">
        <b-form-input
          v-model="data.item.code_no"
          @update="onCodeChanged(data.index)"
          list="subjectNolist"
        ></b-form-input>
        <datalist id="subjectNolist">
          <option v-for="(code, index) in subjectCodeNotDuplicate" :key="index">
            {{ code }}
          </option>
        </datalist>
      </template>
      <template slot="[code_name]" slot-scope="data">
        <b-form-input
          v-model="data.item.code_name"
          list="subjectNamelist"
          @focus="changeSubjectOpts(data.item.code_no)"
          @update="onSubjectNameChanged(data.index)"
        ></b-form-input>
        <datalist id="subjectNamelist">
          <option v-for="(name, index) in tempSubjectArray" :key="index">
            {{ name }}
          </option>
        </datalist>
      </template>
      <template slot="bottom-row">
        <b-td colspan="6">
          <a href="javascript:;" @click="addItem">
            <img :src="$options.imgSrc.add" width="30px" />
            <span>新增一筆資料</span>
          </a>
        </b-td>
      </template>
    </b-table>

    <hr />
    <span style="color: red">{{ cantSaveHint }}</span>
    <b-button variant="info" :disabled="!isEnabledSave" @click="saveItems">
      儲存
    </b-button>
  </div>
</template>

<script>
/* eslint-disable  */
import { mapState } from 'vuex';
import { debounce } from '../../../Common/Utility.js';

export default {  
  imgSrc: {
    trash: `/${webpackDashboardName}/WebData/Picture/icon/material-io/baseline_delete_forever_black_48dp.png`,
    add: `/${webpackDashboardName}/WebData/Picture/icon/material-io/baseline_add_circle_black_48dp.png`,
  },

  name: 'DetailedView',
  components: {},
  props: {
    queryObject: {
      type: Object,
      required: false,
    },
    data: {
      type: Array,
      required: true,
    },
    remark: {
      type: String,
      required: false,
    },
  },
  data() {
    return {
      tempRemark: ``,

      // table
      fields: [
        { key: `delete`, label: `` },
        { key: `no`, label: `項次` },
        { key: `place`, label: `購買地點` },
        { key: `code_amt`, label: `金額` },
        { key: `code_no`, label: `科目代碼` },
        { key: `code_name`, label: `科目名稱` },
      ],
      items: this.data,
      cantSaveHint: ``,
      isEnabledSave: false,
      tempSubjectArray: [],
      tempSubjectNoObj: [],
    };
  },
  methods: {
    deleteItem(index) {
      const { ie_year, ie_mon, ie_day, fam_no } = this.items[0];
      this.items.splice(index, 1);

      if (this.saveItems.length === 0 ){
        this.queryObject.Year = ie_year;
        this.queryObject.Month = ie_mon;
        this.queryObject.Day = ie_day;
        this.queryObject.FamNo = fam_no;
      }
    },
    addItem() {
      this.items.push({
        fam_no: this.queryObject.FamNo || `123456`,
        ie_year: this.queryObject.Year || 2018,
        ie_mon: this.queryObject.Month || 1,
        ie_day: this.queryObject.Day || 1,
        place: ``,
        code_no: ``,
        code_name: ``,
      });
    },
    onCodeChanged(index) {
      const tempItem = this.items[index];

      // get subject name with def_fg
      let codeObj = this.subjectWithDEFFG.find(
        obj => obj.code_no === tempItem.code_no
      );

      // get subject name by first item
      if (!codeObj) {
        codeObj = this.subjectArray.find(
          obj => obj.code_no === tempItem.code_no
        );
      }

      let newName = ``;
      if (codeObj) {
        newName = codeObj.code_name;
      }

      this.$set(this.items[index], `code_name`, newName);
      this.changeSubjectCodeOpts(tempItem.code_no);
    },
    onSubjectNameChanged(index) {
      const tempItem = this.items[index];
      const subjectName = tempItem.code_name;
      const item = this.subjectArray.find(obj => obj.code_name === subjectName);
      if (!item) {
        return;
      }

      this.items[index].code_no = item.code_no;
    },
    saveItems() {
      let Year;
      let Month;
      let Day;
      let FamNo;

      if (this.data.length > 0 || this.items.length > 0) {
        const { ie_year, ie_mon, ie_day, fam_no } = this.data[0] || this.items[0];
        this.items.forEach(obj => {
          obj.ie_year = ie_year;
          obj.ie_mon = ie_mon;
          obj.ie_day = ie_day;
          obj.fam_no = fam_no;
        });

        Year = ie_year;
        Month = ie_mon;
        Day = ie_day;
        FamNo = fam_no;
      } else {
        Year = this.queryObject.Year;
        Month = this.queryObject.Month;
        Day = this.queryObject.Day;
        FamNo = this.queryObject.FamNo;
      }

      this.$emit(`save`, {
        items: this.items,
        remark: this.tempRemark,
        totalCost: this.totalCost,
        queryObject: {
          Year,
          Month,
          Day,
          FamNo,
        },
      });
    },
    async queryDetailedData(queryObject) {
      const resObject = await this.mixinCallBackService(
        this.mixinBackendService.detatiledData,
        {
          totalCost: this.totalCost,
          Action: `READ`,
          ...queryObject,
        }
      );

      this.items = resObject.data.CoExpD;
      this.tempRemark =
        resObject.data.CoExpM.length === 0
          ? ``
          : resObject.data.CoExpM[0].day_rem;
    },
    checkIsEnableSave() {
      // check code no
      const isExistCodeNo = this.items
        .map(obj => obj.code_no)
        .every(
          code =>
            this.subjectArray.findIndex(obj => obj.code_no === code) !== -1
        );
      if (isExistCodeNo === false) {
        this.cantSaveHint = `有科目代碼不存在`;
        this.isEnabledSave = false;
        return;
      }

      // check empty
      const isNoEmpty = this.items.every(obj => obj.code_name.length !== 0);
      if (isNoEmpty === false) {
        this.cantSaveHint = `有科目欄位為空`;
        this.isEnabledSave = false;
        return;
      }

      this.cantSaveHint = ``;
      this.isEnabledSave = true;
    },
    onChangeSubName(index) {
      const totalNum = this.items.length - 1;
      if (totalNum !== index) {
        return;
      }

      this.addItem();
    },
    changeSubjectOpts(codeNo) {
      if (!codeNo) {
        this.tempSubjectArray = this.subjectArray.map(obj => obj.code_name);
        return;
      }

      this.tempSubjectArray = this.subjectArray
        .filter(obj => obj.code_no === codeNo)
        .map(obj => obj.code_name);
    },
    changeSubjectCodeOpts(codeNo) {
      /*
      if (!this.fnUpdateCodeAry) {
        const fnPrint = inputCodeNo => {
          if (this.tempSubjectNoObj.length > 0) {
            const isStartSameNo = this.tempSubjectNoObj.every(code => {
              const regex = new RegExp(
                `^${inputCodeNo.substring(0, inputCodeNo.length - 1)}`
              );
              return regex.test(code);
            });

            if (isStartSameNo) {
              this.tempSubjectNoObj = this.tempSubjectNoObj.filter(code => {
                const regex = new RegExp(`^${inputCodeNo}`);
                return regex.test(code);
              });

              return;
            }
          }

          const isExist = this.subjectCodeNotDuplicate.some(code =>
            code.startsWith(inputCodeNo)
          );

          if (isExist) {
            const array = this.subjectCodeNotDuplicate.filter(code => {
              const regex = new RegExp(`^${inputCodeNo}`);
              return regex.test(code);
            });

            this.tempSubjectNoObj = array;
          } else {
            this.tempSubjectNoObj = [];
          }
        };

        this.fnUpdateCodeAry = debounce(fnPrint, 300);
      }

      this.fnUpdateCodeAry(codeNo);
      */
    },
  },
  created() {},
  async mounted() {
    if (this.remark.length !== 0) {
      this.tempRemark = this.remark;
    }

    if (this.data.length > 0) {
      await this.queryDetailedData({
        Year: this.data[0].ie_year,
        Month: this.data[0].ie_mon,
        Day: this.data[0].ie_day,
        FamNo: this.data[0].fam_no
      });
    }
  },
  computed: {
    ...mapState([`paramArray`, `subjectArray`]),
    subjectWithDEFFG() {
      return this.subjectArray.filter(obj => obj.def_fg.length !== 0);
    },
    subjectCodeNotDuplicate() {
      return this.subjectArray
        .map(obj => obj.code_no)
        .reduce((tempArray, code) => {
          if (tempArray.includes(code) === false) {
            tempArray.push(code);
          }

          return tempArray;
        }, []);
    },
    familyNo() {
      if (this.data.length > 0) {
        return this.data[0].fam_no;
      }

      const { FamNo } = this.queryObject;
      if (FamNo) {
        return FamNo;
      }

      return ``;
    },
    dataDate() {
      if (this.data.length > 0 || this.items.length > 0) {
        const { ie_year, ie_mon, ie_day } = this.data[0] || this.items[0];
        return `${ie_year}年${ie_mon}月${ie_day}日`;
      }

      const { Year, Month } = this.queryObject;
      if (Year && Month) {
        return `${Year}年${Month}月`;
      }

      return ``;
    },
    totalCost() {
      if (this.items.length > 0) {
        return this.items.reduce((totalCost, itemObj) => {
          const cost = parseInt(itemObj.code_amt, 10);
          if (!cost) {
            return totalCost;
          }

          totalCost += cost;
          return totalCost;
        }, 0);
      }

      return 0;
    },
    isNeedSelectDay() {
      if (this.data.length > 0) {
        return false;
      }

      const { Year, Month, FamNo } = this.queryObject;
      if (Year && Month && FamNo) {
        return true;
      }

      return false;
    },
    placeList() {
      return this.paramArray
        .filter(obj => obj.par_typ === `A`)
        .map(obj => obj.par_no)
        .sort((aNo, bNo) => {
          const a = parseInt(aNo, 10);
          const b = parseInt(bNo, 10);
          if (a > b) {
            return 1;
          }

          if (a < b) {
            return -1;
          }
        });
    },
  },
  watch: {
    queryObject: {
      async handler(value) {
        await this.queryDetailedData(value);
      },
      deep: true,
    },
    items: {
      handler() {
        this.checkIsEnableSave();
      },
      deep: true,
    },
  },
  /* eslint-disable  */
};
</script>

<style scoped>
#mainPage {
  text-align: left;
}
</style>
