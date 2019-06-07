using ExpressiveAssertions;
using ExpressiveLogging;
using ExpressiveLogging.CompositeLogging;
using ExpressiveLogging.ConsoleLogging;
using ExpressiveLogging.Filtering;
using ExpressiveLogging.StreamFormatters;

namespace ManyIntervalTrees.Tests
{
  public class TestBase {
    public ILogStream _log;
    public IAssertionTool _assert;

    public TestBase() {
      _log = new ExceptionHandlingLogStream(new UniquenessCodeGeneratorLogStream(new ExceptionFormatterLogStream(new DefaultTextLogStreamFormatter(
                CompositeLogStream.Create(
                    // Filter all errors and alerts to stderr
                    FilterManager.CreateStream(
                        new StderrLogStream(),
                        FilterManager.CreateFilter(lt=>false, lt=>false,lt=>false, lt=>false, null, null, lt=>false, (lt, ct)=>false)
                    ),
                    // everything else to stdout
                    FilterManager.CreateStream(
                        new StdoutLogStream(),
                        FilterManager.CreateFilter(null, null, null, null, lt=>false, lt=>false, null, null)
                    )
                )
            ))));

      _assert = ExpressiveAssertions.Tooling.ShortAssertionRendererTool.Create(
          ExpressiveAssertions.MSTest.MSTestAssertionTool.Create()
      );
    }
  }
}
