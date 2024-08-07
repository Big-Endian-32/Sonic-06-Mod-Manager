namespace SonicNextModManager.Helpers
{
    public class MoonSharpHelper
    {
        /// <summary>
        /// Transforms a string to a DynValue where possible.
        /// </summary>
        /// <param name="in_value">The string containing the value to transform.</param>
        public static DynValue TransformStringToDynValue(string in_value)
        {
            if (bool.TryParse(in_value, out var out_bool))
            {
                return DynValue.NewBoolean(out_bool);
            }
            else if (int.TryParse(in_value, out var out_int))
            {
                return DynValue.NewNumber(out_int);
            }
            else if (double.TryParse(in_value, out var out_double))
            {
                return DynValue.NewNumber(out_double);
            }

            return DynValue.NewString(in_value);
        }
    }
}
