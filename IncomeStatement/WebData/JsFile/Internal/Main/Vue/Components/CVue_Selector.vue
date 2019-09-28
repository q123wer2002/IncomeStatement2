<template>
  <div id="mainPage">
    <b-card-header header-tag="header" class="p-1" role="tab">
      <b-button block href="#" v-b-toggle.accordion-1 variant="info" size="sm">
        展開/收合篩選器
      </b-button>
    </b-card-header>
    <b-collapse
      id="accordion-1"
      visible
      accordion="my-accordion"
      role="tabpanel"
    >
      <div class="filterItem" v-for="item in items" :key="item.key">
        <span v-if="item.required" style="color: red">*</span>
        <span>{{ item.text }}</span>
        <span v-for="(typeValue, typeKey) in item.type" :key="typeKey">
          <b-form-select
            v-if="typeValue === `select`"
            v-model="item.value[typeKey]"
            :options="item.source[typeKey]"
            class="d-inline w-25 h-10"
            size="sm"
          ></b-form-select>

          <template v-else>
            <b-form-input
              v-model="item.value[typeKey]"
              :type="typeValue"
              :list="typeKey"
              class="d-inline w-25 h-25"
              size="sm"
            ></b-form-input>
            <datalist :id="typeKey">
              <option v-for="obj in item.source[typeKey]" :key="obj.key">
                {{ obj }}
              </option>
            </datalist>
          </template>

          <span v-if="typeKey === `start`">~</span>
        </span>
      </div>

      <b-button-group id="btnSearch">
        <b-button
          variant="info"
          size="sm"
          v-if="isDetailed"
          :disabled="!isAddEnabled"
          @click="addDetails"
        >
          新增
        </b-button>
        <b-button
          variant="info"
          size="sm"
          v-if="isDataCheckBtn"
          @click="onCheckData"
        >
          資料檢誤
        </b-button>
        <b-button
          variant="info"
          size="sm"
          @click="search"
          :disabled="!isBtnSearchDisabled"
        >
          查詢
        </b-button>
      </b-button-group>
    </b-collapse>
  </div>
</template>

<script>
export default {
  /* eslint-disable no-undef, no-param-reassign */
  name: 'Selector',
  components: {},
  props: {
    filterModel: {
      type: Array,
      required: true,
    },
    isPreLoadData: {
      type: Boolean,
      required: false,
      default: false,
    },
    isDetailed: {
      type: Boolean,
      required: false,
      default: false,
    },
    isDataCheckBtn: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      isAddEnabled: false,
      items: [],
    };
  },
  methods: {
    async preProcessModel() {
      await this.filterModel.forEach(async obj => {
        // set detault as pre month
        if (obj.key === `date`) {
          const dtcurrent = new Date();
          dtcurrent.setMonth(dtcurrent.getMonth() - 1);
          obj.value.year = dtcurrent.getFullYear() - 1911;
          obj.value.month = dtcurrent.getMonth() + 1;
        }

        // check dynamic options
        Object.keys(obj.source).forEach(async tempObj => {
          if (
            !obj.source[tempObj] ||
            !obj.source[tempObj].type ||
            obj.source[tempObj].type !== `dynamic`
          ) {
            return;
          }

          // dynamic
          const { api, payload, key } = obj.source[tempObj];
          const resObject = await this.mixinCallBackService(
            this.mixinBackendService[api],
            payload
          );

          if (resObject.status === this.mixinBackendErrorCode.success) {
            obj.source[tempObj] = resObject.data.map(apiObj => apiObj[key]);
          }
        });
      });
    },
    initialFilterItem() {
      // key, text, type, value, source
      this.items = this.filterModel;
    },
    search() {
      const itemValueObj = this.items.reduce((tempObj, itemObj) => {
        tempObj[itemObj.key] = itemObj.value;
        return tempObj;
      }, {});

      // emit
      this.$emit(`search`, itemValueObj);
    },
    addDetails() {
      this.search();
      this.$emit(`additem`);
    },
    onCheckData() {
      this.search();
      this.$emit(`checkData`);
    },
  },
  created() {},
  async mounted() {
    await this.preProcessModel();

    // check filer model
    this.initialFilterItem();
  },
  computed: {
    isBtnSearchDisabled() {
      if (this.items.length === 0) {
        return false;
      }

      return this.items.some(itemObj => {
        return Object.keys(itemObj.value).every(key => {
          const value = itemObj.value[key];
          return itemObj.valid[key](value);
        });
      });
    },
  },
  watch: {
    items: {
      handler(newValue) {
        const requiredKeys = [`date`, `port`];
        this.isAddEnabled = newValue
          .filter(obj => requiredKeys.includes(obj.key))
          .every(obj => {
            return Object.keys(obj.value).every(subKey => {
              const value = obj.value[subKey];
              const fnValid = obj.valid[subKey];
              return fnValid(value);
            });
          });
      },
      deep: true,
    },
  },
  /* eslint-disable no-undef, no-param-reassign */
};
</script>

<style scoped>
#mainPage {
  position: relative;
  width: 85%;
  padding: 8px 16px;
  margin: 8px auto;
  border-bottom: 1px solid #61c7d0;
  font-size: 16px;
  overflow-y: hidden;
}
#hintTitle {
  text-align: left;
  font-weight: 900;
}
.filterItem {
  margin: 8px auto;
  text-align: left;
}
#btnSearch {
  float: left;
  margin-top: 8px;
}
</style>
