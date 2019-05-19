#include <iostream>
#include <vector>
#include <algorithm>
#pragma warning(disable : 4996) //_CRT_SECURE_NO_WARNINGS, to use freopen

using namespace std;

const int N = int(1e5);

struct edge
{
	int id;
	int to;
};

vector<edge> g[N];
int vertex_component[N];
int components_size[N];
bool used_vertexes[N];
bool used_edges[N];

int n, m;
int cur_component;

void dfs(int v)
{
	used_vertexes[v] = true;
	vertex_component[v] = cur_component;
	for (int i = 0; i < g[v].size(); i++)
	{
		if (used_edges[g[v][i].id])
		{
			continue;
		}
		components_size[cur_component]++;
		used_edges[g[v][i].id] = true;
		int u = g[v][i].to;
		if (used_vertexes[u])
		{
			continue;
		}
		dfs(u);
	}
}

int main()
{
	freopen("in.txt", "r", stdin);
	freopen("out.txt", "w", stdout);
	cin >> n >> m;
	for (int i = 0; i < m; i++)
	{
		int a, b;
		cin >> a >> b;
		g[a - 1].push_back({ i, b - 1 });
		// Checking (a != b) to avoid duplication of edges in config.
		// It could be also achieved through
		// following validation expression
		// "__a__ < g[__a__][__x__].to || __a__ == g[__a__][__x__].to
		// && __x__ % 2 == 0"
		// but it seems slightly easier to do it in this way and use 
		// this validation expression: "__a__ <= g[__a__][__x__].to"
		if (a != b)
		{
			g[b - 1].push_back({ i , a - 1 });
		}
	}
	for (int i = 0; i < n; i++)
	{
		if (!used_vertexes[i])
		{
			cur_component++;
			dfs(i);
		}
	}
	cout << *max_element(components_size, components_size + cur_component) << endl;
	return 0;
}