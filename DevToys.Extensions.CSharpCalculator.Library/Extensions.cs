namespace DevToys.Extensions.CSharpCalculator.Library;

public static class Extensions
{

    public static T Out<T>(this T value, Action<T> action)
    {
        action?.Invoke(value);
        return value;
    }

    public static T Out<T, TArg1>(this T value, Action<T, TArg1> action, TArg1 arg1)
    {
        action?.Invoke(value, arg1);
        return value;
    }

    public static T Out<T>(this T value, Action<T, Func<T, string>> action, Func<T, string> format)
    {
        action?.Invoke(value, format);
        return value;
    }

    public static T Out<T, TArg1, TArg2>(this T value, Action<T, TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
    {
        action?.Invoke(value, arg1, arg2);
        return value;
    }

    public static T Out<T, TArg1, TArg2, TArg3>(this T value, Action<T, TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        action?.Invoke(value, arg1, arg2, arg3);
        return value;
    }

    public static IEnumerable<T> Trace<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action?.Invoke(item);
            yield return item;
        }
    }
    public static IEnumerable<T> OutAll<T>(this IEnumerable<T> source,
        Action<IEnumerable<T>> action)
    {
        action?.Invoke(source);
        return source;
    }

    public static IEnumerable<T> OutAll<T, TArg1>(this IEnumerable<T> source,
        Action<IEnumerable<T>, TArg1> action, TArg1 arg1)
    {
        action?.Invoke(source, arg1);
        return source;
    }

    public static IEnumerable<T> OutAll<T, TArg1, TArg2>(this IEnumerable<T> source,
        Action<IEnumerable<T>, TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
    {
        action?.Invoke(source, arg1, arg2);
        return source;
    }

    public static IEnumerable<T> OutAll<T, TArg1>(this IEnumerable<T> source,
        Action<IEnumerable<T>, TArg1, Func<T, string>> action, TArg1 arg1, Func<T, string> format)
    {
        action?.Invoke(source, arg1, format);
        return source;
    }

    public static IEnumerable<T> OutAll<T, TArg1, TArg2, TArg3>(this IEnumerable<T> source,
        Action<IEnumerable<T>, TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        action?.Invoke(source, arg1, arg2, arg3);
        return source;
    }



    public static IEnumerable<T> Trace<T, TArg1>(this IEnumerable<T> source, Action<T, TArg1> action, TArg1 arg1)
    {
        foreach (var item in source)
        {
            action?.Invoke(item, arg1);
            yield return item;
        }
    }

    public static IEnumerable<T> Trace<T>(this IEnumerable<T> source, Action<T, Func<T, string>> action, Func<T, string> format)
    {
        foreach (var item in source)
        {
            action?.Invoke(item, format);
            yield return item;
        }
    }


    public static IEnumerable<T> Trace<T, TArg1, TArg2>(this IEnumerable<T> source, Action<T, TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
    {
        foreach (var item in source)
        {
            action?.Invoke(item, arg1, arg2);
            yield return item;
        }
    }

    public static IEnumerable<T> Trace<T, TArg1, TArg2, TArg3>(this IEnumerable<T> source, Action<T, TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        foreach (var item in source)
        {
            action?.Invoke(item, arg1, arg2, arg3);
            yield return item;
        }
    }
}
