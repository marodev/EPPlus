﻿using EPPlusTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OfficeOpenXml.Core.Worksheet
{
    [TestClass]
    public class WorksheetRowsColumnsTests : TestBase
    {
        static ExcelPackage _pck;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _pck = OpenPackage("WorksheetRowCol.xlsx", true);
        }
        [ClassCleanup]
        public static void Cleanup()
        {
            SaveAndCleanup(_pck);
        }
        [TestMethod]
        public void ValidateRowsCollectionEnumeration()
        {
            var ws = _pck.Workbook.Worksheets.Add("Rows");

            ws.Cells["A1:A10"].FillNumber(1);

            int r = 2;
            foreach(var row in ws.Rows[2,10])
            {
                Assert.AreEqual(r++, row.StartRow);
            }
            Assert.AreEqual(11, r);
        }
        [TestMethod]
        public void ValidateRowsCollectionEnumerationEveryOther()
        {
            var ws = _pck.Workbook.Worksheets.Add("RowsEveryOther");

            ws.Cells["A2"].Value = 2;
            ws.Cells["A4"].Value = 4;
            ws.Cells["A6"].Value = 6;
            ws.Cells["A8"].Value = 8;
            ws.Cells["A10"].Value = 10;
            int r = 2;

            foreach (var row in ws.Rows[1, 10])
            {
                Assert.AreEqual(r, row.StartRow);
                r += 2;
            }
            Assert.AreEqual(12, r);
        }
        [TestMethod]
        public void ValidateRowsCollectionEnumerationNoRows()
        {
            var ws = _pck.Workbook.Worksheets.Add("NoRows");

            ws.Cells["A1"].Value = 1;
            ws.Cells["A11"].Value = 11;

            foreach (var row in ws.Rows[2, 10])
            {
                Assert.Fail("No rows should be in the Rows collection.");
            }
        }
        [TestMethod]
        public void ValidateRowsCollectionEnumerationNoIndexerParams()
        {
            var ws = _pck.Workbook.Worksheets.Add("RowsNoIndexerParams");

            ws.Cells["A2"].Value = 2;
            ws.Cells["A11"].Value = 11;
            var rows = 0;
            foreach (var row in ws.Rows)
            {
                if(row.StartRow!=2 && row.StartRow!=11)
                {
                    Assert.Fail("Unknown row in enumeration");
                }
                rows++;
            }
            Assert.AreEqual(2, rows);
        }
        [TestMethod]
        public void ValidateColumnsCollectionEnumeration()
        {
            var ws = _pck.Workbook.Worksheets.Add("Columns");

            ws.Cells["A1:K1"].FillNumber(1,1,eFillDirection.Row);

            int c = 2;
            foreach (var column in ws.Columns[2, 10])
            {
                Assert.AreEqual(c++, column.StartColumn);
            }
            Assert.AreEqual(11, c);
        }
        [TestMethod]
        public void ValidateColumnsCollectionEnumerationColumn3_5()
        {
            var ws = _pck.Workbook.Worksheets.Add("Columns");

            ws.Columns[3, 5].Width=25;

            int columns = 0;
            foreach (var column in ws.Columns[2, 10])
            {
                if(column.StartColumn < 3 || column.StartColumn > 5)
                {
                    Assert.Fail("Invalid columns detected in [Columns] collection");
                }
                columns++;
            }
            Assert.AreEqual(3, columns);
        }
    }
}