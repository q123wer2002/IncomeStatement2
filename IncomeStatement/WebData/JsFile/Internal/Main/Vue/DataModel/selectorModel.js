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
  },
  {
    key: `port`,
    text: `戶號`,
    type: {
      start: `number`,
      end: `number`,
    },
    value: {
      start: 0,
      end: 0,
    },
    source: {
      start: null,
      end: null,
    },
    valid: {
      start: value => {
        return value > 0;
      },
      end: value => {
        return value > 0;
      },
    },
  },
  {
    key: `loginman`,
    text: `登入人員`,
    type: {
      id: `number`,
    },
    value: {
      id: 0,
    },
    source: {
      id: null,
    },
    valid: {
      id: value => {
        return value > 0;
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
      year: () => {
        return true;
      },
      month: () => {
        return true;
      },
    },
  },
  {
    key: `port`,
    text: `戶號`,
    type: {
      num: `text`,
    },
    value: {
      num: 0,
    },
    source: {
      num: {
        type: `dynamic`,
        api: `myFamNo`,
      },
    },
    valid: {
      num: value => {
        return value > 0;
      },
    },
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
];
