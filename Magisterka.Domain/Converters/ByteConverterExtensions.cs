using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Converters
{
    public static class ByteConverterExtensions
    {
        public static double ToMegaBytes(this long bytes)
        {
            var bytesInMegabyte = 1048576.0;
            return bytes / bytesInMegabyte;
        }

        public static long ToBytes(this double megabytes)
        {
            var bytesInMegabyte = 1048576;
            return (long)(megabytes * bytesInMegabyte);
        }
    }
}
