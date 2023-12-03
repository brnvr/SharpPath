using System;
using System.Collections.Generic;

namespace SharpPath
{
    public class Path : List<Node>
    {
        public List<int> ToIntList()
        {
            List<int> list;

            list = new List<int>();

            foreach (Node node in this)
            {
                list.Add(node.X);
                list.Add(node.Y);
            }

            return list;
        }

        public byte[] ToByteArray()
        {
            List<int> intList;
            byte[] byteArr;

            intList = ToIntList();
            byteArr = new byte[intList.Count * 4];

            for (int i = 0; i < intList.Count; i++)
            {
                BitConverter.GetBytes(intList[i]).CopyTo(byteArr, i * 4);
            }

            return byteArr;
        }
    }
}
