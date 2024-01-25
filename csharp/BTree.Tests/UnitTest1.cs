using Wjybxx.BTree;
using Wjybxx.BTree.Branch;

namespace BTree.Tests;

public class Tests
{
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void Test1() {
        int maskOfTask = TaskOverrides.maskOfTask(typeof(SingleRunningChildBranch<string>));
        Console.WriteLine(maskOfTask);
    }
}