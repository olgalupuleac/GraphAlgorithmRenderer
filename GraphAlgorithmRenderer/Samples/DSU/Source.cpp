#include <iostream>
#include <algorithm>

using namespace std;

const int n = 15;

int p[n];
int r[n];


void init()
{
	for(int i = 0; i < n; i++)
	{
		p[i] = i;
	}
}


int get(int i)
{
	if (p[i] == i)
	{
		return i;
	}
	p[i] = get(p[i]);
	return p[i];
}


void join(int a, int b)
{
	a = get(a);
	b = get(b);
	if (r[a] > r[b])
	{
		swap(a, b);
	}
	if (r[a] == r[b])
	{
		r[b]++;
	}
	p[a] = b;
}


int main()
{
	init();
	join(0, 1);
	join(1, 2);
	join(5, 7);
	join(14, 11);
	join(11, 10);
	join(10, 6);
	join(10, 12);
	join(5, 8);
	join(7, 9);
	join(4, 3);
	join(4, 5);
	join(2, 3);
	join(8, 2);

	for (int i = 0; i < 10; i++)
	{
		cout << i << " " << get(i) << endl;
	}
	return 0;
}