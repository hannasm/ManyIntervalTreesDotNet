using System;

namespace ManyIntervalTrees
{
    public class InvalidIntervalException : Exception {
    public InvalidIntervalException(string msg) : base(msg)
    {}
  }
}
