# Graph algorithm visualization for debugging competitive programming tasks

## Table of Content
* [Introduction/Graph Algorithm Renderer](#introductiongraph-algorithm-renderer)
* [Getting started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Rendering DFS](#rendering-dfs)
  - [Other samples](#other-samples)
* [Glossary](#glossary)
  - [Graph config](#graph-config)
  - [Node family](#node-family)
  - [Identifier](#identifier)
  - [Expression](#expression)
  - [Validation template](#validation-template)
  - [Edge family](#edge-family)
  - [Conditional properties](#conditional-properties)
  - [Condition](#condition)
  - [Properties](#properties)
* [Step-by-step DFS](#step-by-step-dfs)
* [Tips & tricks](#tips--tricks)
## Introduction/Graph Algorithm Renderer

This Visual Studio extension was developed as an additional debugging tool for graph algorithms in competitive programming problems. It takes a description of a graph config from the user and renders a graph. The graph is redrawn every time when the debugger's context is changed. 

## Getting started

### Prerequisites

You will need Visual Studio 2017 (the tool was tested on Visual Studio Community 2017 15.9.5) and Visual C++ installed (the extension should work on .NET projects as well, but it was designed for and tested on C++ projects).

### Installation

To install the extension, download the VSIX file from the latest release here <https://github.com/olgalupuleac/GraphAlgorithmRenderer/releases>. Click on the downloaded file and follow the instructions. To open a graph visualization setting window select *View > Other windows > Graph visualization.*

To remove the extension, select *Tools > Extensions and updates > Graph Visualization* and click uninstall.

To update the extension, uninstall it first and then install a new version.

### Rendering DFS

1. Install the extension.
2. Open Visual Studio and create empty C++ project.
3. Add *Source.cpp* file. Copy-paste [the code](<GraphAlgorithmRenderer/Samples/Components/Source.cpp>)
4. Add *in.txt* file. Copy-paste [the input](<GraphAlgorithmRenderer/Samples/Components/in.txt>)
5. Select *View > Other windows > Graph Visualization* 
6. Open tab with JSON.
7. Copy-paste the the [JSON config](<GraphAlgorithmRenderer/Samples/Components/config.json>) in the text box, click *Deserialize*.
8. Set a breakpoint at [line 71](<GraphAlgorithmRenderer/Samples/Components/Source.cpp#L71>).
9. Start debugging.
10. The window with the graph appears.
11. Step into dfs.
12. The window with the graph updates automatically.

### Other samples

To explore other samples, look at the <GraphAlgorithmRenderer/Samples>. Each folder contains a C++ code of some algorithm, JSON config, and screenshots of the resulting graph. To try it out, copy-paste C++ code and add the input file to your project if it is required, then deserialize the config, and start debugging. 

## Glossary

![](readme-images/GraphAlgorithmRenderer_%20families.jpg)

Here is the list of the project's concepts:

#### Graph config 

Defines how to transform your C++ code into a graph. Contains lists of *node families* and *edge families*.
#### Node family

A set of nodes with the same properties. Usually, we have only one node family and edge family in our config. But we will need two node families if in the problem with the bipartite graph the first and the second sets of nodes are stored separately. Each family has a name. Contains *identifier*, *validation template* and *conditional properties*.
#### Identifier

Each node and edge in the graph belong to a *node/edge family*. Each element in the family can be identified by a named tuple of integers or *identifier*. Each index in the tuple has a range of possible values described by *begin template* and *end template*.  Begin template and end template are *expressions* and might contain previous indices. To refer to a certain index in any expression, use `__index_name__` (e.g. `__v__`). For node family, we usually need an identifier with one index. For edge family, we often need to indices, for example, if the graph is stored as an adjacency list `vector<int> g[N]`, the first index defines the vertex `v` and the second defines an index in `g[v]`.
#### Expression

A valid C++ expression with special placeholders for identifier indices (`__index_name__`). There can be also a placeholder for a name of the current function (`__CURRENT_FUNCTION__`) and placeholders for the function arguments (`__ARG1__`, `__ARG2__` and so on). After substitution, the expression will be evaluated using the debugger. Note that functions and class methods are not supported from Standard Template Library. If the expression is not valid, the error is written to log and the result is ignored, except for *begin template* and *end template*.
Example: `p[g[__v__][__i__]] == __v__ || __ARG1__ == 0`

#### Validation template

*Expression* which can be cast to bool. Filters the identifiers, if we don't need all of them. For example, if there is one index named `v` and we want to keep only even values, the validation expression would be `__v__ % 2 == 0`.

#### Edge family 

A set of edges. Almost identical to *node family*. The difference is that the edge should contain a definition of target and source nodes. As we can have several node families, we need to choose which families target and source nodes belong to. (Note that they can belong to different families.) After choosing the family, we need to define how we will get the identifier of the corresponding node, so we specify the *expression* for every index in the node *identifier*. The expression may contain indices of the edge family.

#### Conditional properties

List of *conditions* with *properties*. Each condition may have multiple properties of different types. If a condition is fulfilled, its properties are applied. Conditions with the less index in the list have higher priority.

#### Condition

Contains *condition expression* (an *expression* which can be cast to bool), *function regex* (a regular expression which should much a function name in a stack frame), and a mode.
* *CurrentStackfame* means that the condition is fulfilled if the *condition expression* is true and function regex matches current function name.
* *AllStackframes* means that the condition is fulfilled if there is a stack frame in the call stack there *condition expression* is true and function regex matches current function name. (Note that this option works rather slowly).
* *AllStackframes (args only)* means that the condition is fulfilled if there is a stack frame in the call stack there function regex matches current function name and the *condition expression*, after substitution of function arguments (i.e. `__ARG1__`, `__ARG2__` placeholders, not named arguments) in **that** stack frame is true in the **current** stack frame. For example, we can use this mode if we want to highlight all DFS nodes in the call stack. This option works faster than the previous ones, as it doesn't require changing the stack frame to evaluate the expression.
#### Properties

We have the following types of properties:

* *Label property* defines the edge or node label. A label is a text with additional placeholders `{}` for *expressions*. For example, `cap={edges[__e__].cap}, flow={edges[__e__].flow}`. We can also set the font size.
* *Style property* defines a line style, e. g. dashed. For nodes, it is applied to its border.
* *Line width property* defines a line width. For nodes, it is applied to its border.
* *Line color property* defines a line color. For nodes, it is applied to its border.
* *Fill color property* (nodes only) defines the node fill color.
* *Shape property* (nodes only) defines a shape of the node.
* *Orientation property* (edges only) defines if an arrow at target node and an arrow at source node should be rendered.
## Step-by-step DFS

Let's consider a simple problem <https://www.hackerearth.com/ru/practice/algorithms/graphs/depth-first-search/practice-problems/algorithm/monk-and-graph-problem/>

The code of the possible solution is provided below. We will use a depth-first search to find a component which every vertex belongs to. We also count the number of edges for each component, skipping edges which have been visited.

```c++
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
#ifdef _DEBUG
	freopen("in.txt", "r", stdin);
	freopen("out.txt", "w", stdout);
#endif
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
	fill(vertex_component, vertex_component + n, -1);
	for (int i = 0; i < n; i++)
	{
		if (!used_vertexes[i])
		{
			dfs(i);
			cur_component++;
		}
	}
	cout << *max_element(components_size, components_size + cur_component) << endl;
	return 0;
}
```


Now, let's visualize this code. Assume we have the following input:

~~~~
10 13
1 10
1 2
4 2
3 4
3 4
2 4
2 2
2 3
5 9
9 8
7 8
7 6
5 6
~~~~

Now we will create a config. We will have one node family with the index `v`  with values from `0` to `n`. After that, we will create an edge family with two indices corresponding to `g[][]`: 
`a` from `0` to `n` and `x` from `0` to `g[a].size()`.  Then, we will define the edge source as `a` and edge target as `g[a][x].to`.

1. First, we want to specify the nodes. We can have several families of nodes and edges. For this particular problem, we will only need one node family and one edge family. To add a new node family click *Add* under the list with nodes.![start](readme-images/start.png)

 

2. The window with node family settings opens automatically. The default family names are *node#0, node#1* and so on, but you can specify your own.

![1558348363365](readme-images/node_family_empty.png)

3. In our example, we have one node family with the name *node#0* and one index in its identifier named *v*. Begin template is `0` and end template is `n`, which equals 10, so the nodes will be `node#0 v 0, node#0 v 1, ..., node#0 v 9`.

![1558285655171](readme-images/1558285655171.png)

4. To access the window again after closing it double click its name in the list. ![1558348643653](readme-images/main_window.png)
5. Now let's take a look at the edge family config. First, we will set indices. There is an edge between `a` and `b` if there is `x` such that `g[a][x].to == b`.
   So, our indices will be `a` and `x`. Note that we use a previous index to define a range of `x`.![1558348823781](readme-images/edge_family_window_indices.png)
6. After choosing the family (we have only one option here), we need to set the target and source indices.![1558348965665](readme-images/choose_target_and_source.png)
7. Click *Set source indices*. The following window appear. In our example, the source node corresponds to the first edge index. 

   ![1558362644109](readme-images/source.png)
   
   And the target corresponds to `g[a][x].to`.

   ![1558362581186](readme-images/target.png)



8. Finally, to avoid duplication of edges, we will specify the validation expression. We will keep only those edges, where the source is less than or equals the target. 

![1558285463758](readme-images/1558285463758.png)



9. Now, let's generate our config and see how it looks like. Click *Generate config*.

![1558362786821](readme-images/created_config.png)As we can see, the graph is rendered correctly, but the node labels may seem confusing. To avoid it, let's add conditional properties to our config.

![1558285779613](readme-images/1558285779613.png)

10. To add a conditional property, click *Add* under the list with conditional properties.![1558362922633](readme-images/add_conditional_property.png)
11.  The default label will contain node id. ![1558363055779](readme-images/default_label.png)
12. The short description of the condition appears in the list. To access the window with the conditional property, double click on the description. ![1558285130507](readme-images/1558285130507.png)
13. In the main window, click *Generate config*. After adding a label to nodes, our picture has changed. ![1558363398476](readme-images/default_label_graph.png)
14. Now let's add other node properties. First, we want to see which component a node belongs to a current number of edges in this component. ![1558363747957](readme-images/label.png)
15. This property should have higher priority than the default label. Select a new property and click *Move up*. ![1558363908070](readme-images/move_up.png)
16. Let's highlight the current DFS node... ![1558285025190](readme-images/1558285025190.png)....DFS nodes in the stack...![1558364238009](readme-images/stack_node.png)...and all visited nodes.![1558364398056](readme-images/visited_nodes.png)
17. Now we have the following node properties.![1558364631346](C:\Users\olga\source\repos\GraphAlgorithmRenderer\readme-images\node_properties.png)
18. Edges, visited by DFS...![1558284915478](readme-images/1558284915478.png)
19. Current edge... ![1558284941427](readme-images/1558284941427.png)
20. Now we can the colorized graph! ![1558365514928](C:\Users\olga\source\repos\GraphAlgorithmRenderer\readme-images\colorized_graph.png)
21. Finally, we can serialize the generated config in JSON, save it somewhere, and deserialize it next time to avoid creating this config from the beginning. 

![1558284854490](readme-images/1558284854490.png)

[Here](<GraphAlgorithmRenderer/Samples/Components/config.json>)  is the generated config for this problem.

## Tips & tricks

1. Serialize the config and save it somewhere. Config is discarded after exiting the Visual Studio.
2. Do not forget to click *Generate config* after changing the config settings.
3. Remember that mode *AllStackframes* significantly increase the execution time. It seems more efficient to have a bool array which indicates if the property should be applied.
4. You can use custom functions in the expressions. Note that it works slower than accessing `std::vector` elements.
5. Keep in mind that it takes a second to process 100-200 expressions.
6. The Standard Template Library functions and class methods are not supported in the expressions (`operator[]` being notable exception). It means that you cannot render elements in `std::unordered_set` or use `std::find`. Try to use `std::vector` or arrays instead. You can use custom functions, but 
7. If the begin template, end template or edge source or target cannot be identified, the message box with the error will appear. All other invalid expressions are written to log and ignored by default. To access the log open the *Output window* and set *Show output from* to *Graph Visualization*.
   ![1558294468910](readme-images/1558294468910.png)
