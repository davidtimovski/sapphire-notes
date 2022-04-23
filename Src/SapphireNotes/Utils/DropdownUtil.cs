namespace SapphireNotes.Utils;

public static class DropdownUtil
{
    public static string[] GetOptionsWithFirst<T>(T[] options, string firstOption)
    {
        var result = new string[options.Length + 1];
        result[0] = firstOption;

        for (int i = 0; i < options.Length; i++)
        {
            result[i + 1] = options[i].ToString();
        }

        return result;
    }
}
