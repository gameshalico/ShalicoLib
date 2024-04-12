namespace ShalicoLib.Utility
{
    public static class BitUtility
    {
        public static int CountBits(int n)
        {
            var count = 0;
            while (n != 0)
            {
                n &= n - 1;
                count++;
            }

            return count;
        }
    }
}