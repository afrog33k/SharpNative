using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace List
{


    public class Utils
    {
        public static Random random = new Random();
    }

    public class Node
    {
        public Node next;
        public int value;

        public Node()
        {
            double f = Utils.random.NextDouble();
            this.value = (int)(f * 1000.0);
        }
    }

    class MyList
    {
        Node first;
        int length;

        public MyList()
        {
            first = null;
            length = 0;
        }

        public int Length()
        {
            return length;
        }

        public void Add(Node n)
        {
            n.next = first;
            first = n;
            length++;
        }

        public Node getElementAt(int index)
        {
            if (index >= length)
                return null;

            Node n = first;
            for (int i = 0; i < index; i++)
                n = n.next;

            return n;
        }

        public void BubbleSort()
        {
            bool sorted = false;

            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < length - 1; i++)
                {
                    Node n1 = getElementAt(i);
                    Node n2 = getElementAt(i + 1);

                    if (n1.value > n2.value)
                    {
                        Swap(i, i + 1);
                        sorted = false;
                    }
                }
            }
        }

        private void Swap(int pos1, int pos2)
        {
            Node n1 = getElementAt(pos1);
            Node n2 = getElementAt(pos2);

            n1.next = n2.next;
            n2.next = n1;

            if (pos1 > 0)
            {
                Node nant = getElementAt(pos1 - 1);
                nant.next = n2;
            }

            if (pos2 == 1 && pos1 == 0)
                first = n2;
        }


    }


    class Program
    {
        MyList list;

        static void Main()
        {
            Program p = new Program();
            p.Run();
            p.printList();
            p.Sort();
            p.printList();
        }

        void Run()
        {
            list = new MyList();

            for (int i = 0; i < 100; i++)
                list.Add(new Node());
        }

        void Sort()
        {
            list.BubbleSort();
        }

        void printList()
        {
            Console.WriteLine("****************");
            for (int i = 0; i < list.Length(); i++)
            {
                Console.Write("Node ");
                Console.Write(i);
                Console.Write(": ");
                Console.WriteLine(list.getElementAt(i).value);
            }
        }


    }
}
