export const incomeDataModel = [
  {
    key: `date`,
    text: `年月`,
    type: {
      year: `select`,
      month: `select`,
    },
    value: {
      year: 108,
      month: 7,
    },
    source: {
      year: [106, 107, 108].map(year => {
        return {
          value: year,
          text: `${year}年`,
        };
      }),
      month: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12].map(month => {
        return {
          value: month,
          text: `${month}月`,
        };
      }),
    },
    valid: {
      year: () => {
        return true;
      },
      month: () => {
        return true;
      },
    },
    required: true,
  },
  {
    key: `port`,
    text: `戶號`,
    type: {
      start: `number`,
      end: `number`,
    },
    value: {
      start: ``,
      end: ``,
    },
    source: {
      start: null,
      end: null,
    },
    valid: {
      start: () => {
        return true;
      },
      end: () => {
        return true;
      },
    },
  },
  {
    key: `loginman`,
    text: `登錄人員`,
    type: {
      id: `number`,
    },
    value: {
      id: ``,
    },
    source: {
      id: null,
    },
    valid: {
      id: () => {
        return true;
      },
    },
  },
  {
    key: `reviewman`,
    text: `審核人員`,
    type: {
      id: `number`,
    },
    value: {
      id: ``,
    },
    source: {
      id: null,
    },
    valid: {
      id: () => {
        return true;
      },
    },
  },
  {
    key: `status`,
    text: `資料狀態`,
    type: {
      code: `select`,
    },
    value: {
      code: 1,
    },
    source: {
      code: [
        {
          value: 1,
          text: `處理中`,
        },
        {
          value: 2,
          text: `已登錄`,
        },
        {
          value: 3,
          text: `已審核`,
        },
      ],
    },
    valid: {
      code: () => {
        return true;
      },
    },
  },
];

export const subjectModel = [
  {
    key: `subjectCode`,
    text: `科目代碼`,
    type: {
      code_no: `text`,
    },
    value: {
      code_no: ``,
    },
    source: {
      code_no: null,
    },
    valid: {
      code_no: () => {
        return true;
      },
    },
  },
  {
    key: `subjectName`,
    text: `科目名稱`,
    type: {
      code_name: `text`,
    },
    value: {
      code_name: ``,
    },
    source: {
      code_name: null,
    },
    valid: {
      code_name: () => {
        return true;
      },
    },
  },
];

export const detailedModel = [
  {
    key: `date`,
    text: `年月`,
    type: {
      year: `select`,
      month: `select`,
    },
    value: {
      year: 108,
      month: 7,
    },
    source: {
      year: [106, 107, 108].map(year => {
        return {
          value: year,
          text: `${year}年`,
        };
      }),
      month: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12].map(month => {
        return {
          value: month,
          text: `${month}月`,
        };
      }),
    },
    valid: {
      year: value => {
        return value > 0 || value.length > 0;
      },
      month: value => {
        return value > 0 || value.length > 0;
      },
    },
    required: true,
  },
  {
    key: `port`,
    text: `戶號`,
    type: {
      num: `text`,
    },
    value: {
      num: ``,
    },
    source: {
      num: {
        type: `dynamic`,
        api: `myFamNo`,
      },
    },
    valid: {
      num: value => {
        return value > 0 || value.length > 0;
      },
    },
    required: true,
  },
  {
    key: `duration`,
    text: `期間`,
    type: {
      start: `text`,
      end: `text`,
    },
    value: {
      start: ``,
      end: ``,
    },
    source: {
      start: [
        `00`,
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
        `13`,
        `14`,
        `15`,
        `16`,
        `17`,
        `18`,
        `19`,
        `20`,
        `21`,
        `22`,
        `23`,
        `24`,
        `25`,
        `26`,
        `27`,
        `28`,
        `29`,
        `30`,
        `31`,
      ],
      end: [
        `00`,
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
        `13`,
        `14`,
        `15`,
        `16`,
        `17`,
        `18`,
        `19`,
        `20`,
        `21`,
        `22`,
        `23`,
        `24`,
        `25`,
        `26`,
        `27`,
        `28`,
        `29`,
        `30`,
        `31`,
      ],
    },
    valid: {
      start: () => {
        return true;
      },
      end: value => {
        return value > 0;
      },
    },
  },
  {
    key: `subjectCode`,
    text: `科目代碼`,
    type: {
      code_no: `text`,
    },
    value: {
      code_no: ``,
    },
    source: {
      code_no: null,
    },
    valid: {
      code_no: value => {
        return value > 0;
      },
    },
  },
  {
    key: `subjectName`,
    text: `科目名稱`,
    type: {
      code_name: `text`,
    },
    value: {
      code_name: ``,
    },
    source: {
      code_name: null,
    },
    valid: {
      code_name: value => {
        return value.length > 0;
      },
    },
  },
  {
    key: `dataChecker`,
    text: `資料檢查`,
    type: {
      checker: `select`,
    },
    value: {
      checker: null,
    },
    source: {
      checker: [
        {
          value: null,
          text: `無`,
        },
        {
          value: 0,
          text: `無支出日期`,
        },
        {
          value: 1,
          text: `每日支出合計金額不符`,
        },
      ],
    },
    valid: {
      checker: () => {
        return true;
      },
    },
  },
];
