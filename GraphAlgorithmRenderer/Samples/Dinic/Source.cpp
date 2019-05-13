#include <vector>
#include <deque>
#include <memory.h>
#include <iostream>
#include <algorithm>
#include <cassert>

#pragma warning(disable : 4996) //_CRT_SECURE_NO_WARNINGS

using namespace std;

const int N = 500;
const int INF = 2e9;
int d[N], head[N], p[N];
int s, t, n;
vector<int> c[N];

struct edge
{
	int from, to, cap, flow;
};

vector<edge> edges;
vector<bool> flow_edges;
vector<bool> dfs_edges;

void add_edge(int a, int b, int cap)
{
	edge e1 = {a, b, cap, 0};
	edge e2 = {b, a, cap, 0};
	c[a].push_back(edges.size());
	edges.push_back(e1);
	c[b].push_back(edges.size());
	edges.push_back(e2);
}

bool bfs()
{
	deque<int> q;
	q.push_back(s);
	memset(d, -1, sizeof d);
	d[s] = 0;
	while (q.size() && d[t] == -1)
	{
		int v = q[0];
		for (int x : c[v])
		{
			edge& e = edges[x];
			if (d[e.to] == -1 && e.flow < e.cap)
			{
				q.push_back(e.to);
				d[e.to] = d[v] + 1;
			}
		}
		q.pop_front();
	}
	return d[t] != -1;
}


int dfs(int v, int flow)
{
	if (!flow || v == t)
	{
		return flow;
	}
	for (; head[v] < c[v].size(); head[v]++)
	{
		int id = c[v][head[v]];
		edge& e = edges[id];

		if (d[e.to] != d[v] + 1)
			continue;
		dfs_edges[(id / 2) * 2] = true;
		int pushed = dfs(e.to, min(flow, e.cap - e.flow));
		//dfs_edges.pop_back();
		if (pushed)
		{
			flow_edges[(id / 2) * 2] = true;
			e.flow += pushed;
			edges[id ^ 1].flow -= pushed;
			return pushed;
		}
	}
	return 0;
}

int dinic()
{
	int flow = 0;
	while (true)
	{
		if (!bfs())
			break;
		memset(head, 0, sizeof head);
		while (int pushed = dfs(s, INF))
		{
			flow += pushed;
			fill(flow_edges.begin(), flow_edges.end(), false);
			fill(dfs_edges.begin(), dfs_edges.end(), false);
		}
	}
	return flow;
}

int main()
{
	freopen("in.txt", "r", stdin);
	freopen("out.txt", "w", stdout);

	int m;
	cin >> n >> m;
	flow_edges.resize(2 * m);
	dfs_edges.resize(2 * m);
	s = 0;
	t = n - 1;
	for (int i = 0; i < m; i++)
	{
		int a, b, c;
		cin >> a >> b >> c;
		a--;
		b--;
		if (a > b)
		{
			swap(a, b);
		}
		add_edge(a, b, c);
	}
	cout << dinic() << endl;
	for (int i = 0; i < edges.size(); i += 2)
	{
		cout << edges[i].flow << endl;
	}
	return 0;
}
