using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace USD.NET.Tests
{
    class UsdTests
    {
        readonly List<string> m_filesToDelete = new List<string>();

        [SetUp]
        public void SetUp()
        {
            InitUsd.Initialize();
        }

        protected string CreateTmpUsdFile(string fileName)
        {
            var filePath = Path.Combine(UnityEngine.Application.dataPath, fileName);
            var scene = Scene.Create(filePath);
            scene.Save();
            scene.Close();
            m_filesToDelete.Add(filePath);
            return filePath;
        }

        [TearDown]
        public void DeleteTmpFiles()
        {
            foreach (var file in m_filesToDelete)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }

            m_filesToDelete.Clear();
        }

        protected static void WriteAndRead<T>(ref T inputSample, ref T outputSample)
            where T : SampleBase
        {
            string filename = GetTempFile();
            var scene = Scene.Create(filename);
            scene.Write("/Foo", inputSample);

            scene.Save();
            scene.Close();

            var scene2 = Scene.Open(filename);
            scene2.Read("/Foo", outputSample);
            scene2.Close();

            File.Delete(filename);
        }

        protected static string GetTempFile(string extension = "usd")
        {
            return Path.ChangeExtension(Path.GetTempFileName(), extension);
        }

        protected static void AssertEqual<T>(T[] first, T[] second)
        {
            if (first == null && second == null)
            {
                return;
            }

            Assert.AreEqual(first.Length, second.Length, "Length of arrays do not match");

            for (int i = 0; i < first.Length; i++)
            {
                Assert.AreEqual(first[i], second[i]);
            }
        }

        static protected void AssertEqual(Array first, Array second)
        {
            if (first == null && second == null)
            {
                return;
            }

            Assert.AreEqual(first.Length, second.Length, "Length of arrays do not match");

            for (int i = 0; i < first.Length; i++)
            {
                Assert.AreEqual(first.GetValue(i), second.GetValue(i));
            }
        }

        static protected void AssertEqual(IList first, IList second)
        {
            if (first == null && second == null)
            {
                return;
            }

            if (first.Count != second.Count)
            {
                throw new Exception("Length of arrays do not match");
            }

            for (int i = 0; i < first.Count; i++)
            {
                Assert.AreEqual(first[i], second[i]);
            }
        }

        static protected void AssertEqual(IDictionary first, IDictionary second)
        {
            if (first == null && second == null)
            {
                return;
            }

            if (first.Count != second.Count)
            {
                throw new Exception("Length of arrays do not match");
            }

            foreach (System.Collections.DictionaryEntry kvp in first)
            {
                if (!second.Contains(kvp.Key))
                {
                    throw new Exception("Key in first not found in second: " + kvp.Key);
                }

                Assert.AreEqual(kvp.Value, second[kvp.Key]);
            }
        }

        static protected void AssertEqual<T>(T first, T second)
        {
            if (first == null && second == null)
            {
                return;
            }

            if ((first as IList) != null)
            {
                AssertEqual(first as IList, second as IList);
            }
            else if ((first as IDictionary) != null)
            {
                AssertEqual(first as IDictionary, second as IDictionary);
            }
            else if ((first as Array)?.Length != 0)
            {
                AssertEqual(first as Array, second as Array);
            }
            else if (!first.Equals(second))
            {
                throw new Exception("Values do not match for " + typeof(T).Name);
            }
        }
    }
}