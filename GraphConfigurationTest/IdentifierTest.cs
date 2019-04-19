using System.Collections.Generic;
using GraphConfiguration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphConfigurationTest
{
    [TestClass]
    public class IdentifierTest
    {
        [TestMethod]
        public void Substitute()
        {
            SingleIdentifier a = new SingleIdentifier {Name = "a", Value = 1};
            SingleIdentifier b = new SingleIdentifier {Name = "b", Value = 10};

            Identifier identifier = new Identifier(a, b);
            Assert.AreEqual("g[10].size() == 1", identifier.Substitute("g[__b__].size() == __a__"));
        }

        [TestMethod]
        public void SubstituteWithFunctionName()
        {
            SingleIdentifier a = new SingleIdentifier {Name = "a", Value = 1};
            SingleIdentifier b = new SingleIdentifier {Name = "b", Value = 10};

            Identifier identifier = new Identifier(a, b);
            Assert.AreEqual("\"main\" == \"dfs\" && p[1] == 10",
                identifier.Substitute("\"__CURRENT_FUNCTION__\" == \"dfs\" && p[__a__] == __b__", "main"));
        }

        [TestMethod]
        public void AllIdentifiersOneD()
        {
            var res = Identifier.GetAllIdentifiersInRange(
                new List<SingleIdentifierRange>() {new SingleIdentifierRange("a", 0, 3)});
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(i.ToString(), res[i].Id());
            }
        }

        [TestMethod]
        public void AllIdentifiersTwoD()
        {
            var list = new List<SingleIdentifierRange>
            {
                new SingleIdentifierRange("a", 0, 3), new SingleIdentifierRange("b", 2, 10)
            };
            var res = Identifier.GetAllIdentifiersInRange(list);
            int currentIndexInList = 0;
            for (int aIndex = 0; aIndex < 3; aIndex++)
            {
                for (int bIndex = 2; bIndex < 10; bIndex++)
                {
                    Assert.AreEqual(2, res[currentIndexInList].SingleIdentifiers.Count);
                    Assert.AreEqual("a", res[currentIndexInList].SingleIdentifiers[0].Name);
                    Assert.AreEqual(aIndex, res[currentIndexInList].SingleIdentifiers[0].Value);
                    Assert.AreEqual("b", res[currentIndexInList].SingleIdentifiers[1].Name);
                    Assert.AreEqual(bIndex, res[currentIndexInList].SingleIdentifiers[1].Value);
                    currentIndexInList++;
                }
            }
            Assert.AreEqual(currentIndexInList, res.Count);
        }
    }
}