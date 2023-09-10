﻿using System.Collections.Immutable;

internal class Program
{
    private static void Main(string[] args)
    {
        var graph = new Graph(new int[] { 1, 4, 2, 3, 5, 6 }, 
            new Edge[] { 
                new Edge(1, 4),
                new Edge(4,1),
                new Edge(1,2),
                new Edge(4,3),
                new Edge(3,2),
                new Edge(2,5),
                new Edge(5,3),
                new Edge(5,6),
            });
        var a = graph.TopologicalDecompose();
        a.Print();
    }
}

internal struct Edge
{
    public Edge(int outV, int inV)
    {
        this.outV = outV;
        this.inV = inV;
    }

    public int outV { get; init; }
    public int inV { get; init; }
}


internal class Graph
{
    public void Print()
    {
        foreach (var vert in Verticles)
        {
            var outSeq = Edges.Where(e => e.outV == vert).Select(e => e.inV);
            Console.WriteLine($"G({vert}) = {(outSeq.Any() ? string.Join(", ", outSeq) : 0)}");
        }
        Console.ReadLine();
    }

    private class Subgraph
    {
        public int Index => Verticles.First();

        public IEnumerable<int> Verticles { get; init; }

        public Subgraph(IEnumerable<int> verticles)
        {
            Verticles = verticles;
        }
    }
    public Graph(IEnumerable<int> verticles, IEnumerable<Edge> edges)
    {
        Verticles = verticles.ToImmutableSortedSet();
        Edges = edges;
    }

    public IEnumerable<Edge> Edges { get; private set; }

    public IEnumerable<int> Verticles { get; private set; }

    public Graph TopologicalDecompose()
    {
        var Subgraphes = new List<Subgraph>();

        var availableVerticles = Verticles;

        Subgraph currentSubgraph;

        while (availableVerticles.Any())
        {
            var verticle = availableVerticles.First();
            var R = GetR(verticle);
            var Q = GetQ(verticle);
            currentSubgraph = new Subgraph(R.Intersect(Q));
            if (!currentSubgraph.Verticles.Any())
            {
                var lastSubgraph = new Subgraph(availableVerticles);
                Subgraphes.Add(lastSubgraph);
                break;
            }
            Subgraphes.Add(currentSubgraph);
            availableVerticles = availableVerticles.Except(currentSubgraph.Verticles);
        }

        List<Edge> newEdges = new List<Edge>();
        foreach (var subgraph in Subgraphes)
        {
            var outEdges = Edges.Where(e => subgraph.Verticles.Contains(e.outV) && !subgraph.Verticles.Contains(e.inV));
            var otherSubgraphs = Subgraphes.Except(new Subgraph[] { subgraph });
            foreach (var edge in outEdges)
            {
                var inIndex = otherSubgraphs.FirstOrDefault(s => s.Verticles.Contains(edge.inV))?.Index;
                if (inIndex.HasValue)
                {
                    var newEdge = new Edge(subgraph.Index, inIndex.Value);
                    if (!newEdges.Contains(newEdge))
                    {
                        newEdges.Add(newEdge);
                    }
                }
            }
        }

        return new Graph(Subgraphes.Select(s => s.Index), newEdges);
    }

    private IEnumerable<int> GetR(int v)
    {
        var res = new List<int>() { v };

        IEnumerable<int> achievable;
        for (int i = 0; i < res.Count; i++)
        {
            List<int> alreadyAdded = new List<int>();
            do
            {
                achievable = Edges.Where(edge => edge.outV == res[i]).Select(edge => edge.inV).Except(alreadyAdded).ToArray();
                res = res.Union(achievable).ToList();
                alreadyAdded.AddRange(achievable);
            } while (achievable.Any());
        }

        return res.Distinct();
    }

    private IEnumerable<int> GetQ(int v)
    {
        Stack<int> stack = new Stack<int>();
        List<int> res = new List<int>();
        stack.Push(v);
        
        while (stack.TryPop(out int vert)) 
        {
            foreach (var outVert in Edges.Where(e => e.inV == vert).Select(e => e.outV).Except(res))
            {
                if (!stack.Contains(outVert))
                {
                    stack.Push(outVert);
                    res.Add(outVert);
                }
            }
        }
        return res;
    }
}