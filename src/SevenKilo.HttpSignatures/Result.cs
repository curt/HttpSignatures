namespace SevenKilo.HttpSignatures;

public class Result
{
    private readonly bool _success;
    private readonly IEnumerable<string> _errors;

    public Result()
    {
        _success = true;
        _errors = [];
    }

    public Result(string error)
    {
        _success = false;
        _errors = [error];
    }

    public Result(IEnumerable<string> errors)
    {
        _success = !errors.Any();
        _errors = errors;
    }

    public IEnumerable<string> Errors { get => _errors; }

    public static bool operator true(Result x) => x._success;

    public static bool operator false(Result x) => !x._success;

    public static Result operator &(Result x, Result y)
    {
        return new(x._errors.Union(y._errors));
    }
}
