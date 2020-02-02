using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Verifier;
using Xunit;

namespace VerifierTests
{
	public class Tests
	{
		[Theory]
		[InlineData("TrivialBeatable", true)]
		[InlineData("EasyBeatable", true)]
		[InlineData("MediumBeatable", true)]
		[InlineData("HardBeatable", true)]
		[InlineData("ResourceSuccess", true)]
		[InlineData("ResourceImpossible", false)]
		[InlineData("TrivialImpossible", false)]
		[InlineData("EasyImpossible", false)]
		[InlineData("HardImpossible", false)]
		public void TestBeatable(string mapId, bool expected)
		{
			//Arrange
			var resourceReader = new System.IO.StreamReader(new System.IO.MemoryStream(Properties.Resources.TestLogic));
			var data = JsonConvert.DeserializeObject<Common.SaveData.SaveData>(resourceReader.ReadToEnd());

			var randomMap = TestData.Data[mapId];

			var traverser = new NodeTraverser();

			// Act
			var result = traverser.VerifyBeatable(data, randomMap);

			// Assert
			Assert.Equal(expected, result);
		}
	}
}
