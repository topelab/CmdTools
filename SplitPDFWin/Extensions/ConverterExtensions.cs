namespace SplitPDFWin.Extensions
{
    public static class ConverterExtensions
    {
        public static string ToHumanTime(this double x)
        {
            var horas = (int)x;
            var minutos = (x - horas) * 60;
            return $"{horas}h {minutos:00}m";
        }

    }
}
