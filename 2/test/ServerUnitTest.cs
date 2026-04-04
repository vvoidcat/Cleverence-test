namespace Locks.Test;

// для изоляции тест-кейсов, т.к. тестируется статический класс
[CollectionDefinition("ServerUnitTests", DisableParallelization = true)]
public class ServerTestsCollection { }

[Collection("ServerUnitTests")]
public class ServerUnitTest
{
	public ServerUnitTest()
	{
		Server.Reset();
	}

	[Theory]
	[InlineData(50, 0)]
	[InlineData(1000, 0)]
	[InlineData(-13, 0)]
	public void CheckReset(int addVal, int expected)
	{
		Server.AddToCount(addVal);
		Server.Reset();

		Assert.Equal(expected, Server.GetCount());
	}

	[Theory]
	[InlineData(1, 1)]
	[InlineData(50, 50)]
	[InlineData(-100, -100)]
	public void CheckAddCount(int addVal, int expected)
	{
		Server.AddToCount(addVal);

		Assert.Equal(expected, Server.GetCount());
	}

	[Fact]
	public void CheckParallel_NoCorruptedAdditions()
	{
		int threadsCount = 100;
		int addsPerThread = 10000;
		int addVal = 2;
		int expectedTotal = threadsCount * addsPerThread * addVal;

		var options = new ParallelOptions
		{
			MaxDegreeOfParallelism = Environment.ProcessorCount
		};

		Parallel.For(0, threadsCount, options, _ =>
		{
			for (int j = 0; j < addsPerThread; j++)
			{
				Server.AddToCount(addVal);
			}
		});

		Assert.Equal(expectedTotal, Server.GetCount());
	}
}

