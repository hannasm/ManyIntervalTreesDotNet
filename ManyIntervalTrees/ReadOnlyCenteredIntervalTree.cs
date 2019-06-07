using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccurateIntegerLogarithm;

namespace ManyIntervalTrees
{
  public class ReadOnlyCenteredIntervalTree<T> {
    public Func<T, long> GetStart, GetEnd, GetCenter;

    CenteredIntervalNode<T>[] _nodes;

    public ReadOnlyCenteredIntervalTree(IEnumerable<T> nodes, Func<T, long> getStart, Func<T, long> getEnd, Func<T, long> getCenter)
    {
      GetStart = getStart;
      GetEnd = getEnd;
      GetCenter = getCenter;

      var ns = nodes.OrderBy(n=>getStart(n)).ToArray();
      var startNodes = ns;
      int[] counts = new int[ns.Length];
      _nodes = new CenteredIntervalNode<T>[ns.Length*2];
      for (int i = 0; i < ns.Length; i++) {
        var node = ns[i];

        var nodeStart = getStart(node);
        var nodeEnd = getEnd(node);
        if (nodeStart > nodeEnd) {
          throw new InvalidIntervalException("Invalid interval (" + nodeStart + "," + nodeEnd + "), end < start");
        }
        for (int j = i+1; j < ns.Length; j++) {
          var tn = ns[j];
          if (getStart(node) <= getEnd(tn) &&
              getStart(tn) <= getEnd(node)) {
            counts[i] += 1;
            counts[j] += 1;
          } else {
            break;
          }
        }
      }

      BuildTree(counts, startNodes, 0);
    }

    void BuildTree(Span<int> counts, Span<T> nodesStart, int pos) {
      if (counts.Length == 0 || nodesStart.Length == 0) { return; }
      Func<int,int> growthRate = dist=>2 * dist;
      var center = nodesStart.Length / 2;
      var bestIndex = center;
      var bestScore = counts[bestIndex] + 1;

      var cds = counts.ToArray().Select((c,i)=>new { index = i, count = c }).OrderBy(e=>e.count).ToArray();
      foreach (var n in cds) {
        // dont consider anythign with moire overlap than the center point
        if (n.count > counts[center]) { break; }

        var currentScore = n.count;
        currentScore += growthRate(Math.Abs(center - n.index));

        if (currentScore < bestScore) {
          bestScore = currentScore;
          bestIndex = n.index;
        }
      }

      var tree = this;
      var bestNode = nodesStart[bestIndex];

      int leftIndex = -1, rightIndex = -1;
      var contained = new List<T>();
      for (int i = 0; i < nodesStart.Length; i++) {
        var cur = nodesStart[i];
        if (GetStart(cur) <= GetEnd(bestNode) &&
            GetStart(bestNode) <= GetEnd(cur)) {
          if (contained.Count == 0) {
            leftIndex = i - 1;
          }
          contained.Add(cur);
        }
       // we are guaranteed to pass the best node in this search
       // we will add the best node when we reach it
       // once we find any node that starts after the best node, and is not contained within it, we are done
        else if (contained.Count > 0) {
          rightIndex = i;
          break;
        }
      }
      var best = new CenteredIntervalNode<T>(GetStart, GetEnd, GetCenter(bestNode), contained);
      _nodes[pos] = best;

      if (leftIndex >= 0) {
        var leftCounts = counts.Slice(0, leftIndex+1);
        var leftNodesStart = nodesStart.Slice(0, leftIndex+1);
        var leftPos = pos * 2 + 1;
        BuildTree(leftCounts, leftNodesStart, leftPos);
      }
      if (rightIndex >= 0) {
        var rightCounts = counts.Slice(rightIndex);
        var rightNodesStart = nodesStart.Slice(rightIndex);
        var rightPos = pos * 2 + 2;
        BuildTree(rightCounts, rightNodesStart, rightPos);
      }
    }

    public IEnumerable<T> Query(long point) {
      return Query(point, 0);
    }

    IEnumerable<T> Query(long point, int index) {
      var curNode = _nodes[index];
      if (!curNode.IsInitialized) { return Enumerable.Empty<T>(); }

      if (curNode.Point == point) {
        return curNode.StartNodes;
      }

      IEnumerable<T> targetNode;
      int nextIndex;
      if (curNode.Point > point) {
        targetNode = curNode.StartNodes;
        nextIndex = index * 2 + 1;
      } else {
        targetNode = curNode.EndNodes;
        nextIndex = index * 2 + 2;
      }

      var containedReuslt = curNode.StartNodes.TakeWhile(n=>GetStart(n) <= point && GetEnd(n) >= point);
      var childResult = Query(point, index * 2 + 1);

      return containedReuslt.Concat(childResult);
    }

    AccurateIntegerLogarithmTool logBase2 = new AccurateIntegerLogarithmTool(2);
    public string PrettyPrintTree() {
      var result = new StringBuilder();
      result.AppendFormat("Interval Tree (array={0}) (depth>={1})", _nodes.Length, ((int)logBase2.Log(_nodes.Length)));
      result.AppendLine();


      PrettyPrintTree(result, 0);

      return result.ToString();
    }

    void PrettyPrintTree(StringBuilder b, int pos) {
      if (pos >= _nodes.Length) { return; }
      var node = _nodes[pos];
      if (!node.IsInitialized) { return; }

      var indent = new string(' ', (int)logBase2.Log(pos+1) * 2);
      if (node.StartNodes.Length == 0 ||
        node.EndNodes.Length == 0) {
        b.AppendFormat("{0}NODE WITH NO CHILDREN ({1})", indent, node.Point);
        b.AppendLine();
        PrettyPrintTree(b, pos * 2 + 1);
        PrettyPrintTree(b, pos * 2 + 2);
        return;
      }

      var renderCols = 30M;
      var start = GetStart(node.StartNodes[0]);
      var end = GetEnd(node.EndNodes[0]);
      var range = end - start;
      var incr = range / renderCols;
      b.AppendFormat("{0}({1} <- {3} -> {2})", indent, start, end, node.Point);
      b.AppendLine();

      for (int i = 0; i < node.StartNodes.Length; i++) {
        var inner = node.StartNodes[i];
        var ist = GetStart(inner);
        var ied = GetEnd(inner);
        var i1 = new string(' ', (int)((ist - start) / incr));

        var len = (int)Math.Ceiling((ied - ist)/incr);
        var i2 = new string('o', len);
        var i3 = new string (' ', (int)Math.Max(renderCols - len - i1.Length, 0));
        b.AppendFormat("{0}{6}{1}{2}({3} <- {4} -> {5})", indent, i2, i3, ist, GetCenter(inner), ied, i1);
        b.AppendLine();
      }

      PrettyPrintTree(b, pos * 2 + 1);
      PrettyPrintTree(b, pos * 2 + 2);
    }
  }
}
