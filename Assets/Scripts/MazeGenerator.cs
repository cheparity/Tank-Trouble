using UnityEngine;
using System.Collections.Generic;
using System;

public class MazeGenerator : MonoBehaviour
{
    // public GameObject fatherMap;
    public GameObject tile;
    public GameObject wall;
    private float tileScale = 0.8f;
    // public float wallLength;
    private int row = 10; //10行
    private int column; //在8-16之间随机
    private float offset; //墙相对于地板的偏移量


    // Start is called before the first frame update
    void Start()
    {
        wall.transform.localScale = new Vector3(tileScale + 0.1f, 0.1f);
        generateWalls();
    }

    void initMapLocation()
    {
        column = new System.Random().Next(8, 17);
        // Debug.Log("random colunm is: " + column);
        tile.transform.localScale.Set(tileScale, tileScale, 1); //设置瓷砖大小
        offset = tileScale / 2; //墙壁偏移量是一半瓷砖大小
        var x = -((column - 3) * tileScale) / 2;
        // Debug.Log("x is: " + x);
        gameObject.transform.position = new Vector3((float)x, -4f, 0);
        // Debug.Log("map: " + column + "*" + row);
    }

    void generateWalls()
    {
        initMapLocation();

        //生成地图边界
        Vector3 middleVector = new Vector3((column - 1) * offset, (row - 1) * offset) + transform.position;
        Vector3 upVector = new Vector3(0, row) * offset + middleVector;
        Vector3 downVector = new Vector3(0, -row) * offset + middleVector;
        Vector3 leftVector = new Vector3(-column, 0) * offset + middleVector;
        Vector3 rightVector = new Vector3(column, 0) * offset + middleVector;

        Instantiate(tile, middleVector, Quaternion.identity, this.transform).transform.localScale = new Vector3(column, row) * tileScale;
        Instantiate(wall, upVector, Quaternion.identity, this.transform).transform.localScale = new Vector3(column * tileScale + 0.1f, 0.1f * tileScale); //上边界
        Instantiate(wall, downVector, Quaternion.identity, this.transform).transform.localScale = new Vector3(column * tileScale + 0.1f, 0.1f * tileScale); //下边界
        Instantiate(wall, leftVector, Quaternion.Euler(new Vector3(0, 0, 90)), this.transform).transform.localScale = new Vector3(row * tileScale + 0.1f, 0.1f * tileScale); //左边界
        Instantiate(wall, rightVector, Quaternion.Euler(new Vector3(0, 0, 90)), this.transform).transform.localScale = new Vector3(row * tileScale + 0.1f, 0.1f * tileScale); //右边界

        //Eller算法生成迷宫
        Eller_Algorithm();
    }

    void Eller_Algorithm()
    {
        DisjointSet set = new DisjointSet(row, column);
        var rot = Quaternion.Euler(0, 0, 90);
        for (int n = 0; n < row; ++n)
        {
            //每次取第n行生成迷宫,总共row行
            for (int m = 0; m < column; ++m)
            {
                //(m,n)
                //每次查找是否与右边的格子合并
                bool mer = randBool();
                if (mer && m + 1 < column)
                {
                    set.merge(new Tuple<int, int>(m, n), new Tuple<int, int>(m + 1, n));
                }
            }
            List<int> sameGridList = new List<int>();
            for (int m = 0; m < column; ++m)
            {
                //如果左右两个格子之间不属于同一集合,则创造墙
                if (m + 1 < column)
                {
                    if (!set.isInSameSet(new Tuple<int, int>(m, n), new Tuple<int, int>(m + 1, n)))
                    {
                        //如果不在一个格子
                        Instantiate(wall, transform.position + new Vector3(m * tileScale + offset, n * tileScale), rot, this.transform);
                        if (sameGridList.Count > 0 && n + 1 < row)
                        {
                            //先随机取出一个格子不造墙,再把剩下的格子随机造上墙
                            int ex = new System.Random().Next(sameGridList.Count);
                            sameGridList.Remove(ex);
                            set.merge(new Tuple<int, int>(ex, n), new Tuple<int, int>(ex, n + 1));
                            foreach (var grid in sameGridList)
                            {
                                if (randBool())//向上造墙向上
                                    Instantiate(wall, transform.position + new Vector3(grid * tileScale, offset + n * tileScale), Quaternion.identity, this.transform);
                                else //向上合并
                                    set.merge(new Tuple<int, int>(grid, n), new Tuple<int, int>(grid, n + 1));
                            }
                        }
                    }
                    else sameGridList.Add(m);//如果是同一集合的格子,则记录;当发现不是同一集合的格子时,随机生成向上的墙
                }
            }
            //这一行遍历结束了,收尾(可能会出现直到最后一个格子都是属于同一个集合的情况)
            if (sameGridList.Count > 0 && n + 1 < row)
            {
                //先随机取出一个格子不造墙,再把剩下的格子随机造上墙
                int ex = new System.Random().Next(sameGridList.Count);
                sameGridList.Remove(ex);
                set.merge(new Tuple<int, int>(ex, n), new Tuple<int, int>(ex, n + 1));
                foreach (var grid in sameGridList)
                {
                    if (randBool())//向上造墙向上
                        Instantiate(wall, transform.position + new Vector3(grid * tileScale, offset + n * tileScale), Quaternion.identity, this.transform);
                    else //向上合并
                        set.merge(new Tuple<int, int>(grid, n), new Tuple<int, int>(grid, n + 1));
                }
            }
        }
    }
    bool randBool()
    {
        return new System.Random().NextDouble() < 0.5;
    }

}
internal class DisjointSet
{
    int[] fa;
    private int rowNum;
    internal DisjointSet(int row, int col)
    {
        rowNum = row;
        fa = new int[row * col];
        for (int i = 0; i < row * col; ++i) fa[i] = i;
    }

    internal int find(int x)
    {
        if (x == fa[x])
            return x;
        else
        {
            fa[x] = find(fa[x]);  //父节点设为根节点
            return fa[x];         //返回父节点
        }
    }

    internal void merge(int i, int j)
    {
        fa[find(i)] = find(j);
    }

    internal void merge(Tuple<int, int> a, Tuple<int, int> b)
    {
        var i = a.Item1 * rowNum + a.Item2;
        var j = b.Item1 * rowNum + b.Item2;
        merge(i, j);
    }

    internal bool isInSameSet(int i, int j)
    {
        return find(i) == find(j);
    }

    internal bool isInSameSet(Tuple<int, int> a, Tuple<int, int> b)
    {
        var i = a.Item1 * rowNum + a.Item2;
        var j = b.Item1 * rowNum + b.Item2;
        return isInSameSet(i, j);
    }
}