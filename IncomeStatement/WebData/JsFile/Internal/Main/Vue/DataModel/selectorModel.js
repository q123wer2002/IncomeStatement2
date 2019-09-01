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
      code_no: `number`,
    },
    value: {
      code_no: 0,
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
      num: `number`,
    },
    value: {
      num: 0,
    },
    source: {
      num: null,
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
    key: `subjectCode`,
    text: `科目代碼`,
    type: {
      code_no: `number`,
    },
    value: {
      code_no: 0,
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
    key: `checkData`,
    text: `資料檢查`,
    type: {
      checker: `select`,
    },
    value: {
      checker: ``,
    },
    source: {
      checker: [],
    },
    valid: {
      checker: value => {
        return value.length > 0;
      },
    },
  },
  {
    key: `noPaymentDate`,
    text: `無支出日期`,
    type: {
      date: `text`,
    },
    value: {
      date: ``,
    },
    source: {
      date: null,
    },
    valid: {
      date: value => {
        return value.length > 0;
      },
    },
  },
];
