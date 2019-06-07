using System;
using System.Collections.Generic;
using System.Linq;

namespace ManyIntervalTrees
{
    public struct CenteredIntervalNode<T> {
        public CenteredIntervalNode(Func<T,long> getStart, Func<T, long> getEnd, long point, IEnumerable<T> nodes) {
          StartNodes = nodes.OrderBy(n=>getStart(n)).ToArray();
          EndNodes = nodes.OrderByDescending(n=>getEnd(n)).ToArray();
          Point = point;
          IsInitialized = true;
        }

        public bool IsInitialized;
        public T[] StartNodes;
        public T[] EndNodes;
        public long Point;
    }
}
