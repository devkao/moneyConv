using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoneyConv.Common.Interfaces;

namespace MoneyConv.Implementation.V1.Tests
{
    [TestClass()]
    public class MoneyConvImplV1Tests
    {
        private readonly IMoneyConvV1 _testObj = new MoneyConvImplV1();
        [TestMethod()]
        public void ConvertNumberToWordsTest_0()
        {
            var input = "0";
            var expectedResult = "zero dollars";
            var res = _testObj.ConvertNumberToWords(input);
            Assert.IsTrue(res.Success);

            Assert.AreEqual(expectedResult, res.Value);
        }
        [TestMethod()]
        public void ConvertNumberToWordsTest_1()
        {
            var input = "1";
            var expectedResult = "one dollar";
            var res = _testObj.ConvertNumberToWords(input); 
            Assert.IsTrue(res.Success);

            Assert.AreEqual(expectedResult, res.Value);
        }
        [TestMethod()]
        public void ConvertNumberToWordsTest_25_1()
        {
            var input = "25,1";
            var expectedResult = "twenty-five dollars and ten cents";
            var res = _testObj.ConvertNumberToWords(input);
            Assert.IsTrue(res.Success);

            Assert.AreEqual(expectedResult, res.Value);
        }
        [TestMethod()]
        public void ConvertNumberToWordsTest_0_01()
        {
            var input = "0,01";
            var expectedResult = "zero dollars and one cent";
            var res = _testObj.ConvertNumberToWords(input);
            Assert.IsTrue(res.Success);

            Assert.AreEqual(expectedResult, res.Value);
        }
        [TestMethod()]
        public void ConvertNumberToWordsTest_45100()
        {
            var input = "45 100";
            var expectedResult = "forty-five thousand one hundred dollars";
            var res = _testObj.ConvertNumberToWords(input);
            Assert.IsTrue(res.Success);

            Assert.AreEqual(expectedResult, res.Value);
        }
        [TestMethod()]
        public void ConvertNumberToWordsTest_999999999_99()
        {
            var input = "999 999 999,99";
            var expectedResult = "nine hundred ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and ninety-nine cents";
            var res = _testObj.ConvertNumberToWords(input);
            Assert.IsTrue(res.Success);

            Assert.AreEqual(expectedResult, res.Value);
        }


        [TestMethod()]
        public void ConvertNumberToWordsTest_DEBUG()
        {
            var input = "87 512,1";
            var expectedResult = "eighty-seven thousand five hundred twelve dollars and ten cents";
            var res = _testObj.ConvertNumberToWords(input);
            Assert.IsTrue(res.Success);

            Assert.AreEqual(expectedResult, res.Value);
        }

        [TestMethod()]
        public void ConvertNumberToWordsFail_char()
        {
            var input = "87a 512,1";
            
            var res = _testObj.ConvertNumberToWords(input);
            Assert.IsFalse(res.Success);            
        }

        [TestMethod()]
        public void ConvertNumberToWordsFail_OutOfRangePreComma()
        {
            var input = "5154 812 512,12";

            var res = _testObj.ConvertNumberToWords(input);
            Assert.IsFalse(res.Success);
        }
        [TestMethod()]
        public void ConvertNumberToWordsFail_OutOfRangeAfterComma()
        {
            var input = "812 512,121";

            var res = _testObj.ConvertNumberToWords(input);
            Assert.IsFalse(res.Success);
        }
    }
}