// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Softeq.NetKit.Chat.Tests.Integration.Utility
{
    public class AlphabeticalOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            var testCasesList = testCases.ToList();
            testCasesList.Sort((one, two) => StringComparer.OrdinalIgnoreCase.Compare(one.TestMethod.Method.Name, two.TestMethod.Method.Name));
            return testCasesList;
        }
    }
}