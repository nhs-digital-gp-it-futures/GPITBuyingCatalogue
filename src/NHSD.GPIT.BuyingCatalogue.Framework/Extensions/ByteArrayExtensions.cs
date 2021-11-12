namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string GetString(this byte[] buffer)
        {
            if (buffer is null || buffer.Length == 0)
                return string.Empty;

            return System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
    }
}
