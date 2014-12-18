/*
    CrossNet - C# Benchmark
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBenchmark._Benchmark
{
    public class BinaryTreesTest
    {
        const int minDepth = 4;

        public static volatile int sCheck;

		public static void Main()
		{
			Console.WriteLine(Test (1000));
		}

        public static bool Test(int N)
        {
            int maxDepth = Math.Max(minDepth + 2, 10);
            int stretchDepth = maxDepth + 1;

            for (int loop = 0; loop < N; ++loop)
            {
                sCheck = TreeNode.bottomUpTree(0, stretchDepth).itemCheck();

                TreeNode topTree = TreeNode.bottomUpTree(0, maxDepth);

                for (int depth = minDepth; depth <= maxDepth; depth += 2)
                {
                    int iterations = 1 << (maxDepth - depth + minDepth);

                    sCheck = 0;
                    for (int i = 1; i <= iterations; i++)
                    {
                        sCheck += TreeNode.bottomUpTree(i, depth).itemCheck();

                        sCheck += TreeNode.bottomUpTree(-i, depth).itemCheck();
                    }

                    //  iterations * 2, depth, check
                }

                //  maxDepth, longLivedTree.itemCheck();

                if (maxDepth == 0)
                {
                    return (false);
                }
                sCheck = topTree.itemCheck();
            }
            return (true);
        }

        class TreeNode
        {
            private TreeNode left, right;
            private int item;

            TreeNode(int item)
            {
                this.item = item;
            }

            internal static TreeNode bottomUpTree(int item, int depth)
            {
                if (depth > 0)
                {
                    return new TreeNode(
                         bottomUpTree(2 * item - 1, depth - 1)
                       , bottomUpTree(2 * item, depth - 1)
                       , item
                       );
                }
                else
                {
                    return new TreeNode(item);
                }
            }

            TreeNode(TreeNode left, TreeNode right, int item)
            {
                this.left = left;
                this.right = right;
                this.item = item;
            }

            internal int itemCheck()
            {
                // if necessary deallocate here
                if (left == null)
                    return item;
                else
                    return item + left.itemCheck() - right.itemCheck();
            }
        }
    }
}
