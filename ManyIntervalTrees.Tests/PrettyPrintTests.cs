using System.Collections.Generic;
using ExpressiveLogging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManyIntervalTrees.Tests
{
    [TestClass]
    public class PrettyPrintTests : TestBase
    {
      static readonly ILogToken _lt = LogManager.GetToken();

        public class Interval {
          public long Left;
          public long Right;

          public Interval(long left, long right) {
            Left = left;
            Right = right;
          }
        }

        [TestMethod]
        public void Test001()
        {
          var intervals = new [] {
            new Interval(5,10),
            new Interval(1,3),
            new Interval(15, 20),
            new Interval(30,31),
            new Interval(7, 16)
          };

          var tree = new ReadOnlyCenteredIntervalTree<Interval>(intervals, c=>c.Left, c=>c.Right, c=>(c.Left + c.Right)/2);

          _log.Audit(_lt, m=>m(tree.PrettyPrintTree()));

        }

        [TestMethod]
        public void Test002()
        {
          var count = 100;

          var intervals = new Interval[count];
          for (int i = 0 ; i < count; i++) {
            intervals[i] = new Interval(i*5, i*5 + 3);
          }
          var tree = new ReadOnlyCenteredIntervalTree<Interval>(intervals, c=>c.Left, c=>c.Right, c=>(c.Left + c.Right)/2);

          var prettyTree = tree.PrettyPrintTree();
          _log.Audit(_lt, m=>m(prettyTree));
        }

        [TestMethod]
        public void Test003()
        {
          var count = 100;

          var intervals = new Interval[count];
          for (int i = 0 ; i < count; i++) {
            intervals[i] = new Interval(i*5, i*5 + 7);
          }
          var tree = new ReadOnlyCenteredIntervalTree<Interval>(intervals, c=>c.Left, c=>c.Right, c=>(c.Left + c.Right)/2);

          var prettyTree = tree.PrettyPrintTree();
          _log.Audit(_lt, m=>m(prettyTree));
        }

        [TestMethod]
        public void Test004()
        {
          var count = 4;
          var intervals = new Interval[count];
          intervals[0] = new Interval(1, 10);
          intervals[1] = new Interval(2, 8);
          intervals[2] = new Interval(3,7);
          intervals[3] = new Interval(4, 6);

          var tree = new ReadOnlyCenteredIntervalTree<Interval>(intervals, c=>c.Left, c=>c.Right, c=>(c.Left + c.Right)/2);
          var prettyTree = tree.PrettyPrintTree();

          _log.Audit(_lt, m=>m(prettyTree));
        }

        [TestMethod]
        public void Test005()
        {
          var count = 4;
          var intervals = new Interval[count];
          intervals[0] = new Interval(1, 2);
          intervals[1] = new Interval(2, 8);
          intervals[2] = new Interval(3,7);
          intervals[3] = new Interval(4, 6);

          var tree = new ReadOnlyCenteredIntervalTree<Interval>(intervals, c=>c.Left, c=>c.Right, c=>(c.Left + c.Right)/2);
          var prettyTree = tree.PrettyPrintTree();

          _log.Audit(_lt, m=>m(prettyTree));
        }
    }
}
