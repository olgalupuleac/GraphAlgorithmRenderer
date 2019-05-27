#include <algorithm>

int n;
const int N = 10000;
int a[N];

int get_min()
{
	return a[1];
}

void sift_up(int i)
{
	while (i > 1 && a[i / 2] > a[i])
	{
		std::swap(a[i], a[i / 2]);
		i /= 2;
	}
}

void sift_down(int i)
{
	while (true)
	{
		int l = 2 * i;
		if (l + 1 <= n && a[l + 1] < a[l])
		{
			l++;
		}
		if (!(l <= n && a[l] < a[i]))
		{
			break;
		}
		std::swap(a[l], a[i]);
		i = l;
	}
}

void add(int x)
{
	a[++n] = x;
	sift_up(n);
}

void del_min() 
{
	std::swap(a[1], a[n--]);
	sift_down(1);
}


int main()
{
	add(10);
	add(3);
	add(1);
	add(5);
	add(4);
	add(2);
	add(8);
	del_min();
	add(0);
	return 0;
}