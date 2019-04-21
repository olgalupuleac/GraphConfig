using System.Collections.Generic;
using GraphConfiguration;
using GraphConfiguration.GraphElementIdentifier;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphConfigurationTest
{
    [TestClass]
    public class IdentifierTest
    {
        [TestMethod]
        public void Substitute()
        {
            ScalarId a = new ScalarId("a", 1);
            ScalarId b = new ScalarId("b", 10);

            Identifier identifier = new Identifier(a, b);
            Assert.AreEqual("g[10].size() == 1", identifier.Substitute("g[__b__].size() == __a__"));
        }

        [TestMethod]
        public void AllIdentifiersOneD()
        {
            var res = Identifier.GetAllIdentifiersInRange(
                new List<ScalarRange>() {new ScalarRange("a", 0, 3)});
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual("a$" + i, res[i].Id());
            }
        }

        [TestMethod]
        public void AllIdentifiersTwoD()
        {
            var list = new List<ScalarRange>
            {
                new ScalarRange("a", 0, 3), new ScalarRange("b", 2, 10)
            };
            var res = Identifier.GetAllIdentifiersInRange(list);
            int currentIndexInList = 0;
            for (int aIndex = 0; aIndex < 3; aIndex++)
            {
                for (int bIndex = 2; bIndex < 10; bIndex++)
                {
                    Assert.AreEqual(2, res[currentIndexInList].ScalarIds.Count);
                    Assert.AreEqual("a", res[currentIndexInList].ScalarIds[0].Name);
                    Assert.AreEqual(aIndex, res[currentIndexInList].ScalarIds[0].Value);
                    Assert.AreEqual("b", res[currentIndexInList].ScalarIds[1].Name);
                    Assert.AreEqual(bIndex, res[currentIndexInList].ScalarIds[1].Value);
                    currentIndexInList++;
                }
            }

            Assert.AreEqual(currentIndexInList, res.Count);
        }

        [TestMethod]
        public void Id()
        {
            ScalarId a = new ScalarId("a", 1);
            ScalarId b = new ScalarId("b", 10);
            Identifier identifier = new Identifier(a, b);
            Assert.AreEqual("a$1#b$10", identifier.Id());
        }
    }
}