namespace Votyra.Core.Utils
{
    public static class IntUtils
    {
        public static int DivideUp(this int a, int b)
        {
            var res = a / b;
            if (a % b != 0)
                res++;
            return res;
        }
    }
}