#include <vector>
#include <algorithm>
#include <iostream>
#include <unordered_set>
#pragma warning(disable : 4996) //_CRT_SECURE_NO_WARNINGS, to use freopen

using namespace std;
const int N = 50000;

struct edge
{
	int id;
	int from;
	int to;
};

int up[N];
int tin[N];
int timer;
vector<int> st;
vector<edge> g[N];
vector<unordered_set<int>> components;
vector<edge> edges;
bool last_component[N];
bool stack_nodes[N];
int n, m;

bool dfs_edge[N];
bool bridges_edge[N];


void new_comp(int size = 0)
{
	components.emplace_back();
	while (st.size() > size)
	{
		last_component[st.back()] = true;
		components.back().insert(st.back());
		stack_nodes[st.back()] = false;
		st.pop_back();
	}
}

void find_bridges(int v, int parent_edge = -1)
{
	if (up[v] != 0)
	{
		return;
	}
	up[v] = tin[v] = ++timer;
	stack_nodes[v] = true;
	st.push_back(v);
	for (const edge& e : g[v])
	{
		if (e.id == parent_edge)
		{
			continue;
		}
		int u = e.to;
		if (tin[u] == 0)
		{
			int size = st.size();
			dfs_edge[e.id] = true;
			find_bridges(u, e.id);
			if (up[u] > tin[v]) {
				bridges_edge[e.id] = true;
				fill(last_component, last_component + n, false);
				new_comp(size);
			}
		}
		up[v] = min(up[v], up[u]);
	}
}

int main()
{
	freopen("in.txt", "r", stdin);
	freopen("out.txt", "w", stdout);
	cin >> n >> m;
	edges.resize(m);
	for (int i = 0; i < m; i++)
	{
		int a, b;
		cin >> a >> b;
		g[a - 1].push_back({ i,  a - 1, b - 1 });
		g[b - 1].push_back({ i , b - 1, a - 1 });
		edges[i] = { i,  a - 1, b - 1 };
	}
	for (int i = 0; i < n; i++)
	{
		if (up[i] == 0)
		{
			find_bridges(i);
			fill(dfs_edge, dfs_edge + m, false);
			fill(last_component, last_component + n, false);
			new_comp();
		}
	}
	for (auto& comp : components)
	{
		for (int v : comp)
		{
			cout << v + 1 << " ";
		}
		cout << endl;
	}
	return 0;
}
