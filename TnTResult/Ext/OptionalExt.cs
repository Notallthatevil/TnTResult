namespace TnTResult.Ext;

public static class OptionalExt {

    public static async ValueTask<Optional<OptType>> AndThenAsync<OptType>(this ValueTask<Optional<OptType>> optionalTask, Action<OptType> action) {
        return (await optionalTask).AndThen(action);
    }

    public static async ValueTask<Optional<OptType>> AndThenAsync<OptType>(this Task<Optional<OptType>> optionalTask, Action<OptType> action) {
        return (await optionalTask).AndThen(action);
    }

    public static Optional<OptType> MakeOptional<OptType>(this OptType obj) => Optional<OptType>.MakeOptional(obj);

    public static async ValueTask<Optional<OptType>> OrElseAsync<OptType>(this Task<Optional<OptType>> optionalTask, Action action) {
        return (await optionalTask).OrElse(action);
    }

    public static async ValueTask<Optional<OptType>> OrElseAsync<OptType>(this ValueTask<Optional<OptType>> optionalTask, Action action) {
        return (await optionalTask).OrElse(action);
    }
}