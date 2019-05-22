#include <vector>
#include <algorithm>
#include <iostream>
#pragma warning(disable : 4996) //_CRT_SECURE_NO_WARNINGS, to use freopen
using namespace std;

struct edge
{
	int from;
	int to;
	int cost;
};

int n, m, v;
vector<edge> edges;
const int INF = int(1e9);
vector<int> d;

void solve()
{
	d[v] = 0;
	while(true)
	{
		bool any = true;
		for (int j = 0; j < m; j++)
		{
			if (d[edges[j].from] < INF)
			{
				if (d[edges[j].to] > d[edges[j].from] + edges[j].cost)
				{
					d[edges[j].to] = d[edges[j].from] + edges[j].cost;
					any = true;
				}
			}
		}
		if (!any)
		{
			break;
		}
	}
}

int main()
{
	freopen("in.txt", "r", stdin);
	freopen("out.txt", "w", stdout);
	cin >> n >> m >> v;
	v--;
	for (int i = 0; i < m; i++)
	{
		int a, b, c;
		cin >> a >> b >> c;
		edges.push_back({ a - 1, b - 1, c });
	}
	d.resize(n, INF);
	solve();
	return 0;
}