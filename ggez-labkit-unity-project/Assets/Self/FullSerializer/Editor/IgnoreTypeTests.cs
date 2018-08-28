﻿using System.Collections.Generic;
using NUnit.Framework;

namespace GGEZ.FullSerializer.Tests {
    public class IgnoreTypeTests {
        [fsIgnore]
        private class IgnoredClass {
        }
        private class NotIgnoredClass {
        }
        [fsIgnore]
        private struct IgnoredStruct {
        }
        private struct NotIgnoredStruct {
        }

#pragma warning disable 0649 // X is never assigned to, and will always have its default value `null`
        private class Model {
            public IgnoredClass IgnoredClass;
            public NotIgnoredClass NotIgnoredClass;
            public IgnoredStruct IgnoredStruct;
            public NotIgnoredStruct NotIgnoredStruct;
        }
#pragma warning restore 0649

        [Test]
        public void TestSerializeReadOnlyProperty() {
            var model = new Model();

            fsData data;

            var serializer = new fsSerializer();
            Assert.IsTrue(serializer.TrySerialize(model, out data).Succeeded);

            var expected = fsData.CreateDictionary();
            expected.AsDictionary["NotIgnoredClass"] = new fsData();
            expected.AsDictionary["NotIgnoredStruct"] = fsData.CreateDictionary();
            Assert.AreEqual(expected, data);
        }
    }
}