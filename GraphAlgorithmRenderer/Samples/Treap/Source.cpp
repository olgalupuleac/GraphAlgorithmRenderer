#include <iostream>
#include <cstdlib>
#include <vector>
#include <algorithm>

using namespace std;

struct item
{
	int key;
	int priority;
	item* left;
	item* right;

	item(int k, int p) : key(k), priority(p), left(nullptr), right(nullptr)
	{
	}
};


void split(item* t, item* & l, item* & r, int key)
{
	if (!t)
	{
		l = r = nullptr;
		return;
	}
	if (key < t->key)
	{
		split(t->left, l, t->left, key);
		r = t;
	}
	else
	{
		split(t->right, t->right, r, key);
		l = t;
	}
}

void merge(item* & t, item* l, item* r)
{
	if (! l || ! r)
	{
		t = l ? l : r;
		return;
	}
	if (l->priority > r->priority)
	{
		merge(l->right, l->right, r);
		t = l;
	}
	else
	{
		merge(r->left, l, r->left);
		t = r;
	}
}


void insert(item* & t, item* it)
{
	if (!t)
	{
		t = it;
		return;
	}
	if (it->priority > t->priority)
	{
		split(t, it->left, it->right, it->key);
		t = it;
	}
	else
	{
		insert(it->key < t->key ? t->left : t->right, it);
	}
}

void erase(item* & t, int key)
{
	if (t->key == key)
	{
		merge(t, t->left, t->right);
	}
	else
	{
		erase(key < t->key ? t->left : t->right, key);
	}
}

item* root = new item(0, 0);

vector<item*> nodes;


int main()
{
	nodes.push_back(root);
	for (int i = 0; i < 10; i++)
	{
		int key = rand() % 100;
		int prior = rand() % 100;
		item* it = new item(key, prior);
		nodes.push_back(it);
		insert(root, it);
	}
	return 0;
}
