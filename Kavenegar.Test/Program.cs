using System;
using System.Collections.Generic;
using Kavenegar.NetStandard;

namespace Kavenegar.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = new KavenegarApi("{api key}").Send("{فرستنده}",
                new[] {"{گیرنده}"}, "تست ای پی آی").Result;
        }
    }
}